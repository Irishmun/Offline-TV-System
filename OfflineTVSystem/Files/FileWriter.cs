namespace OTS.Files
{
    public class FileWriter
    {
        /// <summary>Writes the <see cref="ChannelData"/> to a file with the <see cref="ChannelData.FILETYPE"/> extension</summary>
        /// <param name="data">Data to write to the file, will overwrite file with the same name</param>
        /// <param name="path">Path to create the file in</param>
        /// <param name="fileName">Name to call the file, if left empty uses <see cref="ChannelData.Name"/></param>
        public void WriteToDataFile(ChannelData data, string path, string fileName = "")
        {

        }

        /// <summary>Writes the <see cref="ShowData"/> to a file with the <see cref="ShowData.FILETYPE"/> extension</summary>
        /// <param name="data">Data to write to the file, will overwrite file with the same name</param>
        /// <param name="path">Path to create the file in</param>
        /// <param name="fileName">Name to call the file, if left empty uses <see cref="ShowData.Name"/></param>
        public void WriteToDataFile(ShowData data, string path, string fileName = "")
        {

        }
    }
}
