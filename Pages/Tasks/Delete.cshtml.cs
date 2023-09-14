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

namespace ToDoAndNotes2.Pages.Tasks
{
    [Authorize]
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(ApplicationDbContext context,
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

            if (id == null || Context.Folders == null)
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
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");

            if (id == null || Context.Tasks == null)
            {
                return Redirect(ReturnUrl);
            }

            var task = await Context.Tasks.FindAsync(id);
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
            Context.Tasks.Remove(Task);
            await Context.SaveChangesAsync();
            return Redirect(ReturnUrl);
        }
    }
}
