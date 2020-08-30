using System.Collections.Generic;

namespace ICSP.WebProxy.Configuration
{
  public class StaticFiles
  {
    public List<StaticFilesDirectories> Directories { get; set; } = new List<StaticFilesDirectories>();

    public StaticFilesHeaders Headers { get; set; } = new StaticFilesHeaders();
  }
}
