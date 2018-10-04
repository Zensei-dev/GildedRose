using System;
using System.IO;

namespace GildedRose
{
    class Program
    {
        static void Main(string[] args)
        {
            // Going to make the CLI part really simple for this exercise
            // Expect 1 arg: filePath
            // USAGE: GildedRose.exe path/to/inputfile.txt

            var cliArgs = new CliArgs(args);

            if (cliArgs.AreValid())
            {
                var goods = new GoodsFileReader().GetGoods(cliArgs.InputFilePath);
                new FinestGoodUpdater().UpdateGoods(goods);
                new OutputFinestGoodsCli().OutputGoods(goods);
            }
            // cliArgs prints errors
        }
    }

    public class CliArgs
    {
        private string[] _args;

        public bool HasCorrectNumberArgs { get; private set; }

        public string InputFilePath { get; private set; }
        public bool InputFilePathExists { get; private set; }

        public CliArgs(string[] args)
        {
            _args = args;

            HasCorrectNumberArgs = (_args.Length == 1);

            if (HasCorrectNumberArgs)
                InputFilePath = _args[0];
            // We are expecting caller to be diligent and check bools first, or they could get an invalid file path
            // I won't address that scenario currently since we won't do that and this is an exercise

            InputFilePathExists = (File.Exists(InputFilePath));
        }

        public bool AreValid()
        {
            if (!HasCorrectNumberArgs)
            {
                Console.WriteLine("Incorrect number of arguments supplied");
                PrintHelp();
                return false;
            }

            if (!InputFilePathExists)
            {
                Console.WriteLine("Invalid input file path: " + InputFilePath);
                return false;
            }
            return true;
        }

        public void PrintHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("GILDED ROSE");
            Console.WriteLine("-----------");
            Console.WriteLine("Given an input file, performs one Daily Update of inventory management.");
            Console.WriteLine("SellIn and Quality values for listed inventory items are updated according to Inventory Management Rules.");
            Console.WriteLine("");
            Console.WriteLine("USAGE:");
            Console.WriteLine("-----------");
            Console.WriteLine("gildedrose inputfile.txt");
            Console.WriteLine();
            Console.WriteLine("INPUT FILE FORMAT:");
            Console.WriteLine("-----------");
            Console.WriteLine("1 inventory item per line in the format: ITEM TYPE | SELLIN | QUALITY");
            Console.WriteLine("E.g.: \"Aged Bried 1 1\"");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("OUTPUT");
            Console.WriteLine("-----------");
            Console.WriteLine("An output of updated values will be returned to the console window");
        }
    }
}
