using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.DAL.AttributeExtend
{
    [AttributeUsage(AttributeTargets.Property)]

    public class DisplayAttribute:Attribute
    {
        private string _DisplayName = null;
        public DisplayAttribute(string displayName)
        {
            _DisplayName = displayName;

        }
        public string GetDisplayName()
        {
            return this._DisplayName;
        }
    }
}
