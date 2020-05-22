using System;
using System.Diagnostics;
using System.Reflection;

using ICSP.Core.Logging;

namespace ICSPControl.DevStuff
{
  public class Singleton<TSingleton> : IDisposable
  {
    #region Attributes

    // Hilfsfeld für eine sichere Threadsynchronisierung
    private static readonly object SyncRoot;

    private static TSingleton mInstance;

    private static bool mIsDisposed;

    #endregion

    #region Constructors

    static Singleton()
    {
      Singleton<TSingleton>.mInstance = default(TSingleton);
      Singleton<TSingleton>.mIsDisposed = false;
      Singleton<TSingleton>.SyncRoot = new object();
    }

    #endregion

    #region Methods

    public static void Dispose()
    {
      var lInstance = Singleton<TSingleton>.mInstance as IDisposable;

      if (lInstance != null)
        lInstance.Dispose();
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    void IDisposable.Dispose()
    {
      Singleton<TSingleton>.mIsDisposed = true;

      Dispose(true);
    }

    #endregion

    #region Properties

    public static TSingleton Instance
    {
      [DebuggerStepThrough]
      get
      {
        try
        {
          if (Singleton<TSingleton>.mInstance == null || Singleton<TSingleton>.mIsDisposed)
          {
            lock (Singleton<TSingleton>.SyncRoot)
            {
              if (Singleton<TSingleton>.mInstance == null || Singleton<TSingleton>.mIsDisposed)
              {
                var lType = typeof(TSingleton);

                var lConstructors = lType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

                if (lConstructors.Length <= 0)
                  throw new Exception(string.Format("Die Klasse '{0}' benötigt einen nicht-public Instance-Konstruktor für das Singleton-Template.", lType.ToString()));

                var lConstructor = lConstructors[0];

                Singleton<TSingleton>.mInstance = (TSingleton)lConstructor.Invoke(null);
                Singleton<TSingleton>.mIsDisposed = false;
              }
            }
          }
        }
        catch (Exception ex)
        {
          Logger.LogError("Singleton konnte nicht erstellt werden!");
          Logger.LogError(ex);
        }

        return Singleton<TSingleton>.mInstance;
      }
    }

    #endregion
  }
}