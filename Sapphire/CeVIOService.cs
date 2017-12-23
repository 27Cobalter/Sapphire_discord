using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CeVIO.Talk.RemoteService;
using Discord;

namespace Sapphire
{
    class CeVIOService
    {
        private Talker talker;
        public CeVIOService(string cast)
        {
            ServiceControl.StartHost(false);
            talker = new Talker();
            setCast(cast);
            setVolume(100);
        }

        public void speak(string text)
        {
            SpeakingState state = talker.Speak(text);
            state.Wait();
        }
        
        public async Task play(AudioService audio,IGuild guild, IMessageChannel channel,string text,string path = null)
        {
            if(path == null)
            {
                path = "C:\\Sapphire\\cevio.wav";
            }
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                await Task.Delay(50);
            }
            await Task.Delay(10);
            bool isSuccess = talker.OutputWaveToFile(text, path);
            await audio.SendAudioAsync(guild, channel, path);
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
