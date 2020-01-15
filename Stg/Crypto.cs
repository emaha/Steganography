namespace Stg
{
    public class Crypto
    {
        // Simple XOR
        public static byte[] Crypt(byte[] data, string pass, int iterations)
        {
            byte[] result = new byte[data.Length];
            data.CopyTo(result, 0);
            int index = 0;
            for (int iter = 0; iter < iterations; iter++)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    result[i] ^= (byte)pass[index++];
                    if (index == pass.Length) index = 0;
                }
            }
            return result;
        }
    }
}