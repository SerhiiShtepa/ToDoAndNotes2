using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ToDoAndNotes2.Authorization
{
    public static class UserOperations
    {
        public static OperationAuthorizationRequirement FullAccess = new OperationAuthorizationRequirement()
        {
            Name = Constants.FullAccessOperationName
        };
    }
    public class Constants
    {
        public static readonly string FullAccessOperationName = "FullAccess";
    }
}
