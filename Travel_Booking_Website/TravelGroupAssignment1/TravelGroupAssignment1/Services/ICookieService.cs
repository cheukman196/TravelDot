namespace TravelGroupAssignment1.Services
{
    public interface ICookieService
    {
        void SetCookie(string key, string value, int? expireTime);
        void GetCookie(string key);
        void DeleteCookie(string key);
    }
}
