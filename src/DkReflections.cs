namespace Tool.Compet.Core {
	using System.Reflection;
	using System.Text.Json.Serialization;

	public class DkReflections {
		/// Create new object from given type T, result as `dstObj`.
		/// Then copy all properties which be annotated with `JsonPropertyNameAttribute` from `srcObj` to `dstObj`.
		public static T CloneJsonAnnotatedProperties<T>(object srcObj) where T : class {
			var dstObj = DkObjects.NewInstace<T>();
			CopyJsonAnnotatedProperties(srcObj, dstObj);
			return dstObj;
		}

		/// Copy properties which be annotated with `JsonPropertyNameAttribute` from `srcObj` to `dstObj`.
		/// Get properties: https://docs.microsoft.com/en-us/dotnet/api/system.type.getproperties
		public static void CopyJsonAnnotatedProperties(object srcObj, object dstObj) {
			var name2prop_src = new Dictionary<string, PropertyInfo>();

			// Find properties inside the source object
			foreach (var propInfo_src in srcObj.GetType().GetProperties()) {
				var jsonAttribute_src = propInfo_src.GetCustomAttribute<JsonPropertyNameAttribute>();
				if (jsonAttribute_src != null) {
					name2prop_src.Add(jsonAttribute_src.Name!, propInfo_src);
				}
			}

			// For each src-prop, we find dst-prop which name matches with name of the src-prop.
			// Then just assign value of src-prop to dst-prop.
			var propInfos_dst = dstObj.GetType().GetProperties();
			foreach (var propInfo_dst in propInfos_dst) {
				var jsonAttribute_dst = propInfo_dst.GetCustomAttribute<JsonPropertyNameAttribute>();
				if (jsonAttribute_dst != null) {
					// Set value: src -> dst
					if (name2prop_src.TryGetValue(jsonAttribute_dst.Name!, out var propInfo_src)) {
						propInfo_dst.SetValue(dstObj, propInfo_src.GetValue(srcObj));
						if (DkBuildConfig.DEBUG) { Tool.Compet.Log.DkLogs.Debug(typeof(DkReflections), $"Copied property {propInfo_src.Name}"); }
					}
				}
			}
		}
	}
}
