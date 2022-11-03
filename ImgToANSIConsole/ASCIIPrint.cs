using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

/**
 * Extension class to allow BitMap objects to be printed to the console on Windows. Default size: 80 columns
 * */
public static class ASCIIPrint
{
    //modified from: https://stackoverflow.com/questions/33538527/display-a-image-in-a-console-application


    // Map a Color to the ConsoleColor Enum
    private static int ToConsoleColor(System.Drawing.Color c)
    {
        //TODO: improve color mapping somehow
        int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0;
        index |= (c.R > 64) ? 4 : 0;
        index |= (c.G > 64) ? 2 : 0;
        index |= (c.B > 64) ? 1 : 0;
        return index;
    }

    private static string GetShadingArray(bool useLong = false)
    {
        //string shortArr = " ░▒▓█";
        string shortArr = " .:-=+*#%@";
        string longArr = " .'`^\",:;Il!i><~+_-?][}{1)(|/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$";
        return useLong ? longArr : shortArr;
    }

    //used in DominantConsoleColor to determine a pixels weight based on how visible it is
    private static float GetShadingWeight(Color c, bool useLong = false)
    {
        char ch = ToASCIIStr(c, useLong)[0];
        string arr = GetShadingArray(useLong);
        int idx = arr.IndexOf(ch);
        float pct = arr.Length+1 / (idx + 1);
        //ignore blank pixels or illegal characters
        return idx == -1 || ch == arr[0] ? 0 : pct;
    }

    private static string ToASCIIStr(System.Drawing.Color c, bool useLong = false)
    {
        string arr = GetShadingArray(useLong);
        int idx = (int)Math.Round(c.GetSaturation() * (arr.Length - 1));
        char ch = arr[idx];
        string res = ch + "" + ch; //double characters up to make them closer to squares
        return res;
    }

    /**
     * Print the BitMap to the console on Windows.
     */
    public static void PrintASCII(this Bitmap src, int minSide = 0, bool fit = true, bool useLongArray = false)
    {
        ConsoleColor origBg = Console.BackgroundColor;
        ConsoleColor origFg = Console.ForegroundColor;

        if (minSide <= 0)
        {
            minSide = Math.Min(src.Width, src.Height);

        }

        if (fit)
        {
            minSide = Math.Min(minSide, Math.Min(Console.WindowWidth, Console.WindowHeight));
        }


        decimal minW = decimal.Divide(minSide, src.Width);
        decimal minH = decimal.Divide(minSide, src.Height);
        decimal pct = Math.Min(minW, minH);
        int newW = (int)(src.Width * pct);
        int newH = (int)(src.Height * pct);

        Bitmap bmpMin = new Bitmap(src, newW, newH);
        for (int i = 0; i < newH; i++)
        {
            for (int j = 0; j < newW; j++)
            {
                Color c = bmpMin.GetPixel(j, i);
                Console.ForegroundColor = (ConsoleColor)ASCIIPrint.ToConsoleColor(c);
                Console.Write(ASCIIPrint.ToASCIIStr(c, useLongArray));
            }
            System.Console.WriteLine();
        }
        //reset the console colors
        Console.BackgroundColor = origBg;
        Console.ForegroundColor = origFg;
    }

    public static ConsoleColor[] DominantConsoleColors(this Bitmap src, int num = 0, bool useLong = false)
    {
        if (num == 0)
        {
            num = 16;
        }
        num = Math.Max(1, num);
        Dictionary<ConsoleColor, float> colors = new Dictionary<ConsoleColor, float>();
        for (int i = 0; i < src.Height - 1; i++)
        {
            for (int j = 0; j < src.Width - 1; j++)
            {
                Color c = src.GetPixel(j, i);
                ConsoleColor col = (ConsoleColor)ASCIIPrint.ToConsoleColor(c);
                float weight = GetShadingWeight(c, useLong);
                //ignore blank pixels
                if (weight > 0)
                {
                    if (!colors.ContainsKey(col))
                    {
                        colors.Add(col, weight);
                    }
                    else
                    {
                        colors[col] += weight;
                    }
                }
            }
        }
        List<KeyValuePair<ConsoleColor, float>> ordered = colors.OrderByDescending(c => c.Value).ToList();
        return ordered.Take(Math.Min(colors.Count, num)).Select(x => (ConsoleColor)x.Key).ToArray();
    }

    public static ConsoleColor DominantConsolColor(this Bitmap src)
    {
        return src.DominantConsoleColors(1)[0];
    }
}
