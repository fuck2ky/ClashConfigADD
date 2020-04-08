using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YamlDotNet.Serialization;

namespace ClashConfigADD
{
    class Config
    {
        public int port { get; set; }
        [YamlMember(Alias = "socks-port", ApplyNamingConventions = false)]
        public int SocksPort { get; set; }
        [YamlMember(Alias = "allow-lan", ApplyNamingConventions = false)]
        public Boolean allowLan { get; set; }
        public string mode { get; set; }
        [YamlMember(Alias = "log-level", ApplyNamingConventions = false)]
        public string logLevel { get; set; }
        [YamlMember(Alias = "external-controller", ApplyNamingConventions = false)]
        public string controller { get; set; }
        [YamlMember(Alias = "external-ui", ApplyNamingConventions = false)]
        public string ui { get; set; }
        public DNS dns { get; set; }
        [YamlMember(Alias = "cfw-bypass", ApplyNamingConventions = false)]
        public List<string> cfwbypass { get; set; }
        [YamlMember(Alias = "cfw-latency-timeout", ApplyNamingConventions = false)]
        public int cfwlatencytimeout { get; set; }
        [YamlMember(Alias = "cfw-latency-url", ApplyNamingConventions = false)]
        public string cfwlatencyurl { get; set; }
        [YamlMember(Alias = "cfw-conn-break-strategy", ApplyNamingConventions = false)]
        public Strategy cfwstrategy { get; set; }
        [YamlMember(Alias = "cfw-child-process", ApplyNamingConventions = false)]
        public List<string> cfwprocess { get; set; }
        public List<PROXY> Proxy { get; set; }
        [YamlMember(Alias = "Proxy Group", ApplyNamingConventions = false)]
        public List<GROUP> ProxyGroup { get; set; }
        public List<string> Rule { get; set; }

    }
    class DNS
    {
        public bool enable { get; set; }
        public string listen { get; set; }
        [YamlMember(Alias = "enhanced-mode", ApplyNamingConventions = false)]
        public string enhancedMode { get; set; }
        public List<string> nameserver { get; set; }
    }
    class Strategy
    {
        public string proxy { get; set; }
        public bool profile { get; set; }
        public bool mode { get; set; }
    }
    class PROXY
    {
        public string name { get; set; }
        public string type { get; set; }
        public string server { get; set; }
        public int port { get; set; }
        public string cipher { get; set; }
        public string password { get; set; }
        public bool udp { set; get; }
        public string uuid { get; set; }
        public int alterId { get; set; }
        public bool tls { get; set; }
        public string network { get; set; }
        [YamlMember(Alias = "ws-path", ApplyNamingConventions = false)]
        public string wspath { get; set; }
        [YamlMember(Alias = "ws-headers", ApplyNamingConventions = false)]
        public headers wsheaders { get; set; }
        [YamlMember(Alias = "skip-cert-verify", ApplyNamingConventions = false)]
        public bool skipcert { get; set; }
    }
    class headers
    {
        public string Host { get; set; }
    }
    class GROUP
    {
        public string name { get; set; }
        public string type { get; set; }
        public List<String> proxies { get; set; }
        public string url { get; set; }
        public int interval { get; set; }
    }
    //读写Yaml文件类
    class SerializeObject
    {
        public String _filepath = Directory.GetCurrentDirectory() + @"\config.yaml";

        public void setFilePath(String Filepath)
        {
            _filepath = Filepath;
        }
        // 追加写入Yaml文件
        public void Serializer<T>(T obj)
        {
            StreamWriter yamlWriter = File.AppendText(_filepath);
            Serializer yamlserialzer = new Serializer();
            yamlserialzer.Serialize(yamlWriter, obj);
            yamlWriter.Close();
        }
        // 读取Yaml文件
        public T Deserializer<T>()
        {
            if (!File.Exists(_filepath))
            {
                MessageBox.Show("未能加载配置文件！", "错误");
            }
            StreamReader yamlReader = File.OpenText(_filepath);
            Deserializer yamlDeserializer = new Deserializer();
            T info = yamlDeserializer.Deserialize<T>(yamlReader);
            yamlReader.Close();
            return info;
        }
    }
}
