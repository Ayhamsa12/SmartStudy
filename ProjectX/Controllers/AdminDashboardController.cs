using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Data;

namespace ProjectX.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
          
           
            return View();
        }

    }
}
