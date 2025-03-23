using Gibbed.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16FontMaker
{
    public class FF16FNTFormat : FF16FNTStruct
    {
        public static FF16FNTStruct Load(string inputFF16FNT)
        {
            var input = File.OpenRead(inputFF16FNT);

            string magic = input.ReadString(4);

            if (magic != "FNT ")
            {
                throw new Exception("Unsupported format, make sure it's fnt extracted from final fantasy 16");
            }

            FF16FNTStruct FF16FNT = new();

            input.Position = 0;

            //Read header
            ReadHeader(input, ref FF16FNT);

            ReadTableID(input, ref FF16FNT);

            ReadTableCharDesc(input, ref FF16FNT);

            //ReadKernel(input, ref FF16FNT);

            input.Close();

            return FF16FNT;
        }


        private static void ReadHeader(FileStream input, ref FF16FNTStruct FF16FNT)
        {
            input.Position = 0;
            string magic = input.ReadString(4);
            FF16FNT.unknown.unkHeader0 = input.ReadValueU32();
            uint totalSizeTexKerName = input.ReadValueU32(); // = sizeTexName*2 + 16 -  (sizeTexName*2) % 16
            uint sizeFontData = input.ReadValueU32(); // = FileSize - 64 - totalSizeTexKerName = 65536 * 2 + charsCount * 32
            FF16FNT.unknown.unkHeader1 = input.ReadValueU16();
            FF16FNT.generalInfo.charsCount = input.ReadValueU16();
            FF16FNT.generalInfo.kernelsCount = input.ReadValueU32();
            FF16FNT.unknown.unkHeader2 = input.ReadBytes(22); // unk 22 bytes
            FF16FNT.generalInfo.widthImg = input.ReadValueU16();
            FF16FNT.generalInfo.heightImg = input.ReadValueU16();
            uint sizeTexName = input.ReadValueU32();
            input.ReadBytes(10); // 10 zero bytes

            FF16FNT.generalInfo.texName = input.ReadStringZ();

            FF16FNT.generalInfo.kerName = input.ReadStringZ();

            // read padding

            input.ReadBytes(Utilities.calculatePadding((int)sizeTexName * 2, 16));

        }

        public static void WriteHeader(FileStream output, FF16FNTStruct FF16FNT)
        {
            output.Position = 0;
            output.WriteString("FNT ");
            output.WriteValueU32(FF16FNT.unknown.unkHeader0);
            uint totalSizeTexKerName = FF16FNT.generalInfo.calculateTotalSizeTexKerName();
            output.WriteValueU32(totalSizeTexKerName);
            output.WriteValueU32(65536 * 2 + (uint)FF16FNT.generalInfo.charsCount * 32);
            output.WriteValueU16(FF16FNT.unknown.unkHeader1);
            output.WriteValueU16(FF16FNT.generalInfo.charsCount);
            output.WriteValueU32(FF16FNT.generalInfo.kernelsCount);
            output.WriteBytes(FF16FNT.unknown.unkHeader2); // unk 22 bytes
            output.WriteValueU16(FF16FNT.generalInfo.widthImg);
            output.WriteValueU16(FF16FNT.generalInfo.heightImg);
            output.WriteValueS32(FF16FNT.generalInfo.texName.Length + 1);

            output.WriteBytes(new byte[10]);

            output.WriteStringZ(FF16FNT.generalInfo.texName);
            output.WriteStringZ(FF16FNT.generalInfo.kerName);
            output.WriteBytes(new byte[totalSizeTexKerName - (FF16FNT.generalInfo.texName.Length + 1)*2]);
        }


        private static void ReadTableID(FileStream input, ref FF16FNTStruct FF16FNT)
        {
            
            int startPos = (int)input.Position;
            FF16FNT.idList = new ushort[FF16FNT.generalInfo.charsCount];
            ushort baseId = 0;

            while ((input.Position - startPos) / 2 <= 0xFFFF)
            {
                ushort idx = input.ReadValueU16();

                if (idx != 0)
                {
                    FF16FNT.idList[idx] = baseId;

                }
                baseId += 1;
            }
            FF16FNT.idList[0] = (ushort)(FF16FNT.idList[1] - 1);

        }

        public static void WriteTableID(FileStream output, ushort[] idList)
        {
            ushort[] tableID = new ushort[0xFFFF + 1];
            ushort baseID = 0;
            foreach (ushort id in idList)
            {
                tableID[id] = baseID;
                baseID += 1;
            }
            foreach (ushort value in tableID)
            {
                output.WriteValueU16(value);
            }
        }

        private static void ReadTableCharDesc(FileStream input, ref FF16FNTStruct FF16FNT)
        { 
            for (int i = 0; i < FF16FNT.generalInfo.charsCount; i++)
            {
                FF16FNT.charDescList.Add(new CharDesc
                {
                    unk0 = input.ReadValueU16(),
                    kernelCount = input.ReadValueU16(),
                    kernelIndex = input.ReadValueU32(),
                    xOffset = input.ReadValueF32(),
                    yOffset = input.ReadValueF32(),
                    xAdvance = input.ReadValueF32(),
                    unk2 = input.ReadValueU32(),
                    x = input.ReadValueU16() ,
                    y = input.ReadValueU16(),
                    width = input.ReadValueU16(),
                    height = input.ReadValueU16(),
                });
            }
        }

        public static void WriteTableCharDesc(FileStream output, FF16FNTStruct FF16FNT)
        {
            foreach (CharDesc _char in FF16FNT.charDescList)
            {
                output.WriteBytes(new byte[8]); // hiero always generate none kernel so just fill 0
                output.WriteValueF32(_char.xOffset);
                output.WriteValueF32(_char.yOffset);
                output.WriteValueF32(_char.xAdvance);
                output.WriteValueU32(0);
                output.WriteValueU16(_char.x);
                output.WriteValueU16(_char.y);
                output.WriteValueU16(_char.width);
                output.WriteValueU16(_char.height);
            }
            // Write padding 
            output.WriteBytes(new byte[Utilities.calculatePadding((int)output.Position, 16)]);
        }

        private static void ReadKernel(FileStream input, ref FF16FNTStruct FF16FNT)
        {
            for (int i = 0; i < FF16FNT.generalInfo.kernelsCount; i++)
            {
                FF16FNT.kernelDescList.Add(new KernelDesc
                {
                    kernelEnd = input.ReadValueU16(),
                    kernelStart = input.ReadValueU16(),
                    amount = input.ReadValueF32(),
                    unk0 = input.ReadValueU32(),
                });
            }
        }
    }
}
