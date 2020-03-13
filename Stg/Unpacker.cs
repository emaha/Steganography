using System;
using System.Drawing;
using System.IO;

namespace Stg
{
    public class Unpacker
    {
        public void Unpack(string srcFile, string outputFile, int density)
        {
            var bitmapData = GetBitmapData(srcFile);

            byte[] extractedData = ExtractData(bitmapData, density);
            extractedData = Crypto.Crypt(extractedData, "{^q%w><Wq2YUqDU2aF[a)?+P,yN1>%p:", 1);

            SaveToFile(extractedData, outputFile);
        }

        private byte[] ExtractData(byte[] sourceData, int density)
        {
            byte[] extractedData = new byte[sourceData.Length];

            int offset = 8;
            int index = 0;
            foreach (var curByte in sourceData)
            {
                // Очистка
                byte payload = (byte)(curByte << 8 - density);
                payload = (byte)(payload >> 8 - density);

                if (offset - density >= 0)
                {
                    payload = (byte)(payload << offset - density);
                    extractedData[index] |= payload;
                    offset -= density;

                    if (offset == 0)
                    {
                        offset = 8;
                        index++;
                    }
                }
                else
                {
                    int mind = Math.Abs(offset - density);
                    byte a = (byte)(payload >> mind);
                    extractedData[index++] |= a;

                    offset = 8 - mind;

                    if (mind > 0)
                    {
                        a = (byte)(payload << 8 - mind);
                        extractedData[index] |= a;
                    }
                }
            }

            int size = 0;
            int pos = 24;
            int version = extractedData[0];
            for (int i = 1; i < 5; i++)
            {
                size |= extractedData[i] << pos;
                pos -= 8;
            }

            byte[] truncatedData = new byte[size];
            Array.Copy(extractedData, 5, truncatedData, 0, size);

            return truncatedData;
        }

        private void SaveToFile(byte[] data, string path)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                ms.CopyTo(file);
            }
        }

        private byte[] GetBitmapData(string path)
        {
            Bitmap bitmap = new Bitmap(path);
            int height = bitmap.Height;
            int width = bitmap.Width;

            byte[] data = new byte[height * width * 3];
            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    data[index++] = pixel.R;
                    data[index++] = pixel.G;
                    data[index++] = pixel.B;
                }
            }

            return data;
        }
    }
}