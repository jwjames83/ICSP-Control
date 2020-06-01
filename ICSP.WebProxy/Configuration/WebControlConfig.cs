namespace ICSP.WebProxy.Configuration
{
  public class WebControlConfig
  {
    /// <summary>
    /// Module WebControl.axs Version 2.5.5 does not encode UTF8.<br/>
    /// Module WebControl.axs Version 2.6.0 does encode UTF8 by default self.
    /// </summary>
    public bool SupportUTF8 { get; set; } = true;
  }
}
