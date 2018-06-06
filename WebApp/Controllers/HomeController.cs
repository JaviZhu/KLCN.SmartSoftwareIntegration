using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Basic
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["displayname"])){
                HttpContext.Response.Cookies.Append("displayname", CustomUser.DisplayName);
                HttpContext.Response.Cookies.Append("title", CustomUser.Title);
                //HttpContext.Response.Cookies.Append("photo", CustomUser.ThumbnailPhoto);
            }

            return View();
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
