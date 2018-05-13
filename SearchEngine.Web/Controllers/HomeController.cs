using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SearchEngine.Miscellaneous;
using SearchEngine.Web.Models;

namespace SearchEngine.Web.Controllers
{
  public class HomeController : Controller
  {
    private readonly ISearchService searchService;

    public HomeController(ISearchService searchService)
    {
      this.searchService = searchService;
    }

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult ExecuteVectorialSearch(string input)
    {
      var result = searchService.ExecuteVectorialSearch(input);
      return Json(result);
    }

    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
