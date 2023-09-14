using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Security.Policy;
using ToDoAndNotes2.Data;
using ToDoAndNotes2.Models;

namespace ToDoAndNotes2.Pages
{
    [Authorize]
    public class BinModel : DI_BasePageModel
    {
        [BindProperty]
        public SidebarData SidebarData { get; set; }

        public BinModel(ApplicationDbContext context,
            UserManager<IdentityUser> userManager, IAuthorizationService authorizationService) : base(context, userManager, authorizationService)
        {
            SidebarData = new SidebarData()
            {
                ReturnUrl = "/Index"
            };
        }

        [BindProperty(SupportsGet = true, Name = "returnUrl")]
        public string? ReturnUrl { get; set; }
        public IList<Models.Task> Tasks { get; set; } = default!;
        public IList<Note> Notes { get; set; } = default!;
        public IList<Folder> Folders { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            if (Context.Folders != null)
            {
                var userId = UserManager.GetUserId(User);

                var foldersContent = await Context.Folders
                    .Where(f => f.OwnerId == userId)
                    .Include(t => t.Tasks)
                    .Include(n => n.Notes)
                    .ToListAsync();

                Tasks = new List<Models.Task>(); // DELETED Tasks from NOT DELETED Folder
                Notes = new List<Note>();
                Folders = new List<Folder>();

                foreach (var folder in foldersContent.Where(f => f.IsDeleted == false))
                {
                    Tasks.AddRange(folder.Tasks);
                }
                Tasks = Tasks.Where(t => t.IsDeleted == true).ToList();

                foreach (var folder in foldersContent.Where(f => f.IsDeleted == false))
                {
                    Notes.AddRange(folder.Notes);
                }
                Notes = Notes.Where(t => t.IsDeleted == true).ToList();

                Folders = foldersContent.Where(f => f.IsDeleted == true).ToList();
            }
            return Page();
        }
    }
}