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
    public class EditModel : DI_BasePageModel
    {
        public EditModel(ApplicationDbContext context,
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

            if (id == null || Context.Notes == null)
            {
                return Redirect(ReturnUrl);
            }

            var note = await Context.Notes.FirstOrDefaultAsync(n => n.NoteId == id);
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
            ViewData["FolderId"] = new SelectList(Context.Folders, "FolderId", "Name");
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Get", new { id = Note.NoteId, returnUrl = ReturnUrl });
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Note, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Attach(Note).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoteExists(Note.NoteId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Redirect(ReturnUrl!);
        }

        private bool NoteExists(int? id)
        {
            return (Context.Notes?.Any(e => e.NoteId == id)).GetValueOrDefault();
        }
    }
}
