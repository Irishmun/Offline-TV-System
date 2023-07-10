using OTS.Data;
using OTS.Files;
using OTS.Rating;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OTS_Console
{
    internal class CommandHandler
    {
        private ChannelDataIO _channelDataIO = new ChannelDataIO();
        private ShowDataIO _showDataIO = new ShowDataIO();
        private string _workingDirectory = AppDomain.CurrentDomain.BaseDirectory;

        internal void ProcessArgument(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                ProvideHelp();
                Console.WriteLine("Press any key to quit...");
                Console.ReadKey();
                return;
            }

            string command = args[0];
            switch (command.ToLower())
            {
                case COMMAND_VERSION:
                case COMMAND_VERSION_SHORT:
                    Console.Write("Offline Tv System version: " + Assembly.GetExecutingAssembly().GetName().Version);
#if DEBUG
                    Console.WriteLine("-Debug");
#else
                    Console.WriteLine(" - Release");
#endif
                    break;
                case COMMAND_HELP:
                case COMMAND_HELP_SHORT:
                    ProvideHelp();
                    break;
                case COMMAND_SETUP:
                    //Start setup, create folders and stuff
                    break;
                case COMMAND_LISTSHOWS:
                    //list all shows in given path OR in current directory if no path is given (recursive)
                    break;
                case COMMAND_LISTCHANNELS:
                    //list all channels in given path OR in current directory if no path is given (recursive)
                    break;
                case COMMAND_LISTSCHEDULES:
                    //list all schedules in given path OR in current directory if no path is given(recursive)
                    break;
                case COMMAND_CREATESHOW:
                    //create new show
                    CreateShow();
                    break;
                case COMMAND_CREATECHANNEL:
                    //create new channel
                    CreateChannel();
                    break;
                case COMMAND_CREATESCHEDULE:
                    //create new schedule
                    break;
                case COMMAND_AUTOSCHEDULE:
                    //generate a schedule with the existing shows and channel data
                    break;
                case COMMAND_EDITSHOW:
                    //edit the given show file, first pass the show name, then the path it's in (defaults to current directory .shows/)
                    if (args.Length >= 2)
                    {
                        if (_showDataIO.GetExistingPath(Path.Combine(_workingDirectory, ShowDataIO.BASE_FOLDER, ShowData.DEFAULT_FOLDER), out string path, args[1] + ShowData.FILETYPE, args[1] + ShowData.FILETYPE_LONG) == true)
                        {
                            ShowData data = _showDataIO.GetData(path);
                            EditShow(data);
                        }
                        else
                        {
                            Console.WriteLine($"Unable to find show \"{args[1]}\", please check spelling and try again.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Usage: {COMMAND_EDITSHOW} [Show name]");
                    }
                    break;
                case COMMAND_EDITCHANNEL:
                    if (args.Length >= 2)
                    {
                        if (_channelDataIO.GetExistingPath(Path.Combine(_workingDirectory, ChannelDataIO.BASE_FOLDER, ChannelData.DEFAULT_FOLDER), out string path, args[1] + ShowData.FILETYPE, args[1] + ShowData.FILETYPE_LONG) == true)
                        {
                            ChannelData data = _channelDataIO.GetData(path);
                            EditChannel(data);
                        }
                        else
                        {
                            Console.WriteLine($"Unable to find show \"{args[1]}\", please check spelling and try again.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Usage: {COMMAND_EDITSHOW} [Show name]");
                    }
                    //edit the given channel file, first pass the channel name, then the path it's in (defaults to current directory .channels/)
                    break;
                case COMMAND_EDITSCHEDULE:
                    //TODO: consider changing this to a forced update command instead, re-generating the (given) schedule(s) instead of manually editing them
                    //edit the given schedule file, first pass the schedule name, then the path it's in (defaults to current directory schedules/)
                    break;
                case COMMAND_SHOWINFO:
                    //provide info on given show name (if found)
                    break;
                case COMMAND_CHANNELINFO:
                    //provide info on given channel name (if found)
                    break;
                case COMMAND_SCHEDULEINFO:
                    //provide info on given schedule name (if found)
                    break;
                case COMMAND_AGERATINGS:
                    ShowAllRatings();
                    break;
                default:
                    Console.WriteLine($"Command \"{command}\" not recognized, type \"{COMMAND_HELP}\" or \"{COMMAND_HELP_SHORT}\" for help");
                    break;
            }
        }
        #region command methods
        void ProvideHelp()
        {
            Console.WriteLine("Here's a list of all available commands:\n" +
            $"{COMMAND_VERSION} {COMMAND_VERSION_SHORT}: Get the application version.\n" +
            $"{COMMAND_HELP} {COMMAND_HELP_SHORT}: Show all available commands.\n" +
            $"{COMMAND_SETUP}: Start program setup.\n" +
            "\n" +
            $"{COMMAND_LISTSHOWS}: Lists all shows at path (recursive). defaults to current directory\n" +
            $"{COMMAND_LISTCHANNELS}: Lists all channels at path (recursive). defaults to current directory\n" +
            $"{COMMAND_LISTSCHEDULES}: Lists all schedules at path (recursive). defaults to current directory\n" +
            "\n" +
            $"{COMMAND_CREATESHOW}: Creates or updates a show.\n" +
            $"{COMMAND_CREATECHANNEL}: Creates or updates a channel.\n" +
            $"{COMMAND_CREATESCHEDULE}: Creates a schedule file.\n" +
            $"{COMMAND_AUTOSCHEDULE}: Generates a schedule file with the existing shows and channels.\n" +
            "\n" +
            $"{COMMAND_EDITSHOW}: Edit an existing show.\n" +
            $"{COMMAND_EDITCHANNEL}: Edit an existing channel.\n" +
            $"{COMMAND_EDITSCHEDULE}: Edit a schedule.\n" +
            "\n" +
            $"{COMMAND_SHOWINFO}: Shows the info of the given show.\n" +
            $"{COMMAND_CHANNELINFO}: Shows the info of the given channel.\n" +
            $"{COMMAND_AGERATINGS}: Show all available age ratings."
            );
        }

        void CreateShow()
        {
            string name = "show";
            string thumbnailPath = "cover.jpg";
            string summary = string.Empty;
            AgeRating rating = AgeRating.NOT_RATED;
            bool isAds = false, isMovie = false, hasSpecials = false;
            string? output = string.Empty;//used for console output when using ReadLine()

            Console.WriteLine("Creating a new show... (Brackets is default value in capitals)");
            //show name
            Console.Write($"Please provide the show name [\"{name}\"]: ");
            output = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(output) == false)
            {
                name = output;
                thumbnailPath = Path.Combine(name, thumbnailPath);
            }
            //thumbnail
            Console.Write($"Please provide the path to the show's thumbnail [{ShowData.DEFAULT_FOLDER}\\{thumbnailPath}]: ");
            output = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(output) == false)
            {
                thumbnailPath = Path.Combine(name, output);
            }
            //summary
            Console.Write($"Please provide a summary to the show [\"{summary}\"]: ");
            output = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(output) == false)
            {
                summary = output;
            }
            //rating
            Console.Write($"Please provide either the name or id of the rating according to the Age ratings enum ({COMMAND_AGERATINGS}) [{rating}]: ");
            output = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(output) == false)
            {
                if (Enum.TryParse(typeof(AgeRating), output, true, out object result) == true)
                {
                    rating = (AgeRating)result;
                }
                else
                {
                    Console.WriteLine($"Unable to parse Age rating, adjust it if needed with {COMMAND_EDITSHOW}...");
                }
            }
            //is ads?
            Console.Write("Is this show all ads? [y/N]: ");
            output = Console.ReadLine();
            if (output.ToLower().StartsWith("y") == true)
            {
                isAds = true;
            }
            if (isAds == false)
            {

                //is movie?
                Console.Write("Is this show a movie, single or multi-part? [y/N]: ");
                output = Console.ReadLine();
                if (output.ToLower().StartsWith("y") == true)
                {
                    isMovie = true;
                }
                //has specials?
                Console.Write("Does this show have specials? (multi-part, seasonal, etc.) [y/N]: ");
                output = Console.ReadLine();
                if (output.ToLower().StartsWith("y") == true)
                {
                    hasSpecials = true;
                }
            }
            else
            {
                Console.WriteLine("Defaulting IsMovie and HasSpecials to false...");
                isMovie = false;
                hasSpecials = false;
            }
            ShowData show = new ShowData(name, thumbnailPath, summary, rating, isAds, isMovie, hasSpecials);
            Console.WriteLine("Writing to data file...");
            int size = _showDataIO.WriteToDataFile(show, _workingDirectory);
            Console.WriteLine($"({size}KB) Created {show}");
        }
        void CreateChannel()
        {
            Console.WriteLine("Creating a new channel... (Brackets is default value)");
            short number = 0;
            string name = "Channel", channelType = "shows";
            TimeSpan start = TimeSpan.Zero, end = TimeSpan.Zero;
            List<string> shows = new List<string>();
            string? output = string.Empty;//used for console output when using ReadLine()
            //channel name
            Console.Write($"Please provide the channel name [\"{name}\"]: ");
            output = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(output) == false)
            {
                name = output;
            }
            //channel number
            Console.Write($"Please provide the channel number [{number}]: ");
            output = Console.ReadLine();
            if (string.IsNullOrEmpty(output) == false)
            {
                if (short.TryParse(output, out short val) == false)
                {
                    Console.WriteLine("Unable to parse, resorting to default...");
                }
                else
                {
                    number = val;
                }
            }
            //channel type
            Console.Write($"Please provide what type of channel this is [\"{channelType}\"]: ");
            output = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(output) == false)
            {
                channelType = output;
            }
            //start time
            Console.Write("Please provide the STARTING time for the channel in the 24 hour format \"hh:mm\" (default: 00:00): ");
            output = Console.ReadLine();
            if (string.IsNullOrEmpty(output) == false)
            {
                if (TimeSpan.TryParse(output, out TimeSpan val) == false)
                {
                    Console.WriteLine("Unable to parse, hours can only be between 0-23. resorting to default...");

                }
                else
                {
                    start = val;
                }
            }
            //end time
            Console.Write("Please provide the ENDING time for the channel in the 24 hour format format \"hh:mm\" (default: 00:00): ");
            output = Console.ReadLine();
            if (string.IsNullOrEmpty(output) == false)
            {
                if (TimeSpan.TryParse(output, out TimeSpan val) == false)
                {
                    Console.WriteLine("Unable to parse, hours can only be between 0-23. resorting to default...");

                }
                else
                {
                    end = val;
                }
            }
            //shows
            Console.WriteLine("Please provide the names of the shows to add to this channel, separated by \"|\" [no shows]: ");
            string[] showNames = Console.ReadLine().Split("|");//TODO: figure out how to have this separate like normal arguments (space separated unless between quotes)
            if (showNames.Length > 0)
            {
                bool found = false;
                for (int i = 0; i < showNames.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(showNames[i]) == true)
                    { continue; }
                    Console.Write($"Looking for show \"{showNames[i]}\"... ");
                    //try and get the shows by that name
                    if (found == true)
                    {
                        Console.WriteLine("Found! adding...");
                    }
                    else
                    {
                        //will still add, in case someone uses this on a separate machine than the one with the actual shows
                        Console.WriteLine("NOT Found! check spelling, still adding...");
                    }
                    shows.Add(showNames[i]);
                }
            }
            ChannelData channel = new ChannelData(number, name, channelType, start, end, shows.ToArray());
            Console.WriteLine("Writing to data file...");
            int size = _channelDataIO.WriteToDataFile(channel, _workingDirectory);
            Console.WriteLine($"({size}KB) Created {channel}");
        }

        void EditShow(ShowData showToChange)
        {
            string name = showToChange.Name;
            string thumbnailPath = showToChange.ThumbnailPath;
            string summary = showToChange.Summary;
            AgeRating rating = showToChange.Rating;
            bool isAds = showToChange.IsAds, isMovie = showToChange.IsMovie, hasSpecials = showToChange.HasSpecials;
            string? output = string.Empty;//used for console output when using ReadLine()

            Console.WriteLine($"Editing {showToChange.Name}... (Brackets is default value)");
            //show name
            Console.Write($"Name [{name}]: ");
            output = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(output) == false)
            {
                name = output;
            }
            //thumbnail
            Console.Write($"Thumbnail Name [{name}\\{Path.GetFileName(thumbnailPath)}]: ");
            output = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(output) == false)
            {
                thumbnailPath = Path.Combine(name, output);
            }
            else
            {
                thumbnailPath = Path.Combine(name, Path.GetFileName(thumbnailPath));
            }
            //summary
            Console.Write($"Summary, leave empty to use existing summary: ");
            output = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(output) == false)
            {
                summary = output;
            }
            //rating
            Console.Write($"Age rating [{rating}]: ");
            output = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(output) == false)
            {
                if (Enum.TryParse(typeof(AgeRating), output, true, out object result) == true)
                {
                    rating = (AgeRating)result;
                }
                else
                {
                    Console.WriteLine($"Unable to parse Age rating, adjust it if needed with {COMMAND_EDITSHOW}...");
                }
            }
            //[Y/n]
            //is ads?
            if (isAds == true)
            {
                Console.Write("Is ads [Y/n]: ");
            }
            else
            {
                Console.Write("Is ads [y/N]: ");
            }
            output = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(output) == false && output.ToLower().StartsWith("y") == true)
            {
                isAds = true;
            }
            else if (string.IsNullOrWhiteSpace(output) == false && output.ToLower().StartsWith("n") == true)
            {
                isAds = false;
            }
            if (isAds == false)
            {
                //is movie?
                if (isMovie == true)
                {
                    Console.Write("Is movie [Y/n]: ");
                }
                else
                {
                    Console.Write("Is movie [y/N]: ");
                }
                output = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(output) == false && output.ToLower().StartsWith("y") == true)
                {
                    isMovie = true;
                }
                else if (string.IsNullOrWhiteSpace(output) == false && output.ToLower().StartsWith("n") == true)
                {
                    isMovie = false;
                }
                //has specials?
                if (hasSpecials == true)
                {
                    Console.Write("Has specials [Y/n]: ");
                }
                else
                {
                    Console.Write("Has specials [y/N]: ");
                }
                output = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(output) == false && output.ToLower().StartsWith("y") == true)
                {
                    hasSpecials = true;
                }
                else if (string.IsNullOrWhiteSpace(output) == false && output.ToLower().StartsWith("n") == true)
                {
                    hasSpecials = false;
                }
            }
            ShowData show = new ShowData(name, thumbnailPath, summary, rating, isAds, isMovie, hasSpecials);
            Console.WriteLine("Updating data file...");
            int size = _showDataIO.WriteToDataFile(show, _workingDirectory, createContentFolder: false);
            Console.WriteLine($"({size}KB) Updated {show}");
            //if name changes, move all over to other folder
            if (name.Equals(showToChange.Name) == false)
            {
                _showDataIO.GetExistingPath(Path.Combine(_workingDirectory, ShowDataIO.BASE_FOLDER, ShowData.DEFAULT_FOLDER), out string oldDataFile, showToChange.Name + ShowData.FILETYPE, showToChange.Name + ShowData.FILETYPE_LONG);
                _showDataIO.DeleteDataFile(oldDataFile);
                Console.WriteLine($"Changing show folder name {showToChange.Name} > {name}...");
                string oldPath = Path.Combine(_workingDirectory, ShowDataIO.BASE_FOLDER, ShowData.DEFAULT_FOLDER, showToChange.Name);
                string newPath = Path.Combine(_workingDirectory, ShowDataIO.BASE_FOLDER, ShowData.DEFAULT_FOLDER, name);
                Directory.Move(oldPath, newPath);
                Console.WriteLine($"Changed show folder name.");
            }
        }

        void EditChannel(ChannelData channel)
        {

        }
        void ShowAllRatings()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("The following age ratings are available: ");
            str.AppendLine(string.Format("|{0,-10}|{1,-4}|", "name", "id"));
            str.AppendLine(string.Format("|{0,-10}|{1,-4}|", "----------", "----"));
            AgeRating[] ratings = (AgeRating[])Enum.GetValues(typeof(AgeRating));
            for (int i = 0; i < ratings.Length; i++)
            {
                str.AppendLine(string.Format("|{0,-10}|{1,-4}|", ratings[i], ratings[i].ToString("D")));
            }

            Console.WriteLine(str.ToString());
        }
        #endregion
        #region special methods

        #endregion
        #region Command names
        internal const string COMMAND_VERSION = "--version", COMMAND_VERSION_SHORT = "-v";
        internal const string COMMAND_HELP = "--help", COMMAND_HELP_SHORT = "-h";
        internal const string COMMAND_LISTSHOWS = "--listshows";
        internal const string COMMAND_LISTCHANNELS = "--listchannels";
        internal const string COMMAND_LISTSCHEDULES = "--listschedules";
        internal const string COMMAND_CREATESHOW = "--createshow";
        internal const string COMMAND_CREATECHANNEL = "--createchannel";
        internal const string COMMAND_CREATESCHEDULE = "--createschedule";
        internal const string COMMAND_AUTOSCHEDULE = "--autoschedule";
        internal const string COMMAND_EDITCHANNEL = "--editchannel";
        internal const string COMMAND_EDITSHOW = "--editshow";
        internal const string COMMAND_EDITSCHEDULE = "--editschedule";
        internal const string COMMAND_SHOWINFO = "--showinfo";
        internal const string COMMAND_CHANNELINFO = "--channelinfo";
        internal const string COMMAND_SCHEDULEINFO = "--scheduleinfo";
        internal const string COMMAND_SETUP = "--setup";
        internal const string COMMAND_AGERATINGS = "--ageratings";
        #endregion
    }
}
