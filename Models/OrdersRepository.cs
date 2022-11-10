using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace Library.Models
{
    public class OrdersRepository
    {
        
        private IConfiguration _config;
        AccountsRepository _accRepository;

        public OrdersRepository(IConfiguration config)
        {
            _config = config;
            _accRepository = new AccountsRepository(_config);
        }


        public List<Order> GetOrdersByUserId(string IdUser)
        {
            
            var orders = new List<Order>();
            string query;
            query = $"Select * from [Order] where IdUser ='{IdUser}'";


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
                        orders.Add(new Order()
                        {
                            IdOrder = Convert.ToInt32(dataReader["IdOrder"]),
                            IdUser = Convert.ToInt32(dataReader["IdUser"]),
                            IdStatus = Convert.ToInt32(dataReader["IdStatus"]),
                            IdBook = Convert.ToInt32(dataReader["IdBook"])
                        });
                    }
                }
                connection.Close();
            }

            return orders;

        }
    }
}
