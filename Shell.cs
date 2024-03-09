using System;
using System.Diagnostics;

namespace xShell
{
    public class Shell
    {
        public static int exe(string executable, params string[] args)
        {
            using (Process shell = new Process())
            {
                shell.StartInfo = new ProcessStartInfo();
                shell.StartInfo.UseShellExecute = false;
                shell.StartInfo.FileName = "cmd";
                shell.StartInfo.Arguments = $"/c {executable} {(args == null ? string.Empty : string.Join(" ", args))}";

                try
                {
                    shell.Start();
                    shell.WaitForExit();
                    return shell.ExitCode;
                }
                catch (Exception) { }
            }     
            return -1;
        }
    }
}
