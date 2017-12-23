using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Windows.Automation;
using System.Diagnostics;
using System.Threading;
using Discord.WebSocket;

namespace Sapphire
{
    class VoiceroidService
    {

        private IntPtr hWnd;
        Dictionary<string, string> casts = new Dictionary<string, string>();
        string header;

        public VoiceroidService()
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
            header = casts["結月ゆかり"];
        }
        public async Task play(AudioService audio, IGuild guild,SocketMessage message,string msg,string path=null)
        {
            if (path == null)
            {
                path = "C:\\Sapphire\\tmp.wav";
            }
            IVoiceChannel channel = (message.Author as IGuildUser).VoiceChannel;
            await save(msg);
            await audio.JoinChannel(guild, channel);
            await audio.SendAudioAsync(guild, message.Channel, path);
            await audio.LeaveChannel(guild);

        }

        // VOICEROID2 EDITOR ウインドウハンドル検索
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
                if (i < (RetryCount - 1)) Thread.Sleep(RetryWaitms);
            }

            return hWnd;
        }

        // テキスト転記と再生ボタン押下
        private void talk(string talkText)
        {
            talkText = header + talkText;
            if (hWnd == IntPtr.Zero) return;

            AutomationElement ae = AutomationElement.FromHandle(hWnd);
            TreeScope ts1 = TreeScope.Descendants | TreeScope.Element;
            TreeScope ts2 = TreeScope.Descendants;

            // アプリケーションウインドウ
            AutomationElement editorWindow = ae.FindFirst(ts1, new PropertyCondition(AutomationElement.ClassNameProperty, "Window"));

            // 再生ボタン、テキストボックスが配置されているコンテナの名前は“c”
            AutomationElement customC = ae.FindFirst(ts1, new PropertyCondition(AutomationElement.AutomationIdProperty, "c"));

            // テキストボックスにテキストを転記
            AutomationElement textBox = customC.FindFirst(ts2, new PropertyCondition(AutomationElement.AutomationIdProperty, "TextBox"));
            ValuePattern elem1 = textBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            elem1.SetValue(talkText);

            // 再生ボタンを押す。再生ボタンはボタンのコレクション5番目(Index=4)
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

            // アプリケーションウインドウ
            AutomationElement editorWindow = ae.FindFirst(ts1, new PropertyCondition(AutomationElement.ClassNameProperty, "Window"));

            // 再生ボタン、テキストボックスが配置されているコンテナの名前は“c”
            AutomationElement customC = ae.FindFirst(ts1, new PropertyCondition(AutomationElement.AutomationIdProperty, "c"));

            // テキストボックスにテキストを転記
            AutomationElement textBox = customC.FindFirst(ts2, new PropertyCondition(AutomationElement.AutomationIdProperty, "TextBox"));
            ValuePattern elem1 = textBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            elem1.SetValue(saveText);

            AutomationElementCollection buttons = customC.FindAll(ts2, new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, "ボタン"));
            InvokePattern elem2 = buttons[8].GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            elem2.Invoke();

            await Task.Delay(100);
            AutomationElement saveWindow = ae.FindFirst(ts2, new PropertyCondition(AutomationElement.NameProperty, "音声保存"));
            AutomationElement okButton = saveWindow.FindFirst(ts2, new PropertyCondition(AutomationElement.NameProperty, "OK"));
            InvokePattern elem3 = okButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            elem3.Invoke();

            Thread.Sleep(300);
            AutomationElement fileExp = saveWindow.FindFirst(ts2, new PropertyCondition(AutomationElement.NameProperty, "名前を付けて保存"));
            AutomationElement saveButton = fileExp.FindFirst(ts2, new PropertyCondition(AutomationElement.NameProperty, "保存(S)"));
            InvokePattern elem4 = saveButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            elem4.Invoke();

            Thread.Sleep(300);
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
