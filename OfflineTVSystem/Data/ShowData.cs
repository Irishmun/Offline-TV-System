using OTS.Rating;
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
        public string ThumbnailPath { get; set; }
        /// <summary>Summary of the show</summary>
        public string Summary { get; set; }
        /// <summary>Age rating for the show or movie</summary>
        public AgeRating Rating { get; set; }
        /// <summary>Is this show all ads?</summary>
        public bool IsAds { get; set; }//TODO: add "isPromotional" bool, used for if the ads are promoting a movie, then also add the name of the movie (or make that part of the summary?)
        /// <summary>Is this show a movie (single file or multi-part)?</summary>
        public bool IsMovie { get; set; }
        /// <summary>Does this show have seasonal or multi-part specials?</summary>
        public bool HasSpecials { get; set; }//TODO: change this from "hasSpecials" to "hasSeasonal" (and include in IsAds request)

        /// <summary>Struct that contains the data for a show</summary>
        /// <param name="name">The Name of the show or movie (max length 128 characters)</param>
        /// <param name="thumbnailPath">The Path to the show's or movie's thumbnail</param>
        /// <param name="summary">Summary of the show or movie (max length 320 characters)</param>
        /// <param name="rating">Age rating for the show or movie</param>
        public ShowData(string name, string thumbnailPath, string summary, AgeRating rating, bool isAds = false, bool isMovie = false, bool hasSpecials = false)
        {
            this.Name = name;
            this.ThumbnailPath = thumbnailPath;
            this.Summary = summary;
            this.Rating = rating;
            this.IsAds = isAds;
            this.IsMovie = isMovie;
            this.HasSpecials = hasSpecials;
        }

        public static ShowData Default => new ShowData(string.Empty, string.Empty, string.Empty, default);

        public static ShowData TestShow()
        {
            return new ShowData("Test Show", System.AppDomain.CurrentDomain.BaseDirectory, "Lorem ipsum dolor sit amet.", AgeRating.NOT_RATED);
        }

        public override string ToString()
        {
            return $"\"{Name}\", Rated {Rating}, is ads:{IsAds}, is movie:{IsMovie}, has specials:{HasSpecials}. Summary: \"{Summary}\"";
        }
    }
}