using MongoDB.Bson;

namespace NervousMotions.Realm.Maui.Base.Interfaces;

public interface IHaveAnObjectId
{
	ObjectId ObjectId { get; set; }
}