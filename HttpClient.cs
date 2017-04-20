using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;


namespace Microsoft.Samples.Kinect.DepthBasics
{
    class HttpClient
    {

        private static String server_ip = "";

        public static string SEND_URL = "";
        public static string LOAD_URL = null;

        public static HttpClient httpClient = null;
        public static HttpWebRequest request = null;

        private String deviceCode = "SMA-60000";
        private String targetDeviceType = "STEP021";

        private int nettype = 0;

        private String sendData;

        public static HttpClient getInstance()
        {

            if (httpClient == null)
            {
                httpClient = new HttpClient();
            }
            return httpClient;
        }

        public void setting(String serverIP, String targetDevice)
        {
            server_ip = serverIP;
            SEND_URL = "http://" + server_ip + "/api/send";
            targetDeviceType = targetDevice;
            Console.WriteLine("setting Server IP     = " + SEND_URL);
            Console.WriteLine("setting Target Device = " + targetDeviceType);

        }
        byte[] byte1;
        public void sendPosToServer(double x, double y, int longtap)
        {

           // string json = "{\"deviceCode\":\"" + deviceCode + "\",\"targetDeviceType\":\"" + targetDeviceType + "\",\"sendData\":\"{\"nettype\":" + nettype + ",\"x\":" + x + ",\"y\":" + y + ",\"longtap\":" + longtap + "}\"}";
            string sendDataStr = "{\"nettype\":" + nettype + ",\"x\":" + x + ",\"y\":" + y + ",\"longtap\":" + longtap + "}\"";

            Console.WriteLine("sendPosToServer is called... Server IP = " + SEND_URL);
            Console.WriteLine("sendPosToServer is called... Send Data = " + sendDataStr);


            String formData = String.Format("deviceCode={0}&targetDeviceType={1}&sendData={2}", deviceCode, targetDeviceType, sendDataStr);


            byte[] sendData = UTF8Encoding.UTF8.GetBytes(formData);


            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(SEND_URL);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = sendData.Length;

                Stream requestStream = httpWebRequest.GetRequestStream();

                requestStream.Write(sendData, 0, sendData.Length);
                requestStream.Close();



                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                string returnstr = streamReader.ReadToEnd();
                streamReader.Close();
                httpWebResponse.Close();

                Console.Write("return: " + returnstr);


            }
            catch (Exception e)
            {
                Console.Out.WriteLine("--------Server err---------");
                Console.Out.WriteLine(e.ToString());
            }

        }




    }
}
