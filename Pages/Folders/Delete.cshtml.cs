using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ToDoAndNotes2.Authorization;
using ToDoAndNotes2.Data;
using ToDoAndNotes2.Models;

namespace ToDoAndNotes2.Pages.Folders
{
    [Authorize]
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(ApplicationDbContext context,
                       UserManager<IdentityUser> userManager, IAuthorizationService authorizationService) : base(context, userManager, authorizationService)
        {
        }

        [BindProperty]
        public Folder Folder { get; set; } = default!;
        [BindProperty(SupportsGet = true, Name = "returnUrl")]
        public string? ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || Context.Folders == null)
            {
                return Redirect(ReturnUrl);
            }
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");

            var folder = await Context.Folders.FirstOrDefaultAsync(m => m.FolderId == id);

            if (folder == null)
            {
                return Redirect(ReturnUrl);
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, folder, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Folder = folder;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");

            if (id == null || Context.Folders == null)
            {
                return Redirect(ReturnUrl);
            }

            var folder = await Context.Folders.FindAsync(id);
            if (folder == null)
            {
                return Redirect(ReturnUrl);
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, folder, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Folder = folder;
            Context.Folders.Remove(Folder);
            await Context.SaveChangesAsync();
            return Redirect(ReturnUrl);
        }
    }
}
