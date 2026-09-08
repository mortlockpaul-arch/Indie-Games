using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public sealed class CultureAwareComparer : StringComparer, IAlternateEqualityComparer<ReadOnlySpan<char>, string?>, ISerializable
{
	internal static readonly CultureAwareComparer InvariantCaseSensitiveInstance = new CultureAwareComparer(CompareInfo.Invariant, CompareOptions.None);

	internal static readonly CultureAwareComparer InvariantIgnoreCaseInstance = new CultureAwareComparer(CompareInfo.Invariant, CompareOptions.IgnoreCase);

	private readonly CompareInfo _compareInfo;

	private readonly CompareOptions _options;

	internal CultureAwareComparer(CultureInfo culture, CompareOptions options)
		: this(culture.CompareInfo, options)
	{
	}

	internal CultureAwareComparer(CompareInfo compareInfo, CompareOptions options)
	{
		_compareInfo = compareInfo;
		if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.NumericOrdering | CompareOptions.StringSort)) != CompareOptions.None)
		{
			throw new ArgumentException(SR.Argument_InvalidFlag, "options");
		}
		_options = options;
	}

	private CultureAwareComparer(SerializationInfo info, StreamingContext context)
	{
		_compareInfo = (CompareInfo)info.GetValue("_compareInfo", typeof(CompareInfo));
		bool boolean = info.GetBoolean("_ignoreCase");
		object valueNoThrow = info.GetValueNoThrow("_options", typeof(CompareOptions));
		if (valueNoThrow != null)
		{
			_options = (CompareOptions)valueNoThrow;
		}
		_options |= (CompareOptions)(boolean ? 1 : 0);
	}

	public override int Compare(string? x, string? y)
	{
		if ((object)x == y)
		{
			return 0;
		}
		if (x == null)
		{
			return -1;
		}
		if (y == null)
		{
			return 1;
		}
		return _compareInfo.Compare(x, y, _options);
	}

	public override bool Equals(string? x, string? y)
	{
		if ((object)x == y)
		{
			return true;
		}
		if (x == null || y == null)
		{
			return false;
		}
		return _compareInfo.Compare(x, y, _options) == 0;
	}

	public override int GetHashCode(string obj)
	{
		ArgumentNullException.ThrowIfNull(obj, "obj");
		return _compareInfo.GetHashCode(obj, _options);
	}

	int IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.GetHashCode(ReadOnlySpan<char> span)
	{
		return _compareInfo.GetHashCode(span, _options);
	}

	bool IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Equals(ReadOnlySpan<char> span, string target)
	{
		if (span.IsEmpty && target == null)
		{
			return false;
		}
		return _compareInfo.Compare(span, target.AsSpan(), _options) == 0;
	}

	string IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Create(ReadOnlySpan<char> span)
	{
		return span.ToString();
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is CultureAwareComparer cultureAwareComparer && _options == cultureAwareComparer._options)
		{
			return _compareInfo.Equals(cultureAwareComparer._compareInfo);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _compareInfo.GetHashCode() ^ (int)(_options & (CompareOptions)0x7FFFFFFF);
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("_compareInfo", _compareInfo);
		info.AddValue("_options", _options);
		info.AddValue("_ignoreCase", (_options & CompareOptions.IgnoreCase) != 0);
	}

	private protected override bool IsWellKnownCultureAwareComparerCore([NotNullWhen(true)] out CompareInfo compareInfo, out CompareOptions compareOptions)
	{
		compareInfo = _compareInfo;
		compareOptions = _options;
		return true;
	}
}
