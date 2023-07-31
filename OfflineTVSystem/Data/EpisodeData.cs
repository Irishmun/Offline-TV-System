using System.IO;
using System.Runtime.InteropServices;

namespace OTS.Data
{
    /// <summary>Struct that contians data for a show's episodes</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct EpisodeData
    {//TODO: consider putting this data in the file's name instead, this would have to happen regardless.
     //find if there is a json or xml structure that holds this info on things like imdb

        /// <summary>File extension associated with a <see cref="EpisodeData"/> file</summary>
        public const string FILETYPE = ".ep";
        /// <summary> Long file extension associated with a <see cref="EpisodeData"/> file</summary>
        public const string FILETYPE_LONG = ".episode";

        private const int MAX_NAME_LENGTH = 128;
        private const int MAX_SUMMARY_LENGTH = 320;//Average length I could find plus a bit extra just in case. (256+64)

        /// <summary>Show that this episode belongs to</summary>
        public ShowData Show { get; private set; }//TODO: consider changing to just a string for name
        /// <summary>Is an advertisement</summary>
        public bool IsAd { get; set; }
        /// <summary>Season that this episode is in</summary>
        public ushort Season { get; set; }
        /// <summary>Episode number</summary>
        public ushort Episode { get; set; }
        /// <summary>If this episode consists of multiple parts. will cause the schedule to play these episodes in order.</summary>
        public byte Part { get; set; }
        /// <summary>Name of the episode, ignored if a movie or ad</summary>
        public string Title { get; set; }
        /// <summary>Name of episode video file on disk</summary>
        public string FileName { get; set; }

        /// <summary>Struct that contians data for a show's episodes</summary>
        /// <param name="show">Show that this episode belongs to. (will also set <see cref="IsAd"/>)</param>
        /// <param name="fileName">Path to the show's video file</param>
        /// <param name="season">season number for the episode (ignored if <see cref="IsAd"/> is true)</param>
        /// <param name="episode">episode number in the season (ignored if <see cref="IsAd"/> is true)</param>
        /// <param name="part">part number if episode is a multi-part special (ignored if <see cref="IsAd"/> is true)</param>
        /// <param name="title">title of the episode (ignored if <see cref="IsAd"/> is true)</param>
        public EpisodeData(ShowData show, string fileName, ushort season = 1, ushort episode = 1, byte part = 1, string title = "")
        {
            this.Show = show;
            this.FileName = fileName;
            this.IsAd = show.IsAds;
            this.Season = season;
            this.Episode = episode;
            this.Part = part;
            this.Title = title;
        }

        public string folder => Path.Combine(ShowData.DEFAULT_FOLDER, Show.Name);
    }
}
