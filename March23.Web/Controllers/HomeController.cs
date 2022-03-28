using March23.Data;
using March23.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace March23.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Images; Integrated Security=true;";

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(string password, IFormFile file)
        {
            string fileName = $"{Guid.NewGuid()}-{file.FileName}";
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var stream = new FileStream(filePath, FileMode.CreateNew);
            file.CopyTo(stream);
            ImageRepository repo = new ImageRepository(_connectionString);
            int id = repo.Upload(new Image
            {
                FileName = fileName,
                Password = password
            });
            return View(new UploadMessageViewModel { 
                Id = id,
                Password = password
            });
        }


      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
