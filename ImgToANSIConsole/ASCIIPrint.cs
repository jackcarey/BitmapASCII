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

    private static string ToASCIIStr(System.Drawing.Color c, bool useLong = false)
    {
        //string shortArr = " ░▒▓█";
        string shortArr = " .:-=+*#%@";
        string longArr = " .'`^\",:;Il!i><~+_-?][}{1)(|/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$";
        int idx = (int)Math.Round(c.GetSaturation() * ((useLong ? longArr.Length : shortArr.Length) - 1));
        char ch = useLong ? longArr[idx] : shortArr[idx];
        string res = ch + "" + ch; //double characters up to make them closer to squares
        return res;
    }

    /**
     * Print the BitMap to the console on Windows.
     */
    public static void PrintASCII(this Bitmap src, int minSide=0, bool fit = true,bool useLongArray=false)
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
                Console.Write(ASCIIPrint.ToASCIIStr(c,useLongArray));
            }
            System.Console.WriteLine();
        }
        //reset the console colors
        Console.BackgroundColor = origBg;
        Console.ForegroundColor = origFg;
    }
}
