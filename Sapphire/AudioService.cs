using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;

namespace Sapphire
{
    class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task JoinChannel(IGuild guild, IVoiceChannel target)
        {
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                return;
            }
            if (target.Guild.Id != guild.Id)
            {
                return;
            }

            var audioClient = await target.ConnectAsync();

            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
            {
                await Log($"Connected to voice on {guild.Name}");
            }
        }

        public async Task LeaveChannel(IGuild guild)
        {
            IAudioClient client;
            if(ConnectedChannels.TryRemove(guild.Id,out client))
            {
                await client.StopAsync();
                await Log($"Disconnected to voice on {guild.Name}");
            }
        }

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                await Log($"File does not exist {path}");
                return;
            }
            IAudioClient client;
            if(ConnectedChannels.TryGetValue(guild.Id,out client))
            {

                await Log($"Starting playback of {path} in {guild.Name}");
                using (var output = CreateStream(path).StandardOutput.BaseStream)
                using (var stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try { await output.CopyToAsync(stream); }
                    finally { await stream.FlushAsync(); }
                }
            }
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-i \"{path}\" -ac 2 -f s16le -ar 46000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
        
        private Task Log(String msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }
    }
}
