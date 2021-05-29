using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dof_Login
{
    /// <summary>
    /// ConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            DBHost.Text = Config.dbhost;
            DBUser.Text = Config.dbuser;
            DBPass.Text = Config.dbpass;
            GameHost.Text = Config.gamehost;
        }

        private void ModifyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Config.dbhost != DBHost.Text && !string.IsNullOrWhiteSpace(DBHost.Text))
            {
                Config.dbhost = DBHost.Text;
                Utility.WriteINI("数据库信息", "DBHost", Config.dbhost);
            }
            if (Config.dbuser != DBUser.Text && !string.IsNullOrWhiteSpace(DBUser.Text))
            {
                Config.dbuser = DBUser.Text;
                Utility.WriteINI("数据库信息", "DBUser", Config.dbuser);
            }
            if (Config.dbpass != DBPass.Text && !string.IsNullOrWhiteSpace(DBPass.Text))
            {
                Config.dbpass = DBPass.Text;
                Utility.WriteINI("数据库信息", "DBPassword", Config.dbpass);
            }
            if (Config.gamehost != GameHost.Text && !string.IsNullOrWhiteSpace(GameHost.Text))
            {
                Config.gamehost = GameHost.Text;
                Utility.WriteINI("登录器信息", "GameHost", Config.gamehost);
            }

            Login.mySql.Close();
            Login.mySql = Login.GetMySqlCon();
            Login.mySql.Open();
            if (Login.mySql.State != System.Data.ConnectionState.Open)
            {
                MessageBox.Show("数据库连接失败，请检查数据库地址！");
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
