//using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using ToDo_List.Models;

namespace ToDo_List.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ToDoList> ToDoLists { get; set; }
        public DbSet<ImageModel> Images { get; set; }
    }
}