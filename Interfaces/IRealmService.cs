using Realms.Sync;

namespace NervousMotions.Realm.Maui.Base.Interfaces;

public interface IRealmService
{
    // Realm GetRealm();
    Task RegisterAsync(string email, string password);
    Task LoginAsync(string email, string password);
    Task LogoutAsync();
    User GetCurrentUser();
    Realms.Realm GetMainThreadRealm();
    Task LoginAnonymous();
}