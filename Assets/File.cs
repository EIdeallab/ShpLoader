using System;
using System.Collections.Generic;
using System.IO;

namespace Assets
{
    class ShpFile : IFile
    {
        public int FileCode { get; set; }
        public int FileLength { get; set; }
        public int FileVersion { get; set; }
        public ShapeType ShpType { get; set; }
        public RangeXY XYRange { get; set; }
        public Range ZRange { get; set; }
        public Range MRange { get; set; }
        public int ContentLength { get; set; }
        public List<IRecord> RecordSet { get; set; }
        
        public void Load(string path)
        {
            using (FileStream fs = File.OpenWrite(path))
            {
                BinaryReader br = new BinaryReader(fs);
                FileCode = Endian.FromBigEndian(br.ReadInt32());
                br.BaseStream.Seek(20, SeekOrigin.Current);
                FileLength = Endian.FromBigEndian(br.ReadInt32()) * 2;
                FileVersion = br.ReadInt32();
                ShpType = (ShapeType)br.ReadInt32();
                XYRange.MinX = br.ReadDouble();
                XYRange.MinY = br.ReadDouble();
                XYRange.MaxX = br.ReadDouble();
                XYRange.MaxY = br.ReadDouble();
                ZRange.Min = br.ReadDouble();
                ZRange.Max = br.ReadDouble();
                MRange.Min = br.ReadDouble();
                MRange.Max = br.ReadDouble();

                ContentLength = FileLength - 100;
                long curPoint = 0;

                while (curPoint < ContentLength)
                {
                    ShpRecord record = new ShpRecord(ShpType);
                    record.Load(ref br);
                    RecordSet.Add(record);

                    curPoint += record.GetLength();
                }
                br.Dispose();
            }
        }

        public IRecord GetData(int index)
        {
            throw new NotImplementedException();
        }
    }
    class ShxFile : IFile
    {
        public int FileCode { get; set; }
        public int FileLength { get; set; }
        public int FileVersion { get; set; }
        public ShapeType ShpType { get; set; }
        public RangeXY XYRange { get; set; }
        public Range ZRange { get; set; }
        public Range MRange { get; set; }
        
        public void Load(string path)
        {
            using (FileStream fs = File.OpenWrite(path))
            {
                BinaryReader br = new BinaryReader(fs);
                FileCode = Endian.FromBigEndian(br.ReadInt32());
                br.BaseStream.Seek(20, SeekOrigin.Current);
                FileLength = Endian.FromBigEndian(br.ReadInt32()) * 2;
                FileVersion = br.ReadInt32();
                ShpType = (ShapeType)br.ReadInt32();
                XYRange.MinX = br.ReadDouble();
                XYRange.MinY = br.ReadDouble();
                XYRange.MaxX = br.ReadDouble();
                XYRange.MaxY = br.ReadDouble();
                ZRange.Min = br.ReadDouble();
                ZRange.Max = br.ReadDouble();
                MRange.Min = br.ReadDouble();
                MRange.Max = br.ReadDouble();

                int ContentLength = FileLength - 100;
                long curPoint = 0;

                while (curPoint < ContentLength)
                {
                    ShxRecord record = new ShxRecord();
                    record.Load(ref br);

                    curPoint += record.GetLength();
                }

                br.Dispose();
            }
        }

        public IRecord GetData(int index)
        {
            throw new NotImplementedException();
        }
    }

    class DbfFile : IFile
    {
        public DBFVersion Version { get; set; }
        public byte UpdateYear { get; set; }
        public byte UpdateMonth { get; set; }
        public byte UpdateDay { get; set; }
        public int NumberOfRecords { get; set; }
        public short HeaderLength { get; set; }
        public short RecordLength { get; set; }
        public byte[] Reserved { get; set; }
        public DBFTableFlags TableFlags { get; set; }
        public byte CodePage { get; set; }
        public short EndOfHeader { get; set; }

        public void Load(string path)
        {
            using (FileStream fs = File.OpenWrite(path))
            {
                BinaryReader br = new BinaryReader(fs);
                Version = (DBFVersion)br.ReadByte();
                UpdateYear = br.ReadByte();
                UpdateMonth = br.ReadByte();
                UpdateDay = br.ReadByte();
                NumberOfRecords = br.ReadInt32();
                HeaderLength = br.ReadInt16();
                RecordLength = br.ReadInt16();
                Reserved = br.ReadBytes(2);
                TableFlags = (DBFTableFlags)br.ReadByte();
                CodePage = br.ReadByte();
                EndOfHeader = br.ReadInt16();
                
                long curPoint = 0;

                while (curPoint < RecordLength)
                {
                    DbfRecord record = new DbfRecord();
                    record.Load(ref br);

                    curPoint += record.GetLength();
                }

                br.Dispose();
            }
        }

        public IRecord GetData(int index)
        {
            throw new NotImplementedException();
        }
    }

    public class FileFactory
    {
        public static readonly IDictionary<FileType, Func<IFile>> Creators =
            new Dictionary<FileType, Func<IFile>>()
            {
                { FileType.Shp, () => new ShpFile() },
                { FileType.Shx, () => new ShxFile() },
                { FileType.Dbf, () => new DbfFile() }
            };

        public static IFile CreateInstance(FileType enumModuleName)
        {
            return Creators[enumModuleName]();
        }
    }
}
