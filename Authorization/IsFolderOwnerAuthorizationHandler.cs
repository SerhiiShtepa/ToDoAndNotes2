using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoAndNotes2.Data;

namespace ToDoAndNotes2.Authorization
{
    public class IsFolderOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Models.Folder>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IsFolderOwnerAuthorizationHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        protected override System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, Models.Folder? resource)
        {
            if (context.User == null || resource == null) { return Task.CompletedTask; }
            if (requirement.Name != Constants.FullAccessOperationName) { return Task.CompletedTask; }

            if (resource == null) { return Task.CompletedTask; }

            if (resource.OwnerId == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
