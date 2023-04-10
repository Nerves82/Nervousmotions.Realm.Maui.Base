using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using NervousMotions.Realm.Maui.Base.Interfaces;
using Realms;

namespace NervousMotions.Realm.Maui.Base
{
    public abstract class BaseDetailViewModel<T> : NervousMotions.Maui.Base.BaseViewModel where T : RealmObject, IHaveAnObjectId
    {
        public T Model { get; set; }
        public ObjectId ItemId { get; set; }
        public bool IsSaveButtonVisible { get; set; } = false;
        private readonly IRealmService _realmService;

        protected BaseDetailViewModel(IRealmService realmService)
        {
            _realmService = realmService;
        }

        public override async Task Start()
        {
            if (ItemId == default)
            {
                var newItem = Activator.CreateInstance<T>();
                var realm = _realmService.GetMainThreadRealm();
                await realm.WriteAsync(() => { realm.Add(newItem); });
                ItemId = newItem.ObjectId;
            }

            Model = _realmService.GetMainThreadRealm().Find<T>(ItemId);
        }

        public async Task SaveModel()
        {
            var realm = _realmService.GetMainThreadRealm();
            await realm.WriteAsync(() => { realm.Add(Model); });
        }

        // protected abstract bool IsSaveButtonVisibleAction(T model);

        public override async Task Stop()
        {
        }

        public override async Task Refresh()
        {
        }
    }
}