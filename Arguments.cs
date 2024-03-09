namespace SelectDirectory
{
    public struct Arguments 
    {
        public string exe;
        public string args;
        public string startDir;
        public bool quiet;

        public Arguments(string exec = null) 
        {
            exe = exec;
            args = null;
            startDir = null;
            quiet = false;
        }
    }
}
