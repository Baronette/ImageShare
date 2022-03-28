using March23.Data;
using March23.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace March23.Web.Controllers
{
    public class ImagesController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Images; Integrated Security=true;";

        public IActionResult ViewImage(int id)
        {
            var ids = HttpContext.Session.Get<List<int>>("ids");
            if (ids == null || !ids.Contains(id))
            {
                return Redirect($"/images/verify?id={id}");
            }
            ImageRepository repo = new(_connectionString);
            Image image = repo.GetImageById(id);
            image.Views++;
            repo.UpdateCount(id);
            return View(image);
        }
        public IActionResult Verify(int id)
        {
            var ids = HttpContext.Session.Get<List<int>>("ids");
            if (ids != null && ids.Contains(id))
            {
                return Redirect($"/images/viewimage?id={id}");
            }
            return View(new VerifyViewModel
            {
                Id = id,
                Message = (string)TempData["Message"]
            });
        }
        [HttpPost]
        public IActionResult Verify(int id, string password)
        {
            ImageRepository repo = new(_connectionString);
            Image image = repo.GetImageById(id);
            if (password == image.Password)
            {
                var ids = HttpContext.Session.Get<List<int>>("ids");
                if (ids == null)
                {
                    ids = new List<int>();
                }
                ids.Add(id);
                HttpContext.Session.Set<List<int>>("ids", ids);
                return Redirect($"/images/viewimage?id={id}");
            }
            TempData["Message"] = "Invalid password. Please try again.";
            return Redirect($"/images/verify?id={id}");
        }
    }
}
public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
        string value = session.GetString(key);
        return value == null ? default(T) :
            JsonConvert.DeserializeObject<T>(value);
    }
}
