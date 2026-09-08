using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Collections;

[Obsolete("CaseInsensitiveHashCodeProvider has been deprecated. Use StringComparer instead.")]
public class CaseInsensitiveHashCodeProvider : IHashCodeProvider
{
	private readonly CompareInfo _compareInfo;

	[CompilerGenerated]
	private static CaseInsensitiveHashCodeProvider _003CDefaultInvariant_003Ek__BackingField;

	public static CaseInsensitiveHashCodeProvider Default => new CaseInsensitiveHashCodeProvider();

	public static CaseInsensitiveHashCodeProvider DefaultInvariant => _003CDefaultInvariant_003Ek__BackingField ?? (_003CDefaultInvariant_003Ek__BackingField = new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture));

	public CaseInsensitiveHashCodeProvider()
	{
		_compareInfo = CultureInfo.CurrentCulture.CompareInfo;
	}

	public CaseInsensitiveHashCodeProvider(CultureInfo culture)
	{
		ArgumentNullException.ThrowIfNull(culture, "culture");
		_compareInfo = culture.CompareInfo;
	}

	public int GetHashCode(object obj)
	{
		ArgumentNullException.ThrowIfNull(obj, "obj");
		if (!(obj is string source))
		{
			return obj.GetHashCode();
		}
		return _compareInfo.GetHashCode(source, CompareOptions.IgnoreCase);
	}
}
