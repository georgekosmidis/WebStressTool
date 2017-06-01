using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStressTest.Attributes;

namespace WebStressTest.Converters
{
    public class NumericUpDownTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // Attempt to do them all
            return true;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            try
            {
                string Value;
                if (!(value is string))
                {
                    Value = Convert.ChangeType(value, context.PropertyDescriptor.PropertyType).ToString();
                }
                else
                    Value = value as string;
                decimal decVal;
                if (!decimal.TryParse(Value, out decVal))
                    decVal = decimal.One;
                MinMaxAttribute attr = (MinMaxAttribute)context.PropertyDescriptor.Attributes[typeof(MinMaxAttribute)];
                if (attr != null)
                {
                    decVal = attr.PutInRange(decVal);
                }
                return Convert.ChangeType(decVal, context.PropertyDescriptor.PropertyType);
            }
            catch
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            try
            {
                return destinationType == typeof(string)
                   ? Convert.ChangeType(value, context.PropertyDescriptor.PropertyType).ToString()
                   : Convert.ChangeType(value, destinationType);
            }
            catch { }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
