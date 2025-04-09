using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace RegSpAX
{
    internal class Program
    {
        static void Main()
        {
            string regPath = @"SYSTEM\CurrentControlSet\Control\Session Manager\AppCompatCache";
            string valueName = "AppCompatCache";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"
                                        ______           _____        ___  __   __
                                        | ___ \         /  ___|      / _ \ \ \ / /
                                        | |_/ /___  __ _\ `--. _ __ / /_\ \ \ V / 
                                        |    // _ \/ _` |`--. \ '_ \|  _  | /   \ 
                                        | |\ \  __/ (_| /\__/ / |_) | | | |/ /^\ \
                                        \_| \_\___|\__, \____/| .__/\_| |_/\/   \/
                                                    __/ |     | |                 
                                                   |___/      |_|                 

 +-------------------------------------------------------------------------------------------------------------------+
            ");
            Console.ResetColor();

            try
            {
                Console.Title = "RegSpAx - Reg Scanner";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(regPath))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(valueName);
                        if (value is byte[] data)
                        {
                            string extractedText = ExtractPathsFromBytes(data);

                            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpAX.txt");
                            File.WriteAllText(outputPath, extractedText, Encoding.UTF8);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("      Extracted paths.");
                            Console.WriteLine($"      File Saved In: {outputPath}\n\n");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine("AppCompatCache Invalid Entry");
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Reg Key Not Found");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Reg Access Error: " + ex.Message);
                Console.ResetColor();
            }

            Console.WriteLine("\n      Press Enter to exit...");
            Console.ReadLine();
        }

        static string ExtractPathsFromBytes(byte[] data)
        {
            StringBuilder result = new StringBuilder();
            StringBuilder temp = new StringBuilder();

            for (int i = 0; i < data.Length - 1; i += 2)
            {
                char c = (char)(data[i] | (data[i + 1] << 8));

                if ((c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z') ||
                    (c >= '0' && c <= '9') ||
                    c == ':' || c == '\\' || c == '/' || c == '.' || c == '-' || c == '_' || c == ' ' || c == '[' || c == ']')
                {
                    temp.Append(c);
                }
                else
                {
                    string path = temp.ToString();

                    int index = path.IndexOf(@":\");
                    if (index > 0 && index - 1 >= 0 && char.IsLetter(path[index - 1]))
                    {
                        string cleanedPath = path.Substring(index - 1);
                        if (cleanedPath.Length > 3)
                        {
                            result.AppendLine(cleanedPath);
                        }
                    }

                    temp.Clear();
                }
            }

            return result.ToString();
        }
    }
}
