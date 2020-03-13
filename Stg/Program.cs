using System;

namespace Stg
{
    internal class Program
    {
        //internal const int Density = 1;
        internal const int Version = 101;

        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintMan();
                return;
            }

            if (args[0] == "-p")
            {
                if (args.Length < 4)
                {
                    PrintMan();
                    return;
                }

                var sourceFile = args[1];
                var payloadFile = args[2];
                var outputFile = args[3];
                var density = int.Parse(args[4]);

                if (density < 1 || density > 8)
                {
                    PrintMan();
                    return;
                }

                var packer = new Packer();
                packer.Pack(sourceFile, payloadFile, outputFile, density);
                //packer.Pack(@"C:\Temp\nature.png", @"c:\Temp\AmCre\AmCre-1.mp4", @"c:\Temp\out.png", Density);
            }
            else if (args[0] == "-u")
            {
                var sourceFile = args[1];
                var outputFile = args[3];
                var density = int.Parse(args[4]);

                if (density < 1 || density > 8)
                {
                    PrintMan();
                    return;
                }

                var unpacker = new Unpacker();
                unpacker.Unpack(sourceFile, outputFile, density);
                //unpacker.Unpack(@"c:\Temp\out.png", @"c:\Temp\out", Density);
            }
            else
            {
                PrintMan();
                return;
            }

            Console.WriteLine($"{DateTime.Now} - Done");
            //Console.ReadLine();
        }

        private static void PrintMan()
        {
            Console.WriteLine();
            Console.WriteLine("Pack usage: [-p] [sourceFile] [payloadFile] [outputFile] [density]");
            Console.WriteLine("Unpack usage: [-u] [sourceFile] [outputFile] [density]");
            Console.WriteLine("\nDescription:");
            Console.WriteLine("\t\tsourceFile - Image (jpg/png)");
            Console.WriteLine("\t\tpayloadFile - payload");
            Console.WriteLine("\t\toutputFile - Image (jpg/png)");
        }
    }
}