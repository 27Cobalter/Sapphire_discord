using Discord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
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
        private System.Random _r;

        private List<string> castList;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            castList = new List<string>();
            castList.Add("IA");
            castList.Add("ONE");
            castList.Add("結月ゆかり");
            castList.Add("紲星あかり");

            _r = new System.Random();

            _audio = new AudioService();
            _cevio = new CeVIOService();
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
            _client.UserVoiceStateUpdated += UserVoiceStateUpdate;

            string token = "tokon";
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage message)
        {
            string[] messageArr = message.Content.Split();
            string command = messageArr[0];
            if (command == ":ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }
            else if (command == ":join")
            {
                IGuild guild = (message.Author as IGuildUser).Guild;
                IVoiceChannel channel = (message.Author as IGuildUser).VoiceChannel;
                var process = _audio.JoinChannel(guild, channel);
            }
            else if (command == ":leave")
            {
                IGuild guild = (message.Author as IGuildUser).Guild;
                var process = _audio.LeaveChannel(guild);
            }
            else if (command == ":play")
            {
                IGuild guild = (message.Author as IGuildUser).Guild;
                IVoiceChannel channel = (message.Author as IGuildUser).VoiceChannel;
                string path = getParam(message);
                var process = _audio.play(_audio, guild, channel, path);
            }
            else if (command == ":ia")
            {
                _cevio.setCast("IA");
                IGuild guild = (message.Author as IGuildUser).Guild;
                IVoiceChannel channel = (message.Author as IGuildUser).VoiceChannel;
                string text = getParam(message);
                var process = _cevio.play(_audio, guild, channel, text);
            }
            else if (command == ":one")
            {
                _cevio.setCast("ONE");
                IGuild guild = (message.Author as IGuildUser).Guild;
                IVoiceChannel channel = (message.Author as IGuildUser).VoiceChannel;
                string text = getParam(message);
                var process = _cevio.play(_audio, guild, channel, text);
            }
            else if (command == ":yukari")
            {
                _voiceroid.setCast("結月ゆかり");
                IGuild guild = (message.Author as IGuildUser).Guild;
                IVoiceChannel channel = (message.Author as IGuildUser).VoiceChannel;
                string text = getParam(message);
                var process = _voiceroid.play(_audio, guild, channel, text);
            }
            else if (command == ":akari")
            {
                _voiceroid.setCast("紲星あかり");
                IGuild guild = (message.Author as IGuildUser).Guild;
                IVoiceChannel channel = (message.Author as IGuildUser).VoiceChannel;
                string text = getParam(message);
                var process = _voiceroid.play(_audio, guild, channel, text);
            }
        }

        private async Task UserVoiceStateUpdate(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            Console.WriteLine(user.Username);
            await Task.Delay(1);
            if (user.Username == _client.CurrentUser.Username)
            {
                return;
            }
            IGuild guild;
            IVoiceChannel channel;
            string text;
            if (before.VoiceChannel != null && after.VoiceChannel != null)
            {
                if (before.VoiceChannel.Equals(after.VoiceChannel) && before.IsSelfMuted != after.IsSelfMuted)
                {
                    Console.WriteLine(user);
                    channel = after.VoiceChannel;
                    text = user.Username + "さんのマイクが" + (after.IsSelfMuted ? "オフ" : "オン") + "になりました";
                }
                else
                {
                    channel = after.VoiceChannel;
                    text = user.Username + "さんが" + before.VoiceChannel + "から" + after.VoiceChannel + "に移動しました";
                }
            }
            else if (before.VoiceChannel != null)
            {
                channel = after.VoiceChannel;
                text = user.Username + "さんが" + before.VoiceChannel + "から退出しました";
            }
            else if (after.VoiceChannel != null)
            {
                channel = after.VoiceChannel;
                text = user.Username + "さんが" + after.VoiceChannel + "に入室しました";
            }
            else
            {
                channel = after.VoiceChannel;
                text = "どこから来たんです?";
            }
            guild = channel.Guild;

            int cast = _r.Next(castList.Count);
            if (cast < castList.FindIndex(x => x.Equals("結月ゆかり")))
            {
                _cevio.setCast(castList[cast]);
                var prcocess = _cevio.play(_audio, guild, channel, text);
            }
            else
            {
                _voiceroid.setCast(castList[cast]);
                var prcocess = _voiceroid.play(_audio, guild, channel, text);
            }

        }

        private string getParam(SocketMessage message)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\"(?<text>.*?)\"");
            if (regex.IsMatch(message.Content))
            {
                return regex.Match(message.Content).Groups["text"].Value;
            }
            return "ゆかいあはいいぞ";
        }
    }
}
