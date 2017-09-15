using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sapphire
{
    class Voiceroid
    {
        public const int BM_CLICK = 0x00F5;
        public const int EM_REPLACESEL = 0x00C2;
        public const int EM_SETSEL = 0x00B1;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public static int GWL_STYLE = -16;
        public static int GW_CHILD = 0x0005;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, string lParam);
        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpszClass, string lpdzWindow);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, int wParam);
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClasName, int nMaxCount);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private VoiceroidWindow Yukari;

        public Voiceroid()
        {
            var mainWindowHandle = IntPtr.Zero;
            bool flag = true;
            while (true)
            {
                try
                {
                    mainWindowHandle = Process.GetProcessesByName("VOICEROID")[0].MainWindowHandle;
                    break;
                }
                catch
                {
                    if (flag)
                    {
                        System.Diagnostics.Process.Start((System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\AHS\\VOICEROID+\\YukariEx\\VOICEROID.exe"));
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                    System.Threading.Thread.Sleep(5000);
                }
            }
            Yukari = InitWindow(mainWindowHandle);
        }

        public void SaveText(string text)
        {
            if (System.IO.File.Exists("T:\\Sapphire\\tmp.wav"))
            {
                System.IO.File.Delete("T:\\Sapphire\\tmp.wav");
                System.IO.File.Delete("T:\\Sapphire\\tmp.txt");
                System.Threading.Thread.Sleep(50);
            }
            SendMessage(Yukari.Content, text);
            PostMessage(Yukari.AudioSaveButton, BM_CLICK, 0, 0);
            System.Threading.Thread.Sleep(500);
            var savemain = FindWindow("#32770", "音声ファイルの保存");
            var file = savemain;
            System.Threading.Thread.Sleep(50);
            for (int i = 0; i< 5; i++)
                file = GetWindow(file, GW_CHILD);
            var save = FindWindowEx(savemain,IntPtr.Zero,"BUTTON","保存(&S)");
            SendMessage(file, "tmp");
            SendMessage(save, BM_CLICK, 0, 0);
            System.Threading.Thread.Sleep(500);
        }

        private VoiceroidWindow InitWindow(IntPtr mainWindow)
        {
            var content = FindTargetObject(GetWindow(mainWindow), 0, "RichEdit");
            var audio = FindTargetObject(GetWindow(mainWindow), 2, "BUTTON");
            return new VoiceroidWindow(content,audio);
        }

        private static void SendMessage(IntPtr hWnd, string str)
        {
            SendMessage(hWnd, EM_SETSEL, 0, -1);
            SendMessage(hWnd, EM_REPLACESEL, 1, str);
        }

        private static IntPtr FindTargetObject(Window top, int cnt, string objname)
        {
            var all = GetAllChildWindows(top, new List<Window>());
            return all.Where(x => x.ClassName.IndexOf(objname) != -1).Skip(cnt).First().hWnd;
        }

        private static List<Window> GetAllChildWindows(Window parent, List<Window> dest)
        {
            dest.Add(parent);
            ENumChildWindows(parent.hWnd).ToList().ForEach(x => GetAllChildWindows(x, dest));
            return dest;
        }

        private static IEnumerable<Window> ENumChildWindows(IntPtr hParentWindow)
        {
            IntPtr hWnd = IntPtr.Zero;
            while ((hWnd = FindWindowEx(hParentWindow, hWnd, null, null)) != IntPtr.Zero)
            {
                yield return GetWindow(hWnd);
            }
        }

        private static Window GetWindow(IntPtr hWnd)
        {
            int textLen = GetWindowTextLength(hWnd);
            string windowText = null;
            if (0 < textLen)
            {
                StringBuilder windowTextBuffer = new StringBuilder(textLen + 1);
                GetWindowText(hWnd, windowTextBuffer, windowTextBuffer.Capacity);
                windowText = windowTextBuffer.ToString();
            }
            StringBuilder classNameBuffer = new StringBuilder(256);
            GetClassName(hWnd, classNameBuffer, classNameBuffer.Capacity);
            int style = GetWindowLong(hWnd, GWL_STYLE);
            return new Window(hWnd, windowText, classNameBuffer.ToString(), style) { };

        }

        private class VoiceroidWindow
        {
            public VoiceroidWindow(IntPtr content, IntPtr audio)
            {
                Content = content;
                AudioSaveButton = audio;
            }
            public IntPtr Content;
            public IntPtr AudioSaveButton;
        }

        private class Window
        {
            public Window(IntPtr hwnd, string title, string name, int style)
            {
                ClassName = name;
                Title = title;
                hWnd = hwnd;
                Style = style;
            }
            public string ClassName;
            public string Title;
            public IntPtr hWnd;
            public int Style;
        }
    }
}
