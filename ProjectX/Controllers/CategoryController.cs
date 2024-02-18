using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Interface;
using ProjectX.Models;
using ProjectX.ViewModels;

namespace ProjectX.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private ApplicationDbContext _db;
        public CategoryController(ICategoryRepository categoryRepository, ApplicationDbContext db)
        {
            _categoryRepository = categoryRepository;
            _db = db;
        }

        public async Task<IActionResult> Category(string Searchstring)
        {
            var categories = await _categoryRepository.GetAll();

           

            return View(categories);
        }

        [HttpGet]  // Add the attribute to explicitly specify it's for GET requests
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Add anti-forgery token protection
        public IActionResult CreateCategory(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    CategoryName = categoryViewModel.CategoryName
                };
                _categoryRepository.Add(category);
                return RedirectToAction("Category");
            }
            return View(categoryViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var categoryViewModel = new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
            return View(categoryViewModel);
        }

        [HttpPost]
        public IActionResult EditCategory(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    CategoryId = categoryViewModel.CategoryId,
                    CategoryName = categoryViewModel.CategoryName
                };
                _categoryRepository.Update(category);
                return RedirectToAction("CategoryList");
            }
            return View(categoryViewModel);
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> CategoryList()
        {
            var category = await _categoryRepository.GetAll();
            return View(category);
        }



    }
}
