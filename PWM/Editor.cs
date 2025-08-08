using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PWM
{
    public partial class Editor : Form
    {
        public enum PasteSpecialFormat
        {
            PlainText,
            RichText,
            UnicodeText,
            Html,
            Markdown,
            CodeBlock,
            NoImages,
            Quote,
            Timestamped,
            Comment,
            TitleCase,
            Lowercase,
            Uppercase,
            ReversedText,
            BulletList,
            NumberedList,
            JsonBlock,
            XmlBlock,
            Table,
            Hashtags,
            EmojiText,
            MorseCode,
            PigLatin,
            ROT13,
        }

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
                finally
                {
                    this.Text = fileName;
                    isSaved = true;
                    fileSystemWatcher1.Path = filePath;
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
                            finally
                            {
                                this.Text = fileName;
                                isSaved = true;
                                fileSystemWatcher1.Path = filePath;
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
                        DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                DialogResult dr = MessageBox.Show(ex.StackTrace, "An Error as Occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                {
                    DestroyHandle();
                }
            }
            catch (Exception ex) 
            {
                DialogResult dr = MessageBox.Show(ex.StackTrace,"An Error as Occurred!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                {
                    DestroyHandle();
                }
            }
            finally
            {
                this.Text = fileName;
                isSaved = true;
                fileSystemWatcher1.Path = filePath;
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

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                OpenFile(e.FullPath, DetectFileEncoding(e.FullPath));
                SaveFile();
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

        private void fileSystemWatcher1_Deleted(object sender, FileSystemEventArgs e)
        {
            if (this.Text.EndsWith("*") == false)
            {
                this.Text = this.Text + "*";
                isSaved = false;
            }
            DialogResult dr =  MessageBox.Show($"The file: {e.Name} has been deleted!You want to save again?", "The File Has Been Deleted!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                SaveFile();
            }
            else
            {
                return;
            }
        }

        private void fileSystemWatcher1_Renamed(object sender, RenamedEventArgs e)
        {
            filePath = e.FullPath;
            fileName = e.Name;
            this.Text = fileName;
        }

        public void PasteSpecial(RichTextBox richTextBox, PasteSpecialFormat format)
        {
            IDataObject clipboardData = Clipboard.GetDataObject();
            if (clipboardData == null) return;

            string input = Clipboard.GetText();

            switch (format)
            {
                case PasteSpecialFormat.PlainText:
                    richTextBox.SelectedText = input;
                    break;

                case PasteSpecialFormat.RichText:
                    if (clipboardData.GetDataPresent(DataFormats.Rtf))
                        richTextBox.SelectedRtf = Clipboard.GetText(TextDataFormat.Rtf);
                    else
                        richTextBox.SelectedText = input;
                    break;

                case PasteSpecialFormat.UnicodeText:
                    richTextBox.SelectedText = Clipboard.GetText(TextDataFormat.UnicodeText);
                    break;

                case PasteSpecialFormat.Html:
                    richTextBox.SelectedText = Clipboard.GetText(TextDataFormat.Html);
                    break;

                case PasteSpecialFormat.Markdown:
                    richTextBox.SelectedText = ConvertToMarkdown(input);
                    break;

                case PasteSpecialFormat.CodeBlock:
                    richTextBox.SelectionFont = new Font("Consolas", 10);
                    richTextBox.SelectedText = input;
                    break;

                case PasteSpecialFormat.NoImages:
                    string rtf = Clipboard.GetText(TextDataFormat.Rtf);
                    richTextBox.SelectedRtf = StripImagesFromRtf(rtf);
                    break;

                case PasteSpecialFormat.Quote:
                    richTextBox.SelectedText = "> " + input.Replace("\n", "\n> ");
                    break;

                case PasteSpecialFormat.Timestamped:
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    richTextBox.SelectedText = $"[{timestamp}]\n{input}";
                    break;

                case PasteSpecialFormat.Comment:
                    richTextBox.SelectedText = $"/* {input} */";
                    break;

                case PasteSpecialFormat.TitleCase:
                    richTextBox.SelectedText = ToTitleCase(input);
                    break;

                case PasteSpecialFormat.Lowercase:
                    richTextBox.SelectedText = input.ToLower();
                    break;

                case PasteSpecialFormat.Uppercase:
                    richTextBox.SelectedText = input.ToUpper();
                    break;

                case PasteSpecialFormat.ReversedText:
                    richTextBox.SelectedText = new string(input.Reverse().ToArray());
                    break;

                case PasteSpecialFormat.BulletList:
                    richTextBox.SelectedText = ToBulletList(input);
                    break;

                case PasteSpecialFormat.NumberedList:
                    richTextBox.SelectedText = ToNumberedList(input);
                    break;

                case PasteSpecialFormat.JsonBlock:
                    richTextBox.SelectedText = $"{{ \"content\": \"{EscapeJson(input)}\" }}";
                    break;

                case PasteSpecialFormat.XmlBlock:
                    richTextBox.SelectedText = $"<content>{System.Security.SecurityElement.Escape(input)}</content>";
                    break;

                case PasteSpecialFormat.Table:
                    richTextBox.SelectedText = ConvertToTable(input);
                    break;

                case PasteSpecialFormat.Hashtags:
                    richTextBox.SelectedText = ConvertToHashtags(input);
                    break;

                case PasteSpecialFormat.EmojiText:
                    richTextBox.SelectedText = ConvertToEmojiText(input);
                    break;

                case PasteSpecialFormat.MorseCode:
                    richTextBox.SelectedText = ConvertToMorseCode(input);
                    break;

                case PasteSpecialFormat.PigLatin:
                    richTextBox.SelectedText = ConvertToPigLatin(input);
                    break;

                case PasteSpecialFormat.ROT13:
                    richTextBox.SelectedText = ApplyROT13(input);
                    break;
            }
        }

        private string ToTitleCase(string input) =>
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());

        private string ToBulletList(string input) =>
            string.Join("\n", input.Split('\n').Select(line => "• " + line));

        private string ToNumberedList(string input) =>
            string.Join("\n", input.Split('\n').Select((line, i) => $"{i + 1}. {line}"));

        private string EscapeJson(string input) =>
            input.Replace("\\", "\\\\").Replace("\"", "\\\"");

        private string ConvertToMarkdown(string input) =>
            input.Replace("<b>", "**").Replace("</b>", "**").Replace("<i>", "*").Replace("</i>", "*");

        private string StripImagesFromRtf(string rtf) =>
            Regex.Replace(rtf, @"\\pict[\s\S]+?}", "");

        private string ConvertToTable(string input)
        {
            var rows = input.Split('\n');
            var table = new StringBuilder();
            foreach (var row in rows)
            {
                var cells = row.Split('\t');
                table.AppendLine(string.Join(" | ", cells));
            }
            return table.ToString();
        }

        private string ConvertToHashtags(string input)
        {
            var words = Regex.Matches(input, @"\b\w+\b")
                             .Cast<Match>()
                             .Select(m => "#" + m.Value);
            return string.Join(" ", words);
        }

        private string ConvertToEmojiText(string input)
        {
            return input.Replace("happy", "😊")
                        .Replace("sad", "😢")
                        .Replace("love", "❤️")
                        .Replace("cool", "😎");
        }

        private string ConvertToMorseCode(string input)
        {
            var morseMap = new Dictionary<char, string>
            {
                ['a'] = ".-",
                ['b'] = "-...",
                ['c'] = "-.-.",
                ['d'] = "-..",
                ['e'] = ".",
                ['f'] = "..-.",
                ['g'] = "--.",
                ['h'] = "....",
                ['i'] = "..",
                ['j'] = ".---",
                ['k'] = "-.-",
                ['l'] = ".-..",
                ['m'] = "--",
                ['n'] = "-.",
                ['o'] = "---",
                ['p'] = ".--.",
                ['q'] = "--.-",
                ['r'] = ".-.",
                ['s'] = "...",
                ['t'] = "-",
                ['u'] = "..-",
                ['v'] = "...-",
                ['w'] = ".--",
                ['x'] = "-..-",
                ['y'] = "-.--",
                ['z'] = "--..",
                [' '] = "/"
            };

            return string.Join(" ", input.ToLower().Where(c => morseMap.ContainsKey(c)).Select(c => morseMap[c]));
        }

        private string ConvertToPigLatin(string input)
        {
            return string.Join(" ", input.Split(' ').Select(word =>
            {
                if (string.IsNullOrWhiteSpace(word)) return word;
                string first = word.Substring(0, 1);
                string rest = word.Substring(1);
                return rest + first + "ay";
            }));
        }

        private string ApplyROT13(string input)
        {
            return new string(input.Select(c =>
            {
                if (!char.IsLetter(c)) return c;
                char offset = char.IsUpper(c) ? 'A' : 'a';
                return (char)(((c - offset + 13) % 26) + offset);
            }).ToArray());
        }
    }
}
