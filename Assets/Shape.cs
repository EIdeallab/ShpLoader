using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Net.IPAddress;

namespace Assets
{
    public interface IShape
    {
        void Load(BinaryReader br);
    }

    public static class BigEndian
    {
        public static short ToBigEndian(this short value) => HostToNetworkOrder(value);
        public static int ToBigEndian(this int value) => HostToNetworkOrder(value);
        public static long ToBigEndian(this long value) => HostToNetworkOrder(value);
        public static short FromBigEndian(this short value) => NetworkToHostOrder(value);
        public static int FromBigEndian(this int value) => NetworkToHostOrder(value);
        public static long FromBigEndian(this long value) => NetworkToHostOrder(value);
    }

    public class BoundingBox : IShape
    {
        public double Xmin;
        public double Ymin;
        public double Xmax;
        public double Ymax;
        public double Zmin;
        public double Zmax;
        public double Mmin;
        public double Mmax;

        public void Load(BinaryReader br)
        {
            Xmin = br.ReadDouble();
            Ymin = br.ReadDouble();
            Xmax = br.ReadDouble();
            Ymax = br.ReadDouble();
            Zmin = br.ReadDouble();
            Zmax = br.ReadDouble();
            Mmin = br.ReadDouble();
            Mmax = br.ReadDouble();
        }
    }

    public class Point : IShape
    {
        double x;
        double y;

        public void Load(BinaryReader br)
        {
            x = br.ReadDouble();
            y = br.ReadDouble();
        }
    }

    public class PolyLine : IShape
    {
        BoundingBox Box;
        int NumParts;
        int NumPoints;
        int[] Parts;
        Point[] Points;

        public void Load(BinaryReader br)
        {
            Box = new BoundingBox();
            Box.Load(br);
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
                Points[i].Load(br);
            }
        }
    }

    public class Polygon : IShape
    {
        BoundingBox Box;
        int NumParts;
        int NumPoints;
        int[] Parts;
        Point[] Points;

        public void Load(BinaryReader br)
        {
            Box = new BoundingBox();
            Box.Load(br);
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
                Points[i].Load(br);
            }
        }
    }

    public class MultiPoint : IShape
    {
        BoundingBox Box;
        int NumPoints;
        Point[] Points;

        public void Load(BinaryReader br)
        {
            Box = new BoundingBox();
            Box.Load(br);
            NumPoints = br.ReadInt32();
            Points = new Point[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(br);
            }
        }
    }

    public class PointZ : IShape
    {
        double x;
        double y;
        double z;

        public void Load(BinaryReader br)
        {
            x = br.ReadDouble();
            y = br.ReadDouble();
            z = br.ReadDouble();
        }
    }

    public class PolyLineZ : IShape
    {
        BoundingBox Box;
        int NumParts;
        int NumPoints;
        int[] Parts;
        PointZ[] Points;

        public void Load(BinaryReader br)
        {
            Box = new BoundingBox();
            Box.Load(br);
            NumParts = br.ReadInt32();
            NumPoints = br.ReadInt32();
            Parts = new int[NumParts];
            Points = new PointZ[NumPoints];
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(br);
            }
        }
    }

    public class PolygonZ : IShape
    {
        BoundingBox Box;
        int NumParts;
        int NumPoints;
        int[] Parts;
        PointZ[] Points;

        public void Load(BinaryReader br)
        {
            Box = new BoundingBox();
            Box.Load(br);
            NumParts = br.ReadInt32();
            NumPoints = br.ReadInt32();
            Parts = new int[NumParts];
            Points = new PointZ[NumPoints];
            for (int i = 0; i < NumParts; i++)
            {
                Parts[i] = br.ReadInt32();
            }
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(br);
            }
        }
    }

    public class MultiPointZ : IShape
    {
        BoundingBox Box;
        int NumPoints;
        PointZ[] Points;

        public void Load(BinaryReader br)
        {
            Box = new BoundingBox();
            Box.Load(br);
            NumPoints = br.ReadInt32();
            Points = new PointZ[NumPoints];
            for (int i = 0; i < NumPoints; i++)
            {
                Points[i].Load(br);
            }
        }
    }

    public class PointM : IShape
    {
        double x;
        double y;
        double m;

        public void Load(BinaryReader br)
        {
            throw new NotImplementedException();
        }
    }

    public class PolyLineM : IShape
    {
        BoundingBox Box;
        int NumParts;
        int NumPoints;
        int[] Parts;
        PointM[] Points;

        public void Load(BinaryReader br)
        {
            throw new NotImplementedException();
        }
    }
     
    public class PolygonM : IShape
    {
        BoundingBox Box;
        int NumParts;
        int NumPoints;
        int[] Parts;
        PointM[] Points;

        public void Load(BinaryReader br)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiPointM : IShape
    {
        BoundingBox Box;
        int numPoints;
        PointM[] Points;

        public void Load(BinaryReader br)
        {
            throw new NotImplementedException();
        }
    }
}
