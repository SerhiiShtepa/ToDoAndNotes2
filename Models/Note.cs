using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToDoAndNotes2.Models
{
    public class Note
    {
        public int? NoteId { get; set; }
        public int FolderId { get; set; }
        [DisplayName("Назва"), MaxLength(50), Required(ErrorMessage = "Поле є обов'язковим до заповнення")]
        public string Title { get; set; }
        public string? Content { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public Folder? Folder { get; set; }
    }
}
