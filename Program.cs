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
            DateTime start_time = DateTime.Now;
            while (true)
            {
                string Path = "";
                Bitmap bitmap = new Bitmap(1, 1);
                while (true)
                {
                    Log("Please type in the file path to the image. Write \"exit\" to exit the program");
                    try
                    {
                        Console.Write(">>> ");
                        Path = Console.ReadLine()!;
                        // TESTING PURPOSES
                        if (Path.ToLower() == "image") {
                            Path = "C:\\Users\\Lenovo\\OneDrive\\Billeder\\test\\image.jpg";
                            //break;
                        }
                        // exiting
                        if (Path.ToLower() == "exit" || Path.ToLower() == "quit")
                        {
                            Path = "EXIT";
                            break;
                        }
                        Log("Loading file");
                        // loads image into a bitmap 
                        bitmap = (Bitmap)Image.FromFile(Path);
                        break;
                    }
                    // file not found
                    catch (System.IO.FileNotFoundException) {
                        Console.WriteLine($"ERORR: File \"{Path}\" was not found!");
                        continue;
                        throw;
                    }
                    // no path given
                    catch (System.ArgumentException) {
                        Console.WriteLine($"ERROR: No path given");
                        continue;
                        throw;
                    }
                    // general catch
                    catch (System.Exception e)
                    {
                        Log("Error!");
                        Console.WriteLine($"DEBUG: {e}");   
                        continue;
                        throw;
                    }
                }
                // if statement for exit commands
                if (Path == "EXIT")
                    //Console.WriteLine("last check");
                    break;
                
                Console.Clear();

                
                Console.CursorVisible = false;

                Log("Width: " + bitmap.Width + ". Height: " + bitmap.Height);

                Log("Scrambling");
                Scrambler.Scramble(bitmap);

                Log("Save file");
                string pathName = Path.Split('.')[0] + "-ScrambledCopy." + Path.Split('.')[1];
                bitmap.Save(pathName);

                Log("Dispose");
                bitmap.Dispose();

                Log("Done");
                Console.CursorVisible = true;
                Console.WriteLine($"Finished after {DateTime.Now - start_time}");
                Log("Press Enter to continue");
                Console.ReadLine();
                Console.Clear();
            }
            Console.ResetColor();
            Console.CursorVisible = true;
        }
    }
}