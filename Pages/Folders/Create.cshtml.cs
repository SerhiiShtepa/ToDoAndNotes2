using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoAndNotes2.Data;
using ToDoAndNotes2.Models;

namespace ToDoAndNotes2.Pages.Folders
{
    [Authorize]
    public class CreateModel : DI_BasePageModel
    {
        public CreateModel(ApplicationDbContext context,
                       UserManager<IdentityUser> userManager, IAuthorizationService authorizationService) : base(context, userManager, authorizationService)
        {
        }
        [BindProperty]
        public Folder Folder { get; set; } = new Folder();
        [BindProperty(SupportsGet = true, Name = "returnUrl")]
        public string? ReturnUrl { get; set; }

        public IActionResult OnGet()
        {
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            ReturnUrl = ReturnUrl ?? Url.Content("/Index");

            if (!ModelState.IsValid || Folder == null)
            {
                return Redirect(ReturnUrl);
            }
            Folder.OwnerId = UserManager.GetUserId(User);
            await Context.Folders.AddAsync(Folder);
            await Context.SaveChangesAsync();

            return Redirect(ReturnUrl);
        }
    }
}
