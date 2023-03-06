using System.Drawing;
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
                string image_file_path = "";
                Bitmap bitmap = new Bitmap(1, 1);
                while (true)
                {
                    Log("Please type in the file path to the image. Write \"exit\" to exit the program");
                    try
                    {
                        Console.Write(">>> ");
                        image_file_path = Console.ReadLine()!;
                        // TESTING PURPOSES
                        if (image_file_path.ToLower() == "image")
                        {
                            image_file_path = "C:\\Users\\Lenovo\\OneDrive\\Billeder\\test\\image.jpg";
                        }
                        // exiting
                        if (image_file_path.ToLower() == "exit" || image_file_path.ToLower() == "quit")
                        {
                            image_file_path = "EXIT";
                            break;
                        }
                        Log("Loading file");
                        // loads image into a bitmap 
                        bitmap = (Bitmap)Image.FromFile(image_file_path);
                        break;
                    }
                    // file not found
                    catch (System.IO.FileNotFoundException)
                    {
                        Console.WriteLine($"ERORR: File \"{image_file_path}\" was not found!");
                        continue;
                        throw;
                    }
                    // no path given
                    catch (System.ArgumentException)
                    {
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
                if (image_file_path == "EXIT")
                    break;
                Console.Clear();

                Console.CursorVisible = false;

                Log("Width: " + bitmap.Width + ". Height: " + bitmap.Height);

                Log("Scrambling");
                Scrambler.Scramble(bitmap);

                Log("Saving file");
                string file_name_no_ext   = Path.GetFileNameWithoutExtension(image_file_path);
                string file_name_just_ext = Path.GetExtension(image_file_path);
                string file_path          = Path.GetDirectoryName(image_file_path);
                string pathName           = Path.Combine(file_path, $"{file_name_no_ext}-ScrambledCopy{file_name_just_ext}");
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