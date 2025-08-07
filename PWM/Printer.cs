using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PWM
{
    public static class Printer
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int WM_USER = 0x0400;
        private const int EM_FORMATRANGE = WM_USER + 57;
        private const int EM_FORMATRANGE_RELEASE = WM_USER + 58;

        public static int Print(this RichTextBox rtb, int charFrom, int charTo, PrintPageEventArgs e)
        {
            // Convert .NET units (1/100 inch) to twips (1/1440 inch)
            int ToTwips(int val) => (int)(val * 14.4);

            // Set up the printing area
            RECT rc = new RECT
            {
                Top = ToTwips(e.MarginBounds.Top),
                Bottom = ToTwips(e.MarginBounds.Bottom),
                Left = ToTwips(e.MarginBounds.Left),
                Right = ToTwips(e.MarginBounds.Right)
            };

            RECT rcPage = new RECT
            {
                Top = ToTwips(e.PageBounds.Top),
                Bottom = ToTwips(e.PageBounds.Bottom),
                Left = ToTwips(e.PageBounds.Left),
                Right = ToTwips(e.PageBounds.Right)
            };

            CHARRANGE chrg = new CHARRANGE
            {
                cpMin = charFrom,
                cpMax = charTo
            };

            FORMATRANGE fmtRange = new FORMATRANGE
            {
                hdc = e.Graphics.GetHdc(),
                hdcTarget = e.Graphics.GetHdc(),
                rc = rc,
                rcPage = rcPage,
                chrg = chrg
            };

            IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
            Marshal.StructureToPtr(fmtRange, lParam, false);

            IntPtr result = SendMessage(rtb.Handle, EM_FORMATRANGE, (IntPtr)1, lParam);

            Marshal.FreeCoTaskMem(lParam);
            e.Graphics.ReleaseHdc(fmtRange.hdc);

            return result.ToInt32();
        }

        public static void ReleaseFormatRange(this RichTextBox rtb)
        {
            SendMessage(rtb.Handle, EM_FORMATRANGE_RELEASE, IntPtr.Zero, IntPtr.Zero);
        }

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
    }
}
