using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcTest.Models;
using MyAccounting.Data;

namespace MvcTest.Controllers
{
    public class HomeController : BaseAuthorizeController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, SqlDBContext context) : base(context)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
