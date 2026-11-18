using System.Net;

namespace Shopping
{
    public interface IIdentityClient
    {
        Task<bool> IsHouseholdMemberAsync(Guid householdId, string userId);
    }

    public class IdentityClient(HttpClient http) : IIdentityClient
    {
        public async Task<bool> IsHouseholdMemberAsync(Guid householdId, string userId)
        {
            var response = await http.GetAsync($"internal/household/{householdId}/member/{Uri.EscapeDataString(userId)}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();

            return true;
        }
    }
}
