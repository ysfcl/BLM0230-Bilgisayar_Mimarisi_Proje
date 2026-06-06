using System;
using System.Text;

namespace BLM230_Proje
{
    public static class HammingEncoder
    {
        public static string Encode(string dataBits)
        {
            int dataLength = dataBits.Length;
            int r = 0;

            while (Math.Pow(2, r) < (dataLength + r + 1))
            {
                r++;
            }

            int totalLength = dataLength + r + 1;
            char[] hammingCode = new char[totalLength + 1];

            for (int i = 1; i <= totalLength; i++)
            {
                hammingCode[i] = '0';
            }

            int j = 0;
            for (int i = 1; i <= totalLength; i++)
            {
                if (!IsPowerOfTwo(i) && i != 1)
                {
                    if (j < dataBits.Length)
                    {
                        hammingCode[i] = dataBits[j++];
                    }
                }
            }

            for (int i = 0; i < r; i++)
            {
                int parityPos = (int)Math.Pow(2, i);
                int parity = 0;
                for (int k = 1; k <= totalLength; k++)
                {
                    if (((k >> i) & 1) == 1 && k != parityPos)
                    {
                        if (hammingCode[k] == '1') parity ^= 1;
                    }
                }
                hammingCode[parityPos] = (char)(parity + '0');
            }

            int overall = 0;
            for (int i = 2; i <= totalLength; i++)
            {
                if (hammingCode[i] == '1') overall ^= 1;
            }
            hammingCode[1] = (char)(overall + '0');

            // C# string constructor mantığı Java'dan farklıdır, array'i stringe çevirip substring alıyoruz
            return new string(hammingCode).Substring(1, totalLength);
        }

        public static string DetectAndCorrect(string code, out int detectedPosition)
        {
            detectedPosition = 0;
            int n = code.Length;
            int r = 0;
            while ((int)Math.Pow(2, r) < n) r++;

            int syndrome = 0;

            for (int i = 0; i < r; i++)
            {
                int parityPos = (int)Math.Pow(2, i);
                int parity = 0;

                for (int k = 1; k <= n; k++)
                {
                    if (((k >> i) & 1) == 1)
                    {
                        if (code[k - 1] == '1')
                            parity ^= 1;
                    }
                }

                if (parity == 1)
                {
                    syndrome += parityPos;
                }
            }

            StringBuilder sb = new StringBuilder(code);

            if (syndrome == 0)
            {
                return "Hata tespit edilmedi.\nVeri dogru.\nKod:\n" + code;
            }
            if (syndrome <= n)
            {
                sb[syndrome - 1] = (sb[syndrome - 1] == '0') ? '1' : '0';
                detectedPosition = syndrome; // Pozisyonu dışarı fırlatıyoruz ki GUI'de yeşil yapabilelim
                return "Hata tespit edildi! Pozisyon: " + syndrome + "\nDuzeltilmis Kod:\n" + sb.ToString();
            }

            return "Cift hata veya gecersiz hata bulundu ve duzeltilemedi.";
        }

        private static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }
    }
}