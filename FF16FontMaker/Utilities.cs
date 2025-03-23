using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Utilities
{
    public static string StringBetween(string STR, string FirstString, string LastString)
    {
        string FinalString;
        int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
        int Pos2 = STR.IndexOf(LastString, Pos1);
        if (Pos2 == -1)
        {
            Pos2 = STR.LastIndexOf(STR.Last()) + 1;
        }
        FinalString = STR[Pos1..Pos2];
        return FinalString;
    }

    public static (float, float, float, float) getUVmappingFromPoint(float x, float y, float width, float height, int WidthImg, int HeightImg)
    {
        float UVLeft = x / (float)WidthImg;
        float UVTop = y / (float)HeightImg;
        float UVRight = (x + width) / (float)WidthImg;
        float UVBottom = (y + height) / (float)HeightImg;
        return (UVLeft, UVTop, UVRight, UVBottom);
    }

    public static (float, float, float, float) getPointFromUVmapping(float UVLeft, float UVTop, float UVRight, float UVBottom, int WidthImg, int HeightImg)
    {
        float x = UVLeft * WidthImg;
        float y = UVTop * HeightImg;
        float width = (UVRight * WidthImg) - x;
        float height = (UVBottom * HeightImg) - y;
        return (x, y, width, height);
    }

    public static int intScaleInt(int number, float Scale)
    {
        return (int)((float)number * Scale);
    }
    public static int floatScaleInt(float number, float Scale)
    {
        return (int)((float)number * Scale);
    }

    public static float floatRevScale(float number, float Scale)
    {
        return ((float)number / Scale);
    }

    public static int calculatePadding(int length, int paddingSize)
    {

        int remainder = length % paddingSize;
        return remainder == 0 ? 0 : paddingSize - remainder;
    }
}
