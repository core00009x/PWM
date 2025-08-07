using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PWM
{
    public partial class Editor : Form
    {
        private enum BufferSizes
        {
            TooSmall = 512,
            Balanced = 1024,
            Recommended = 4096,
            High = 8192,
            Large = 16384,
            UltraLarge = 32768,
            SuperLarge = 65536
        }

        private RichTextBox rtb;
        public int defaultBufferSize = 16384;

        public string filePath = null;
        public string fileName = null;
        public string fileParent = null;

        public bool isSaved = false;
        public bool autoSave = false;

        public Editor()
        {
            InitializeComponent();

            Layout();
        }

        private void Layout() 
        {
            this.Text = "new";

            rtb = new RichTextBox();
            rtb.Dock = DockStyle.Fill;
            rtb.Text = "";

            rtb.TextChanged += Rtb_TextChanged;
            rtb.GotFocus += Rtb_GotFocus;

            this.FormClosing += Editor_FormClosing;

            this.Controls.Add(rtb);
        }

        private void Rtb_GotFocus(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isSaved == false)
            {
                DialogResult dr = MessageBox.Show("Do you want to save changes to " + this.Text + "?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    SaveFile();
                }
                else if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void Rtb_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.Text.EndsWith("*") == false)
                {
                    this.Text = this.Text + "*";
                    isSaved = false;
                }
                if (autoSave == true)
                {
                    SaveFile();
                    if (this.Text.EndsWith("*"))
                    {
                        this.Text = this.Text.Substring(0, this.Text.Length - 1);
                        isSaved = true;
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                {
                    DestroyHandle();
                }
            }
            catch (Exception ex)
            {
                DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                {
                    DestroyHandle();
                }
            }
        }

        public RichTextBox GetRTB() 
        { 
            return this.rtb; 
        }

        public string GetRTBText() 
        { 
            return rtb.Text;
        }

        public void SetRTBText(String text) 
        {
            rtb.Text = text;
        }

        public void SaveFileAs()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Save File As...";
                saveFileDialog.InitialDirectory = "C:";
                saveFileDialog.Filter = "";
                try
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (filePath == null)
                        {
                            filePath = saveFileDialog.FileName;
                            string[] strings = filePath.Split(Convert.ToChar("\\"));
                            fileName = strings[strings.Length - 1];
                            strings[strings.Length - 1] = "";
                            fileParent = String.Join("\\", strings);
                            this.Text = fileName;
                        }
                        try
                        {
                            StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, false, DetectFileEncoding(saveFileDialog.FileName), defaultBufferSize);
                            streamWriter.Write(rtb.Text);
                            streamWriter.Close();
                        }
                        catch (InvalidCastException ex)
                        {
                            DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            if (dr == DialogResult.OK)
                            {
                                DestroyHandle();
                            }
                        }
                        catch (Exception ex)
                        {
                            DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            if (dr == DialogResult.OK)
                            {
                                DestroyHandle();
                            }
                        }
                    }
                }
                catch (FileNotFoundException ex)
                {
                    DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (dr == DialogResult.OK)
                    {
                        DestroyHandle();
                    }
                }
                catch (Exception ex)
                {
                    DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (dr == DialogResult.OK)
                    {
                        DestroyHandle();
                    }
                }
            }
            sw.Stop();
        }

        public void SaveFile()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (filePath != null)
            {
                try
                {
                    StreamWriter streamWriter = new StreamWriter(filePath, false, DetectFileEncoding(filePath), defaultBufferSize);
                    streamWriter.Write(rtb.Text);
                    streamWriter.Close();
                }
                catch (InvalidCastException ex)
                {
                    DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (dr == DialogResult.OK)
                    {
                        DestroyHandle();
                    }
                }
                catch (Exception ex)
                {
                    DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (dr == DialogResult.OK)
                    {
                        DestroyHandle();
                    }
                }
                finally
                {
                    this.Text = fileName;
                    isSaved = true;
                }
            }
            else
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Title = "Save File As...";
                    saveFileDialog.InitialDirectory = "C:";
                    saveFileDialog.Filter = "";
                    try
                    {
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            filePath = saveFileDialog.FileName;
                            string[] strings = filePath.Split(Convert.ToChar("\\"));
                            fileName = strings[strings.Length - 1];
                            strings[strings.Length - 1] = "";
                            fileParent = String.Join("\\", strings);
                            this.Text = fileName;
                            try
                            {
                                StreamWriter streamWriter = new StreamWriter(filePath, false, DetectFileEncoding(filePath), defaultBufferSize);
                                streamWriter.Write(rtb.Text);
                                streamWriter.Close();
                            }
                            catch (InvalidCastException ex)
                            {
                                DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                if (dr == DialogResult.OK)
                                {
                                    DestroyHandle();
                                }
                            }
                            catch (Exception ex)
                            {
                                DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                if (dr == DialogResult.OK)
                                {
                                    DestroyHandle();
                                }
                            }
                            finally
                            {
                                this.Text = fileName;
                                isSaved = true;
                            }
                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (dr == DialogResult.OK)
                        {
                            DestroyHandle();
                        }
                    }
                    catch (Exception ex)
                    {
                        DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (dr == DialogResult.OK)
                        {
                            DestroyHandle();
                        }
                    }
                }
            }
            sw.Stop();
        }

        public void OpenFile(String path, Encoding encoding)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                StreamReader streamReader = new StreamReader(path, DetectFileEncoding(path), false, defaultBufferSize);
                var content = streamReader.ReadToEnd();
                SetRTBText(content);
                streamReader.Close();
            }
            catch (FileNotFoundException ex)
            {
                DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                {
                    DestroyHandle();
                }
            }
            catch (Exception ex) 
            {
                DialogResult dr = MessageBox.Show(ex.Message,"An Error as Occurred!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                {
                    DestroyHandle();
                }
            }
            finally
            {
                this.Text = fileName;
                isSaved = true;
            }
            sw.Stop();
        }

        private Encoding DetectFileEncoding(string filePath)
        {
            byte[] bom = new byte[4];
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file.Read(bom, 0, bom.Length);
            }

            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode;
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode;
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0x00 && bom[3] == 0x00) return Encoding.UTF32;

            return Encoding.UTF8;
        }
    }
}
