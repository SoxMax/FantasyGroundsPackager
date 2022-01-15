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
            string rootDir = GetPackageDirectory(args);
            string definitionFile = ValidatePackageDirectory(rootDir);
            List<string> validFiles = ReadPackageContents(rootDir, definitionFile);
            validFiles.AddRange(Directory.EnumerateFiles(rootDir, "*.md").Select(mdFile => Path.GetFileName(mdFile)));

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

        static string ValidatePackageDirectory(string directory)
        {
            string extensionFile = Path.Join(directory, "extension.xml");
            if (!File.Exists(extensionFile))
            {
                throw new FileNotFoundException("No extension file found!", extensionFile);
            }
            return "extension.xml";
        }

        static List<string> ReadPackageContents(string dir, string packageFile)
        {
            Console.WriteLine(Path.Join(dir, packageFile));
            List<string> validFiles = new() { packageFile };
            if (Path.GetExtension(packageFile) == ".xml")
            {
                XmlDocument xmlDoc = new();
                xmlDoc.Load(Path.Join(dir, packageFile));
                List<string> files = ReadNodesForFiles(xmlDoc.SelectNodes("/root/*"));
                foreach (string file in files)
                {
                    List<string> contents = ReadPackageContents(dir, file);
                    validFiles.AddRange(contents);
                }
            }
            return validFiles;
        }
        static List<string> ReadNodesForFiles(XmlNodeList nodes)
        {
            List<string> validFiles = new() { };
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "includefile")
                {
                    validFiles.Add(node.Attributes.GetNamedItem("source").Value);
                }
                else if (node.Name == "script" || node.Name == "icon")
                {
                    string value = node.Attributes.GetNamedItem("file")?.Value ?? null;
                    if (value != null)
                    {
                        validFiles.Add(value);
                    }
                }
                validFiles.AddRange(ReadNodesForFiles(node.ChildNodes));
            }
            return validFiles;
        }

    }
}
