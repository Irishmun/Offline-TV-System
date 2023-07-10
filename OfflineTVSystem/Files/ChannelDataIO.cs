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

        public override ChannelData GetData(string path)
        {
            if (File.Exists(path) == false)
            { throw new FileNotFoundException(); }
            string json = ReadFileContents(path);
            ChannelData res = JsonSerializer.Deserialize<ChannelData>(json);
            return res;
        }

    }
}