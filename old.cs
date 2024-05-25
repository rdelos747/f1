/*
Just storing old/ failed attempts here
*/


/*
This is the shit I had to do to get tessaract to work :/
*/
// static Dictionary<char, char[]> CHAR_ALTS = new Dictionary<char, char[]>
//     {
//         {'A', new char[] {'A'}},
//         {'B', new char[] {'B'}},
//         {'C', new char[] {'C'}},
//         {'D', new char[] {'D', 'O'}},
//         {'E', new char[] {'E', 'B'}},
//         {'F', new char[] {'F'}},
//         {'G', new char[] {'G'}},
//         {'H', new char[] {'H'}},
//         {'I', new char[] {'I'}},
//         {'J', new char[] {'J'}},
//         {'K', new char[] {'K'}},
//         {'L', new char[] {'L'}},
//         {'M', new char[] {'M'}},
//         {'N', new char[] {'N'}},
//         {'O', new char[] {'O'}},
//         {'P', new char[] {'P'}},
//         {'Q', new char[] {'Q'}},
//         {'R', new char[] {'R'}},
//         {'S', new char[] {'S'}},
//         {'T', new char[] {'T'}},
//         {'U', new char[] {'U'}},
//         {'V', new char[] {'V'}},
//         {'W', new char[] {'W'}},
//         {'X', new char[] {'X'}},
//         {'Y', new char[] {'Y'}},
//         {'Z', new char[] {'Z'}},

//     };


//     static string[] DRIVERS = {
//         "ALB", // 1
//         "ALO", // 2
//         "BOT", // 3
//         "GAS", // 4
//         "HAM", // 5
//         "HUL", // 6
//         "LEC", // 7
//         "MAG", // 8
//         "NOR", // 9
//         "OCO", // 10
//         "PER", // 11
//         "PIA", // 12
//         "RIC", // 13
//         "RUS", // 14
//         "SAI", // 15
//         "SAR", // 16
//         "STR", // 17
//         "TSU", // 18
//         "VER", // 19
//         "ZHO", // 20
//     };

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