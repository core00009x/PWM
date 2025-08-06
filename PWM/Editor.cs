using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PWM
{
    public partial class Editor : TabPage
    {
        private RichTextBox rtb;
        private const int defaultBufferSize = 65536;

        public string filePath = null;
        public string fileName = null;
        public string fileParent = null;

        public bool isSaved = false;

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

            this.Controls.Add(rtb);
        }

        private void Rtb_TextChanged(object sender, EventArgs e)
        {
            if (this.Text.EndsWith("*") == false)
            {
                this.Text = this.Text + "*";
            }
            else
            {
                return;
            }
        }

        public string GetRTBText() 
        { 
            return rtb.Text;
        }

        public void SetRTBText(String text) 
        {
            rtb.Text = text;
        }

        public void OpenFile(String path, Encoding encoding)
        {
            try
            {
                if (encoding == null)
                {
                    StreamReader streamReader = new StreamReader(path, DetectFileEncoding(path), false, defaultBufferSize);
                    var content = streamReader.ReadToEnd();
                    SetRTBText(content);
                    streamReader.Close();
                }
                else
                {
                    StreamReader streamReader = new StreamReader(path, encoding, false, defaultBufferSize);
                    var content = streamReader.ReadToEnd();
                    SetRTBText(content);
                    streamReader.Close();
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
                DialogResult dr = MessageBox.Show(ex.Message,"An Error as Occurred!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                {
                    DestroyHandle();
                }
            }
            finally
            {
                this.Text = fileName;
            }
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
