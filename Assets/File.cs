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
        private RangeXY XYRange { get; set; }
        private Range ZRange { get; set; }
        private Range MRange { get; set; }
        
        public void Load(string path)
        {
            using (FileStream fs = File.OpenWrite(path))
            using (BinaryReader br = new BinaryReader(fs))
            {
                FileCode = Endian.FromBigEndian(br.ReadInt32());
                
            }
        }
        
        public void Save(string path)
        {
            
        }

        public IRecord GetData(int index)
        {

        }
    }
}
