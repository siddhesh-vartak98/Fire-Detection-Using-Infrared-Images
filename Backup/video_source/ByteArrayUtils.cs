namespace AForge.Video
{
    using System;

    internal static class ByteArrayUtils
    {
        public static bool Compare(byte[] array, byte[] needle, int startIndex)
        {
            int length = needle.Length;
            int index = 0;
            for (int i = startIndex; index < length; i++)
            {
                if (array[i] != needle[index])
                {
                    return false;
                }
                index++;
            }
            return true;
        }

        public static int Find(byte[] array, byte[] needle, int startIndex, int sourceLength)
        {
            int length = needle.Length;
            while (sourceLength >= length)
            {
                int num2 = Array.IndexOf<byte>(array, needle[0], startIndex, (sourceLength - length) + 1);
                if (num2 == -1)
                {
                    return -1;
                }
                int index = 0;
                for (int i = num2; index < length; i++)
                {
                    if (array[i] != needle[index])
                    {
                        break;
                    }
                    index++;
                }
                if (index == length)
                {
                    return num2;
                }
                sourceLength -= (num2 - startIndex) + 1;
                startIndex = num2 + 1;
            }
            return -1;
        }
    }
}

