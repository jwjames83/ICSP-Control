using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using ICSP;
using ICSP.Client;

namespace TpControls
{
  public partial class TpButton : Label
  {
    private ICSPManager mManager;

    private State mCurrentState;

    private StateType mState;

    public TpButton()
    {
      InitializeComponent();

      Channel = new Channel();

      StateAll = new State();
      StateOff = new State();
      StateOn = new State();

      StateOff.Font = Font;
      StateOn.Font = Font;

      mCurrentState = StateOff;

      StateAll.PropertyChanged += OnPropertyChanged;
      StateOff.PropertyChanged += OnPropertyChanged;
      StateOn.PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if(sender == StateAll)
      {
        switch(e.PropertyName)
        {
          case "BorderColor":
            {
              StateOff.SetBorderColor(StateAll.BorderColor);
              StateOn.SetBorderColor(StateAll.BorderColor);
              break;
            }
          case "BorderSize":
            {
              StateOff.SetBorderSize(StateAll.BorderSize);
              StateOn.SetBorderSize(StateAll.BorderSize);
              break;
            }
          case "FillColor":
            {
              StateOff.SetFillColor(StateAll.FillColor);
              StateOn.SetFillColor(StateAll.FillColor);

              base.BackColor = CurrentState.FillColor;
              break;
            }
          case "TextColor":
            {
              StateOff.SetTextColor(StateAll.TextColor);
              StateOn.SetTextColor(StateAll.TextColor);

              base.ForeColor = CurrentState.TextColor;
              break;
            }
          case "Bitmap":
            {
              StateOff.SetBitmap(StateAll.Bitmap);
              StateOn.SetBitmap(StateAll.Bitmap);

              base.Image = CurrentState.Bitmap;
              break;
            }
          case "BitmapJustification":
            {
              StateOff.SetBitmapJustification(StateAll.BitmapJustification);
              StateOn.SetBitmapJustification(StateAll.BitmapJustification);

              base.ImageAlign = CurrentState.BitmapJustification ?? ContentAlignment.MiddleCenter;
              break;
            }
          case "Font":
            {
              StateOff.SetFont(StateAll.Font);
              StateOn.SetFont(StateAll.Font);

              base.Font = CurrentState.Font;
              break;
            }
          case "Text":
            {
              StateOff.SetText(StateAll.Text);
              StateOn.SetText(StateAll.Text);

              base.Text = CurrentState.Text;
              break;
            }
          case "TextJustification":
            {
              StateOff.SetTextJustification(StateAll.TextJustification);
              StateOn.SetTextJustification(StateAll.TextJustification);

              base.TextAlign = CurrentState.TextJustification ?? ContentAlignment.MiddleCenter;
              break;
            }
        }
      }

      if(sender == StateOff || sender == StateOn)
      {
        switch(e.PropertyName)
        {
          case "BorderColor":
            {
              if(StateOff.BorderColor == StateOn.BorderColor)
                StateAll.SetBorderColor(StateOff.BorderColor);
              else
                StateAll.SetBorderColor(Color.Empty);

              break;
            }
          case "BorderSize":
            {
              if(StateOff.BorderSize == StateOn.BorderSize)
                StateAll.SetBorderSize(StateOff.BorderSize);
              else
                StateAll.SetBorderSize(null);

              break;
            }
          case "FillColor":
            {
              if(StateOff.FillColor == StateOn.FillColor)
                StateAll.SetFillColor(StateOff.FillColor);
              else
                StateAll.SetFillColor(Color.Empty);

              base.BackColor = CurrentState.FillColor;
              break;
            }
          case "TextColor":
            {
              if(StateOff.TextColor == StateOn.TextColor)
                StateAll.SetTextColor(StateOff.TextColor);
              else
                StateAll.SetTextColor(Color.Empty);

              base.ForeColor = CurrentState.TextColor;
              break;
            }
          case "Bitmap":
            {
              if(StateOff.Bitmap == StateOn.Bitmap)
                StateAll.SetBitmap(StateOff.Bitmap);
              else
                StateAll.SetBitmap(null);

              base.Image = CurrentState.Bitmap;
              break;
            }
          case "BitmapJustification":
            {
              if(StateOff.BitmapJustification == StateOn.BitmapJustification)
                StateAll.SetBitmapJustification(StateOff.BitmapJustification);
              else
                StateAll.SetBitmapJustification(null);

              base.ImageAlign = CurrentState.BitmapJustification ?? ContentAlignment.MiddleCenter;
              break;
            }
          case "Font":
            {
              if(StateOff.Font == StateOn.Font)
                StateAll.SetFont(StateOff.Font);
              else
                StateAll.SetFont(null);

              base.Font = CurrentState.Font;
              break;
            }
          case "Text":
            {
              if(StateOff.Text == StateOn.Text)
                StateAll.SetText(StateOff.Text);
              else
                StateAll.SetText(string.Empty);

              base.Text = CurrentState.Text;
              break;
            }
          case "TextJustification":
            {
              if(StateOff.TextJustification == StateOn.TextJustification)
                StateAll.SetTextJustification(StateOff.TextJustification);
              else
                StateAll.SetTextJustification(null);

              base.TextAlign = CurrentState.TextJustification ?? ContentAlignment.MiddleCenter;
              break;
            }
        }
      }

      Refresh();
    }

    #region Properties

    [Category("Data")]
    public Channel Channel { get; set; }

    private bool ShouldSerializeStateAll()
    {
      return true;
    }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public State StateAll { get; private set; }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public State StateOff { get; private set; }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public State StateOn { get; private set; }

    [Category("Data")]
    [DefaultValue(StateType.Off)]
    public StateType State
    {
      get
      {
        return mState;
      }
      set
      {
        mState = value;

        if(DesignMode)
        {
          if(value == StateType.On)
            CurrentState = StateOn;
          else
            CurrentState = StateOff;
        }
        else
        {
          if(Channel.Feedback == FeedbackType.AllwaysOn)
            CurrentState = StateOn;
          else
          {
            if(value == StateType.On)
              CurrentState = StateOn;
            else
              CurrentState = StateOff;
          }
        }
      }
    }

    [Browsable(false)]
    [Bindable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public State CurrentState
    {
      get
      {
        return mCurrentState;
      }
      private set
      {
        mCurrentState = value;

        base.Image = CurrentState.Bitmap;
        base.ImageAlign = CurrentState.BitmapJustification ?? ContentAlignment.MiddleCenter;

        Refresh();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color BackColor
    {
      get
      {
        return CurrentState?.FillColor ?? Control.DefaultBackColor;
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color ForeColor
    {
      get
      {
        return CurrentState?.TextColor ?? SystemColors.ControlText;
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Image Image
    {
      get
      {
        return CurrentState?.Bitmap;
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ContentAlignment ImageAlign
    {
      get
      {
        return CurrentState?.BitmapJustification ?? ContentAlignment.MiddleCenter;
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Font Font
    {
      get
      {
        return CurrentState?.Font ?? Control.DefaultFont;
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string Text
    {
      get
      {
        return CurrentState?.Text ?? string.Empty;
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override ContentAlignment TextAlign
    {
      get
      {
        return CurrentState?.TextJustification ?? ContentAlignment.MiddleCenter;
      }
    }

    #endregion

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      var lColor = CurrentState?.BorderColor ?? Color.Empty;
      var lSize = CurrentState?.BorderSize ?? 0;

      ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
        lColor, lSize, ButtonBorderStyle.Solid,
        lColor, lSize, ButtonBorderStyle.Solid,
        lColor, lSize, ButtonBorderStyle.Solid,
        lColor, lSize, ButtonBorderStyle.Solid);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);

      if(Channel.Feedback == FeedbackType.Momentary)
        CurrentState = StateOn;

      Refresh();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);

      if(Channel.Feedback == FeedbackType.Momentary)
        CurrentState = StateOff;

      Refresh();
    }

    public void Reset()
    {
      if(Channel.Feedback == FeedbackType.AllwaysOn)
      {
        CurrentState = StateOn;
      }
      else
      {
        CurrentState = StateOff;
      }
    }

    public void SetManager(ICSPManager manager)
    {
      if(mManager != null)
      {
        mManager.Disconnected -= OnManagerDisconnected;
        mManager.ChannelEvent -= OnChannelEvent;
      }

      Reset();

      mManager = manager;

      if(mManager != null)
      {
        mManager.Disconnected += OnManagerDisconnected;
        mManager.ChannelEvent += OnChannelEvent;
      }
    }

    private void OnManagerDisconnected(object sender, ClientConnectedEventArgs e)
    {
      CurrentState = StateOff;
    }

    private void OnChannelEvent(object sender, ChannelEventArgs e)
    {
      if(e.Device.Port == Channel?.ChannelPort && e.Channel == Channel?.ChannelCode)
      {
        switch(Channel.Feedback)
        {
          case FeedbackType.Channel:
            {
              if(e.Enabled)
                CurrentState = StateOn;
              else
                CurrentState = StateOff;

              break;
            }
          case FeedbackType.InvertedChannel:
            {
              if(e.Enabled)
                CurrentState = StateOff;
              else
                CurrentState = StateOn;

              break;
            }
          case FeedbackType.Blink:
            {
              break;
            }
        }

        Refresh();
      }
    }
  }
}
