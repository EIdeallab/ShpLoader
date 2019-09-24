using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using static System.Net.IPAddress;

namespace Assets
{
    public static class Util
    {
        public static short ToBigEndian(this short value) => HostToNetworkOrder(value);
        public static int ToBigEndian(this int value) => HostToNetworkOrder(value);
        public static long ToBigEndian(this long value) => HostToNetworkOrder(value);
        public static short FromBigEndian(this short value) => NetworkToHostOrder(value);
        public static int FromBigEndian(this int value) => NetworkToHostOrder(value);
        public static long FromBigEndian(this long value) => NetworkToHostOrder(value);

        public static long GetArraySize(Array arr)
        {
            return arr.LongLength * Marshal.SizeOf(arr.GetType().GetElementType());
        }

        // Vertex position in 0-1 UV space
        // 4 points in the list for the square made of two triangles:
        //  0 *--* 1
        //    | /|
        //    |/ |
        //  3 *--* 2
        public static Mesh CreateMesh(Vector2[] points)
        {
            List<Vector2> ptList = new List<Vector2>(points);
            ptList.RemoveAt(ptList.Count - 1);

            int[] tris = new int[ptList.Count]; // Every 3 ints represents a triangle
            Triangulator tr = new Triangulator(ptList);
            int[] indices = tr.Triangulate();

            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[ptList.Count];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(ptList[i].x, 0, ptList[i].y);
            }

            Vector3[] normals = new Vector3[vertices.Length];
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = Vector3.up;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.normals = normals;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
        
    }
}
