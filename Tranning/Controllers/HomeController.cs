using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tranning.DataDBContext;
using Tranning.Models;

namespace Tranning.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TranningDBContext _dbContext;

        public HomeController(ILogger<HomeController> logger, TranningDBContext context)
        {
            _logger = logger;
            _dbContext = context;
        }


        [HttpGet]
        public IActionResult Index(string SearchString)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionUsername")))
            {
                return RedirectToAction(nameof(LoginController.Index), "Login");
            }

            CategoryModel categoryModel = new CategoryModel();
            categoryModel.CategoryDetailLists = new List<CategoryDetail>();


            var data = from m in _dbContext.Categories select m;

            data = data.Where(m => m.deleted_at == null);
            if (!string.IsNullOrEmpty(SearchString))
            {
                data = data.Where(m => m.name.Contains(SearchString));
            }
            var categoryList = data.ToList();

            foreach (var item in categoryList)
            {

                categoryModel.CategoryDetailLists.Add(new CategoryDetail
                {
                    id = item.id,
                    name = item.name,
                    icon = item.icon,
                    description = item.description,
                });
            }
            ViewData["CurrentFilter"] = SearchString;
            return View(categoryModel);
        }

        [HttpGet]
        public IActionResult TopicDetail(int id = 0)
        {
            TopicDetail topic = new TopicDetail();
            var data = _dbContext.Topics.FirstOrDefault();
            topic.id = data.id;
            topic.name = data.name;
            topic.course_id = data.course_id;
            topic.description = data.description;
            topic.videos = data.videos;
            topic.documents = data.documents;
            topic.attach_file = data.attach_file;
            topic.status = data.status;
            return View(topic);
        }

        public IActionResult Privacy()
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