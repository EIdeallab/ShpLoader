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
    }

    public interface IRecordContents : IRecord
    {
        long GetLength();
    }
}
