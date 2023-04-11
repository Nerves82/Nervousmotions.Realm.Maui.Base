
using System;
using System.Text.Json;
using System.Threading.Tasks;
using NervousMotions.Realm.Maui.Base.Interfaces;
using Realms.Sync;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NervousMotions.Realm.Maui.Base.Services;

public class RealmService : IRealmService
{
    private readonly IDataReaderService _dataReaderService;
    private bool _serviceInitialised;
    private App _app;
    private Realms.Realm _mainThreadRealm;

    public required Action<Realms.Realm> PopulateInitialSubscriptions { get; set; }

    // private readonly FlexibleSyncConfiguration.InitialSubscriptionsDelegate _populateInitialSubscriptions;
    // public required FlexibleSyncConfiguration.InitialSubscriptionsDelegate PopulateInitialSubscriptions { get; init; }

    // public RealmService(FlexibleSyncConfiguration.InitialSubscriptionsDelegate populateInitialSubscriptions)
    // {
    // 	// _populateInitialSubscriptions = populateInitialSubscriptions ?? throw new ArgumentNullException();
    // }

    public RealmService(IDataReaderService dataReaderService)
    {
        _dataReaderService = dataReaderService;
    }

    public User GetCurrentUser()
    {
        CheckInit();
        return _app?.CurrentUser;
    }

    public void Init()
    {
        try
        {
            if (_serviceInitialised)
            {
                return;
            }
            var data = _dataReaderService.ReadJsonFile("atlasConfig.json");
            var config = JsonSerializer.Deserialize<RealmAppConfig>(data,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var appConfiguration = new AppConfiguration(config.AppId)
            {
                BaseUri = new Uri(config.BaseUrl)
            };

            _app = App.Create(appConfiguration);
            _serviceInitialised = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public Realms.Realm GetMainThreadRealm()
    {
        return _mainThreadRealm ??= GetRealm();
    }

    public Realms.Realm GetRealm()
    {
        CheckInit();

        var config = new FlexibleSyncConfiguration(_app.CurrentUser)
        {
            PopulateInitialSubscriptions = realm =>
            {
                PopulateInitialSubscriptions(realm);
            }
        };

        Console.WriteLine($"Database Path {config.DatabasePath}");

        return Realms.Realm.GetInstance(config);
    }

    public async Task RegisterAsync(string email, string password)
    {
        CheckInit();
        await _app.EmailPasswordAuth.RegisterUserAsync(email, password);
    }

    public async Task LoginAsync(string email, string password)
    {
        CheckInit();

        await _app.LogInAsync(Credentials.EmailPassword(email, password));

        //This will populate the initial set of subscriptions the first time the realm is opened
        using var realm = GetRealm();
        await realm.Subscriptions.WaitForSynchronizationAsync();
    }

    public async Task LoginAnonymous()
    {
        CheckInit();
        _ = await _app.LogInAsync(Credentials.Anonymous());
        
        //This will populate the initial set of subscriptions the first time the realm is opened
        using var realm = GetRealm();
        await realm.Subscriptions.WaitForSynchronizationAsync();
    }

    private void CheckInit()
    {
        if (_serviceInitialised == false) Init();
    }

    public async Task LogoutAsync()
    {
        CheckInit();
        await _app.CurrentUser.LogOutAsync();
        _mainThreadRealm?.Dispose();
        _mainThreadRealm = null;
    }
}

public class RealmAppConfig
{
    public string AppId { get; set; }
    public string BaseUrl { get; set; }
}