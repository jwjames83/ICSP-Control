namespace ICSP.Core.Model
{
  public enum JustificationType
  {
    Absolute     /**/ = 0,
    TopLeft      /**/ = 1,
    TopMiddle    /**/ = 2,
    TopRight     /**/ = 3,
    CenterLeft   /**/ = 4,
    CenterMiddle /**/ = 5,
    CenterRight  /**/ = 6,
    BottomLeft   /**/ = 7,
    BottomMiddle /**/ = 8,
    BottomRight  /**/ = 9,

    // G5 Only
    ScaleToFit               /**/ = 10,
    ScaleMaintainAspectRatio /**/ = 11,
  }
}
