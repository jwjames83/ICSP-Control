using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSP.WebProxy.Configuration.Options
{
  public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
  {
    private readonly IWebHostEnvironment mEnvironment;
    private readonly IOptionsMonitor<T> mOptions;

    private readonly string mSection;
    private readonly string mFile;

    public WritableOptions(IWebHostEnvironment environment, IOptionsMonitor<T> options, string section, string file)
    {
      mEnvironment = environment;
      mOptions = options;

      mSection = section;
      mFile = file;
    }

    public T Value => mOptions.CurrentValue;

    public T Get(string name) => mOptions.Get(name);

    public void Update(Action<T> applyChanges)
    {
      var lCts = new CancellationTokenSource();

      // OnChange return a IDisposable which should be disposed to stop listening for changes
      using var lMonitorListener = mOptions.OnChange(config => { lCts.Cancel(); });

      var lFileProvider = mEnvironment.ContentRootFileProvider;
      var lFileInfo = lFileProvider.GetFileInfo(mFile);
      var lPhysicalPath = lFileInfo.PhysicalPath;

      var lObj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(lPhysicalPath));
      
      var lSection = lObj.TryGetValue(mSection, out JToken section) ? JsonConvert.DeserializeObject<T>(section.ToString()) : (Value ?? new T());

      applyChanges(lSection);

      lObj[mSection] = JObject.Parse(JsonConvert.SerializeObject(lSection));

      File.WriteAllText(lPhysicalPath, JsonConvert.SerializeObject(lObj, Formatting.Indented));
    }

    public async Task UpdateAsync(int millisecondsDelay, Action<T> applyChanges)
    {
      var lCts = new CancellationTokenSource();

      // OnChange return a IDisposable which should be disposed to stop listening for changes
      using var lMonitorListener = mOptions.OnChange(config => { lCts.Cancel(); });


      var lFileProvider = mEnvironment.ContentRootFileProvider;
      var lFileInfo = lFileProvider.GetFileInfo(mFile);
      var lPhysicalPath = lFileInfo.PhysicalPath;

      var lObj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(lPhysicalPath));

      var lSection = lObj.TryGetValue(mSection, out JToken section) ? JsonConvert.DeserializeObject<T>(section.ToString()) : (Value ?? new T());

      applyChanges(lSection);

      lObj[mSection] = JObject.Parse(JsonConvert.SerializeObject(lSection));

      File.WriteAllText(lPhysicalPath, JsonConvert.SerializeObject(lObj, Formatting.Indented));

      try
      {
        await Task.Delay(millisecondsDelay, lCts.Token);
      }
      catch(TaskCanceledException) { }
    }
  }
}
