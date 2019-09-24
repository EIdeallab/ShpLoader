using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets
{
    public interface IFile
    {
        void Load();
        IRecord GetData(int index);
    }

    public interface IShpFile : IFile
    {
        int FileCode { get; set; }
        int FileLength { get; set; }
        int FileVersion { get; set; }
        ShapeType ShpType { get; set; }
        RangeXY TotalXYRange { get; set; }
        Range ZRange { get; set; }
        Range MRange { get; set; }
        int ContentLength { get; set; }
    }
}
