using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data
{
    /// <summary>Struct that contains the data for a schedule</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ScheduleData
    {
        ShowData Show;
        string EpisodeFile;
        TimeSpan StartsAiring;
        bool UniqueEpisode;
        bool ToAd;
        bool FromAd;
        bool AdInterupts;//should the currently playing show have it's playback time be stored to continue from after the ads are done?

		/// <param name="show">Which show will have an episode played</param>
		/// <param name="episodeFile">The file to play</param>
		/// <param name="startsAiring">When the Episode starts airing</param>
		/// <param name="seperateScheduleBlock">Whether this show should create a seperate block on the schedule or be connected to the current schedule block</param>
		/// <param name="toAds">If the file indicates that ads will be playing after this</param>
		/// <param name="fromAds">If the file indicates that the episode will be playing after this</param>
		/// <param name="InteruptsShow">If this file pauses the currently playing episode or not</param>
        public ScheduleData(ShowData show, string episodeFile, TimeSpan startsAiring, bool seperateScheduleBlock, bool toAds = false, bool fromAds = false, bool InteruptsShow = false)
        {
			this.Show = show;
			this.EpisodeFile = episodeFile;
			this.StartsAiring = startsAiring;
			this.UniqueEpisode = seperateScheduleBlock;
			this.ToAd = toAds;
			this.FromAd = fromAds;
			this.AdInterupts = InteruptsShow;
        }
        /*
         * Dictionary<ChannelData,ScheduleData[]>
        string channel_name; //will also have the channel number
        array[]:
            string show_name; //Gets the ".show" file to determine all that goes with it
            string episode_file; //handles getting the right episode and it's data
            TimeSpan starts_airing;
            bool unique_episode; //whether or not to make this a seperate block on the guide screen 
            bool To_Ad; //is this video a "we'll be right back" video
            bool From_Ad; //is this video a "back to the show" video
            bool adInterupts; //if this ad "interupts" the show or not
         */
    }
}

/*
 [
	{
	 "channel": "TestChannel", //this will get the Channel Data
	 "videos":[ //channel-startTime to channel-stopTime sized array of episodes
	  	{//show that would show up on the guide
		"show_name": "UTOPIA",
		"episode_file": "S001E0001 - Episode 1.mp4",
		"starts_airing": "17:00:00",
		"unique_episode": true, //will start a block at this timestamp on the guide
		"To_ad": false,
		"From_ad": false,
		"adInterupts": false,
		},
		{//indicator of going to ads
		"show_name": "",//no name will assume that whatever is looked for is looked for in the channel folder
		"episode_file": "1999_Nickelodeon_ToAds.mp4",
		"starts_airing": "17:59:54",
		"unique_episode": false, //will NOT start a block at this timestamp on the guide
		"To_ad": true, //will know to look in the "ToAds" folder in the channel's content folder
		"From_ad": false, 
		"adInterupts": false,
		},
		{
		"show_name": "nickAds", //knows it's an ad from here
		"episode_file": "1999_Nickelodeon_Advertisements-EscapeToColor.mp4",
		"starts_airing": "17:59:59",
		"unique_episode": false,
		"To_ad": false,
		"From_ad": false,
		"adInterupts": false,
		},
		{
		"show_name": "",
		"episode_file": "1999_Nickelodeon_FromAds.mp4",
		"starts_airing": "18:00:09",
		"unique_episode": false,
		"To_ad": false,
		"From_ad": true, //will know to look in the "ToAds" folder in the channel's content folder
		"adInterupts": false,
		}
		{
		"show_name": "UTOPIA",
		"episode_file": "S001E0002 - Episode 2.mp4",
		"starts_airing": "18:00:14",
		"unique_episode": true, //will start a block at this timestamp on the guide, closing the previous one before it
		"To_ad": false,
		"From_ad": false,
		"adInterupts": false,
		}
	  ]
	}
]
 */
