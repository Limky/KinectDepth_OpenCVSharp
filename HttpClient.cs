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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    class HttpClient
    {

        public static string SERVER_IP = "192.168.2.200";
        public static string SERVER_PORT = ":3000";
        public static string SEND_URL = "http://"+ SERVER_IP + SERVER_PORT + "/setXml";
        public static string LOAD_URL = "http://" + SERVER_IP + SERVER_PORT + "/getXml";

        public static HttpClient httpClient = null;
        public static HttpWebRequest request = null;

        public static HttpClient getInstance()
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient();
            }
            return httpClient;
        }

        public void sendXMLToServer()
        {

            try
            {
                XmlTextReader reader = new XmlTextReader(@"./config.xml");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);
                reader.Close();
                string postData = xmlDoc.InnerXml;

                XmlDocument doc = new XmlDocument();

                doc.LoadXml(postData);
                string json = JsonConvert.SerializeXmlNode(doc);
                Console.WriteLine(json);

                byte[] sendData = UTF8Encoding.UTF8.GetBytes(json);
                //  Console.WriteLine(postData);

                request = (HttpWebRequest)WebRequest.Create(SEND_URL);
                request.Method = "POST";

                request.ContentType = "application/json";
                // request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.ContentLength = sendData.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(sendData, 0, sendData.Length);
                requestStream.Close();
          
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            string response = responseReader.ReadToEnd();
                            Console.Out.WriteLine(response);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("--------Server err---------");
                Console.Out.WriteLine(e.Message);
            }

        }



        public XmlDocument loadXMLFromServer() {
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
                            String outstr = responseStr.Replace("\"","'");

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
