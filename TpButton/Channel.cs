using System.ComponentModel;

namespace TpControls
{
  [TypeConverter(typeof(PropertySorter))]
  public class Channel
  {
    [DefaultValue(FeedbackType.None)]
    [PropertyOrder(1)]
    public FeedbackType Feedback { get; set; } = FeedbackType.None;

    [DefaultValue(1)]
    [PropertyOrder(2)]
    public int ChannelPort { get; set; } = 1;

    [DefaultValue(0)]
    [PropertyOrder(3)]
    public int ChannelCode { get; set; } = 0;
  }
}
