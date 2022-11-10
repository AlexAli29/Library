using Microsoft.Data.SqlClient;

namespace Library.Models
{
    public class AccountsRepository
    {
        private IConfiguration _config;

        public AccountsRepository(IConfiguration config)
        {
            _config = config;
        }

        public User GetUserByUserName(string UserName)
        {
            string query = $"select * from [User] where UserName='{UserName}'";
            User user = new User();

            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = query;
                SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        user.Id = Convert.ToInt32(dataReader["IdUser"]);
                        user.UserName = dataReader["UserName"].ToString();
                        user.Email = dataReader["Email"].ToString();
                        user.Password = dataReader["Password"].ToString();
                        user.ContactNumber = dataReader["ContactNumber"].ToString();
                        user.Address = dataReader["Address"].ToString();
                        user.RoleId = Convert.ToInt32(dataReader["RoleID"]);

                    }
                }
                connection.Close();
            }
            return user;
        }

        public string GetUserNameById(int IdUser)
        {
            string query = $"Select UserName from [User] where IdUser ='{IdUser}'";
            string UserName = " ";

            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = query;
                SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {

                        UserName = dataReader["UserName"].ToString();


                    }
                }
                connection.Close();
            }
            return UserName;

        }

        public int ExecuteDML(string Query)
        {
            int Result;
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = Query;
                SqlCommand command = new SqlCommand(sql, connection);
                Result = command.ExecuteNonQuery();
                connection.Close();
            }
            return Result;

        }

        public bool UserAlreadyExists( string UserName,string Email)
        {
            string query = $"Select * from [User] where UserName ='{UserName}'" +
               $" OR Email='{Email}'";

            bool flag = false;

            string connectionString = _config["ConnectionStrings:DefaultConnection"];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = query;
                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader rd = command.ExecuteReader();
                if (rd.HasRows)
                {
                    flag = true;
                }
                connection.Close();
            }
            return flag;
        }       

    }
}

