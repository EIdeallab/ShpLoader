using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public enum FileType
    {
        Shp,
        Shx,
        Dbf
    }

    public enum ShapeType
    {
        Null = 0,
        Point = 1,
        Polyline = 3,
        Polygon = 5,
        MultiPoint = 8,
        PointZ = 11,
        PolylineZ = 13,
        PolygonZ = 15,
        MultiPointZ = 18,
        PointM = 21,
        PolylineM = 23,
        PolygonM = 25,
        MultiPointM = 28,
        MultiPatch = 31
    }

    public struct BoundingBox
    {
        public double Xmin;
        public double Ymin;
        public double Xmax;
        public double Ymax;
        public double Zmin;
        public double Zmax;
        public double Mmin;
        public double Mmax;
    }

    public struct Point
    {
        double x;
        double y;
    }

    public struct PolyLine
    {
        BoundingBox Box;
        int NumParts;
        int NumPoints;
        int[] Parts;
        Point[] Points;
    }

    public struct Polygon
    {
        BoundingBox Box;
        int NumParts;
        int NumPoints;
        int[] Parts;
        Point[] Points;
    }

    public struct MultiPoint
    {
        BoundingBox Box;
        int numPoints;
        Point[] Points;
    }

    public struct PointZ
    {
        double x;
        double y;
        double z;
    }

    public struct PolyLineZ
    {
        BoundingBox Box;
        int NumParts;
        int NumPoints;
        int[] Parts;
        PointZ[] Points;
    }

    public struct PolygonZ
    {
        BoundingBox Box;
        int NumParts;
        int NumPoints;
        int[] Parts;
        PointZ[] Points;
    }

    public struct MultiPointZ
    {
        BoundingBox Box;
        int numPoints;
        PointZ[] Points;
    }
    
    public struct PointM
    {
        double x;
        double y;
        double m;
    }

    public struct PolyLineM
    {
        BoundingBox Box;
        int NumParts;
        int NumPoints;
        int[] Parts;
        PointM[] Points;
    }

    public struct PolygonM
    {
        BoundingBox Box;
        int NumParts;
        int NumPoints;
        int[] Parts;
        PointM[] Points;
    }

    public struct MultiPointM
    {
        BoundingBox Box;
        int numPoints;
        PointM[] Points;
    }
}
