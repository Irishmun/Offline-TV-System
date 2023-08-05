using OTS.Data;
using System;
using System.IO;
using System.Text.Json;

namespace OTS.Files
{
    public class ChannelDataIO : DataFileBase<ChannelData>
    {
        public override int WriteToDataFile(ChannelData data, string path, string fileName = "", bool useShortFiletype = false, bool createContentFolder = true)
        {
            fileName = string.IsNullOrWhiteSpace(fileName) == true ? data.Name : fileName;
            path = Path.Combine(path, DataFileBase<ChannelData>.BASE_FOLDER, ChannelData.DEFAULT_FOLDER);
            string filetype = useShortFiletype == true ? ChannelData.FILETYPE : ChannelData.FILETYPE_LONG;
            return WriteToFile(data, path, fileName + filetype, createContentFolder);
        }

        public override bool GetData(string path, out ChannelData result)
        {
            if (File.Exists(path) == false)
            { throw new FileNotFoundException(); }
            string json = ReadFileContents(path);
            try
            {
                result = JsonSerializer.Deserialize<ChannelData>(json);
                return true;
            }
            catch (Exception)
            {
                result = ChannelData.Default;
                return false;
            }
        }

        public override string GetPath()
        {
            return Path.Combine(workingDirectory, BASE_FOLDER, ChannelData.DEFAULT_FOLDER);
        }
    }
}