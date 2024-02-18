using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Data;
using ProjectX.Interface;
using ProjectX.Models;
using ProjectX.Repository;
using ProjectX.ViewModels;

namespace ProjectX.Controllers
{
 
    public class ReferenceController : Controller
    {
        private readonly IReferenceRepository _referenceRepository;
        private readonly ApplicationDbContext _db;
        private readonly ISubjectRepository _subjectRepository;
        public ReferenceController(IReferenceRepository referenceRepository, ApplicationDbContext dbContext, ISubjectRepository subjectRepository)
        {
            _referenceRepository = referenceRepository;
            _db = dbContext;
            _subjectRepository = subjectRepository;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult AddReference()
        {

            var viewModel = new ReferenceViewModel
            {
                Categories = _db.Categories.ToList(),
                Subjects = _db.Subjects.ToList()
            };

            return View(viewModel);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> AddReference(ReferenceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var reference = new Reference
                {
                    
                    SubjectId = model.SubjectId,
                    // Set other properties
                    Name = model.Name,
                    YouTubePlaylist = model.YouTubePlaylist
                };

                _db.References.Add(reference);
                await _db.SaveChangesAsync();

                return RedirectToAction("ReferenceList");
            }


            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> GetSubjectsByCategory(int categoryId)
        {
            var subjects = await _subjectRepository.GetSubjectsByCategoryAsync(categoryId);
            return PartialView("_SubjectDropdown", subjects);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult ReferenceList()
        {
            var references = _db.References.ToList();
            return View(references);
        }

        //DeleteReference

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var reference = await _referenceRepository.GetByIdAsync(id);
            if (reference == null)
            {
                return NotFound();
            }

            _referenceRepository.Delete(reference);
            return RedirectToAction("ReferenceList");
        }
    }
}
