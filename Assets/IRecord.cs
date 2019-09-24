using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace Assets
{
    public interface IRecord
    {
        void Load(ref BinaryReader br);
        long GetLength();
    }

    public interface IElement
    {
        void Load(ref BinaryReader br);
        long GetLength();
    }

    public interface IRenderable
    {
        void Render(Color color);
    }

    public interface IRenderableData
    {
        void Render(RangeXY range, Color color);
    }
}
