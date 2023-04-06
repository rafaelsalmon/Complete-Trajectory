using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Camera
{
    // Camera Control

    public class Camera
    {
        public string Fotografa()
        {
            return Run();
        }

        static string Run()
        {
            try
            {
                //Configure process
                var psi = new ProcessStartInfo()
                {
                    FileName = "bash",
                    Arguments = "camerascript", //bash script with camera instructions
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                };

                //Start new process with command to execute camera script as argument
                Process proc = new Process() { StartInfo = psi, };
                proc.Start();

                //
                string result = proc.StandardOutput.ReadToEnd(); //reads console
                proc.WaitForExit(); //synchronizes external process finishe execution with console app execution of next step

                if (string.IsNullOrWhiteSpace(result)) //no console output
                {
                    Console.WriteLine("The Command '" + psi.Arguments + "' ended with exitcode: " + proc.ExitCode); // Logs process execution result with exit code
                    return proc.ExitCode.ToString(); // returns process executin exit code to caller
                }
                return result; //returns console output to caller
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString()); //Logs error in case of occurence
            }

            return "-1"; //In error case, returns "-1"
        }
    }
}
