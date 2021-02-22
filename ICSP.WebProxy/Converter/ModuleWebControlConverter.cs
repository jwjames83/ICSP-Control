using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ICSP.Core;
using ICSP.Core.Constants;
using ICSP.Core.Logging;
using ICSP.Core.Manager.DeviceManager;
using ICSP.WebProxy.Configuration;
using ICSP.WebProxy.Proxy;

using Microsoft.Extensions.Options;

namespace ICSP.WebProxy.Converter
{
  /// <summary>
  /// Implementation for Module WebControl Version 2.7.1
  /// </summary>
  public class ModuleWebControlConverter : IMessageConverter
  {
    public const string STX = "\u0002";
    public const string SSX = ":;";
    public const string ETX = "\u0003";

    public const string WsEvtType_Push     /**/ = "PUSH";
    public const string WsEvtType_Release  /**/ = "RELEASE";
    public const string WsEvtType_Command  /**/ = "COMMAND";
    public const string WsEvtType_String   /**/ = "STRING";
    public const string WsEvtType_Level    /**/ = "LEVEL";

    private const string CaptureGroup_Type = "type";
    private const string CaptureGroup_Port = "port";
    private const string CaptureGroup_Arg1 = "arg1";
    private const string CaptureGroup_Arg2 = "arg2";

    private const string CaptureGroup_Key   /**/ = "key";
    private const string CaptureGroup_Value /**/ = "value";

    // http://regexstorm.net/tester
    // ^(?<type>[^:]+):(?<port>[^:]+):(?<arg1>[^:]+):?(?<arg2>[^;]+)?
    private static readonly Regex RegexMsg = new Regex($"^(?<{CaptureGroup_Type}>[^:]+):(?<{CaptureGroup_Port}>[^:]+):(?<{CaptureGroup_Arg1}>[^:]+):?(?<{CaptureGroup_Arg2}>[^;]+)?");

    // ^(?<type>[^:]+):(?<port>[^:]+):(?<arg1>[^:]+):?(?<arg2>[^;]+)?
    // ^(?<key>\w+)=(?<value>.+)?
    private static readonly Regex RegexKeyValue = new Regex($"^(?<{CaptureGroup_Key}>\\w+)=(?<{CaptureGroup_Value}>.+)?");

    /*
    --------------------------------------
    File: comm.js
    function processMessage (data) { ... }

    // Type   = data[0]
    // Port   = data[1]
    // value1 = data[2]
    // value2 = data[3]
    --------------------------------------

    Command's:
    ON
    OFF
    LEVEL
    ^TXT-
    PAGE-
    PPON-
    @PPN-
    PPOF-
    @PPF-
    @PPX
    ^ICO-
    ^SHO-
    ^ENA-
    ^ANI-

    #TODO COMMAND:
    ^RAF- --> #TODO
    ^RMF- --> #TODO
    ^BCF- --> #TODO

    @PPA  --> #TODO
    @PPK  --> #TODO
    ^BBR- --> #TODO
    ^BMP- --> #TODO
    ^SDM- --> #TODO
    ^VKS- --> #TODO ???
    @AKB  --> #TODO
    AKEYB --> #TODO
    AKEYP --> #TODO
    AKEYR --> #TODO
    @AKP  --> #TODO
    @AKR  --> #TODO
    @SOU  --> #TODO

    #DONE:

    STRING: --> not implemented yet.
    - Regex for commands! > COMMAND:1:^TXT-1,0,//TECHNIK/SZENEN SPEICHERN; --> fixed [^\S{1,}:\d{1,}:.{1,};$]
    */

    public ModuleWebControlConverter(IOptions<WebControlConfig> config)
    {
      SupportUTF8 = config.Value.SupportUTF8;
    }

    /// <summary>
    /// Module WebControl.axs Version 2.5.5 does not encode UTF8.<br/>
    /// Module WebControl.axs Version 2.6.0 does encode UTF8 by default self.
    /// </summary>
    public bool SupportUTF8 { get; set; }

    public ProxyClient Client { get; set; }

    public ushort Device { get; set; }

    public ushort System { get; set; }

    public AmxDevice Dest { get; set; }

    public string FromChannelEvent(ChannelEventArgs e)
    {
      // NetLinx:
      // SendToWebSocket(WS_OpCode_TextFrame, "'ON', SSX, itoa(nPort), SSX, itoa(nChannel)")
      // SendToWebSocket(WS_OpCode_TextFrame, "'OFF', SSX, itoa(nPort), SSX, itoa(nChannel)")

      if(e.Enabled)
        return string.Concat(STX, "ON", SSX, e.Device.Port, SSX, e.Channel, ETX);
      else
        return string.Concat(STX, "OFF", SSX, e.Device.Port, SSX, e.Channel, ETX);
    }

    public string FromLevelEvent(LevelEventArgs e)
    {
      // NetLinx:
      // SendToWebSocket(WS_OpCode_TextFrame, "'LEVEL', SSX, itoa(nPort), SSX, itoa(level.input.level), SSX, format('%d', level.value)")

      return string.Concat(STX, "LEVEL", SSX, e.Device.Port, SSX, e.Level, SSX, e.Value, ETX);
    }

    public string FromCommandEvent(CommandEventArgs e)
    {
      // NetLinx:
      // SendToWebSocket(WS_OpCode_TextFrame, "'COMMAND', SSX, itoa(Data.Device.Port), SSX, cStr")

      // Convert from UTF8 to default ...
      var lStr = SupportUTF8 ? Encoding.Default.GetString(Encoding.UTF8.GetBytes(e.Text)) : e.Text;

      // Workaround for "!T"
      if(lStr.Contains("!T", StringComparison.OrdinalIgnoreCase))
        lStr = ConvertToTxtCommand(lStr);

      // Workaround for @ICO
      if(lStr.Contains("@ICO", StringComparison.OrdinalIgnoreCase))
        lStr = ConvertToIcoCommand(lStr);

      return string.Concat(STX, "COMMAND", SSX, e.Device.Port, SSX, lStr, ETX);
    }

    public string FromStringEvent(StringEventArgs e)
    {
      // NetLinx: Not implemented ...
      return string.Concat(STX, "STRING", SSX, e.Device.Port, SSX, e.Text, ETX);
    }

    public ICSPMsg DeviceOnline()
    {
      // File    : js/comm.js
      // Function: function onOpen (evt) { ...}
      // Line 165: sendCommand('UPDATE', 1, 0);

      return MsgCmdStringDevMaster.CreateRequest(Dest, new AmxDevice(Device, 1, System), "UPDATE");
    }

    public string DeviceOffline()
    {
      // WebControl: Not Implemented
      return null;
    }

    public string OnTransferFilesComplete()
    {
      // Zum Updaten: send_command dvTp_01, "'RELOAD'"
      // Der Port ist egal, muss nur als command verpackt sein.

      return string.Concat(STX, "COMMAND", SSX, 0, SSX, "RELOAD", ETX);
    }

    public async Task<ICSPMsg> ToDevMessageAsync(string msg)
    {
      /*
      WebSocket: PUSH:[Port]:[Channel];
      NetLinx  : do_push_timed(vdDevice[1].number:nPort:vdDevice[1].system, nChannel, DO_PUSH_TIMED_INFINITE)

      WebSocket: RELEASE:[Port]:[Channel];
      NetLinx  : do_release(vdDevice[1].number:nPort:vdDevice[1].system, nChannel)

      WebSocket: COMMAND:[Port]:[Command]
      NetLinx  : send_command vdDevice[1].number:nPort:vdDevice[1].system, cResponse

      WebSocket: STRING:[Port]:[String]
      NetLinx  : send_string vdDevice[1].number:nPort:vdDevice[1].system, cResponse

      WebSocket: LEVEL:[Port]:[Level]:[Value];
      NetLinx  : send_level vdDevice[1].number:nPort:vdDevice[1].system, nLevel, nValue
      */

      var lMatch = RegexMsg.Match(msg);

      if(lMatch.Success)
      {
        ushort.TryParse(lMatch.Groups[CaptureGroup_Port].Value, out var lPort);

        var lSource = new AmxDevice(Device, lPort, System);

        switch(lMatch.Groups[CaptureGroup_Type].Value.ToUpper())
        {
          case WsEvtType_Push:
          {
            if(ushort.TryParse(lMatch.Groups[CaptureGroup_Arg1].Value.TrimEnd(';'), out var lChnl))
            {
              return MsgCmdInputChannelOnStatus.CreateRequest(Dest, lSource, lChnl);
            }
            break;
          }
          case WsEvtType_Release:
          {
            if(ushort.TryParse(lMatch.Groups[CaptureGroup_Arg1].Value.TrimEnd(';'), out var lChnl))
            {
              return MsgCmdInputChannelOffStatus.CreateRequest(Dest, lSource, lChnl);
            }
            break;
          }
          case WsEvtType_Command:
          {
            return MsgCmdCommandDevMaster.CreateRequest(Dest, lSource, lMatch.Groups[CaptureGroup_Arg1].Value);
          }
          case WsEvtType_String:
          {
            return MsgCmdStringDevMaster.CreateRequest(Dest, lSource, lMatch.Groups[CaptureGroup_Arg1].Value);
          }
          case WsEvtType_Level:
          {
            if(ushort.TryParse(lMatch.Groups[CaptureGroup_Arg1].Value, out var lLevel))
            {
              if(float.TryParse(lMatch.Groups[CaptureGroup_Arg2].Value.TrimEnd(';'), out var lValue))
              {
                return MsgCmdLevelValueDevMaster.CreateRequest(Dest, lSource, lLevel, (ushort)lValue);
              }
            }

            break;
          }
          default:
          {
            return null;
          }
        }

        await Task.CompletedTask;
      }

      Logger.LogInfo($"Msg={msg}");

      lMatch = RegexKeyValue.Match(msg);

      if(lMatch.Success)
      {
        /*
        _websocket.send('PANEL_TYPE=' + project.settings.panelType + ';');
        _websocket.send('PORT_COUNT=' + project.settings.portCount + ';');
        _websocket.send('CHANNEL_COUNT=' + project.settings.channelCount + ';');
        _websocket.send('ADDRESS_COUNT=' + project.settings.addressCount + ';');
        _websocket.send('LEVEL_COUNT=' + project.settings.levelCount + ';');
        _websocket.send('UPDATE' + ';');
        */

        switch(lMatch.Groups[CaptureGroup_Key].Value.ToUpper())
        {
          case "PANEL_TYPE":
          {
            var lPanelType = lMatch.Groups[CaptureGroup_Value].Value.TrimEnd(';');

            var lPanel = Panels.GetPanelByDeviceType(lPanelType);
            
            if (lPanel == Panels.Empty)
              lPanel = Panels.GetPanelByDeviceProduct(lPanelType);

            if(lPanel != Panels.Empty)
            {
              if(string.IsNullOrWhiteSpace(Client.DeviceName))
                Client.DeviceName = lPanel.Product;

              Client.DeviceId = lPanel.DeviceId;
            }
            else
            {
              if(string.IsNullOrWhiteSpace(Client.DeviceName))
                Client.DeviceName = lPanelType;
            }
            
            // await Client.UpdateDeviceInfoAsync();

            // _ = Client.CreateDeviceInfoAsync(true);

            // Client.Manager.Disconnect(true);

            // _ = Client.Manager.ConnectAsync(Client.DeviceConfig.RemoteHost, Client.DeviceConfig.RemotePort);

            break;
          }
          case "PORT_COUNT":
          {
            if(ushort.TryParse(lMatch.Groups[CaptureGroup_Value].Value.TrimEnd(';'), out var lPortCount))
            {
              Client.DevicePortCount = lPortCount;

              // await Client.UpdateDeviceInfoAsync();
            }

            break;
          }
          case "CHANNEL_COUNT":
          {
            if(ushort.TryParse(lMatch.Groups[CaptureGroup_Value].Value.TrimEnd(';'), out var lChannelCount))
            {
              // Logger.LogDebug("lChannelCount={0}", lChannelCount);

              Client.DeviceChannelCount = lChannelCount;

              // await Client.UpdateDeviceInfoAsync();
            }

            break;
          }
          case "ADDRESS_COUNT": 
            {
              if(ushort.TryParse(lMatch.Groups[CaptureGroup_Value].Value.TrimEnd(';'), out var lAddressCount))
              {
                // Logger.LogDebug("lChannelCount={0}", lChannelCount);

                Client.AddressCount = lAddressCount;

                // await Client.UpdateDeviceInfoAsync();
              }

              break;
            }

          case "LEVEL_COUNT":
          {
            if(ushort.TryParse(lMatch.Groups[CaptureGroup_Value].Value.TrimEnd(';'), out var lLevelCount))
            {
              // Logger.LogDebug("lChannelCount={0}", lChannelCount);

              Client.DeviceLevelCount = lLevelCount;

              // await Client.UpdateDeviceInfoAsync();
            }

            break;
          }
        }
      }

      return null;
    }

    public string ConvertToTxtCommand(string msg)
    {
      // !T[Chnl]... -> ^TXT-Chnl,0,...
      return string.Concat("^TXT-", msg[2].ToString(), ",0,", msg.Substring(3));
    }

    public string ConvertToIcoCommand(string msg)
    {
      // @ICO[Chnl]... -> ^ICO-Chnl,0,...
      return string.Concat("^ICO-", msg[4].ToString(), ",0,", msg[5].ToString());
    }

    /*
    define_function char[100] ConvertToTXT(char cMsg[])
        {
        stack_var char cEncoded[100]
        stack_var integer i

        cEncoded = '^TXT-'
        cEncoded = "cEncoded, itoa(cMsg[3]), ',0,'"

        for (i = 4; i <= length_string(cMsg); i++)
            cEncoded = "cEncoded, cMsg[i]"

        return cEncoded;
        }

    define_function char[100] ConvertICOCommand(char cMsg[])
        {
        stack_var char cEncoded[100]
        stack_var integer i

        cEncoded = '^ICO-'
        cEncoded = "cEncoded, itoa(cMsg[5]), ',0,', itoa(cMsg[6])"

        return cEncoded
        }
    */
  }
}
