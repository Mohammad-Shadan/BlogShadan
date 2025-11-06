using BlogShadan.Data;
using BlogShadan.Models;
using BlogShadan.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlogShadan.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtension = { ".jpg", ".jpeg", ".png" };

        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index(int? categoryID)
        {
            var postQuery = _context.Posts.Include(p=>p.Category).AsQueryable();
            if (categoryID.HasValue)
            {
                postQuery = postQuery.Where(p=>p.CategoryId == categoryID);
            }
            var posts = postQuery.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var post = _context.Posts.Include(p=>p.Category).Include(p=>p.Comments).
                FirstOrDefault(p=>p.Id == id);

            if(post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var postViewModel = new PostViewModel();
            postViewModel.Categories= _context.Categories.Select(c=>
            new SelectListItem { 
                Value = c.Id.ToString(),
                Text = c.Name
            }
            ).ToList();
            return View(postViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postFromDB = await _context.Posts.FirstOrDefaultAsync(p=>p.Id == id);
            if (postFromDB == null) { 
            return NotFound();
            }
            EditViewModel editViewModel = new EditViewModel
            {
                Post = postFromDB,
                Categories = _context.Categories.Select(c =>
                new SelectListItem
                 {
                     Value = c.Id.ToString(),
                       Text = c.Name
                 }
                ).ToList()
            };
                
           return View(editViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var postFromDB = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (postFromDB == null) { return NotFound(); }

            return View(postFromDB);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var postFromDB = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (postFromDB == null) { return NotFound(); }

            if(string.IsNullOrEmpty(postFromDB.FeatureImagePath))
            {

                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images",
                   Path.GetFileName(postFromDB.FeatureImagePath));
                var existingFilepath1 = postFromDB.FeatureImagePath;

                if (System.IO.File.Exists(existingFilePath))
                {

                    System.IO.File.Delete(existingFilePath);

                }
            }

            _context.Posts.Remove(postFromDB);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }
            var postFromDB = await  _context.Posts.AsNoTracking().FirstOrDefaultAsync(p=>p.Id==editViewModel.Post.Id);
            if (postFromDB == null) {
                return NotFound();
            }

            if (editViewModel.FeatureImage != null)
            {
                var inputFileExtension = Path.GetExtension(editViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = _allowedExtension.Contains(inputFileExtension);
                if (!isAllowed)
                {
                    ModelState.AddModelError("", "Invalid Image Format. Alloweed Format are .jpg, .jpeg, .png");
                    return View(editViewModel);
                }
                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images",
                    Path.GetFileName(postFromDB.FeatureImagePath));
            

                if (System.IO.File.Exists(existingFilePath))
                {

                    System.IO.File.Delete(existingFilePath);

                }
                editViewModel.Post.FeatureImagePath = await UploadFileToFolder(editViewModel.FeatureImage);
            }
            else {
                editViewModel.Post.FeatureImagePath=postFromDB.FeatureImagePath;  
            }
            _context.Posts.Update(editViewModel.Post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", new { id = editViewModel.Post.Id });

        }

        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var inputFileExtension = Path.GetExtension(postViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = _allowedExtension.Contains(inputFileExtension);
                if (!isAllowed)
                {
                    ModelState.AddModelError("", "Invalid Image Format. Alloweed Format are .jpg, .jpeg, .png");
                    return View(postViewModel);
                }

                postViewModel.Post.FeatureImagePath = await UploadFileToFolder(postViewModel.FeatureImage);
               await  _context.Posts.AddAsync(postViewModel.Post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }


            postViewModel.Categories = _context.Categories.Select(c =>
            new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }
            ).ToList();
            return View(postViewModel);

        }

        public JsonResult AddComment([FromBody]Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return Json(new
            {
                username = comment.UserName,
                commentdate= comment.CommentDate.ToString("MMMM dd,yyyy"),
                content = comment.Content
            });
        }

        private async Task<string> UploadFileToFolder(IFormFile file)
        {
            var inputFileExtension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + inputFileExtension;
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var imageFolderPath = Path.Combine(wwwRootPath, "images");

            if (!Directory.Exists(imageFolderPath)) { 
                Directory.CreateDirectory(imageFolderPath);
            }

            var filePath = Path.Combine(imageFolderPath, fileName);
            try
            {
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch(Exception ex) { 
                 return "Error uploading Images : "+ex.Message;
            }
            return "/images/" + fileName;
        }
    }
}
