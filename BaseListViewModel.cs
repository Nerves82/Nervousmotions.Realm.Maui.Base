using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Realms;

namespace NervousMotions.Realm.Maui.Base
{
    public class BaseListViewModel<T> : NervousMotions.Maui.Base.BaseViewModel where T : IRealmObject
    {
        private readonly Interfaces.IRealmService _realmService;
        public IEnumerable<T> ItemsList { get; private set; }
        public bool IsEmptyViewVisible { get; set; } = true;

        public BaseListViewModel(Interfaces.IRealmService realmService)
        {
            _realmService = realmService;
        }

        public override async Task Start()
        {
            var realm = _realmService.GetMainThreadRealm();
            ItemsList = realm.All<T>();
            IsEmptyViewVisible = !ItemsList.Any();
        }

        public override async Task Stop()
        {
        }

        public override async Task Refresh()
        {
        }
    }
}