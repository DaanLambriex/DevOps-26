using Microsoft.AspNetCore.Mvc;

namespace GithubManager.Controllers
{
    public class WeatherForecastController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
