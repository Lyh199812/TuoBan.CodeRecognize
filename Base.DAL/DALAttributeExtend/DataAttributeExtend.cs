using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BS.DAL.AttributeExtend
{
    public static class DataAttributeExtend
    {
        public static string GetDisplayName(this PropertyInfo property)
        {
            if (property.IsDefined(typeof(DisplayAttribute),true))
            {
                DisplayAttribute attribute = (DisplayAttribute)property.GetCustomAttribute(typeof(DisplayAttribute), true);
                return attribute.GetDisplayName();
            }
            else
            {
                return property.Name;
            }


        }

        public static string GetColumnName(this PropertyInfo property)
        {
            if (property.IsDefined(typeof(ColumnAttribute), true))
            {
                ColumnAttribute attribute = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute), true);
                return attribute.GetColName();
            }
            else
            {
                return property.Name;
            }


        }

    }
}
