using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoAndNotes2.Data;

namespace ToDoAndNotes2.Authorization
{
    public class IsTaskOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Models.Task>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public IsTaskOwnerAuthorizationHandler(UserManager<IdentityUser> userManager, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }
        protected override System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, Models.Task? resource)
        {
            if (context.User == null || resource == null) { return Task.CompletedTask; }
            if (requirement.Name != Constants.FullAccessOperationName) { return Task.CompletedTask; }

            resource = _applicationDbContext.Tasks
                .Include(t => t.Folder).AsNoTracking().FirstOrDefault(t => t.TaskId == resource.TaskId);
            if (resource == null) { return Task.CompletedTask; }

            if (resource?.Folder?.OwnerId == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
