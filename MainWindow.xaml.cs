using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace ClashConfigADD
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string filename;
        private static Config YamlConfig;
        private static SerializeObject yaml;
        struct Rule_Struct
        {
            public string method { get; set; }
            public string target { get; set; }
            public string rule { get; set; }
            public Rule_Struct(string v1,string v2,string v3):this()
            {
                this.method = v1;
                this.target = v2;
                this.rule=v3;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                filename = Directory.GetCurrentDirectory() + @"\config.yaml";
                if (File.Exists(filename))
                {
                    yaml = new SerializeObject();
                    yaml.setFilePath(filename.Replace("\\", "/"));
                    YamlConfig = yaml.Deserializer<Config>();
                    if (YamlConfig != null)
                    {
                        foreach (GROUP G in YamlConfig.ProxyGroup)
                        {
                            rulebox.Items.Add(G.name);
                        }
                        foreach(string r in YamlConfig.Rule)
                        {
                            string[] z = r.Split(',');
                            if (z.Length >= 3)
                            {
                                RuleList.Items.Add(new Rule_Struct(z[0], z[1], z[2]));
                            }
                            else
                            {
                                RuleList.Items.Add(new Rule_Struct(z[0], "剩余目标地址", z[1]));
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择配置文件";
            dialog.Filter = "配置文件(*.yaml,*.yml)|*.yaml;*.yml";
            if (dialog.ShowDialog() == true)
            {
                filename = dialog.FileName;
                loadConfig();
            }
        }
        public void loadConfig()
        {
            yaml = new SerializeObject();
            yaml.setFilePath(filename);
            YamlConfig = yaml.Deserializer<Config>();
            if (YamlConfig != null)
            {
                foreach (GROUP G in YamlConfig.ProxyGroup)
                {
                    rulebox.Items.Add(G.name);
                }
                foreach (string r in YamlConfig.Rule)
                {
                    string[] z = r.Split(',');
                    if (z.Length >= 3)
                    {
                        RuleList.Items.Add(new Rule_Struct(z[0], z[1], z[2]));
                    }
                    else
                    {
                        RuleList.Items.Add(new Rule_Struct(z[0], "剩余目标地址", z[1]));
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var method = methodbox.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ","");
            var target = targetbox.Text;
            var rule =rulebox.SelectedItem.ToString();
            StringBuilder content = new StringBuilder(method);
            content.Append(",").Append(target).Append(",").Append(rule);
            var Rule = new[]
            {
                content.ToString()
            };
            yaml.Serializer<object>(Rule);
            RuleList.Items.Add(new Rule_Struct(method, target, rule));
            MessageBox.Show("策略添加成功，重载配置生效", "策略添加成功");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (filename == null)
            {
                MessageBox.Show("请先加载配置", "警告");
                return;
            }
            StringBuilder url = new StringBuilder("http://");
            url.Append(YamlConfig.controller).Append("/configs");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("path", filename);
            string json = JsonConvert.SerializeObject(dic);
            string code = HTTPAPI.HttpRequestPUT(url.ToString(), json).ToString();
            if (code == "请求成功")
            {
                MessageBox.Show("重载配置完成", "成功");
            }
            else
            {
                MessageBox.Show(code, "失败");
            }
        }
    }

}
