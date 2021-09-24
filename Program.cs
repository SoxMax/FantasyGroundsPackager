using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Linq;

namespace FantasyGroundsPackager
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string rootDir = GetPackageDirectory(args);
                ValidatePackageDirectory(rootDir);
                List<string> validFiles = ReadPackageContents(Path.Join(rootDir, "extension.xml"));

                string packageFile = Path.Join(rootDir, $"{Path.GetFileName(rootDir)}.ext");
                if (File.Exists(packageFile))
                {
                    File.Delete(packageFile);
                }
                using ZipArchive archive = ZipFile.Open(packageFile, ZipArchiveMode.Create);
                foreach (string file in validFiles)
                {
                    archive.CreateEntryFromFile(Path.Join(rootDir, file), file);
                }
                Console.WriteLine("Success!");
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static string GetPackageDirectory(string[] args)
        {
            if (args.Length > 0 && args[0].Trim().Length > 0)
            {
                return Path.GetFullPath(args[0].Trim());
            }
            else
            {
                return Environment.CurrentDirectory;
            }
        }

        static void ValidatePackageDirectory(string directory)
        {
            string extensionFile = Path.Join(directory, "extension.xml");
            if (!File.Exists(extensionFile))
            {
                throw new FileNotFoundException("No extension file found!", extensionFile);
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
            validFiles.AddRange(Directory.EnumerateFiles(Path.GetDirectoryName(extensionFile), "*.md").Select(mdFile => Path.GetFileName(mdFile)));
            return validFiles;
        }
    }
}
