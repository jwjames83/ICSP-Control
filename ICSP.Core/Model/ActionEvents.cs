using System.Collections.Generic;

using Newtonsoft.Json;

namespace ICSP.Core.Model
{
  public class ActionEvents
  {
    /// <summary>
    /// Page Flips
    /// </summary>
    [JsonProperty("pgFlip", Order = 1, NullValueHandling = NullValueHandling.Ignore)]
    public List<ActionEventPageFlip> PageFlips { get; set; }

    /// <summary>
    /// Actions of type launch
    /// </summary>
    [JsonProperty("launch", Order = 2, NullValueHandling = NullValueHandling.Ignore)]
    public List<ActionEventLaunch> ActionsLaunch { get; set; }

    /// <summary>
    /// Actions of type command
    /// </summary>
    [JsonProperty("command", Order = 3, NullValueHandling = NullValueHandling.Ignore)]
    public List<ActionEventCommand> ActionsCommand { get; set; }

    /// <summary>
    /// Actions of type string
    /// </summary>
    [JsonProperty("string", Order = 4, NullValueHandling = NullValueHandling.Ignore)]
    public List<ActionEventString> ActionsString { get; set; }

    /// <summary>
    /// Actions of type custom
    /// </summary>
    [JsonProperty("custom", Order = 5, NullValueHandling = NullValueHandling.Ignore)]
    public List<ActionEventCustom> ActionsCustom { get; set; }
  }
}
