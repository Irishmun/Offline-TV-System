using System;

namespace OTS_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ChannelData data = ChannelData.TestData();
            CommandHandler handler = new CommandHandler();
#if DEBUG
            if (args != null && args.Length > 0 && args[0].Equals("--runasprogram"))
            {
                Console.WriteLine("Running application as program, use \"|\" to separate arguments");
                RunAsProgram();
            }
#endif
            handler.ProcessArgument(args);
#if DEBUG
            void RunAsProgram()
            {
                Console.WriteLine("insert command (split arguments with \"|\"):");
                string[] command = Console.ReadLine().Split("|");
                if (command.Length > 0)
                {
                    for (int i = 0; i < command.Length; i++)
                    {
                        command[i] = command[i].Trim();
                    }
                    handler.ProcessArgument(command);
                }
                RunAsProgram();
            }
#endif
        }

    }
}