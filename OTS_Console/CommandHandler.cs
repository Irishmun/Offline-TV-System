using OTS.Data;
using OTS.Files;
using OTS.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OTS_Console
{
    internal class CommandHandler
    {
        private Regex illegalChars = new Regex("[/\\\\?%*:|\"<>]/g");
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
                    ListAllShows(Path.Combine(_workingDirectory + ShowDataIO.BASE_FOLDER));
                    break;
                case COMMAND_LISTCHANNELS:
                    //list all channels in given path OR in current directory if no path is given (recursive)
                    ListAllChannels(Path.Combine(_workingDirectory + ChannelDataIO.BASE_FOLDER));
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
                            if (_showDataIO.GetData(path, out ShowData data) == true)
                            {
                                EditShow(data);
                            }
                            else
                            {
                                Console.WriteLine($"Show file for \"{args[1]}\" is corrupted!");
                            }
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
                        if (_channelDataIO.GetExistingPath(Path.Combine(_workingDirectory, ChannelDataIO.BASE_FOLDER, ChannelData.DEFAULT_FOLDER), out string path, args[1] + ChannelData.FILETYPE, args[1] + ChannelData.FILETYPE_LONG) == true)
                        {
                            if (_channelDataIO.GetData(path, out ChannelData data) == true)
                            {
                                EditChannel(data);

                            }
                            else
                            {
                                Console.WriteLine($"Channel file for \"{args[1]}\" is corrupted!");
                            }
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
                case COMMAND_UPDATEPROGRAM:
                    //check github releases for latest version, compare to this version. update if needed.
                    //perhaps add a debug flag, update to latest debug version?
                    break;
                case COMMAND_UPDATESHOW:
                    if (args.Length >= 2)
                    {
                        if (_showDataIO.GetExistingPath(Path.Combine(_workingDirectory, ShowDataIO.BASE_FOLDER, ShowData.DEFAULT_FOLDER), out string path, args[1] + ShowData.FILETYPE, args[1] + ShowData.FILETYPE_LONG) == true)
                        {
                            if (_showDataIO.GetData(path, out ShowData data) == true)
                            {
                                UpdateShow(data, path);
                            }
                            else
                            {
                                Console.WriteLine($"Show file for \"{args[1]}\" is corrupted!");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Unable to find show \"{args[1]}\", please check spelling and try again.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Usage: {COMMAND_UPDATESHOW} [Show name]");
                    }
                    break;
                case COMMAND_UPDATECHANNEL:
                    if (args.Length >= 2)
                    {
                        if (_channelDataIO.GetExistingPath(Path.Combine(_workingDirectory, ChannelDataIO.BASE_FOLDER, ChannelData.DEFAULT_FOLDER), out string path, args[1] + ChannelData.FILETYPE, args[1] + ChannelData.FILETYPE_LONG) == true)
                        {
                            if (_channelDataIO.GetData(path, out ChannelData data) == true)
                            {
                                UpdateChannel(data, path);
                            }
                            else
                            {
                                Console.WriteLine($"Channel file for \"{args[1]}\" is corrupted!");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Unable to find show \"{args[1]}\", please check spelling and try again.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Usage: {COMMAND_UPDATECHANNEL} [Show name]");
                    }
                    break;
                case COMMAND_UPDATESCHEDULE:
                    break;
                case COMMAND_SHOWINFO:
                    //provide info on given show name (if found)
                    if (args.Length >= 2)
                    {
                        if (_showDataIO.GetExistingPath(Path.Combine(_workingDirectory, ShowDataIO.BASE_FOLDER, ShowData.DEFAULT_FOLDER), out string path, args[1] + ShowData.FILETYPE, args[1] + ShowData.FILETYPE_LONG) == true)
                        {
                            if (_showDataIO.GetData(path, out ShowData data) == true)
                            {
                                Console.WriteLine(data.ToString());
                            }
                            else
                            {
                                Console.WriteLine($"Show file for \"{args[1]}\" is corrupted!");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Unable to find show \"{args[1]}\", please check spelling and try again.");
                        }
                    }
                    break;
                case COMMAND_CHANNELINFO:
                    //provide info on given channel name (if found)
                    if (args.Length >= 2)
                    {
                        if (_channelDataIO.GetExistingPath(Path.Combine(_workingDirectory, ChannelDataIO.BASE_FOLDER, ChannelData.DEFAULT_FOLDER), out string path, args[1] + ChannelData.FILETYPE, args[1] + ChannelData.FILETYPE_LONG) == true)
                        {
                            if (_channelDataIO.GetData(path, out ChannelData data) == true)
                            {
                                Console.WriteLine(data.ToString());
                                Console.WriteLine($"{data.Name} has the following shows:");
                                Console.WriteLine(data.GetAllShows());
                            }
                            else
                            {
                                Console.WriteLine($"Channel file for \"{args[1]}\" is corrupted!");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Unable to find channel \"{args[1]}\", please check spelling and try again.");
                        }
                    }
                    break;
                case COMMAND_SCHEDULEINFO:
                    //provide info on given schedule name (if found)
                    break;
                case COMMAND_AGERATINGS:
                    ShowAllRatings();
                    break;
                case COMMAND_SEQUENTIAL:
                    ShowAllSequentials();
                    break;
                default:
                    Console.WriteLine($"Command \"{command}\" not recognized, type \"{COMMAND_HELP}\" or \"{COMMAND_HELP_SHORT}\" for help");
                    break;
            }
        }

        #region util methods
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
            $"{COMMAND_SCHEDULEINFO}: Shows the info of the given schedule.\n" +
            $"{COMMAND_AGERATINGS}: Show all available age ratings.\n" +
            $"{COMMAND_SEQUENTIAL}: Shows all available episode sequential importances."
            );
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
        private void ShowAllSequentials()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("This value is used to in what order a show's episodes should be aired.");
            str.AppendLine("The following episode sequential importance values are available: ");
            str.AppendLine(string.Format("|{0,-10}|{1,-4}|", "name", "id"));
            str.AppendLine(string.Format("|{0,-10}|{1,-4}|", "----------", "----"));
            str.AppendLine(string.Format("|{0,-10}|{1,-4}|{2}", SequentialImportance.ANY_ORDER, (int)SequentialImportance.ANY_ORDER, "Episode order does not matter, any episode of any season can be aired at any point"));
            str.AppendLine(string.Format("|{0,-10}|{1,-4}|{2}", SequentialImportance.ALL_IN_ORDER, (int)SequentialImportance.ALL_IN_ORDER, "(default)Episode order matters, all episodes will be aired in order. reruns will only be for previously aired episodes"));
            str.AppendLine(string.Format("|{0,-10}|{1,-4}|{2}", SequentialImportance.START_AND_END, (int)SequentialImportance.START_AND_END, "Only the first and last episode of a season matters, the first episode will air, the last episode will be aired only once the all other episodes have aired. every episode in between will air in any order."));
            str.AppendLine(string.Format("|{0,-10}|{1,-4}|{2}", SequentialImportance.PER_SEASON, (int)SequentialImportance.PER_SEASON, "episode order does not matter, only that no later season episode is aired before the current season has aired all its episodes."));

            Console.WriteLine(str.ToString());
        }
        #endregion
        #region create file methods
        void CreateShow()
        {
            string name = "show";
            string thumbnailPath = "cover.jpg";
            string summary = string.Empty;
            AgeRating rating = AgeRating.NOT_RATED;
            SequentialImportance sequential = SequentialImportance.ANY_ORDER;
            TimeSpan start = TimeSpan.Zero, end = TimeSpan.Zero;
            bool isAds = false, isPromotional = false, isMovie = false, hasSpecials = false;
            string? output = string.Empty;//used for console val when using ReadLine()

            Console.WriteLine("Creating a new show... (Brackets is default value in capitals)");
            //is ads?
            Console.Write("Is this show all ads? [y/N]: ");
            isAds = DefaultToFalse();
            if (isAds == false)
            {
                //is movie?
                Console.Write("Is this show a movie, single or multi-part? [y/N]: ");
                isMovie = DefaultToTrue();
                Console.WriteLine("Defaulting \"Has Specials\" to false...");
                hasSpecials = false;
            }
            else
            {
                Console.WriteLine("Defaulting \"Is Movie\" to false...");
                isMovie = false;
                //is promotion?
                Console.Write("Are these ads promoting another show? (if so, give this the same name as the show) [y/N]:");
                isPromotional = DefaultToFalse();
            }
            //has specials?
            if (isMovie == false)
            {
                Console.Write("Does this show have seasonal specials? (halloween, christmas, etc.) [y/N]: ");
                hasSpecials = DefaultToFalse();
            }
            //show name
            Console.Write($"Please provide the show name [\"{name}\"]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                output = illegalChars.Replace(output, "_");
                name = isPromotional == true ? "promoting-" + output : output;
                thumbnailPath = Path.Combine(name, thumbnailPath);
            }
            if (isAds == false)
            {//don't need a summary or thumbnail for ads
                //thumbnail
                Console.Write($"Please provide the path to the show's thumbnail [{ShowData.DEFAULT_FOLDER}\\{thumbnailPath}]: ");
                output = Console.ReadLine();
                if (IsEmptyString(output) == false)
                {

                    thumbnailPath = Path.Combine(name, output);
                }
                //summary
                Console.Write($"Please provide a summary to the show [\"{summary}\"]: ");
                output = Console.ReadLine();
                if (IsEmptyString(output) == false)
                {
                    summary = output;
                }
            }
            //rating
            Console.Write($"Please provide either the name or id of the rating according to the Age ratings enum ({COMMAND_AGERATINGS}) [{rating}]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                rating = TryParseAgeRating(output);
            }
            //sequential order
            Console.Write($"Please provide the episode air order importance ({COMMAND_SEQUENTIAL}) [{sequential}]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                sequential = TryParseSequentialImportance(output);
            }
            //start time
            Console.Write($"Please provide the start time at which the show can be aired on the channel [00:00]:");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                start = TryParseTime(output, TimeSpan.Zero);
            }
            //end time
            Console.Write($"Please provide the end time at which the show can be aired on the channel [00:00]:");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                end = TryParseTime(output, TimeSpan.Zero);
            }
            ShowData show = new ShowData(name, thumbnailPath, summary, rating, sequential, start, end, isAds, isPromotional, isMovie, hasSpecials);
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
            string? output = string.Empty;//used for console val when using ReadLine()
            //channel name
            Console.Write($"Please provide the channel name [\"{name}\"]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                name = output;
            }
            //channel number
            Console.Write($"Please provide the channel number [{number}]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
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
            if (IsEmptyString(output) == false)
            {
                channelType = output;
            }
            //start time
            Console.Write("Please provide the STARTING time for the channel in the 24 hour format \"hh:mm\" [00:00]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                start = TryParseTime(output, TimeSpan.Zero);
            }
            //end time
            Console.Write("Please provide the ENDING time for the channel in the 24 hour format format \"hh:mm\" [00:00]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                end = TryParseTime(output, TimeSpan.Zero);
            }
            //shows
            Console.WriteLine("Please provide the names of the shows to add to this channel, separated by \"|\" [no shows]: ");
            string[] showNames = Console.ReadLine().Split("|");//TODO: figure out how to have this separate like normal arguments (space separated unless between quotes)
            if (showNames.Length > 0)
            {
                for (int i = 0; i < showNames.Length; i++)
                {
                    if (IsEmptyString(showNames[i]) == true)
                    { continue; }
                    Console.Write($"Looking for show \"{showNames[i]}\"... ");
                    //try and get the shows by that name                   

                    if (_showDataIO.GetExistingPath(Path.Combine(_workingDirectory, ShowDataIO.BASE_FOLDER, ShowData.DEFAULT_FOLDER), out string path, showNames[i] + ShowData.FILETYPE, showNames[i] + ShowData.FILETYPE_LONG) == true)
                    {
                        shows.Add(Path.GetFileNameWithoutExtension(path));
                        Console.WriteLine("Found! adding...");
                    }
                    else
                    {
                        //will still add, in case someone uses this on a separate machine than the one with the actual shows
                        Console.WriteLine("NOT Found! check spelling. still adding...");
                        shows.Add(showNames[i]);
                    }
                }
            }
            ChannelData channel = new ChannelData(number, name, channelType, start, end, shows.ToArray());
            Console.WriteLine("Writing to data file...");
            int size = _channelDataIO.WriteToDataFile(channel, _workingDirectory);
            Console.WriteLine($"({size}KB) Created {channel}");
        }
        #endregion
        #region edit file methods
        void EditShow(ShowData showToChange)
        {
            string name = showToChange.Name;
            string thumbnailPath = showToChange.ThumbnailPath;
            string summary = showToChange.Summary;
            AgeRating rating = showToChange.Rating;
            SequentialImportance sequential = showToChange.EpisodeImportance;
            TimeSpan start = TimeSpan.Zero, end = TimeSpan.Zero;
            bool isAds = showToChange.IsAds, isMovie = showToChange.IsMovie, hasSeasonal = showToChange.HasSeasonal, isPromotional = showToChange.IsPromotional;
            string? output = string.Empty;//used for console val when using ReadLine()

            Console.WriteLine($"Editing {showToChange.Name}... (Brackets is default value)");
            //is ads?
            if (isAds == true)
            {
                Console.Write("Is ads [Y/n]: ");
                isAds = DefaultToTrue();
                if (isAds == true)
                {
                    if (isPromotional == true)
                    {
                        Console.WriteLine("Is promotional [Y/n]:");
                        isPromotional = DefaultToTrue();
                    }
                    else
                    {
                        Console.WriteLine("Is promotional [y/N]:");
                        isPromotional = DefaultToFalse();
                    }
                }
            }
            else
            {
                Console.Write("Is ads [y/N]: ");
                isAds = DefaultToFalse();
            }
            if (isAds == false)
            {
                //is movie?
                if (isMovie == true)
                {
                    Console.Write("Is movie [Y/n]: ");
                    isMovie = DefaultToTrue();
                }
                else
                {
                    Console.Write("Is movie [y/N]: ");
                    isMovie = DefaultToFalse();
                }
            }
            //has specials?
            if (isMovie == false)
            {
                if (hasSeasonal == true)
                {
                    Console.Write("Has specials [Y/n]: ");
                    hasSeasonal = DefaultToTrue();
                }
                else
                {
                    Console.Write("Has specials [y/N]: ");
                    hasSeasonal = DefaultToFalse();
                }
            }
            else
            {
                hasSeasonal = false;
            }
            //show name
            Console.Write($"Name [{name}]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {//TODO: remove "promoting-" if isPromotional is now false, add it if true
                name = output;
            }

            if (isAds == false)
            {
                //thumbnail
                Console.Write($"Thumbnail Name [{name}\\{Path.GetFileName(thumbnailPath)}]: ");
                output = Console.ReadLine();
                if (IsEmptyString(output) == false)
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
                if (IsEmptyString(output) == false)
                {
                    summary = output;
                }
            }
            //rating
            Console.Write($"Age rating [{rating}]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                rating = TryParseAgeRating(output, rating);
            }
            //sequential order
            Console.Write($"Air order importance [{sequential}]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                sequential = TryParseSequentialImportance(output, sequential);
            }
            //[Y/n]
            //start time
            Console.Write($"Please provide the start time at which the show can be aired on the channel [00:00]:");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                start = TryParseTime(output, TimeSpan.Zero);
            }
            //end time
            Console.Write($"Please provide the end time at which the show can be aired on the channel [00:00]:");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                end = TryParseTime(output, TimeSpan.Zero);
            }
            ShowData show = new ShowData(name, thumbnailPath, summary, rating, sequential, start, end, isAds, isPromotional, isMovie, hasSeasonal);
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
                try
                {
                    Directory.Move(oldPath, newPath);
                }
                catch (Exception)
                {
                    Console.WriteLine($"No show folder named \"{showToChange.Name}\" found, creating folder...");
                    Directory.CreateDirectory(newPath);
                }
                Console.WriteLine($"Changed show folder name.");
            }
        }
        void EditChannel(ChannelData channelToChange)
        {
            string name = channelToChange.Name;
            string channelType = channelToChange.ChannelType;
            string shows = string.Join("|", channelToChange.Shows);
            short number = channelToChange.Number;
            TimeSpan start = channelToChange.StartTime, end = channelToChange.EndTime;
            List<string> showsList = channelToChange.Shows.ToList();

            string? output = string.Empty;//used for console val when using ReadLine()

            Console.WriteLine($"Editing {channelToChange.Name}... (Brackets is default value)");
            //channel name
            Console.Write($"Name [{name}]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                name = output;
            }
            //number
            Console.Write($"channel number [{number}]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
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
            Console.Write($"Channel type [{channelType}]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                channelType = output;
            }
            //start time
            //start time
            Console.Write($"Start time [{start.ToString(@"hh\:mm")}]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                start = TryParseTime(output, start);
            }
            //end time
            Console.Write($"End time [{end.ToString(@"hh\:mm")}]: ");
            output = Console.ReadLine();
            if (IsEmptyString(output) == false)
            {
                end = TryParseTime(output, end);
            }
            //shows
            Console.WriteLine($"Shows, separated by \"|\" [{shows}]: ");
            string[] showNames = Console.ReadLine().Split("|");//TODO: figure out how to have this separate like normal arguments (space separated unless between quotes)
            if (showNames.Length > 0)
            {
                bool found = false;
                showsList.Clear();
                for (int i = 0; i < showNames.Length; i++)
                {
                    if (IsEmptyString(showNames[i].Trim()) == true)
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
                    showsList.Add(showNames[i].Trim());
                }
            }

            ChannelData channel = new ChannelData(number, name, channelType, start, end, showsList.ToArray());
            Console.WriteLine("Updating data file...");
            int size = _channelDataIO.WriteToDataFile(channel, _workingDirectory, createContentFolder: false);
            Console.WriteLine($"({size}KB) Updated {channel}");
            //if name changes, move all over to other folder
            if (name.Equals(channelToChange.Name) == false)
            {
                _channelDataIO.GetExistingPath(Path.Combine(_workingDirectory, ChannelDataIO.BASE_FOLDER, ChannelData.DEFAULT_FOLDER), out string oldDataFile, channelToChange.Name + ChannelData.FILETYPE, channelToChange.Name + ChannelData.FILETYPE_LONG);
                _channelDataIO.DeleteDataFile(oldDataFile);
                Console.WriteLine($"Changing channel folder name {channelToChange.Name} > {name}...");
                string oldPath = Path.Combine(_workingDirectory, ChannelDataIO.BASE_FOLDER, ChannelData.DEFAULT_FOLDER, channelToChange.Name);
                string newPath = Path.Combine(_workingDirectory, ChannelDataIO.BASE_FOLDER, ChannelData.DEFAULT_FOLDER, name);
                try
                {
                    Directory.Move(oldPath, newPath);
                }
                catch (Exception)
                {
                    Console.WriteLine($"No channel folder named \"{channelToChange.Name}\" found, creating folder...");
                    Directory.CreateDirectory(newPath);
                }
                Console.WriteLine($"Changed channel folder name.");
            }

        }
        #endregion
        #region update file methods
        void UpdateShow(ShowData showToUpdate, string path)
        {
            Console.WriteLine("Updating show data...");
            int size = _showDataIO.WriteToDataFile(showToUpdate, path, createContentFolder: false);
            Console.WriteLine($"({size}KB) Updated {showToUpdate}");
        }
        void UpdateChannel(ChannelData channelToUpdate, string path)
        {
            Console.WriteLine("Updating channel data...");
            int size = _channelDataIO.WriteToDataFile(channelToUpdate, path, createContentFolder: false);
            Console.WriteLine($"({size}KB) Updated {channelToUpdate}");
        }
        #endregion
        #region list files methods
        void ListAllShows(string path)
        {
            List<string> extensions = new List<string> { ShowData.FILETYPE, ShowData.FILETYPE_LONG };
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                .Where(f => extensions.IndexOf(Path.GetExtension(f)) >= 0).ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (string show in files)
            {
                sb.AppendLine("- " + Path.GetFileNameWithoutExtension(show));
            }
            Console.WriteLine($"{files.Length} shows found:");
            Console.WriteLine(sb.ToString());
        }
        void ListAllChannels(string path)
        {
            List<string> extensions = new List<string> { ChannelData.FILETYPE, ChannelData.FILETYPE_LONG };
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                .Where(f => extensions.IndexOf(Path.GetExtension(f)) >= 0).ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (string channel in files)
            {
                sb.AppendLine("- " + Path.GetFileNameWithoutExtension(channel));
            }
            Console.WriteLine($"{files.Length} channels found:");
            Console.WriteLine(sb.ToString());
        }
        #endregion
        #region special methods
        /// <summary>Returns whether value starts with "y" or is empty</summary>
        /// <param name="val">value to check</param>
        private bool DefaultToTrue()
        {
            string output = Console.ReadLine();
            if (IsEmptyString(output) == true)
            { return true; }
            return output.ToLower().StartsWith("y") == true;
        }
        /// <summary>Returns whether value starts with "n" or is empty</summary>
        /// <param name="val">value to check</param>
        private bool DefaultToFalse()
        {
            string output = Console.ReadLine();
            if (IsEmptyString(output) == true)
            { return false; }
            return output.ToLower().StartsWith("n") == false;
        }
        /// <summary>Returns string.IsNullOrWhiteSpace</summary>
        /// <param name="val">value to check</param>
        private bool IsEmptyString(string val)
        {
            return string.IsNullOrWhiteSpace(val);
        }
        /// <summary>Returns a parsed timespan string, writes to console if fails.</summary>
        /// <param name="val">value to parse</param>
        /// <param name="def">default value to fall back to</param>
        private TimeSpan TryParseTime(string val, TimeSpan def)
        {
            if (TimeSpan.TryParse(val, out TimeSpan time) == true)
            {
                return time;
            }
            Console.WriteLine("Unable to parse, hours can only be between 0-23. resorting to default...");
            return def;
        }
        /// <summary>Returns a parsed <see cref="AgeRating"/>, writes to console if fails.</summary>
        /// <param name="val">value to parse</param>
        /// <param name="def">default value to fall back to</param>
        private AgeRating TryParseAgeRating(string val, AgeRating def = AgeRating.NOT_RATED)
        {
            if (Enum.TryParse(typeof(AgeRating), val, true, out object result) == true)
            {
                return (AgeRating)result;
            }
            else
            {
                Console.WriteLine($"Unable to parse Age rating, adjust it if needed with {COMMAND_EDITSHOW}...");
                return def;
            }
        }
        /// <summary>Returns a parsed <see cref="SequentialImportance"/>, writes to console if fails.</summary>
        /// <param name="val">value to parse</param>
        /// <param name="def">default to fall back to</param>
        /// <returns></returns>
        private SequentialImportance TryParseSequentialImportance(string val, SequentialImportance def = SequentialImportance.ALL_IN_ORDER)
        {
            if (Enum.TryParse(typeof(SequentialImportance), val, true, out object result) == true)
            {
                return (SequentialImportance)result;
            }
            else
            {
                Console.WriteLine($"Unable to parse airing order importance, adjust it if needed with {COMMAND_EDITSHOW}...");
                return def;
            }
        }

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
        internal const string COMMAND_UPDATEPROGRAM = "--update";
        internal const string COMMAND_UPDATESHOW = "--updateshow";
        internal const string COMMAND_UPDATECHANNEL = "--updatechannel";
        internal const string COMMAND_UPDATESCHEDULE = "--updateschedule";
        internal const string COMMAND_SHOWINFO = "--showinfo";
        internal const string COMMAND_CHANNELINFO = "--channelinfo";
        internal const string COMMAND_SCHEDULEINFO = "--scheduleinfo";
        internal const string COMMAND_SETUP = "--setup";
        internal const string COMMAND_AGERATINGS = "--ageratings";
        internal const string COMMAND_SEQUENTIAL = "--EpisodeImportances";
        #endregion
    }
}
