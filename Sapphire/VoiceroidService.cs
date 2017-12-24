using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using System.Windows.Automation;
using System.Diagnostics;

namespace Sapphire
{
    class VoiceroidService
    {

        private IntPtr hWnd;
        Dictionary<string, string> casts = new Dictionary<string, string>();
        string header;

        public VoiceroidService(string cast = "結月ゆかり")
        {
            hWnd = IntPtr.Zero;
            while (hWnd == IntPtr.Zero)
            {
                hWnd = GetVoiceroid2hWnd();
                if (hWnd == IntPtr.Zero)
                {
                    System.Diagnostics.Process.Start(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\AHS\\VOICEROID2\\VoiceroidEditor.exe");
                }
            }
            casts.Add("結月ゆかり", "結月ゆかり(v1)＞");
            casts.Add("紲星あかり", "紲星あかり＞");
            header = casts[cast];
        }
        public async Task play(AudioService audio, IGuild guild, IVoiceChannel channel, string msg, string path = null)
        {
            if (path == null)
            {
                path = "C:\\Sapphire\\tmp.wav";
            }
            await save(msg);
            await audio.JoinChannel(guild, channel);
            await audio.SendAudioAsync(guild, path);
            await audio.LeaveChannel(guild);

        }

        private static IntPtr GetVoiceroid2hWnd()
        {
            IntPtr hWnd = IntPtr.Zero;

            string winTitle1 = "VOICEROID2";
            string winTitle2 = winTitle1 + "*";
            int RetryCount = 3;
            int RetryWaitms = 1000;

            for (int i = 0; i < RetryCount; i++)
            {
                Process[] ps = Process.GetProcesses();

                foreach (Process pitem in ps)
                {
                    if ((pitem.MainWindowHandle != IntPtr.Zero) &&
                           ((pitem.MainWindowTitle.Equals(winTitle1)) || (pitem.MainWindowTitle.Equals(winTitle2))))
                    {
                        hWnd = pitem.MainWindowHandle;
                    }
                }
                if (hWnd != IntPtr.Zero) break;
                if (i < (RetryCount - 1)) System.Threading.Thread.Sleep(RetryWaitms);
            }

            return hWnd;
        }

        private void talk(string talkText)
        {
            talkText = header + talkText;
            if (hWnd == IntPtr.Zero) return;

            AutomationElement ae = AutomationElement.FromHandle(hWnd);
            TreeScope ts1 = TreeScope.Descendants | TreeScope.Element;
            TreeScope ts2 = TreeScope.Descendants;

            AutomationElement editorWindow = ae.FindFirst(ts1, new PropertyCondition(AutomationElement.ClassNameProperty, "Window"));

            AutomationElement customC = ae.FindFirst(ts1, new PropertyCondition(AutomationElement.AutomationIdProperty, "c"));

            AutomationElement textBox = customC.FindFirst(ts2, new PropertyCondition(AutomationElement.AutomationIdProperty, "TextBox"));
            ValuePattern elem1 = textBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            elem1.SetValue(talkText);

            AutomationElementCollection buttons = customC.FindAll(ts2, new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, "ボタン"));
            InvokePattern elem2 = buttons[4].GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            elem2.Invoke();
        }

        private async Task save(string saveText)
        {
            saveText = header + saveText;
            if (System.IO.File.Exists("C:\\Sapphire\\tmp.wav"))
            {
                System.IO.File.Delete("C:\\Sapphire\\tmp.wav");
            }

            if (hWnd == IntPtr.Zero) return;

            AutomationElement ae = AutomationElement.FromHandle(hWnd);
            TreeScope ts1 = TreeScope.Descendants | TreeScope.Element;
            TreeScope ts2 = TreeScope.Descendants;

            AutomationElement editorWindow = ae.FindFirst(ts1, new PropertyCondition(AutomationElement.ClassNameProperty, "Window"));

            AutomationElement customC = ae.FindFirst(ts1, new PropertyCondition(AutomationElement.AutomationIdProperty, "c"));

            AutomationElement textBox = customC.FindFirst(ts2, new PropertyCondition(AutomationElement.AutomationIdProperty, "TextBox"));
            ValuePattern elem1 = textBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            elem1.SetValue(saveText);

            AutomationElementCollection buttons = customC.FindAll(ts2, new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, "ボタン"));
            InvokePattern elem2 = buttons[8].GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            elem2.Invoke();

            await Task.Delay(1);
            AutomationElement saveWindow = null;
            InvokePattern elem3 = null;
            while (elem3 == null)
            {
                try
                {
                    saveWindow = ae.FindFirst(ts2, new PropertyCondition(AutomationElement.NameProperty, "音声保存"));
                    AutomationElement okButton = saveWindow.FindFirst(ts2, new PropertyCondition(AutomationElement.NameProperty, "OK"));
                    elem3 = okButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                }
                catch
                {
                    System.Threading.Thread.Sleep(100);
                }
            }

            elem3.Invoke();

            AutomationElement fileExp = null;
            InvokePattern elem4 = null;
            while (elem4 == null)
            {
                try
                {
                    fileExp = saveWindow.FindFirst(ts2, new PropertyCondition(AutomationElement.NameProperty, "名前を付けて保存"));
                    AutomationElement saveButton = fileExp.FindFirst(ts2, new PropertyCondition(AutomationElement.NameProperty, "保存(S)"));
                    elem4 = saveButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                }
                catch
                {
                    System.Threading.Thread.Sleep(100);
                }
            }

            elem4.Invoke();

            System.Threading.Thread.Sleep(300);
            AutomationElement fileExp2 = fileExp.FindFirst(ts2, new PropertyCondition(AutomationElement.NameProperty, "名前を付けて保存"));
            if (fileExp2 != null)
            {
                AutomationElement yes = fileExp2.FindFirst(ts2, new PropertyCondition(AutomationElement.NameProperty, "はい(Y)"));
                InvokePattern elem5 = yes.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                elem5.Invoke();
            }

        }

        public void setCast(string cast)
        {
            header = casts[cast];
        }
    }
}
