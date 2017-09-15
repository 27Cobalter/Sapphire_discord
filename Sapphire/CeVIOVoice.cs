using CeVIO.Talk.RemoteService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sapphire
{
    class CeVIOVoice
    {
        private Talker talker;
        public CeVIOVoice()
        {
            talker = new Talker();
        }
        public void SaveText(string text, string cast, uint tone = 70, uint volume = 100)
        {
            ServiceControl.StartHost(false);
            if (System.IO.File.Exists("T:\\Sapphire\\cevio.wav"))
            {
                System.IO.File.Delete("T:\\Sapphire\\cevio.wav");
                System.Threading.Thread.Sleep(50);
            }
            talker.Cast = cast;
            if (cast.Equals("IA"))
                talker.Speed = 56;
            else
                talker.Speed = 50;
            talker.Volume = volume;
            try
            {
                string version = CeVIO.Talk.RemoteService.ServiceControl.HostVersion.ToString();
                talker.ToneScale = tone;
            }
            catch
            {

            }
            talker.OutputWaveToFile(text, "T:\\Sapphire\\cevio.wav") ;
        }
    }
}
