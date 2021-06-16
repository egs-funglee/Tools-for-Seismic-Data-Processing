using System;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;


namespace CNV2RXY
{
    class INI
    {
        internal static void Save_ini(IniData ini)
        {
            FileIniDataParser parser = new FileIniDataParser();
            parser.WriteFile(@"C:\EGS\Tools_for_Seismic_Data_Processing.ini", ini);
        }

        internal static IniData Read_ini()
        {
            FileIniDataParser parser = new FileIniDataParser();
            string settingfile = @"C:\EGS\Tools_for_Seismic_Data_Processing.ini";
            if (System.IO.File.Exists(settingfile))
                return parser.ReadFile(settingfile);
            else
                return new IniData();
        }
    }
}
