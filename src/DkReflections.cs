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
			var name2prop_src = _CollectJsonAnnotatedPropertiesRecursively(srcObj.GetType());
			var name2prop_dst = _CollectJsonAnnotatedPropertiesRecursively(dstObj.GetType());

			foreach (var item_dst in name2prop_dst) {
				// Look up at this property
				var targetPropName = item_dst.Key;

				// Copy value at the property from srcObj -> dstObj
				if (name2prop_src.TryGetValue(targetPropName, out var prop_src)) {
					item_dst.Value.SetValue(dstObj, prop_src.GetValue(srcObj));
				}
			}
		}

		public static void TrimJsonAnnotatedProperties(object obj) {
			var name2prop = _CollectJsonAnnotatedPropertiesRecursively(obj.GetType());

			foreach (var kvPair in name2prop) {
				var prop = kvPair.Value;
				var propValue = prop.GetValue(obj);
				if (propValue is string) {
					prop.SetValue(obj, ((string)(propValue)).Trim());
				}
			}
		}

		private static Dictionary<string, PropertyInfo> _CollectJsonAnnotatedPropertiesRecursively(Type type) {
			var result_name2prop = new Dictionary<string, PropertyInfo>();

			var props = type.GetProperties();
			for (var index = props.Length - 1; index >= 0; --index) {
				var prop = props[index];
				var jsonAttribute = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
				if (jsonAttribute != null) {
					// Use Set (do not use Add to avoid exception when duplicated key)
					result_name2prop[jsonAttribute.Name] = prop;
				}
			}

			var baseType = type.BaseType;
			if (baseType != null) {
				// Use Set (do not use Add to avoid exception when duplicated key)
				foreach (var name2prop in _CollectJsonAnnotatedPropertiesRecursively(baseType)) {
					result_name2prop[name2prop.Key] = name2prop.Value;
				}
			}

			return result_name2prop;
		}
	}
}
