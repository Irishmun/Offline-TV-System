using OTS.Rating;
using System.Data;

namespace OTS.Files
{
    public struct ShowData
    {
        /// <summary>File extension associated with a <see cref="ShowData"/> file</summary>
        public const string FILETYPE = "shw";
        /// <summary> Long file extension associated with a <see cref="ShowData"/> file</summary>
        public const string FILETYPE_LONG = "show";
        private const int MAX_NAME_LENGTH = 100; //"Why don't you just switch off your television set and go and do something less boring instead?"
        private const int MAX_SUMMARY_LENGTH = 300;//Average length I could find plus a bit extra just in case.

        /// <summary>Name of the show</summary>
        public string Name { get; set; }
        /// <summary>Path to the show's default thumbnail</summary>
        public string ThumbnailPath { get; set; }
        /// <summary>Summary of the show</summary>
        public string Summary { get; set; }
        /// <summary>Age rating for the show or movie</summary>
        public AgeRating Rating { get; set; }
    }
}