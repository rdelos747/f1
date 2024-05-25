using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Graphics.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

namespace f1
{
    class Utils
    {
        public static int ParseInt(string s, string message = null)
        {
            int v = 0;
            try
            {
                v = int.Parse(s);
            }
            catch (Exception)
            {
                throw new ArgumentException($"COULD NOT PARSE INT FROM STRING {s}");
            }
            return v;
        }

        public static async Task<BitmapDecoder> GetBitmapDecoder(string path)
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            var stream = await file.OpenAsync(FileAccessMode.Read);
            return await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
        }

        public async static Task<OcrResult> GetOcrWithCrop(OcrEngine engine, BitmapDecoder decoder, Crop crop)
        {
            uint fullWidth = decoder.PixelWidth;
            uint fullHeight = decoder.PixelHeight;

            var softwareBitmap = await decoder.GetSoftwareBitmapAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied,
                new BitmapTransform()
                {
                    Bounds = new BitmapBounds()
                    {
                        X = (uint)Math.Floor(fullWidth * crop.Left),
                        Y = (uint)Math.Floor(fullHeight * crop.Top),
                        Width = (uint)Math.Floor(fullWidth * crop.Width),
                        Height = (uint)Math.Floor(fullHeight * crop.Height)
                    }
                },
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.ColorManageToSRgb
            );

            return await engine.RecognizeAsync(softwareBitmap);
        }

        public static Bitmap TakeScreenshot(Rectangle bounds)
        {
            Bitmap bmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            return bmp;
        }

        public static string SantitizeDriverInput(string input)
        {
            string san = String.Join("", input.Split(' ', '-'));
            if (san != input)
            {
                Console.WriteLine($"[[{input}]] => [[{san}]]");
            }
            /*
            Try common replacements, because im lazy
            */
            if (san == "RIJS")
            {
                return "RUS";
            }

            if (san == "HIJL")
            {
                return "HUL";
            }

            return san;
        }
    }
}