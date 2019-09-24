using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;

namespace Assets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class ShxRecord : IRecord
    {
        public int Offset { get; set; }
        public int Length { get; set; }

        public void Load(ref BinaryReader br)
        {
            Offset = Util.FromBigEndian(br.ReadInt32()) * 2;
            Length = Util.FromBigEndian(br.ReadInt32()) * 2;
        }

        public long GetLength()
        {
            return Marshal.SizeOf(this);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class ShpRecord : IRecord, IRenderableData
    {
        public ShpRecordHeader Header { get; set; }
        public IElement Contents { get; set; }

        public ShpRecord(ShapeType type)
        {
            Header = new ShpRecordHeader();
            Contents = ShapeFactory.CreateInstance(type);
        }

        public void Load(ref BinaryReader br)
        {
            Header.Load(ref br);
            Contents.Load(ref br);
        }

        public long GetLength()
        {
            return Header.GetLength() + Contents.GetLength();
        }

        public void Render(RangeXY range, Color color)
        {
            try
            {
                IRenderableData renderContents = (IRenderableData)Contents;
                renderContents.Render(range, color);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
    public class ShpRecordHeader : IRecord
    {
        public int RecordNumber { get; set; }
        public int ContentLength { get; set; }
        public ShapeType Type { get; set; }

        public void Load(ref BinaryReader br)
        {
            RecordNumber = Util.FromBigEndian(br.ReadInt32());
            ContentLength = Util.FromBigEndian(br.ReadInt32()) * 2;
            Type = (ShapeType)br.ReadInt32();
        }

        public long GetLength()
        {
            return Marshal.SizeOf(this);
        }
    }

    public class DbfFieldDiscriptor : IRecord
    {
        public string FieldName { get; set; }
        public DBFFieldType FieldType { get; set; }
        public int Address { get; set; }
        public byte FieldLength { get; set; }
        public byte DecimalCount { get; set; }
        public byte[] Reserved1 { get; set; }
        public byte WorkArea { get; set; }
        public byte[] Reserved2 { get; set; }
        public byte FieldFlag { get; set; }

        public void Load(ref BinaryReader br)
        {
            FieldName = new string(br.ReadChars(11));
            FieldType = (DBFFieldType)br.ReadChar();
            Address = br.ReadInt32();
            FieldLength = br.ReadByte();
            DecimalCount = br.ReadByte();
            Reserved1 = br.ReadBytes(2);
            WorkArea = br.ReadByte();
            Reserved2 = br.ReadBytes(2);
            FieldFlag = br.ReadByte();
            br.ReadBytes(8);
        }

        public long GetLength()
        {
            long size = 32;
            return size;
        }
    }

    public class DbfRecord : IRecord
    {
        List<DbfFieldDiscriptor> FieldList { get; set; }
        public char DeletionMarker { get; set; }
        public List<IElement> Record { get; set; }

        public DbfRecord(List<DbfFieldDiscriptor> fieldList)
        {
            FieldList = fieldList;
            Record = new List<IElement>();
        }

        public void Load(ref BinaryReader br)
        {
            DeletionMarker = br.ReadChar();
            foreach (DbfFieldDiscriptor fd in FieldList)
            {
                IElement element = DataFactory.CreateInstance(fd);
                element.Load(ref br);
                Record.Add(element);
            }
        }

        public long GetLength()
        {
            long size = sizeof(byte) + FieldList.Sum(field => field.FieldLength);
            return size;
        }
    }

    public class ShapeFactory
    {
        public static readonly IDictionary<ShapeType, Func<IElement>> Creators =
            new Dictionary<ShapeType, Func<IElement>>()
            {
                { ShapeType.Point, () => new Point() },
                { ShapeType.PolyLine, () => new PolyLine() },
                { ShapeType.Polygon, () => new Polygon() },
                { ShapeType.MultiPoint, () => new MultiPoint() },
                { ShapeType.PointM, () => new PointM() },
                { ShapeType.PolyLineM, () => new PolyLineM() },
                { ShapeType.PolygonM, () => new PolygonM() },
                { ShapeType.MultiPointM, () => new MultiPointM() },
                { ShapeType.PointZ, () => new PointZ() },
                { ShapeType.PolyLineZ, () => new PolyLineZ() },
                { ShapeType.PolygonZ, () => new PolygonZ() },
                { ShapeType.MultiPointZ, () => new MultiPointZ() },
                { ShapeType.MultiPatch, () => new MultiPatch() }
            };

        public static IElement CreateInstance(ShapeType shapeType)
        {
            return Creators[shapeType]();
        }
    }

    public class DataFactory
    {
        public static readonly IDictionary<DBFFieldType, Func<DbfFieldDiscriptor, IElement>> Creators =
            new Dictionary<DBFFieldType, Func<DbfFieldDiscriptor, IElement>>()
            {
                { DBFFieldType.Character, (fd) => new DBCharacter(fd) },
                { DBFFieldType.Logical, (fd) => new DBLogical(fd) },
                { DBFFieldType.Numeric, (fd) => new DBNumeric(fd) },
                { DBFFieldType.Float, (fd) => new DBFloat(fd) },
                { DBFFieldType.Date, (fd) => new DBDate(fd) },
                { DBFFieldType.Memo, (fd) => new DBMemo(fd) }
            };

        public static IElement CreateInstance(DbfFieldDiscriptor fd)
        {
            return Creators[fd.FieldType](fd);
        }
    }
}
