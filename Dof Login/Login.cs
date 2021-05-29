using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Dof_Login
{
    public static class Login
    {
        public static MySqlConnection mySql = null;

        /// <summary>
        /// 登录游戏
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string LoginGame(string username, string password)
        {
            MySqlCommand comm = GetSqlCommand("select * from d_taiwan.accounts where accountname=@p_user and password=@p_pwd", mySql);
            comm.Parameters.AddWithValue("p_user", username);
            comm.Parameters.AddWithValue("p_pwd", GetMD5(password));
            MySqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                if (reader.HasRows)
                {
                    int uid = reader.GetInt32(0);
                    string data = string.Format("{0}010101010101010101010101010101010101010101010101010101010101010155914510010403030101", uid.ToString("X8"));
                    data = RSACrypto.ByteArrayToHexString(data);
                    string encrypted = RSACrypto.RSAEncryptByPrivateKey(RSACrypto.PrivateKeyDecFun(RSA.private_key), data);

                    reader.Close();
                    comm.Clone();
                    return encrypted;
                }
                else
                {
                    reader.Close();
                    comm.Clone();
                    return "Username or Password Error";
                }
            }
            else
            {
                reader.Close();
                comm.Clone();
                return "Username or Password Error";
            }

        }

        /// <summary>
        /// 注册账号
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="qq"></param>
        /// <returns></returns>
        public static string RegisterUser(string username, string password, string qq)
        {
            MySqlCommand comm = GetSqlCommand("select * from d_taiwan.accounts where accountname=@p_user", mySql);
            comm.Parameters.AddWithValue("p_user", username);
            MySqlDataReader reader = comm.ExecuteReader();
            reader.Read();
            if (!reader.HasRows)
            {
                reader.Close();
                comm.Clone();
                comm = GetSqlCommand("select * from d_taiwan.accounts order by UID desc limit 1", mySql);
                MySqlDataReader readerUID = comm.ExecuteReader();
                readerUID.Read();
                int uid;
                if (!readerUID.HasRows)
                {
                    uid = 18000000;
                }
                else
                {
                    uid = readerUID.GetInt32(0) + 1;
                }

                readerUID.Close();
                comm.Clone();
                comm = GetSqlCommand("insert into d_taiwan.accounts (UID,accountname,password,qq) VALUES (@uid,@username,@password,@qq)", mySql);
                comm.Parameters.AddWithValue("uid", uid);
                comm.Parameters.AddWithValue("username", username);
                comm.Parameters.AddWithValue("password", GetMD5(password));
                comm.Parameters.AddWithValue("qq", qq);
                if (GetInsert(comm) == 1)
                {
                    string currTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    comm.Clone();
                    comm = GetSqlCommand("insert into d_taiwan.limit_create_character (m_id) VALUES (@uid)", mySql);
                    comm.Parameters.AddWithValue("uid", uid);
                    GetInsert(comm);

                    comm.Clone();
                    comm = GetSqlCommand("insert into d_taiwan.member_info (m_id,user_id) VALUES (@uid,@uid)", mySql);
                    comm.Parameters.AddWithValue("uid", uid);
                    GetInsert(comm);

                    comm.Clone();
                    comm = GetSqlCommand("insert into d_taiwan.member_join_info (m_id) VALUES (@uid)", mySql);
                    comm.Parameters.AddWithValue("uid", uid);
                    GetInsert(comm);

                    comm.Clone();
                    comm = GetSqlCommand("insert into d_taiwan.member_miles (m_id) VALUES (@uid)", mySql);
                    comm.Parameters.AddWithValue("uid", uid);
                    GetInsert(comm);

                    comm.Clone();
                    comm = GetSqlCommand("insert into d_taiwan.member_white_account (m_id) VALUES (@uid)", mySql);
                    comm.Parameters.AddWithValue("uid", uid);
                    GetInsert(comm);

                    comm.Clone();
                    comm = GetSqlCommand("insert into taiwan_login.member_login (m_id) VALUES (@uid)", mySql);
                    comm.Parameters.AddWithValue("uid", uid);
                    GetInsert(comm);

                    comm.Clone();
                    comm = GetSqlCommand("insert into taiwan_billing.cash_cera (account,cera,mod_date,reg_date) VALUES (@uid,@regdb,@now,@now)", mySql);
                    comm.Parameters.AddWithValue("uid", uid);
                    comm.Parameters.AddWithValue("regdb", 1000);
                    comm.Parameters.AddWithValue("now", currTime);
                    GetInsert(comm);

                    comm.Clone();
                    comm = GetSqlCommand("insert into taiwan_billing.cash_cera_point (account,cera_point,reg_date,mod_date) VALUES (@uid,@regdd,@now,@now)", mySql);
                    comm.Parameters.AddWithValue("uid", uid);
                    comm.Parameters.AddWithValue("regdd", 0);
                    comm.Parameters.AddWithValue("now", currTime);
                    GetInsert(comm);

                    comm.Clone();

                    return "Registered Success";
                }
                else
                {
                    return "Registered Fail";
                }
            }
            else
            {
                return "Registered Repeat";
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        public static string ModifyPassword(string username, string password,string newpassword)
        {
            MySqlCommand comm = GetSqlCommand("select * from d_taiwan.accounts where accountname=@username and password=@password", mySql);
            comm.Parameters.AddWithValue("username", username);
            comm.Parameters.AddWithValue("password", GetMD5(password));
            MySqlDataReader reader = comm.ExecuteReader();
            reader.Read();
            if (!reader.HasRows)
                return "Username or Password Error";

            reader.Close();
            comm.Clone();
            comm = GetSqlCommand("update d_taiwan.accounts set password=@newpassword where accountname=@username", mySql);
            comm.Parameters.AddWithValue("username", username);
            comm.Parameters.AddWithValue("newpassword", GetMD5(newpassword));
            if (GetUpdate(comm) == 1)
            {
                return "Modify Success";
            }
            else
            {
                return "Modify Fail";
            }
        }

        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="username"></param>
        /// <param name="qq"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string BackPassword(string username, string qq, string password)
        {
            MySqlCommand comm = GetSqlCommand("select * from d_taiwan.accounts where accountname=@username and qq=@qq", mySql);
            comm.Parameters.AddWithValue("username", username);
            comm.Parameters.AddWithValue("qq", qq);
            MySqlDataReader reader = comm.ExecuteReader();
            reader.Read();
            if (!reader.HasRows)
                return "Username or QQ Error";

            reader.Close();
            comm.Clone();
            comm = GetSqlCommand("update d_taiwan.accounts set password=@password where accountname=@username", mySql);
            comm.Parameters.AddWithValue("username", username);
            comm.Parameters.AddWithValue("password", GetMD5(password));
            if (GetUpdate(comm) == 1)
            {
                return "Back Success";
            }
            else
            {
                return "Back Fail";
            }
        }

        /// <summary>
        /// 建立mysql数据库链接
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection GetMySqlCon()
        {
            string mysqlStr = string.Format("Database=test;Data Source={0};User Id={1};Password={2};pooling=false;CharSet=utf8;port=3306", Config.dbhost, Config.dbuser, Config.dbpass);
            MySqlConnection mysql = new MySqlConnection(mysqlStr);
            return mysql;
        }

        /// <summary>
        /// 建立执行命令语句对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="mysql"></param>
        /// <returns></returns>
        public static MySqlCommand GetSqlCommand(string sql, MySqlConnection mysql)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(sql, mysql);
            return mySqlCommand;
        }

        /// <summary>
        /// 查询并获得结果集并遍历
        /// </summary>
        /// <param name="mySqlCommand"></param>
        public static void GetResultset(MySqlCommand mySqlCommand)
        {
            MySqlDataReader reader = mySqlCommand.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("查询失败了！");
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="mySqlCommand"></param>
        public static int GetInsert(MySqlCommand mySqlCommand)
        {
            try
            {
                return mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                Console.WriteLine("插入数据失败了！" + message);
                return 0;
            }
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="mySqlCommand"></param>
        public static int GetUpdate(MySqlCommand mySqlCommand)
        {
            try
            {
                return mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                Console.WriteLine("修改数据失败了！" + message);
                return 0;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="mySqlCommand"></param>
        public static int GetDel(MySqlCommand mySqlCommand)
        {
            try
            {
                return mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                Console.WriteLine("删除数据失败了！" + message);
                return 0;
            }
        }

        /// <summary>
        /// 获取大写的MD5签名结果
        /// </summary>
        /// <param name="encypStr"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string GetMD5(string encypStr, string charset = "UTF-8")
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            try
            {
                inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
            }
            catch (Exception)
            {
                inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
            }
            outputBye = m5.ComputeHash(inputBye);
            m5.Clear();
            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToLower();
            return retStr;
        }



    }
}
