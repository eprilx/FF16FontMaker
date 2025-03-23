using BMFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16FontMaker
{
    public static class ConverterFunction
    {
        public static void ConvertFF16FNTtoFNT(string inputFF16FNT, string outputFNT)
        {
            //Load FF16FNT
            Console.Write("Load FF16FNT... ");
            FF16FNTStruct FF16FNT = FF16FNTFormat.Load(inputFF16FNT);
            Console.WriteLine("Success");

            // create BMF
            BMFontStruct bmf = new();

            Console.Write("Convert FF16FNT to FNT... ");
            //convert infoFF16FNT 2 infoBMF
            bmf.generalInfo.charsCount = (int)FF16FNT.generalInfo.charsCount;

            bmf.generalInfo.pages = 1;
            for (int i = 0; i < bmf.generalInfo.pages; i++)
            {
                bmf.generalInfo.idImg.Add(i);
                bmf.generalInfo.fileImg.Add(FF16FNT.generalInfo.texName);
            }
            bmf.generalInfo.WidthImg = (int)FF16FNT.generalInfo.widthImg / 4;
            bmf.generalInfo.HeightImg = (int)FF16FNT.generalInfo.heightImg / 4;

            //convert charDescFF16FNT 2 charDescBMF
            foreach (FF16FNTStruct.CharDesc charFF16FNT in FF16FNT.charDescList)
            {

                BMFontStruct.charDesc charBMF = new();

                charBMF.x = charFF16FNT.x / 4;
                charBMF.y = charFF16FNT.y / 4;
                charBMF.width = charFF16FNT.width / 4;
                charBMF.height =  charFF16FNT.height / 4;
                charBMF.xoffset = charFF16FNT.xOffset;
                charBMF.yoffset = charFF16FNT.yOffset;
                charBMF.xadvance = charFF16FNT.xAdvance;
                //charBMF.yoffset = FF16FNT.generalInfo.lineHeight - charFF16FNT.bearingY1_1 * FF16FNT.generalInfo.size;
                charBMF.page = 0;
                bmf.charDescList.Add(charBMF);


            }

            // convert idList
            int count = 0;
            foreach (ushort id in FF16FNT.idList)
            {
                bmf.charDescList[count].id = id;
                count += 1;
            }

            // convert kernel
            //int idx = 1;
            //foreach (FF16FNTStruct.CharDesc charFF16FNT in FF16FNT.charDescList)
            //{

            //    for (int i = 0; i < charFF16FNT.kernelCount; i++)
            //    {
            //        BMFontStruct.kernelDesc kernelBMF = new();
            //        kernelBMF.first = FF16FNT.idList[idx];
            //        kernelBMF.second = FF16FNT.kernelDescList[(int)charFF16FNT.kernelIndex + i].kernelStart;
            //        kernelBMF.amount = FF16FNT.kernelDescList[(int)charFF16FNT.kernelIndex + i].amount;
            //        bmf.kernelDescList.Add(kernelBMF);

            //    }
            //    idx += 1;
                
            //}

            //bmf.generalInfo.kernsCount = bmf.kernelDescList.Count;


            Console.WriteLine("Success");

            Console.Write("Create FNT... ");
            BMFontFormat.CreateText(outputFNT, bmf);
            Console.WriteLine("Success");

            Console.WriteLine("ALL SUCCESS");
        }


        public static void CreateFF16FNTfromFNT(string inputFF16FNT, string inputBMF, string outputFF16FNT, int customXoffset, int customYoffset, float customXadvance, float customMultiYoffset)
        {
            //Load FF16FNT
            Console.Write("Load FF16FNT... ");
            FF16FNTStruct FF16FNT = FF16FNTFormat.Load(inputFF16FNT);
            Console.WriteLine("Success");

            //Load BMFont
            Console.Write("Load FNT... ");
            BMFontStruct bmf = BMFontFormat.Load(inputBMF);
            Console.WriteLine("Success");

            Console.Write("Convert FNT to FF16FNT... ");
            //Create FF16FNT
            var output = File.Create(outputFF16FNT);

            bool haveChar0 = bmf.charDescList.Any(charDesc => charDesc.id == 0);

            if (!haveChar0)
            {
                // add init char 0
                BMFontStruct.charDesc charr = new BMFontStruct.charDesc {};
                bmf.charDescList.Insert(0, charr);
                bmf.generalInfo.charsCount += 1;
            }

            //convert infoBMF 2 infoFF16FNT
            FF16FNT.generalInfo.charsCount = (ushort)bmf.generalInfo.charsCount;

            FF16FNT.generalInfo.widthImg = (ushort)(bmf.generalInfo.WidthImg * 4);
            FF16FNT.generalInfo.heightImg = (ushort)(bmf.generalInfo.HeightImg * 4);

            // write header
            FF16FNTFormat.WriteHeader(output, FF16FNT);
            //convert charDescBMF 2 charDescFF16FNT
            FF16FNT.charDescList.Clear();

            // convert idList
            List<ushort> idList = new();
            foreach (BMFontFormat.charDesc _char in bmf.charDescList)
            {
                idList.Add((ushort)_char.id);
            }
            FF16FNT.idList = idList.ToArray();
            FF16FNTFormat.WriteTableID(output, FF16FNT.idList);

            foreach (BMFontStruct.charDesc charBMF in bmf.charDescList)
            {
                FF16FNTStruct.CharDesc charFF16FNT = new();

                charFF16FNT.x = (ushort)(charBMF.x * 4);
                charFF16FNT.y = (ushort)(charBMF.y * 4);
                charFF16FNT.width = (ushort)(charBMF.width * 4);
                charFF16FNT.height = (ushort)(charBMF.height * 4);
                charFF16FNT.xOffset = charBMF.xoffset + customXoffset;
                charFF16FNT.yOffset = (float)(charBMF.yoffset*customMultiYoffset + customYoffset);
                charFF16FNT.xAdvance = (float)(charBMF.xadvance * customXadvance);

                if (charBMF.id == 32 || charBMF.id == 9 || charBMF.id == 10 || charBMF.id == 13)
                {
                    charFF16FNT.x = 0;

                    charFF16FNT.y = 0;

                    charFF16FNT.width = 0;

                    charFF16FNT.height = 0;

                }
                FF16FNT.charDescList.Add(charFF16FNT);
            }
            FF16FNTFormat.WriteTableCharDesc(output, FF16FNT);



            Console.WriteLine("Success");


            output.Close();

            Console.WriteLine("ALL SUCCESS");
        }
    
    }
}
