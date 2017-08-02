namespace ICSP.Constants
{
  /// <summary>
  /// Device Manager Messages (Table A)
  /// </summary>
  public static class DeviceManagerCmd
  {
    public const int Ack = 0x0001;                        // 1
    public const int Nak = 0x0002;                        // 2

    public const int InputChannelOnStatus = 0x0084;       // 132 Push
    public const int InputChannelOffStatus = 0x0085;      // 133 Release

    public const int OutputChannelOn = 0x0006;            // 6
    public const int OutputChannelOnStatus = 0x0086;      // 134

    public const int OutputChannelOff = 0x0007;           // 7
    public const int OutputChannelOffStatus = 0x0087;     // 135

    public const int InputOutputChannelOn = 0x0088;       // 136
    public const int InputOutputChannelOff = 0x0089;      // 137

    public const int FeedbackChannelOn = 0x0018;          // 24
    public const int FeedbackChannelOff = 0x0019;         // 25

    public const int LevelValueMasterDev = 0x000A;        // 10
    public const int LevelValueDevMaster = 0x008A;        // 138

    public const int StringMasterDev = 0x000B;            // 11
    public const int StringDevMaster = 0x008B;            // 139

    public const int CommandMasterDev = 0x000C;           // 12
    public const int CommandDevMaster = 0x008C;           // 140

    public const int RequestLevelValue = 0x000E;          // 14
    public const int RequestOutputChannelStatus = 0x000F; // 15 ??
    public const int RequestPortCount = 0x0010;           // 16
    public const int PortCountBy = 0x0090;                // 144

    public const int RequestOutputChannelCount = 0x0011;  // 17
    public const int OutputChannelCount = 0x0091;         // 145

    public const int RequestLevelCount = 0x0012;          // 18
    public const int LevelCount = 0x0092;                 // 146

    public const int RequestStringSize = 0x0013;          // 19
    public const int StringSize = 0x0093;                 // 147

    public const int RequestCommandSize = 0x0014;         // 20
    public const int CommandSize = 0x0094;                // 148

    public const int RequestLevelSize = 0x0015;           // 21
    public const int LevelSize = 0x0095;                  // 149

    public const int RequestStatus = 0x0016;              // 22
    public const int Status = 0x0096;                     // 150

    public const int RequestDeviceInfo = 0x0017;          // 23
    public const int DeviceInfo = 0x0097;                 // 151
    public const int DeviceInfoEOT = 0x0098;              // 152

    public const int RequestMasterStatus = 0x00A1;        // 161
    public const int MasterStatus = 0x0021;               // 33
  }
}