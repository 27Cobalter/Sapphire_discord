using Discord;
using Discord.Audio;
using System;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Sapphire
{
    class Voice
    {
        private DiscordClient Client { get; set; }

        public IAudioClient VoiceClient { get; set; }
        private bool _audioflag = true;
        private bool _exeflag = false;

        public void Audioflag(bool flag)
        {
            _audioflag = flag;
        }
        public bool Exeflag()
        {
            return _exeflag;
        }

        public Voice(DiscordClient client)
        {
            Client = client;
        }

        public async Task SendAudio(Channel vChannel, string filepath)
        {
            await JoinChannel(vChannel);
            PlayAudio(vChannel, filepath);
        }

        private async Task JoinChannel(Channel vChannel)
        {
            VoiceClient = await Client.GetService<AudioService>().Join(vChannel);
        }

        private async void LeaveChannel(Channel vChannel)
        {
            Console.WriteLine("leave");
            await Client.GetService<AudioService>().Leave(vChannel);
        }

        private void PlayAudio(Channel vChannel, string filepath)
        {
            if (!System.IO.File.Exists(filepath))
                throw new Exception("not found " + filepath);

            var channelCount = Client.GetService<AudioService>().Config.Channels;
            var OutFormat = new WaveFormat(48000, 16, channelCount);
            using (AudioFileReader Reader = new AudioFileReader(filepath))
            using (var resampler = new MediaFoundationResampler(Reader, OutFormat))
            {
                resampler.ResamplerQuality = 60;
                int blockSize = OutFormat.AverageBytesPerSecond / 50;
                byte[] buffer = new byte[blockSize];
                int byteCount;


                _exeflag = true;
                while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0 && _audioflag)
                {
                    if (byteCount < blockSize)
                    {
                        for (int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                    }
                    VoiceClient.Send(buffer, 0, blockSize);
                }
                System.Threading.Thread.Sleep(2000);
                _exeflag = false;
                LeaveChannel(vChannel);
            }
        }
    }
}
