
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NervousMotions.Realm.Maui.Base.Interfaces;
using Newtonsoft.Json;

namespace NervousMotions.Realm.Maui.Base.Services
{

	public class DataReaderService : IDataReaderService
	{
		public List<TModel> ReadJsonIntoRealm<TModel>(string jsonFileName)
		{
			var jsonString = ReadJsonFile(jsonFileName);
			var list = JsonConvert.DeserializeObject<List<TModel>>(jsonString);
			return list;
		}

		public string ReadJsonFile(string jsonFileName)
		{
			var assembly = typeof(DataReaderService).GetTypeInfo().Assembly;
			var stream =
				assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{jsonFileName}");
			using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
			var jsonString = reader.ReadToEnd();
			return jsonString;
		}
	}
}