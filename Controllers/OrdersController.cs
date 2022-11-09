using Library.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class OrdersController : Controller
    {

        private IConfiguration _config;
        AccountsRepository _accRepository;

        public OrdersController(IConfiguration config)
        {
            _config = config;
            _accRepository = new AccountsRepository(_config);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateOrder(int? IdBook)
        {
            string Query = "Insert into [Order](IdUser,IdStatus,IdBook)" +
                $" values('{HttpContext.Session.GetString("IdUser")}','{1}','{IdBook}')";
            int result = _accRepository.ExecuteDML(Query);
            return RedirectToAction("Index", "Books");
        }

        [HttpPost]
        public IActionResult ConfirmOrder(int? IdOrder)
        {
            string Query = $"UPDATE [Order] SET IdStatus='2' WHERE IdOrder='{IdOrder}'";
            int result = _accRepository.ExecuteDML(Query);
            return RedirectToAction("AccountAdmin", "Accounts");
        }

        [HttpPost]
        public IActionResult DenyOrder(int? IdOrder)
        {
            string Query = $"UPDATE [Order] SET IdStatus='3' WHERE IdOrder='{IdOrder}'";
            int result = _accRepository.ExecuteDML(Query);
            return RedirectToAction("AccountAdmin", "Accounts");
        }


    }
}
