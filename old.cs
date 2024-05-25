/*
Just storing old/ failed attempts here
*/


// public static List<string> ParseContent(List<string> lines)
// {
//     List<string> output = new List<string>();

//     for (int i = ROW_DRIVER_START; i < ROW_DRIVER_START + 20; i++)
//     {
//         string[] items = lines[i].Split(" ");
//         Console.WriteLine(String.Join(" | ", items));
//         var name = MatchString(items[COL_NAME], DRIVERS);
//         Console.WriteLine(name);
//     }

//     return output;
// }

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