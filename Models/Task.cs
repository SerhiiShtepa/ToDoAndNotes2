using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToDoAndNotes2.Models
{
    public class Task
    {
        public int? TaskId { get; set; }
        public int FolderId { get; set; }
        [DisplayName("Назва"), MaxLength(50), Required(ErrorMessage = "Поле є обов'язковим до заповнення")]
        public string? Title { get; set; }
        [DisplayName("Опис"), MaxLength(200)]
        public string? Description { get; set; } = default!;
        public bool IsCompleted { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public Folder? Folder { get; set; }
    }
}
