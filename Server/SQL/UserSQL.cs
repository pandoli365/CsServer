namespace Server.SQL
{
    public class User
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string count { get; set; }
    }

    public class UserSQL : SQL<User>
    {
        public void userInsert(User user)
        {
            string qurry = sqlInsert(user);
        }
    }
}
//쿼리 전송
