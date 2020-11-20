using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private ToDoContext context;
        public HomeController(ToDoContext ctx) => context = ctx;

        public IActionResult Index(string id)
        {
            var model = new ToDoViewModel();
            model.Filters = new Filters(id);
            model.Categories = context.Categories.ToList();
            model.Statuses = context.Statuses.ToList();
            model.DueFilters = Filters.DueFilterValues;

            IQueryable<ToDo> query = context.toDos.Include(c => c.Category).Include(s => s.Status);

            if (model.Filters.HasCategory)
                query = query.Where(t => t.CategoryId == model.Filters.CategoryId);

            if (model.Filters.HasStatus)
                query = query.Where(t => t.StatusId == model.Filters.StatusId);

            if (model.Filters.HasDue)
            {
                var today = DateTime.Today;

                if (model.Filters.IsPast)
                    query = query.Where(t => t.DueDate < today);
                else if (model.Filters.IsFuture)
                    query = query.Where(t => t.DueDate > today);
                else if (model.Filters.IsToday)
                    query = query.Where(t => t.DueDate == today);
            }

            var tasks = query.OrderBy(t => t.DueDate).ToList();

            model.Tasks = tasks;

            return View(model);
        }

        [HttpGet]
        public IActionResult Add(  )
    }
}
