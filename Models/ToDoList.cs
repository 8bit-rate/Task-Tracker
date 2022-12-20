using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToDo_List.Models
{
    public class ToDoList
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Task")]
        public string Content{ get; set; }
        public string? Description { get; set; }
        public ImageModel? ImageModel { get; set; }
        public string? Note { get; set; }
        
        public TaskStatus Status { get; set; } = TaskStatus.Created;
		
		public Importance Importance { get; set; } = Importance.Low;
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        [Required]
        [DisplayName("Date start")]
        public DateTime DateStart { get; set; }
        [Required]
        [DisplayName("Deadline")]
        public DateTime Deadline { get; set; }
    }
}