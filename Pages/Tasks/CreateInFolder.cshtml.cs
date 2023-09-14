using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoAndNotes2.Authorization;
using ToDoAndNotes2.Data;
using ToDoAndNotes2.Models;

namespace ToDoAndNotes2.Pages.Tasks
{
    [Authorize]
    public class CreateInFolderModel : DI_BasePageModel
    {
        public CreateInFolderModel(ApplicationDbContext context,
                       UserManager<IdentityUser> userManager, IAuthorizationService authorizationService) : base(context, userManager, authorizationService)
        {
        }
        [BindProperty]
        public Models.Task Task { get; set; } = new Models.Task();
        [BindProperty(SupportsGet = true, Name = "currentFolderId")]
        public int? CurrentFolderId { get; set; } = default!;
        public Folder? CurrentFolder { get; set; }

        [BindProperty(SupportsGet = true, Name = "returnUrl")]
        public string? ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");
            Task.FolderId = CurrentFolderId ?? -1;
            if (Task.FolderId == -1)
            {
                return RedirectToPage("/Tasks/Create", new { returnUrl = ReturnUrl });
            }

            var folder = Context.Folders.FirstOrDefault(f => f.FolderId == Task.FolderId);
            if (folder == null)
            {
                return RedirectToPage("/Tasks/Create", new { returnUrl = ReturnUrl });
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, folder, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            CurrentFolder = folder;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Context.Tasks == null || Task == null)
            {
                return RedirectToAction("Get", new { currentFolderId = CurrentFolderId, returnUrl = ReturnUrl });
            }

            var folder = Context.Folders.FirstOrDefault(f => f.FolderId == CurrentFolderId);
            if (folder == null)
            {
                return RedirectToAction("Get", new { currentFolderId = CurrentFolderId, returnUrl = ReturnUrl });
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, folder, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            await Context.Tasks.AddAsync(Task);
            await Context.SaveChangesAsync();

            return Redirect(ReturnUrl!);
        }
    }
}
