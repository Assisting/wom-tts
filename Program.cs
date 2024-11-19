using WebSocketSharp.Server;

namespace wom_tts
{
    internal class Program
    {
        private const string LocalEndpoint = "/tts";
        private const int WebSocketPort = 8398; // "TEXT"

        public static void Main(string[] args)
        {
            Console.WriteLine(@"
         __      __   _ _        ___       _      __  __        _ _        
         \ \    / / _(_) |_ ___ / _ \ _ _ | |_  _|  \/  |___ __| (_)__ _  
          \ \/\/ / '_| |  _/ -_) (_) | ' \| | || | |\/| / -_) _` | / _` | 
           \_/\_/|_| |_|\__\___|\___/|_||_|_|\_, |_|  |_\___\__,_|_\__,_| 
                                             |__/                         
            Write Only Media Text to Speech Server(tm)
             2024 — Rights that exist may be reserved");
            Console.WriteLine();
            Console.WriteLine($"\tStarting web socket listener at 127.0.0.1:{WebSocketPort}");
            var wssv = new WebSocketServer($"ws://127.0.0.1:{WebSocketPort}");
            wssv.AddWebSocketService<TtsSocketCliet>(LocalEndpoint);
            wssv.Start();
            Console.WriteLine("\tListening. Press any key to shut down.");
            Console.ReadKey(true);
            wssv.Stop();
        }
    }
}
