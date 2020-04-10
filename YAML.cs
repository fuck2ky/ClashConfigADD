using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YamlDotNet.Serialization;

namespace ClashConfig
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
    class YamlTool
    {
        public string _filepath;
        public void setFilePath(string Filepath)
        {
            _filepath = Filepath;
        }
        // 追加写入Yaml文件
        public void Append<T>(T obj)
        {
            StreamWriter yamlWriter = File.AppendText(_filepath);
            Serializer yamlserialzer = new Serializer();
            yamlserialzer.Serialize(yamlWriter, obj);
            yamlWriter.Close();
        }
        public void Change<T>(T obj)
        {
            File.Delete(_filepath);
            StreamWriter yamlWriter = File.CreateText(_filepath);
            Serializer yamlserialzer = new Serializer();
            yamlserialzer.Serialize(yamlWriter, obj);
            yamlWriter.Close();

        }
        // 读取Yaml文件
        public T Read<T>()
        {
            if (!File.Exists(_filepath))
            {
                throw new FileNotFoundException();
            }
            StreamReader yamlReader = File.OpenText(_filepath);
            Deserializer yamlDeserializer = new Deserializer();
            T info = yamlDeserializer.Deserialize<T>(yamlReader);
            yamlReader.Close();
            return info;
        }
    }
    class ConfigTools
    {
        /// <summary>
        /// 定义yamltool实例
        /// </summary>
        private YamlTool yaml = null;
        /// <summary>
        /// 默认文件是同目录下的Profile目录中的config.yaml文件
        /// </summary>
        private string _filename = Directory.GetCurrentDirectory() + @"\Profile\config.yaml";
        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <param name="filename">
        /// 配置文件名
        /// </param>
        public ConfigTools(string filename)
        {
            _filename = filename;
            yaml = new YamlTool();
            yaml.setFilePath(_filename);

        }
        /// <summary>
        /// 读取Config配置文件
        /// </summary>
        /// <returns>
        /// 返回Config类型对象
        /// </returns>
        public Config ReadConfig()
        {
            return yaml.Read<Config>();
        }
        /// <summary>
        /// 删除指定的规则
        /// </summary>
        /// <param name="config">
        /// config对象
        /// </param>
        /// <param name="Rule">
        /// string类型的规则
        /// </param>
        /// <returns>
        /// 删除成功返回True，失败False
        /// </returns>
        public bool DeleteRule(Config config, string Rule)
        {
            try
            {
                config.Rule.Remove(Rule);
                yaml.Change<Config>(config);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 删除指定的网络节点
        /// </summary>
        /// <param name="config">
        /// config对象
        /// </param>
        /// <param name="Nodename">
        /// 节点名称
        /// </param>
        /// <returns>
        /// 删除成功返回True，失败False
        /// </returns>
        public bool DeleteNode(Config config, string Nodename)
        {
            bool flag = true;
            foreach (PROXY node in config.Proxy)
            {
                if (node.name == Nodename)
                {
                    config.Proxy.Remove(node);
                    flag = false;
                }
            }
            if (flag)
            {
                return false;
            }
            try
            {
                yaml.Change<Config>(config);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 添加规则，将规则添加到倒数第三个
        /// </summary>
        /// <param name="config">
        /// config对象
        /// </param>
        /// <param name="Rule">
        /// string类型的规则
        /// </param>
        /// <returns>
        /// 添加成功返回true，失败返回False
        /// </returns>
        public bool AddRule(Config config, string Rule)
        {
            try
            {
                int index = config.Rule.Count - 2;
                config.Rule.Insert(index, Rule);
                yaml.Change(config);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 添加网络节点
        /// </summary>
        /// <param name="config">
        /// config对象
        /// </param>
        /// <param name="Node">
        /// PROXY类型对象
        /// </param>
        /// <returns></returns>
        public bool AddNode(Config config, PROXY Node)
        {
            try
            {
                config.Proxy.Add(Node);
                yaml.Change(config);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 修改规则
        /// </summary>
        /// <param name="config"> config对象</param>
        /// <param name="OldRule">旧规则</param>
        /// <param name="newRule">新规则</param>
        /// <returns>返回成功与否</returns>
        public bool ChangeRule(Config config, string OldRule, string newRule)
        {
            try
            {
                config.Rule[config.Rule.IndexOf(OldRule)] = newRule;
                yaml.Change(config);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 修改节点信息
        /// </summary>
        /// <param name="config"> config对象</param>
        /// <param name="Node">PROXY对象</param>
        /// <returns>返回成功与否</returns>
        public bool ChangeNode(Config config, PROXY Node)
        {
            try
            {
                foreach (PROXY N in config.Proxy)
                {
                    if (N.name == Node.name)
                    {
                        config.Proxy[config.Proxy.IndexOf(N)] = Node;
                    }
                }
                yaml.Change(config);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 添加策略集
        /// </summary>
        /// <param name="config">config对象</param>
        /// <param name="Group">策略集</param>
        /// <returns>成功与否</returns>
        public bool AddProxyGroup(Config config, GROUP Group)
        {
            try
            {
                config.ProxyGroup.Add(Group);
                yaml.Change(config);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
