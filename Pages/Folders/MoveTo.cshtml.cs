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
    public class MoveToModel : DI_BasePageModel
    {
        public MoveToModel(ApplicationDbContext context,
                       UserManager<IdentityUser> userManager, IAuthorizationService authorizationService) : base(context, userManager, authorizationService)
        {
        }

        [BindProperty]
        public Folder Folder { get; set; } = default!;
        [BindProperty(SupportsGet = true, Name = "returnUrl")]
        public string? ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetToBinAsync(int? id)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");

            if (id == null || Context.Folders == null)
            {
                return Redirect(ReturnUrl);
            }

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
        public async Task<IActionResult> OnPostToBinAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Folder, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Folder.IsDeleted = true;

            var tasks = Context.Tasks.Where(t => t.FolderId == Folder.FolderId);
            var notes = Context.Notes.Where(t => t.FolderId == Folder.FolderId);
            Context.Attach(Folder).State = EntityState.Modified;

            foreach (var task in tasks)
            {
                task.IsDeleted = true;
                Context.Attach(task).State = EntityState.Modified;
            }
            foreach (var note in notes)
            {
                note.IsDeleted = true;
                Context.Attach(note).State = EntityState.Modified;
            }

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FolderExists(Folder.FolderId))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    throw;
                }
            }
            return Redirect(ReturnUrl);
        }
        public async Task<IActionResult> OnPostRestoreAsync(int? id)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");

            if (id == null || Context.Folders == null)
            {
                return Redirect(ReturnUrl);
            }

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

            folder.IsDeleted = false;
            Context.Attach(folder).State = EntityState.Modified;

            var tasks = Context.Tasks.Where(t => t.FolderId == folder.FolderId);
            var notes = Context.Notes.Where(t => t.FolderId == folder.FolderId);

            foreach (var task in tasks)
            {
                task.IsDeleted = false;
                Context.Attach(task).State = EntityState.Modified;
            }
            foreach (var note in notes)
            {
                note.IsDeleted = false;
                Context.Attach(note).State = EntityState.Modified;
            }

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FolderExists(folder.FolderId))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    throw;
                }
            }
            return Redirect(ReturnUrl);
        }

        private bool FolderExists(int id)
        {
            return (Context.Folders?.Any(e => e.FolderId == id)).GetValueOrDefault();
        }
    }
}
