using System.Windows;
using ReceiptPrinterEmulator.Emulator;
using ReceiptPrinterEmulator.Networking;
using WebSocketServer = WebSocketSharp.Server.WebSocketServer;

namespace ReceiptPrinterEmulator
{
    public partial class App : Application
    {
        public static ReceiptPrinter? Printer = null;
        public static NetServer? Server = null;

        private static WebSocketServer _webSocketServer = default!;
        
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Printer = new ReceiptPrinter(PaperConfiguration.Default);

            Server = new NetServer(1234);
            _ = Server.Run();
            _webSocketServer = new WebSocketServer("ws://localhost:5000");
            _webSocketServer.AddWebSocketService<PrintingBehavior>("/ws", () => new PrintingBehavior());
            _webSocketServer.Start();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Server?.Stop();
            _webSocketServer.Stop();
        }
    }
}