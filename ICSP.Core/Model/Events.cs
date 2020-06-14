using System.Collections.Generic;
using Newtonsoft.Json;

namespace ICSP.Core.Model
{
  public class Events
  {
    /*
      "ep": [
        {
          "pgFlip": [
            {
              "item": "0",
              "type": "sShow",
              "#text": "Popup Page 1"
            },
            {
              "item": "1",
              "type": "sHide",
              "#text": "Popup Page 1"
            },
            {
              "item": "2",
              "type": "sToggle",
              "#text": "Popup Page 1"
            },
            {
              "item": "3",
              "type": "scPage",
              "#text": "Page 1"
            },
            {
              "item": "4",
              "type": "scPanel",
              "#text": "n/a"
            }
          ],
          "launch": [
            {
              "item": "5",
              "action": "close_all",
              "id": "0",
              "#text": "n/a"
            },
            {
              "item": "6",
              "action": "status_show",
              "id": "0",
              "#text": "n/a"
            },
            {
              "item": "7",
              "action": "status_hide",
              "id": "0",
              "#text": "n/a"
            }
          ],
          "command": {
            "item": "8",
            "port": "1",
            "#text": "none"
          },
          "string": {
            "item": "9",
            "port": "1",
            "#text": "none"
          },
          "custom": {
            "item": "10",
            "value1": "none",
            "value2": "none",
            "value3": "none",
            "text": "none",
            "encode": "none",
            "#text": "none"
          }
        }
      ],
      "er": [
        {
          "pgFlip": {
            "item": "0",
            "type": "sShow",
            "#text": "Popup Page 1"
          }
        }
      ],
    */

    /// <summary>
    /// Pageflips
    /// </summary>
    [JsonProperty("pgFlip", Order = 1, NullValueHandling = NullValueHandling.Ignore)]
    public List<object> PageFlips { get; set; }

    /// <summary>
    /// Actions of type command launch
    /// </summary>
    [JsonProperty("launch", Order = 2, NullValueHandling = NullValueHandling.Ignore)]
    public List<object> ActionsLaunch { get; set; }

    /// <summary>
    /// Actions of type command
    /// </summary>
    [JsonProperty("command", Order = 3, NullValueHandling = NullValueHandling.Ignore)]
    public List<object> ActionsCommand { get; set; }

    /// <summary>
    /// Actions of type string
    /// </summary>
    [JsonProperty("string", Order = 4, NullValueHandling = NullValueHandling.Ignore)]
    public List<object> ActionsString { get; set; }

    /// <summary>
    /// Actions of type custom
    /// </summary>
    [JsonProperty("custom", Order = 5, NullValueHandling = NullValueHandling.Ignore)]
    public List<object> ActionsCustom { get; set; }
  }
}
