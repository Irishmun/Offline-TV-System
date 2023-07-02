using System;
using System.IO;
using System.Text.Json;

namespace OTS.Files
{
    public class Files
    {
        private const int BYTES_PER_KILOBYTE = 1024;
        private const int BYTES_PER_MEGABYTE = 1048576; //1024 * 1024
        private const string BASE_FOLDER = "OTS";
        /// <summary>Writes the <see cref="ChannelData"/> to a file with the <see cref="ChannelData.FILETYPE"/> extension</summary>
        /// <param name="data">Data to write to the file, will overwrite file with the same name</param>
        /// <param name="path">Path to create the file in</param>
        /// <param name="fileName">Name to call the file, if left empty uses <see cref="ChannelData.Name"/></param>
        /// <param name="useShortFiletype">Whether to use the 3-character file extension or the longer file extension</param>
        /// <returns>The size of the created file in Kb. -1 if failed</returns>
        public int WriteToDataFile(ChannelData data, string path, string fileName = "", bool useShortFiletype = false)
        {
            fileName = string.IsNullOrWhiteSpace(fileName) == true ? data.Name : fileName;
            path = Path.Combine(path, BASE_FOLDER, ChannelData.DEFAULT_FOLDER);
            string filetype = useShortFiletype == true ? ChannelData.FILETYPE : ChannelData.FILETYPE_LONG;
            return WriteToFile(data, path, fileName + filetype);
        }

        /// <summary>Writes the <see cref="ShowData"/> to a file with the <see cref="ShowData.FILETYPE"/> extension</summary>
        /// <param name="data">Data to write to the file, will overwrite file with the same name</param>
        /// <param name="path">Path to create the file in</param>
        /// <param name="fileName">Name to call the file, if left empty uses <see cref="ShowData.Name"/></param>
        /// <param name="useShortFiletype">Whether to use the 3-character file extension or the longer file extension</param>
        /// <returns>The size of the created file in Kb. -1 if failed</returns>
        public int WriteToDataFile(ShowData data, string path, string fileName = "", bool useShortFiletype = false)
        {
            fileName = string.IsNullOrWhiteSpace(fileName) == true ? data.Name : fileName;
            path = Path.Combine(path, BASE_FOLDER, ShowData.DEFAULT_FOLDER);
            string filetype = useShortFiletype == true ? ShowData.FILETYPE : ShowData.FILETYPE_LONG;
            return WriteToFile(data, path, fileName + filetype);
        }

        /// <summary>Writes the binary data to the given file at path</summary>
        /// <param name="data">data to write</param>
        /// <param name="fullPath">full path, with filename and extension (ex: C:\Windows\System32\cmd.exe)</param>
        /// <param name="createSubdirectory">Whether to create a subfolder with the name of the given file</param>
        /// <returns>The filepath that was created</returns>
        private int WriteToFile(object data, string destination, string filename, bool createSubdirectory = true)
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
            return (int)Math.Ceiling(((decimal)fi.Length / (decimal)BYTES_PER_KILOBYTE));
        }
    }
}