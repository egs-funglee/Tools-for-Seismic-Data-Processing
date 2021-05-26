using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static CNV2RXY.DAT;
using static CNV2RXY.RXY;
using static CNV2RXY.RXYZ;
using static CNV2RXY.REF;

namespace CNV2RXY
{
    public partial class Form1 : Form
    {
        int mode = 0;
        public Form1()
        {
            InitializeComponent();
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

                    + Environment.NewLine
                    + "C-View Bathy v1.9.1 can update H0 in REF directly" + Environment.NewLine
                    + "Functions below were alternative method" + Environment.NewLine + Environment.NewLine

                    + "REF file (1 file)" + Environment.NewLine
                    + "Extract the Trace# Easting and Northing to RXY file for CBG update" + Environment.NewLine + Environment.NewLine

                    + "REF+RXYZ (2 file)" + Environment.NewLine
                    + "Update H0 in REF with Updated RXYZ (RXY originated from REF)" + Environment.NewLine                    

                    + Environment.NewLine + Environment.NewLine + Environment.NewLine
                    + "v20210526";
            }
            else
            {
                mode = 2;
                textBox1.Text = "Drag in files to rename" + Environment.NewLine;
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

            //try REF
            foreach (string file in files)
            {
                if (file.ToUpper().EndsWith(".REF")) filelist.Add(file);
                if (file.ToUpper().EndsWith(".RXYZ")) filelist.Add(file);
            }
            if (filelist.Count > 0) { Work_on_REF(filelist); return; }

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
            string path = System.IO.Path.GetDirectoryName(files[0]) + "\\";
            bool overwrite = checkBox1.Checked;
            bool movefile = radioButton3.Checked;
            foreach (string file in filelist)
            {
                if (System.IO.File.Exists(file)) //check source exists
                {
                    string newfn = System.IO.Path.GetFileName(file).Replace(findwhat, replacewith);
                    textBox1.AppendText(file + " -> " + newfn);
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
                                    textBox1.AppendText(" - Moved (Overwritten)" + Environment.NewLine);
                                }
                                else
                                {
                                    textBox1.AppendText(" - Target exists, not moved" + Environment.NewLine);
                                }
                            }
                            else
                            {
                                System.IO.File.Move(file, newfn);
                                textBox1.AppendText(" - Moved" + Environment.NewLine);
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
                                    textBox1.AppendText(" - Copied (Overwritten)" + Environment.NewLine);
                                }
                                else
                                {
                                    textBox1.AppendText(" - Target exists, not copied" + Environment.NewLine);
                                }
                            }
                            else
                            {
                                System.IO.File.Copy(file, newfn);
                                textBox1.AppendText(" - Copied" + Environment.NewLine);
                            }
                        }
                    else
                        textBox1.AppendText(" - File name unchanged" + Environment.NewLine);
                }
                else textBox1.AppendText(file + " - File not exist" + Environment.NewLine);
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
            bool do_extrapolate = false;

            DialogResult dialogResult = MessageBox.Show(
                "Extrapolate the undefined Z values at the begin and end section?",
                "Extrapolate?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
                do_extrapolate = true;

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

                RXYZZ_Interpolate(ref lRXYZ);

                if (lRXYZ.Count > 0)
                {
                    mlRXYZ.AddRange(lRXYZ);
                    //List<string> result = List_RXYZ_to_String(ref lRXYZ);
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
                System.IO.File.WriteAllLines(path + "RXYZ Combination Output.txt", List_RXYZ_to_LXYRT(ref mlRXYZ, 1530));
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
                    System.IO.File.WriteAllLines(output_fn, result);
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

        private void Work_on_REF(List<string> filelist)
        {
            //filelist.Sort();
            textBox1.Text = string.Empty;
            if (filelist.Count == 1)
            {
                MessageBox.Show("Convert REF to RXY");
                foreach (string file in filelist)
                {
                    string output_fn = System.IO.Path.GetFileNameWithoutExtension(file) + ".rxy";
                    textBox1.AppendText("Working on: " +
                        System.IO.Path.GetFileName(file) +
                        " -> " +
                        output_fn);

                    List<string> result = Read_REF2RXY(file);

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
                return;
            }

            if (filelist.Count == 2) //rxyz + ref
            {
                byte files_are_ready = 0;
                foreach (string file in filelist)
                {
                    if (file.ToUpper().EndsWith(".REF"))
                    {
                        files_are_ready++;
                    }
                    if (file.ToUpper().EndsWith(".RXYZ"))
                    {
                        files_are_ready++;
                    }
                }
                if (files_are_ready != 2) return;

                MessageBox.Show("Replace H0 in REF with RXYZ file");

                List<REF_LINE> REF_result = new List<REF_LINE>();
                List<RXYZZ> RXYZ_result = new List<RXYZZ>();
                string output_fn = "";

                foreach (string file in filelist)
                {
                    if (file.ToUpper().EndsWith(".REF"))
                    {
                        REF_result = Read_REF(file);
                        output_fn = System.IO.Path.GetFileNameWithoutExtension(file) + "_Updated.ref";
                        textBox1.AppendText("Working on REF: " +
                            System.IO.Path.GetFileName(file) + Environment.NewLine);
                    }
                    if (file.ToUpper().EndsWith(".RXYZ"))
                    {
                        RXYZ_result = Read_RXYZ(file);
                        textBox1.AppendText("Reading H0 from: " + System.IO.Path.GetFileName(file) + Environment.NewLine);
                    }
                }

                if (REF_result.Count > 0 && RXYZ_result.Count > 0)
                {
                    if (REF_result.Count == RXYZ_result.Count) //just check if they have same total number of items
                    {
                        //match and update here
                        List<string> result = new List<string>();
                        for (int i = 0; i < REF_result.Count; i++)
                        {

                            if (REF_result[i].x == RXYZ_result[i].x && REF_result[i].y == RXYZ_result[i].y)
                            { REF_result[i].h0 = RXYZ_result[i].Z1(); }
                            else
                            { REF_result[i].h0 = "-999.0"; }
                            result.Add(REF_result[i].ToStr());
                        }

                        textBox1.AppendText("--> " + output_fn + " OK" + Environment.NewLine);
                        output_fn = System.IO.Path.GetDirectoryName(filelist[0]) + "\\" + output_fn;
                        System.IO.File.WriteAllLines(output_fn, result);

                    }
                    else
                    {
                        textBox1.AppendText(" Error" + Environment.NewLine);
                    }
                }
                else
                {
                    textBox1.AppendText(" Error" + Environment.NewLine);
                }
                return;
            }

            if (filelist.Count > 2)
            {
                MessageBox.Show("Select REF (1 file) or REF+RXYZ (2 files)");
            }
        }
    }
}
