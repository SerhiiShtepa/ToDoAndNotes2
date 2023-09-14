using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDoAndNotes2.Authorization;
using ToDoAndNotes2.Data;
using ToDoAndNotes2.Models;

namespace ToDoAndNotes2.Pages.Folders
{
    [Authorize]
    public class EditModel : DI_BasePageModel
    {
        public EditModel(ApplicationDbContext context,
                       UserManager<IdentityUser> userManager, IAuthorizationService authorizationService) : base(context, userManager, authorizationService)
        {
        }

        [BindProperty]
        public Folder Folder { get; set; } = default!;
        [BindProperty]
        public string? ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string? returnUrl = null)
        {
            if (id == null || Context.Folders == null)
            {
                return NotFound();
            }
            ReturnUrl = returnUrl ?? Url.Content("/Index");

            var folder = await Context.Folders.FirstOrDefaultAsync(m => m.FolderId == id);
            if (folder == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, folder, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Folder = folder;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            ReturnUrl = returnUrl ?? Url.Content("/Index");

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Folder, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Attach(Folder).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FolderExists(Folder.FolderId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage(ReturnUrl);
        }

        private bool FolderExists(int id)
        {
            return (Context.Folders?.Any(e => e.FolderId == id)).GetValueOrDefault();
        }
    }
}
