using System.Collections.Specialized;

namespace System.Diagnostics;

internal static class TraceUtils
{
	internal static void VerifyAttributes(StringDictionary attributes, string[] supportedAttributes, object parent)
	{
		ArgumentNullException.ThrowIfNull(attributes, "attributes");
		foreach (string key in attributes.Keys)
		{
			if (supportedAttributes == null || !supportedAttributes.Contains(key))
			{
				throw new ArgumentException(System.SR.Format(System.SR.AttributeNotSupported, key, parent.GetType().FullName));
			}
		}
	}
}
