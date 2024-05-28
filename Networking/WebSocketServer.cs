using System;
using System.Media;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;
using Logger = ReceiptPrinterEmulator.Logging.Logger;


namespace ReceiptPrinterEmulator.Networking;

public class PrintingBehavior : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        Logger.Info("new message received");
        try
        {
            var message = "";
            if (e.IsBinary)
            {
                message = Encoding.ASCII.GetString(e.RawData);
            }
            else message = e.Data;
            Logger.Info(message);
            Dispatcher.CurrentDispatcher.Invoke(()=> App.Printer?.FeedEscPos(message));
        }
        catch (Exception exception)
        {
            Logger.Exception(exception, exception.Message);
        }
        base.OnMessage(e);
    }

    protected override void OnError(ErrorEventArgs e)
    {
        Logger.Exception(e.Exception, e.Message);
        base.OnError(e);
    }

    protected override void OnClose(CloseEventArgs e)
    {
        Logger.Info($"Closing connection: {this.ID}", e.Reason);
        base.OnClose(e);
    }

    protected override void OnOpen()
    {
        Logger.Info($"New connection opened {this.ID}");
        base.OnOpen();
    }
}
// public class WebSocketServer
// {
//     private HttpListener HttpListener;
//     public async Task HandleWebSocket(HttpListenerContext context)
//     {
//         if (context.Request.IsWebSocketRequest)
//         {
//             HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
//             WebSocket webSocket = webSocketContext.WebSocket;
//
//             byte[] buffer = new byte[1024 * 4];
//             while (webSocket.State == WebSocketState.Open)
//             {
//                 WebSocketReceiveResult result =
//                     await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//                 string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
//                 Logger.Info($"Received: {message}");
//
//                 // Process the message
//                 HandleMessage(message);
//
//                 // Optionally send a response back to the client
//                 byte[] responseBuffer = Encoding.ASCII.GetBytes("Message received");
//                 await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true,
//                     CancellationToken.None);
//
//                 if (result.MessageType == WebSocketMessageType.Close)
//                 {
//                     await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
//                 }
//             }
//         }
//         else
//         {
//             context.Response.StatusCode = 400;
//             context.Response.Close();
//         }
//     }
//     
//     private static void HandleMessage(string message)
//     {
//         try
//         {
//             // Implement your message handling logic here
//             App.Printer?.FeedEscPos(message);
//         }
//         catch (Exception e)
//         {
//            Logger.Exception(e,e.Message);
//         }
//        
//     }
//     
//     public async Task StartServer(string uriPrefix)
//     {
//         try
//         {
//             HttpListener = new HttpListener();
//             HttpListener.Prefixes.Add(uriPrefix);
//             HttpListener.Start();
//             Logger.Info($"Listening for WebSocket connections on {uriPrefix}");
//             
//             while (HttpListener.IsListening)
//             {
//                 HttpListenerContext context = await HttpListener.GetContextAsync();
//                 _ = Task.Run(() => HandleWebSocket(context)); // Handle each connection in a new task
//             }
//
//         }
//         catch (Exception e)
//         {
//            Logger.Exception(e, e.Message);
//         }
//     }
//
//     public void Stop()
//     {
//         HttpListener.Stop();
//     }
//
// }

