using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace TpControls
{
  internal sealed class ApplicableToButtonAttribute : Attribute
  {
  }

  internal class FlatButtonAppearanceConverter : ExpandableObjectConverter
  {
    // Methods
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if(destinationType == typeof(string))
      {
        return "";
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
    {
      if((context != null) && (context.Instance is Button))
      {
        Attribute[] array = new Attribute[attributes.Length + 1];

        attributes.CopyTo(array, 0);
        array[attributes.Length] = new ApplicableToButtonAttribute();
        attributes = array;
      }
      return TypeDescriptor.GetProperties(value, attributes);
    }
  }
}
