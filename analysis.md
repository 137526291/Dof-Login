## 源码分析
通过账户密码，从数据库d_taiwan.accounts查找对应账户 (密码是MD5)
```
select * from d_taiwan.accounts where accountname=@p_user and password=@p_pwd
```
取账户uid 添加后缀（41Byte）， 做RSA（此处用到登陆器私钥, 对应公钥需放server里）返回加密后的字符串

2.更改hosts文件将域名start.dnf.tw 定向到IP（如192.168.200.131）须admin权限
3.使用startprocess启动dxf.exe 入参为加密后的字符串作为连接凭据

