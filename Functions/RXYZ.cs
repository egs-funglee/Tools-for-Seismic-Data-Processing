using System;
using System.Collections.Generic;

namespace CNV2RXY
{
    class RXYZ
    {
        internal static List<RXYZZ> Read_RXYZ(string fpath)
        {
            string[] lines = System.IO.File.ReadAllLines(fpath);
            string filename = System.IO.Path.GetFileNameWithoutExtension(fpath);
            List<RXYZZ> lRXYZ = new List<RXYZZ>();
            char[] chars = new[] { ' ' };

            foreach (string line in lines)
            {
                if (line.Length > 0)
                {
                    string[] ele = line.Split(chars, StringSplitOptions.RemoveEmptyEntries);
                    if (ele.Length == 4)
                    {
                        RXYZZ trxyz = new RXYZZ
                        {
                            linename = filename,
                            r = ele[0],
                            x = ele[1],
                            y = ele[2],
                            z = ele[3],
                            zz = double.Parse(ele[3])
                        };
                        lRXYZ.Add(trxyz);
                    }
                    else
                    {
                        lRXYZ.Clear();
                        break;
                    }
                }
            }
            return lRXYZ;
        }
        internal static List<string> List_RXYZ_to_String(ref List<RXYZZ> lRXYZ)
        {
            List<string> result = new List<string>();
            //return empty result when error
            if (lRXYZ.Count == 0) return result;

            foreach (RXYZZ irxyz in lRXYZ)
                result.Add(string.Format("{0,-7}{1,13}{2,13} {3}", irxyz.r, irxyz.x, irxyz.y, irxyz.z));
            return result;
        }
        internal static List<string> List_RXYZ_to_LXYRT(ref List<RXYZZ> mRXYZ, int vel)
        {
            List<string> result = new List<string>();
            //return empty result when error
            if (mRXYZ.Count == 0) return result;

            foreach (RXYZZ irxyz in mRXYZ)
                result.Add(string.Format("{0}\t{1,13}{2,13}{3,11}{4,13}",
                    irxyz.linename, irxyz.x, irxyz.y, irxyz.r, irxyz.T(vel)));
            return result;
        }
        internal static void List_RXYZ_Append(ref List<RXYZZ> mRXYZ, ref List<RXYZZ> lRXYZ)
        {
            mRXYZ.AddRange(lRXYZ);
        }
        internal static void RXYZZ_Extrapolate(ref List<RXYZZ> lRXYZ)
        {
            //head - copy first valid wd to head
            if (lRXYZ[0].zz == 0)
            {
                string firstwds = string.Empty;
                double firstwd = 0;
                for (int i = 0; i < lRXYZ.Count; i++)
                {
                    if (lRXYZ[i].zz != 0)
                    {
                        firstwds = lRXYZ[i].z;
                        firstwd = lRXYZ[i].zz;
                        break;
                    }
                }

                for (int i = 0; i <= lRXYZ.Count; i++)
                {
                    if (lRXYZ[i].zz != 0) break;
                    lRXYZ[i].z = firstwds;
                    lRXYZ[i].zz = firstwd;
                }
            }

            //tail - copy last valid wd
            if (lRXYZ[lRXYZ.Count - 1].zz == 0)
            {
                string lastwds = string.Empty;
                double lastwd = 0;
                for (int i = lRXYZ.Count - 1; i >= 0; i--)
                {
                    if (lRXYZ[i].zz != 0)
                    {
                        lastwds = lRXYZ[i].z;
                        lastwd = lRXYZ[i].zz;
                        break;
                    }
                }

                for (int i = lRXYZ.Count - 1; i >= 0; i--)
                {
                    if (lRXYZ[i].zz != 0) break;
                    lRXYZ[i].z = lastwds;
                    lRXYZ[i].zz = lastwd;
                }
            }
        }
        internal static void RXYZZ_Interpolate(ref List<RXYZZ> lRXYZ)
        {
            //middle - linear interpolate only by record interval, input can be non-extrapolated

            int init_i;//skip head with zero
            for (init_i = 0; init_i < lRXYZ.Count; init_i++)
                if (lRXYZ[init_i].zz != 0) break;

            for (int i = init_i; i < lRXYZ.Count; i++)
            {
                if (lRXYZ[i].zz == 0)
                {
                    int starti = i, endi = i;
                    double startz = lRXYZ[i - 1].zz, endz = startz;

                    for (int j = starti; j < lRXYZ.Count; j++) //find next valid wd
                    {
                        endi = j;
                        if (lRXYZ[j].zz != 0)
                        {
                            endz = lRXYZ[j].zz;
                            break;
                        }
                    }

                    if (endi == lRXYZ.Count - 1 && lRXYZ[endi].zz == 0) return;

                    if (endi < lRXYZ.Count) //check if still inside the range for zero tail
                    {
                        double stepz = (endz - startz) / (endi - starti + 1);

                        for (int j = starti; j < endi; j++)
                        {
                            lRXYZ[j].zz = lRXYZ[j - 1].zz + stepz;
                            lRXYZ[j].Update_z();
                        }

                        for (int j = starti; j < endi; j++)
                            lRXYZ[j].zz = Math.Round(lRXYZ[j].zz, 2, MidpointRounding.AwayFromZero);
                    }

                    i = endi;//jumper
                }
            }
        }
        internal class RXYZZ //RXYZ object
        {
            public string linename;
            public string r;
            public string x;
            public string y;
            public string z;
            public double zz;
            public void Update_z()
            {
                z = zz.ToString("F2", System.Globalization.CultureInfo.InvariantCulture); //update the string z from double zz
            }
            public string T(int vel)
            {
                double twt = zz / vel * 2000;
                return twt.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            }
            public string Z1()
            {
                double z1 = Math.Round(zz, 1, MidpointRounding.AwayFromZero);
                return z1.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}
