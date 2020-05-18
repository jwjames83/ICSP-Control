using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace TpControls
{
  [TypeConverter(typeof(PropertySorter))]
  public class State : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private Color mBorderColor;
    private int? mBorderSize = 1;
    private Color mFillColor;
    private Color mTextColor;
    private Image mBitmap;
    private ContentAlignment? mBitmapJustification;
    private Font mFont;
    private string mText;
    private ContentAlignment? mTextJustification;

    public State()
    {
      Font = Control.DefaultFont;

      BorderSize = 1;

      BorderColor = SystemColors.ControlDark;

      FillColor = SystemColors.Control;

      BitmapJustification = ContentAlignment.MiddleCenter;

      mText = string.Empty;

      TextJustification = ContentAlignment.MiddleCenter;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
      if(EqualityComparer<T>.Default.Equals(field, value))
        return false;

      field = value;

      OnPropertyChanged(propertyName);

      return true;
    }
    
    public virtual void ResetFont()
    {
      Font = Control.DefaultFont;
    }

    private bool ShouldSerializeFont()
    {
      return !Font.Equals(Control.DefaultFont);
    }

    [DefaultValue(1)]
    [RefreshProperties(RefreshProperties.All)]
    public int? BorderSize
    {
      get
      {
        return mBorderSize;
      }
      set
      {
        if(value < 0)
          throw new ArgumentOutOfRangeException("BorderSize");

        if(value == null)
          return;

        SetField(ref mBorderSize, value);
      }
    }

    [PropertyOrder(2)]
    [DefaultValue(typeof(Color), "ControlDark")]
    [RefreshProperties(RefreshProperties.All)]
    public Color BorderColor
    {
      get
      {
        return mBorderColor;
      }
      set
      {
        SetField(ref mBorderColor, value);
      }
    }
    
    [PropertyOrder(3)]
    [DefaultValue(typeof(Color), "Control")]
    [RefreshProperties(RefreshProperties.All)]
    public Color FillColor
    {
      get
      {
        return mFillColor;
      }
      set
      {
        SetField(ref mFillColor, value);
      }
    }

    [PropertyOrder(4)]
    [DefaultValue(typeof(Color), "ControlText")]
    [RefreshProperties(RefreshProperties.All)]
    public Color TextColor
    {
      get
      {
        return mTextColor;
      }
      set
      {
        SetField(ref mTextColor, value);
      }
    }

    [PropertyOrder(5)]
    [DefaultValue(null)]
    [RefreshProperties(RefreshProperties.All)]
    public Image Bitmap
    {
      get
      {
        return mBitmap;
      }
      set
      {
        SetField(ref mBitmap, value);
      }
    }

    [PropertyOrder(6)]
    [DefaultValue(ContentAlignment.MiddleCenter)]
    [RefreshProperties(RefreshProperties.All)]
    public ContentAlignment? BitmapJustification
    {
      get
      {
        return mBitmapJustification;
      }
      set
      {
        SetField(ref mBitmapJustification, value);
      }
    }
    
    [PropertyOrder(7)]
    [RefreshProperties(RefreshProperties.All)]
    public Font Font
    {
      get
      {
        return mFont;
      }
      set
      {
        SetField(ref mFont, value);
      }
    }

    [PropertyOrder(8)]
    [DefaultValue("")]
    [RefreshProperties(RefreshProperties.All)]
    public string Text
    {
      get
      {
        return mText;
      }
      set
      {
        SetField(ref mText, value);
      }
    }

    [PropertyOrder(9)]
    [DefaultValue(ContentAlignment.MiddleCenter)]
    [RefreshProperties(RefreshProperties.All)]
    public ContentAlignment? TextJustification
    {
      get
      {
        return mTextJustification;
      }
      set
      {
        SetField(ref mTextJustification, value);
      }
    }

    internal void SetBorderColor(Color color)
    {
      mBorderColor = color;
    }

    internal void SetBorderSize(int? size)
    {
      mBorderSize = size;
    }

    internal void SetFillColor(Color color)
    {
      mFillColor = color;
    }

    internal void SetTextColor(Color color)
    {
      mTextColor = color;
    }
    internal void SetBitmap(Image bitmap)
    {
      mBitmap = bitmap;
    }

    internal void SetBitmapJustification(ContentAlignment? alignment)
    {
      mBitmapJustification = alignment;
    }

    internal void SetFont(Font font)
    {
      mFont = font;
    }

    internal void SetText(string text)
    {
      mText = text;
    }
    internal void SetTextJustification(ContentAlignment? alignment)
    {
      mTextJustification = alignment;
    }
  }
}
