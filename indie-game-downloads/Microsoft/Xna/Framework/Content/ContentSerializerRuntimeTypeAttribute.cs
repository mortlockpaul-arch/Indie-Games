using System;

namespace Microsoft.Xna.Framework.Content;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ContentSerializerRuntimeTypeAttribute : Attribute
{
	public string RuntimeType { get; private set; }

	public ContentSerializerRuntimeTypeAttribute(string runtimeType)
	{
		if (string.IsNullOrEmpty(runtimeType))
		{
			throw new ArgumentNullException("runtimeType");
		}
		RuntimeType = runtimeType;
	}
}
