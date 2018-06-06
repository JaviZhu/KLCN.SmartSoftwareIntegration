using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Basic
    {
        public IActionResult Index()
        {
            try
            {
                FindActiveDictionay(User.Identity.Name);
            }
            catch
            {
                customUser.DisplayName = User.Identity.Name;
                customUser.Title = string.Empty;
            }
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["displayname"]))
            {
                HttpContext.Response.Cookies.Append("displayname", customUser.DisplayName);
                HttpContext.Response.Cookies.Append("title", customUser.Title);
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
