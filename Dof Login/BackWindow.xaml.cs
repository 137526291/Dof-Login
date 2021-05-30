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
    /// BackWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BackWindow : Window
    {
        public delegate void GetTextHandler(string username, string password, TextBox value1, PasswordBox value2);  //声明委托
        public GetTextHandler getTextHandler;

        public BackWindow()
        {
            InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserName.Text.Trim()) || string.IsNullOrWhiteSpace(Password.Password.Trim()) || string.IsNullOrWhiteSpace(QQ.Text.Trim()))
            {
                MessageBox.Show("账号或密码为空！");
                return;
            }
            if (Password.Password.Trim().Length < 6)
            {
                MessageBox.Show("密码不能小于6位！");
                return;
            }

            switch (Login.BackPassword(UserName.Text.Trim(), QQ.Text.Trim(), Password.Password.Trim()))
            {
                case "Back Success":
                    MessageBox.Show(UserName.Text.Trim() + " 找回密码成功！");
                    getTextHandler(UserName.Text.Trim(), Password.Password.Trim(), UserName, Password);
                    Close();
                    break;
                case "Username or QQ Error":
                    MessageBox.Show("账号或QQ错误！");
                    break;
                case "Back Fail":
                    MessageBox.Show("找回密码失败！");
                    break;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
