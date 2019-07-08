using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assets
{
    public class ShxRecord : IRecord
    {
        public RecordHeader header { get; set; }

        public void Load(ref BinaryReader br)
        {
            header.Load(ref br);
        }
    }

    public class ShpRecord : IRecord
    {
        public RecordHeader header { get; set; }
        public IRecordContents Contents { get; set; }

        public void Load(ref BinaryReader br)
        {
            header.Load(ref br);
            Contents.Load(ref br);
        }
    }

    public class RecordHeader : IRecord
    {
        public int RecordNumber { get; set; }
        public int ContentLength { get; set; }

        public void Load(ref BinaryReader br)
        {
            RecordNumber = Endian.FromBigEndian(br.ReadInt32());
            ContentLength = Endian.FromBigEndian(br.ReadInt32());
        }
    }
}
