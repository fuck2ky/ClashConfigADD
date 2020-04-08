using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClashConfigADD
{
    class HTTPAPI
    {
        public static string HttpRequestPUT(string url,string Putparameter)
        {
            byte[] datas = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(Putparameter);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "PUT";
            request.ContentType = "application/json";
            Stream requestStream = null;
            string responseStr = null;
            try
            { 
                if (datas != null)
                {
                    request.ContentLength = datas.Length;
                    requestStream = request.GetRequestStream();
                    requestStream.Write(datas, 0, datas.Length);
                    requestStream.Close();
                }
                else
                {
                    request.ContentLength = 0;
                }
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    Stream getStream = webResponse.GetResponseStream();
                    byte[] outBytes = ReadFully(getStream);
                    getStream.Close();
                    responseStr = Encoding.UTF8.GetString(outBytes);
                    if (responseStr =="")
                    {
                        responseStr = "请求成功";
                    }
                }
            }
            catch (Exception e)
            {
                responseStr = e.Message;
            }
            finally
            {
                request = null;
                requestStream = null;
            }
            return responseStr;
        }
        public static byte[] ReadFully(Stream stream)
        {
            byte[] buffer = new byte[512];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}
