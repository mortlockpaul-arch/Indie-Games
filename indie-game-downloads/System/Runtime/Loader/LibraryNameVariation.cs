using System.Collections.Generic;

namespace System.Runtime.Loader;

internal struct LibraryNameVariation(string prefix, string suffix)
{
	public string Prefix = prefix;

	public string Suffix = suffix;

	internal static IEnumerable<LibraryNameVariation> DetermineLibraryNameVariations(string libName, bool isRelativePath)
	{
		yield return new LibraryNameVariation(string.Empty, string.Empty);
		if (isRelativePath && !libName.EndsWith('.') && !libName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) && !libName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
		{
			yield return new LibraryNameVariation(string.Empty, ".dll");
		}
	}
}
