using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assets
{
    public class ShxRecord : IRecord
    {
        public ShpRecordHeader header { get; set; }
        
        public void Load(ref BinaryReader br)
        {
            header.Load(ref br);
        }

        public long GetLength()
        {
            return header.GetLength();
        }
    }

    public class ShpRecord : IRecord
    {
        public ShpRecordHeader header { get; set; }
        public IRecord Contents { get; set; }

        public ShpRecord(ShapeType type)
        {
            header = new ShpRecordHeader();
            Contents = ShapeFactory.CreateInstance(type);
        }

        public void Load(ref BinaryReader br)
        {
            header.Load(ref br);
            Contents.Load(ref br);
        }

        public long GetLength()
        {
            return header.GetLength() + Contents.GetLength();
        }
    }

    public class ShpRecordHeader : IRecord
    {
        public int RecordNumber { get; set; }
        public int ContentLength { get; set; }
        
        public void Load(ref BinaryReader br)
        {
            RecordNumber = Endian.FromBigEndian(br.ReadInt32());
            ContentLength = Endian.FromBigEndian(br.ReadInt32()) * 2;
        }

        public long GetLength()
        {
            return sizeof(int) * 2;
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

    public class DbfRecord : IDBRecord
    {
        public char DeletionMarker { get; set; }
        public IDBRecord Contents { get; set; }

        public DbfRecord(List<DbfFieldDiscriptor> fieldList)
        {
            foreach(DbfFieldDiscriptor fieldDiscriptor in fieldList)
            {
                Contents = DataFactory.CreateInstance(fieldDiscriptor);
            }
        }

        public void Load(ref BinaryReader br, DbfFieldDiscriptor fd)
        {
            DeletionMarker = br.ReadChar();
            Contents.Load(ref br, fd);
        }

        public long GetLength()
        {
            long size = sizeof(char) + Contents.GetLength();
            return size;
        }
    }

    public class ShapeFactory
    {
        public static readonly IDictionary<ShapeType, Func<IRecord>> Creators =
            new Dictionary<ShapeType, Func<IRecord>>()
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

        public static IRecord CreateInstance(ShapeType shapeType)
        {
            return Creators[shapeType]();
        }
    }

    public class DataFactory
    {
        public static readonly IDictionary<DBFFieldType, Func<IDBRecord>> Creators =
            new Dictionary<DBFFieldType, Func<IDBRecord>>()
            {
                { DBFFieldType.Character, () => new DBCharacter() },
                { DBFFieldType.Logical, () => new DBLogical() },
                { DBFFieldType.Numeric, () => new DBNumeric() },
                { DBFFieldType.Float, () => new DBFloat() },
                { DBFFieldType.Date, () => new DBDate() },
                { DBFFieldType.Memo, () => new DBMemo() }
            };

        public static IDBRecord CreateInstance(DbfFieldDiscriptor fieldDiscriptor)
        {
            return Creators[fieldDiscriptor.FieldType]();
        }
    }
}
