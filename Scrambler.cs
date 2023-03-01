using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace IS
{
    unsafe class Scrambler
    {
        [StructLayout(LayoutKind.Sequential)]
        struct PixelARGB // same as System.Drawing.Color
        {
            public byte A;
            public byte R;
            public byte G;
            public byte B;

            public PixelARGB(byte a, byte r, byte g, byte b)
            {
                A = a;
                R = r;
                G = g;
                B = b;
            }

            public int GetARGB()
            {
                int pixel = A << 24;
                pixel |= R << 16;
                pixel |= G << 8;
                pixel |= B;
                return pixel;
            }

            public void FromARGB(int pixel)
            {
                A = (byte)((pixel >> 24) & 0xff);
                R = (byte)((pixel >> 16) & 0xff);
                G = (byte)((pixel >> 8) & 0xff);
                B = (byte)(pixel & 0xff);
            }
        }

        static bool LockSrcamble = false;
        static PixelARGB* buffer;
        static PixelARGB* SwapBuffer(PixelARGB* newBuffer)
        {
            var oldBuffer = buffer;
            buffer = newBuffer;
            return oldBuffer;
        }


        const int LineNum = 15; // progress line location
        static int LastNum = 0;
        static void Progress(float num, float goal)
        {
            int newNum = (int)(num / goal * 100);
            if (newNum == LastNum) // no need to update
                return;
            LastNum = newNum;
            var cursor = Console.GetCursorPosition();
            Console.SetCursorPosition(0, LineNum);
            Console.WriteLine("                  ");
            Console.SetCursorPosition(0, LineNum);
            Console.WriteLine(newNum + "%");
            Console.SetCursorPosition(cursor.Left, cursor.Top);
        }

        private static Random rand = new Random();

        /// <summary>
        /// This function locks the bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        public static void Scramble(Bitmap bitmap)
        {
            if (LockSrcamble)
                return;
            LockSrcamble = true;

            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            int* pixelData = (int*)bitmapdata.Scan0;

            int pixelCount = bitmap.Width * bitmap.Height;
            buffer = (PixelARGB*)Marshal.AllocHGlobal(pixelCount * sizeof(PixelARGB));

            Program.Log("Scrambling");

            Program.Log("Copy image to pixel buffer");
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    buffer[i + j * bitmap.Width].FromARGB(pixelData[i + j * bitmap.Width]);
                }
                Progress(i, bitmap.Width);
            }


            Program.Log("Scrambling Colors");
            ScrambleColors(bitmap.Width, bitmap.Height);

            Program.Log("Scrambling Lines");
            ScrambleLines(bitmap.Width, bitmap.Height);

            Program.Log("Scrambling With Grains");
            ScrambleWithGrains(bitmap.Width, bitmap.Height);

            Program.Log("Scrambling Color Shift");
            ScrambleColorShift(bitmap.Width, bitmap.Height);

            Program.Log("Copy buffer to image");
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    pixelData[i + j * bitmap.Width] = buffer[i + j * bitmap.Width].GetARGB();
                }
                Progress(i, bitmap.Width);
            }
            Progress(1, 1);

            Marshal.FreeHGlobal((IntPtr)buffer);
            bitmap.UnlockBits(bitmapdata);
            LockSrcamble = true;
        }

        private static void ScrambleColors(int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int pixelIndex = i + j * width;
                    // buffer[pixelIndex].R = 255;
                    buffer[pixelIndex].R = RandomizeByte(buffer[pixelIndex].R);
                    buffer[pixelIndex].G = RandomizeByte(buffer[pixelIndex].G);
                    buffer[pixelIndex].B = RandomizeByte(buffer[pixelIndex].B);
                }
                Progress(i, width);
            }
            Progress(1, 1);
        }

        private static void ScrambleLines(int width, int height)
        {
            int iterations = height / 10;

            for (int i = 0; i < iterations; i++)
            {
                int lineWidth = rand.Next(width / 50, width / 10);
                int lineHeight = rand.Next(1, Math.Clamp(height / 1000, 1, height));
                int srcX = rand.Next(0, width - lineWidth);
                int srcY = rand.Next(0, height - lineHeight);
                int dstX = rand.Next(0, width - lineWidth);
                int dstY = rand.Next(0, height - lineHeight);

                int srcPos = srcX + srcY * width;
                int dstPos = dstX + dstY * width;

                for (int j = 0; j < lineWidth; j++)
                {
                    for (int k = 0; k < lineHeight; k++)
                        buffer[dstPos + width * k] = buffer[srcPos + width * k];
                    srcPos++;
                    dstPos++;
                }
                Progress(i, iterations);
            }
            Progress(1, 1);
        }

        private static void ScrambleWithGrains(int width, int height)
        {
            int iterations = height / 400 * width / 400;

            for (int i = 0; i < iterations; i++)
            {
                int grainXY = Math.Max(height / 800, 1) + Math.Max(width / 800, 1);
                int xPos = rand.Next(0, width - grainXY);
                int yPos = rand.Next(0, height - grainXY);

                for (int x = 0; x < grainXY; x++)
                {
                    for (int y = 0; y < grainXY; y++)
                    {
                        if (rand.Next(0, 5) == 1) // gaps in grains
                            continue;
                        int pos = xPos + x + (yPos + y) * width;
                        buffer[pos] = LowerRGBBrightness(buffer[pos], 180);
                    }
                }

                Progress(i, iterations);
            }
            Progress(1, 1);
        }

        private static void ScrambleColorShift(int width, int height)
        {
            const int RXSht = -10;
            const int RYSht = -10;
            const int GXSht = 0;
            const int GYSht = 0;
            const int BXSht = 10;
            const int BYSht = 10;

            PixelARGB* dstBuffer = (PixelARGB*)Marshal.AllocHGlobal(width * height * sizeof(PixelARGB));

            // Shift colors
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int dstPos = i + j * width;
                    int rSourcePos = Math.Clamp(dstPos + (RXSht + RYSht * width), 0, width * height);
                    int gSourcePos = Math.Clamp(dstPos + (GXSht + GYSht * width), 0, width * height);
                    int bSourcePos = Math.Clamp(dstPos + (BXSht + BYSht * width), 0, width * height);

                    dstBuffer[dstPos] = new PixelARGB(
                        buffer[dstPos].A,
                        buffer[rSourcePos].R,
                        buffer[gSourcePos].G,
                        buffer[bSourcePos].B
                    );
                }
                Progress(i, width);
            }

            var oldBuffer = SwapBuffer(dstBuffer);
            Marshal.FreeHGlobal((IntPtr)oldBuffer);
            Progress(1, 1);
        }

        private static byte RandomizeByte(byte val, int range = 50) => (byte)Math.Clamp(val + rand.Next(0, range * 2) - range, 0, 255);
        private static PixelARGB LowerRGBBrightness(PixelARGB pixel, int amount)
        {
            byte a = pixel.A;
            byte r = pixel.R;
            byte g = pixel.G;
            byte b = pixel.B;

            return new PixelARGB(
                a,
                (byte)Math.Clamp(r - amount, 0, 255),
                (byte)Math.Clamp(g - amount, 0, 255),
                (byte)Math.Clamp(b - amount, 0, 255)
            );
        }
    }
}