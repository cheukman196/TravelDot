namespace TravelGroupAssignment1.Services
{
    public class CookieService : ICookieService

    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void DeleteCookie(string key)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);


        }

        public string GetCookie(string key)
        {
            return _httpContextAccessor.HttpContext.Request.Cookies[key];

        }

        public void SetCookie(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();
            if(expireTime.HasValue)
            {
                option.Expires=DateTime.Now.AddMinutes(expireTime.Value);
            }
            else
            {
                option.Expires = DateTime.Now.AddDays(7);

            }
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, option);
        }

        void ICookieService.GetCookie(string key)
        {
            throw new NotImplementedException();
        }
    }
}
