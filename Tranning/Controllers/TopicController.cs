using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Tranning.DataDBContext;
using Tranning.Models;

namespace Tranning.Controllers
{
    public class TopicController : Controller
    {
        private readonly TranningDBContext _dbContext;

        public TopicController(TranningDBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index(string SearchString)
        {
            TopicModel topicModel = new TopicModel();
            topicModel.TopicDetailLists = new List<TopicDetail>();

            var data = from m in _dbContext.Topics select m;

            data = data.Where(m => m.deleted_at == null);
            if (!string.IsNullOrEmpty(SearchString))
            {
                data = data.Where(m => m.name.Contains(SearchString));
            }
            var dataList = data.ToList();

            foreach (var item in dataList)
            {
                var dataC = _dbContext.Courses.Where(m => m.id == item.course_id).FirstOrDefault();

                topicModel.TopicDetailLists.Add(new TopicDetail
                {
                    id = item.id,
                    course_id = item.course_id,
                    course_name = dataC.name,
                    name = item.name,
                    description = item.description,
                    videos = item.videos,
                    documents = item.documents,
                    attach_file = item.attach_file,
                    status = item.status,
                    created_at = item.created_at,
                    updated_at = item.updated_at
                });
            }
            ViewData["CurrentFilter"] = SearchString;
            return View(topicModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
            TopicDetail topic = new TopicDetail();
            var courseList = _dbContext.Courses
                .Where(m => m.deleted_at == null)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.name }).ToList();
            ViewBag.Stores = courseList;
            return View(topic);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TopicDetail topic, IFormFile? videoFile, IFormFile? documentsFile, IFormFile? attach_fileFile)
        {
            if (ModelState.IsValid)
            {
                var uniqueFileNameVideo = "";
                var uniqueFileNameDc = "";
                var uniqueFileNameFile = "";
                try
                {
                    if (topic.videoFile != null)
                    {
                        uniqueFileNameVideo = UploadFile(topic.videoFile);
                    }
                    if (topic.documentsFile != null)
                    {
                        uniqueFileNameDc = UploadFile(topic.documentsFile);
                    }
                    if (topic.attach_fileFile != null)
                    {
                        uniqueFileNameFile = UploadFile(topic.attach_fileFile);
                    }
                    var topicData = new Topic()
                    {
                        name = topic.name,
                        course_id = topic.course_id,
                        description = topic.description,
                        videos = uniqueFileNameVideo,
                        documents = uniqueFileNameDc,
                        attach_file = uniqueFileNameFile,
                        status = topic.status,
                        created_at = DateTime.Now,
                    };

                    _dbContext.Topics.Add(topicData);
                    _dbContext.SaveChanges(true);
                    TempData["saveStatus"] = true;
                }
                catch(Exception ex)
                {
                    TempData["saveStatus"] = false;
                }
                return RedirectToAction(nameof(TopicController.Index), "Topic");
            }
            //foreach (var modelStateEntry in ModelState.Values)
            //{
            //    foreach (var error in modelStateEntry.Errors)
            //    {
            //        Console.WriteLine($"Error: {error.ErrorMessage}");
            //    }
            //}
            var courseList = _dbContext.Courses
                .Where(m => m.deleted_at == null)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.name }).ToList();
            ViewBag.Stores = courseList;
            Console.WriteLine(ModelState.IsValid);
            return View(topic);
        }
        [HttpGet]
        public IActionResult Update(int id = 0)
        {
            TopicDetail topic = new TopicDetail();
            var data = _dbContext.Topics.Where(m => m.id == id).FirstOrDefault();

            if (data != null)
            {
                topic.id = data.id;
                topic.name = data.name;
                topic.course_id = data.course_id;
                topic.description = data.description;
                topic.videos = data.videos;
                topic.documents = data.documents;
                topic.attach_file = data.attach_file;
                topic.status = data.status;
            }
            var courseList = _dbContext.Courses
                .Where(m => m.deleted_at == null)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.name }).ToList();
            ViewBag.Stores = courseList;
            return View(topic);
        }

        [HttpPost]
        public IActionResult Update(TopicDetail topic, IFormFile documentsFile, IFormFile attach_fileFile)
        {
            try
            {

                var data = _dbContext.Topics.Where(m => m.id == topic.id).FirstOrDefault();
                string uniqueDocument = "";
                string uniqueAttachFile = "";
                if (topic.documentsFile != null)
                {
                    uniqueDocument = uniqueDocument = UploadFile(topic.documentsFile);
                }
                if (topic.attach_fileFile != null)
                {
                    uniqueAttachFile = uniqueAttachFile = UploadFile(topic.attach_fileFile);
                }
                if (data != null)
                {
                    // gan lai du lieu trong db bang du lieu tu form model gui len
                    data.name = topic.name;
                    data.course_id = topic.course_id;
                    data.description = topic.description;
                    data.videos = topic.videos;
                    data.status = topic.status;
                    if (!string.IsNullOrEmpty(uniqueDocument))
                    {
                        data.documents = uniqueDocument;
                    }
                    if (!string.IsNullOrEmpty(uniqueAttachFile))
                    {
                        data.attach_file = uniqueAttachFile;
                    }
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
            return RedirectToAction(nameof(TopicController.Index), "Topic");
        }

        [HttpGet]
        public IActionResult Delete(int id = 0)
        {
            try
            {
                var data = _dbContext.Topics.Where(m => m.id == id).FirstOrDefault();
                if (data != null)
                {
                    data.deleted_at = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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
            return RedirectToAction(nameof(TopicController.Index), "Topic");
        }

        private string UploadFile(IFormFile file)
        {
            string uniqueFileName;
            try
            {
                string pathUploadServer = "wwwroot\\uploads\\file";

                string fileName = file.FileName;
                fileName = Path.GetFileName(fileName);
                string uniqueStr = Guid.NewGuid().ToString(); // random tao ra cac ky tu khong trung lap
                // tao ra ten fil ko trung nhau
                fileName = uniqueStr + "-" + fileName;
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), pathUploadServer, fileName);
                var stream = new FileStream(uploadPath, FileMode.Create);
                file.CopyToAsync(stream);
                // lay lai ten anh de luu database sau nay
                uniqueFileName = fileName;
            }
            catch (Exception ex)
            {
                uniqueFileName = "Can not read this file";
            }
            return uniqueFileName;
        }
    }
}
