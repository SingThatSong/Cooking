using System;

namespace Cooking.WPF.Services
{
    public static class StringCompare
    {
        public static int DiffLength(string str1, string str2)
        {
            str1 = str1.ToUpperInvariant();
            str2 = str2.ToUpperInvariant();

            if (str1.Equals(str2, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            int m = str1.Length, n = str2.Length;

            int[,] E = new int[m + 1, n + 1];

            int Diff(int x, int y) => str1[x] == str2[y] ? 0 : 1;

            for (int i = 0; i < m + 1; i++)
            {
                E[i, 0] = i;
            }

            for (int j = 0; j < n + 1; j++)
            {
                E[0, j] = j;
            }

            for (int i = 1; i < m + 1; i++)
            {
                for (int j = 1; j < n + 1; j++)
                {
                    E[i, j] = Math.Min(
                                Math.Min(E[i - 1, j] + 1,
                                E[i, j - 1] + 1),
                                E[i - 1, j - 1] + Diff(i - 1, j - 1)
                              );
                }
            }

            return E[m, n];
        }
    }
}