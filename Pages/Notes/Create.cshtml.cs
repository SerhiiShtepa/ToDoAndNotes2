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

namespace ToDoAndNotes2.Pages.Notes
{
    [Authorize]
    public class CreateModel : DI_BasePageModel
    {
        public CreateModel(ApplicationDbContext context,
                       UserManager<IdentityUser> userManager, IAuthorizationService authorizationService) : base(context, userManager, authorizationService)
        {
        }
        [BindProperty]
        public Note Note { get; set; } = new Note();
        [BindProperty(SupportsGet = true, Name = "returnUrl")]
        public string? ReturnUrl { get; set; }

        public IActionResult OnGetAsync()
        {
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");
            ViewData["FolderId"] = new SelectList(Context.Folders.Where(f => f.OwnerId == UserManager.GetUserId(User) && f.IsDeleted == false), "FolderId", "Name");
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Context.Notes == null || Note == null)
            {
                return RedirectToAction("Get", new { returnUrl = ReturnUrl });
            }

            var folder = Context.Folders.FirstOrDefault(f => f.FolderId == Note.FolderId);
            if (folder == null)
            {
                return RedirectToAction("Get", new { returnUrl = ReturnUrl });
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, folder, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            await Context.Notes.AddAsync(Note);
            await Context.SaveChangesAsync();

            return Redirect(ReturnUrl!);
        }
    }
}
