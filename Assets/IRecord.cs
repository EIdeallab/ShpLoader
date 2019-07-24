using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Assets
{
    public interface IRecord
    {
        void Load(ref BinaryReader br);
        long GetLength();
    }

    public interface IDBRecord
    {
        void Load(ref BinaryReader br, DbfFieldDiscriptor fd);
        long GetLength();
    }
}
