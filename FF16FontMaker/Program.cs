using Mono.Options;
using System.Globalization;
using System.Reflection;

namespace FF16FontMaker
{
    class Program
    {

        static void Main(string[] args)
        {
            string ToolVersion;
            try
            {
                ToolVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                ToolVersion = ToolVersion.Remove(ToolVersion.Length - 2);
            }
            catch
            {
                ToolVersion = "1.0.0";
            }
            string originalFF16FNT = null;
            string fntBMF = null;
            string output = null;
            int customXoffset = 0;
            int customYoffset = -50;
            float customXadvance = 1.4f;
            float customMultiYoffset = 1.4f;
            //string version = null;
            //bool show_list = false;
            string command = null;


            //List<string> SupportedGame = DefaultConfig.GetSupportedList();

            var p = new OptionSet()
            {
                {"fnt2ff16fnt", "Convert Hiero FNT To Final Fantasy 16 FNT",
                v => {command = "fnt2ff16fnt"; } },
                {"ff16fnt2fnt", "Convert Final Fantasy 16 FNT to Hiero FNT",
                v=> {command = "ff16fnt2fnt"; } },
                //{ "l|list", "show list supported games",
                //    v => show_list = v != null }
            };
            p.Parse(args);

            switch (command)
            {
                case "fnt2ff16fnt":
                    p = new OptionSet() {
                { "f|originalFF16FNT=", "(required) Original Final Fantasy 16 FNT file (*.fnt)",
                    v => originalFF16FNT = v },
                { "b|charDesc=", "(required) Character description file from Hiero (*.fnt)",
                    v => fntBMF = v },
                { "o|NewFF16FNT=",
                   "(optional) Output new Final Fantasy 16 FNT file",
                    v => output = v },
                { "CustomXoffset=",
                   "(optional) Custom xoffset, use to fix when font 'left' or 'right' than normal font, usually value [-100,100], default = 0",
                    v => {
                        if (int.TryParse(v, out int result)) {
                            customXoffset = result;
                        } else {
                            Console.WriteLine($"Error: Invalid integer value for CustomXoffset: {v}");
                            return;
                            
                        }
                    } },
                { "CustomYoffset=",
                   "(optional) Custom yoffset, use to fix when font 'upper' or 'lower' than normal font, usually value [-100,100], default = -50",
                    v => {
                        if (int.TryParse(v, out int result)) {
                            customYoffset = result;
                        } else {
                            Console.WriteLine($"Error: Invalid float value for CustomXadvance: {v}");
                            return;
                            
                        }
                    } },
                { "CustomMultiYoffset=",
                   "(optional) Custom multiYoffset, use to fix when some character upper but some character lower, usually value [1.0, 2.0], default = 1.4",
                    v => {
                        if (float.TryParse(v, out float result)) {
                            customMultiYoffset = result;
                        } else {
                            Console.WriteLine($"Error: Invalid float value for customMultiYoffset: {v}");
                            return;
                            
                        }
                    } },
                { "CustomXadvance=",
                   "(optional) Custom xadvance, use to fix when font stretched/narrowed horizontally, usually value [1.0, 2.0], default = 1.4",
                    v => {
                        if (float.TryParse(v, out float result)) {
                            customXadvance = result;
                        } else {
                            Console.WriteLine($"Error: Invalid float value for CustomXadvance: {v}");
                            return;
                            
                        }
                    } },
                };
                    break;
                case "ff16fnt2fnt":
                    p = new OptionSet() {
                //{ "v|version=", "(required) Name of game",
                //   v => version = v  },
                { "f|originalFF16FNT=", "(required) Original Final Fantasy 16 FNT file (*.fnt)",
                    v => originalFF16FNT = v },
                { "o|NewFNT=",
                   "(optional) Output Hiero FNT file",
                    v => output = v },
                };
                    break;
            }
            p.Parse(args);

            if (args.Length == 0 || originalFF16FNT == null || (fntBMF == null && command == "fnt2ff16fnt"))
            {
                ShowHelp(p);
                return;
            }

            if (!originalFF16FNT.EndsWith(".fnt", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Unknown FNT file.");
                ShowHelp(p);
                return;
            }

            if (command == "fnt2ff16fnt")
            {
                if (!fntBMF.EndsWith(".fnt", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Unknown character description file.");
                    ShowHelp(p);
                    return;
                }
            }

            // CreateFF16FNT
            try
            {
                switch (command)
                {
                    case "fnt2ff16fnt":
                        if (output == null)
                            output = originalFF16FNT + ".new";
                        ConverterFunction.CreateFF16FNTfromFNT(originalFF16FNT, fntBMF, output, customXoffset, customYoffset, customXadvance, customMultiYoffset);
                        break;
                    case "ff16fnt2fnt":
                        if (output == null)
                            output = originalFF16FNT + ".fnt";
                        ConverterFunction.ConvertFF16FNTtoFNT(originalFF16FNT, output);
                        break;
                }
                Done();
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }

            void ShowHelp(OptionSet p)
            {
                switch (command)
                {
                    case "fnt2ff16fnt":
                        Console.WriteLine("\nUsage: FF16FontMaker --fnt2ff16fnt [OPTIONS]");
                        break;
                    case "ff16fnt2fnt":
                        Console.WriteLine("\nUsage: FF16FontMaker --ff16fnt2fnt [OPTIONS]");
                        break;
                    default:
                        PrintCredit();
                        Console.WriteLine("\nUsage: FF16FontMaker [OPTIONS]");
                        break;
                }

                Console.WriteLine("Options:");
                p.WriteOptionDescriptions(Console.Out);

                if (command == null)
                {
                    Console.WriteLine("\nExample:");
                    Console.WriteLine("FF16FontMaker --fnt2ff16fnt -f arianlt-demi.fnt -b test.fnt -o arianlt-demi.fnt.new");
                    Console.WriteLine("FF16FontMaker --ff16fnt2fnt -f arianlt-demi.fnt -o arianlt-demi.fnt.fnt");
                    Console.WriteLine("FF16FontMaker --fnt2ff16fnt -f arianlt-demi.fnt -b gen2.fnt --CustomXoffset 0 --CustomYoffset -68 --CustomXadvance 1.4 --CustomMultiYoffset 1.4");
                    Console.WriteLine("\nMore usage: https://github.com/eprilx/FF16FontMaker#usage");
                    Console.Write("More update: ");
                    Console.WriteLine("https://github.com/eprilx/FF16FontMaker/releases");
                }
            }

            void PrintCredit()
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nFF16FontMaker v" + ToolVersion);
                Console.WriteLine(" by eprilx");
                Console.ResetColor();
            }
            void Done()
            {
                Console.Write("\n********************************************");
                PrintCredit();
                Console.WriteLine("********************************************");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n" + output + " has been created!");
                Console.ResetColor();
            }
        }
    }
}