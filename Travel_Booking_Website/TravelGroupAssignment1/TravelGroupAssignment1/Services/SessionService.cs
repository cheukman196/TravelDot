using System.Runtime.CompilerServices;
using System.Text.Json;

namespace TravelGroupAssignment1.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public SessionService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public T GetSessionData<T>(string key)
        {
            var session = _contextAccessor.HttpContext.Session;
            var value = session.GetString(key); //creates session if not made-normally serialize json store
            return value == null ? default : JsonSerializer.Deserialize<T>(value); //default
        }
        public void SetSessionData<T>(string key, T value)
        {
            var session = _contextAccessor.HttpContext.Session;
            session.SetString(key, JsonSerializer.Serialize(value)); //store object in json format
            //throw new NotImplementedException();
        }
    }
}
