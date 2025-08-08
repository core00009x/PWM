using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PWM
{
    public partial class PWM : Form
    {
        public List<String> pathOfRecentFiles = new List<String>();

        private bool isHaveRecentFiles = false;
        private PrintDocument _printDocument;
        private int _startChar;
        private RichTextBox _richTextBox;

        public PWM()
        {
            InitializeComponent();

            pathOfRecentFiles.Clear();

            _printDocument = new PrintDocument();
            _printDocument.BeginPrint += BeginPrint;
            _printDocument.PrintPage += PrintPage;
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
                    DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (dr == DialogResult.OK)
                    {
                        DestroyHandle();
                    }
                }
                catch (Exception ex)
                {
                    DialogResult dr = MessageBox.Show(ex.StackTrace, "An Erryor as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                {
                    DestroyHandle();
                }
            }
            catch (Exception ex)
            {
                DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public void Print(RichTextBox rb)
        {
            _richTextBox = rb;
            PrintDialog printDialog = new PrintDialog
            {
                Document = _printDocument
            };

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                _printDocument.Print();
            }
        }

        private void BeginPrint(object sender, PrintEventArgs e)
        {
            _startChar = 0;
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            _startChar = PrintRichText(_richTextBox, _startChar, _richTextBox.TextLength, e);
            e.HasMorePages = _startChar < _richTextBox.TextLength;
        }

        private int PrintRichText(RichTextBox rtb, int charFrom, int charTo, PrintPageEventArgs e)
        {
            // Create a RECT structure for the printable area
            RECT rectToPrint;
            rectToPrint.Top = HundredthInchToTwips(e.MarginBounds.Top);
            rectToPrint.Bottom = HundredthInchToTwips(e.MarginBounds.Bottom);
            rectToPrint.Left = HundredthInchToTwips(e.MarginBounds.Left);
            rectToPrint.Right = HundredthInchToTwips(e.MarginBounds.Right);

            // Create a RECT structure for the page
            RECT rectPage;
            rectPage.Top = HundredthInchToTwips(e.PageBounds.Top);
            rectPage.Bottom = HundredthInchToTwips(e.PageBounds.Bottom);
            rectPage.Left = HundredthInchToTwips(e.PageBounds.Left);
            rectPage.Right = HundredthInchToTwips(e.PageBounds.Right);

            IntPtr hdc = e.Graphics.GetHdc();

            FORMATRANGE fmtRange;
            fmtRange.chrg.cpMin = charFrom;
            fmtRange.chrg.cpMax = charTo;
            fmtRange.hdc = hdc;
            fmtRange.hdcTarget = hdc;
            fmtRange.rc = rectToPrint;
            fmtRange.rcPage = rectPage;

            IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
            Marshal.StructureToPtr(fmtRange, lParam, false);

            IntPtr wParam = new IntPtr(1); // Render

            int printedChars = SendMessage(rtb.Handle, EM_FORMATRANGE, wParam, lParam);

            Marshal.FreeCoTaskMem(lParam);
            e.Graphics.ReleaseHdc(hdc);

            return printedChars;
        }

        private int HundredthInchToTwips(int n)
        {
            return (int)(n * 14.4);
        }

        // Win32 API declarations
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FORMATRANGE
        {
            public IntPtr hdc;
            public IntPtr hdcTarget;
            public RECT rc;
            public RECT rcPage;
            public CHARRANGE chrg;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int WM_USER = 0x0400;
        private const int EM_FORMATRANGE = WM_USER + 57;

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                Print(b.GetRTB());
            }
        }

        public void ShowPageSetup()
        {
            PageSetupDialog setupDialog = new PageSetupDialog
            {
                Document = _printDocument
            };
            setupDialog.ShowDialog();
        }

        public void ShowPrintPreview()
        {
            PrintPreviewDialog previewDialog = new PrintPreviewDialog
            {
                Document = _printDocument,
                Width = 800,
                Height = 600
            };
            previewDialog.ShowDialog();
        }

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                _richTextBox = b.GetRTB();
                ShowPageSetup();
            }
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                _richTextBox = b.GetRTB();
                ShowPrintPreview();
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                if (b.GetRTB().CanUndo == true)
                {
                    b.GetRTB().Undo();
                }
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                if (b.GetRTB().CanRedo == true)
                {
                    b.GetRTB().Redo();
                }
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.GetRTB().Cut();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.GetRTB().Copy();
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.GetRTB().Paste();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.GetRTB().SelectedText = "";
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.GetRTB().SelectAll();
            }
        }

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                DuplicateCurrentLine(b.GetRTB());
            }
        }

        public void DuplicateCurrentLine(RichTextBox richTextBox)
        {
            int lineIndex = richTextBox.GetLineFromCharIndex(richTextBox.SelectionStart);
            int lineStart = richTextBox.GetFirstCharIndexFromLine(lineIndex);
            int lineEnd = (lineIndex < richTextBox.Lines.Length - 1)
                ? richTextBox.GetFirstCharIndexFromLine(lineIndex + 1)
                : richTextBox.TextLength;
            string lineText = richTextBox.Text.Substring(lineStart, lineEnd - lineStart);
            richTextBox.SelectionStart = lineEnd;
            richTextBox.SelectionLength = 0;
            richTextBox.SelectedText = lineText;
        }

        private void plainTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.PlainText);
            }
        }

        private void richTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.RichText);
            }
        }

        private void unicodeTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.UnicodeText);
            }
        }

        private void htmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.Html);
            }
        }

        private void markdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.Markdown);
            }
        }

        private void codeBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.CodeBlock);
            }
        }

        private void noImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.NoImages);
            }
        }

        private void quoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.Quote);
            }
        }

        private void timestampedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.Timestamped);
            }
        }

        private void commentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.Comment);
            }
        }

        private void titlecaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.TitleCase);
            }
        }

        private void lowercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.Lowercase);
            }
        }

        private void uppercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.Uppercase);
            }
        }

        private void reversedTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.ReversedText);
            }
        }

        private void bulletListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.BulletList);
            }
        }

        private void numberedListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.NumberedList);
            }
        }

        private void jsonBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.JsonBlock);
            }
        }

        private void xmlBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.XmlBlock);
            }
        }

        private void tableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.Table);
            }
        }

        private void hashtagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.Hashtags);
            }
        }

        private void emojiTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.EmojiText);
            }
        }

        private void morseCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.MorseCode);
            }
        }

        private void pigLatinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.PigLatin);
            }
        }

        private void rot13ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                var b = this.ActiveMdiChild as Editor;
                b.PasteSpecial(b.GetRTB(), Editor.PasteSpecialFormat.ROT13);
            }
        }

        private void undoLastGlobalActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length > 0)
            {
                var b = this.MdiChildren;
                foreach (var form in b)
                {
                    var x = form as Editor;
                    if (x.GetRTB().CanUndo == true)
                    {
                        x.GetRTB().Undo();
                    }
                }
            }
        }

        private void redoLastGlobalActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length > 0)
            {
                var b = this.MdiChildren;
                foreach (var form in b)
                {
                    var x = form as Editor;
                    if (x.GetRTB().CanRedo == true)
                    {
                        x.GetRTB().Redo();
                    }
                }
            }
        }
    }
}
