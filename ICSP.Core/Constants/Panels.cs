using System.Collections.Generic;
using System.Linq;

namespace ICSP.Core.Constants
{
  public static class Panels
  {
    public static PanelType Empty = new PanelType();
    
    // =================================================================================
    // G4 Panels (From PPF.xml)
    // =================================================================================

    public static PanelType G4_MVP_5150                /**/ = new PanelType() { DeviceId = 333, DeviceType = "MVP-5150" };
    public static PanelType G4_MVP_5200i               /**/ = new PanelType() { DeviceId = 329, DeviceType = "MVP-5200i" };
    public static PanelType G4_MVP_750                 /**/ = new PanelType() { DeviceId = 288, DeviceType = "MVP-750" };
    public static PanelType G4_MVP_8400i               /**/ = new PanelType() { DeviceId = 323, DeviceType = "MVP-8400i" };
    public static PanelType G4_MVP_8400                /**/ = new PanelType() { DeviceId = 289, DeviceType = "MVP-8400" };
    public static PanelType G4_MVP_9000i               /**/ = new PanelType() { DeviceId = 343, DeviceType = "MVP-9000i" };
    public static PanelType G4_NXD_430                 /**/ = new PanelType() { DeviceId = 342, DeviceType = "NXD-430" };
    public static PanelType G4_NXD_500i                /**/ = new PanelType() { DeviceId = 331, DeviceType = "NXD-500i" };
    public static PanelType G4_NXD_CV5                 /**/ = new PanelType() { DeviceId = 313, DeviceType = "NXD-CV5" };
    public static PanelType G4_NXD_700i                /**/ = new PanelType() { DeviceId = 377, DeviceType = "NXD-700i" };
    public static PanelType G4_NXD_700Vi               /**/ = new PanelType() { DeviceId = 324, DeviceType = "NXD-700Vi" };
    public static PanelType G4_NXD_CV7                 /**/ = new PanelType() { DeviceId = 291, DeviceType = "NXD-CV7" };
    public static PanelType G4_NXT_CA7                 /**/ = new PanelType() { DeviceId = 376, DeviceType = "NXT-CA7" };
    public static PanelType G4_NXT_CV7                 /**/ = new PanelType() { DeviceId = 291, DeviceType = "NXT-CV7" };
    public static PanelType G4_NXD_1000Vi              /**/ = new PanelType() { DeviceId = 325, DeviceType = "NXD-1000Vi" };
    public static PanelType G4_NXD_CV10                /**/ = new PanelType() { DeviceId = 292, DeviceType = "NXD-CV10" };
    public static PanelType G4_NXT_CV10                /**/ = new PanelType() { DeviceId = 292, DeviceType = "NXT-CV10" };
    public static PanelType G4_NXD_1200V               /**/ = new PanelType() { DeviceId = 294, DeviceType = "NXD-1200V" };
    public static PanelType G4_NXT_1200V               /**/ = new PanelType() { DeviceId = 294, DeviceType = "NXT-1200V" };
    public static PanelType G4_NXD_1200VG              /**/ = new PanelType() { DeviceId = 295, DeviceType = "NXD-1200VG" };
    public static PanelType G4_NXT_1200VG              /**/ = new PanelType() { DeviceId = 295, DeviceType = "NXT-1200VG" };
    public static PanelType G4_NXD_CA12                /**/ = new PanelType() { DeviceId = 277, DeviceType = "NXD-CA12" };
    public static PanelType G4_NXT_CA12                /**/ = new PanelType() { DeviceId = 277, DeviceType = "NXT-CA12" };
    public static PanelType G4_NXD_CV12                /**/ = new PanelType() { DeviceId = 278, DeviceType = "NXD-CV12" };
    public static PanelType G4_NXT_CV12                /**/ = new PanelType() { DeviceId = 278, DeviceType = "NXT-CV12" };
    public static PanelType G4_NXD_1500VG              /**/ = new PanelType() { DeviceId = 296, DeviceType = "NXD-1500VG" };
    public static PanelType G4_NXT_1500VG              /**/ = new PanelType() { DeviceId = 296, DeviceType = "NXT-1500VG" };
    public static PanelType G4_NXD_CA15                /**/ = new PanelType() { DeviceId = 281, DeviceType = "NXD-CA15" };
    public static PanelType G4_NXT_CA15                /**/ = new PanelType() { DeviceId = 281, DeviceType = "NXT-CA15" };
    public static PanelType G4_NXD_CV15                /**/ = new PanelType() { DeviceId = 282, DeviceType = "NXD-CV15" };
    public static PanelType G4_NXT_CV15                /**/ = new PanelType() { DeviceId = 282, DeviceType = "NXT-CV15" };
    public static PanelType G4_NXD_1700VG              /**/ = new PanelType() { DeviceId = 297, DeviceType = "NXD-1700VG" };
    public static PanelType G4_NXT_1700VG              /**/ = new PanelType() { DeviceId = 297, DeviceType = "NXT-1700VG" };
    public static PanelType G4_NXD_CV17                /**/ = new PanelType() { DeviceId = 284, DeviceType = "NXD-CV17" };
    public static PanelType G4_NXT_CV17                /**/ = new PanelType() { DeviceId = 284, DeviceType = "NXT-CV17" };
    public static PanelType G4_MST_431                 /**/ = new PanelType() { DeviceId = 398, DeviceType = "MST-431" };
    public static PanelType G4_MSD_431                 /**/ = new PanelType() { DeviceId = 399, DeviceType = "MSD-431" };
    public static PanelType G4_MST_701                 /**/ = new PanelType() { DeviceId = 400, DeviceType = "MST-701" };
    public static PanelType G4_MSD_701                 /**/ = new PanelType() { DeviceId = 401, DeviceType = "MSD-701" };
    public static PanelType G4_MST_1001                /**/ = new PanelType() { DeviceId = 402, DeviceType = "MST-1001" };
    public static PanelType G4_MSD_1001                /**/ = new PanelType() { DeviceId = 403, DeviceType = "MSD-1001" };
    public static PanelType G4_HPX_MSP_7               /**/ = new PanelType() { DeviceId = 468, DeviceType = "HPX-MSP-7" };
    public static PanelType G4_HPX_MSP_10              /**/ = new PanelType() { DeviceId = 469, DeviceType = "HPX-MSP-10" };
    public static PanelType G4_MXD_430                 /**/ = new PanelType() { DeviceId = 375, DeviceType = "MXD-430" };
    public static PanelType G4_MXD_430_L               /**/ = new PanelType() { DeviceId = 375, DeviceType = "MXD-430-L" };
    public static PanelType G4_MXT_700                 /**/ = new PanelType() { DeviceId = 373, DeviceType = "MXT-700" };
    public static PanelType G4_MXD_700                 /**/ = new PanelType() { DeviceId = 374, DeviceType = "MXD-700" };
    public static PanelType G4_MXT_1000                /**/ = new PanelType() { DeviceId = 371, DeviceType = "MXT-1000" };
    public static PanelType G4_MXD_1000                /**/ = new PanelType() { DeviceId = 372, DeviceType = "MXD-1000" };
    public static PanelType G4_MXT_1900L_PAN           /**/ = new PanelType() { DeviceId = 369, DeviceType = "MXT-1900L-PAN" };
    public static PanelType G4_MXD_1900L_PAN           /**/ = new PanelType() { DeviceId = 370, DeviceType = "MXD-1900L-PAN" };
    public static PanelType G4_MXT_2000XL_PAN          /**/ = new PanelType() { DeviceId = 361, DeviceType = "MXT-2000XL-PAN" };
    public static PanelType G4_MXD_2000XL_PAN          /**/ = new PanelType() { DeviceId = 368, DeviceType = "MXD-2000XL-PAN" };
    public static PanelType G4_NXP_PLV                 /**/ = new PanelType() { DeviceId = 326, DeviceType = "NXP-PLV" };
    public static PanelType G4_NXP_TPI4                /**/ = new PanelType() { DeviceId = 274, DeviceType = "NXP-TPI4" };
    public static PanelType G4_TPI_PRO                 /**/ = new PanelType() { DeviceId = 327, DeviceType = "TPI-PRO" };
    public static PanelType G4_TPI_PRO_DVI             /**/ = new PanelType() { DeviceId = 334, DeviceType = "TPI-PRO-DVI" };
    public static PanelType G4_NXV_300                 /**/ = new PanelType() { DeviceId = 341, DeviceType = "NXV-300" };
    public static PanelType G4_R_4                     /**/ = new PanelType() { DeviceId = 322, DeviceType = "R-4" };
    public static PanelType G4_Mio_Modero_DMS          /**/ = new PanelType() { DeviceId = 304, DeviceType = "Mio Modero DMS" };
    public static PanelType G4_Mio_Modero_DMS_Pinnacle /**/ = new PanelType() { DeviceId = 305, DeviceType = "Mio Modero DMS Pinnacle" };
    public static PanelType G4_iPod_touch_TPC          /**/ = new PanelType() { DeviceId = 349, DeviceType = "iPod touch (TPC)" };

    // =================================================================================
    // G5 Panels
    // =================================================================================

    public static PanelType G5_MXT_701      /**/ = new PanelType() { DeviceId = 415, DeviceType = "MXT-701" };
    public static PanelType G5_MXD_701      /**/ = new PanelType() { DeviceId = 417, DeviceType = "MXD-701" };
    public static PanelType G5_MT_702       /**/ = new PanelType() { DeviceId = 519, DeviceType = "MT-702" };
    public static PanelType G5_MD_702       /**/ = new PanelType() { DeviceId = 520, DeviceType = "MD-702" };
    public static PanelType G5_MXT_1001     /**/ = new PanelType() { DeviceId = 412, DeviceType = "MXT-1001" };
    public static PanelType G5_MXD_1001     /**/ = new PanelType() { DeviceId = 414, DeviceType = "MXD-1001" };
    public static PanelType G5_MT_1002      /**/ = new PanelType() { DeviceId = 517, DeviceType = "MT-1002" };
    public static PanelType G5_MD_1002      /**/ = new PanelType() { DeviceId = 518, DeviceType = "MD-1002" };
    public static PanelType G5_MXR_1001     /**/ = new PanelType() { DeviceId = 470, DeviceType = "MXR-1001" };
    public static PanelType G5_MXT_1901_PAN /**/ = new PanelType() { DeviceId = 409, DeviceType = "MXT-1901-PAN" };
    public static PanelType G5_MXD_1901_PAN /**/ = new PanelType() { DeviceId = 411, DeviceType = "MXD-1901-PAN" };
    public static PanelType G5_MXT_2001_PAN /**/ = new PanelType() { DeviceId = 406, DeviceType = "MXT-2001-PAN" };
    public static PanelType G5_MXD_2001_PAN /**/ = new PanelType() { DeviceId = 408, DeviceType = "MXD-2001-PAN" };
    public static PanelType G5_MT_2002      /**/ = new PanelType() { DeviceId = 514, DeviceType = "MT-2002" };
    public static PanelType G5_TPControl    /**/ = new PanelType() { DeviceId = 348, DeviceType = "TPControl" };

    private static List<PanelType> AllPanels = new List<PanelType>();

    static Panels()
    {
      // =================================================================================
      // G4 Panels (From PPF.xml)
      // =================================================================================

      AllPanels.Add(G4_MVP_5150);
      AllPanels.Add(G4_MVP_5200i);
      AllPanels.Add(G4_MVP_750);
      AllPanels.Add(G4_MVP_8400i);
      AllPanels.Add(G4_MVP_8400);
      AllPanels.Add(G4_MVP_9000i);
      AllPanels.Add(G4_NXD_430);
      AllPanels.Add(G4_NXD_500i);
      AllPanels.Add(G4_NXD_CV5);
      AllPanels.Add(G4_NXD_700i);
      AllPanels.Add(G4_NXD_700Vi);
      AllPanels.Add(G4_NXD_CV7);
      AllPanels.Add(G4_NXT_CA7);
      AllPanels.Add(G4_NXT_CV7);
      AllPanels.Add(G4_NXD_1000Vi);
      AllPanels.Add(G4_NXD_CV10);
      AllPanels.Add(G4_NXT_CV10);
      AllPanels.Add(G4_NXD_1200V);
      AllPanels.Add(G4_NXT_1200V);
      AllPanels.Add(G4_NXD_1200VG);
      AllPanels.Add(G4_NXT_1200VG);
      AllPanels.Add(G4_NXD_CA12);
      AllPanels.Add(G4_NXT_CA12);
      AllPanels.Add(G4_NXD_CV12);
      AllPanels.Add(G4_NXT_CV12);
      AllPanels.Add(G4_NXD_1500VG);
      AllPanels.Add(G4_NXT_1500VG);
      AllPanels.Add(G4_NXD_CA15);
      AllPanels.Add(G4_NXT_CA15);
      AllPanels.Add(G4_NXD_CV15);
      AllPanels.Add(G4_NXT_CV15);
      AllPanels.Add(G4_NXD_1700VG);
      AllPanels.Add(G4_NXT_1700VG);
      AllPanels.Add(G4_NXD_CV17);
      AllPanels.Add(G4_NXT_CV17);
      AllPanels.Add(G4_MST_431);
      AllPanels.Add(G4_MSD_431);
      AllPanels.Add(G4_MST_701);
      AllPanels.Add(G4_MSD_701);
      AllPanels.Add(G4_MST_1001);
      AllPanels.Add(G4_MSD_1001);
      AllPanels.Add(G4_HPX_MSP_7);
      AllPanels.Add(G4_HPX_MSP_10);
      AllPanels.Add(G4_MXD_430);
      AllPanels.Add(G4_MXD_430_L);
      AllPanels.Add(G4_MXT_700);
      AllPanels.Add(G4_MXD_700);
      AllPanels.Add(G4_MXT_1000);
      AllPanels.Add(G4_MXD_1000);
      AllPanels.Add(G4_MXT_1900L_PAN);
      AllPanels.Add(G4_MXD_1900L_PAN);
      AllPanels.Add(G4_MXT_2000XL_PAN);
      AllPanels.Add(G4_MXD_2000XL_PAN);
      AllPanels.Add(G4_NXP_PLV);
      AllPanels.Add(G4_NXP_TPI4);
      AllPanels.Add(G4_TPI_PRO);
      AllPanels.Add(G4_TPI_PRO_DVI);
      AllPanels.Add(G4_NXV_300);
      AllPanels.Add(G4_R_4);
      AllPanels.Add(G4_Mio_Modero_DMS);
      AllPanels.Add(G4_Mio_Modero_DMS_Pinnacle);
      AllPanels.Add(G4_iPod_touch_TPC);

      // =================================================================================
      // G5 Panels
      // =================================================================================

      AllPanels.Add(G5_MXT_701);
      AllPanels.Add(G5_MXD_701);
      AllPanels.Add(G5_MT_702);
      AllPanels.Add(G5_MD_702);
      AllPanels.Add(G5_MXT_1001);
      AllPanels.Add(G5_MXD_1001);
      AllPanels.Add(G5_MT_1002);
      AllPanels.Add(G5_MD_1002);
      AllPanels.Add(G5_MXR_1001);
      AllPanels.Add(G5_MXT_1901_PAN);
      AllPanels.Add(G5_MXD_1901_PAN);
      AllPanels.Add(G5_MXT_2001_PAN);
      AllPanels.Add(G5_MXD_2001_PAN);
      AllPanels.Add(G5_MT_2002);
      AllPanels.Add(G5_TPControl);
    }

    public static PanelType GetPanelByDeviceId(ushort deviceId)
    {
      return AllPanels.FirstOrDefault(p => p.DeviceId == deviceId);
    }

    public static PanelType GetPanelByDeviceType(string deviceType)
    {
      if(string.IsNullOrWhiteSpace(deviceType))
        return Empty;

      return AllPanels.FirstOrDefault(p => p.DeviceType == deviceType);
    }
  }
}
