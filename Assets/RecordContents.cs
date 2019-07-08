using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assets
{
    public class Range : IRecordContents
    {
        public double Min { get; set; }
        public double Max { get; set; }
        
        public void Load(ref BinaryReader br)
        {
            Min = br.ReadDouble();
            Max = br.ReadDouble();
        }

        public long GetLength()
        {
            long size = 0;
            size += sizeof(double) * 2;
            return size;
        }
    }

    public class RangeXY : IRecordContents
    {
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }

        public void Load(ref BinaryReader br)
        {
            MinX = br.ReadDouble();
            MaxX = br.ReadDouble();
            MinY = br.ReadDouble();
            MaxY = br.ReadDouble();
        }

        public long GetLength()
        {
            long size = 0;
            size += sizeof(double) * 4;
            return size;
        }
    }

    public class Point : IRecordContents
    {
        public double X { get; set; }
        public double Y { get; set; }

        public void Load(ref BinaryReader br)
        {
            X = br.ReadDouble();
            Y = br.ReadDouble();
        }

        public long GetLength()
        {
            long size = 0;
            size += sizeof(double) * 2;
            return size;
        }
    }
    
    public class MultiPoint : IRecordContents
    {
        public RangeXY XYRange { get; set; }
        public int NumPoints { get; set; }
        public Point[] Points { get; set; }

        public void Load(ref BinaryReader br)
        {
            XYRange = new RangeXY();
            XYRange.Load(ref br);
            NumPoints = br.ReadInt32();
            Points = new Point[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(ref br);
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int);
            size += sizeof(double) * NumPoints * 2;
            return size;
        }
    }

    public class PolyLine : IRecordContents
    {
        public RangeXY XYRange { get; set; }
        public int NumParts { get; set; }
        public int NumPoints { get; set; }
        public int[] Parts { get; set; }
        public Point[] Points { get; set; }

        public void Load(ref BinaryReader br)
        {
            XYRange = new RangeXY();
            XYRange.Load(ref br);
            NumParts = br.ReadInt32();
            NumPoints = br.ReadInt32();
            Parts = new int[NumParts];
            Points = new Point[NumPoints];
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(ref br);
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int) * 2;
            size += sizeof(int) * NumParts;
            size += sizeof(double) * NumPoints * 2;
            return size;
        }
    }

    public class Polygon : IRecordContents
    {
        public RangeXY XYRange { get; set; }
        public int NumParts { get; set; }
        public int NumPoints { get; set; }
        public int[] Parts { get; set; }
        public Point[] Points { get; set; }
        
        public void Load(ref BinaryReader br)
        {
            XYRange = new RangeXY();
            XYRange.Load(ref br);
            NumParts = br.ReadInt32();
            NumPoints = br.ReadInt32();
            Parts = new int[NumParts];
            Points = new Point[NumPoints];
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(ref br);
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int) * 2;
            size += sizeof(int) * NumParts;
            size += sizeof(double) * NumPoints * 2;
            return size;
        }
    }

    public class PointM : IRecordContents
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double M { get; set; }

        public void Load(ref BinaryReader br)
        {
            X = br.ReadDouble();
            Y = br.ReadDouble();
            M = br.ReadDouble();
        }

        public long GetLength()
        {
            long size = 0;
            size += sizeof(double) * 3;
            return size;
        }
    }

    public class MultiPointM : IRecordContents
    {
        public RangeXY XYRange { get; set; }
        public int NumPoints { get; set; }
        public Point[] Points { get; set; }
        public Range MRange { get; set; }
        public double[] MValues { get; set; }
        
        public void Load(ref BinaryReader br)
        {
            XYRange = new RangeXY();
            XYRange.Load(ref br);
            NumPoints = br.ReadInt32();
            Points = new Point[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(ref br);
            }
            MRange.Load(ref br);
            for (int i = 0; i < NumPoints; i++)
            {
                MValues[i] = br.ReadDouble();
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int);
            size += sizeof(double) * NumPoints * 2;
            size += MRange.GetLength();
            size += sizeof(double) * NumPoints;
            return size;
        }
    }

    public class PolyLineM : IRecordContents
    {
        public RangeXY XYRange { get; set; }
        public int NumParts { get; set; }
        public int NumPoints { get; set; }
        public int[] Parts { get; set; }
        public Point[] Points { get; set; }
        public Range MRange { get; set; }
        public double[] MValues { get; set; }

        public void Load(ref BinaryReader br)
        {
            XYRange = new RangeXY();
            XYRange.Load(ref br);
            NumParts = br.ReadInt32();
            NumPoints = br.ReadInt32();
            Parts = new int[NumParts];
            Points = new Point[NumPoints];
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(ref br);
            }
            MRange.Load(ref br);
            for (int i = 0; i < NumPoints; i++)
            {
                MValues[i] = br.ReadDouble();
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int) * 2;
            size += sizeof(int) * NumParts;
            size += sizeof(double) * NumPoints * 2;
            size += MRange.GetLength();
            size += sizeof(double) * NumPoints;
            return size;
        }
    }

    public class PolygonM : IRecordContents
    {
        public RangeXY XYRange { get; set; }
        public int NumParts { get; set; }
        public int NumPoints { get; set; }
        public int[] Parts { get; set; }
        public Point[] Points { get; set; }
        public Range MRange { get; set; }
        public double[] MValues { get; set; }

        public void Load(ref BinaryReader br)
        {
            XYRange = new RangeXY();
            XYRange.Load(ref br);
            NumParts = br.ReadInt32();
            NumPoints = br.ReadInt32();
            Parts = new int[NumParts];
            Points = new Point[NumPoints];
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(ref br);
            }
            MRange.Load(ref br);
            for (int i = 0; i < NumPoints; i++)
            {
                MValues[i] = br.ReadDouble();
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int) * 2;
            size += sizeof(int) * NumParts;
            size += sizeof(double) * NumPoints * 2;
            size += MRange.GetLength();
            size += sizeof(double) * NumPoints;
            return size;
        }
    }


    public class PointZ : IRecordContents
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double M { get; set; }

        public void Load(ref BinaryReader br)
        {
            X = br.ReadDouble();
            Y = br.ReadDouble();
            Z = br.ReadDouble();
        }

        public long GetLength()
        {
            long size = 0;
            size += sizeof(double) * 4;
            return size;
        }
    }

    public class MultiPointZ : IRecordContents
    {
        public RangeXY XYRange { get; set; }
        public int NumPoints { get; set; }
        public PointZ[] Points { get; set; }
        public Range ZRange { get; set; }
        public double[] ZValues { get; set; }
        public Range MRange { get; set; }
        public double[] MValues { get; set; }

        public void Load(ref BinaryReader br)
        {
            XYRange = new RangeXY();
            XYRange.Load(ref br);
            NumPoints = br.ReadInt32();
            Points = new PointZ[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(ref br);
            }
            ZRange.Load(ref br);
            for (int i = 0; i < NumPoints; i++)
            {
                ZValues[i] = br.ReadDouble();
            }
            MRange.Load(ref br);
            for (int i = 0; i < NumPoints; i++)
            {
                MValues[i] = br.ReadDouble();
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int);
            size += sizeof(double) * NumPoints * 2;
            size += MRange.GetLength();
            size += sizeof(double) * NumPoints;
            size += ZRange.GetLength();
            size += sizeof(double) * NumPoints;
            return size;
        }
    }

    public class PolyLineZ : IRecordContents
    {
        public RangeXY XYRange { get; set; }
        public int NumParts { get; set; }
        public int NumPoints { get; set; }
        public int[] Parts { get; set; }
        public Point[] Points { get; set; }
        public Range ZRange { get; set; }
        public double[] ZValues { get; set; }
        public Range MRange { get; set; }
        public double[] MValues { get; set; }

        public void Load(ref BinaryReader br)
        {
            XYRange = new RangeXY();
            XYRange.Load(ref br);
            NumParts = br.ReadInt32();
            NumPoints = br.ReadInt32();
            Parts = new int[NumParts];
            Points = new Point[NumPoints];
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(ref br);
            }
            ZRange.Load(ref br);
            for (int i = 0; i < NumPoints; i++)
            {
                ZValues[i] = br.ReadDouble();
            }
            MRange.Load(ref br);
            for (int i = 0; i < NumPoints; i++)
            {
                MValues[i] = br.ReadDouble();
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int);
            size += sizeof(double) * NumPoints * 2;
            size += MRange.GetLength();
            size += sizeof(double) * NumPoints;
            size += ZRange.GetLength();
            size += sizeof(double) * NumPoints;
            return size;
        }
    }

    public class PolygonZ : IRecordContents
    {
        public RangeXY XYRange { get; set; }
        public int NumParts { get; set; }
        public int NumPoints { get; set; }
        public int[] Parts { get; set; }
        public Point[] Points { get; set; }
        public Range ZRange { get; set; }
        public double[] ZValues { get; set; }
        public Range MRange { get; set; }
        public double[] MValues { get; set; }

        public void Load(ref BinaryReader br)
        {
            XYRange = new RangeXY();
            XYRange.Load(ref br);
            NumParts = br.ReadInt32();
            NumPoints = br.ReadInt32();
            Parts = new int[NumParts];
            Points = new Point[NumPoints];
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(ref br);
            }
            ZRange.Load(ref br);
            for (int i = 0; i < NumPoints; i++)
            {
                ZValues[i] = br.ReadDouble();
            }
            MRange.Load(ref br);
            for (int i = 0; i < NumPoints; i++)
            {
                MValues[i] = br.ReadDouble();
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int) * 2;
            size += sizeof(int) * Parts.Length;
            size += sizeof(double) * NumPoints * 2;
            size += MRange.GetLength();
            size += sizeof(double) * NumPoints;
            size += ZRange.GetLength();
            size += sizeof(double) * NumPoints;
            return size;
        }
    }

    public class MultiPatch : IRecordContents
    {
        public RangeXY XYRange { get; set; }
        public int NumParts { get; set; }
        public int NumPoints { get; set; }
        public int[] Parts { get; set; }
        public int[] PartsTypes { get; set; }
        public Point[] Points { get; set; }
        public Range ZRange { get; set; }
        public double[] ZValues { get; set; }
        public Range MRange { get; set; }
        public double[] MValues { get; set; }

        public void Load(ref BinaryReader br)
        {
            XYRange = new RangeXY();
            XYRange.Load(ref br);
            NumParts = br.ReadInt32();
            NumPoints = br.ReadInt32();
            Parts = new int[NumParts];
            Points = new Point[NumPoints];
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }
            for (int i = 0; i < NumParts; i++)
            {
                PartsTypes[i] = br.ReadInt32();
            }
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(ref br);
            }
            ZRange.Load(ref br);
            for (int i = 0; i < NumPoints; i++)
            {
                ZValues[i] = br.ReadDouble();
            }
            MRange.Load(ref br);
            for (int i = 0; i < NumPoints; i++)
            {
                MValues[i] = br.ReadDouble();
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int) * 2;
            size += sizeof(int) * NumParts;
            size += sizeof(int) * NumParts;
            size += sizeof(double) * NumPoints * 2;
            size += MRange.GetLength();
            size += sizeof(double) * NumPoints;
            size += ZRange.GetLength();
            size += sizeof(double) * NumPoints;
            return size;
        }
    }
}
