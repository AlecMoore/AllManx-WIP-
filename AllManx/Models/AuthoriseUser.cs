using Microsoft.AspNetCore.Authorization;

namespace allmanx.models
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedroles;
        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }
        protected bool AuthoriseAttribute()
        {
            bool authorize = false;
            foreach (var role in allowedroles)
            {
                var user = context.appuser.where(m => m.userid == getuser.currentuser/* getting user form current context */ && m.role == role &&
                m.isactive == true); // checking active users with allowed roles.  
                if (user.count() > 0)
                {
                    authorize = true; /* return true if entity has current user(active) with specific role */
                }
            }
            return authorize;
        }
        protected override void handleunauthorizedrequest(authorizationcontext filtercontext)
        {
            filtercontext.result = new httpunauthorizedresult();
        }
    }
}
