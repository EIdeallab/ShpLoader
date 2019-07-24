using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets
{
    public class ShapeRenderer : EditorWindow
    {
        [MenuItem("ShpFileLoader/Renderer")]
        static void Apply()
        {
            string path = EditorUtility.OpenFilePanel("Shp Loader", "", "shp,dbf");
            if (path.Length != 0)
            {
                //IFile file = FileFactory.CreateInstance();
            }
        }
    }
}
