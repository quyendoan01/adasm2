using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Tranning.DataDBContext;
using Tranning.Models;
using static Tranning.Models.Trainee_courseModel;

namespace Tranning.Controllers
{
    public class AssignController : Controller
    {
        private readonly TranningDBContext _dbContext;
        public AssignController(TranningDBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            Trainee_courseModel trainee_courseModel = new Trainee_courseModel();
            trainee_courseModel.Trainee_courseDetailLists = new List<Trainee_courseModelDetail>();

            var data = from m in _dbContext.Trainee_courses select m;

            data = data.Where(m => m.deleted_at == null);
            var dataList = data.ToList();

            foreach (var item in dataList)
            {
                var dataT = _dbContext.Users.Where(m => m.id == item.trainee_id && m.deleted_at == null).FirstOrDefault();
                var dataC = _dbContext.Courses.Where(m => m.id == item.course_id && m.deleted_at == null).FirstOrDefault();

                trainee_courseModel.Trainee_courseDetailLists.Add(new Trainee_courseModelDetail
                {
                    trainee_id = item.trainee_id,
                    traineeName = dataT.full_name,
                    course_id = item.course_id,
                    courseName = dataC.name,
                    status = item.status,
                    created_at = item.created_at,
                    updated_at = item.updated_at
                });
            }
            return View(trainee_courseModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
            // check dang nhap
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionUsername")))
            //{
            //    return RedirectToAction(nameof(LoginController.Index), "Login");
            //}

            Trainee_courseModelDetail tcourse = new Trainee_courseModelDetail();
            var courseList = _dbContext.Courses
                .Where(m => m.deleted_at == null)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.name }).ToList();
            ViewBag.Stores = courseList;
            var userList = _dbContext.Users
                .Where(m => m.deleted_at == null && m.role_id == 3)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.full_name }).ToList();
            ViewBag.Users = userList;
            return View(tcourse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Trainee_courseModelDetail tcourse)
        {
            if (ModelState.IsValid)
            {
                //try
                //{
                    var tcourseData = new Trainee_course()
                    {
                        trainee_id = tcourse.trainee_id,
                        course_id = tcourse.course_id,
                        status = tcourse.status,
                        created_at = DateTime.Now,
                    };
                    _dbContext.Trainee_courses.Add(tcourseData);
                    _dbContext.SaveChanges(true);
                    TempData["saveStatus"] = true;
                //}
                //catch
                //{
                //    TempData["saveStatus"] = false;
                //}
                return RedirectToAction(nameof(AssignController.Index), "Assign");
            }
            return View(tcourse);
        }

        //[HttpGet]
        //public IActionResult Update(int id = 0)
        //{
        //    CategoryDetail category = new CategoryDetail();
        //    var data = _dbContext.Categories.Where(m => m.id == id).FirstOrDefault();
        //    if (data != null)
        //    {
        //        category.id = data.id;
        //        category.name = data.name;
        //        category.description = data.description;
        //        category.icon = data.icon;
        //        category.status = data.status;
        //    }

        //    return View(category);
        //}

        //[HttpPost]
        //public IActionResult Update(CategoryDetail category, IFormFile file)
        //{
        //    try
        //    {

        //        var data = _dbContext.Categories.Where(m => m.id == category.id).FirstOrDefault();
        //        string uniqueIconAvatar = "";
        //        if (category.Photo != null)
        //        {
        //            uniqueIconAvatar = uniqueIconAvatar = UploadFile(category.Photo);
        //        }

        //        if (data != null)
        //        {
        //            // gan lai du lieu trong db bang du lieu tu form model gui len
        //            data.name = category.name;
        //            data.description = category.description;
        //            data.status = category.status;
        //            if (!string.IsNullOrEmpty(uniqueIconAvatar))
        //            {
        //                data.icon = uniqueIconAvatar;
        //            }
        //            data.updated_at = DateTime.Now;
        //            _dbContext.SaveChanges(true);
        //            TempData["UpdateStatus"] = true;
        //        }
        //        else
        //        {
        //            TempData["UpdateStatus"] = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["UpdateStatus"] = false;
        //    }
        //    return RedirectToAction(nameof(CategoryController.Index), "Category");
        //}

        //[HttpGet]
        //public IActionResult Delete(int id = 0)
        //{
        //    try
        //    {
        //        var data = _dbContext.Categories.Where(m => m.id == id).FirstOrDefault();
        //        if (data != null)
        //        {
        //            data.deleted_at = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //            _dbContext.SaveChanges(true);
        //            TempData["DeleteStatus"] = true;
        //        }
        //        else
        //        {
        //            TempData["DeleteStatus"] = false;
        //        }
        //    }
        //    catch
        //    {
        //        TempData["DeleteStatus"] = false;
        //    }
        //    return RedirectToAction(nameof(CategoryController.Index), "Category");
        //}
    }
}
