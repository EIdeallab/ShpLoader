using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public enum FileType : byte
    {
        Shp,
        Shx,
        Dbf
    }

    public enum ShapeType : byte
    {
        Null = 0,
        Point = 1,
        PolyLine = 3,
        Polygon = 5,
        MultiPoint = 8,
        PointZ = 11,
        PolyLineZ = 13,
        PolygonZ = 15,
        MultiPointZ = 18,
        PointM = 21,
        PolyLineM = 23,
        PolygonM = 25,
        MultiPointM = 28,
        MultiPatch = 31
    }

    public enum PartType : byte
    {
        TriangleStrip,
        TriangleFan,
        OuterRing,
        InnerRing,
        FirstRing,
        Ring
    }

    public enum DBFFieldType : int
    {
        Character = 'C',
        Currency = 'Y',
        Numeric = 'N',
        Float = 'F',
        Date = 'D',
        DateTime = 'T',
        Double = 'B',
        Integer = 'I',
        Logical = 'L',
        Memo = 'M',
        General = 'G',
        Picture = 'P'
    }
    
    public enum DBFVersion : byte
    {
        Unknown = 0,
        FoxBase = 0x02,
        FoxBaseDBase3NoMemo = 0x03,
        VisualFoxPro = 0x30,
        VisualFoxProWithAutoIncrement = 0x31,
        dBase4SQLTableNoMemo = 0x43,
        dBase4SQLSystemNoMemo = 0x63,
        FoxBaseDBase3WithMemo = 0x83,
        dBase4WithMemo = 0x8B,
        dBase4SQLTableWithMemo = 0xCB,
        FoxPro2WithMemo = 0xF5,
        FoxBASE = 0xFB
    }

    [Flags]
    public enum TransactionFlag : byte
    {
        None = 0x00,
        HasStructuralCDX = 0x01,
        HasMemoField = 0x02,
        IsDBC = 0x04
    }
}
