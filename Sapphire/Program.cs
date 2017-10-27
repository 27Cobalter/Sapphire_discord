using CeVIO.Talk.RemoteService;
using Discord;
using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sapphire
{
    class Program
    {
        static void Main(string[] args) => new Program().Start();

        private DiscordClient _client;
        private Voice vo;
        private bool enable = true;

        public void Start()
        {
            ServiceControl.StartHost(false);
            Console.WriteLine("Sapphire is get up!");
            _client = new DiscordClient();
            _client.UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
            });

            _client.MessageReceived += async (s, e) =>
            {
                Console.WriteLine(e.Server.ToString() + " -> #" + e.Channel.ToString() + " -> " + e.User.ToString() + "\"" + e.Message.Text.ToString() + "\"");
                if (!e.Message.Text.ToString().Equals(""))
                {
                    //コマンド記述
                    if (e.Message.Text.ToString().Substring(0, 1).Equals(":") && enable)
                    {
                        string[] messageArr = e.Message.Text.ToString().Split();
                        List<string> message = new List<string>(messageArr);
                        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\"(?<text>.*?)\"");
                        //IPコマンド
                        if (message[0].Equals(":ip"))
                        {
                            WebClient wclient = new WebClient();
                            string page = wclient.DownloadString("https://www.cman.jp/network/support/go_access.cgi");
                            System.Text.RegularExpressions.Regex ipregex = new System.Text.RegularExpressions.Regex("<div class=\"outIp\">(?<text>.*?)</div>");
                            string ipadd = ipregex.Match(page).Groups["text"].Value;
                            Console.WriteLine(ipadd);
                            await e.Channel.SendMessage(ipadd);
                        }
                        else if (message[0].Equals(":voice"))
                        {
                            if (vo != null)
                            {
                                if (vo.Exeflag())
                                {
                                    vo.Audioflag(false);
                                    System.Threading.Thread.Sleep(1000);
                                    vo.Audioflag(true);
                                }
                            }

                            if (!(message[1].Equals("stop")))
                            {
                                vo = new Voice(_client);
                                try
                                {
                                    await vo.SendAudio(e.User.VoiceChannel, regex.Match(e.Message.Text).Groups["text"].Value);
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception.Message);
                                    await e.Channel.SendMessage(exception.Message);
                                }
                            }
                        }
                        else if (message[0].Equals(":yukari"))
                        {
                            if (regex.IsMatch(e.Message.Text))
                            {
                                enable = false;
                                Voiceroid Yukari = new Voiceroid();
                                Yukari.SaveText(regex.Match(e.Message.Text).Groups["text"].Value);
                                enable = true;

                                if (vo != null)
                                {
                                    if (vo.Exeflag())
                                    {
                                        vo.Audioflag(false);
                                        System.Threading.Thread.Sleep(1000);
                                        vo.Audioflag(true);
                                    }
                                }

                                vo = new Voice(_client);
                                try
                                {
                                    await vo.SendAudio(e.User.VoiceChannel, "C:\\Sapphire\\tmp.wav");
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception.Message);
                                    await e.Channel.SendMessage(exception.Message);
                                }
                            }
                        }
                        else if (message[0].Equals(":ia") || message[0].Equals(":one"))
                        {
                            if (regex.IsMatch(e.Message.Text))
                            {
                                enable = false;
                                CeVIOVoice cevi = new CeVIOVoice();
                                bool flag = message[0].Equals(":ia");
                                cevi.SaveText(regex.Match(e.Message.Text).Groups["text"].Value, flag ? "IA" : "ONE");
                                enable = true;

                                if (vo != null)
                                {
                                    if (vo.Exeflag())
                                    {
                                        vo.Audioflag(false);
                                        System.Threading.Thread.Sleep(1000);
                                        vo.Audioflag(true);
                                    }
                                }

                                vo = new Voice(_client);
                                try
                                {
                                    await vo.SendAudio(e.User.VoiceChannel, "C:\\Sapphire\\cevio.wav");
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception.Message);
                                    await e.Channel.SendMessage(exception.Message);
                                }
                            }
                        }
                    }
                }
                else
                    await Task.Delay(100);
            };

            _client.UserUpdated += async (s, e) =>
            {
                if (e.Before.Name.Equals("Sapphire") || !enable) return;
                if (e.Before.VoiceChannel == e.After.VoiceChannel) return;
                enable = false;
                string mes = null;
                Channel chan;
                int chnum;
                if (e.After.VoiceChannel == null)
                {
                    try
                    {
                        if (e.Before.VoiceChannel.Users.Count() == 0)
                            return;
                    }
                    catch { return; }
                    chnum = 1;
                    mes = e.After.Name + "さんが" + e.Before.VoiceChannel + "から退出しました";
                    chan = e.Before.VoiceChannel;
                    Console.WriteLine(mes);
                }
                else if (e.Before.VoiceChannel == null)
                {
                    try
                    {
                        if (e.After.VoiceChannel.Users.Count() == 0)
                            return;
                    }
                    catch { return; }
                    chnum = 1;
                    mes = e.After.Name + "さんが" + e.After.VoiceChannel + "に入室しました";
                    chan = e.After.VoiceChannel;
                    Console.WriteLine(mes);
                }
                else
                {
                    try
                    {
                        if (e.Before.VoiceChannel.Users.Count() == 0)
                            return;
                        chnum = 2;
                    }
                    catch { chnum = 1; }
                    mes = e.After.Name + "さんが" + e.Before.VoiceChannel + "から" + e.After.VoiceChannel + "へ移動しました";
                    chan = e.Before.VoiceChannel;
                    Console.WriteLine(mes);
                }

                CeVIOVoice cevi = new CeVIOVoice();
                bool flag = (Environment.TickCount % 2 == 1);
                cevi.SaveText(mes, flag ? "IA" : "ONE");

                if (vo != null)
                {
                    if (vo.Exeflag())
                    {
                        vo.Audioflag(false);
                        System.Threading.Thread.Sleep(1000);
                        vo.Audioflag(true);
                    }
                }

                for (int i = 0; i < chnum; i++)
                {
                    vo = new Voice(_client);
                    try
                    {
                        await vo.SendAudio(chan, "C:\\Sapphire\\cevio.wav");
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                        await Task.Delay(100);
                    }
                    if (chnum == 2)
                    {
                        while (vo.Exeflag()) ;
                        System.Threading.Thread.Sleep(1000);
                        chan = e.After.VoiceChannel;
                        await _client.GetService<AudioService>().Join(chan);
                    }
                }
                enable = true;
            };

            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect("MjU3ODgyMzk5MzMxOTc1MTY5.CzBLdQ.WiXHPHMSHiTvm4D7vCPq5LH0D7k", TokenType.Bot);
            });


        }
    }
}
