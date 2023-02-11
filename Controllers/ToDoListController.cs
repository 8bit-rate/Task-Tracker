﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
			IEnumerable<ToDoList> tasksList = _db.ToDoLists.Include(o => o.ImageModel);
			return View(tasksList);
		}

		//GET
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(ToDoList task)
		{
			if (task.DateStart >= task.Deadline)
				ModelState.AddModelError("deadline", "Deadline can not be before or equal date start");

			if (ModelState.IsValid)
			{
				var EntityModel = _db.ToDoLists.Add(task);
				_db.SaveChanges();

				int Id = EntityModel.Entity.Id;
				string wwwrootpath = _webHostEnvironment.WebRootPath;
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

					using (var fileStream = new FileStream(path, FileMode.Create))
						task.ImageModel.ImageFile.CopyTo(fileStream);

					_db.ToDoLists.Update(task);
					_db.SaveChanges();
				}

				TempData["success"] = "Create successfuly";

				return RedirectToAction("Index");
			}
			return View(task);
		}

		//GET
		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
				return NotFound();

			var taskFromDb = _db.ToDoLists.Include(o => o.ImageModel).FirstOrDefault(o => o.Id == id);

			if (taskFromDb == null)
				return NotFound();

			taskFromDb.TasksIdsAndContentsFromDb = _db.ToDoLists.AsNoTracking().Where(o => o.Id != id).ToDictionary(o => o.Id, o => o.Content);

			return View(taskFromDb);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(ToDoList task)
		{
			if (task.DateStart >= task.Deadline)
				ModelState.AddModelError("deadline", "Deadline can not be before or equal date start");

			if (ModelState.IsValid)
			{
				var EntityModel = _db.ToDoLists.Update(task);

				_db.SaveChanges();

				int Id = EntityModel.Entity.Id;

				string wwwrootpath = _webHostEnvironment.WebRootPath;

				if (task.ImageModel != null)
				{
					string fileName = Path.GetFileNameWithoutExtension(task.ImageModel.ImageFile!.FileName);
					string extension = Path.GetExtension(task.ImageModel.ImageFile.FileName);

					task.ImageModel.ImageTitle = $"{fileName}{Id}{extension}";

					string path = Path.Combine($"{wwwrootpath}/Tasks/Task{Id}/{task.ImageModel.ImageTitle}");

					using (var fileStream = new FileStream(path, FileMode.Create))
						task.ImageModel.ImageFile.CopyTo(fileStream);

					_db.ToDoLists.Update(task);
					_db.SaveChanges();
				}

				TempData["success"] = "Update successfuly";

				return RedirectToAction("Index");
			}
			return View(task);
		}

		//GET
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
				return NotFound();

			var taskFromDb = _db.ToDoLists.Include(o => o.ImageModel).FirstOrDefault(o => o.Id == id);

			if (taskFromDb == null)
				return NotFound();

			return View(taskFromDb);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult DeletePOST(int? id)
		{
			var taskFromDb = _db.ToDoLists.Include(o => o.ImageModel).FirstOrDefault(o => o.Id == id);

			if (taskFromDb == null)
				return NotFound();

			string wwwrootpath = _webHostEnvironment.WebRootPath;

			DirectoryInfo df = new($"{wwwrootpath}/Tasks/Task{id}");
			// Удаление каталога задачи из wwwroot
			if (df.Exists)																				
				df.Delete(true);
			// Удаление записи из БД, связанной с картинкой задачи, если она существует
			if (taskFromDb.ImageModel != null)															
				_db.Images.Remove(taskFromDb.ImageModel!);

			_db.ToDoLists.Remove(taskFromDb);
			// Поиск задач, которые связаны с текущей(удаляемой) по полю RelatedTaskId
			var relatedTasks = _db.ToDoLists.Where(o => o.RelatedTaskId == id).ToArray();
			// Если задачи, связанная с текущей(удаляемой) задачей, найдены, ставим у связанных задач соответствующее поле в null
			if (relatedTasks != null && relatedTasks.Length > 0)
			{
				foreach (var task in relatedTasks)
					task.RelatedTaskId = null;
			}

			_db.SaveChanges();

			TempData["success"] = "Removed successfuly";

			return RedirectToAction("Index");
		}
	}
}