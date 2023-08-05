using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace OTS.Data
{
    /// <summary>Struct that contians data for a show's episodes</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct EpisodeData
    {//TODO: consider putting this data in the file's filename instead, this would have to happen regardless.
     //find if there is a json or xml structure that holds this info on things like imdb

        private const string SEASON_PREFIX = "S", EPISODE_PREFIX = "E", PART_PREFIX = "P", TITLE_PREFIX = " - ";

        /// <summary>File extension associated with a <see cref="EpisodeData"/> file</summary>
        public const string FILETYPE = ".ep";
        /// <summary> Long file extension associated with a <see cref="EpisodeData"/> file</summary>
        public const string FILETYPE_LONG = ".episode";

        private const byte MAX_PATH = byte.MaxValue;//windows MAX_PATH

        /// <summary>Show that this episode belongs to</summary>
        public ShowData Show { get; private set; }//TODO: consider changing to just a string for filename
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
        /// <param filename="show">Show that this episode belongs to. (will also set <see cref="IsAd"/>)</param>
        /// <param filename="fileName">Path to the show's video file</param>
        /// <param filename="season">season number for the episode</param>
        /// <param filename="episode">episode number in the season</param>
        /// <param filename="part">part number if episode is a multi-part special (leave 0 if not multi-parter)</param>
        /// <param filename="title">title of the episode</param>
        /// <remarks>all values, except for <see cref="FileName"/> are ignored if <see cref="IsAd"/> is true</remarks>
        public EpisodeData(ShowData show, string fileName, ushort season = 1, ushort episode = 1, byte part = 0, string title = "")
        {
            this.Show = show;
            this.FileName = fileName;
            this.IsAd = show.IsAds;
            this.Season = season;
            this.Episode = episode;
            this.Part = part;
            this.Title = title;
        }


        /// <summary>Struct that contians data for a show's episodes</summary>
        /// <param filename="show">Show that this episode belongs to. (will also set <see cref="IsAd"/>)</param>
        /// <param filename="filename">string that will be seperated to the values</param>
        public EpisodeData(ShowData show, string filename)
        {

            this.Show = show;
            this.IsAd = show.IsAds;
            this.FileName = filename;
            this.Season = 1;
            this.Episode = 1;
            this.Part = 0;
            this.Title = string.Empty;

            string name = Path.GetFileNameWithoutExtension(filename);

            if (TrySeparateFileName(name, out ushort season, out ushort episode, out byte part, out string title) == true)
            {
                this.Season = season;
                this.Episode = episode;
                this.Part = part;
                this.Title = title;
            }
        }
        public override string ToString()
        {
            if (IsAd == true)
            {
                return "(ad)" + FileName;
            }
            if (Part > 0)
            {
                return $"{Show.Name}: Season {Season} Episode {Episode} Part {Part}";
            }
            return $"{Show.Name}: Season {Season} Episode {Episode}";
        }

        public string CreateFileName()
        {
            StringBuilder createdName = new StringBuilder(MAX_PATH);
            //season number ex: "S001" 
            if (Season > 1000)//Sazae-san, show with the most seasons at 214 (2023)
            { Season = 999; }
            createdName.Append(SEASON_PREFIX + Season.ToString("000"));
            //episode number ex: "E0001" 
            if (Episode > 10000)//sesame street, whilst seasoned, only uses season numbers starting from season 44
            { Season = 9999; }
            createdName.Append(EPISODE_PREFIX + Episode.ToString("0000"));
            //multiple parts, if needed //no show that I could find had any multi-parters longer than 3 parts.
            if (Part > 0)//episode has parts
            {
                createdName.Append(PART_PREFIX + Part % 10);//only use the last number, byte is always positive
            }
            //title
            createdName.Append(TITLE_PREFIX + Title);
            return createdName.ToString();
        }

        /// <summary>Tries to separate given filename into episode data</summary>
        /// <param filename="name">filename to separate</param>
        /// <param filename="season">found season value ("S001")</param>
        /// <param filename="episode">fond episode value ("E0001")</param>
        /// <param filename="part">found part value ("P0")</param>
        /// <param filename="title">remainder of string without " - " if present</param>
        /// <returns>Whether separation was successfull, returns default values if failed</returns>
        public static bool TrySeparateFileName(string name, out ushort season, out ushort episode, out byte part, out string title)
        {
            season = 1;
            episode = 1;
            part = 0;
            title = string.Empty;
            //default values, regardless of what is found
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            int curPos = 0;
            //season
            if (PrefixIsAtCurPos(SEASON_PREFIX) == true)
            {
                if (ushort.TryParse(name.Substring(curPos + SEASON_PREFIX.Length, 3), out season) == true)
                {
                    curPos += SEASON_PREFIX.Length + 3;
                }
                else
                {
                    return false;
                }
            }
            //episode
            if (PrefixIsAtCurPos(EPISODE_PREFIX) == true)
            {
                if (ushort.TryParse(name.Substring(curPos + EPISODE_PREFIX.Length, 4), out episode) == true)
                {
                    curPos += EPISODE_PREFIX.Length + 4;
                }
                else
                {
                    return false;
                }
            }
            //part
            if (PrefixIsAtCurPos(PART_PREFIX) == true)
            {
                if (byte.TryParse(name.Substring(curPos + PART_PREFIX.Length, 1), out part) == true)
                {
                    curPos += PART_PREFIX.Length + 1;
                }
            }
            //title
            if (PrefixIsAtCurPos(TITLE_PREFIX) == true)
            {//is this needed? the end of the filename is always the title, only TITLE_PREFIX would need to be removed
                title = name.Substring(curPos + TITLE_PREFIX.Length).Trim();
                return true;
            }
            return false;

            bool PrefixIsAtCurPos(string val)
            {
                return name.Substring(curPos, val.Length).Equals(val) == true;
            }
        }

        /// <summary>Returns if Filename is a valid Episode name</summary>
        /// <param name="name">name to try against (will remove file extension)</param>
        public static bool IsValidFileName(string name)
        {
            name = Path.GetFileNameWithoutExtension(name);
            return TrySeparateFileName(name, out _, out _, out _, out _);
        }
    }
}
