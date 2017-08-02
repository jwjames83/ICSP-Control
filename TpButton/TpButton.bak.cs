using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSP;
using ICSP.Client;

namespace TpControls
{
  public partial class TpButton : Button, ISupportInitialize
  {
    private ICSPManager mManager;

    private bool mInitializing;

    private StateType mOn;
    
    public TpButton()
    {
      InitializeComponent();

      Channel = new Channel();

      StateAll = new State();
      StateOff = new State();
      StateOn = new State();

      StateOff.Font = Font;
      StateOn.Font = Font;

      CurrentState = StateOff;

      StateAll.PropertyChanged += OnPropertyChanged;
      StateOff.PropertyChanged += OnPropertyChanged;
      StateOn.PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      var lState = sender as State;

      if(lState != null)
      {
        // BorderColor
        if(e.PropertyName == "BorderColor")
        {
          if(sender == StateAll)
          {
            StateOff.SetBorderColor(lState.BorderColor);
            StateOn.SetBorderColor(lState.BorderColor);
          }

          if(sender == StateOff || sender == StateOn)
          {
            if(StateOff.BorderColor == StateOn.BorderColor)
              StateAll.SetBorderColor(lState.BorderColor);
            else
              StateAll.SetBorderColor(Color.Empty);
          }

          base.FlatAppearance.BorderColor = CurrentState.BorderColor;
        }

        // FillColor
        if(e.PropertyName == "FillColor")
        {
          if(sender == StateAll)
          {
            StateOff.SetFillColor(lState.FillColor);
            StateOn.SetFillColor(lState.FillColor);
          }

          if(sender == StateOff || sender == StateOn)
          {
            if(StateOff.FillColor == StateOn.FillColor)
              StateAll.SetFillColor(lState.FillColor);
            else
              StateAll.SetFillColor(Color.Empty);
          }

          base.BackColor = CurrentState.FillColor;
        }

        // Text
        if(e.PropertyName == "Text")
        {
          if(sender == StateAll)
          {
            StateOff.SetText(lState.Text);
            StateOn.SetText(lState.Text);
          }

          if(sender == StateOff || sender == StateOn)
          {
            if(StateOff.Text == StateOn.Text)
              StateAll.SetText(lState.Text);
            else
              StateAll.SetText(string.Empty);
          }

          base.Text = CurrentState.Text;
        }

        Refresh();
      }
    }

    #region Properties

    [Category("Data")]
    public Channel Channel { get; set; }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
        return mOn;
      }
      set
      {
        mOn = value;
        
        if(value == StateType.On)
          CurrentState = StateOn;
        else
          CurrentState = StateOff;

        Refresh();
      }
    }

    [Browsable(false)]
    [Bindable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public State CurrentState { get; private set; }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color BackColor
    {
      get
      {
        return CurrentState.FillColor;
      }
    }
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string Text
    {
      get
      {
        return CurrentState.Text;
      }
    }
    
    #endregion

    public void SetManager(ICSPManager manager)
    {
      if(mManager != null)
      {
        mManager.Disconnected -= OnManagerDisconnected;
        mManager.ChannelEvent -= OnChannelEvent;
      }

      mManager = manager;

      if(mManager != null)
      {
        mManager.Disconnected += OnManagerDisconnected;
        mManager.ChannelEvent += OnChannelEvent;
      }
    }

    private void OnManagerDisconnected(object sender, ClientConnectedEventArgs e)
    {
      OnChannelEvent(this, new ChannelEventArgs(Channel?.ChannelPort ?? 0, Channel?.ChannelCode ?? 0, false));
    }

    private void OnChannelEvent(object sender, ChannelEventArgs e)
    {
      if(e.ChannelCode == Channel?.ChannelCode && e.ChannelPort == Channel?.ChannelPort)
      {
        if(e.Enabled)
          CurrentState = StateOn;
        else
          CurrentState = StateOff;

        Refresh();
      }
    }

    public void BeginInit()
    {
      mInitializing = true;
    }

    public void EndInit()
    {
      mInitializing = false;
    }
  }
}
