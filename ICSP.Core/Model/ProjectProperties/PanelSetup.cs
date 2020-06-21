using System.Collections.Generic;

using Newtonsoft.Json;

namespace ICSP.Core.Model.ProjectProperties
{
  public class PanelSetup
  {
    [JsonProperty("portCount", Order = 1)]
    public int PortCount { get; set; }

    [JsonProperty("setupPort", Order = 2)]
    public int SetupPort { get; set; }

    [JsonProperty("addressCount", Order = 3)]
    public int AddressCount { get; set; }

    [JsonProperty("channelCount", Order = 4)]
    public int ChannelCount { get; set; }

    [JsonProperty("levelCount", Order = 5)]
    public int LevelCount { get; set; }

    [JsonProperty("powerUpPage", Order = 6)]
    public string PowerUpPage { get; set; }

    [JsonProperty("powerUpPopup", Order = 7)]
    public List<string> PowerUpPopup { get; set; }

    [JsonProperty("feedbackBlinkRate", Order = 8)]
    public int FeedbackBlinkRate { get; set; }

    [JsonProperty("startupString", Order = 9)]
    public string StartupString { get; set; }

    [JsonProperty("wakeupString", Order = 10)]
    public string WakeupString { get; set; }

    [JsonProperty("sleepString", Order = 11)]
    public string SleepString { get; set; }

    [JsonProperty("standbyString", Order = 12)]
    public string StandbyString { get; set; }

    [JsonProperty("shutdownString", Order = 13)]
    public string ShutdownString { get; set; }

    [JsonProperty("idlePage", Order = 14)]
    public string IdlePage { get; set; }

    [JsonProperty("idleTimeout", Order = 15)]
    public int IdleTimeout { get; set; }

    [JsonProperty("extButtonsKey", Order = 16)]
    public string ExtButtonsKey { get; set; }

    [JsonProperty("screenWidth", Order = 17)]
    public int ScreenWidth { get; set; }

    [JsonProperty("screenHeight", Order = 18)]
    public int ScreenHeight { get; set; }

    [JsonProperty("screenRefresh", Order = 19)]
    public int ScreenRefresh { get; set; }

    [JsonProperty("screenRotate", Order = 20)]
    public int ScreenRotate { get; set; }

    [JsonProperty("screenDescription", Order = 21)]
    public string ScreenDescription { get; set; }

    [JsonProperty("pageTracking", Order = 22)]
    public int PageTracking { get; set; }

    [JsonProperty("cursor", Order = 23)]
    public int Cursor { get; set; }

    [JsonProperty("brightness", Order = 24)]
    public int Brightness { get; set; }

    [JsonProperty("lightSensorLevelPort", Order = 25)]
    public int LightSensorLevelPort { get; set; }

    [JsonProperty("lightSensorLevelCode", Order = 26)]
    public int LightSensorLevelCode { get; set; }

    [JsonProperty("lightSensorChannelPort", Order = 27)]
    public int LightSensorChannelPort { get; set; }

    [JsonProperty("lightSensorChannelCode", Order = 28)]
    public int LightSensorChannelCode { get; set; }    

    [JsonProperty("motionSensorChannelPort", Order = 29)]
    public int MotionSensorChannelPort { get; set; }

    [JsonProperty("motionSensorChannelCode", Order = 30)]
    public int MotionSensorChannelCode { get; set; }

    [JsonProperty("batteryLevelPort", Order = 31)]
    public int BatteryLevelPort { get; set; }

    [JsonProperty("batteryLevelCode", Order = 32)]
    public int BatteryLevelCode { get; set; }

    [JsonProperty("irPortAMX38Emit", Order = 33)]
    public int IrPortAMX38Emit { get; set; }

    [JsonProperty("irPortAMX455Emit", Order = 34)]
    public int IrPortAMX455Emit { get; set; }

    [JsonProperty("irPortAMX38Recv", Order = 35)]
    public int IrPortAMX38Recv { get; set; }

    [JsonProperty("irPortAMX455Recv", Order = 36)]
    public int IrPortAMX455Recv { get; set; }

    [JsonProperty("irPortUser1", Order = 37)]
    public int IrPortUser1 { get; set; }

    [JsonProperty("irPortUser2", Order = 38)]
    public int IrPortUser2 { get; set; }

    [JsonProperty("cradleChannelPort", Order = 39)]
    public int CradleChannelPort { get; set; }

    [JsonProperty("cradleChannelCode", Order = 40)]
    public int CradleChannelCode { get; set; }

    [JsonProperty("uniqueID", Order = 41)]
    public string UniqueID { get; set; }

    [JsonProperty("appCreated", Order = 42)]
    public string AppCreated { get; set; }

    [JsonProperty("buildNumber", Order = 43)]
    public int BuildNumber { get; set; }

    [JsonProperty("appModified", Order = 44)]
    public string AppModified { get; set; }

    [JsonProperty("buildNumberMod", Order = 45)]
    public int BuildNumberMod { get; set; }

    [JsonProperty("buildStatusMod", Order = 46)]
    public string BuildStatusMod { get; set; }

    [JsonProperty("activePalette", Order = 47)]
    public int ActivePalette { get; set; }

    [JsonProperty("marqueeSpeed", Order = 48)]
    public int MarqueeSpeed { get; set; }

    [JsonProperty("setupPagesProject", Order = 49)]
    public int SetupPagesProject { get; set; }

    [JsonProperty("voipCommandPort", Order = 50)]
    public int VoipCommandPort { get; set; }
  }
}