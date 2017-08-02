using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TpControls
{
  [TypeConverter(typeof(FlatButtonAppearanceConverter))]
  public class FlatButtonAppearance
  {
    // Fields
    private Color borderColor = Color.Empty;
    private int borderSize = 1;
    private Color checkedBackColor = Color.Empty;
    private Color mouseDownBackColor = Color.Empty;
    private Color mouseOverBackColor = Color.Empty;
    private ButtonBase owner;

    // Methods
    internal FlatButtonAppearance()
    {
      this.owner = null;
    }

    // Properties
    [Browsable(true), NotifyParentProperty(true), Category("Appearance"), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(typeof(Color), "")]
    public Color BorderColor
    {
      get
      {
        return this.borderColor;
      }
      set
      {
        if(value.Equals(Color.Transparent))
        {
          throw new NotSupportedException("InvalidBorderColor");
        }
        if(this.borderColor != value)
        {
          this.borderColor = value;
          this.owner.Invalidate();
        }
      }
    }
    
    [Browsable(true), NotifyParentProperty(true), Category("Appearance"), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(1)]
    public int BorderSize
    {
      get
      {
        return this.borderSize;
      }
      set
      {
        if(value < 0)
        {
          // object[] args = new object[] { "BorderSize", value.ToString(CultureInfo.CurrentCulture), 0.ToString(CultureInfo.CurrentCulture) };
          throw new ArgumentOutOfRangeException("BorderSize", value, "InvalidLowBoundArgumentEx");
        }
        if(this.borderSize != value)
        {
          this.borderSize = value;

         //  if((this.owner != null) && (this.owner.ParentInternal != null))
          {
            // LayoutTransaction.DoLayoutIf(this.owner.AutoSize, this.owner.ParentInternal, this.owner, PropertyNames.FlatAppearanceBorderSize);
          }
          this.owner.Invalidate();
        }
      }
    }

    [Browsable(true), NotifyParentProperty(true), Category("Appearance"), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(typeof(Color), "")]
    public Color CheckedBackColor
    {
      get
      {
        return this.checkedBackColor;
      }
      set
      {
        if(this.checkedBackColor != value)
        {
          this.checkedBackColor = value;
          this.owner.Invalidate();
        }
      }
    }

    [Browsable(true), NotifyParentProperty(true), Category("Appearance"), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(typeof(Color), "")]
    public Color MouseDownBackColor
    {
      get
      {
        return this.mouseDownBackColor;
      }
      set
      {
        if(this.mouseDownBackColor != value)
        {
          this.mouseDownBackColor = value;
          this.owner.Invalidate();
        }
      }
    }

    [Browsable(true), NotifyParentProperty(true), Category("Appearance"), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(typeof(Color), "")]
    public Color MouseOverBackColor
    {
      get
      {
        return this.mouseOverBackColor;
      }
      set
      {
        if(this.mouseOverBackColor != value)
        {
          this.mouseOverBackColor = value;
          this.owner.Invalidate();
        }
      }
    }
  }
}
