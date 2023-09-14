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
    public class IndexModel : DI_BasePageModel
    {
        [BindProperty]
        public SidebarData SidebarData { get; set; }

        public IndexModel(ApplicationDbContext context,
             UserManager<IdentityUser> userManager, IAuthorizationService authorizationService) : base(context, userManager, authorizationService)
        {
            SidebarData = new SidebarData()
            {
                ReturnUrl = "/Index"
            };
        }

        [BindProperty(SupportsGet = true, Name = "currentFolderId")]
        public int? CurrentFolderId { get; set; } = default!;
        public IList<Models.Task> Tasks { get; set; } = default!;
        public IList<Note> Notes { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            if (CurrentFolderId == null || Context.Folders == null)
            {
                var userId = UserManager.GetUserId(User);

                var foldersContent = await Context.Folders
                    .Where(f => f.OwnerId == userId)
                    .Include(t => t.Tasks)
                    .Include(n => n.Notes)
                    .ToListAsync();

                Tasks = new List<Models.Task>();
                Notes = new List<Note>();

                foreach (var folder in foldersContent)
                {
                    Tasks.AddRange(folder.Tasks);
                    Notes.AddRange(folder.Notes);
                }
                Tasks = Tasks.Where(t => t.IsDeleted == false).ToList();
                Notes = Notes.Where(n => n.IsDeleted == false).ToList();
                return Page();
            }

            ViewData["CurrentFolderId"] = CurrentFolderId;
            var currentFolder = await Context.Folders.FirstOrDefaultAsync(m => m.FolderId == CurrentFolderId);
            Tasks = await Context.Tasks.Where(t => t.FolderId == CurrentFolderId && t.IsDeleted == false).ToListAsync();
            Notes = await Context.Notes.Where(n => n.FolderId == CurrentFolderId && n.IsDeleted == false).ToListAsync();

            return Page();
        }
    }
}