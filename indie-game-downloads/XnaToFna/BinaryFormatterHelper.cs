using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;

namespace XnaToFna;

public static class BinaryFormatterHelper
{
	public class XnaToFnaSerializationBinderWrapper : SerializationBinder
	{
		public readonly SerializationBinder Inner;

		public XnaToFnaSerializationBinderWrapper(SerializationBinder inner)
		{
			Inner = inner;
		}

		public override Type BindToType(string assemblyName, string typeName)
		{
			if (assemblyName != "Microsoft.Xna.Framework" && !assemblyName.StartsWith("Microsoft.Xna.Framework,") && !assemblyName.StartsWith("Microsoft.Xna.Framework."))
			{
				return Inner?.BindToType(assemblyName, typeName);
			}
			return FNA.GetType(typeName);
		}

		public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			if (Inner != null)
			{
				Inner.BindToName(serializedType, out assemblyName, out typeName);
			}
			else
			{
				base.BindToName(serializedType, out assemblyName, out typeName);
			}
		}
	}

	public static readonly Assembly FNA = typeof(Game).Assembly;

	public static BinaryFormatter Create()
	{
		return new BinaryFormatter
		{
			Binder = new XnaToFnaSerializationBinderWrapper(null)
		};
	}

	public static BinaryFormatter Create(ISurrogateSelector selector, StreamingContext context)
	{
		return new BinaryFormatter(selector, context)
		{
			Binder = new XnaToFnaSerializationBinderWrapper(null)
		};
	}

	public static SerializationBinder get_Binder(this BinaryFormatter self)
	{
		return (self.Binder as XnaToFnaSerializationBinderWrapper)?.Inner ?? self.Binder;
	}

	public static void set_Binder(this BinaryFormatter self, SerializationBinder binder)
	{
		self.Binder = new XnaToFnaSerializationBinderWrapper(binder);
	}
}
