using System;
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

    public static PanelType G4_MVP_5150                /**/ = new PanelType(PanelGeneration.G4, 333, "MVP-5150",                /**/ "MVP-5150",                 /**/ "");
    public static PanelType G4_MVP_5200i               /**/ = new PanelType(PanelGeneration.G4, 329, "MVP-5200i",               /**/ "MVP-5200i",                /**/ "");
    public static PanelType G4_MVP_750                 /**/ = new PanelType(PanelGeneration.G4, 288, "MVP-750",                 /**/ "MVP-750",                  /**/ "");
    public static PanelType G4_MVP_8400i               /**/ = new PanelType(PanelGeneration.G4, 323, "MVP-8400i",               /**/ "MVP-8400i",                /**/ "");
    public static PanelType G4_MVP_8400                /**/ = new PanelType(PanelGeneration.G4, 289, "MVP-8400",                /**/ "MVP-8400",                 /**/ "");
    public static PanelType G4_MVP_9000i               /**/ = new PanelType(PanelGeneration.G4, 343, "MVP-9000i",               /**/ "MVP-9000i",                /**/ "");
    public static PanelType G4_NXD_430                 /**/ = new PanelType(PanelGeneration.G4, 342, "NXD-430",                 /**/ "NXD-430",                  /**/ "");
    public static PanelType G4_NXD_500i                /**/ = new PanelType(PanelGeneration.G4, 331, "NXD-500i",                /**/ "NXD-500i",                 /**/ "");
    public static PanelType G4_NXD_CV5                 /**/ = new PanelType(PanelGeneration.G4, 313, "NXD-CV5",                 /**/ "NXD-CV5",                  /**/ "");
    public static PanelType G4_NXD_700i                /**/ = new PanelType(PanelGeneration.G4, 377, "NXD-700i",                /**/ "NXD-700i",                 /**/ "");
    public static PanelType G4_NXD_700Vi               /**/ = new PanelType(PanelGeneration.G4, 324, "NXD-700Vi",               /**/ "NXD-700Vi",                /**/ "");
    public static PanelType G4_NXD_CV7                 /**/ = new PanelType(PanelGeneration.G4, 291, "NXD-CV7",                 /**/ "NXD-CV7",                  /**/ "");
    public static PanelType G4_NXT_CA7                 /**/ = new PanelType(PanelGeneration.G4, 376, "NXT-CA7",                 /**/ "NXT-CA7",                  /**/ "");
    public static PanelType G4_NXT_CV7                 /**/ = new PanelType(PanelGeneration.G4, 291, "NXT-CV7",                 /**/ "NXT-CV7",                  /**/ "");
    public static PanelType G4_NXD_1000Vi              /**/ = new PanelType(PanelGeneration.G4, 325, "NXD-1000Vi",              /**/ "NXD-1000Vi",               /**/ "");
    public static PanelType G4_NXD_CV10                /**/ = new PanelType(PanelGeneration.G4, 292, "NXD-CV10",                /**/ "NXD-CV10",                 /**/ "");
    public static PanelType G4_NXT_CV10                /**/ = new PanelType(PanelGeneration.G4, 292, "NXT-CV10",                /**/ "NXT-CV10",                 /**/ "");
    public static PanelType G4_NXD_1200V               /**/ = new PanelType(PanelGeneration.G4, 294, "NXD-1200V",               /**/ "NXD-1200V",                /**/ "");
    public static PanelType G4_NXT_1200V               /**/ = new PanelType(PanelGeneration.G4, 294, "NXT-1200V",               /**/ "NXT-1200V",                /**/ "");
    public static PanelType G4_NXD_1200VG              /**/ = new PanelType(PanelGeneration.G4, 295, "NXD-1200VG",              /**/ "NXD-1200VG",               /**/ "");
    public static PanelType G4_NXT_1200VG              /**/ = new PanelType(PanelGeneration.G4, 295, "NXT-1200VG",              /**/ "NXT-1200VG",               /**/ "");
    public static PanelType G4_NXD_CA12                /**/ = new PanelType(PanelGeneration.G4, 277, "NXD-CA12",                /**/ "NXD-CA12",                 /**/ "");
    public static PanelType G4_NXT_CA12                /**/ = new PanelType(PanelGeneration.G4, 277, "NXT-CA12",                /**/ "NXT-CA12",                 /**/ "");
    public static PanelType G4_NXD_CV12                /**/ = new PanelType(PanelGeneration.G4, 278, "NXD-CV12",                /**/ "NXD-CV12",                 /**/ "");
    public static PanelType G4_NXT_CV12                /**/ = new PanelType(PanelGeneration.G4, 278, "NXT-CV12",                /**/ "NXT-CV12",                 /**/ "");
    public static PanelType G4_NXD_1500VG              /**/ = new PanelType(PanelGeneration.G4, 296, "NXD-1500VG",              /**/ "NXD-1500VG",               /**/ "");
    public static PanelType G4_NXT_1500VG              /**/ = new PanelType(PanelGeneration.G4, 296, "NXT-1500VG",              /**/ "NXT-1500VG",               /**/ "");
    public static PanelType G4_NXD_CA15                /**/ = new PanelType(PanelGeneration.G4, 281, "NXD-CA15",                /**/ "NXD-CA15",                 /**/ "");
    public static PanelType G4_NXT_CA15                /**/ = new PanelType(PanelGeneration.G4, 281, "NXT-CA15",                /**/ "NXT-CA15",                 /**/ "");
    public static PanelType G4_NXD_CV15                /**/ = new PanelType(PanelGeneration.G4, 282, "NXD-CV15",                /**/ "NXD-CV15",                 /**/ "");
    public static PanelType G4_NXT_CV15                /**/ = new PanelType(PanelGeneration.G4, 282, "NXT-CV15",                /**/ "NXT-CV15",                 /**/ "");
    public static PanelType G4_NXD_1700VG              /**/ = new PanelType(PanelGeneration.G4, 297, "NXD-1700VG",              /**/ "NXD-1700VG",               /**/ "");
    public static PanelType G4_NXT_1700VG              /**/ = new PanelType(PanelGeneration.G4, 297, "NXT-1700VG",              /**/ "NXT-1700VG",               /**/ "");
    public static PanelType G4_NXD_CV17                /**/ = new PanelType(PanelGeneration.G4, 284, "NXD-CV17",                /**/ "NXD-CV17",                 /**/ "");
    public static PanelType G4_NXT_CV17                /**/ = new PanelType(PanelGeneration.G4, 284, "NXT-CV17",                /**/ "NXT-CV17",                 /**/ "");
    public static PanelType G4_MST_431                 /**/ = new PanelType(PanelGeneration.G4, 398, "MST-431",                 /**/ "MST-431",                  /**/ "");
    public static PanelType G4_MSD_431                 /**/ = new PanelType(PanelGeneration.G4, 399, "MSD-431",                 /**/ "MSD-431",                  /**/ "");
    public static PanelType G4_MST_701                 /**/ = new PanelType(PanelGeneration.G4, 400, "MST-701",                 /**/ "MST-701",                  /**/ "");
    public static PanelType G4_MSD_701                 /**/ = new PanelType(PanelGeneration.G4, 401, "MSD-701",                 /**/ "MSD-701",                  /**/ "");
    public static PanelType G4_MST_1001                /**/ = new PanelType(PanelGeneration.G4, 402, "MST-1001",                /**/ "MST-1001",                 /**/ "");
    public static PanelType G4_MSD_1001                /**/ = new PanelType(PanelGeneration.G4, 403, "MSD-1001",                /**/ "MSD-1001",                 /**/ "");
    public static PanelType G4_HPX_MSP_7               /**/ = new PanelType(PanelGeneration.G4, 468, "HPX-MSP-7",               /**/ "HPX-MSP-7",                /**/ "");
    public static PanelType G4_HPX_MSP_10              /**/ = new PanelType(PanelGeneration.G4, 469, "HPX-MSP-10",              /**/ "HPX-MSP-10",               /**/ "");
    public static PanelType G4_MXD_430                 /**/ = new PanelType(PanelGeneration.G4, 375, "MXD-430",                 /**/ "MXD-430",                  /**/ "");
    public static PanelType G4_MXD_430_L               /**/ = new PanelType(PanelGeneration.G4, 375, "MXD-430-L",               /**/ "MXD-430-L",                /**/ "");
    public static PanelType G4_MXT_700                 /**/ = new PanelType(PanelGeneration.G4, 373, "MXT-700",                 /**/ "MXT-700",                  /**/ "");
    public static PanelType G4_MXD_700                 /**/ = new PanelType(PanelGeneration.G4, 374, "MXD-700",                 /**/ "MXD-700",                  /**/ "");
    public static PanelType G4_MXT_1000                /**/ = new PanelType(PanelGeneration.G4, 371, "MXT-1000",                /**/ "MXT-1000",                 /**/ "");
    public static PanelType G4_MXD_1000                /**/ = new PanelType(PanelGeneration.G4, 372, "MXD-1000",                /**/ "MXD-1000",                 /**/ "");
    public static PanelType G4_MXT_1900L_PAN           /**/ = new PanelType(PanelGeneration.G4, 369, "MXT-1900L-PAN",           /**/ "MXT-1900L-PAN",            /**/ "");
    public static PanelType G4_MXD_1900L_PAN           /**/ = new PanelType(PanelGeneration.G4, 370, "MXD-1900L-PAN",           /**/ "MXD-1900L-PAN",            /**/ "");
    public static PanelType G4_MXT_2000XL_PAN          /**/ = new PanelType(PanelGeneration.G4, 361, "MXT-2000XL-PAN",          /**/ "MXT-2000XL-PAN",           /**/ "");
    public static PanelType G4_MXD_2000XL_PAN          /**/ = new PanelType(PanelGeneration.G4, 368, "MXD-2000XL-PAN",          /**/ "MXD-2000XL-PAN",           /**/ "");
    public static PanelType G4_NXP_PLV                 /**/ = new PanelType(PanelGeneration.G4, 326, "NXP-PLV",                 /**/ "NXP-PLV",                  /**/ "");
    public static PanelType G4_NXP_TPI4                /**/ = new PanelType(PanelGeneration.G4, 274, "NXP-TPI4",                /**/ "NXP-TPI4",                 /**/ "");
    public static PanelType G4_TPI_PRO                 /**/ = new PanelType(PanelGeneration.G4, 327, "TPI-PRO",                 /**/ "TPI-PRO",                  /**/ "");
    public static PanelType G4_TPI_PRO_DVI             /**/ = new PanelType(PanelGeneration.G4, 334, "TPI-PRO-DVI",             /**/ "TPI-PRO-DVI",              /**/ "");
    public static PanelType G4_NXV_300                 /**/ = new PanelType(PanelGeneration.G4, 341, "NXV-300",                 /**/ "NXV-300",                  /**/ "");
    public static PanelType G4_R_4                     /**/ = new PanelType(PanelGeneration.G4, 322, "R-4",                     /**/ "R-4",                      /**/ "");
    public static PanelType G4_Mio_Modero_DMS          /**/ = new PanelType(PanelGeneration.G4, 304, "Mio Modero DMS",          /**/ "Mio Modero DMS",           /**/ "");
    public static PanelType G4_Mio_Modero_DMS_Pinnacle /**/ = new PanelType(PanelGeneration.G4, 305, "Mio Modero DMS Pinnacle", /**/ "Mio Modero DMS Pinnacle",  /**/ "");
    public static PanelType G4_iPod_touch_TPC          /**/ = new PanelType(PanelGeneration.G4, 349, "iPod touch (TPC)",        /**/ "iPod touch (TPC)",         /**/ "");

    // =================================================================================
    // G5 Panels
    // =================================================================================

    public static PanelType G5_MXT_701      /**/ = new PanelType(PanelGeneration.G5, 415, "XF-700T",    /**/ "MXT-701",      /**/ "Modero X 7\" tabletop touch panel");
    public static PanelType G5_MXD_701      /**/ = new PanelType(PanelGeneration.G5, 417, "XF-700D",    /**/ "MXD-701",      /**/ "Modero X 7\" wall - mount touch panel");
    public static PanelType G5_MT_702       /**/ = new PanelType(PanelGeneration.G5, 519, "TET-700T",   /**/ "MT-702",       /**/ "Modero 7\" tabletop touch panel");
    public static PanelType G5_MD_702       /**/ = new PanelType(PanelGeneration.G5, 520, "TET-700D",   /**/ "MD-702",       /**/ "Modero 7\" wall - mount touch panel");
    public static PanelType G5_MXT_1001     /**/ = new PanelType(PanelGeneration.G5, 412, "XF-1000T",   /**/ "MXT-1001",     /**/ "Modero X 10\" tabletop touch panel");
    public static PanelType G5_MXD_1001     /**/ = new PanelType(PanelGeneration.G5, 414, "XF-1000D",   /**/ "MXD-1001",     /**/ "Modero X 10\" wall - mount touch panel");
    public static PanelType G5_MT_1002      /**/ = new PanelType(PanelGeneration.G5, 517, "TET-1000T",  /**/ "MT-1002",      /**/ "Modero 10\" tabletop touch panel");
    public static PanelType G5_MD_1002      /**/ = new PanelType(PanelGeneration.G5, 518, "TET-1000D",  /**/ "MD-1002",      /**/ "Modero 10\" wall - mount touch panel");
    public static PanelType G5_MXR_1001     /**/ = new PanelType(PanelGeneration.G5, 470, "CAB-1001PU", /**/ "MXR-1001",     /**/ "Modero X 10\" Retractable touch panel");
    public static PanelType G5_MXT_1901_PAN /**/ = new PanelType(PanelGeneration.G5, 409, "XF-1900T",   /**/ "MXT-1901-PAN", /**/ "Modero X 19\" Panoramic tabletop touch panel");
    public static PanelType G5_MXD_1901_PAN /**/ = new PanelType(PanelGeneration.G5, 411, "XF-1900D",   /**/ "MXD-1901-PAN", /**/ "Modero X 19\" Panoramic wall - mount touch panel");
    public static PanelType G5_MXT_2001_PAN /**/ = new PanelType(PanelGeneration.G5, 406, "XF-2000T",   /**/ "MXT-2001-PAN", /**/ "Modero X 20\" Panoramic tabletop touch panel");
    public static PanelType G5_MXD_2001_PAN /**/ = new PanelType(PanelGeneration.G5, 408, "XF-2000D",   /**/ "MXD-2001-PAN", /**/ "Modero X 20\" Panoramic wall - mount touch panel");
    public static PanelType G5_MT_2002      /**/ = new PanelType(PanelGeneration.G5, 514, "TET-2000T",  /**/ "MT-2002",      /**/ "Modero 20\" Panoramic tabletop touch panel");
    public static PanelType G5_TPControl    /**/ = new PanelType(PanelGeneration.G5, 348, "TPControl",  /**/ "TPControl",    /**/ "TPControl (www.touchpanelcontrol.com)");

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

      return AllPanels.FirstOrDefault(p => p.Type.Equals(deviceType, StringComparison.OrdinalIgnoreCase));
    }
  }
}
