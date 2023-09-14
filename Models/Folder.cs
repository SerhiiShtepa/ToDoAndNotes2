using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToDoAndNotes2.Models
{
    public class Folder
    {
        public int FolderId { get; set; }
        public string? OwnerId { get; set; } // user ID from AspNetUser table.
        [DisplayName("Назва"), MaxLength(25), Required(ErrorMessage = "Поле є обов'язковим до заповнення")]
        public string Name { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public ICollection<Note>? Notes { get; set; }
        public ICollection<Task>? Tasks { get; set; }

        //public string DisplayFolderName(int sNumber)
        //{
        //    return Name.Length > sNumber ? Name.Substring(0, sNumber) + "..." : Name;
        //}
    }
}