using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProjectX.Data;
using ProjectX.Interface;
using ProjectX.Models;
using ProjectX.ViewModels;

namespace ProjectX.Controllers
{
    public class RequestController : Controller
    {
        private ApplicationDbContext _db;
        private readonly UserManager<Users> _userManeger;
        private readonly SignInManager<Users> _signInManeger;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ICloudService _cloudService;
        public RequestController(UserManager<Users> userManager, SignInManager<Users> signInManager, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, ISubjectRepository subjectRepository, ICloudService cloudService)
        {
            _userManeger = userManager;
            _signInManeger = signInManager;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _subjectRepository = subjectRepository;
            _cloudService = cloudService;
        }
        [Authorize]
        public IActionResult SubmitRequest()
        {
            var model = new GalaxsyViewModel
            {
                Categories = _db.Categories.ToList(),
                Subjects = _db.Subjects.ToList()
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SubmitRequest(GalaxsyViewModel co)
        { 
            var user = await _userManeger.GetUserAsync(User);

            if (user != null)
            {
                if (co.AttachmentFile == null || co.AttachmentFile.Length == 0)
                {
                    // File not provided, add a model error
                    ModelState.AddModelError("AttachmentFile", "Please choose a file to upload.");
                    // Return to the view with model errors
                    return View(co);
                }
                string cloudinaryUrl = null;
                if (co.AttachmentFile != null)
                {
                    // Upload the file to Cloudinary
                    cloudinaryUrl = await _cloudService.UploadFileAsync(co.AttachmentFile);
                }

                var model = new Galaxsy
                {
                    //CategoryId = co.CategoryId,
                    SubjectId = co.SubjectId,
                    UserId = user.Id,
                    Message = !string.IsNullOrEmpty(co.Message) ? co.Message : "Default Request Message",
                    IsApproved = false,
                    Name = !string.IsNullOrEmpty(co.Name) ? co.Name : "Default Request Name",
                    AttachmentFile = cloudinaryUrl,
                };

                _db.galaxsies.Add(model);
                await _db.SaveChangesAsync();

                return RedirectToAction("RequestConfirmation");
            }

            return NotFound();
        }

        [Authorize]
        public IActionResult RequestConfirmation()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ViewRequestsAsync(GalaxsyViewModel gakaxyVM)
        {
            var requests = _db.galaxsies
                //.Include(r => r.Category)
                .Include(r => r.User)
                .Include(r => r.Subject)  // Include the Subject navigation property
                .Where(r => !r.IsApproved)
                .ToList();

            // Load the Subject object for each request using GetByIdAsync
            foreach (var request in requests)
            {
                request.Subject = await _subjectRepository.GetByIdAsync(request.SubjectId);
            }

            return View(requests);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> ApproveRequest(int requestId)
        {
            var request = await _db.galaxsies
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.GalaxsyId == requestId);

            if (request != null)
            {
                request.IsApproved = true;
                await _db.SaveChangesAsync();

                return RedirectToAction("ViewRequests");
            }

            return View();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> DeclineRequest(int requestId)
        {
            var request = await _db.galaxsies.FindAsync(requestId);
            if (request != null)
            {
                _db.galaxsies.Remove(request);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("ViewRequests");
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjectsByCategory(int categoryId)
        {
            var subjects = await _subjectRepository.GetSubjectsByCategoryAsync(categoryId);
            return PartialView("_SubjectDropdown", subjects);
        }

    }
    
}

