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
			var name2prop_src = CollectPropertiesRecursively(srcObj.GetType());
			var name2prop_dst = CollectPropertiesRecursively(dstObj.GetType());

			foreach (var item_dst in name2prop_dst) {
				// Look up at this property
				var targetPropName = item_dst.Key;

				// Copy value at the property from srcObj -> dstObj
				if (name2prop_src.TryGetValue(targetPropName, out var prop_src)) {
					item_dst.Value.SetValue(dstObj, prop_src.GetValue(srcObj));
				}
			}
		}

		private static Dictionary<string, PropertyInfo> CollectPropertiesRecursively(Type type) {
			var name2prop = new Dictionary<string, PropertyInfo>();

			var props = type.GetProperties();
			for (var index = props.Length - 1; index >= 0; --index) {
				var prop = props[index];
				var jsonAttribute = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
				if (jsonAttribute != null) {
					// Set (not add to avoid exception when duplicated key)
					name2prop[jsonAttribute.Name] = prop;
				}
			}

			var baseType = type.BaseType;
			if (baseType != null) {
				// Set (not add to avoid exception when duplicated key)
				foreach (var name_prop in CollectPropertiesRecursively(baseType)) {
					name2prop[name_prop.Key] = name_prop.Value;
				}
			}

			return name2prop;
		}
	}
}
