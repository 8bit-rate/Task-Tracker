using System.ComponentModel.DataAnnotations;

namespace ToDo_List.Models
{
    public class ToDoList
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Content{ get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Created;
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        [Required]
        public DateTime DateStart { get; set; }
        [Required]
        public DateTime DeadLine { get; set; }
    }
}