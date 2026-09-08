using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace XnaToFna.ProxyReflection;

public static class FieldInfoHelper
{
	public class XnaToFnaFieldInfo : FieldInfo
	{
		internal Type _DeclaringType;

		internal readonly Type _FieldType;

		internal string _Name;

		internal readonly Func<object, object> _OnGetValue;

		internal readonly Action<object, object> _OnSetValue;

		public override FieldAttributes Attributes
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override Type DeclaringType => _DeclaringType;

		public override RuntimeFieldHandle FieldHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override Type FieldType => _FieldType;

		public override string Name => _Name;

		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public XnaToFnaFieldInfo(Type fieldType, Func<object, object> onGetValue = null, Action<object, object> onSetValue = null)
		{
			_FieldType = fieldType;
			_OnGetValue = onGetValue;
			_OnSetValue = onSetValue;
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object GetValue(object obj)
		{
			return _OnGetValue?.Invoke(obj);
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
		{
			_OnSetValue?.Invoke(obj, value);
		}
	}

	private static readonly Dictionary<Type, Dictionary<string, XnaToFnaFieldInfo>> Map = new Dictionary<Type, Dictionary<string, XnaToFnaFieldInfo>> { 
	{
		typeof(StringBuilder),
		new Dictionary<string, XnaToFnaFieldInfo> { 
		{
			"m_StringValue",
			new XnaToFnaFieldInfo(typeof(string), (object obj) => ((StringBuilder)obj).ToString(), delegate(object obj, object val)
			{
				((StringBuilder)obj).Clear().Append(val);
			})
		} }
	} };

	public static FieldInfo GetField(Type self, string name, BindingFlags bindingAttr)
	{
		if (Map.TryGetValue(self, out var value) && value.TryGetValue(name, out var value2))
		{
			return value2;
		}
		return self.GetField(name, bindingAttr);
	}

	public static FieldInfo GetField(Type self, string name)
	{
		return GetField(self, name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
	}
}
