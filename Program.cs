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
                string Path = "";
                Bitmap bitmap = new Bitmap(1, 1);
                while (true)
                {
                    Log("Please type in the file path to the image: Write \"Exit\" to exit the program");
                    try
                    {
                        Path = Console.ReadLine()!;
                        if (Path.ToLower() == "image") {
                            Path = "C:\\Users\\Lenovo\\OneDrive\\Billeder\\test\\image.jpg";
                        }
                        if (Path.ToLower() == "exit")
                        {
                            Path = "";
                            break;
                        }
                        Log("Loading file");
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