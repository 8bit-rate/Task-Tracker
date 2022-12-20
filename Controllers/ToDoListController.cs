using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using ToDo_List.Data;
using ToDo_List.Models;

namespace ToDo_List.Controllers
{
	public class ToDoListController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ToDoListController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
		{
			_db = db;
			_webHostEnvironment = webHostEnvironment;
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
				var EntityModel = _db.ToDoLists.Add(obj);
				_db.SaveChanges();

				int Id = EntityModel.Entity.Id;
				string wwwrootpath = _webHostEnvironment.WebRootPath;
				string subDirPath = $"Task{Id}";
				DirectoryInfo directoryInfo = new($"{wwwrootpath}/Tasks");

				if (directoryInfo.Exists)
				{
					directoryInfo.Create();
				}

				directoryInfo.CreateSubdirectory(subDirPath);

				if (obj.ImageModel != null)
				{
					string fileName = Path.GetFileNameWithoutExtension(obj.ImageModel.ImageFile!.FileName);
					string extension = Path.GetExtension(obj.ImageModel.ImageFile.FileName);
					obj.ImageModel.ImageTitle = $"{fileName}{Id}{extension}";
					string path = Path.Combine($"{wwwrootpath}/Tasks/{subDirPath}/{obj.ImageModel.ImageTitle}");

					using (var fileStream = new FileStream(path, FileMode.Create))
					{
						obj.ImageModel.ImageFile.CopyTo(fileStream);
					}

					_db.ToDoLists.Update(obj);
					_db.SaveChanges();
				}
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
				var EntityModel = _db.ToDoLists.Update(obj);
				_db.SaveChanges();

				int Id = EntityModel.Entity.Id;
				string wwwrootpath = _webHostEnvironment.WebRootPath;
				string subDirPath = $"Task{Id}";

				if (obj.ImageModel != null)
				{
					string fileName = Path.GetFileNameWithoutExtension(obj.ImageModel.ImageFile!.FileName);
					string extension = Path.GetExtension(obj.ImageModel.ImageFile.FileName);
					obj.ImageModel.ImageTitle = $"{fileName}{Id}{extension}";
					string path = Path.Combine($"{wwwrootpath}/Tasks/{subDirPath}/{obj.ImageModel.ImageTitle}");

					using (var fileStream = new FileStream(path, FileMode.Create))
					{
						obj.ImageModel.ImageFile.CopyTo(fileStream);
					}

					_db.ToDoLists.Update(obj);
					_db.SaveChanges();
				}

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
			var taskFromDb = _db.ToDoLists.Include(o => o.ImageModel).FirstOrDefault(o => o.Id == id);
			if (taskFromDb == null)
			{
				return NotFound();
			}

			string wwwrootpath = _webHostEnvironment.WebRootPath;

			DirectoryInfo df = new($"{wwwrootpath}/Tasks/Task{id}");
			if (df.Exists)
			{
				df.Delete(true);
				_db.Images.Remove(taskFromDb.ImageModel!);
			}

			_db.ToDoLists.Remove(taskFromDb);
			//_db.Images.Remove(taskFromDb.ImageModel);

			_db.SaveChanges();
			TempData["success"] = "Removed successfuly";
			return RedirectToAction("Index");
		}
	}
}