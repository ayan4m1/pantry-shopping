using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Shopping.Services
{
    public static class Policies
    {
        public const string HouseholdMember = nameof(HouseholdMember);
    }

    public record HouseholdMemberRequirement : IAuthorizationRequirement;

    public class HouseholdMemberHandler(IIdentityClient identity) : AuthorizationHandler<HouseholdMemberRequirement>
    {
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, HouseholdMemberRequirement requirement)
        {
            if (context.Resource is not HttpContext http ||
                !Guid.TryParse(http.GetRouteValue("householdId")?.ToString(), out var householdId))
                return;

            var userId = context.User.FindFirstValue("sub");

            if (userId != null && await identity.IsHouseholdMemberAsync(householdId, userId))
                context.Succeed(requirement);
        }
    }
}
