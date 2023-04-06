using System;
using System.Diagnostics;

//Testing program - Not used in the solution

Console.Write("Starting...");
string? entrada = Console.ReadLine();

string comandoLinux = "libcamera-hello";
comandoLinux = "libcamera-still";
string? res = Run(comandoLinux);

Console.Write("Ending...");
entrada = Console.ReadLine();

static string Run(string cmd, bool sudo = false)
{
    try
    {

        List<string> argumentos = new List<string>();

        var psi = new ProcessStartInfo()
        {
            FileName = "bash",
            Arguments = "camerascript", 
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
        };

        Process proc = new Process() { StartInfo = psi, };
        proc.Start();
        string result = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();

        if (string.IsNullOrWhiteSpace(result))
        {
            Console.WriteLine("The Command '" + psi.Arguments + "' ended with exitcode: " + proc.ExitCode);
            return proc.ExitCode.ToString();
        }
        return result;
    }
    catch (Exception exc)
    {
           Console.WriteLine(exc.ToString());
           Debug.WriteLine("Native Linux comand failed: " + cmd);
           Debug.WriteLine(exc.ToString());
    }

    return "-1";
}