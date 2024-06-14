using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GJJ.Model.Resault
{
    public class DetectedObject
    {
        public DetectedObject(string Path) 
        {
   
        }

        public HObject hObject { get; set; }//获取图片对象

        public Dictionary<int,RegionObject> regions { get; set; }//产品信息

        
        public void GetImage()
        {
            switch (0)
            {
                case 0:
                    {
                        GetImageByTif()
                        break;
                    }
            }
               
        }

        public void GetImageInfo()
        {
            switch (0)
            {
                case 0:
                    {
                        GetImageInfoByTif();
                        break;
                    }
            }
        }

        #region Tif
        private void GetImageByTif()
        {

        }
        private void GetImageInfoByTif()
        {

        }
        #endregion

    }
}
