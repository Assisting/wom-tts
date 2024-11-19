using System.Text;
using System.Text.Json;
using NAudio.Wave;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace wom_tts
{
    public class TtsSocketCliet : WebSocketBehavior
    {
        private const string RemoteServerUrl = "http://eriklabine.net:8255/tts";

        protected override async void OnMessage(MessageEventArgs socketMessage)
        {
            if (!socketMessage.IsText)
            {
                Console.WriteLine("That's not text you silly goose");
                return;
            }

            IpcMessage? ipcMessage = JsonSerializer.Deserialize<IpcMessage>(socketMessage.Data);
            string payload = ipcMessage?.Payload ?? "";
            string text = Encoding.ASCII.GetString(
                Encoding.Convert(
                    Encoding.UTF8,
                    Encoding.GetEncoding(
                        Encoding.ASCII.EncodingName,
                        new EncoderReplacementFallback(string.Empty),
                        new DecoderExceptionFallback()
                        ),
                    Encoding.UTF8.GetBytes(payload)
                )
            );

            Console.WriteLine($"\t\t{ipcMessage?.Speaker}> {payload}");

            try
            {
                using var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"{RemoteServerUrl}?text={Uri.EscapeDataString(text)}");

                var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using var audioStream = await response.Content.ReadAsStreamAsync();
                var waveOut = new WaveOutEvent();
                var bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(11025, 16, 1));
                waveOut.Init(bufferedWaveProvider);

                byte[] buffer = new byte[8192];
                int bytesRead;
                bool playbackStarted = false;

                try
                {
                    while ((bytesRead = await audioStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        bufferedWaveProvider.AddSamples(buffer, 0, bytesRead);

                        if (!playbackStarted && bufferedWaveProvider.BufferedBytes > bufferedWaveProvider.WaveFormat.AverageBytesPerSecond / 2)
                        {
                            playbackStarted = true;
                            waveOut.Play();
                        }
                    }

                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(100);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error during playback: {ex.Message}");
                }
                finally
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error during request handling: {ex.Message}");
            }
        }
    }
}
