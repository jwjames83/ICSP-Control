using System;
using System.Text;
using System.Text.RegularExpressions;

using ICSP.Core;
using ICSP.Core.Manager.DeviceManager;

namespace ICSP.WebProxy.Converter
{
  /// <summary>
  /// Implementation for AMX Module WebControl Version 2.7.1
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

    private static Regex RegexMsg = new Regex($"^(?<{CaptureGroup_Type}>[^:]+):(?<{CaptureGroup_Port}>[^:]+):(?<{CaptureGroup_Arg1}>[^:]+):?(?<{CaptureGroup_Arg2}>[^;])?");

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

    /// <summary>
    ///  AMX Module WebControl Version 2.5.5 does not encode UTF8<br/>
    ///  AMX Module WebControl Version 2.6.0 does encode UTF8 by default self
    /// </summary>
    public bool EncodeUTF8 { get; set; } = true;

    public ushort Device { get; set; }

    public ushort System { get; set; }

    public AmxDevice Dest { get; set; }

    public string FromChannelEvent(ChannelEventArgs e)
    {
      // NetLinx:
      // SendToWebSocket(WS_OpCode_TextFrame, "'ON', SSX, itoa(nPort), SSX, itoa(nChannel)")
      // SendToWebSocket(WS_OpCode_TextFrame, "'OFF', SSX, itoa(nPort), SSX, itoa(nChannel)")

      if(e.Enabled)
        return string.Concat("ON", SSX, e.Device.Port, SSX, e.Channel);
      else
        return string.Concat("OFF", SSX, e.Device.Port, SSX, e.Channel);
    }

    public string FromLevelEvent(LevelEventArgs e)
    {
      // NetLinx:
      // SendToWebSocket(WS_OpCode_TextFrame, "'LEVEL', SSX, itoa(nPort), SSX, itoa(level.input.level), SSX, format('%d', level.value)")

      return string.Concat("LEVEL", SSX, e.Device.Port, SSX, e.Level, SSX, e.Level);
    }

    public string FromCommandEvent(CommandEventArgs e)
    {
      // NetLinx:
      // SendToWebSocket(WS_OpCode_TextFrame, "'COMMAND', SSX, itoa(Data.Device.Port), SSX, cStr")

      // Convert from UTF8 to default ...
      var lStr = EncodeUTF8 ? Encoding.Default.GetString(Encoding.UTF8.GetBytes(e.Text)) : e.Text;

      // Workaround for "!T"
      if(lStr.Contains("!T", StringComparison.OrdinalIgnoreCase))
        lStr = ConvertToTxtCommand(lStr);

      // Workaround for @ICO
      if(lStr.Contains("@ICO", StringComparison.OrdinalIgnoreCase))
        lStr = ConvertToIcoCommand(lStr);

      return string.Concat("COMMAND", SSX, e.Device.Port, SSX, lStr);
    }

    public string FromStringEvent(StringEventArgs e)
    {
      // NetLinx:
      // Not implemented ...

      return string.Concat("STRING", SSX, e.Device.Port, SSX, e.Text);
    }

    public ICSPMsg ToDevMessage(string msg)
    {
      // TODO (Parsing) ...
      /*
      send_string vdDevice[1], "itoa(vdDevice[1].Number), ':OFFLINE:', cClientIp"
      send_string vdDevice[1], "itoa(vdDevice[1].number), ':ONLINE:', cClientIp"
      
      http://regexstorm.net/tester
      ^(?<type>[^:]+):(?<port>[^:]+):(?<arg1>[^:;]+):?(?<arg2>[^;])?

      PUSH:[Port]:[Channel];
      do_push_timed(vdDevice[1].number:nPort:vdDevice[1].system, nChannel, DO_PUSH_TIMED_INFINITE)

      RELEASE:[Port]:[Channel];
      do_release(vdDevice[1].number:nPort:vdDevice[1].system, nChannel)

      COMMAND:[Port]:[Command]
      send_command vdDevice[1].number:nPort:vdDevice[1].system,"cResponse"

      STRING:[Port]:[String]
      send_string vdDevice[1].number:nPort:vdDevice[1].system,"cResponse"

      LEVEL:[Port]:[Level]:[Value];
      send_level vdDevice[1].number:nPort:vdDevice[1].system, nLevel, nValue
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
              Console.WriteLine("[Push]: Port={0}, Channel={1}", lPort, lChnl);
              
              return MsgCmdInputChannelOnStatus.CreateRequest(Dest, lSource, lChnl);
            }
            break;
          }
          case WsEvtType_Release:
          {
            if(ushort.TryParse(lMatch.Groups[CaptureGroup_Arg1].Value.TrimEnd(';'), out var lChnl))
            {
              Console.WriteLine("[Release]: Port={0}, Channel={1}", lPort, lChnl);

              return MsgCmdInputChannelOffStatus.CreateRequest(Dest, lSource, lChnl);
            }
            break;
          }
          case WsEvtType_Command:
          {
            Console.WriteLine("[Command]: Port={0}, Command={1}", lPort, lMatch.Groups[CaptureGroup_Arg1].Value);

            return MsgCmdCommandDevMaster.CreateRequest(lSource, Dest, lMatch.Groups[CaptureGroup_Arg1].Value);
          }
          case WsEvtType_String:
          {
            Console.WriteLine("[String]: Port={0}, String={1}", lPort, lMatch.Groups[CaptureGroup_Arg1].Value);

            return MsgCmdStringDevMaster.CreateRequest(lSource, Dest, lMatch.Groups[CaptureGroup_Arg1].Value);
          }
          case WsEvtType_Level:
          {
            if(ushort.TryParse(lMatch.Groups[CaptureGroup_Arg1].Value, out var lLevel))
            {
              if(float.TryParse(lMatch.Groups[CaptureGroup_Arg2].Value.TrimEnd(';'), out var lValue))
              {
                Console.WriteLine("[Level]: Port={0}, Level={1}, Value={2}", lPort, lLevel, lValue);

                return MsgCmdLevelValueDevMaster.CreateRequest(lSource, Dest, lLevel, (ushort)lValue);
              }
            }

            break;
          }
          default:
          {
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
