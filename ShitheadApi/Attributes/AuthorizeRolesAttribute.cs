using Microsoft.AspNetCore.Authorization;

namespace ShitheadApi.Attributes
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles) : base() => Roles = string.Join(",", roles);
    }

    public static class AuthorizationRole
    {
        /// <summary>
        /// Role name for Shithead admin
        /// </summary>
        public const string ShitheadAdmin = "Shithead_Administrator_Off_All";

        /// <summary>
        /// Role name for Shithead user
        /// </summary>
        public const string ShitheadUser = "Shithead_User_Can_Read_Write";
    }
}
