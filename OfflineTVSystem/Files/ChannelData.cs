using OTS.Rating;
using System;

namespace OTS.Files
{
    public struct ChannelData
    {
        /// <summary>File extension associated with a <see cref="ChannelData"/> file</summary>
        public const string FILETYPE = "cnl";
        /// <summary> Long file extension associated with a <see cref="ChannelData"/> file</summary>
        public const string FILETYPE_LONG = "chnl";

        private const int MAX_NAME_DATA_LENGTH = 200;//100 characters, 2 bytes per character. "Why don't you just switch off your television set and go and do something less boring instead?"

        /// <summary>Number that this channel will air on, if two channels have the same number, they will air according to <see cref="StartTime"/> and <see cref="EndTime"/>.
        /// If there are times where no channel is airing, will show blank instead</summary>
        public short Number { get; set; }
        /// <summary>Name of the channel</summary>
        public string Name { get; set; }
        /// <summary>What type of content this channel airs.</summary>
        public string ChannelType { get; set; }//maybe change to enum as well (maybe even just leave out)
        /// <summary>Time that this channel starts airing for the day. </summary>
        ///<remarks>If this value is the same as <see cref="EndTime"/>, channel will be treated as 24 hour channel.
        ///This value takes priority over <see cref="EndTime"/>, if another channel with the same <see cref="Number"/> ends airing when this one would have started, 
        ///it will instead end directly when this starts airing.</remarks>
        public TimeSpan StartTime { get; set; }
        /// <summary>Time that this channel ends airing for the day. </summary>
        ///<remarks>///If <see cref="EndTime"/> is earlier than <see cref="StartTime"/>, it is assumed that airing will end on the next day.
        ///<see cref="StartTime"/> takes priority, if a startingtime from a different channel with the same <see cref="Number"/> 
        ///is earlier than this value, this channel will stop airing at that time instead.</remarks>
        public TimeSpan EndTime { get; set; }
        /// <summary>Shows that are aired on this channel</summary>
        public ShowData[] Shows { get; set; }//consider changing this to list for easier addition/removal of shows
    }
}