using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace FantasyGroundsPackager
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootDir = Path.GetFullPath(args[0]);
            FileAttributes rootDirAttributes = File.GetAttributes(rootDir);
            rootDirAttributes.HasFlag(FileAttributes.Directory);
            List<string> validFiles = ReadPackageContents(Path.Join(rootDir, "extension.xml"));

            string packageFile = Path.Join(rootDir, $"{Path.GetFileName(rootDir)}.ext");
            if(File.Exists(packageFile))
            {
                File.Delete(packageFile);
            }
            using ZipArchive archive = ZipFile.Open(packageFile, ZipArchiveMode.Create);
            foreach (string file in validFiles)
            {
                archive.CreateEntryFromFile(Path.Join(rootDir, file), file);
            }
        }
        static List<string> ReadPackageContents(string extensionFile)
        {
            List<string> validFiles = new() { "extension.xml" };
            XmlDocument extensionDoc = new();
            extensionDoc.Load(extensionFile);
            foreach (XmlNode fileNode in extensionDoc.SelectNodes("/root/base/*"))
            {
                if (fileNode.Name == "includefile")
                {
                    validFiles.Add(fileNode.Attributes.GetNamedItem("source").Value);
                }
                else if (fileNode.Name == "script" || fileNode.Name == "icon")
                {
                    validFiles.Add(fileNode.Attributes.GetNamedItem("file").Value);
                }
            }
            return validFiles;
        }
    }
}
