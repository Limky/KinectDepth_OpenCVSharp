﻿using System;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public void settingServerIP(String serverIP) {
            server_ip = serverIP;
            SEND_URL = "http://" + server_ip + "/api/send";
            Console.WriteLine("settingServerIP is called... setting Server IP = " + SEND_URL);

        }
        byte[] byte1;
        public void sendPosToServer(double x , double y , int longtap)
        {

            string json = "{\"deviceCode\":\"" + deviceCode + "\",\"targetDeviceType\":\"" + targetDeviceType + "\",\"sendData\":\"{\"nettype\":"+ nettype+",\"x\":"+x+",\"y\":"+y+",\"longtap\":" + longtap + "}\"}";
            string sendDataStr = "{\"nettype\":" + nettype + ",\"x\":" + x + ",\"y\":" + y + ",\"longtap\":" + longtap + "}\"";

            Console.WriteLine("sendPosToServer is called... Server IP = " + SEND_URL);
            Console.WriteLine("sendPosToServer is called... Send json = " + json);

          
            
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
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(),Encoding.GetEncoding("UTF-8"));
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



        public XmlDocument loadXMLFromServer(String SERVER_IP_Address)
        {
            LOAD_URL = "http://" + SERVER_IP_Address + "/getXml";

            try
            {

                request = (HttpWebRequest)WebRequest.Create(LOAD_URL);
                request.Method = "POST";
                request.ContentType = "application/json";
                // request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            string responseStr = responseReader.ReadToEnd();
                            String outstr = responseStr.Replace("\"", "'");

                            Console.Out.WriteLine(outstr);

                            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(outstr);
                            return doc;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("--------Server err---------");
                Console.Out.WriteLine(e.Message);

            }

            return null;
        }





    }
}
