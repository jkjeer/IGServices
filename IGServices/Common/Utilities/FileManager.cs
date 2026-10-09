using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IGServices.Common.Utilities
{
    public class FileManager
    {
        public static List<FileInfo> GetFiles(string directory, string extension = null)
        {
            FileInfo[] files;
            try
            {
                var dir = new DirectoryInfo(directory);
                files = dir.GetFiles();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return files.ToList();
        }

        public static string CreateFolder(string parentPath, string folderName)
        {
            string pathString;
            try
            {
                pathString = System.IO.Path.Combine(parentPath, folderName);
                if (!Directory.Exists(pathString))
                {
                    System.IO.Directory.CreateDirectory(pathString);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return pathString;
        }

        public static bool CopyFile(string sourceFolderPath, string destFolderPath, string fileName)
        {            
            try
            {
                if (!Directory.Exists(destFolderPath))
                {
                    System.IO.Directory.CreateDirectory(destFolderPath);
                }
                File.Copy(Path.Combine(sourceFolderPath, fileName), Path.Combine(destFolderPath, fileName), true);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static bool DeleteFile(string fullFilePath)
        {
            try
            {
                File.Delete(fullFilePath);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
