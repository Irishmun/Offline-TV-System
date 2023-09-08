using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace OTS.Files
{
    public abstract class DataFileBase<T>
    {
        public const int BYTES_PER_KILOBYTE = 1024;
        public const int BYTES_PER_MEGABYTE = 1048576; //1024 * 1024
        public const string BASE_FOLDER = "OTS";

        protected string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>Writes the <see cref="Data.ChannelData"/> to a file with the <see cref="Data.ChannelData.FILETYPE"/> extension</summary>
        /// <param name="data">Data to write to the file, will overwrite file with the same name</param>
        /// <param name="path">Path to create the file in</param>
        /// <param name="fileName">Name to call the file, if left empty uses <see cref="Data.ChannelData.Name"/></param>
        /// <param name="useShortFiletype">Whether to use the 3-character file extension or the longer file extension</param>
        /// <param name="createContentFolder">Should a Content folder be created?</param>
        /// <returns>The size of the created file in Kb. -1 if failed</returns>
        public abstract int WriteToDataFile(T data, string path, string fileName = "", bool useShortFiletype = false, bool createContentFolder = true);

        /// <summary>Returns the content from the given file as <typeparamref name="T"/></summary>
        /// <param name="path">path to the file to get data from</param>
        /// <returns><typeparamref name="T"/> with the data from file</returns>
        /// <remarks>note that, if a property name changes, its value will be ignored</remarks>
        public abstract bool GetData(string path, out T result);

        /// <summary>Returns the relative filepath from the application to the folder</summary>
        public abstract string GetPath();

        /// <summary>Returns first found file path if any of the given filenames exists at their shared path.</summary>
        /// <param name="sharedPath">path that all possible files share</param>
        /// <param name="existingPath">resulting path if any is existing</param>
        /// <param name="possibleFileNames">possible filenames that are to be searched</param>
        /// <returns>first found file path</returns>
        public bool GetExistingPath(string sharedPath, out string existingPath, params string[] possibleFileNames)
        {
            existingPath = string.Empty;
            if (possibleFileNames.Length < 1)
            { return false; }//nothing found regardless

            string possiblePath;

            for (int i = 0; i < possibleFileNames.Length; i++)
            {
                //check each path until an existing one is found
                possiblePath = Path.Combine(sharedPath, possibleFileNames[i]);
                if (File.Exists(possiblePath) == true)
                {
                    existingPath = possiblePath;
                    return true;
                }
            }
            //none are found, return false and empty existing path
            return false;
        }

        /// <summary>Tries to delete the file at the given path</summary>
        /// <param name="path">file to delete</param>
        /// <returns>True if file doesn't exist or has been deleted successfully</returns>
        public bool DeleteDataFile(string path)
        {
            if (File.Exists(path) == false)//no need to delete, file doesn't exist
            { return true; }
            try
            {
                File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Reads file and returns contents as single string</summary>
        /// <param name="path">path of file to read</param>
        /// <returns>string of file contents</returns>
        /// <exception cref="FileNotFoundException"></exception>
        protected string ReadFileContents(string path)
        {
            if (File.Exists(path) == false)
            { throw new FileNotFoundException(); }
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }


        /// <summary>Writes the binary data to the given file at path</summary>
        /// <param name="data">data to write</param>
        /// <param name="fullPath">full path, with filename and extension (ex: C:\Windows\System32\cmd.exe)</param>
        /// <param name="createSubdirectory">Whether to create a subfolder with the name of the given file</param>
        /// <returns>The filepath that was created</returns>
        protected int WriteToFile(object data, string destination, string filename, bool createSubdirectory = true)
        {
            if (Directory.Exists(destination) == false)
            {
                Directory.CreateDirectory(destination);
            }
            if (createSubdirectory == true)
            {
                Directory.CreateDirectory(Path.Combine(destination, Path.GetFileNameWithoutExtension(filename)));
            }
            string jsonString = JsonSerializer.Serialize(data);
            destination = Path.Combine(destination, filename);
            File.WriteAllText(destination, jsonString);
            FileInfo fi = new FileInfo(destination);
            if (fi.Exists == false)
            { return -1; }
#if DEBUG
            System.Diagnostics.Debug.WriteLine(destination);
#endif
            return (int)Math.Ceiling(((decimal)fi.Length / (decimal)BYTES_PER_KILOBYTE));
        }
    }
}
