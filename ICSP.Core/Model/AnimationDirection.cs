namespace ICSP.Core.Model
{
  public enum AnimationDirection
  {
    None       /**/ = 0,
    Center     /**/ = 1,
    TopOrDown  /**/ = 2,
    UpOrBottom /**/ = 3,
    Right      /**/ = 4, // AnimationType Wipe: -> Left
    Left       /**/ = 5, // AnimationType Wipe: -> Right
    LowerLeft  /**/ = 6,
    LowerRight /**/ = 7,
    UpperLeft  /**/ = 8,
    UpperRight /**/ = 9,
  }
}
