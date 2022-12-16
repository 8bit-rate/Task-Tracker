using Microsoft.AspNetCore.Mvc;
using ToDo_List.Data;
using ToDo_List.Models;

namespace ToDo_List.Controllers
{
    public class ToDoListController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ToDoListController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<ToDoList> objCategoryList = _db.ToDoLists;
            return View(objCategoryList);
        }
    }
}
