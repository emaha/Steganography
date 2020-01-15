using System;

namespace Stg
{
    internal class Program
    {
        internal const int Density = 1;
        internal const int Version = 101;

        private static void Main(string[] args)
        {
            var packer = new Packer();
            packer.Pack(@"C:\Temp\nature.png", @"c:\Temp\AmCre\AmCre-1.mp4", @"c:\Temp\out.png", Density);

            //var unpacker = new Unpacker();
            //unpacker.Unpack(@"c:\Temp\out.png", @"c:\Temp\out", Density);

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}