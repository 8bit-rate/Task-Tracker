using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo_List.Models
{
	public class ImageModel
	{
		[Key]
		public int Id { get; set; }
		[NotMapped]
		public IFormFile? ImageFile { get; set; }
		public string? ImageTitle { get; set; }
	}
}