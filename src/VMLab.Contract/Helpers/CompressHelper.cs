using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Serilog;

namespace VMLab.Contract.Helpers
{
    public class CompressHelper : ICompressHelper
    {
        private readonly ILogger _log;

        public CompressHelper(ILogger log)
        {
            _log = log;
        }

        public Stream GetFileFromZip(string archive, string filename)
        {
            _log.Information("Extracting {filename} from {archive}", filename, archive);

            using (var zip = ZipFile.OpenRead(archive))
            {
                foreach (var entry in zip.Entries)
                    if (entry.Name.ToLower() == filename)
                    {
                        var stream = entry.Open();
                        var memoryStream = new MemoryStream();
                        var buffer = new byte[16384];

                        int read;

                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            memoryStream.Write(buffer, 0, read);
                        }

                        stream.Close();

                        return memoryStream;
                    }
            }

            return new MemoryStream();
        }

        public string GetTextFromZip(string archive, string filename)
        {
            _log.Information("Getting text from {filename} in {archive}", filename, archive);

            var data = GetFileFromZip(archive, filename);
            data.Seek(0, SeekOrigin.Begin);
            return new StreamReader(data).ReadToEnd();
        }

        public void ExtractToFolder(string archive, string path)
        {
            _log.Information("Extracting {archive} to {path}", archive, path);

            ZipFile.ExtractToDirectory(archive, path);
        }

        public void CreateFromDirectory(string sourcePath, string destinationPath)
        {
            CreateFromDirectory(sourcePath, destinationPath, CompressionLevel.Optimal, false, Encoding.UTF8, s => true);
        }

        public void CreateFromDirectory(string sourcePath, string destinationPath, CompressionLevel compressionLevel, bool includeBaseDirectory, Encoding entryNameEncoding, Func<string, bool> filter)
        {
            _log.Information("Compressing directory {sourcePath} to {destinationPath} with compression level {compressionLevel}", sourcePath, destinationPath, compressionLevel);

            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentNullException(nameof(sourcePath));
            }
            if (string.IsNullOrEmpty(destinationPath))
            {
                throw new ArgumentNullException(nameof(destinationPath));
            }
            var filesToAdd = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
            var entryNames = GetEntryNames(filesToAdd, sourcePath, includeBaseDirectory);
            using (var zipFileStream = new FileStream(destinationPath, FileMode.Create))
            {
                using (var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Create))
                {
                    for (var i = 0; i < filesToAdd.Length; i++)
                    {
                        // Add the following condition to do filtering:
                        if (!filter(filesToAdd[i]))
                        {
                            _log.Information("Skipping file: {file}", filesToAdd[i]);
                            continue;
                        }
                        _log.Information("Adding file: {file}", filesToAdd[i]);
                        archive.CreateEntryFromFile(filesToAdd[i], entryNames[i], compressionLevel);
                    }
                }
            }
        }

        private static string[] GetEntryNames(IReadOnlyList<string> names, string sourceFolder, bool includeBaseName)
        {
            if (names == null || names.Count == 0)
                return new string[0];

            if (includeBaseName)
                sourceFolder = Path.GetDirectoryName(sourceFolder);

            var length = string.IsNullOrEmpty(sourceFolder) ? 0 : sourceFolder.Length;
            if (length > 0 && sourceFolder != null && sourceFolder[length - 1] != Path.DirectorySeparatorChar && sourceFolder[length - 1] != Path.AltDirectorySeparatorChar)
                length++;

            var result = new string[names.Count];
            for (var i = 0; i < names.Count; i++)
            {
                result[i] = names[i].Substring(length);
            }

            return result;
        }

    }
}
