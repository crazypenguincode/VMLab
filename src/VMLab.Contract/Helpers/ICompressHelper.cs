using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace VMLab.Contract.Helpers
{
    public interface ICompressHelper
    {
        void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName,
            CompressionLevel compressionLevel, bool includeBaseDirectory, Encoding entryNameEncoding,
            Func<string, bool> filter);

        void ExtractToFolder(string archive, string path);

        Stream GetFileFromZip(string archive, string filename);
        string GetTextFromZip(string archive, string filename);
    }
}
