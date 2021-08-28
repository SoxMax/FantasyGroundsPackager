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
            string rootDir = @"C:\Users\User\source\repos\FantasyGroundsPackager\FG-PFRPG-Live-Hitpoints";
            FileAttributes rootDirAttributes = File.GetAttributes(rootDir);
            rootDirAttributes.HasFlag(FileAttributes.Directory);
            XmlDocument extensionFile = new XmlDocument();
            List<string> validFiles = new List<string> { "extension.xml" };
            extensionFile.Load($@"{rootDir}\extension.xml");
            XmlNodeList includes = extensionFile.SelectNodes("/root/base/*");
            foreach (XmlNode fileNode in includes)
            {
                if(fileNode.Name == "includefile") {
                    validFiles.Add(fileNode.Attributes.GetNamedItem("source").Value);
                } else if(fileNode.Name == "script" || fileNode.Name == "icon") {
                    validFiles.Add(fileNode.Attributes.GetNamedItem("file").Value);
                }
            }
            
            using (ZipArchive archive = ZipFile.Open($@"{rootDir}\release.zip", ZipArchiveMode.Update))
            {
                foreach (string file in validFiles)
                {
                    archive.CreateEntryFromFile($@"{rootDir}\{file}", file);
                }
            }
        }
    }
}
