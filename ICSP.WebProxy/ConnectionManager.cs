using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ICSP.WebProxy
{
  public class ConnectionManager
  {
    private static int SocketCounter = 0;

    private ConcurrentDictionary<int, WebSocket> mSockets = new ConcurrentDictionary<int, WebSocket>();

    public WebSocket GetSocketById(int id)
    {
      return mSockets.FirstOrDefault(p => p.Key == id).Value;
    }

    public ConcurrentDictionary<int, WebSocket> GetAll()
    {
      return mSockets;
    }

    public int GetId(WebSocket context)
    {
      return mSockets.FirstOrDefault(p => p.Value == context).Key;
    }

    public int AddSocket(WebSocket socket)
    {
      var lSocketId = Interlocked.Increment(ref SocketCounter);

      mSockets.TryAdd(lSocketId, socket);
      
      return lSocketId;
    }

    public async Task RemoveSocket(int id)
    {
      mSockets.TryRemove(id, out WebSocket lSocket);

      if(lSocket?.State == WebSocketState.CloseReceived)
        await lSocket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the ConnectionManager", CancellationToken.None);
    }
  }
}
