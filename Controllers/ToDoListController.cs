using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo_List.Data;
using ToDo_List.Models;

namespace ToDo_List.Controllers
{
	public class ToDoListController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly string wwwrootpath = string.Empty;
		public ToDoListController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment) =>
			(_db, _webHostEnvironment, wwwrootpath) = (db, webHostEnvironment, webHostEnvironment.WebRootPath);
		
		public async Task<IActionResult> Index()
		{
			IEnumerable<ToDoList> tasksList = await _db.ToDoLists.Include(o => o.ImageModel).ToArrayAsync();
			return View(tasksList);
		}

		//GET
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ToDoList task)
		{
			if (task.DateStart >= task.Deadline)
				ModelState.AddModelError("deadline", "Deadline can not be before or equal date start");

			if (ModelState.IsValid)
			{
				var EntityModel = _db.ToDoLists.Add(task);
				await _db.SaveChangesAsync();

				int Id = EntityModel.Entity.Id;
				string subDirName = $"Task{Id}";

				DirectoryInfo directoryInfo = new($"{wwwrootpath}/Tasks");

				if (directoryInfo.Exists)
					directoryInfo.Create();

				directoryInfo.CreateSubdirectory(subDirName);

				if (task.ImageModel != null)
				{
					string fileName = Path.GetFileNameWithoutExtension(task.ImageModel.ImageFile!.FileName);
					string extension = Path.GetExtension(task.ImageModel.ImageFile.FileName);
					task.ImageModel.ImageTitle = $"{fileName}{Id}{extension}";
					string path = Path.Combine($"{wwwrootpath}/Tasks/{subDirName}/{task.ImageModel.ImageTitle}");

					using var fileStream = new FileStream(path, FileMode.Create);
						task.ImageModel.ImageFile.CopyTo(fileStream);

					_db.ToDoLists.Update(task);
					await _db.SaveChangesAsync();
				}				

				TempData["success"] = "Create successfuly";

				return RedirectToAction("Index");
			}
			return View(task);
		}

		//GET
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || id == 0)
				return NotFound();

			var taskFromDb = await _db.ToDoLists.Include(o => o.ImageModel).FirstOrDefaultAsync(o => o.Id == id);

			if (taskFromDb == null)
				return NotFound();

			taskFromDb.TasksIdsAndContentsFromDb = await _db.ToDoLists.AsNoTracking().Where(o => o.Id != id).ToDictionaryAsync(o => o.Id, o => o.Content);

			return View(taskFromDb);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(ToDoList task)
		{
			if (task.DateStart >= task.Deadline)
				ModelState.AddModelError("deadline", "Deadline can not be before or equal date start");

			if (ModelState.IsValid)
			{
				var EntityModel = _db.ToDoLists.Update(task);
				await _db.SaveChangesAsync();

				if (task.ImageModel != null)
				{
					int Id = EntityModel.Entity.Id;

					string fileName = Path.GetFileNameWithoutExtension(task.ImageModel.ImageFile!.FileName);
					string extension = Path.GetExtension(task.ImageModel.ImageFile.FileName);

					task.ImageModel.ImageTitle = $"{fileName}{Id}{extension}";

					string path = Path.Combine($"{wwwrootpath}/Tasks/Task{Id}/{task.ImageModel.ImageTitle}");

					using (var fileStream = new FileStream(path, FileMode.Create))
						task.ImageModel.ImageFile.CopyTo(fileStream);

					_db.ToDoLists.Update(task);
					await _db.SaveChangesAsync();
				}
				TempData["success"] = "Update successfuly";

				return RedirectToAction("Index");
			}
			return View(task);
		}

		//GET
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || id == 0)
				return NotFound();

			var taskFromDb = await _db.ToDoLists.Include(o => o.ImageModel).FirstOrDefaultAsync(o => o.Id == id);

			if (taskFromDb == null)
				return NotFound();

			return View(taskFromDb);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeletePOST(int? id)
		{
			var taskFromDb = await _db.ToDoLists.Include(o => o.ImageModel).FirstOrDefaultAsync(o => o.Id == id);

			if (taskFromDb == null)
				return NotFound();

			DirectoryInfo df = new($"{wwwrootpath}/Tasks/Task{id}");
			// Удаление каталога задачи из wwwroot
			if (df.Exists)																				
				df.Delete(true);
			// Удаление записи из БД, связанной с картинкой задачи, если она существует
			if (taskFromDb.ImageModel != null)															
				_db.Images.Remove(taskFromDb.ImageModel!);

			_db.ToDoLists.Remove(taskFromDb);
			// Поиск задач, которые связаны с текущей(удаляемой) по полю RelatedTaskId
			var relatedTasks = await _db.ToDoLists.Where(o => o.RelatedTaskId == id).ToArrayAsync();
			// Если задачи, связанная с текущей(удаляемой) задачей, найдены, ставим у связанных задач соответствующее поле в null
			if (relatedTasks != null && relatedTasks.Length > 0)
			{
				foreach (var task in relatedTasks)
					task.RelatedTaskId = null;
			}

			await _db.SaveChangesAsync();

			TempData["success"] = "Removed successfuly";

			return RedirectToAction("Index");
		}
	}
}