using System;
using System.Collections.Generic;

namespace CNV2RXY
{
    class RXY
    {
        internal static List<string> Read_CNV2RXY(string fpath)
        {
            string[] lines = System.IO.File.ReadAllLines(fpath);
            List<string> result = new List<string>();
            char[] chars = new[] { ' ' };

            foreach (string line in lines)
            {
                if (line.Length > 0)
                {
                    string[] ele = line.Split(chars, StringSplitOptions.RemoveEmptyEntries);
                    if (ele.Length > 4)
                    {
                        result.Add(string.Format("{0,-7}{1,13}{2,13}", ele[2], ele[3], ele[4]));
                    }
                    else
                    {
                        result.Clear();
                        break;
                    }
                }
            }
            return result;
        }

        internal static List<string> Read_PC2RXY(string fpath)
        {
            string[] lines = System.IO.File.ReadAllLines(fpath);
            List<string> result = new List<string>();
            char[] chars = new[] { ' ', '|', '/', 'E', 'M', 'N', 'S' };

            foreach (string line in lines)
            {
                if (!line.StartsWith("|") && !line.StartsWith("-") && line.Length > 0)
                {
                    string[] ele = line.Split(chars, StringSplitOptions.RemoveEmptyEntries);
                    if (ele.Length == 65)
                    {
                        result.Add(string.Format("{0,-7}{1,13}{2,13}", ele[0], ele[63], ele[64]));
                    }
                    else
                    {
                        result.Clear();
                        break;
                    }
                }
            }
            return result;
        }
    }
}
