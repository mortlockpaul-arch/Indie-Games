using System;
using System.Xml.Linq;

namespace MS.Internal.Xml.Linq.ComponentModel;

internal sealed class XDeferredSingleton<T> where T : XObject
{
	private readonly Func<XElement, XName, T> _func;

	internal XElement element;

	internal XName name;

	public T this[string expandedName]
	{
		get
		{
			ArgumentNullException.ThrowIfNull(expandedName, "expandedName");
			if (name == null)
			{
				name = expandedName;
			}
			else if (name != expandedName)
			{
				return null;
			}
			return _func(element, name);
		}
	}

	public XDeferredSingleton(Func<XElement, XName, T> func, XElement element, XName name)
	{
		ArgumentNullException.ThrowIfNull(func, "func");
		ArgumentNullException.ThrowIfNull(element, "element");
		_func = func;
		this.element = element;
		this.name = name;
	}
}
