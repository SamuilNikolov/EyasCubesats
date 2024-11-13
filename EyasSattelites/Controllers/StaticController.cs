using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace EyasSattelites.Controllers
{
    public class StaticController : Controller
    {
        // This action serves the HTML file from wwwroot/static/example.html
        [Route("static/html")]
        public IActionResult ShowHtml()
        {
            // Define the path to your HTML file
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/static/3dsatvis.html");

            // Serve the file as an HTML document
            return PhysicalFile(filePath, "text/html");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
