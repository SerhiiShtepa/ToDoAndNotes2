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

namespace ToDoAndNotes2.Pages.Tasks
{
    [Authorize]
    public class EditModel : DI_BasePageModel
    {
        public EditModel(ApplicationDbContext context,
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

            Task = task;
            ViewData["FolderId"] = new SelectList(Context.Folders, "FolderId", "Name");
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Get", new { id = Task.TaskId, returnUrl = ReturnUrl });
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Task, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Attach(Task).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(Task.TaskId))
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
        public async Task<IActionResult> OnPostChangeState(int? taskIdChangeState)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");

            var task = await Context.Tasks.FirstOrDefaultAsync(m => m.TaskId == taskIdChangeState);
            if (task == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, task, UserOperations.FullAccess);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            task.IsCompleted = !task.IsCompleted;
            Context.Attach(task).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.TaskId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Redirect(ReturnUrl);
        }

        private bool TaskExists(int? id)
        {
            return (Context.Tasks?.Any(e => e.TaskId == id)).GetValueOrDefault();
        }
    }
}
