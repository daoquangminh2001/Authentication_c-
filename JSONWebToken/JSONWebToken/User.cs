namespace JSONWebToken
{
    public class User
    {
        public string UserName { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }  //chuỗi băm mật khẩu
        public byte[] PasswordSalt { get; set; } //đây chỉ là một chuỗi ngẫu nhiên được tạo ra cho mỗi chuỗi băm
    }
}
