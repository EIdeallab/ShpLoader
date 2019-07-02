using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public interface IFile
    {
        void Load(string path);
        void Save(string path);
        void GetData(int index);
    }
}
