using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Tesseract;

namespace f1;

class Program
{
    static Dictionary<char, char[]> CHAR_ALTS = new Dictionary<char, char[]>
    {
        {'A', new char[] {'A'}},
        {'B', new char[] {'B'}},
        {'C', new char[] {'C'}},
        {'D', new char[] {'D', 'O'}},
        {'E', new char[] {'E', 'B'}},
        {'F', new char[] {'F'}},
        {'G', new char[] {'G'}},
        {'H', new char[] {'H'}},
        {'I', new char[] {'I'}},
        {'J', new char[] {'J'}},
        {'K', new char[] {'K'}},
        {'L', new char[] {'L'}},
        {'M', new char[] {'M'}},
        {'N', new char[] {'N'}},
        {'O', new char[] {'O'}},
        {'P', new char[] {'P'}},
        {'Q', new char[] {'Q'}},
        {'R', new char[] {'R'}},
        {'S', new char[] {'S'}},
        {'T', new char[] {'T'}},
        {'U', new char[] {'U'}},
        {'V', new char[] {'V'}},
        {'W', new char[] {'W'}},
        {'X', new char[] {'X'}},
        {'Y', new char[] {'Y'}},
        {'Z', new char[] {'Z'}},

    };


    static string[] DRIVERS = {
        "ALB", // 1
        "ALO", // 2
        "BOT", // 3
        "GAS", // 4
        "HAM", // 5
        "HUL", // 6
        "LEC", // 7
        "MAG", // 8
        "NOR", // 9
        "OCO", // 10
        "PER", // 11
        "PIA", // 12
        "RIC", // 13
        "RUS", // 14
        "SAI", // 15
        "SAR", // 16
        "STR", // 17
        "TSU", // 18
        "VER", // 19
        "ZHO", // 20
    };

    static int ROW_DRIVER_START = 2; // First driver row in the content list (probably max lol)\
    // static int COL_NAME = 1;
    // static int COL_PLACE_ICON = 2;
    // static int COL_PLACE_DELTA = 3;
    // static int COL_LEADER = 4;

    static string TEST_IMG_PATH_PREFIX = @"./images/miami_";
    static string TESS_PATH = @"./tessdata";

    static void Main(string[] args)
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
        using var engine = new TesseractEngine(TESS_PATH, "eng", EngineMode.TesseractOnly);

        if (mode == "test")
        {
            RunTestMode(engine);
        }
    }

    public static void RunTestMode(TesseractEngine engine)
    {
        int idx = 0;
        while (idx < 10)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            string path = $"{TEST_IMG_PATH_PREFIX}{idx}.png";
            using var img = Pix.LoadFromFile(path);
            using var page = engine.Process(img);
            string text = page.GetText();
            watch.Stop();

            Console.WriteLine($"PROCESSED IMAGE {path} IN {watch.ElapsedMilliseconds}ms");
            Console.WriteLine($"MEAN CONFIDENCE: {page.GetMeanConfidence()}\n");
            List<string> lines = GetTextLines(text);

            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("");

            List<string> content = ParseContent(lines);

            idx++;
            System.Threading.Thread.Sleep(2000);
        }
    }

    public static List<string> GetTextLines(string s)
    {
        List<string> output = new List<string>();
        string[] lines = s.Split('\n');

        foreach (string line in lines)
        {
            if (line.All(char.IsWhiteSpace))
            {
                continue;
            }
            output.Add(line);
        }

        return output;
    }

    public static List<string> ParseContent(List<string> lines)
    {
        List<string> output = new List<string>();

        string[] drivers = DRIVERS.Select(a => (string)a.Clone()).ToArray();

        for (int i = ROW_DRIVER_START; i < ROW_DRIVER_START + 20; i++)
        {
            string[] items = lines[i].Split(" ");
            Console.WriteLine(String.Join(" | ", items));
            int col = 0;

            string name = "???";
            while (name == "???" && col < items.Length)
            {
                name = FindDriver()
            }
            // var name = MatchString(items[COL_NAME], DRIVERS);

            // Console.WriteLine(name);
        }

        return output;
    }

    public string FindDriver(string[] columns) { }

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

    public static float MatchNumber(string input, string[] samples)
    {
        return 1;
    }
}
