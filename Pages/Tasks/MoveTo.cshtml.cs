using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

namespace ToDoAndNotes2.Pages.Tasks
{
    [Authorize]
    public class MoveToModel : DI_BasePageModel
    {
        public MoveToModel(ApplicationDbContext context,
                       UserManager<IdentityUser> userManager, IAuthorizationService authorizationService) : base(context, userManager, authorizationService)
        {
        }

        [BindProperty]
        public Models.Task Task { get; set; } = default!;
        [BindProperty(SupportsGet = true, Name = "returnUrl")]
        public string? ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");

            if (id == null || Context.Tasks == null)
            {
                return Redirect(ReturnUrl);
            }

            var task = await Context.Tasks
                .Include(t => t.Folder)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return Redirect(ReturnUrl);
            }
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, task, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            Task = task;
            return Page();
        }
        public async Task<IActionResult> OnPostToBinAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Task, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Task.IsDeleted = true;
            Context.Attach(Task).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(Task.FolderId))
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

            if (id == null || Context.Tasks == null)
            {
                return Redirect(ReturnUrl);
            }

            var task = await Context.Tasks.FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return Redirect(ReturnUrl);
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, task, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            task.IsDeleted = false;
            Context.Attach(task).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.FolderId))
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

        private bool TaskExists(int id)
        {
            return (Context.Tasks?.Any(e => e.TaskId == id)).GetValueOrDefault();
        }
    }
}
