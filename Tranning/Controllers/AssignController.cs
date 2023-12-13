using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
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
                    id = item.id,
                    trainee_id = item.trainee_id,
                    traineeName = dataT.full_name,
                    course_id = item.course_id,
                    courseName = dataC.name,
                    status = item.status,
                    created_at = item.created_at,
                    updated_at = item.updated_at
                });
            }
            Trainer_topicModel tt = new Trainer_topicModel();
            tt.Trainer_topicDetailLists = new List<Trainer_topicModelDetail>();

            var datatt = from m in _dbContext.Trainer_topics select m;

            datatt = datatt.Where(m => m.deleted_at == null);
            var datattList = datatt.ToList();

            foreach (var item in datattList)
            {
                var dataTr = _dbContext.Users.Where(m => m.id == item.trainer_id && m.deleted_at == null).FirstOrDefault();
                var dataTo = _dbContext.Topics.Where(m => m.id == item.topic_id && m.deleted_at == null).FirstOrDefault();

                tt.Trainer_topicDetailLists.Add(new Trainer_topicModelDetail
                {
                    id = item.id,
                    trainer_id = item.trainer_id,
                    trainerName = dataTr.full_name,
                    topic_id = item.topic_id,
                    topicName = dataTo.name,
                    status = item.status,
                    created_at = item.created_at,
                    updated_at = item.updated_at
                });
            };
            var tctt = new tctt
            {
                tc = trainee_courseModel,
                tt = tt,
            };
            return View(tctt);
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

        [HttpGet]
        public IActionResult Update(int id = 0)
        {
            var courseList = _dbContext.Courses
               .Where(m => m.deleted_at == null)
               .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.name }).ToList();
            ViewBag.Stores = courseList;
            var userList = _dbContext.Users
                .Where(m => m.deleted_at == null && m.role_id == 3)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.full_name }).ToList();
            ViewBag.Users = userList;
            Trainee_courseModelDetail tc = new Trainee_courseModelDetail();
            var data = _dbContext.Trainee_courses.Where(m => m.id == id).FirstOrDefault();
            if (data != null)
            {
                tc.id = data.id;
                tc.trainee_id = data.trainee_id;
                tc.course_id = data.course_id;
                tc.status = data.status;
            }

            return View(tc);
        }

        [HttpPost]
        public IActionResult Update(Trainee_courseModelDetail tc)
        {
            try
            {
                var data = _dbContext.Trainee_courses.Where(m => m.id == tc.id).FirstOrDefault();
                if (data != null)
                {
                    data.trainee_id = tc.trainee_id;
                    data.course_id = tc.course_id;
                    data.status = tc.status;
                    data.updated_at = DateTime.Now;
                    _dbContext.SaveChanges(true);
                    TempData["UpdateStatus"] = true;
                }
                else
                {
                    TempData["UpdateStatus"] = false;
                }
            }
            catch (Exception ex)
            {
                TempData["UpdateStatus"] = false;
            }
            return RedirectToAction(nameof(AssignController.Index), "Assign");
        }

        [HttpGet]
        public IActionResult Delete(int id = 0)
        {
            try
            {
                var data = _dbContext.Trainee_courses.Where(m => m.id == id).FirstOrDefault();
                if (data != null)
                {
                    data.deleted_at = DateTime.Now;
                    _dbContext.SaveChanges(true);
                    TempData["DeleteStatus"] = true;
                }
                else
                {
                    TempData["DeleteStatus"] = false;
                }
            }
            catch
            {
                TempData["DeleteStatus"] = false;
            }
            return RedirectToAction(nameof(AssignController.Index), "Assign");
        }

        [HttpGet]
        public IActionResult Add2()
        {
            // check dang nhap
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionUsername")))
            //{
            //    return RedirectToAction(nameof(LoginController.Index), "Login");
            //}

            Trainer_topicModelDetail tt = new Trainer_topicModelDetail();
            var topicList = _dbContext.Topics
                .Where(m => m.deleted_at == null)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.name }).ToList();
            ViewBag.Stores = topicList;
            var userList = _dbContext.Users
                .Where(m => m.deleted_at == null && m.role_id == 2)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.full_name }).ToList();
            ViewBag.Users = userList;
            return View(tt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add2(Trainer_topicModelDetail tt)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var ttData = new Trainer_topic()
                {
                    trainer_id = tt.trainer_id,
                    topic_id = tt.topic_id,
                    status = tt.status,
                    created_at = DateTime.Now,
                };
                _dbContext.Trainer_topics.Add(ttData);
                _dbContext.SaveChanges(true);
                TempData["saveStatus2"] = true;
            }
                catch
                {
                TempData["saveStatus2"] = false;
            }
            return RedirectToAction(nameof(AssignController.Index), "Assign");
            }
            return View(tt);
        }

        [HttpGet]
        public IActionResult Update2(int id = 0)
        {
            Trainer_topicModelDetail tt = new Trainer_topicModelDetail();
            var topicList = _dbContext.Topics
                .Where(m => m.deleted_at == null)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.name }).ToList();
            ViewBag.Stores = topicList;
            var userList = _dbContext.Users
                .Where(m => m.deleted_at == null && m.role_id == 2)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.full_name }).ToList();
            ViewBag.Users = userList;
            var data = _dbContext.Trainer_topics.Where(m => m.id == id).FirstOrDefault();
            if (data != null)
            {
                tt.id = data.id;
                tt.trainer_id = data.trainer_id;
                tt.topic_id = data.topic_id;
                tt.status = data.status;
            }

            return View(tt);
        }

        [HttpPost]
        public IActionResult Update2(Trainer_topicModelDetail tt)
        {
            try
            {
                var data = _dbContext.Trainer_topics.Where(m => m.id == tt.id).FirstOrDefault();
                if (data != null)
                {
                    data.trainer_id = tt.trainer_id;
                    data.topic_id = tt.topic_id;
                    data.status = tt.status;
                    data.updated_at = DateTime.Now;
                    _dbContext.SaveChanges(true);
                    TempData["UpdateStatus2"] = true;
                }
                else
                {
                    TempData["UpdateStatus2"] = false;
                }
            }
            catch (Exception ex)
            {
                TempData["UpdateStatus2"] = false;
            }
            return RedirectToAction(nameof(AssignController.Index), "Assign");
        }

        [HttpGet]
        public IActionResult Delete2(int id = 0)
        {
            try
            {
                var data = _dbContext.Trainer_topics.Where(m => m.id == id).FirstOrDefault();
                if (data != null)
                {
                    data.deleted_at = DateTime.Now;
                    _dbContext.SaveChanges(true);
                    TempData["DeleteStatus2"] = true;
                }
                else
                {
                    TempData["DeleteStatus2"] = false;
                }
            }
            catch
            {
                TempData["DeleteStatus2"] = false;
            }
            return RedirectToAction(nameof(AssignController.Index), "Assign");
        }
    }
}
