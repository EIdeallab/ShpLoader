using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

namespace Assets
{
    public class DBCharacter : IElement
    {
        public DbfFieldDiscriptor discriptor { get; set; }
        public string Value { get; set; }

        public DBCharacter(DbfFieldDiscriptor fd)
        {
            discriptor = fd;
        }

        public long GetLength()
        {
            return discriptor.FieldLength;
        }

        public void Load(ref BinaryReader br)
        {
            char[] rawData = br.ReadChars(discriptor.FieldLength);
            Value = new string(rawData);
        }
    }

    public class DBDate : IElement
    {
        public DbfFieldDiscriptor discriptor { get; set; }
        public DateTime Value { get; set; }

        public DBDate(DbfFieldDiscriptor fd)
        {
            discriptor = fd;
        }

        public long GetLength()
        {
            return discriptor.FieldLength;
        }

        public void Load(ref BinaryReader br)
        {
            char[] rawData = br.ReadChars(discriptor.FieldLength);
            Value = Convert.ToDateTime(rawData);
        }
    }

    public class DBFloat : IElement
    {
        public DbfFieldDiscriptor discriptor { get; set; }
        public float Value { get; set; }

        public DBFloat(DbfFieldDiscriptor fd)
        {
            discriptor = fd;
        }

        public long GetLength()
        {
            return discriptor.FieldLength;
        }

        public void Load(ref BinaryReader br)
        {
            char[] rawData = br.ReadChars(discriptor.FieldLength);
            Value = Convert.ToSingle(rawData);
        }
    }

    public class DBLogical : IElement
    {
        public DbfFieldDiscriptor discriptor { get; set; }
        public bool Value { get; set; }

        public DBLogical(DbfFieldDiscriptor fd)
        {
            discriptor = fd;
        }

        public long GetLength()
        {
            return discriptor.FieldLength;
        }

        public void Load(ref BinaryReader br)
        {
            char[] rawData = br.ReadChars(discriptor.FieldLength);
            Value = Convert.ToBoolean(rawData);
        }
    }

    public class DBMemo : IElement
    {
        public DbfFieldDiscriptor discriptor { get; set; }
        public char[] Value { get; set; }

        public DBMemo(DbfFieldDiscriptor fd)
        {
            discriptor = fd;
        }

        public long GetLength()
        {
            return discriptor.FieldLength;
        }

        public void Load(ref BinaryReader br)
        {
            Value = br.ReadChars(discriptor.FieldLength);
        }
    }

    public class DBNumeric : IElement
    {
        public DbfFieldDiscriptor discriptor { get; set; }
        public char[] Value { get; set; }

        public DBNumeric(DbfFieldDiscriptor fd)
        {
            discriptor = fd;
        }
        public long GetLength()
        {
            return discriptor.FieldLength;
        }

        public void Load(ref BinaryReader br)
        {
            Value = br.ReadChars(discriptor.FieldLength);
        }
    }
    
    public class Range : IElement
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
            return sizeof(double) * 2;
        }
    }
    
    public class RangeXY : IElement
    {
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }

        public double Width  { get { return MaxX - MinX; } }
        public double Height { get { return MaxY - MinY; } }

        public void Load(ref BinaryReader br)
        {
            MinX = br.ReadDouble();
            MaxX = br.ReadDouble();
            MinY = br.ReadDouble();
            MaxY = br.ReadDouble();
        }

        public long GetLength()
        {
            return sizeof(double) * 4;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Point : IElement
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
            return sizeof(double) * 2;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MultiPoint : IElement
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
                Points[i] = new Point();
                Points[i].Load(ref br);
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int);
            size += Util.GetArraySize(Points);
            return size;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PolyLine : IElement, IRenderableData
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
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }

            Points = new Point[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i] = new Point();
                Points[i].Load(ref br);
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int);
            size += sizeof(int);
            size += Util.GetArraySize(Parts);
            size += Util.GetArraySize(Points);
            return size;
        }

        public void Render(RangeXY range, Color color)
        {
            GameObject gameObject = new GameObject("2DPolyLine");
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.positionCount = NumPoints;

            for (int i = 0; i < NumPoints; i++)
            {
                // For unity float acurracy,, 
                double relativeX = (Points[i].X - (range.MaxX + range.MinX) / 2) % 100000;
                double relativeY = (Points[i].Y - (range.MaxY + range.MinY) / 2) % 100000;
                Vector3 pt = new Vector3((float)relativeX, 0, (float)relativeY);
                lineRenderer.SetPosition(i, pt);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Polygon : IElement, IRenderableData
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
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }

            Points = new Point[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i] = new Point();
                Points[i].Load(ref br);
            }
        }

        public long GetLength()
        {
            long size = 0;
            size += XYRange.GetLength();
            size += sizeof(int);
            size += sizeof(int);
            size += Util.GetArraySize(Parts);
            size += Util.GetArraySize(Points);
            return size;
        }

        public void Render(RangeXY range, Color color)
        {
            GameObject shape = new GameObject("2DPolygon");
            shape.AddComponent<MeshRenderer>();
            shape.AddComponent<MeshFilter>();

            // Change Point type
            Vector2[] ptList = new Vector2[Points.Length];
            for (int i = 0; i< Points.Length; i++)
            {
                // For unity float acurracy,, 
                double relativeX = (Points[i].X - (range.MaxX + range.MinX) / 2) % 100000;
                double relativeY = (Points[i].Y - (range.MaxY + range.MinY) / 2) % 100000;
                ptList[i] = new Vector2((float)relativeX, (float)relativeY);
            }
            shape.GetComponent<MeshFilter>().mesh = Util.CreateMesh(ptList);
            shape.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            shape.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", color);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PointM : IElement
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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MultiPointM : IElement
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
                Points[i] = new Point();
                Points[i].Load(ref br);
            }

            MRange = new Range();
            MRange.Load(ref br);

            MValues = new double[NumPoints];
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
            size += Util.GetArraySize(Points);
            size += MRange.GetLength();
            size += Util.GetArraySize(MValues);
            return size;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PolyLineM : IElement, IRenderableData
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
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }

            Points = new Point[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i] = new Point();
                Points[i].Load(ref br);
            }

            MRange = new Range();
            MRange.Load(ref br);

            MValues = new double[NumPoints];
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
            size += sizeof(int);
            size += Util.GetArraySize(Parts);
            size += Util.GetArraySize(Points);
            size += MRange.GetLength();
            size += Util.GetArraySize(MValues);
            return size;
        }

        public void Render(RangeXY range, Color color)
        {
            GameObject gameObject = new GameObject("2DPolyLineM");
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.positionCount = NumPoints;

            for (int i = 0; i < NumPoints; i++)
            {
                // For unity float acurracy,, 
                double relativeX = (Points[i].X - (range.MaxX + range.MinX) / 2) % 100000;
                double relativeY = (Points[i].Y - (range.MaxY + range.MinY) / 2) % 100000;
                Vector3 pt = new Vector3((float)relativeX, 0, (float)relativeY);
                lineRenderer.SetPosition(i, pt*100);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PolygonM : IElement, IRenderableData
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
            size += sizeof(int);
            size += sizeof(int);
            size += Util.GetArraySize(Parts);
            size += Util.GetArraySize(Points);
            size += MRange.GetLength();
            size += Util.GetArraySize(MValues);
            return size;
        }

        public void Render(RangeXY range, Color color)
        {
            GameObject shape = new GameObject("2DPolygonM");
            shape.AddComponent<MeshRenderer>();
            shape.AddComponent<MeshFilter>();

            // Change Point type
            Vector2[] ptList = new Vector2[Points.Length];
            for (int i = 0; i< Points.Length; i++)
            {
                // For unity float acurracy,, 
                double relativeX = (Points[i].X - (range.MaxX + range.MinX) / 2) % 100000; ;
                double relativeY = (Points[i].Y - (range.MaxY + range.MinY) / 2) % 100000;;
                ptList[i] = new Vector2((float)relativeX, (float)relativeY);
            }
            shape.GetComponent<MeshFilter>().mesh = Util.CreateMesh(ptList);
            shape.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Default-Diffuse.mat"));
            shape.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", color);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PointZ : IElement
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
            M = br.ReadDouble();
        }

        public long GetLength()
        {
            return sizeof(double) * 4;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MultiPointZ : IElement
    {
        public RangeXY XYRange { get; set; }
        public int NumPoints { get; set; }
        public Point[] Points { get; set; }
        public Range ZRange { get; set; }
        public double[] ZValues { get; set; }
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
                Points[i] = new Point();
                Points[i].Load(ref br);
            }

            ZRange = new Range();
            ZRange.Load(ref br);

            ZValues = new double[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                ZValues[i] = br.ReadDouble();
            }

            MRange = new Range();
            MRange.Load(ref br);

            MValues = new double[NumPoints];
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
            size += Util.GetArraySize(Points);
            size += ZRange.GetLength();
            size += Util.GetArraySize(ZValues);
            size += MRange.GetLength();
            size += Util.GetArraySize(MValues);
            return size;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PolyLineZ : IElement, IRenderableData
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
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }

            Points = new Point[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i] = new Point();
                Points[i].Load(ref br);
            }

            ZRange = new Range();
            ZRange.Load(ref br);

            ZValues = new double[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                ZValues[i] = br.ReadDouble();
            }

            MRange.Load(ref br);
            MValues = new double[NumPoints];
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
            size += sizeof(int);
            size += Util.GetArraySize(Parts);
            size += Util.GetArraySize(Points);
            size += ZRange.GetLength();
            size += Util.GetArraySize(ZValues);
            size += MRange.GetLength();
            size += Util.GetArraySize(MValues);
            return size;
        }

        public void Render(RangeXY range, Color color)
        {
            GameObject shape = new GameObject("3DPolyLineZ");
            shape.AddComponent<MeshRenderer>();
            shape.AddComponent<MeshFilter>();

            // Change Point type
            Vector2[] ptList = new Vector2[Points.Length];
            for (int i = 0; i < Points.Length; i++) // last is same with first..
            {
                // For unity float acurracy,, 
                double relativeX = (Points[i].X - (range.MaxX + range.MinX) / 2) % 100000;
                double relativeY = (Points[i].Y - (range.MaxY + range.MinY) / 2) % 100000;
                Vector2 pt = new Vector2((float)relativeX, (float)relativeY);
                ptList[i] = pt;
            }
            shape.GetComponent<MeshFilter>().mesh = Util.CreateMesh(ptList);
            shape.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Default-Diffuse.mat"));
            shape.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", color);

        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PolygonZ : IElement, IRenderableData
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
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }

            Points = new Point[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i] = new Point();
                Points[i].Load(ref br);
            }

            ZRange = new Range();
            ZRange.Load(ref br);

            ZValues = new double[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                ZValues[i] = br.ReadDouble();
            }

            MRange = new Range();
            MRange.Load(ref br);

            MValues = new double[NumPoints];
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
            size += sizeof(int);
            size += Util.GetArraySize(Parts);
            size += Util.GetArraySize(Points);
            size += ZRange.GetLength();
            size += Util.GetArraySize(ZValues);
            size += MRange.GetLength();
            size += Util.GetArraySize(MValues);
            return size;
        }

        public void Render(RangeXY range, Color color)
        {
            GameObject shape = new GameObject("3DPolygonZ");
            shape.AddComponent<MeshRenderer>();
            shape.AddComponent<MeshFilter>();

            // Change Point type
            Vector2[] ptList = new Vector2[Points.Length];
            for (int i = 0; i < Points.Length; i++)
            {
                // For unity float acurracy,, 
                double relativeX = (Points[i].X - (range.MaxX + range.MinX) / 2) % 100000;
                double relativeY = (Points[i].Y - (range.MaxY + range.MinY) / 2) % 100000;
                Vector2 pt = new Vector2((float)relativeX, (float)relativeY); 
                ptList[i] = pt;
            }

            shape.GetComponent<MeshFilter>().mesh = Util.CreateMesh(ptList);
            shape.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Default-Diffuse.mat"));
            shape.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", color);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MultiPatch : IElement, IRenderableData
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
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }

            PartsTypes = new int[NumParts];
            for (int i = 0; i < NumParts; i++)
            {
                PartsTypes[i] = br.ReadInt32();
            }

            Points = new Point[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i] = new Point();
                Points[i].Load(ref br);
            }

            ZRange = new Range();
            ZRange.Load(ref br);

            ZValues = new double[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                ZValues[i] = br.ReadDouble();
            }

            MRange = new Range();
            MRange.Load(ref br);

            MValues = new double[NumPoints];
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
            size += sizeof(int);
            size += Util.GetArraySize(Parts);
            size += Util.GetArraySize(PartsTypes);
            size += Util.GetArraySize(Points);
            size += ZRange.GetLength();
            size += Util.GetArraySize(ZValues);
            size += MRange.GetLength();
            size += Util.GetArraySize(MValues);
            return size;
        }
        
        public void Render(RangeXY range, Color color)
        {
            throw new NotImplementedException();
        }
    }
}
