﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI
{
    internal class TableColumnConverter : ColumnConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return context.Instance is ITabularTableObject || context.Instance is ITabularNamedObject[];
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.Instance is ITabularNamedObject[])
            {
                var cols = (context.Instance as ITabularNamedObject[]).Cast<Column>();
                return new StandardValuesCollection(Table(context).Columns.Where(c => !cols.Contains(c)).OrderBy(c => c.Name).ToList());
            }
            else
            {
                var col = (context.Instance as Column);
                return new StandardValuesCollection(Table(context).Columns.Where(c => c != col).OrderBy(c => c.Name).ToList());
            }
        }
    }

    internal class ColumnConverter: TypeConverter
    {
        protected Table Table(ITypeDescriptorContext context)
        {
            if (context.Instance is ITabularTableObject) return (context.Instance as ITabularTableObject).Table;
            if (context.Instance is ITabularNamedObject[]) return ((context.Instance as ITabularNamedObject[]).First() as Column)?.Table;
            if (context.Instance is object[] objectArray) return (objectArray.First() as Column)?.Table;
            return null;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                var name = (string)value;
                if (string.IsNullOrEmpty(name)) return null;

                var table = Table(context);
                if (!table.Columns.Contains(name)) throw new ArgumentException(string.Format("The table does not contain a column named \"{0}\"", name), context.PropertyDescriptor.Name);

                return table.Columns[name];
            }
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return (value as Column)?.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
