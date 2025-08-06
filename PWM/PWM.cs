using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PWM
{
    public partial class PWM : Form
    {
        public PWM()
        {
            InitializeComponent();
        }

        private void openSimpleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Open File...";
                openFileDialog.Multiselect = true;
                openFileDialog.InitialDirectory = "C:";
                openFileDialog.Filter = "";
                try
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        foreach (string file in openFileDialog.FileNames)
                        {
                            var editor = new Editor();
                            editor.OpenFile(file, null);

                            editor.filePath = file;

                            string x = file;
                            var strings = x.Split(Convert.ToChar("\\"));
                            editor.fileName = strings[strings.Length - 1];
                            strings[strings.Length - 1] = "";
                            var res = String.Join("\\", strings);
                            editor.fileParent = res;

                            editor.Text = editor.fileName;
                            tabControl1.Controls.Add(editor);
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

        private void createSimpleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var editor = new Editor();
            editor.Text = "new file";
            tabControl1.Controls.Add(editor);
        }
    }
}
