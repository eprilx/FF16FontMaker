using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMFont
{
    class BMFontStruct
    {
        public general generalInfo;
        public List<charDesc> charDescList;
        public List<kernelDesc> kernelDescList;

        public void SortCharDescListById()
        {
            this.charDescList.Sort((x, y) => x.id.CompareTo(y.id));
        }
        public BMFontStruct()
        {
            generalInfo = new();
            charDescList = new();
            kernelDescList = new();
        }
        public class general
        {
            public int lineHeight;
            public int _base;
            public int WidthImg; // width image
            public int HeightImg; // height image
            public int pages;

            public string face;
            public int size;
            public int bold;
            public int italic;
            public int charsCount;
            public int kernsCount;
            public List<int> idImg;
            public List<string> fileImg;

            public general()
            {
                face = "";
                size = 0;
                bold = 0;
                italic = 0;
                lineHeight = 0;
                _base = 0;
                idImg = new();
                fileImg = new();
            }
        }
        public class charDesc
        {
            public int id;
            public float x;
            public float y;
            public float width;
            public float height;
            public float xoffset;
            public float yoffset;
            public float xadvance;
            public int page;
            public int chnl;

            public charDesc()
            {
                id = 0;
                x = 0;
                y = 0;
                width = 0;
                height = 0;
                xoffset = 0;
                yoffset = 0;
                xadvance = 0;
                page = 0;
                chnl = 0;
            }
        }
        public class kernelDesc
        {
            public int first;
            public int second;
            public float amount;
        }
    }
}
