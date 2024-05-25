using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Graphics.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Text;
using Windows.Graphics.Capture;
using Windows.Storage.Pickers;

namespace f1;

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
        Width = 0.04,
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

class Program
{
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
        { "ALB", new DriverData() {StartPlace = -1} },
        { "ALO", new DriverData() {StartPlace = -1} },
        { "BOT", new DriverData() {StartPlace = -1} },
        { "GAS", new DriverData() {StartPlace = -1} },
        { "HAM", new DriverData() {StartPlace = -1} },
        { "HUL", new DriverData() {StartPlace = -1} },
        { "LEC", new DriverData() {StartPlace = -1} },
        { "MAG", new DriverData() {StartPlace = -1} },
        { "NOR", new DriverData() {StartPlace = -1} },
        { "OCO", new DriverData() {StartPlace = -1} },
        { "PER", new DriverData() {StartPlace = -1} },
        { "PIA", new DriverData() {StartPlace = -1} },
        { "RIC", new DriverData() {StartPlace = -1} },
        { "RUS", new DriverData() {StartPlace = -1} },
        { "SAI", new DriverData() {StartPlace = -1} },
        { "SAR", new DriverData() {StartPlace = -1} },
        { "STR", new DriverData() {StartPlace = -1} },
        { "TSU", new DriverData() {StartPlace = -1} },
        { "VER", new DriverData() {StartPlace = -1} },
        { "ZHO", new DriverData() {StartPlace = -1} },
    };

    static string ABS_IMG_PREFIX = @"C:\Users\rdelo\Documents\csharp\f1\images";

    // dotnet run bounds -2000 -505 1800 1015
    static int SCREEN_X = -2000;
    static int SCREEN_Y = -505;
    static int SCREEN_W = 1800;
    static int SCREEN_H = 1015;

    static int DELAY_TIME = 2000;

    static async Task Main(string[] args)
    {
        var engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en-US"));
        Rectangle bounds = new Rectangle(SCREEN_X, SCREEN_Y, SCREEN_W, SCREEN_H);

        if (args.Length == 0 || args[0] == "help")
        {
            Console.Write("HELP COMING SOON");
        }

        for (int i = 1; i < args.Length; i++)
        {
            string[] vals = args[i].Split("=");
            string cmd = vals[0];

            if (cmd == "-bounds")
            {
                string[] dimensions = vals[1].Split(",");
                try
                {
                    bounds = new Rectangle(
                        Utils.ParseInt(dimensions[0]),
                        Utils.ParseInt(dimensions[1]),
                        Utils.ParseInt(dimensions[2]),
                        Utils.ParseInt(dimensions[3])
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR, INVALID BOUNDS PARAMETERS");
                    return;
                }
            }
            else
            {
                Console.WriteLine($"ARG {vals[0]} NOT VALID");
                return;
            }
        }


        if (args[0] == "capture")
        {
            await RunCapture(engine, bounds);
        }
        else if (args[0] == "validate")
        {
            await RunBoundsTest(engine, bounds);
        }
        else if (args[0] == "test")
        {
            await RunTestMode(engine);
        }
    }

    public async static Task RunCapture(OcrEngine engine, Rectangle bounds)
    {
        while (true)
        {
            Console.WriteLine("");
            string path = @$"{ABS_IMG_PREFIX}\capture.bmp";
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Bitmap bmp = Utils.TakeScreenshot(bounds);
            bmp.Save(path, ImageFormat.Bmp);

            watch = System.Diagnostics.Stopwatch.StartNew();

            BitmapDecoder decoder = await Utils.GetBitmapDecoder(path);

            OcrResult driverResult = await Utils.GetOcrWithCrop(engine, decoder, Percents.Driver);
            OcrResult gapResult = await Utils.GetOcrWithCrop(engine, decoder, Percents.Gap);
            OcrResult intervalResult = await Utils.GetOcrWithCrop(engine, decoder, Percents.Interval);
            watch.Stop();

            ParseOcrResults(driverResult, gapResult, intervalResult);

            // Console.WriteLine($"PROCESSED IMAGE {path} IN {watch.ElapsedMilliseconds}ms");
            System.Threading.Thread.Sleep(DELAY_TIME);
        }
    }

    public async static Task RunBoundsTest(OcrEngine engine, Rectangle bounds)
    {
        Console.WriteLine($"VALIDATING AGAINST BOUNDS {bounds}");
        string path = @$"{ABS_IMG_PREFIX}\bounds.bmp";
        var watch = System.Diagnostics.Stopwatch.StartNew();

        Bitmap bmp = Utils.TakeScreenshot(bounds);
        bmp.Save(path, ImageFormat.Bmp);

        Console.WriteLine($"CAPTURED IMAGE {path} IN {watch.ElapsedMilliseconds}ms");
        watch = System.Diagnostics.Stopwatch.StartNew();

        BitmapDecoder decoder = await Utils.GetBitmapDecoder(path);

        OcrResult driverResult = await Utils.GetOcrWithCrop(engine, decoder, Percents.Driver);
        OcrResult gapResult = await Utils.GetOcrWithCrop(engine, decoder, Percents.Gap);
        OcrResult intervalResult = await Utils.GetOcrWithCrop(engine, decoder, Percents.Interval);
        watch.Stop();

        ParseOcrResults(driverResult, gapResult, intervalResult);

        Console.WriteLine($"PROCESSED IMAGE {path} IN {watch.ElapsedMilliseconds}ms");
    }

    public async static Task RunTestMode(OcrEngine engine)
    {
        int idx = 0;
        while (idx < 10)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            string path = @$"{ABS_IMG_PREFIX}\miami_{idx}.png";
            // var file = await StorageFile.GetFileFromPathAsync(path);
            // var stream = await file.OpenAsync(FileAccessMode.Read);
            // var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
            BitmapDecoder decoder = await Utils.GetBitmapDecoder(path);

            OcrResult driverResult = await Utils.GetOcrWithCrop(engine, decoder, Percents.Driver);
            OcrResult gapResult = await Utils.GetOcrWithCrop(engine, decoder, Percents.Gap);
            OcrResult intervalResult = await Utils.GetOcrWithCrop(engine, decoder, Percents.Interval);
            watch.Stop();

            Console.WriteLine($"PROCESSED IMAGE {path} IN {watch.ElapsedMilliseconds}ms");

            ParseOcrResults(driverResult, gapResult, intervalResult);

            idx++;
            System.Threading.Thread.Sleep(DELAY_TIME);
        }
    }

    public static void ParseOcrResults(OcrResult driverResult, OcrResult gapResult, OcrResult intervalResult)
    {
        OcrLine[] driverLines = driverResult.Lines.ToArray();
        OcrLine[] gapLines = gapResult.Lines.ToArray();
        OcrLine[] intervalLines = intervalResult.Lines.ToArray();

        for (int i = 0; i < driverLines.Length; i++)
        {
            string key = Utils.SantitizeDriverInput(driverLines[i].Text.ToUpper());
            DRIVERS.TryGetValue(key, out DriverData? data);
            if (data == null)
            {
                Console.WriteLine($"Could not parse driver name: [[{key}]]");
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

            Console.WriteLine($"{i + 1} {key} {deltaIcon}{delta} {data.Gap} {data.Interval}");
        }
    }

    public void PrintDriverData()
    {

    }
}
