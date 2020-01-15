using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Stg
{
    public class Packer
    {
        private readonly Random _random = new Random();

        public void Pack(string srcFile, string payloadFile, string outputFile, int density)
        {
            var bitmap = new Bitmap(srcFile);
            var imageData = GetImageData(bitmap);

            var payloadData = GetPayloadData(payloadFile);
            payloadData = Crypto.Crypt(payloadData, "qwe", 1);

            int imageSize = imageData.Length;
            float bytesNeeded = payloadData.Length / density * 8;
            float coverage = bytesNeeded / imageSize * 100f;

            Console.WriteLine($"need: {bytesNeeded}/{imageSize} coverage: {coverage:F1}%");

            if (coverage > 100f)
            {
                Console.WriteLine("Не влезет");
                return;
            }

            var mergedData = Add(imageData, payloadData, density);

            var resultBitmap = GetImageFromData(bitmap, mergedData);

            resultBitmap.Save(outputFile, ImageFormat.Png);
        }

        private byte[] Add(byte[] imageData, byte[] payloadData, int density)
        {
            int size = payloadData.Length;
            byte[] payload = new byte[size + 5];

            payload[0] = Program.Version;
            // Размер
            int pos = 24;
            for (int i = 1; i < 5; i++)
            {
                payload[i] = (byte)(size >> pos);
                pos -= 8;
            }

            payloadData.CopyTo(payload, 5);

            for (var i = 0; i < imageData.Length; i++)
            {
                imageData[i] >>= density;
                imageData[i] <<= density;
            }

            int index = 0;
            int mind = 0;
            foreach (var curByte in payload)
            {
                int offset = 8;
                if (mind > 0)
                {
                    byte a = (byte)(curByte >> 8 - mind);
                    offset -= mind;
                    mind = 0;
                    imageData[index++] |= a;
                }

                while (offset > 0)
                {
                    byte a = (byte)(curByte << 8 - offset);
                    a = (byte)(a >> 8 - offset);

                    if (offset >= density)
                    {
                        a = (byte)(a >> offset - density);
                        imageData[index++] |= a;
                        offset -= density;
                    }
                    else
                    {
                        mind = Math.Abs(offset - density);
                        a = (byte)(a << mind);
                        imageData[index] |= a;
                        break;
                    }
                }
            }

            // Замыливаем конец файла
            int max = (int)Math.Pow(2, density);
            for (int i = index + 1; i < imageData.Length; i++)
            {
                imageData[i] |= (byte)_random.Next(max);
            }

            return imageData;
        }

        private Bitmap GetImageFromData(Bitmap originBitmap, byte[] data)
        {
            var height = originBitmap.Height;
            var width = originBitmap.Width;

            var bitmap = new Bitmap(width, height);

            var index = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var pixel = Color.FromArgb(data[index++], data[index++], data[index++]);
                    bitmap.SetPixel(x, y, pixel);
                }
            }

            return bitmap;
        }

        private byte[] GetPayloadData(string payloadFile)
        {
            byte[] payloadData;

            using (var file = File.OpenRead(payloadFile))
            {
                var payloadLength = (int)file.Length;

                payloadData = new byte[payloadLength];
                file.Read(payloadData, 0, payloadLength);
            }

            return payloadData;
        }

        private byte[] GetImageData(Bitmap bitmap)
        {
            var height = bitmap.Height;
            var width = bitmap.Width;

            var srcData = new byte[height * width * 3];
            var index = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    srcData[index++] = pixel.R;
                    srcData[index++] = pixel.G;
                    srcData[index++] = pixel.B;
                }
            }

            return srcData;
        }
    }
}