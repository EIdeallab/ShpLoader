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

    public class DbfRecord : IRecord
    {
        public string FieldName { get; set; }
        public char FieldType { get; set; }
        public int Address { get; set; }
        public byte FieldLength { get; set; }
        public byte DecimalCount { get; set; }
        public short WorkAreaID { get; set; }
        public byte Example { get; set; }
        public byte[] Reserved { get; set; }
        public byte MDXFieildFlag { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1}", FieldName, FieldType);
        }

        public void Load(ref BinaryReader br)
        {
            FieldName = new string(br.ReadChars(11));
            FieldType = br.ReadChar();
            Address = br.ReadInt32();
            FieldLength = br.ReadByte();
            DecimalCount = br.ReadByte();
            WorkAreaID = br.ReadInt16();
            Example = br.ReadByte();
            Reserved = br.ReadBytes(10);
            MDXFieildFlag = br.ReadByte();
        }

        public long GetLength()
        {
            long size = 32;
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
                { ShapeType.MultiPatch, () => new MultiPatch() },
            };

        public static IRecord CreateInstance(ShapeType shapeType)
        {
            return Creators[shapeType]();
        }
    }


}
