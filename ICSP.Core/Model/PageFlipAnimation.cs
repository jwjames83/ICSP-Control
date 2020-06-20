using System.ComponentModel;

using Newtonsoft.Json;

namespace ICSP.Core.Model
{
  public class PageFlipAnimation
  {
    /// <summary>
    /// Animation
    /// </summary>
    [DefaultValue(AnimationType.None)]
    [JsonProperty("#text", Order = 2, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public AnimationType Animation { get; set; }

    /// <summary>
    /// Direction
    /// </summary>
    [JsonProperty("di", Order = 1)]
    public AnimationDirection Direction { get; set; }
  }
}
