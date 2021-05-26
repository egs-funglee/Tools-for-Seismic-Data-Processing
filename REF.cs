using System;
using System.Collections.Generic;

namespace CNV2RXY
{
    class REF
    {
        internal static List<string> Read_REF2RXY(string fpath)
        {
            string[] lines = System.IO.File.ReadAllLines(fpath);
            List<string> result = new List<string>();
            char[] chars = new[] { ' ' };

            foreach (string line in lines)
            {
                if (line.Length > 0)
                {
                    string[] ele = line.Split(chars, StringSplitOptions.RemoveEmptyEntries);
                    if (ele.Length > 2)
                    {
                        result.Add(string.Format("{0,-7}{1,13}{2,13}", ele[0], ele[1], ele[2]));
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

        internal static List<REF_LINE> Read_REF(string fpath)
        {
            string[] lines = System.IO.File.ReadAllLines(fpath);
            List<REF_LINE> result = new List<REF_LINE>();
            char[] chars = new[] { ' ' };

            foreach (string line in lines)
            {
                if (line.Length > 0)
                {
                    string[] ele = line.Split(chars, StringSplitOptions.RemoveEmptyEntries);
                    if (ele.Length == 14)
                    {
                        REF_LINE rline = new REF_LINE
                        {
                            r = ele[0],
                            x = ele[1],
                            y = ele[2],
                            h0 = ele[3],
                            h1 = ele[4],
                            h2 = ele[5],
                            h3 = ele[6],
                            h4 = ele[7],
                            h5 = ele[8],
                            h6 = ele[9],
                            h7 = ele[10],
                            h8 = ele[11],
                            h9 = ele[12],
                            h10 = ele[13]
                        };
                        result.Add(rline);
                    }
                }
                else
                {
                    result.Clear();
                    break;
                }
            }
            return result;
        }

        internal class REF_LINE
        {
            public string r;
            public string x;
            public string y;
            public string h0;//water depth
            public string h1;//horizon depth below h0
            public string h2;
            public string h3;
            public string h4;
            public string h5;
            public string h6;
            public string h7;
            public string h8;
            public string h9;
            public string h10;
            public string ToStr()
            {
                return string.Format("{0,8} {1,12} {2,12} {3,7} {4,7} {5,7} {6,7} {7,7} {8,7} {9,7} {10,7} {11,7} {12,7} {13,7}", r, x, y, h0, h1, h2, h3, h4, h5, h6, h7, h8, h9, h10);
            }
        }
    }
}
