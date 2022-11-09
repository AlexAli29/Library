using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Library.Controllers
{
    public class AccountsController : Controller
    {
        private IConfiguration _config;
        AccountsRepository _accRepository;

        public AccountsController(IConfiguration config)
        {
            _config = config;
            _accRepository = new AccountsRepository(_config);
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Account", "Accounts");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            

            bool userExists = _accRepository.UserAlreadyExists(user.UserName, user.Email);
            if(userExists ==true)
            {
                ViewBag.Error = "UserName and Email Already Exists";
                return RedirectToAction("Login","Accounts");
            }

            string Query = "Insert into [User](UserName,Email,Password,ContactNumber," +
                $"Address,RoleID) values('{user.UserName}','{user.Email}','{passwordHash}'" +
                $",'{user.ContactNumber}','{user.Address}','{2}')";

            int result = _accRepository.ExecuteDML(Query);
            
            User userFromDB = _accRepository.GetUserByUserName(user.UserName);
            if(result > 0)
            {
                HttpContext.Session.SetString("IdUser", userFromDB.Id.ToString());
                HttpContext.Session.SetString("UserName", userFromDB.UserName);
                HttpContext.Session.SetString("Email", userFromDB.Email);
                HttpContext.Session.SetString("ContactNumber", userFromDB.ContactNumber);
                HttpContext.Session.SetString("Address", userFromDB.Address);
                HttpContext.Session.SetString("RoleId", userFromDB.RoleId.ToString());
                return RedirectToAction("Index","Books");
            }

            return View();  
        }

       

        [HttpGet]
        public IActionResult Login()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Account", "Accounts");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if(string.IsNullOrEmpty(user.UserName) && string.IsNullOrEmpty(user.Password))
            {
                ViewBag.ErrorMessage = "User and Password Empty";
                return View();
            }
            else
            {
                bool Isfind = UserSignIn(user.UserName, user.Password);
                if(Isfind == true)
                {                    
                    return RedirectToAction("Index", "Books");
                }
                return View();
            }
        }

        private bool UserSignIn(string userName,string password)
        {            
            bool flag = false;           
            var userDetails = _accRepository.GetUserByUserName(userName);            
             
            if (userDetails.UserName != null && BCrypt.Net.BCrypt.Verify(password, userDetails.Password))
            {
                flag = true;
                HttpContext.Session.SetString("IdUser", userDetails.Id.ToString());
                HttpContext.Session.SetString("UserName", userDetails.UserName);
                HttpContext.Session.SetString("Email", userDetails.Email);
                HttpContext.Session.SetString("ContactNumber", userDetails.ContactNumber);
                HttpContext.Session.SetString("Address", userDetails.Address);
                HttpContext.Session.SetString("RoleId", userDetails.RoleId.ToString());

            }
            else
            {
                ViewBag.Error = "UserName or Password wrong";
            }
            return flag;
        }

        [HttpGet]
        public IActionResult Account()
        {
            var orders = new List<Order>();
            var ordersView = new List<OrderViewModel>();
            string query;            
            query = $"Select * from [Order] where IdUser ='{HttpContext.Session.GetString("IdUser")}'";           
                   

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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var model in orders)
                {

                    string sql2 = $"Select * from [Book] where BookID ='{model.IdBook}'";
                    SqlCommand command1 = new SqlCommand(sql2, connection);
                    using (SqlDataReader dataReader = command1.ExecuteReader())
                    {


                        while (dataReader.Read())
                        {
                            ordersView.Add(new OrderViewModel()
                            {
                                IdStatus = model.IdStatus,                               
                                BookName = dataReader.GetString("BookName"),
                                Author = dataReader.GetString("Author"),
                                Price = dataReader.GetDouble("Price"),
                                PrintYear = dataReader.GetInt32("PrintYear"),                                
                                ImagePath = dataReader.GetString("ImagePath")                                
                            });
                        }
                    }

                }
                connection.Close();
            }                
            
            return View(ordersView);
        }


        [HttpGet]
        public IActionResult AccountAdmin()
        {
            var orders = new List<Order>();
            var ordersView = new List<OrderViewModelAdmin>();
            string query;
            query = $"Select * from [Order]";           

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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var model in orders)
                {

                    string sql2 = $"Select * from [Book] where BookID ='{model.IdBook}'";                    
                    SqlCommand command1 = new SqlCommand(sql2, connection);
                    using (SqlDataReader dataReader = command1.ExecuteReader())
                    {


                        while (dataReader.Read())
                        {
                            ordersView.Add(new OrderViewModelAdmin()
                            {
                                IdOrder = model.IdOrder,
                                IdStatus = model.IdStatus,
                                BookName = dataReader.GetString("BookName"),
                                Author = dataReader.GetString("Author"),
                                Price = dataReader.GetDouble("Price"),
                                PrintYear = dataReader.GetInt32("PrintYear"),
                                ImagePath = dataReader.GetString("ImagePath"),
                                UserName = _accRepository.GetUserNameById(model.IdUser)
                            });
                        }
                    }

                }
                connection.Close();
            }

            return View(ordersView);
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Books");
        }

    }
}
