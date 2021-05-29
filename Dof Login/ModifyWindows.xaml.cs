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
    /// ModifyWindows.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyWindows : Window
    {
        public ModifyWindows()
        {
            InitializeComponent();
        }

        private void ModifyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserName.Text.Trim()) || string.IsNullOrWhiteSpace(Password.Password.Trim()) || string.IsNullOrWhiteSpace(NewPassword.Password.Trim()))
            {
                MessageBox.Show("账号或密码为空！");
                return;
            }
            switch (Login.ModifyPassword(UserName.Text.Trim(), Password.Password.Trim(), NewPassword.Password.Trim()))
            {
                case "Modify Success":
                    MessageBox.Show(UserName.Text.Trim() + " 修改密码成功！");
                    break;
                case "Username or Password Error":
                    MessageBox.Show("账号或密码错误！");
                    break;
                case "Modify Fail":
                    MessageBox.Show("修改密码失败！");
                    break;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
