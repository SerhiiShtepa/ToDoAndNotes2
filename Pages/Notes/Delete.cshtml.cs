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

namespace ToDoAndNotes2.Pages.Notes
{
    [Authorize]
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(ApplicationDbContext context,
                       UserManager<IdentityUser> userManager, IAuthorizationService authorizationService) : base(context, userManager, authorizationService)
        {
        }

        [BindProperty]
        public Note Note { get; set; } = default!;
        [BindProperty(SupportsGet = true, Name = "returnUrl")]
        public string? ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");

            if (id == null || Context.Folders == null)
            {
                return Redirect(ReturnUrl);
            }

            var note = await Context.Notes
                .Include(n => n.Folder)
                .FirstOrDefaultAsync(n => n.NoteId == id);
            if (note == null)
            {
                return Redirect(ReturnUrl);
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, note, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Note = note;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");

            if (id == null || Context.Notes == null)
            {
                return Redirect(ReturnUrl);
            }

            var note = await Context.Notes.FindAsync(id);
            if (note == null)
            {
                return Redirect(ReturnUrl);
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, note, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Note = note;
            Context.Notes.Remove(Note);
            await Context.SaveChangesAsync();
            return Redirect(ReturnUrl);
        }
    }
}
