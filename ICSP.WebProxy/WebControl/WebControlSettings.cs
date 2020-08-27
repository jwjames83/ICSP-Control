using System.Collections.Generic;
using System.Linq;

using ICSP.Core.Model.ProjectProperties;

using Newtonsoft.Json;

namespace ICSP.WebProxy.WebControl
{
  public class WebControlSettings
  {
    public WebControlSettings()
    {
      PageList = new List<WebControlPageEntry>();

      PaletteList = new List<Palette>();

      ResourceList = new Dictionary<string, WebControlResource>();

      IconList = new Dictionary<int, IconItem>();
    }

    // ==============================================================================================
    // 5: List
    // ==============================================================================================

    [JsonProperty("pageList", Order = 1)]
    public List<WebControlPageEntry> PageList { get; set; }

    [JsonProperty("paletteList", Order = 2)]
    public List<Palette> PaletteList { get; set; }

    [JsonProperty("resourceList", Order = 3)]
    public Dictionary<string, WebControlResource> ResourceList { get; set; }

    [JsonProperty("iconList", Order = 4)]
    public Dictionary<int, IconItem> IconList { get; set; }

    // ==============================================================================================
    // 4: PanelSetup
    // ==============================================================================================

    [JsonProperty("portCount", Order = 5)]
    public int PortCount { get; set; }

    [JsonProperty("setupPort", Order = 6)]
    public int SetupPort { get; set; }

    [JsonProperty("addressCount", Order = 7)]
    public int AddressCount { get; set; }

    [JsonProperty("channelCount", Order = 8)]
    public int ChannelCount { get; set; }

    [JsonProperty("levelCount", Order = 9)]
    public int LevelCount { get; set; }

    [JsonProperty("powerUpPage", Order = 10)]
    public string PowerUpPage { get; set; }

#warning TODO: Correct Order: 11
    [JsonProperty("powerUpPopup", Order = 4)]
    public List<string> PowerUpPopup { get; set; }

    [JsonProperty("feedbackBlinkRate", Order = 12)]
    public int FeedbackBlinkRate { get; set; }

    [JsonProperty("startupString", Order = 13)]
    public string StartupString { get; set; }

    [JsonProperty("wakeupString", Order = 14)]
    public string WakeupString { get; set; }

    [JsonProperty("sleepString", Order = 15)]
    public string SleepString { get; set; }

    [JsonProperty("standbyString", Order = 16)]
    public string StandbyString { get; set; }

    [JsonProperty("shutdownString", Order = 17)]
    public string ShutdownString { get; set; }

    [JsonProperty("idlePage", Order = 18)]
    public string IdlePage { get; set; }

    [JsonProperty("idleTimeout", Order = 19)]
    public int IdleTimeout { get; set; }

    [JsonProperty("extButtonsKey", Order = 20)]
    public string ExtButtonsKey { get; set; }

    [JsonProperty("screenWidth", Order = 21)]
    public int ScreenWidth { get; set; }

    [JsonProperty("screenHeight", Order = 22)]
    public int ScreenHeight { get; set; }

    [JsonProperty("screenRefresh", Order = 23)]
    public int ScreenRefresh { get; set; }

    [JsonProperty("screenRotate", Order = 24)]
    public int ScreenRotate { get; set; }

    [JsonProperty("screenDescription", Order = 25)]
    public string ScreenDescription { get; set; }

    [JsonProperty("pageTracking", Order = 26)]
    public int PageTracking { get; set; }

    [JsonProperty("cursor", Order = 27)]
    public int Cursor { get; set; }

    [JsonProperty("brightness", Order = 28)]
    public int Brightness { get; set; }

    [JsonProperty("lightSensorLevelPort", Order = 29)]
    public int LightSensorLevelPort { get; set; }

    [JsonProperty("lightSensorLevelCode", Order = 30)]
    public int LightSensorLevelCode { get; set; }

    [JsonProperty("lightSensorChannelPort", Order = 31)]
    public int LightSensorChannelPort { get; set; }

    [JsonProperty("lightSensorChannelCode", Order = 32)]
    public int LightSensorChannelCode { get; set; }

    [JsonProperty("motionSensorChannelPort", Order = 33)]
    public int MotionSensorChannelPort { get; set; }

    [JsonProperty("motionSensorChannelCode", Order = 34)]
    public int MotionSensorChannelCode { get; set; }

    [JsonProperty("batteryLevelPort", Order = 35)]
    public int BatteryLevelPort { get; set; }

    [JsonProperty("batteryLevelCode", Order = 36)]
    public int BatteryLevelCode { get; set; }

    [JsonProperty("irPortAMX38Emit", Order = 37)]
    public int IrPortAMX38Emit { get; set; }

    [JsonProperty("irPortAMX455Emit", Order = 38)]
    public int IrPortAMX455Emit { get; set; }

    [JsonProperty("irPortAMX38Recv", Order = 39)]
    public int IrPortAMX38Recv { get; set; }

    [JsonProperty("irPortAMX455Recv", Order = 40)]
    public int IrPortAMX455Recv { get; set; }

    [JsonProperty("irPortUser1", Order = 41)]
    public int IrPortUser1 { get; set; }

    [JsonProperty("irPortUser2", Order = 42)]
    public int IrPortUser2 { get; set; }

    [JsonProperty("cradleChannelPort", Order = 43)]
    public int CradleChannelPort { get; set; }

    [JsonProperty("cradleChannelCode", Order = 44)]
    public int CradleChannelCode { get; set; }

    [JsonProperty("uniqueID", Order = 45)]
    public string UniqueID { get; set; }

    [JsonProperty("appCreated", Order = 46)]
    public string AppCreated { get; set; }

    [JsonProperty("buildNumber", Order = 47)]
    public int BuildNumber { get; set; }

    [JsonProperty("appModified", Order = 48)]
    public string AppModified { get; set; }

    [JsonProperty("buildNumberMod", Order = 49)]
    public int BuildNumberMod { get; set; }

    [JsonProperty("buildStatusMod", Order = 50)]
    public string BuildStatusMod { get; set; }

    [JsonProperty("activePalette", Order = 51)]
    public int ActivePalette { get; set; }

    [JsonProperty("marqueeSpeed", Order = 52)]
    public int MarqueeSpeed { get; set; }

    [JsonProperty("setupPagesProject", Order = 53)]
    public int SetupPagesProject { get; set; }

    [JsonProperty("voipCommandPort", Order = 54)]
    public int VoipCommandPort { get; set; }

    // ==============================================================================================
    // 3: SupportFileList
    // ==============================================================================================

    [JsonProperty("mapFile", Order = 55)]
    public string MapFile { get; set; }

    [JsonProperty("colorFile", Order = 56)]
    public string ColorFile { get; set; }

    [JsonProperty("fontFile", Order = 57)]
    public string FontFile { get; set; }

    [JsonProperty("themeFile", Order = 58)]
    public string ThemeFile { get; set; }

    [JsonProperty("iconFile", Order = 59)]
    public string IconFile { get; set; }

    [JsonProperty("externalButtonFile", Order = 60)]
    public string ExternalButtonFile { get; set; }

    [JsonProperty("appFile", Order = 61, NullValueHandling = NullValueHandling.Ignore)] // G5
    public string AppFile { get; set; }

    [JsonProperty("logFile", Order = 62, NullValueHandling = NullValueHandling.Ignore)] // G5
    public string LogFile { get; set; }

    // ==============================================================================================
    // 2: ProjectInfo
    // ==============================================================================================

    [JsonProperty("protection", Order = 63)]
    public ProtectionType Protection { get; set; }

    [JsonProperty("password", Order = 64)]
    public string Password { get; set; }

    [JsonProperty("panelType", Order = 65)]
    public string PanelType { get; set; }

    [JsonProperty("fileRevision", Order = 66)]
    public string FileRevision { get; set; }

    [JsonProperty("userProfile", Order = 67, NullValueHandling = NullValueHandling.Ignore)] // G5
    public string UserProfile { get; set; }

    [JsonProperty("dealerId", Order = 68)]
    public string DealerId { get; set; }

    [JsonProperty("jobName", Order = 69)]
    public string JobName { get; set; }

    [JsonProperty("salesOrder", Order = 70)]
    public string SalesOrder { get; set; }

    [JsonProperty("purchaseOrder", Order = 71)]
    public string PurchaseOrder { get; set; }

    [JsonProperty("jobComment", Order = 72)]
    public string JobComment { get; set; }

    [JsonProperty("designerId", Order = 73)]
    public string DesignerId { get; set; }

    [JsonProperty("creationDate", Order = 74)]
    public string CreationDate { get; set; }

    [JsonProperty("revisionDate", Order = 75)]
    public string RevisionDate { get; set; }

    [JsonProperty("lastSaveDate", Order = 76)]
    public string LastSaveDate { get; set; }

    [JsonProperty("fileName", Order = 77)]
    public string FileName { get; set; }

    [JsonProperty("convLog", Order = 78, NullValueHandling = NullValueHandling.Ignore)] // G5
    public string ConvLog { get; set; }

    [JsonProperty("colorChoice", Order = 79)]
    public string ColorChoice { get; set; }

    [JsonProperty("specifyPortCount", Order = 80)]
    public int SpecifyPortCount { get; set; }

    [JsonProperty("specifyChanCount", Order = 81)]
    public int SpecifyChanCount { get; set; }

    // ==============================================================================================
    // 1: VersionInfo
    // ==============================================================================================

    [JsonProperty("formatVersion", Order = 81, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int FormatVersion { get; set; }

    [JsonProperty("graphicsVersion", Order = 83, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int GraphicsVersion { get; set; }

    [JsonProperty("g5appsVersion", Order = 84, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int G5AppsVersion { get; set; }

    [JsonProperty("fileVersion", Order = 85, NullValueHandling = NullValueHandling.Ignore)]
    public int FileVersion { get; set; }

    [JsonProperty("designVersion", Order = 86, NullValueHandling = NullValueHandling.Ignore)]
    public int DesignVersion { get; set; }

    public static implicit operator WebControlSettings(Project project)
    {
      return new WebControlSettings()
      {
        // ==============================================================================================
        // PageList
        // ==============================================================================================

        PageList                /**/ = project.PageList?.Select(s => (WebControlPageEntry)s)?.ToList() ?? new List<WebControlPageEntry>(),
        PaletteList             /**/ = project.PaletteList ?? new List<Palette>(),
        ResourceList            /**/ = project.ResourceList?.ToDictionary(k => k.Name, s => (WebControlResource)s) ?? new Dictionary<string, WebControlResource>(),
        // IconList             /**/ = project.IconList,

        // ==============================================================================================
        // VersionInfo
        // ==============================================================================================
        FormatVersion           /**/ = project.VersionInfo.FormatVersion,
        GraphicsVersion         /**/ = project.VersionInfo.GraphicsVersion,
        G5AppsVersion           /**/ = project.VersionInfo.G5AppsVersion,
        FileVersion             /**/ = project.VersionInfo.FileVersion,
        DesignVersion           /**/ = project.VersionInfo.DesignVersion,

        // ==============================================================================================
        // ProjectInfo
        // ==============================================================================================

        Protection              /**/ = project.ProjectInfo.Protection,
        Password                /**/ = project.ProjectInfo.Password,
        PanelType               /**/ = project.ProjectInfo.PanelType,
        FileRevision            /**/ = project.ProjectInfo.FileRevision,
        UserProfile             /**/ = project.ProjectInfo.UserProfile,
        DealerId                /**/ = project.ProjectInfo.DealerId,
        JobName                 /**/ = project.ProjectInfo.JobName,
        SalesOrder              /**/ = project.ProjectInfo.SalesOrder,
        PurchaseOrder           /**/ = project.ProjectInfo.PurchaseOrder,
        JobComment              /**/ = project.ProjectInfo.JobComment,
        DesignerId              /**/ = project.ProjectInfo.DesignerId,
        CreationDate            /**/ = project.ProjectInfo.CreationDate,
        RevisionDate            /**/ = project.ProjectInfo.RevisionDate,
        LastSaveDate            /**/ = project.ProjectInfo.LastSaveDate,
        FileName                /**/ = project.ProjectInfo.FileName,
        ConvLog                 /**/ = project.ProjectInfo.ConvLog,
        ColorChoice             /**/ = project.ProjectInfo.ColorChoice,
        SpecifyPortCount        /**/ = project.ProjectInfo.SpecifyPortCount,
        SpecifyChanCount        /**/ = project.ProjectInfo.SpecifyChanCount,

        // ==============================================================================================
        // SupportFileList
        // ==============================================================================================
        MapFile                 /**/ = project.SupportFileList.MapFile,
        ColorFile               /**/ = project.SupportFileList.ColorFile,
        FontFile                /**/ = project.SupportFileList.FontFile,
        ThemeFile               /**/ = project.SupportFileList.ThemeFile,
        IconFile                /**/ = project.SupportFileList.IconFile,
        ExternalButtonFile      /**/ = project.SupportFileList.ExternalButtonFile,
        AppFile                 /**/ = project.SupportFileList.AppFile,
        LogFile                 /**/ = project.SupportFileList.LogFile,

        // ==============================================================================================
        // PanelSetup
        // ==============================================================================================
        PortCount               /**/ = project.PanelSetup.PortCount,
        SetupPort               /**/ = project.PanelSetup.SetupPort,
        AddressCount            /**/ = project.PanelSetup.AddressCount,
        ChannelCount            /**/ = project.PanelSetup.ChannelCount,
        LevelCount              /**/ = project.PanelSetup.LevelCount,
        PowerUpPage             /**/ = project.PanelSetup.PowerUpPage,
        PowerUpPopup            /**/ = project.PanelSetup.PowerUpPopup ?? new List<string>(),
        FeedbackBlinkRate       /**/ = project.PanelSetup.FeedbackBlinkRate,
        StartupString           /**/ = project.PanelSetup.StartupString,
        WakeupString            /**/ = project.PanelSetup.WakeupString,
        SleepString             /**/ = project.PanelSetup.SleepString,
        StandbyString           /**/ = project.PanelSetup.StandbyString,
        ShutdownString          /**/ = project.PanelSetup.ShutdownString,
        IdlePage                /**/ = project.PanelSetup.IdlePage,
        IdleTimeout             /**/ = project.PanelSetup.IdleTimeout,
        ExtButtonsKey           /**/ = project.PanelSetup.ExtButtonsKey,
        ScreenWidth             /**/ = project.PanelSetup.ScreenWidth,
        ScreenHeight            /**/ = project.PanelSetup.ScreenHeight,
        ScreenRefresh           /**/ = project.PanelSetup.ScreenRefresh,
        ScreenRotate            /**/ = project.PanelSetup.ScreenRotate,
        ScreenDescription       /**/ = project.PanelSetup.ScreenDescription,
        PageTracking            /**/ = project.PanelSetup.PageTracking,
        Cursor                  /**/ = project.PanelSetup.Cursor,
        Brightness              /**/ = project.PanelSetup.Brightness,
        LightSensorLevelPort    /**/ = project.PanelSetup.LightSensorLevelPort,
        LightSensorLevelCode    /**/ = project.PanelSetup.LightSensorLevelCode,
        LightSensorChannelPort  /**/ = project.PanelSetup.LightSensorChannelPort,
        LightSensorChannelCode  /**/ = project.PanelSetup.LightSensorChannelCode,
        MotionSensorChannelPort /**/ = project.PanelSetup.MotionSensorChannelPort,
        MotionSensorChannelCode /**/ = project.PanelSetup.MotionSensorChannelCode,
        BatteryLevelPort        /**/ = project.PanelSetup.BatteryLevelPort,
        BatteryLevelCode        /**/ = project.PanelSetup.BatteryLevelCode,
        IrPortAMX38Emit         /**/ = project.PanelSetup.IrPortAMX38Emit,
        IrPortAMX455Emit        /**/ = project.PanelSetup.IrPortAMX455Emit,
        IrPortAMX38Recv         /**/ = project.PanelSetup.IrPortAMX38Recv,
        IrPortAMX455Recv        /**/ = project.PanelSetup.IrPortAMX455Recv,
        IrPortUser1             /**/ = project.PanelSetup.IrPortUser1,
        IrPortUser2             /**/ = project.PanelSetup.IrPortUser2,
        CradleChannelPort       /**/ = project.PanelSetup.CradleChannelPort,
        CradleChannelCode       /**/ = project.PanelSetup.CradleChannelCode,
        UniqueID                /**/ = project.PanelSetup.UniqueID,
        AppCreated              /**/ = project.PanelSetup.AppCreated,
        BuildNumber             /**/ = project.PanelSetup.BuildNumber,
        AppModified             /**/ = project.PanelSetup.AppModified,
        BuildNumberMod          /**/ = project.PanelSetup.BuildNumberMod,
        BuildStatusMod          /**/ = project.PanelSetup.BuildStatusMod,
        ActivePalette           /**/ = project.PanelSetup.ActivePalette,
        MarqueeSpeed            /**/ = project.PanelSetup.MarqueeSpeed,
        SetupPagesProject       /**/ = project.PanelSetup.SetupPagesProject,
        VoipCommandPort         /**/ = project.PanelSetup.VoipCommandPort,
      };
    }

    public static explicit operator Project(WebControlSettings settings)
    {
      return new Project()
      {
        // ==============================================================================================
        // PageList
        // ==============================================================================================

        PageList                /**/ = settings.PageList?.Select(s => (PageEntry)s)?.ToList(),
        PaletteList             /**/ = settings.PaletteList,
        ResourceList            /**/ = settings.ResourceList?.Values?.Select(s => (Resource)s).ToList(),

        // ==============================================================================================
        // VersionInfo
        // ==============================================================================================

        VersionInfo = new VersionInfo
        {
          FormatVersion           /**/ = settings.FormatVersion,
          GraphicsVersion         /**/ = settings.GraphicsVersion,
          G5AppsVersion           /**/ = settings.G5AppsVersion,
          FileVersion             /**/ = settings.FileVersion,
          DesignVersion           /**/ = settings.DesignVersion,
        },

        // ==============================================================================================
        // ProjectInfo
        // ==============================================================================================

        ProjectInfo = new ProjectInfo
        {
          Protection              /**/ = settings.Protection,
          Password                /**/ = settings.Password,
          PanelType               /**/ = settings.PanelType,
          FileRevision            /**/ = settings.FileRevision,
          UserProfile             /**/ = settings.UserProfile,
          DealerId                /**/ = settings.DealerId,
          JobName                 /**/ = settings.JobName,
          SalesOrder              /**/ = settings.SalesOrder,
          PurchaseOrder           /**/ = settings.PurchaseOrder,
          JobComment              /**/ = settings.JobComment,
          DesignerId              /**/ = settings.DesignerId,
          CreationDate            /**/ = settings.CreationDate,
          RevisionDate            /**/ = settings.RevisionDate,
          LastSaveDate            /**/ = settings.LastSaveDate,
          FileName                /**/ = settings.FileName,
          ConvLog                 /**/ = settings.ConvLog,
          ColorChoice             /**/ = settings.ColorChoice,
          SpecifyPortCount        /**/ = settings.SpecifyPortCount,
          SpecifyChanCount        /**/ = settings.SpecifyChanCount,
        },

        // ==============================================================================================
        // SupportFileList
        // ==============================================================================================

        SupportFileList = new SupportFileList
        {
          MapFile                 /**/ = settings.MapFile,
          ColorFile               /**/ = settings.ColorFile,
          FontFile                /**/ = settings.FontFile,
          ThemeFile               /**/ = settings.ThemeFile,
          IconFile                /**/ = settings.IconFile,
          ExternalButtonFile      /**/ = settings.ExternalButtonFile,
          AppFile                 /**/ = settings.AppFile,
          LogFile                 /**/ = settings.LogFile,
        },

        // ==============================================================================================
        // PanelSetup
        // ==============================================================================================

        PanelSetup = new PanelSetup
        {
          PortCount               /**/ = settings.PortCount,
          SetupPort               /**/ = settings.SetupPort,
          AddressCount            /**/ = settings.AddressCount,
          ChannelCount            /**/ = settings.ChannelCount,
          LevelCount              /**/ = settings.LevelCount,
          PowerUpPage             /**/ = settings.PowerUpPage,
          PowerUpPopup            /**/ = settings.PowerUpPopup,
          FeedbackBlinkRate       /**/ = settings.FeedbackBlinkRate,
          StartupString           /**/ = settings.StartupString,
          WakeupString            /**/ = settings.WakeupString,
          SleepString             /**/ = settings.SleepString,
          StandbyString           /**/ = settings.StandbyString,
          ShutdownString          /**/ = settings.ShutdownString,
          IdlePage                /**/ = settings.IdlePage,
          IdleTimeout             /**/ = settings.IdleTimeout,
          ExtButtonsKey           /**/ = settings.ExtButtonsKey,
          ScreenWidth             /**/ = settings.ScreenWidth,
          ScreenHeight            /**/ = settings.ScreenHeight,
          ScreenRefresh           /**/ = settings.ScreenRefresh,
          ScreenRotate            /**/ = settings.ScreenRotate,
          ScreenDescription       /**/ = settings.ScreenDescription,
          PageTracking            /**/ = settings.PageTracking,
          Cursor                  /**/ = settings.Cursor,
          Brightness              /**/ = settings.Brightness,
          LightSensorLevelPort    /**/ = settings.LightSensorLevelPort,
          LightSensorLevelCode    /**/ = settings.LightSensorLevelCode,
          LightSensorChannelPort  /**/ = settings.LightSensorChannelPort,
          LightSensorChannelCode  /**/ = settings.LightSensorChannelCode,
          MotionSensorChannelPort /**/ = settings.MotionSensorChannelPort,
          MotionSensorChannelCode /**/ = settings.MotionSensorChannelCode,
          BatteryLevelPort        /**/ = settings.BatteryLevelPort,
          BatteryLevelCode        /**/ = settings.BatteryLevelCode,
          IrPortAMX38Emit         /**/ = settings.IrPortAMX38Emit,
          IrPortAMX455Emit        /**/ = settings.IrPortAMX455Emit,
          IrPortAMX38Recv         /**/ = settings.IrPortAMX38Recv,
          IrPortAMX455Recv        /**/ = settings.IrPortAMX455Recv,
          IrPortUser1             /**/ = settings.IrPortUser1,
          IrPortUser2             /**/ = settings.IrPortUser2,
          CradleChannelPort       /**/ = settings.CradleChannelPort,
          CradleChannelCode       /**/ = settings.CradleChannelCode,
          UniqueID                /**/ = settings.UniqueID,
          AppCreated              /**/ = settings.AppCreated,
          BuildNumber             /**/ = settings.BuildNumber,
          AppModified             /**/ = settings.AppModified,
          BuildNumberMod          /**/ = settings.BuildNumberMod,
          BuildStatusMod          /**/ = settings.BuildStatusMod,
          ActivePalette           /**/ = settings.ActivePalette,
          MarqueeSpeed            /**/ = settings.MarqueeSpeed,
          SetupPagesProject       /**/ = settings.SetupPagesProject,
          VoipCommandPort         /**/ = settings.VoipCommandPort,
        },
      };
    }
  }
}