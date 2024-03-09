using System;
using System.IO;
using System.Linq;
using xShell;

using static SelectDirectory.STRINGS_EN;

namespace SelectDirectory
{
    public class SelDir
    {
        // Author, tommojphillips, 05.03.2024
        // Github, https://github.com/tommojphillips
        // A tool to select a directory and open it with a specified program
        
        private readonly Arguments sw;

        private string curDir;
        private string prevDir;
        private string parentDir;
        private string[] subDirs;

        private bool goToParent;
        private bool isRoot;
        private bool hasSelected;

        private int index;
        private int page;
        private int count;
        private int clOffset;
        private int headerHeight;

        private bool redraw;
        private int rowsPP;

        public SelDir(Arguments arguments)
        {
            sw = arguments;
            curDir = sw.startDir;

            redraw = true;
        }

        public void run()
        {
            // Entry point

            if (!checkDir(curDir))
                return;

            while (!hasSelected)
            {
                loop();
            }

            finish();
        }
        private void finish()
        {
            Console.Clear();

            if (!hasSelected || curDir == null)
                return;

            if (!checkDir(curDir))
            {
                Console.WriteLine("Invalid directory: " + curDir);
                if (!sw.quiet)
                    Console.ReadKey(true);
                return;
            }

            if (string.IsNullOrEmpty(sw.exe))
            {
                Console.WriteLine("Invalid executable: " + sw.exe);
                if (!sw.quiet)
                    Console.ReadKey(true);
                return;
            }
            int error = Shell.exe(sw.exe, sw.args, $"\"{curDir}\"");

            if (error != 0)
            {
                Console.WriteLine("Error: " + error);
                if (!sw.quiet)
                    Console.ReadKey(true);
            }
        }
        private void loop()
        {
            // Main loop

            if (redraw)
            {
                redraw = false;

                headerHeight = STR_HEADER.Count(c => c == NEW_LINE[0]);

                Console.Clear();

                update(ref curDir);

                listDirs();
            }

            setCursor(false);
            waitForInput();
        }

        private void listDirs()
        {
            // List subdirectories

            clOffset = 0;
            rowsPP = Console.WindowHeight - 2 - headerHeight;
            page = index / rowsPP;

            int relPoint = page * rowsPP;
            string name;
            int lastIndex;

            for (int i = relPoint; i < count; i++)
            {
                if (i > rowsPP * (page + 1))
                    break;

                lastIndex = subDirs[i].LastIndexOf(SLASH);
                name = subDirs[i].Substring(lastIndex > 0 ? lastIndex + 1 : 0) + SLASH;

                if (name.Length > clOffset)
                {
                    clOffset = name.Length;
                }

                Console.Write($"{i + 1:00}.) {name}" + NEW_LINE);
            }

            clOffset += 2 + 3;
        }
        private void waitForInput()
        {
            // Wait for user input and handle it

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.KeyChar == '?')
            {                
                Program.usage();
                redraw = true;
                return;
            }

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    setCursor(true);
                    index -= 1;
                    break;
                case ConsoleKey.DownArrow:
                    setCursor(true);
                    index += 1;
                    break;

                case ConsoleKey.PageDown:
                    setCursor(true);
                    index = count - 1;
                    break;
                case ConsoleKey.PageUp:
                    setCursor(true);
                    index = 0;
                    break;

                case ConsoleKey.Enter:
                    if (count == 0)
                        return;
                    redraw = true;
                    curDir = subDirs[index];
                    index = 0;
                    break;

                case ConsoleKey.Backspace:
                    if (isRoot)
                        return;
                    redraw = true;
                    goToParent = true;
                    prevDir = curDir;
                    curDir = parentDir;                    
                    break;

                case ConsoleKey.RightArrow:
                    hasSelected = true;
                    goto case ConsoleKey.Enter;

                case ConsoleKey.S:
                    hasSelected = true;
                    return;

                case ConsoleKey.R:
                    redraw = true;
                    return;
            }

            if (index > count - 1)
            {
                index = 0;
                redraw = true;
            }
            else if (index > rowsPP * (page + 1))
            {
                redraw = true;
            }
            else if (index < 0)
            {
                index = count - 1;
                redraw = true;
            }
            else if (index < rowsPP * page)
            {
                redraw = true;
            }
        }
        private void setCursor(bool clear)
        {
            if (count == 0)
                return;

            Console.SetCursorPosition(clOffset, index + headerHeight - page - (rowsPP - 1) * page);

            if (clear)
            {
                for (int i = 0; i < STR_SEL.Length; i++)
                {
                    Console.Write(STR_SPACE);
                }
                return;
            }

            Console.Write(STR_SEL);
        }

        private string parseName(string format, string path)
        {
            // Parse directory name to fit in console window

            int prefixLen = format.Replace("%1", "").Length;

            if (path.Length + path.IndexOf(NEW_LINE) + 1 < Console.WindowWidth)
                return format.Replace("%1", path);

            return format.Replace("%1", path.Substring(path.Length + prefixLen - Console.WindowWidth, path.Length - 1));
        }
        private void parseSubDirs(string path)
        {
            subDirs = Directory.GetDirectories(path);

            parentDir = Directory.GetParent(path)?.FullName;
            isRoot = parentDir == null;

            if (isRoot)
            {
                subDirs = subDirs.Where(d =>
                !d.EndsWith("$RECYCLE.BIN") &&
                !d.EndsWith("System Volume Information")).ToArray();
            }

            count = subDirs.Length;
        }
        private void update(ref string path)
        {
            Console.Write(parseName(STR_HEADER, path));

            try 
            {
                parseSubDirs(path);
            }
            catch (Exception) 
            { 
                return;
            }

            if (goToParent)
            {
                index = Math.Max(0, Array.IndexOf(subDirs, prevDir));
                goToParent = false;
            }
        }

        private bool checkDir(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            Console.WriteLine(STR_DIR_NOT_FOUND + path);
            if (!sw.quiet)
                Console.ReadKey();
            return false;
        }
    }
}
