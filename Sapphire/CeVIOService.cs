using System.Threading.Tasks;
using CeVIO.Talk.RemoteService;
using Discord;

namespace Sapphire
{
    class CeVIOService
    {
        private Talker talker;
        public CeVIOService(string cast = "IA")
        {
            ServiceControl.StartHost(false);
            talker = new Talker();
            setCast(cast);
        }

        public void speak(string text)
        {
            SpeakingState state = talker.Speak(text);
            state.Wait();
        }

        public async Task play(AudioService audio, IGuild guild, IVoiceChannel channel, string text, string path = null)
        {
            if (path == null)
            {
                path = "C:\\Sapphire\\cevio.wav";
            }
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                await Task.Delay(50);
            }
            setVolume(100);
            await Task.Delay(10);
            bool isSuccess = talker.OutputWaveToFile(text, path);
            await audio.JoinChannel(guild, channel);
            await audio.SendAudioAsync(guild, path);
            await audio.LeaveChannel(guild);
        }

        public void setCast(string cast)
        {
            talker.Cast = cast;
        }

        public void setVolume(uint volume)
        {
            talker.Volume = volume;
        }

        public void setSpeed(uint speed)
        {
            talker.Speed = speed;
        }

        public void setTone(uint tone)
        {
            talker.Tone = tone;
        }

        public void setAlpha(uint alpha)
        {
            talker.Alpha = alpha;
        }

        public void setToneScale(uint tonescale)
        {
            talker.ToneScale = tonescale;
        }
    }
}
