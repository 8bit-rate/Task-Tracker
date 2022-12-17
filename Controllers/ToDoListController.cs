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
		// Create actions
		//GET
		public IActionResult Create()
		{
			return View();
		}
		//POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(ToDoList obj)
		{
			if (obj.DateStart >= obj.Deadline)
			{
				ModelState.AddModelError("deadline", "Deadline can not be before or equal date start");
			}
			if (ModelState.IsValid)
			{
				_db.ToDoLists.Add(obj);
				_db.SaveChanges();
				TempData["success"] = "Create successfuly";
				return RedirectToAction("Index");
			}
			return View(obj);
		}

		// Edit acions

		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			var taskFromDb = _db.ToDoLists.Find(id);
			if (taskFromDb == null)
			{
				return NotFound();
			}
			return View(taskFromDb);
		}
		//POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(ToDoList obj)
		{
			if (obj.DateStart >= obj.Deadline)
			{
				ModelState.AddModelError("deadline", "Deadline can not be before or equal date start");
			}
			if (ModelState.IsValid)
			{
				_db.ToDoLists.Update(obj);
				_db.SaveChanges();
				TempData["success"] = "Update successfuly";
				return RedirectToAction("Index");
			}
			return View(obj);
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			var taskFromDb = _db.ToDoLists.Find(id);
			if (taskFromDb == null)
			{
				return NotFound();
			}
			return View(taskFromDb);
		}
		//POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult DeletePOST(int? id)
		{
			var taskFromDb = _db.ToDoLists.Find(id);
			if (taskFromDb == null)
			{
				return NotFound();
			}
			_db.ToDoLists.Remove(taskFromDb);
			_db.SaveChanges();
			TempData["success"] = "Removed successfuly";
			return RedirectToAction("Index");
		}
	}
}