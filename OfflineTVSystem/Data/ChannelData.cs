using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace OTS.Data
{
    /// <summary>Struct that contains the data for a channel</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ChannelData
    {
        /// <summary>File extension associated with a <see cref="ChannelData"/> file</summary>
        public const string FILETYPE = ".cnl";
        /// <summary> Long file extension associated with a <see cref="ChannelData"/> file</summary>
        public const string FILETYPE_LONG = ".channel";//or use .chnl
        /// <summary>Default folder for data files and content folders</summary>
        public const string DEFAULT_FOLDER = ".channels";

        private const int MAX_NAME_DATA_LENGTH = 200;//100 characters, 2 bytes per character. "Why don't you just switch off your television set and go and do something less boring instead?"

        /// <summary>Number that this channel will air on, if two channels have the same number, they will air according to <see cref="StartTime"/> and <see cref="EndTime"/>.
        /// If there are times where no channel is airing, will show blank instead</summary>
        public short Number { get; set; }
        /// <summary>Name of the channel</summary>
        public string Name { get; set; }
        /// <summary>What type of content this channel airs.</summary>
        public string ChannelType { get; set; }//maybe change to enum as well (maybe even just leave out)
        /// <summary>Time that this channel starts airing for the day.</summary>
        ///<remarks>If this value is the same as <see cref="EndTime"/>, channel will be treated as 24 hour channel.
        ///This value takes priority over <see cref="EndTime"/>, if another channel with the same <see cref="Number"/> ends airing when this one would have started, 
        ///it will instead end directly when this starts airing.</remarks>
        public TimeSpan StartTime { get; set; }
        /// <summary>Time that this channel ends airing for the day.</summary>
        ///<remarks>///If <see cref="EndTime"/> is earlier than <see cref="StartTime"/>, it is assumed that airing will end on the next day.
        ///<see cref="StartTime"/> takes priority, if a startingtime from a different channel with the same <see cref="Number"/> 
        ///is earlier than this value, this channel will stop airing at that time instead.</remarks>
        public TimeSpan EndTime { get; set; }
        /// <summary>Shows that are aired on this channel</summary>
        public HashSet<string> Shows { get; set; }
        private HashSet<ShowData> _showDatas;

        /// <summary>Struct that contains the data for a channel</summary>
        /// <param name="number">Number that this channel will air on, if two channels have the same number, they will air according to <see cref="StartTime"/> and <see cref="EndTime"/>.
        /// If there are times where no channel is airing, will show blank instead</param>
        /// <param name="name">Name of the channel</param>
        /// <param name="channelType">What type of content this channel airs.</param>
        /// <param name="start">Time that this channel starts airing for the day.</param>
        /// <param name="end">Time that this channel ends airing for the day.</param>
        /// <param name="shows">Shows that are aired on this channel</param>
        /// <remarks>Use <see cref="ChannelData.TestData()"/> or <see cref="ChannelData.TestDataWithShow()"/> to create a test show</remarks>
        public ChannelData(short number, string name, string channelType, TimeSpan start, TimeSpan end, string[] shows)
        {
            this.Number = number;
            this.Name = name;
            this.ChannelType = channelType;
            this.StartTime = start;
            this.EndTime = end;
            this.Shows = new HashSet<string>(shows);
            this._showDatas = new HashSet<ShowData>();//try and get all showDatas with the names in Shows
        }

        public static ChannelData TestData()
        {
            return new ChannelData(-1, "TestShow", "Test Channel", TimeSpan.Zero, TimeSpan.Zero, new string[0]);
        }

        public static ChannelData TestDataWithShow()
        {
            return new ChannelData(-1, "TestShow", "Test Channel", TimeSpan.Zero, TimeSpan.Zero, new string[] { ShowData.TestShow().Name });
        }

        public string GetAllShows()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ShowData show in _showDatas)
            {
                sb.AppendLine(show.Name);
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return $"{Name}({ChannelType}), channel no.{Number}, Starts at {StartTime.ToString(@"hh\:mm")} hours, Ends at {EndTime.ToString(@"hh\:mm")} hours, Has {Shows.Count} shows.";
        }
    }
}