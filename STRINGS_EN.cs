using System;

namespace SelectDirectory
{
    public class STRINGS_EN
    {
        public const string STR_HEADER = "Select Directory (%1)" + NEW_LINE +
                                         STR_CONTROLS + NEW_LINE;
        public const string STR_CONTROLS = "↑↓ move | ↵ open | → sel | ⌫  parent | ? help" + NEW_LINE;
        public const string STR_SEL = " <<";
        public const string STR_DIR_NOT_FOUND = "Directory not found: ";
        public const string STR_SPACE = " ";
        public const string SLASH = "\\";

        public const string NEW_LINE = "\n";

        public static string STR_USAGE = $"%1 v%2" + NEW_LINE +
                "A tool to select a directory and open it with a specified program." + NEW_LINE +
                $"Usage: %1 /cmd <command> <arguments> [/dir <start_dir>] [/q]" + NEW_LINE + NEW_LINE +
                STR_CONTROLS + NEW_LINE +
                "Press any key to return...";
    }
}
