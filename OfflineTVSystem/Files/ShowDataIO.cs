using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OTS.Data;

namespace OTS.Files
{
    public class ShowDataIO : DataFileBase<ShowData>
    {
        public override int WriteToDataFile(ShowData data, string path, string fileName = "", bool useShortFiletype = false, bool createContentFolder = true)
        {
            fileName = string.IsNullOrWhiteSpace(fileName) == true ? data.Name : fileName;
            path = Path.Combine(path, BASE_FOLDER, ShowData.DEFAULT_FOLDER);
            string filetype = useShortFiletype == true ? ShowData.FILETYPE : ShowData.FILETYPE_LONG;
            return WriteToFile(data, path, fileName + filetype, createContentFolder);
        }


        public override bool GetData(string path, out ShowData result)
        {
            if (File.Exists(path) == false)
            { throw new FileNotFoundException(); }
            string json = ReadFileContents(path);
            try
            {
                result = JsonSerializer.Deserialize<ShowData>(json);
                return true;
            }
            catch (Exception)
            {
                result = ShowData.Default;
                return false;
            }
        }

    }
}