using OTS.Util;
using System;
using System.Runtime.InteropServices;

namespace OTS.Data
{
    /// <summary>Struct that contains the data for a show</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ShowData
    {
        /// <summary>File extension associated with a <see cref="ShowData"/> file</summary>
        public const string FILETYPE = ".shw";
        /// <summary> Long file extension associated with a <see cref="ShowData"/> file</summary>
        public const string FILETYPE_LONG = ".show";
        /// <summary>Default folder for data files and content folders</summary>
        public const string DEFAULT_FOLDER = ".shows";

        private const int MAX_NAME_LENGTH = 128;
        private const int MAX_SUMMARY_LENGTH = 320;//Average length I could find plus a bit extra just in case. (256+64)

        /// <summary>Name of the show</summary>
        public string Name { get; set; }
        /// <summary>Path to the show's default thumbnail</summary>
        public string ThumbnailFile { get; set; }
        /// <summary>Summary of the show</summary>
        public string Summary { get; set; }
        ///<summary>Base path for all content of this show</summary>
        public string ContentPath { get; set; }
        /// <summary>Age rating for the show or movie</summary>
        public AgeRating Rating { get; set; }
        /// <summary>Time that this show starts airing for the day.</summary>
        ///<remarks>If this value is the same as <see cref="EndTime"/>, show will be airable during the entire channel airtime.</remarks>
        public TimeSpan StartTime { get; set; }
        /// <summary>Time that this show ends airing for the day.</summary>
        ///<remarks>If this value is the same as <see cref="StartTime"/>, show will be airable during the entire channel airtime.</remarks>
        public TimeSpan EndTime { get; set; }
        /// <summary>Is this show all ads?</summary>
        public bool IsAds { get; set; }//TODO: add "isPromotional" bool, used for if the ads are promoting a movie, then also add the name of the movie (or make that part of the summary?)
        ///<summary>Is this show promotion for another show? Only applies when <see cref="IsAds"/> is true</summary>
        public bool IsPromotional { get; set; }
        /// <summary>Is this show a movie (single file or multi-part)?</summary>
        public bool IsMovie { get; set; }
        /// <summary>Does this show have seasonal specials?</summary>
        public bool HasSeasonal { get; set; }//TODO: include in IsAds request
        /// <summary>Importance of episode sequentiality</summary>
        public SequentialImportance EpisodeImportance { get; set; }

        /// <summary>Struct that contains the data for a show</summary>
        /// <param name="name">The Name of the show (max length 128 characters)</param>
        /// <param name="thumbnailFile">The filename of the show's thumbnail</param>
        /// <param name="summary">Summary of the show (max length 320 characters)</param>
        /// <param name="basePath">base directory for this show's content (video's thumbnails, etc.)</param>
        /// <param name="rating">Age rating for the show</param>
        public ShowData(string name, string thumbnailFile, string summary, string basePath, AgeRating rating, SequentialImportance episodeImportance = SequentialImportance.ALL_IN_ORDER, TimeSpan start = default, TimeSpan end = default, bool isAds = false, bool isPromotional = false, bool isMovie = false, bool hasSeasonal = false)
        {
            this.Name = name;
            this.ThumbnailFile = thumbnailFile;
            this.Summary = summary;
            this.ContentPath = System.IO.Path.Combine(basePath, name);
            this.Rating = rating;
            this.EpisodeImportance = episodeImportance;
            this.IsAds = isAds;
            this.IsPromotional = isPromotional;
            this.IsMovie = isMovie;
            this.HasSeasonal = hasSeasonal;
            this.StartTime = start == default ? TimeSpan.Zero : start;
            this.EndTime = end == default ? TimeSpan.Zero : end;
        }

        public string ShowNameAndProperty()
        {
            string showType;
            if (IsAds == true)
            {
                showType = "Ads";
            }
            else if (IsMovie == true)
            {
                showType = "Movie";
            }
            else
            {
                showType = "show";
            }
            return $"({showType}){Name}";
        }

        public static ShowData Default => new ShowData(string.Empty, string.Empty, string.Empty, string.Empty, default);

        public static ShowData TestShow()
        {
            return new ShowData("Test Show", "cover.jpg", "Lorem ipsum dolor sit amet.", System.AppDomain.CurrentDomain.BaseDirectory, AgeRating.NOT_RATED);
        }

        public override string ToString()
        {
            return $"\"{Name}\", Rated {Rating}, is ads:{IsAds}, is movie:{IsMovie}, has specials:{HasSeasonal}. Summary: \"{Summary}\"";
        }
    }
}