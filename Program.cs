﻿using System.Drawing;
using System.Runtime.InteropServices;

namespace IS
{
    internal class Program
    {
        public static void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("{h:mm:ss tt}");
            Console.WriteLine($"{timestamp} {message}");
        }

        [STAThread]
        static void Main()
        {
            while (true)
            {
                string Path = "";
                Bitmap bitmap = new Bitmap(1, 1);
                while (true)
                {
                    Log("Please type in the file path to the image: Write \"Exit\" to exit the program");
                    try
                    {
                        Path = Console.ReadLine()!;

                        if (Path.ToLower() == "exit")
                        {
                            Path = "";
                            break;
                        }
                        bitmap = (Bitmap)Image.FromFile(Path);
                        break;
                    }
                    catch (System.Exception)
                    {
                        Log("Error!");
                        continue;
                        throw;
                    }
                }
                if (Path == "")
                    break;

                Console.Clear();

                Console.CursorVisible = false;
                Log("Load file");

                Log("Width: " + bitmap.Width + ". Height: " + bitmap.Height);

                Log("Scrambler");
                Scrambler.Scramble(bitmap);

                Log("Save file");
                string pathName = Path.Split('.')[0] + "-ScrambledCopy." + Path.Split('.')[1];
                bitmap.Save(pathName);

                Log("Dispose");
                bitmap.Dispose();

                Log("Done");
                Console.CursorVisible = true;

                Log("Press Enter to continue");
                Console.ReadLine();
                Console.Clear();
            }
            Console.ResetColor();
            Console.CursorVisible = true;
        }
    }
}