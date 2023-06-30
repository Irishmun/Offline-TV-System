using System;

namespace OTS_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ChannelData data = ChannelData.TestData();
            CommandHandler handler = new CommandHandler();

            handler.ProcessArgument(args);
        }

    }
}