using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using ICSP.Core;

namespace ICSP.WebProxy.Proxy
{
  public class ICSPConnectionManager
  {
    private readonly object mLockObj = new object();

    private readonly Dictionary<string, ICSPManager> mManagers = new Dictionary<string, ICSPManager>();

    public ICSPManager GetOrCreate(string host, int port)
    {
      var lIpAddress = Dns.GetHostAddresses(host).Where(p => p.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();

      var lKey = $"{lIpAddress}:{port}";

      lock(mLockObj)
      {
        if(!mManagers.TryGetValue(lKey, out var manager))
        {
          manager = new ICSPManager();

          mManagers.Add(lKey, manager);
        }

        return manager;
      }
    }

    public List<ICSPManager> GetAll()
    {
      return mManagers.Values.ToList();
    }

    public bool TryAdd(ICSPManager manager)
    {
      if(manager == null)
        throw new ArgumentNullException(nameof(manager));

      var lIpAddress = Dns.GetHostAddresses(manager.Host).Where(p => p.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();

      var lKey = $"{lIpAddress}:{manager.Port}";

      lock(mLockObj)
      {
        if(mManagers.ContainsKey(lKey))
          return false;

        mManagers.Add(lKey, manager);
      }

      return true;
    }

    public void Remove(ICSPManager manager)
    {
      if(manager == null)
        throw new ArgumentNullException(nameof(manager));
      
      Remove(manager.Host, manager.Port);
    }

    public void Remove(string host, int port)
    {
      if(host == null)
        throw new ArgumentNullException(nameof(host));

      var lIpAddress = Dns.GetHostAddresses(host).Where(p => p.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();

      var lKey = $"{lIpAddress}:{port}";

      mManagers.Remove(lKey);
    }
  }
}
