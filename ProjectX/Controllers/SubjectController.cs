using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Interface;
using ProjectX.Models;
using ProjectX.Repository;
using ProjectX.ViewModels;
using System.Reflection.Metadata.Ecma335;

namespace ProjectX.Controllers
{
    [Authorize]
    public class SubjectController : Controller
    {

        private readonly ISubjectRepository _subjectRepository;
        
        private readonly IReferenceRepository _referenceRepository;
        private readonly ICategoryRepository _categoryRepository;
        private ApplicationDbContext _db;
        private readonly IGalaxsyRepository _galaxsyRepository;

        public SubjectController(ISubjectRepository subjectRepository,  IReferenceRepository referenceRepository, ICategoryRepository categoryRepository, ApplicationDbContext db, IGalaxsyRepository galaxsyRepository)
        {

            this._subjectRepository = subjectRepository;
            
            _referenceRepository = referenceRepository;
            _categoryRepository = categoryRepository;
            _db = db;
            _galaxsyRepository = galaxsyRepository;
        }

        public async Task<IActionResult> Subject(int categoryId)
        {
            var subjectsInCategory = await _subjectRepository.GetSubjectsByCategoryAsync(categoryId);
            return View("Subject", subjectsInCategory);
        }

        public async Task<IActionResult> Search(int categoryId, string searchString)
        {
            var allSubjects = await _db.Subjects.ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                allSubjects = allSubjects
                    .Where(x => x.SubjectName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

         

            return View("Subject", allSubjects);
        }
        public async Task<IActionResult> Detail(int id)
        {
            Subject subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            IEnumerable<Galaxsy> summaries = _galaxsyRepository.GetSummariesBySubject(id);
            IEnumerable<Reference> references = _referenceRepository.GetReferencesBySubject(id);

            var viewModel = new SubjectDetailViewModel
            {
                Subject = subject,
                Summaries = summaries,
                References = references
            };

            return View(viewModel);
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Create()
        {
            var model = new SubjectViewModel();


            ViewBag.Categories = (await _categoryRepository.GetAll()).Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Create(SubjectViewModel model)
        { 
            if (ModelState.IsValid)
            {
                var subject = new Subject
                {

                    SubjectName = model.SubjectName,
                    CategoryId = model.CategoryId,
                    // Map other properties as needed
                };

                _subjectRepository.Add(subject);
                TempData["SubjectSuccess"] = "The Subject has been Added Successflly ";

                return RedirectToAction("Create"/*, new { categoryId = model.CategoryId }*/);
            }

            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            var categoryId = subject.CategoryId;

            _subjectRepository.Delete(subject);
            return RedirectToAction("SubjectList");
        }

        //EditeSubject
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            var model = new SubjectViewModel
            {
                SubjectName = subject.SubjectName,
                CategoryId = subject.CategoryId,
                
            };

            ViewBag.Categories = (await _categoryRepository.GetAll()).Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, SubjectViewModel model)
        {
            var existingSubject = await _subjectRepository.GetByIdAsync(id);
            if (existingSubject == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingSubject.SubjectName = model.SubjectName;
                existingSubject.CategoryId = model.CategoryId;
                

                _subjectRepository.Update(existingSubject);

                return RedirectToAction("SubjectList"/*, new { categoryId = model.CategoryId }*/);
            }

            // If ModelState is not valid, fetch categories again for the dropdown
            ViewBag.Categories = (await _categoryRepository.GetAll()).Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();

            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> SubjectList()
        {
            var subject = await _subjectRepository.GetAll();
            return View(subject);
        }

        public async Task<IActionResult> SubjectListSearch(int categoryId, string searchString)
        {
            var allSubjects = await _db.Subjects.ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                allSubjects = allSubjects
                    .Where(x => x.SubjectName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            return View("SubjectList", allSubjects);
        }



    }

}
