using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace ClashConfig
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string filename;
        private static Config YamlConfig;
        private static ConfigTools tools;
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
                    tools = new ConfigTools(filename.Replace("\\", "/"));
                    YamlConfig = tools.ReadConfig();
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
            tools = new ConfigTools(filename.Replace("\\", "/"));
            YamlConfig = tools.ReadConfig();
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
            if (methodbox.SelectedItem == null)
            {
                MessageTips("请先添加策略", sender, e);
                return;
            }
            var method = methodbox.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ","");
            var target = targetbox.Text;
            var rule =rulebox.SelectedItem.ToString();
            StringBuilder content = new StringBuilder(method);
            content.Append(",").Append(target).Append(",").Append(rule);
            tools.AddRule(YamlConfig, content.ToString());
            RuleList.Items.Add(new Rule_Struct(method, target, rule));
            
            MessageTips("策略添加成功，重载配置生效", sender, e);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (filename == null)
            {
                MessageTips("请先加载配置",sender,e);
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
                MessageTips("重载配置完成", sender, e);
            }
            else
            {
                MessageTips(code, sender, e);
            }
        }
        public async void MessageTips(string message, object sender, RoutedEventArgs e)
        {
            var sampleMessageDialog = new SimpleMessage

            {
                Message = { Text = message }
            };
            await DialogHost.Show(sampleMessageDialog, "RootDialog");
        }
    }

}
