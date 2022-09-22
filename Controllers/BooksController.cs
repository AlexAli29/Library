using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Library.Controllers
{
    public class BooksController : Controller
    {
        private readonly IConfiguration _configuration;

        public BooksController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // GET: Books
        public IActionResult Index()
        {
            DataTable dataTable = new DataTable();
            using(SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlData = new SqlDataAdapter("BookViewAll", sqlConnection);
                sqlData.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlData.Fill(dataTable);
            }
            return View(dataTable);
        }

     

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,BookName,Author,Price,PrintYear,Amount,ImagePath,Rating")] Book book)
        {
           
            return View();
        }

        // GET: Books/AddOrEdit/
        public IActionResult AddOrEdit(int? id)
        {

            BookViewModel model = new BookViewModel();

            
            return View(model);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(int id, [Bind("BookID,BookName,Author,Price,PrintYear,Amount,Rating,Image")] BookViewModel obj)
        {
          if(ModelState.IsValid)
          {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    if (obj.Image != null)
                    {
                        var currentDirectory = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images\thumbnails");
                        var fileName = Guid.NewGuid().ToString() + ".jpg";
                        var destinationPath = Path.Combine(currentDirectory, fileName);
                        using (var stream = System.IO.File.Create(destinationPath))
                        {
                            obj.Image.CopyTo(stream);
                        }

                        obj.ImagePath = @"/images/thumbnails/" + fileName;

                       
                    }

                    var book = obj.CreateBook();

                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("BookAddOrEdit", sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("BookID", book.BookID);
                    sqlCmd.Parameters.AddWithValue("BookName", book.BookName);
                    sqlCmd.Parameters.AddWithValue("Author", book.Author);
                    sqlCmd.Parameters.AddWithValue("Price", book.Price);
                    sqlCmd.Parameters.AddWithValue("PrintYear", book.PrintYear);
                    sqlCmd.Parameters.AddWithValue("Amount", book.Amount);
                    sqlCmd.Parameters.AddWithValue("ImagePath", book.ImagePath != null ? book.ImagePath : book.ImagePath = " ");
                    sqlCmd.Parameters.AddWithValue("Rating", book.Rating);
                    sqlCmd.ExecuteNonQuery();
                    
                }

               
          }
                return RedirectToAction(nameof(Index));
        }

            
    

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
          

            return View();
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            return View();

        }

       
    }
}

