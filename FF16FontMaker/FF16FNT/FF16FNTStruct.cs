using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16FontMaker
{
    public class FF16FNTStruct
    {
        public General generalInfo = new();
        public List<CharDesc> charDescList = new();
        public List<KernelDesc> kernelDescList = new();
        public ushort[] idList;
        public UnknownData unknown = new();
        public FF16FNTStruct()
        {

        }

        public class General
        {

            public ushort charsCount;
            public uint kernelsCount;
            //public float size;
            //public float lineHeight;
            public ushort widthImg;
            public ushort heightImg;
            public string texName;
            public string kerName;

            public uint calculateTotalSizeTexKerName()
            {
                int stringLength = this.texName.Length + this.kerName.Length + 2;
                return (uint)(stringLength + Utilities.calculatePadding(stringLength, 16));
            }
        }

        public class CharDesc
        {
            //public uint unk0; // =0
            //public uint unk1;
            public ushort unk0; // = 0
            public ushort kernelCount;
            public uint kernelIndex;
            public float xOffset;
            public float yOffset;
            public float xAdvance;
            public uint unk2; // =0
            public ushort x; // = x * 4
            public ushort y; // = y * 4
            public ushort width; // = width * 4
            public ushort height; // = height * 4

        }
        public class KernelDesc
        {
            public ushort kernelEnd;
            public ushort kernelStart;
            public float amount;
            public uint unk0;
        }
        public class UnknownData
        {
            public uint unkHeader0;
            public ushort unkHeader1;

            public byte[] unkHeader2; // 26 bytes
        }
    }
}
