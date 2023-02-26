using System.Drawing;
using System.Runtime.InteropServices;

namespace IS
{
    internal class Program
    {
        [STAThread]
        static void Main()
        {
            while (true)
            {
                string Path = "";
                Bitmap bitmap = new Bitmap(1, 1);
                while (true)
                {
                    Console.WriteLine("Please type in the file path to the image: Write \"Exit\" to exit the program");
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
                        Console.WriteLine("Error!");
                        continue;
                        throw;
                    }
                }
                if (Path == "")
                    break;

                Console.Clear();

                Console.CursorVisible = false;
                Console.WriteLine("Load file");

                Console.WriteLine("Width: " + bitmap.Width + ". Height: " + bitmap.Height);

                Console.WriteLine("Scrambler");
                Scrambler.Scramble(bitmap);

                Console.WriteLine("Save file");
                string pathName = Path.Split('.')[0] + "-ScrambledCopy." + Path.Split('.')[1];
                bitmap.Save(pathName);

                Console.WriteLine("Dispose");
                bitmap.Dispose();

                Console.WriteLine("Done");
                Console.CursorVisible = true;

                Console.WriteLine("Press Enter to continue");
                Console.ReadLine();
                Console.Clear();
            }
            Console.ResetColor();
            Console.CursorVisible = true;
        }
    }
}