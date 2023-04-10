namespace NervousMotions.Realm.Maui.Base.Interfaces;

public interface IDataReaderService
{
	List<TModel> ReadJsonIntoRealm<TModel>(string jsonFileName);
	string ReadJsonFile(string jsonFileName);
}