using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

     

   


        // GET: Books/AddOrEdit/
        public IActionResult AddOrEdit(int? id)
        {

            BookViewModel model = new BookViewModel();
            if(id >0)
            {
                model = FetchBookByID(id);
            }

            
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

                    book.BookName = stripXSS(book.BookName);
                    book.Author = stripXSS(book.Author);

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


        private string stripXSS(string value)
        {
            value = value.Replace("<", "");
            value = value.Replace(">", "");
            value = value.Replace("/", "");
            return value;
        }



        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
           BookViewModel bookViewModel = FetchBookByID(id);
           return View(bookViewModel);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("BookDelteByID", sqlConnection);                
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("BookID", id);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));

        }

        [NonAction]
        public BookViewModel FetchBookByID(int? id)
        {
            BookViewModel book = new BookViewModel();
           
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlData = new SqlDataAdapter("BookViewByID", sqlConnection);
                sqlData.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlData.SelectCommand.Parameters.AddWithValue("BookID", id);               
                sqlData.Fill(dtbl);
                if(dtbl.Rows.Count == 1)
                {
                    book.BookID = Convert.ToInt32(dtbl.Rows[0]["BookID"].ToString());
                    book.BookName = dtbl.Rows[0]["BookName"].ToString();
                    book.Author = dtbl.Rows[0]["Author"].ToString();
                    book.Price = Convert.ToInt32(dtbl.Rows[0]["Price"].ToString());
                    book.PrintYear = Convert.ToInt32(dtbl.Rows[0]["PrintYear"].ToString());
                    book.Amount = Convert.ToInt32(dtbl.Rows[0]["Amount"].ToString());
                    book.ImagePath = dtbl.Rows[0]["ImagePath"].ToString();                
                    book.Rating = Convert.ToDouble(dtbl.Rows[0]["Rating"].ToString());                  
                }
                  return book;
            }        

        }

       
    }
}

