using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
// using Tesseract;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Graphics.Imaging;
using System.Text;

namespace f1;

class Program
{
    public class Crop
    {
        public double Left;
        public double Top;
        public double Width;
        public double Height;
    }

    static class Percents
    {
        public static Crop Driver = new Crop()
        {
            Left = 0.062,
            Top = 0.12,
            Width = 0.05,
            Height = 0.7,
        };

        public static Crop Gap = new Crop()
        {
            Left = 0.13,
            Top = 0.14,
            Width = 0.05,
            Height = 0.7,
        };

        public static Crop Interval = new Crop()
        {
            Left = 0.18,
            Top = 0.14,
            Width = 0.05,
            Height = 0.7,
        };
    }

    public class DriverData
    {
        // public string Name = "Latifi";
        public int Place;
        public int StartPlace = -1;
        public string Gap = ""; //todo make numeric
        public string Interval = ""; //todo make numeric
        public string LastLap = ""; //todo make numeric
    }

    static Dictionary<string, DriverData> DRIVERS = new Dictionary<string, DriverData>(){
        {"ALB", new DriverData()},
        {"ALO", new DriverData()},
        {"BOT", new DriverData()},
        {"GAS", new DriverData()},
        {"HAM", new DriverData()},
        {"HUL", new DriverData()},
        {"LEC", new DriverData()},
        {"MAG", new DriverData()},
        {"NOR", new DriverData()},
        {"OCO", new DriverData()},
        {"PER", new DriverData()},
        {"PIA", new DriverData()},
        {"RIC", new DriverData()},
        {"RUS", new DriverData()},
        {"SAI", new DriverData()},
        {"SAR", new DriverData()},
        {"STR", new DriverData()},
        {"TSU", new DriverData()},
        {"VER", new DriverData()},
        {"ZHO", new DriverData()},
    };

    static int ROW_DRIVER_START = 2; // First driver row in the content list (probably max lol)\
                                     // static int COL_NAME = 1;
                                     // static int COL_PLACE_ICON = 2;
                                     // static int COL_PLACE_DELTA = 3;
                                     // static int COL_LEADER = 4;

    static string TEST_IMG_PATH_PREFIX = @"C:\Users\rdelo\Documents\csharp\f1\images\miami_";
    // static string TESS_PATH = @"./tessdata";

    static async Task Main(string[] args)
    {
        string mode = "test";
        if (args.Length == 0 || args[0] != "test")
        {
            throw new ArgumentException("ONLY TEST MODE SUPPORTED CURRENTLY");
        }

        /*
        Test results
        EngineMode:
            - TesseractOnly: 0.76
            - Default: 0.56
            - LstmOnly: 0.62
            - TesseractAndLstm: 0.56
        */
        /*
        using var engine = new TesseractEngine(TESS_PATH, "eng", EngineMode.TesseractOnly);


        */
        var engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en-US"));

        if (mode == "test")
        {
            await RunTestMode(engine);
        }
    }

    // public static async void Run(OcrEngine engine)
    // {
    //     var file = await StorageFile.GetFileFromPathAsync(filePath);
    // }

    // public static void RunTestMode(TesseractEngine engine)
    public async static Task<string> RunTestMode(OcrEngine engine)
    {
        int idx = 0;
        while (idx < 10)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            string path = $"{TEST_IMG_PATH_PREFIX}{idx}.png";
            var file = await StorageFile.GetFileFromPathAsync(path);
            var stream = await file.OpenAsync(FileAccessMode.Read);
            var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);

            OcrResult driverResult = await GetOcrWithCrop(engine, decoder, Percents.Driver);
            OcrResult gapResult = await GetOcrWithCrop(engine, decoder, Percents.Gap);
            OcrResult intervalResult = await GetOcrWithCrop(engine, decoder, Percents.Interval);
            watch.Stop();

            Console.WriteLine($"PROCESSED IMAGE {path} IN {watch.ElapsedMilliseconds}ms");

            OcrLine[] driverLines = driverResult.Lines.ToArray();
            OcrLine[] gapLines = gapResult.Lines.ToArray();
            OcrLine[] intervalLines = intervalResult.Lines.ToArray();

            for (int i = 0; i < 20; i++)
            {
                string key = driverLines[i].Text.ToUpper();
                DriverData? data;
                DRIVERS.TryGetValue(key, out data);
                if (data == null)
                {
                    Console.WriteLine($"Could not get for key {key}");
                    continue;
                }
                if (i < gapLines.Length)
                {
                    data.Gap = gapLines[i].Text;
                }
                if (i < intervalLines.Length + 1)
                {
                    if (i > 0)
                    {
                        data.Interval = intervalLines[i - 1].Text;
                    }
                    else
                    {
                        data.Interval = "---";
                    }
                }

                data.Place = i + 1;
                if (data.StartPlace == -1)
                {
                    data.StartPlace = data.Place;
                }

                int delta = data.StartPlace - data.Place;
                string deltaIcon = " ";
                if (delta > 0)
                {
                    deltaIcon = "+";
                }
                else if (delta < 0)
                {
                    deltaIcon = "";
                }

                Console.WriteLine($"{key} {deltaIcon}{delta} {data.Gap} {data.Interval}");
            }

            idx++;
            System.Threading.Thread.Sleep(2000);


        }
        return "hey";
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

    // public static List<string> GetTextLines(string s)
    // {
    //     List<string> output = new List<string>();
    //     string[] lines = s.Split('\n');

    //     foreach (string line in lines)
    //     {
    //         if (line.All(char.IsWhiteSpace))
    //         {
    //             continue;
    //         }
    //         output.Add(line.ToUpper());
    //     }

    //     return output;
    // }

    /*
    The columns can shift between images, so we can't know
    upfront exactly where certain information is. Instead,
    we go down the row and attempt to see if we can locate
    certain info.
    */
    // public static List<string> ParseContent(List<string> lines)
    // {
    //     List<string> output = new List<string>();

    //     string[] drivers = DRIVERS.Select(a => (string)a.Clone()).ToArray();

    //     for (int i = ROW_DRIVER_START; i < ROW_DRIVER_START + 20; i++)
    //     {
    //         string[] items = lines[i].Split(" ");
    //         Console.WriteLine(String.Join(" | ", items));
    //         int col = 0;

    //         // string name = "???";
    //         // while (name == "???" && col < items.Length)
    //         // {
    //         //     name = FindDriver()
    //         // }
    //         // var name = MatchString(items[COL_NAME], DRIVERS);

    //         // Console.WriteLine(name);
    //     }

    //     return output;
    // }

    // public string FindDriver(string[] columns, string[] drivers, ref int startCol)
    // {
    //     /*
    //     This is a bit of a hack. In my tests the
    //     */
    //     // for (int i = startCol; i < 3; i++)
    // }

    // public static void ParseRow(string row)

    // public static string? MatchString(string input, string[] samples)
    // {
    //     input = input.ToUpper();
    //     foreach (string sample in samples)
    //     {
    //         if (input == sample)
    //         {
    //             return sample;
    //         }

    //         /*
    //         I find that tesseract sometimes makes text shorter than it should be.
    //         Eg ive seen Gasly look like "GAva", but Ive never seen text be longer
    //         than it should be. For that reason, we can assume that if the sample is
    //         shorter than the input, we can just move on.
    //         */
    //         if (input.Length > sample.Length)
    //         {
    //             continue;
    //         }

    //         bool found = true;
    //         for (int i = 0; i < input.Length; i++)
    //         {
    //             char[] possibleChars = CHAR_ALTS[input[i]];
    //             if (!possibleChars.Contains(sample[i]))
    //             {
    //                 found = false;
    //             }
    //         }
    //         if (found)
    //         {
    //             return sample;
    //         }
    //     }

    //     return null;
    // }

    // public static float MatchNumber(string input, string[] samples)
    // {
    //     return 1;
    // }
}
