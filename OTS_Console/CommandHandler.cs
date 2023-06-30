using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS_Console
{
    internal class CommandHandler
    {
        private void DebugCommand(string arg)
        {

        }

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
                case COMMAND_HELP:
                case COMMAND_HELP_SHORT:
                    ProvideHelp();
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
                    break;
                case COMMAND_CREATECHANNEL:
                    //create new channel
                    break;
                case COMMAND_CREATESCHEDULE:
                    //create new schedule
                    break;
                case COMMAND_SHOWINFO:
                    //provide info on given show name (if found)
                    break;
                case COMMAND_CHANNELINFO:
                    //provide info on given channel name (if found)
                    break;
                case COMMAND_SCHEDULEINFO:
                    //provice info on given schedule name (if found)
                    break;
                default:
                    Console.WriteLine($"Command not recognized, type \"{COMMAND_HELP}\" or \"{COMMAND_HELP_SHORT}\" for help");
                    break;
            }
        }

        void ProvideHelp()
        {
            Console.WriteLine("Here's a list of all available commands:\n"+
            $"{COMMAND_VERSION} {COMMAND_VERSION_SHORT}: Get the application version.\n"+
            $"{COMMAND_HELP} {COMMAND_HELP_SHORT}: Show all available commands.\n"+
            $"{COMMAND_LISTSHOWS}: Lists all shows at path (recursive). defaults to current directory\n"+
            $"{COMMAND_LISTCHANNELS}: Lists all channels at path (recursive). defaults to current directory\n"+
            $"{COMMAND_LISTSCHEDULES}: Lists all schedules at path (recursive). defaults to current directory\n"+
            $"{COMMAND_CREATESHOW}: Creates or updates a show.\n"+
            $"{COMMAND_CREATECHANNEL}: Creates or updates a channel.\n"+
            $"{COMMAND_CREATESCHEDULE}: Creates a schedule file.\n"+
            $"{COMMAND_SHOWINFO}: Shows the info of the given show.\n"+
            $"{COMMAND_CHANNELINFO}: Shows the info of the given channel.");
        }

        internal const string COMMAND_VERSION = "--version", COMMAND_VERSION_SHORT = "-v";
        internal const string COMMAND_HELP = "--help", COMMAND_HELP_SHORT = "-h";
        internal const string COMMAND_LISTSHOWS = "--listshows";
        internal const string COMMAND_LISTCHANNELS = "--listchannels";
        internal const string COMMAND_LISTSCHEDULES = "--listschedules";
        internal const string COMMAND_CREATESHOW = "--createshow";
        internal const string COMMAND_CREATECHANNEL = "--createchannel";
        internal const string COMMAND_CREATESCHEDULE = "--createschedule";
        internal const string COMMAND_SHOWINFO = "--showinfo";
        internal const string COMMAND_CHANNELINFO = "--channelinfo";
        internal const string COMMAND_SCHEDULEINFO = "--scheduleinfo";
    }
}
