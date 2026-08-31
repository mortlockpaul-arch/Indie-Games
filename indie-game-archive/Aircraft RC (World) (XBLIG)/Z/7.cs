using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace Z
{
	internal class _7
	{
		private static Dictionary<Type, Dictionary<string, PropertyInfo>> HCB = new Dictionary<Type, Dictionary<string, PropertyInfo>>(128);

		private static Dictionary<string, PropertyInfo> _0002_0001(Type P_0)
		{
			if (HCB.ContainsKey(P_0))
			{
				return HCB[P_0];
			}
			PropertyInfo[] properties = P_0.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Dictionary<string, PropertyInfo> dictionary = new Dictionary<string, PropertyInfo>(properties.Length);
			HCB.Add(P_0, dictionary);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (!(propertyInfo.Name == "UniqueId") && !(propertyInfo.Name == "CollisionMove") && ((object)propertyInfo.DeclaringType != typeof(Effect) || !(propertyInfo.Name == "CurrentTechnique")))
				{
					dictionary.Add(propertyInfo.Name, propertyInfo);
				}
			}
			return dictionary;
		}

		internal static void _0002w(object P_0, object P_1)
		{
			Type type = P_0.GetType();
			Type type2 = P_1.GetType();
			Dictionary<string, PropertyInfo> dictionary = _0002_0001(type);
			Dictionary<string, PropertyInfo> dictionary2 = _0002_0001(type2);
			foreach (string key in dictionary.Keys)
			{
				if (!dictionary.ContainsKey(key) || !dictionary2.ContainsKey(key))
				{
					continue;
				}
				PropertyInfo propertyInfo = dictionary[key];
				PropertyInfo propertyInfo2 = dictionary2[key];
				if ((object)propertyInfo.PropertyType == propertyInfo2.PropertyType && propertyInfo2.CanWrite && propertyInfo.CanRead)
				{
					MethodInfo getMethod = propertyInfo.GetGetMethod(nonPublic: true);
					MethodInfo setMethod = propertyInfo2.GetSetMethod(nonPublic: true);
					if (!getMethod.IsPrivate && !getMethod.IsFamily && !setMethod.IsPrivate && !setMethod.IsFamily)
					{
						propertyInfo2.SetValue(P_1, propertyInfo.GetValue(P_0, null), null);
					}
				}
			}
		}
	}
}
namespace z
{
	internal class _7 : SystemException
	{
		public _7()
			: base("Error occured during a cryptographic operation.")
		{
			base.HResult = -2146233296;
		}

		public _7(int hr)
		{
			base.HResult = hr;
		}

		public _7(string message)
			: base(message)
		{
			base.HResult = -2146233296;
		}

		public _7(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233296;
		}

		public _7(string format, string insert)
			: base(string.Format(format, insert))
		{
			base.HResult = -2146233296;
		}
	}
}
