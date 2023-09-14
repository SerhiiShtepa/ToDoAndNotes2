using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToDoAndNotes2.Data;

namespace ToDoAndNotes2.Pages
{
    public class DI_BasePageModel : PageModel
    {
        protected ApplicationDbContext Context { get; }
        protected UserManager<IdentityUser> UserManager { get; }
        protected IAuthorizationService AuthorizationService { get; }


        public DI_BasePageModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IAuthorizationService authorizationService) : base()
        {
            Context = context;
            UserManager = userManager;
            AuthorizationService = authorizationService;
        }
    }
}
