using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Media;

namespace VoiceVoxTest
{
    class Program
    {
        static HttpClient httpClient;
        static void Main(string[] args)
        {
            using(httpClient = new HttpClient())
            {
                while (true)
                {
                    string readString = Console.ReadLine();
                    if (readString == "exit")
                    {
                        break;
                    }
                    SendToVoiceVox(readString);
                }
            }
        }


        static async void SendToVoiceVox(string text)
        {
            string speaker = "0";

            var parameters = new Dictionary<string, string>()
            {
                { "text", text },
                { "speaker", speaker },
            };
            string encodedParamaters = await new FormUrlEncodedContent(parameters).ReadAsStringAsync();
            using (var resultAudioQuery = await httpClient.PostAsync(@"http://localhost:50021/audio_query?" + encodedParamaters, null))
            {
                string resBodyStr = await resultAudioQuery.Content.ReadAsStringAsync();

                var content = new StringContent(resBodyStr, Encoding.UTF8, @"application/json");
                using (var resultSynthesis = await httpClient.PostAsync(@"http://localhost:50021/synthesis?speaker=" + speaker, content))
                {
                    Stream stream = await resultSynthesis.Content.ReadAsStreamAsync();
                    SoundPlayer soundPlayer = new SoundPlayer(stream);
                    soundPlayer.PlaySync();
                }
            }
        }
    }
}
