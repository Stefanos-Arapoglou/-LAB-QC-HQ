/* NOTES
 
Simple HomeController to reroute to Index page when entering the app.
As of now, it only has Index and a bit of Error Handling.

In the future some more actions may be added here.
 
 */


using System.Diagnostics;
using _LAB__QC_HQ.Models;
using Microsoft.AspNetCore.Mvc;

namespace _LAB__QC_HQ.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/
        // app entry point
        public IActionResult Index()
        {
            return View();
        }



        // GET: /Home/Error
        // Error handling action
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
