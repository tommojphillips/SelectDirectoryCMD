using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using static SelectDirectory.STRINGS_EN;

namespace SelectDirectory
{
    public class Program
    {
        // Author, tommojphillips, 05.03.2024
        // Github, https://github.com/tommojphillips
        // A console command-line tool to select a directory and open it with a specified program

        static Arguments sw;

        static Encoding originalEncoding;

        static void Main(string[] args)
        {
            Console.Title = "Select Directory";

            Console.CursorVisible = false;
            originalEncoding = Console.OutputEncoding;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (!parseArgs(args))
                return;
            
            SelDir sDir = new SelDir(sw);
            sDir.run();

            Console.OutputEncoding = originalEncoding;
            Console.CursorVisible = true;
        }

        static bool parseArgs(string[] args)
        {
            // Parse command line arguments
            
            if (args.Length < 1) goto Usage;

            sw = new Arguments();

            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("/"))
                    continue;

                switch (args[i].ToLower())
                {
                    case "-?":
                    case "/?":
                        goto Usage;
                    case "/exe":
                    case "/cmd":

                        int j = i + 1;

                        if (args[j].Contains(" "))
                        {
                            sw.exe = args[j].Split(' ')[0];
                            sw.args = args[j].Replace(sw.exe, string.Empty).Trim();
                        }
                        else
                        {
                            sw.exe = args[j];
                            sw.args = string.Empty;
                        }
                        j++;
                        while (j < args.Length && !args[j].StartsWith("/"))
                        {
                            sw.args += args[j] + " ";
                            j++;
                        }
                        break;
                    case "/dir":
                        try
                        {
                            sw.startDir = args[i + 1];
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            goto Error;
                        }
                        break;
                    case "/q":
                        sw.quiet = true;
                        break;
                }
            }

            if (sw.exe == null)
                goto Usage;

            return true;

        Usage:
            usage();
            return false;
        Error:
            if (!sw.quiet)
                Console.ReadKey(true);
            return false;
        }

        public static void usage()
        {
            // Display usage information and pause.

            Console.Clear();
            FileVersionInfo info = Process.GetCurrentProcess().MainModule.FileVersionInfo;
            string name = Path.GetFileNameWithoutExtension(info.FileName);
            string version = info.FileVersion;
            string usageStr = STR_USAGE.Replace("%1", name).Replace("%2", version);
            Console.WriteLine(usageStr);
            Console.ReadKey(true);
        }
    }
}
