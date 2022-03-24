using System.Security.Claims;

namespace JSONWebToken.Sevices
{
    public class UserSevices:IUserSevices
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public UserSevices(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }
        public string GetName()
        {
            var Result = string.Empty;
            if(_contextAccessor.HttpContext!=null)
            {
                Result = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return Result;
        }

    }
}
