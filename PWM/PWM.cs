using System;
using System.CodeDom;
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
        public List<String> pathOfRecentFiles = new List<String>();

        private bool isHaveRecentFiles = false;

        public PWM()
        {
            InitializeComponent();

            pathOfRecentFiles.Clear();
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
                            editor.OpenFile(file, Encoding.UTF8);

                            editor.filePath = file;

                            string x = file;
                            var strings = x.Split(Convert.ToChar("\\"));
                            editor.fileName = strings[strings.Length - 1];
                            strings[strings.Length - 1] = "";
                            var res = String.Join("\\", strings);
                            editor.fileParent = res;

                            editor.Text = editor.fileName;
                            editor.Visible = true;
                            editor.MdiParent = this;
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
            editor.Visible = true;
            editor.MdiParent = this;
        }

        private void saveCurrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                try
                {
                    this.ActiveMdiChild?.GetType().GetMethod("SaveFile")?.Invoke(this.ActiveMdiChild, null);
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

        private void saveCurrentAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                try
                {
                    this.ActiveMdiChild?.GetType().GetMethod("SaveFileAs")?.Invoke(this.ActiveMdiChild, null);
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

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length > 0)
            {
                try
                {
                    foreach (Form form in this.MdiChildren)
                    {
                        form?.GetType().GetMethod("SaveFile")?.Invoke(form, null);
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

        private void autoSaveToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (autoSaveToolStripMenuItem.Checked)
            {
                if (this.ActiveMdiChild != null)
                {
                    try
                    {
                        var x = this.ActiveMdiChild as Editor;
                        x.autoSave = true;
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
            else
            {
                if (this.ActiveMdiChild != null)
                {
                    try
                    {
                        var x = this.ActiveMdiChild as Editor;
                        x.autoSave = false;
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
        }

        private void PWM_MdiChildActivate(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length > 1)
            {
                try
                {
                    var x = this.ActiveMdiChild as Editor;
                    if (x.filePath != null)
                    {
                        autoSaveToolStripMenuItem.Enabled = true;
                        if (x.autoSave == true)
                        {
                            autoSaveToolStripMenuItem.Checked = true;
                        }
                        else
                        {
                            autoSaveToolStripMenuItem.Checked = false;
                        }
                        if (pathOfRecentFiles.Contains(x.filePath) == false && pathOfRecentFiles.Count <= 10)
                        {
                            try
                            {
                                pathOfRecentFiles.Add(x.filePath);
                                var z = new ToolStripMenuItem();
                                z.AccessibleDescription = x.filePath;
                                z.ToolTipText = x.filePath;
                                var y = x.filePath.Split('\\');
                                z.AccessibleName = y[y.Length - 1];
                                z.Text = y[y.Length - 1];
                                y[y.Length - 1] = "";
                                z.AccessibleDefaultActionDescription = String.Join("\\", y);

                                z.Click += X_Click;

                                recentFilesToolStripMenuItem.DropDownItems.Add(z);
                            }
                            catch (Exception ex)
                            {
                                DialogResult dr = MessageBox.Show(ex.Message, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                if (dr == DialogResult.OK)
                                {
                                    DestroyHandle();
                                }
                            }
                            if (isHaveRecentFiles == false)
                            {
                                var u = new ToolStripMenuItem();
                                u.Text = "Remove All()";

                                u.Click += Z_Click;

                                recentFilesToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
                                recentFilesToolStripMenuItem.DropDownItems.Add(u);
                                isHaveRecentFiles = true;
                            }
                        }
                    }
                    else
                    {
                        autoSaveToolStripMenuItem.Enabled = false;
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

        private void closeCurrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                this.ActiveMdiChild.Dispose();
                this.ActiveMdiChild.Close();
            }
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length > 0)
            {
                try
                {
                    foreach (Form form in this.MdiChildren)
                    {
                        form.Dispose();
                        form.Close();
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeAllToolStripMenuItem.PerformClick();
            this.Dispose();
            this.Close();
        }

        private void PWM_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                StreamWriter sw = new StreamWriter("config\\recent.cfg", false, Encoding.UTF8, 4096);
                foreach (string recentFilePath in pathOfRecentFiles)
                {
                    sw.WriteLine(recentFilePath);
                }
                sw.Close();
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

        private void PWM_Load(object sender, EventArgs e)
        {
            try
            {
                StreamReader sr = new StreamReader("config\\recent.cfg", Encoding.UTF8, true, 4096);
                var content = sr.ReadToEnd();
                var contentLines = content.Split('\n');
                if (contentLines.Length > 0)
                {
                    foreach (var line in contentLines)
                    {
                        if (File.Exists(line) == true)
                        {
                            var x = new ToolStripMenuItem();
                            x.AccessibleDescription = line;
                            x.ToolTipText = line;
                            var y = line.Split('\\');
                            x.AccessibleName = y[y.Length - 1];
                            x.Text = y[y.Length - 1];
                            y[y.Length - 1] = "";
                            x.AccessibleDefaultActionDescription = String.Join("\\", y);

                            x.Click += X_Click;

                            recentFilesToolStripMenuItem.DropDownItems.Add(x);
                        }
                    }
                    if (recentFilesToolStripMenuItem.DropDownItems.Count > 0)
                    {
                        var z = new ToolStripMenuItem();
                        z.Text = "Remove All()";

                        z.Click += Z_Click;

                        recentFilesToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
                        recentFilesToolStripMenuItem.DropDownItems.Add(z);
                        isHaveRecentFiles = true;
                    }
                }
                sr.Close();
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

        private void X_Click(object sender, EventArgs e)
        {
            try
            {
                var x = sender as ToolStripMenuItem;
                if (x != null)
                {
                    var editor = new Editor();
                    editor.OpenFile(x.AccessibleDescription, Encoding.UTF8);
                    editor.Visible = true;
                    editor.MdiParent = this;
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

        private void Z_Click(object sender, EventArgs e)
        {
            try
            {
                StreamWriter sw = new StreamWriter("config\\recent.cfg", false, Encoding.UTF8, 4096);
                sw.Write(" ");
                sw.Close();
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
                recentFilesToolStripMenuItem.DropDownItems.Clear();
                pathOfRecentFiles.Clear();
                isHaveRecentFiles = false;
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
    }
}
