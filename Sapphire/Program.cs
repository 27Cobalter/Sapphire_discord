using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Audio;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Sapphire;

namespace test
{
    class Program
    {
        private AudioService _audio;
        private CeVIOService _cevio;
        private CommandService _commands;
        private DiscordSocketClient _client;
        private VoiceroidService _voiceroid;
        private IServiceProvider _services;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        [Command(RunMode=RunMode.Async)]
        public async Task MainAsync()
        {
            _audio = new AudioService();
            _cevio = new CeVIOService("IA");
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _voiceroid = new VoiceroidService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(_audio)
                .BuildServiceProvider();

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

            string token = "token";
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        [Command(RunMode = RunMode.Async)]
        private async Task MessageReceived(SocketMessage message)
        {
            string[] messageArr = message.Content.Split();
            string command = messageArr[0];
            if (command== ":ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }
            else if (command== ":join")
            {
                IGuild guild = (message.Author as IGuildUser).Guild;
                IVoiceChannel voiceChannel = (message.Author as IGuildUser).VoiceChannel;
                var process = _audio.JoinChannel(guild, voiceChannel);
            }
            else if (command== ":leave")
            {
                IGuild guild = (message.Author as IGuildUser).Guild;
                var process = _audio.LeaveChannel((message.Author as IGuildUser).Guild);
            }
            else if (command== ":ppp")
            {
                IGuild guild = (message.Author as IGuildUser).Guild;
                var process = _audio.SendAudioAsync(guild, message.Channel, "C:\\Sapphire\\tmp.wav");
            }
            else if (command== ":play")
            {
                IGuild guild = (message.Author as IGuildUser).Guild;
                var process = _audio.SendAudioAsync(guild, message.Channel, "C:\\Sapphire\\aaa.wav");
            }
            else if (command== ":IA")
            {
                IGuild guild = (message.Author as IGuildUser).Guild;
                var sleep = _cevio.play(_audio, guild, message.Channel, "いあちゃんです");
            }
            else if (command== ":yukari")
            {
                IGuild guild = (message.Author as IGuildUser).Guild;
                _voiceroid.setCast("結月ゆかり");
                string text = getParam(message);
                if (text == null)
                {
                    text = "にゃーん";
                }
                    var process = _voiceroid.play(_audio, guild, message, text);
            }else if(command== ":akari")
            {
                IGuild guild = (message.Author as IGuildUser).Guild;
                _voiceroid.setCast("紲星あかり");
                string text = getParam(message);
                if (text == null)
                {
                    text = "にゃーん";
                }
                    var process = _voiceroid.play(_audio, guild, message, text);
            }
        }

        public string getParam(SocketMessage message)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\"(?<text>.*?)\"");
            if (regex.IsMatch(message.Content))
            {
                return regex.Match(message.Content).Groups["text"].Value;
            }
            return null;
        }
    }
}
