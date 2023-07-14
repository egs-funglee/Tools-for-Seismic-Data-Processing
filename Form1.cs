using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static CNV2RXY.DAT;
using static CNV2RXY.INI;
using static CNV2RXY.RXY;
using static CNV2RXY.RXYZ;

namespace CNV2RXY
{
    public partial class Form1 : Form
    {
        int mode = 0;
        private readonly IniParser.Model.IniData ini;
        public Form1()
        {
            InitializeComponent();
            ini = Read_ini();
            string string_mode = ini["Software"]["Mode"];
            string string_search = ini["Search and Replace History"]["Search"];
            string string_replace = ini["Search and Replace History"]["Replace"];
            string string_move_or_copy = ini["Search and Replace History"]["Move/Copy"];
            string string_overwrite = ini["Search and Replace History"]["Overwrite"];
            string string_replace_ext = ini["Search and Replace History"]["Replace_Ext"];

            if (string_mode == "Rename") radioButton2.Checked = true;
            if (string_search != null) textBox2.Text = string_search;
            if (string_replace != null) textBox3.Text = string_replace;
            if (string_move_or_copy == "Copy") radioButton4.Checked = true;
            if (string_overwrite == "Yes") checkBox1.Checked = true;
            if (string_replace_ext == "Yes") checkBox2.Checked = true;

            Reset_TB1();
        }

        private void Reset_TB1()
        {
            if (radioButton1.Checked)
            {
                mode = 1;
                groupBox2.Enabled = false;
                textBox1.Text =
                    "Tools for OpendTect and C-View Processing" + Environment.NewLine + "Drag in ..."
                    + Environment.NewLine + Environment.NewLine

                    + "CNV / PC files" + Environment.NewLine
                    + "to convert them to RXY files" + Environment.NewLine + Environment.NewLine

                    + "RXYZ files" + Environment.NewLine
                    + "to linear interpolate the data gap (zero) in Z value" + Environment.NewLine
                    + "and make 'RXYZ Combination Output.txt' with SV 1530 m/s" + Environment.NewLine + Environment.NewLine

                    + "DAT files (OpendTect 2D Horizon)" + Environment.NewLine
                    + "to reformat for SBP Horizon Proc.exe" + Environment.NewLine
                    + "nFix decimal and >9999 SP# exported by OpendTect v6.6" + Environment.NewLine + Environment.NewLine

                    + Environment.NewLine + Environment.NewLine
                    + "v20220812";
            }
            else
            {
                mode = 2;
                textBox1.Text = "Drag in files to rename" + Environment.NewLine + "V20220125" + Environment.NewLine;
                groupBox2.Enabled = true;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            switch (mode)
            {
                case 1:
                    Mode1(files);
                    break;
                case 2:
                    Mode2(files);
                    break;
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        //When Mode Changed reset TB1 (RB checked changed)
        private void ModeChanged(object sender, EventArgs e) { Reset_TB1(); }

        //Mode 1 : Convert files
        private void Mode1(string[] files)
        {
            List<string> filelist = new List<string>();

            //try CNV
            foreach (string file in files)
                if (file.ToUpper().EndsWith(") FILTERED.CNV")) filelist.Add(file);
            if (filelist.Count > 0) { Work_on_CNV_to_RXY(filelist); return; }

            //try PC
            foreach (string file in files)
            {
                string teststr = file.Substring(0, file.Length - 1);
                if (teststr.ToUpper().EndsWith(".PC")) filelist.Add(file);
            }
            if (filelist.Count > 0) { Work_on_PC_to_RXY(filelist); return; }

            //try RXYZ
            foreach (string file in files)
                if (file.ToUpper().EndsWith(".RXYZ")) filelist.Add(file);
            if (filelist.Count > 0) { Work_on_RXYZ(filelist); return; }

            //try DAT
            foreach (string file in files)
                if (file.ToUpper().EndsWith(".DAT")) filelist.Add(file);
            if (filelist.Count > 0) { Work_on_DAT(filelist); return; }

            Reset_TB1();
        }

        //Mode 2 : Rename files
        private void Mode2(string[] files)
        {
            List<string> filelist = files.ToList();
            filelist.Sort();

            textBox1.Text = string.Empty;
            string findwhat = textBox2.Text;
            string replacewith = textBox3.Text;
            string path;
            bool overwrite = checkBox1.Checked;
            bool movefile = radioButton3.Checked;
            bool replace_ext = checkBox2.Checked;
            foreach (string file in filelist)
            {
                path = System.IO.Path.GetDirectoryName(file) + "\\";
                if (System.IO.File.Exists(file)) //check source exists
                {
                    string newfn;

                    if (replace_ext)
                    {
                        newfn = System.IO.Path.GetFileName(file).Replace(findwhat, replacewith);
                    }
                    else
                    {
                        newfn = System.IO.Path.GetFileNameWithoutExtension(file).Replace(findwhat, replacewith) +
                            System.IO.Path.GetExtension(file);
                    }

                    textBox1.AppendText(file + " ===>>> " + newfn);
                    newfn = path + newfn;
                    if (newfn != file)
                        if (movefile)
                        {
                            if (System.IO.File.Exists(newfn))
                            {
                                if (overwrite)
                                {
                                    System.IO.File.Delete(newfn);
                                    System.IO.File.Move(file, newfn);
                                    textBox1.AppendText(" === Moved (Overwritten)" + Environment.NewLine);
                                }
                                else
                                {
                                    textBox1.AppendText(" === Target exists, not moved" + Environment.NewLine);
                                }
                            }
                            else
                            {
                                System.IO.File.Move(file, newfn);
                                textBox1.AppendText(" === Moved" + Environment.NewLine);
                            }
                        }
                        else //copy file
                        {
                            if (System.IO.File.Exists(newfn))
                            {
                                if (overwrite)
                                {
                                    System.IO.File.Delete(newfn);
                                    System.IO.File.Copy(file, newfn);
                                    textBox1.AppendText(" === Copied (Overwritten)" + Environment.NewLine);
                                }
                                else
                                {
                                    textBox1.AppendText(" === Target exists, not copied" + Environment.NewLine);
                                }
                            }
                            else
                            {
                                System.IO.File.Copy(file, newfn);
                                textBox1.AppendText(" === Copied" + Environment.NewLine);
                            }
                        }
                    else
                        textBox1.AppendText(" === File name unchanged" + Environment.NewLine);
                }
                else textBox1.AppendText(file + " === File not exist" + Environment.NewLine);
            }
        }

        private void Work_on_PC_to_RXY(List<string> filelist)
        {
            filelist.Sort();
            textBox1.Text = string.Empty;
            foreach (string file in filelist)
            {
                string output_fn = System.IO.Path.GetFileNameWithoutExtension(file)
                    + " (" + file.Last() + ").rxy";
                textBox1.AppendText("Working on: " +
                    System.IO.Path.GetFileName(file) +
                    " -> " +
                    output_fn);

                List<string> result = Read_PC2RXY(file);

                if (result.Count > 0)
                {
                    output_fn = System.IO.Path.GetDirectoryName(file) + "\\" + output_fn;
                    System.IO.File.WriteAllLines(output_fn, result);
                    textBox1.AppendText(" OK" + Environment.NewLine);
                }
                else
                {
                    textBox1.AppendText(" Error" + Environment.NewLine);
                }
            }
        }

        private void Work_on_CNV_to_RXY(List<string> filelist)
        {
            filelist.Sort();
            textBox1.Text = string.Empty;
            foreach (string file in filelist)
            {
                string output_fn = file.Substring(0, file.Length - 17)
                    + " (" + file.Substring(file.Length - 15, 1) + ").rxy";

                textBox1.AppendText("Working on: " +
                    System.IO.Path.GetFileName(file) +
                    " -> " +
                    System.IO.Path.GetFileName(output_fn));

                List<string> result = Read_CNV2RXY(file);

                if (result.Count > 0)
                {
                    System.IO.File.WriteAllLines(output_fn, result);
                    textBox1.AppendText(" OK" + Environment.NewLine);
                }
                else
                {
                    textBox1.AppendText(" Error" + Environment.NewLine);
                }
            }
        }

        private void Work_on_RXYZ(List<string> filelist)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Water Column", "Sound Velocity", "1530", -1, -1);
            if (!int.TryParse(input, out int sv))
            {
                MessageBox.Show("Cannot convert to number",
                                "Incorrect Input",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
            bool do_extrapolate = false;
            bool do_interpolate = false;

            DialogResult dialogResult = MessageBox.Show(
                "Extrapolate the undefined Z values at the begin and end section?",
                "Extrapolate?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                do_extrapolate = true;

            dialogResult = MessageBox.Show(
                "Interpolate the undefined Z values between valid data points?",
                "Interpolate?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                do_interpolate = true;

            if (filelist.Count == 0) return;
            filelist.Sort();
            textBox1.Text = string.Empty;
            List<RXYZZ> mlRXYZ = new List<RXYZZ>();

            string path = System.IO.Path.GetDirectoryName(filelist[0]) + "\\Processed\\";
            try
            {
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                MessageBox.Show($"The process failed: {e}", "Error");
            }

            foreach (string file in filelist)
            {
                string output_fn = path + System.IO.Path.GetFileName(file);
                textBox1.AppendText("Working on: " + System.IO.Path.GetFileName(file));

                List<RXYZZ> lRXYZ = Read_RXYZ(file);

                if (do_extrapolate)
                    RXYZZ_Extrapolate(ref lRXYZ);

                if (do_interpolate)
                    RXYZZ_Interpolate(ref lRXYZ);

                if (lRXYZ.Count > 0)
                {
                    mlRXYZ.AddRange(lRXYZ);
                    //List<string> result = List_RXYZ_to_String(ref lRXYZ);
                    if (do_interpolate | do_extrapolate)
                        System.IO.File.WriteAllLines(output_fn, List_RXYZ_to_String(ref lRXYZ));
                    textBox1.AppendText(" OK" + Environment.NewLine);
                }
                else
                {
                    textBox1.AppendText(" Error" + Environment.NewLine);
                }
            }

            if (mlRXYZ.Count > 0)
            {
                System.IO.File.WriteAllLines(path + "RXYZ Combination Output.txt", List_RXYZ_to_LXYRT(ref mlRXYZ, sv));
                textBox1.AppendText("RXYZ Combination Output.txt OK" + Environment.NewLine);
            }
        }

        private void Work_on_DAT(List<string> filelist)
        {
            if (filelist.Count == 0) return;
            filelist.Sort();
            textBox1.Text = string.Empty;

            filelist.Sort();
            textBox1.Text = string.Empty;

            string path = System.IO.Path.GetDirectoryName(filelist[0]) + "\\Fixed\\";
            try
            {
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                MessageBox.Show($"The process failed: {e}", "Error");
            }

            foreach (string file in filelist)
            {
                string output_fn = System.IO.Path.GetFileName(file);
                textBox1.AppendText("Working on: " +
                    System.IO.Path.GetFileName(file) +
                    " -> \\Fixed\\" +
                    output_fn);

                List<string> result = Read_DAT(file);

                if (result.Count > 0)
                {
                    output_fn = path + output_fn;
                    //System.IO.File.WriteAllLines(output_fn, result);
                    using (var writer = new System.IO.StreamWriter(output_fn))
                    {
                        writer.NewLine = "\n";
                        foreach (var line in result)
                        {
                            writer.WriteLine(line);
                        }
                    }
                    if (result[result.Count - 1].StartsWith("ERROR"))
                        textBox1.AppendText(" Error" + Environment.NewLine);
                    else
                        textBox1.AppendText(" OK" + Environment.NewLine);
                }
                else
                {
                    textBox1.AppendText(" Error" + Environment.NewLine);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string snr_type = "Move";
            string sw_mode = "Convert";
            string overwrite = "No";
            string replace_ext = "No";
            if (radioButton2.Checked) sw_mode = "Rename";
            if (radioButton4.Checked) snr_type = "Copy";
            if (checkBox1.Checked) overwrite = "Yes";
            if (checkBox2.Checked) replace_ext = "Yes";

            if (ini["Software"]["Mode"] != sw_mode ||
                ini["Search and Replace History"]["Search"] != textBox2.Text ||
                ini["Search and Replace History"]["Replace"] != textBox3.Text ||
                ini["Search and Replace History"]["Move/Copy"] != snr_type ||
                ini["Search and Replace History"]["Overwrite"] != overwrite ||
                ini["Search and Replace History"]["Replace_Ext"] != replace_ext)

            {
                ini["Software"]["Mode"] = sw_mode;
                ini["Search and Replace History"]["Search"] = textBox2.Text;
                ini["Search and Replace History"]["Replace"] = textBox3.Text;
                ini["Search and Replace History"]["Move/Copy"] = snr_type;
                ini["Search and Replace History"]["Overwrite"] = overwrite;
                ini["Search and Replace History"]["Replace_Ext"] = replace_ext;
                Save_ini(ini);
            }
        }
    }
}
