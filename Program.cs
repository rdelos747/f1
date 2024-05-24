using Tesseract;

namespace f1;

class Program
{
    static void Main(string[] args)
    {
        // Console.WriteLine("Hello, World!");
        string imgPath = "./images/fl.png";
        string tessPath = @"./tessdata";
        // using (var engine = new TesseractEngine(tessPath, "eng", EngineMode.Default))
        // {

        // }
        using var engine = new TesseractEngine(tessPath, "eng", EngineMode.Default);
        //using var img = Pix.LoadFromFile(imgPath);
        //using var page = engine.Process(img);
        //var text = page.GetText();

        //Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());
        //Console.WriteLine("Text (GetText): \r\n{0}", text);
    }
}
