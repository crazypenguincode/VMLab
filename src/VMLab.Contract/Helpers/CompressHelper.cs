using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace VMLab.Contract.Helpers
{
    public class CompressHelper : ICompressHelper
    {

        public Stream GetFileFromZip(string archive, string filename)
        {
            using (var zip = ZipFile.OpenRead(archive))
            {
                foreach (var entry in zip.Entries)
                    if (entry.Name.ToLower() == filename)
                    {
                        var stream = entry.Open();
                        var memoryStream = new MemoryStream();
                        var buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        memoryStream.Write(buffer, 0, buffer.Length);
                        stream.Close();

                        return memoryStream;
                    }
            }

            return new MemoryStream();
        }

        public string GetTextFromZip(string archive, string filename)
        {
            return new StreamReader(GetFileFromZip(archive, filename)).ReadToEnd();
        }

        public void ExtractToFolder(string archive, string path)
        {
            ZipFile.ExtractToDirectory(archive, path);
        }

        public void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory, Encoding entryNameEncoding, Func<string, bool> filter)
        {
            if (string.IsNullOrEmpty(sourceDirectoryName))
            {
                throw new ArgumentNullException(nameof(sourceDirectoryName));
            }
            if (string.IsNullOrEmpty(destinationArchiveFileName))
            {
                throw new ArgumentNullException(nameof(destinationArchiveFileName));
            }
            var filesToAdd = Directory.GetFiles(sourceDirectoryName, "*", SearchOption.AllDirectories);
            var entryNames = GetEntryNames(filesToAdd, sourceDirectoryName, includeBaseDirectory);
            using (var zipFileStream = new FileStream(destinationArchiveFileName, FileMode.Create))
            {
                using (var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Create))
                {
                    for (var i = 0; i < filesToAdd.Length; i++)
                    {
                        // Add the following condition to do filtering:
                        if (!filter(filesToAdd[i]))
                        {
                            continue;
                        }
                        archive.CreateEntryFromFile(filesToAdd[i], entryNames[i], compressionLevel);
                    }
                }
            }
        }

        private string[] GetEntryNames(string[] names, string sourceFolder, bool includeBaseName)
        {
            if (names == null || names.Length == 0)
                return new string[0];

            if (includeBaseName)
                sourceFolder = Path.GetDirectoryName(sourceFolder);

            var length = string.IsNullOrEmpty(sourceFolder) ? 0 : sourceFolder.Length;
            if (length > 0 && sourceFolder != null && sourceFolder[length - 1] != Path.DirectorySeparatorChar && sourceFolder[length - 1] != Path.AltDirectorySeparatorChar)
                length++;

            var result = new string[names.Length];
            for (var i = 0; i < names.Length; i++)
            {
                result[i] = names[i].Substring(length);
            }

            return result;
        }

    }
}
