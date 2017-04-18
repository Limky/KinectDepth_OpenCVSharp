using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    class sendData
    {
        private int nettype; 
        private double x; 
        private double y;
        private int longtap; 


        public void setNettype(int param) {
            nettype = param;
        }

        public void setXY(double xx, double yy)
        {
            x = xx;
            y = yy;
        }
        public void setLongtab(int param)
        {
            longtap = param;
        
        }
        
    }
}
