using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dof_Login
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public void GetValue(string username, string password, TextBox value1, PasswordBox value2)
        {
            UserName.Text = username;
            Password.Password = password;
        }

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            if (!File.Exists(Utility.GetConfigPath()))
            {
                Stream s = new FileStream(Utility.GetConfigPath(), FileMode.Create);
                UTF8Encoding utf8 = new UTF8Encoding(false);
                StreamWriter streamWriter = new StreamWriter(s, utf8);
                streamWriter.Write("");
                streamWriter.Close();
                s.Close();

                s = new FileStream(System.IO.Path.Combine(Environment.CurrentDirectory, "publickey.pem"), FileMode.Create);
                streamWriter = new StreamWriter(s, utf8);
                streamWriter.Write(RSA.public_key);
                streamWriter.Close();
                s.Close();

                Utility.WriteINI("数据库信息", "DBHost", Config.dbhost);
                Utility.WriteINI("数据库信息", "DBUser", Config.dbuser);
                Utility.WriteINI("数据库信息", "DBPassword", Utility.Encrypt(Config.dbpass));
                Utility.WriteINI("数据库信息", "DBPort", Config.dbport);

                Utility.WriteINI("登录器信息", "GameHost", Config.gamehost);

                Utility.WriteINI("账号信息", "UserName", "");
                Utility.WriteINI("账号信息", "Password", "");
            }
            else
            {
                Config.dbhost = Utility.ReadINI("数据库信息", "DBHost");
                Config.dbuser = Utility.ReadINI("数据库信息", "DBUser");
                Config.dbpass = Utility.Decrypt(Utility.ReadINI("数据库信息", "DBPassword"));
                Config.dbport = Utility.ReadINI("数据库信息", "DBPort");

                Config.gamehost = Utility.ReadINI("登录器信息", "GameHost");

                UserName.Text = Utility.ReadINI("账号信息", "UserName");
                Password.Password = Utility.Decrypt(Utility.ReadINI("账号信息", "Password"));
            }

            Login.mySql = Login.GetMySqlCon();
            try
            {
                Login.mySql.Open();
                if (Login.mySql.State != System.Data.ConnectionState.Open)
                {
                    MessageBox.Show("数据库连接失败，请检查数据库地址！");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("数据库连接失败，请检查数据库地址！");
            }

        }

        /// <summary>
        /// 登录按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserName.Text.Trim()) || string.IsNullOrWhiteSpace(Password.Password.Trim()))
            {
                MessageBox.Show("账号或密码为空！");
                return;
            }

            string arg = Login.LoginGame(UserName.Text.Trim(), Password.Password.Trim());
            if (arg == "Username or Password Error")
            {
                MessageBox.Show("账号或密码错误！");
                UserName.Text = "";
                Password.Password = "";
                return;
            }
            if (arg.Length < 300)
            {
                MessageBox.Show("参数错误！");
                return;
            }

            Utility.WriteINI("账号信息", "UserName", UserName.Text.Trim());
            Utility.WriteINI("账号信息", "Password", Utility.Encrypt(Password.Password.Trim()));

            //if (!ModifyHostsFile(string.Format("\n{0}   start.dnf.tw\n", Config.gamehost)))
            //{
            //    MessageBox.Show("Hosts修改失败，请以管理员方式运行！");
            //    return;
            //}

            // StartProcess(System.IO.Path.Combine(Environment.CurrentDirectory, "dnf.exe"), new string[] { arg });
            if (StartProcess(System.IO.Path.Combine(Environment.CurrentDirectory, "dnf.exe"), new string[] { arg }))
            {
                Application.Current.Shutdown();
            }

        }

        /// <summary>
        /// 注册按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.getTextHandler = GetValue;
            registerWindow.ShowDialog();
        }

        /// <summary>
        /// 修改密码按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyBtn_Click(object sender, RoutedEventArgs e)
        {
            ModifyWindows modifyWindows = new ModifyWindows();
            modifyWindows.getTextHandler = GetValue;
            modifyWindows.ShowDialog();
        }

        /// <summary>
        /// 找回密码按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            BackWindow backWindow = new BackWindow();
            backWindow.getTextHandler = GetValue;
            backWindow.ShowDialog();
        }

        /// <summary>
        /// 配置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow configWindow = new ConfigWindow();
            configWindow.ShowDialog();
        }

        /// <summary>
        /// 以参数运行
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool StartProcess(string filename, string[] args)
        {
            try
            {
                string s = "";
                foreach (string arg in args)
                {
                    s = s + arg + " ";
                }
                s = s.Trim();
                Process myprocess = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo(filename, s);
                myprocess.StartInfo = startInfo;

                //通过以下参数可以控制exe的启动方式，具体参照 myprocess.StartInfo.下面的参数，如以无界面方式启动exe等
                myprocess.StartInfo.UseShellExecute = false;
                myprocess.Start();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动应用程序时出错！原因：" + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// 修改Hosts文件
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static bool ModifyHostsFile(string entry)
        {
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");
            string hostsStr = File.ReadAllText(path);
            Regex regex = new Regex(entry);
            if (regex.IsMatch(hostsStr))
                return true;
            try
            {
                File.SetAttributes(path, File.GetAttributes(path) & (~FileAttributes.ReadOnly));//取消只读
                using (StreamWriter w = File.AppendText(path))
                {
                    w.WriteLine(entry);
                    File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.ReadOnly);//设置只读
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Login.mySql.Close();
        }

        private void LinkHelp_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
        }
    }
}
