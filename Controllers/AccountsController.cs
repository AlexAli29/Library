﻿using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class AccountsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
