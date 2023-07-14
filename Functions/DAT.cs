using System;
using System.Collections.Generic;

namespace CNV2RXY
{
    class DAT
    {
        internal static List<string> Read_DAT(string fpath)
        {
            string[] lines = System.IO.File.ReadAllLines(fpath);
            List<string> result = new List<string>();
            char[] chars = new[] { ' ' };

            foreach (string line in lines)
            {
                if (line.Length > 0)
                {
                    string[] ele = line.Split(chars, StringSplitOptions.RemoveEmptyEntries);
                    if (ele.Length == 6)
                    {
                        DATL tDATL = new DATL
                        {
                            linename = ele[0],
                            x = double.Parse(ele[1]),
                            y = double.Parse(ele[2]),
                            sp = (int)Math.Round(double.Parse(ele[3]), 0, MidpointRounding.AwayFromZero),
                            tr = (int)Math.Round(double.Parse(ele[4]), 0, MidpointRounding.AwayFromZero),
                            t = double.Parse(ele[5])
                        };
                        if (tDATL.t > 0 && tDATL.t < 9999)
                            result.Add(tDATL.ToStr());
                    }
                    else if (ele.Length == 5)
                    {
                        int first_dot = ele[2].IndexOf('.');
                        string part_x = ele[2].Substring(0, first_dot + 3);
                        string part_sp = ele[2].Substring(first_dot + 3, ele[2].Length - first_dot - 3);
                        DATL tDATL = new DATL
                        {
                            linename = ele[0],
                            x = double.Parse(ele[1]),
                            y = double.Parse(part_x),
                            sp = (int)Math.Round(double.Parse(part_sp), 0, MidpointRounding.AwayFromZero),
                            tr = (int)Math.Round(double.Parse(ele[3]), 0, MidpointRounding.AwayFromZero),
                            t = double.Parse(ele[4])
                        };
                        result.Add(tDATL.ToStr());
                    }
                    else
                    {
                        result.Add("ERROR Cannot decode this line -> " + line);
                        break;
                    }
                }
            }
            return result;
        }

        internal class DATL //DAT Line
        {
            public string linename;
            public double x;
            public double y;
            public int sp;
            public int tr;
            public double t;
            public string ToStr()
            {
                string tt = "1e30";
                if (t > 0 && t < 9999) tt = string.Format("{0:0.0000}", t);
                return string.Format("{0,18} {1,11:0.00} {2,11:0.00} {3,9:0} {4,9:0} {5,9}", linename, x, y, sp, tr, tt);
            }
        }
    }
}
