using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets
{
    class ShpFile : IShpFile, IRenderable
    {
        private bool disposed;
        private FileStream fs;
        private BinaryReader br;

        public int FileCode { get; set; }
        public int FileLength { get; set; }
        public int FileVersion { get; set; }
        public ShapeType ShpType { get; set; }
        public RangeXY TotalXYRange { get; set; }
        public Range ZRange { get; set; }
        public Range MRange { get; set; }
        public int ContentLength { get; set; }
        public List<ShpRecord> RecordSet { get; set; }

        public ShpFile(string path)
        {
            fs = File.OpenRead(path);
            br = new BinaryReader(fs);
        }

        ~ShpFile() // the finalizer
        {
            Dispose(false);
        }

        public void Load()
        {
            FileCode = Util.FromBigEndian(br.ReadInt32());
            br.BaseStream.Seek(20, SeekOrigin.Current);
            FileLength = Util.FromBigEndian(br.ReadInt32()) * 2;
            FileVersion = br.ReadInt32();
            ShpType = (ShapeType)br.ReadInt32();

            TotalXYRange = new RangeXY();
            ZRange = new Range();
            MRange = new Range();
            TotalXYRange.Load(ref br);
            ZRange.Load(ref br);
            MRange.Load(ref br);

            ContentLength = FileLength - 100;
            long curPoint = 0;

            RecordSet = new List<ShpRecord>();
            while (curPoint < ContentLength)
            {
                ShpRecord record = new ShpRecord(ShpType);
                record.Load(ref br);
                long size = record.GetLength();
                RecordSet.Add(record);

                curPoint += record.GetLength();
            }
        }

        public IRecord GetData(int index)
        {
            return RecordSet.ElementAt(index);
        }

        public IRecord GetData(ShapeType type, int offset, int length)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            IRecord record = new ShpRecord(type);
            record.Load(ref br);
            return record;
        }

        public void Render(Color color)
        {
            foreach (ShpRecord record in RecordSet)
            {
                record.Render(TotalXYRange, color);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                br.Dispose();
                fs.Dispose();
            }
            disposed = true;
        }
    }

    class ShxFile : IShpFile, IDisposable, IRenderable
    {
        private bool disposed;
        private FileStream fs;
        private BinaryReader br;
        private string shpPath;

        public int FileCode { get; set; }
        public int FileLength { get; set; }
        public int FileVersion { get; set; }
        public ShapeType ShpType { get; set; }
        public RangeXY TotalXYRange { get; set; }
        public Range ZRange { get; set; }
        public Range MRange { get; set; }
        public int ContentLength { get; set; }
        public List<ShxRecord> RecordSet { get; set; }
        public ShpFile ContentsFile { get; set; }

        public ShxFile(string path)
        {
            fs = File.OpenRead(path);
            br = new BinaryReader(fs);
            shpPath = path;
        }

        ~ShxFile() // the finalizer
        {
            Dispose(false);
        }

        public void Load()
        {
            FileCode = Util.FromBigEndian(br.ReadInt32());
            br.BaseStream.Seek(20, SeekOrigin.Current);
            FileLength = Util.FromBigEndian(br.ReadInt32()) * 2;
            FileVersion = br.ReadInt32();
            ShpType = (ShapeType)br.ReadInt32();

            TotalXYRange = new RangeXY();
            ZRange = new Range();
            MRange = new Range();
            TotalXYRange.Load(ref br);
            ZRange.Load(ref br);
            MRange.Load(ref br);

            int ContentLength = FileLength - 100;
            long curPoint = 0;

            RecordSet = new List<ShxRecord>();
            while (curPoint < ContentLength)
            {
                ShxRecord record = new ShxRecord();
                record.Load(ref br);
                RecordSet.Add(record);

                curPoint += record.GetLength();
            }
            try
            {
                ContentsFile = (ShpFile)FileFactory.CreateInstance(shpPath);
            }
            catch (Exception)
            {
                ContentsFile = null;
            }
        }

        public IRecord GetData(int index)
        {
            if(ContentsFile != null)
            {
                ShxRecord shxRecord = (ShxRecord)RecordSet[index];
                IRecord shpRecord = ContentsFile.GetData(ShpType, shxRecord.Offset, shxRecord.Length);
                return shpRecord;
            }
            else
            {
                return null;
            }
        }

        public void Render(Color color)
        {
            ContentsFile.Render(color);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                br.Dispose();
                fs.Dispose();
                ContentsFile.Dispose();
            }
            disposed = true;
        }
    }

    class DbfFile : IFile
    {
        private bool disposed;
        private FileStream fs;
        private BinaryReader br;

        public DBFVersion Version { get; set; }
        public byte UpdateYear { get; set; }
        public byte UpdateMonth { get; set; }
        public byte UpdateDay { get; set; }
        public int UpdateDate { get { return UpdateYear * 10000 + UpdateMonth * 100 + UpdateDay; } }
        public int NumberOfRecords { get; set; }
        public short HeaderLength { get; set; }
        public short RecordLength { get; set; }
        public byte[] Reserved { get; set; }
        public List<DbfFieldDiscriptor> FieldList { get; set; }
        public List<DbfRecord> RecordSet { get; set; }

        public DbfFile(string path)
        {
            fs = File.OpenRead(path);
            br = new BinaryReader(fs);
        }
        
        public void Load()
        {   
            Version = (DBFVersion)br.ReadByte();
            UpdateYear = br.ReadByte();
            UpdateMonth = br.ReadByte();
            UpdateDay = br.ReadByte();
            NumberOfRecords = br.ReadInt32();
            HeaderLength = br.ReadInt16();
            RecordLength = br.ReadInt16();
            Reserved = br.ReadBytes(20);

            FieldList = new List<DbfFieldDiscriptor>();
            RecordSet = new List<DbfRecord>();
            while (br.PeekChar() != 0x0d)
            {
                DbfFieldDiscriptor field = new DbfFieldDiscriptor();
                field.Load(ref br);
                FieldList.Add(field);
            }

            br.BaseStream.Position = HeaderLength;

            foreach(DbfFieldDiscriptor field in FieldList)
            {
                DbfRecord record = new DbfRecord(FieldList);
                record.Load(ref br);
                RecordSet.Add(record);
            }
        }

        public IRecord GetData(int index)
        {
            throw new NotImplementedException();
        }
    }

    public class FileFactory
    {
        public static readonly IDictionary<string, Func<string, IFile>> Creators =
            new Dictionary<string, Func<string, IFile>>()
            {
                { ".shp", (path) => new ShpFile(path) },
                { ".shx", (path) => new ShxFile(path) },
                { ".dbf", (path) => new DbfFile(path) }
            };

        public static IFile CreateInstance(string path)
        {
            return Creators[Path.GetExtension(path)](path);
        }
    }
}
