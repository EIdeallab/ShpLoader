using System;
using System.Collections.Generic;
using System.IO;

namespace Assets
{
    class ShpFile : IFile
    {
        private int FileCode { get; set; }
        private int FlieLength { get; set; }
        private int FileVersion { get; set; }
        private ShapeType ShpType { get; set; }
        private BoundingBox ShpBox { get; set; }

        public ShpFile(int code, int length, int version, ShapeType type, BoundingBox box)
        {
            FileCode = code;
            FlieLength = length;
            FileVersion = version;
            ShpType = type;
            ShpBox = box;
        }

        public void Load(string path)
        {
            throw new NotImplementedException();
        }

        public void GetData(int index)
        {
            throw new NotImplementedException();
        }

        public void Save(string path)
        {
            throw new NotImplementedException();
        }
    }
}
