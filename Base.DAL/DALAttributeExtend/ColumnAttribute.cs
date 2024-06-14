using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.DAL.AttributeExtend
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        private string _ColumnName = null;
        public ColumnAttribute(string ColumnName)
        {
            _ColumnName = ColumnName;

        }
        public string GetColName()
        {
            return this._ColumnName;
        }
    }
}
