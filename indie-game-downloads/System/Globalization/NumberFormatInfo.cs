using System.Runtime.CompilerServices;
using System.Text;

namespace System.Globalization;

public sealed class NumberFormatInfo : IFormatProvider, ICloneable
{
	internal static readonly string[] s_asciiDigits = new string[10] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

	internal static readonly int[] s_intArrayWithElement3 = new int[1] { 3 };

	internal int[] _numberGroupSizes = s_intArrayWithElement3;

	internal int[] _currencyGroupSizes = s_intArrayWithElement3;

	internal int[] _percentGroupSizes = s_intArrayWithElement3;

	internal string _positiveSign = "+";

	internal string _negativeSign = "-";

	internal string _numberDecimalSeparator = ".";

	internal string _numberGroupSeparator = ",";

	internal string _currencyGroupSeparator = ",";

	internal string _currencyDecimalSeparator = ".";

	internal string _currencySymbol = "¤";

	internal string _nanSymbol = "NaN";

	internal string _positiveInfinitySymbol = "Infinity";

	internal string _negativeInfinitySymbol = "-Infinity";

	internal string _percentDecimalSeparator = ".";

	internal string _percentGroupSeparator = ",";

	internal string _percentSymbol = "%";

	internal string _perMilleSymbol = "‰";

	internal byte[] _positiveSignUtf8;

	internal byte[] _negativeSignUtf8;

	internal byte[] _currencySymbolUtf8;

	internal byte[] _numberDecimalSeparatorUtf8;

	internal byte[] _currencyDecimalSeparatorUtf8;

	internal byte[] _currencyGroupSeparatorUtf8;

	internal byte[] _numberGroupSeparatorUtf8;

	internal byte[] _percentSymbolUtf8;

	internal byte[] _percentDecimalSeparatorUtf8;

	internal byte[] _percentGroupSeparatorUtf8;

	internal byte[] _perMilleSymbolUtf8;

	internal byte[] _nanSymbolUtf8;

	internal byte[] _positiveInfinitySymbolUtf8;

	internal byte[] _negativeInfinitySymbolUtf8;

	internal string[] _nativeDigits = s_asciiDigits;

	internal int _numberDecimalDigits = 2;

	internal int _currencyDecimalDigits = 2;

	internal int _currencyPositivePattern;

	internal int _currencyNegativePattern;

	internal int _numberNegativePattern = 1;

	internal int _percentPositivePattern;

	internal int _percentNegativePattern;

	internal int _percentDecimalDigits = 2;

	internal int _digitSubstitution = 1;

	internal bool _isReadOnly;

	private bool _hasInvariantNumberSigns = true;

	private bool _allowHyphenDuringParsing;

	[CompilerGenerated]
	private static NumberFormatInfo _003CInvariantInfo_003Ek__BackingField;

	internal bool HasInvariantNumberSigns => _hasInvariantNumberSigns;

	public static NumberFormatInfo InvariantInfo => _003CInvariantInfo_003Ek__BackingField ?? (_003CInvariantInfo_003Ek__BackingField = CultureInfo.InvariantCulture.NumberFormat);

	public int CurrencyDecimalDigits
	{
		get
		{
			return _currencyDecimalDigits;
		}
		set
		{
			if (value < 0 || value > 99)
			{
				ThrowHelper.ThrowArgumentOutOfRange_Range("value", value, 0, 99);
			}
			VerifyWritable();
			_currencyDecimalDigits = value;
		}
	}

	public string CurrencyDecimalSeparator
	{
		get
		{
			return _currencyDecimalSeparator;
		}
		set
		{
			VerifyWritable();
			ArgumentException.ThrowIfNullOrEmpty(value, "value");
			_currencyDecimalSeparator = value;
			_currencyDecimalSeparatorUtf8 = null;
		}
	}

	public bool IsReadOnly => _isReadOnly;

	public int[] CurrencyGroupSizes
	{
		get
		{
			return (int[])_currencyGroupSizes.Clone();
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			VerifyWritable();
			int[] array = (int[])value.Clone();
			CheckGroupSize("value", array);
			_currencyGroupSizes = array;
		}
	}

	public int[] NumberGroupSizes
	{
		get
		{
			return (int[])_numberGroupSizes.Clone();
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			VerifyWritable();
			int[] array = (int[])value.Clone();
			CheckGroupSize("value", array);
			_numberGroupSizes = array;
		}
	}

	public int[] PercentGroupSizes
	{
		get
		{
			return (int[])_percentGroupSizes.Clone();
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			VerifyWritable();
			int[] array = (int[])value.Clone();
			CheckGroupSize("value", array);
			_percentGroupSizes = array;
		}
	}

	public string CurrencyGroupSeparator
	{
		get
		{
			return _currencyGroupSeparator;
		}
		set
		{
			VerifyWritable();
			ArgumentNullException.ThrowIfNull(value, "value");
			_currencyGroupSeparator = value;
			_currencyGroupSeparatorUtf8 = null;
		}
	}

	public string CurrencySymbol
	{
		get
		{
			return _currencySymbol;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			VerifyWritable();
			_currencySymbol = value;
			_currencySymbolUtf8 = null;
		}
	}

	public static NumberFormatInfo CurrentInfo
	{
		get
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			if (!currentCulture._isInherited)
			{
				NumberFormatInfo numInfo = currentCulture._numInfo;
				if (numInfo != null)
				{
					return numInfo;
				}
			}
			return (NumberFormatInfo)currentCulture.GetFormat(typeof(NumberFormatInfo));
		}
	}

	public string NaNSymbol
	{
		get
		{
			return _nanSymbol;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			VerifyWritable();
			_nanSymbol = value;
			_nanSymbolUtf8 = null;
		}
	}

	public int CurrencyNegativePattern
	{
		get
		{
			return _currencyNegativePattern;
		}
		set
		{
			if (value < 0 || value > 16)
			{
				ThrowHelper.ThrowArgumentOutOfRange_Range("value", value, 0, 16);
			}
			VerifyWritable();
			_currencyNegativePattern = value;
		}
	}

	public int NumberNegativePattern
	{
		get
		{
			return _numberNegativePattern;
		}
		set
		{
			if (value < 0 || value > 4)
			{
				ThrowHelper.ThrowArgumentOutOfRange_Range("value", value, 0, 4);
			}
			VerifyWritable();
			_numberNegativePattern = value;
		}
	}

	public int PercentPositivePattern
	{
		get
		{
			return _percentPositivePattern;
		}
		set
		{
			if (value < 0 || value > 3)
			{
				ThrowHelper.ThrowArgumentOutOfRange_Range("value", value, 0, 3);
			}
			VerifyWritable();
			_percentPositivePattern = value;
		}
	}

	public int PercentNegativePattern
	{
		get
		{
			return _percentNegativePattern;
		}
		set
		{
			if (value < 0 || value > 11)
			{
				ThrowHelper.ThrowArgumentOutOfRange_Range("value", value, 0, 11);
			}
			VerifyWritable();
			_percentNegativePattern = value;
		}
	}

	public string NegativeInfinitySymbol
	{
		get
		{
			return _negativeInfinitySymbol;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			VerifyWritable();
			_negativeInfinitySymbol = value;
			_negativeInfinitySymbolUtf8 = null;
		}
	}

	public string NegativeSign
	{
		get
		{
			return _negativeSign;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			VerifyWritable();
			_negativeSign = value;
			_negativeSignUtf8 = null;
			InitializeInvariantAndNegativeSignFlags();
		}
	}

	public int NumberDecimalDigits
	{
		get
		{
			return _numberDecimalDigits;
		}
		set
		{
			if (value < 0 || value > 99)
			{
				ThrowHelper.ThrowArgumentOutOfRange_Range("value", value, 0, 99);
			}
			VerifyWritable();
			_numberDecimalDigits = value;
		}
	}

	public string NumberDecimalSeparator
	{
		get
		{
			return _numberDecimalSeparator;
		}
		set
		{
			VerifyWritable();
			ArgumentException.ThrowIfNullOrEmpty(value, "value");
			_numberDecimalSeparator = value;
			_numberDecimalSeparatorUtf8 = null;
		}
	}

	public string NumberGroupSeparator
	{
		get
		{
			return _numberGroupSeparator;
		}
		set
		{
			VerifyWritable();
			ArgumentNullException.ThrowIfNull(value, "value");
			_numberGroupSeparator = value;
			_numberGroupSeparatorUtf8 = null;
		}
	}

	public int CurrencyPositivePattern
	{
		get
		{
			return _currencyPositivePattern;
		}
		set
		{
			if (value < 0 || value > 3)
			{
				ThrowHelper.ThrowArgumentOutOfRange_Range("value", value, 0, 3);
			}
			VerifyWritable();
			_currencyPositivePattern = value;
		}
	}

	public string PositiveInfinitySymbol
	{
		get
		{
			return _positiveInfinitySymbol;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			VerifyWritable();
			_positiveInfinitySymbol = value;
			_positiveInfinitySymbolUtf8 = null;
		}
	}

	public string PositiveSign
	{
		get
		{
			return _positiveSign;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			VerifyWritable();
			_positiveSign = value;
			_positiveSignUtf8 = null;
			InitializeInvariantAndNegativeSignFlags();
		}
	}

	public int PercentDecimalDigits
	{
		get
		{
			return _percentDecimalDigits;
		}
		set
		{
			if (value < 0 || value > 99)
			{
				ThrowHelper.ThrowArgumentOutOfRange_Range("value", value, 0, 99);
			}
			VerifyWritable();
			_percentDecimalDigits = value;
		}
	}

	public string PercentDecimalSeparator
	{
		get
		{
			return _percentDecimalSeparator;
		}
		set
		{
			VerifyWritable();
			ArgumentException.ThrowIfNullOrEmpty(value, "value");
			_percentDecimalSeparator = value;
			_percentDecimalSeparatorUtf8 = null;
		}
	}

	public string PercentGroupSeparator
	{
		get
		{
			return _percentGroupSeparator;
		}
		set
		{
			VerifyWritable();
			ArgumentNullException.ThrowIfNull(value, "value");
			_percentGroupSeparator = value;
			_percentGroupSeparatorUtf8 = null;
		}
	}

	public string PercentSymbol
	{
		get
		{
			return _percentSymbol;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			VerifyWritable();
			_percentSymbol = value;
			_percentSymbolUtf8 = null;
		}
	}

	public string PerMilleSymbol
	{
		get
		{
			return _perMilleSymbol;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			VerifyWritable();
			_perMilleSymbol = value;
			_perMilleSymbolUtf8 = null;
		}
	}

	public string[] NativeDigits
	{
		get
		{
			return (string[])_nativeDigits.Clone();
		}
		set
		{
			VerifyWritable();
			VerifyNativeDigits(value, "value");
			_nativeDigits = value;
		}
	}

	public DigitShapes DigitSubstitution
	{
		get
		{
			return (DigitShapes)_digitSubstitution;
		}
		set
		{
			VerifyWritable();
			VerifyDigitSubstitution(value, "value");
			_digitSubstitution = (int)value;
		}
	}

	public NumberFormatInfo()
	{
	}

	private static void VerifyNativeDigits(string[] nativeDig, string propertyName)
	{
		ArgumentNullException.ThrowIfNull(nativeDig, "nativeDig");
		if (nativeDig.Length != 10)
		{
			throw new ArgumentException(SR.Argument_InvalidNativeDigitCount, propertyName);
		}
		for (int i = 0; i < nativeDig.Length; i++)
		{
			if (nativeDig[i] == null)
			{
				throw new ArgumentNullException(propertyName, SR.ArgumentNull_ArrayValue);
			}
			if (nativeDig[i].Length != 1)
			{
				if (nativeDig[i].Length != 2)
				{
					throw new ArgumentException(SR.Argument_InvalidNativeDigitValue, propertyName);
				}
				if (!char.IsSurrogatePair(nativeDig[i][0], nativeDig[i][1]))
				{
					throw new ArgumentException(SR.Argument_InvalidNativeDigitValue, propertyName);
				}
			}
			if (CharUnicodeInfo.GetDecimalDigitValue(nativeDig[i], 0) != i && CharUnicodeInfo.GetUnicodeCategory(nativeDig[i], 0) != UnicodeCategory.PrivateUse)
			{
				throw new ArgumentException(SR.Argument_InvalidNativeDigitValue, propertyName);
			}
		}
	}

	private static void VerifyDigitSubstitution(DigitShapes digitSub, string propertyName)
	{
		if ((uint)digitSub > 2u)
		{
			throw new ArgumentException(SR.Argument_InvalidDigitSubstitution, propertyName);
		}
	}

	internal bool AllowHyphenDuringParsing()
	{
		return _allowHyphenDuringParsing;
	}

	private void InitializeInvariantAndNegativeSignFlags()
	{
		_hasInvariantNumberSigns = _positiveSign == "+" && _negativeSign == "-";
		bool flag = _negativeSign.Length == 1;
		if (flag)
		{
			bool flag2;
			switch (_negativeSign[0])
			{
			case '‒':
			case '⁻':
			case '₋':
			case '−':
			case '➖':
			case '﹣':
			case '－':
				flag2 = true;
				break;
			default:
				flag2 = false;
				break;
			}
			flag = flag2;
		}
		_allowHyphenDuringParsing = flag;
	}

	internal NumberFormatInfo(CultureData cultureData)
	{
		if (cultureData != null)
		{
			cultureData.GetNFIValues(this);
			InitializeInvariantAndNegativeSignFlags();
		}
	}

	private void VerifyWritable()
	{
		if (_isReadOnly)
		{
			throw new InvalidOperationException(SR.InvalidOperation_ReadOnly);
		}
	}

	public static NumberFormatInfo GetInstance(IFormatProvider? formatProvider)
	{
		if (formatProvider != null)
		{
			return GetProviderNonNull(formatProvider);
		}
		return CurrentInfo;
		static NumberFormatInfo GetProviderNonNull(IFormatProvider provider)
		{
			if (provider.GetType() == typeof(CultureInfo))
			{
				NumberFormatInfo numInfo = ((CultureInfo)provider)._numInfo;
				if (numInfo != null)
				{
					return numInfo;
				}
			}
			return (provider as NumberFormatInfo) ?? (provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo) ?? CurrentInfo;
		}
	}

	public object Clone()
	{
		NumberFormatInfo obj = (NumberFormatInfo)MemberwiseClone();
		obj._isReadOnly = false;
		return obj;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> CurrencyDecimalSeparatorTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_currencyDecimalSeparatorUtf8 ?? (_currencyDecimalSeparatorUtf8 = Encoding.UTF8.GetBytes(_currencyDecimalSeparator))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_currencyDecimalSeparator.AsSpan());
	}

	internal static void CheckGroupSize(string propName, int[] groupSize)
	{
		for (int i = 0; i < groupSize.Length; i++)
		{
			if (groupSize[i] < 1)
			{
				if (i == groupSize.Length - 1 && groupSize[i] == 0)
				{
					break;
				}
				throw new ArgumentException(SR.Argument_InvalidGroupSize, propName);
			}
			if (groupSize[i] > 9)
			{
				throw new ArgumentException(SR.Argument_InvalidGroupSize, propName);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> CurrencyGroupSeparatorTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_currencyGroupSeparatorUtf8 ?? (_currencyGroupSeparatorUtf8 = Encoding.UTF8.GetBytes(_currencyGroupSeparator))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_currencyGroupSeparator.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> CurrencySymbolTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_currencySymbolUtf8 ?? (_currencySymbolUtf8 = Encoding.UTF8.GetBytes(_currencySymbol))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_currencySymbol.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> NaNSymbolTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_nanSymbolUtf8 ?? (_nanSymbolUtf8 = Encoding.UTF8.GetBytes(_nanSymbol))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_nanSymbol.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> NegativeInfinitySymbolTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_negativeInfinitySymbolUtf8 ?? (_negativeInfinitySymbolUtf8 = Encoding.UTF8.GetBytes(_negativeInfinitySymbol))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_negativeInfinitySymbol.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> NegativeSignTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_negativeSignUtf8 ?? (_negativeSignUtf8 = Encoding.UTF8.GetBytes(_negativeSign))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_negativeSign.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> NumberDecimalSeparatorTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_numberDecimalSeparatorUtf8 ?? (_numberDecimalSeparatorUtf8 = Encoding.UTF8.GetBytes(_numberDecimalSeparator))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_numberDecimalSeparator.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> NumberGroupSeparatorTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_numberGroupSeparatorUtf8 ?? (_numberGroupSeparatorUtf8 = Encoding.UTF8.GetBytes(_numberGroupSeparator))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_numberGroupSeparator.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> PositiveInfinitySymbolTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_positiveInfinitySymbolUtf8 ?? (_positiveInfinitySymbolUtf8 = Encoding.UTF8.GetBytes(_positiveInfinitySymbol))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_positiveInfinitySymbol.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> PositiveSignTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_positiveSignUtf8 ?? (_positiveSignUtf8 = Encoding.UTF8.GetBytes(_positiveSign))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_positiveSign.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> PercentDecimalSeparatorTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_percentDecimalSeparatorUtf8 ?? (_percentDecimalSeparatorUtf8 = Encoding.UTF8.GetBytes(_percentDecimalSeparator))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_percentDecimalSeparator.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> PercentGroupSeparatorTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_percentGroupSeparatorUtf8 ?? (_percentGroupSeparatorUtf8 = Encoding.UTF8.GetBytes(_percentGroupSeparator))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_percentGroupSeparator.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> PercentSymbolTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_percentSymbolUtf8 ?? (_percentSymbolUtf8 = Encoding.UTF8.GetBytes(_percentSymbol))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_percentSymbol.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ReadOnlySpan<TChar> PerMilleSymbolTChar<TChar>() where TChar : unmanaged, IUtfChar<TChar>
	{
		if (!(typeof(TChar) == typeof(char)))
		{
			return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)(_perMilleSymbolUtf8 ?? (_perMilleSymbolUtf8 = Encoding.UTF8.GetBytes(_perMilleSymbol))));
		}
		return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(_perMilleSymbol.AsSpan());
	}

	public object? GetFormat(Type? formatType)
	{
		if (!(formatType == typeof(NumberFormatInfo)))
		{
			return null;
		}
		return this;
	}

	public static NumberFormatInfo ReadOnly(NumberFormatInfo nfi)
	{
		ArgumentNullException.ThrowIfNull(nfi, "nfi");
		if (nfi.IsReadOnly)
		{
			return nfi;
		}
		NumberFormatInfo obj = (NumberFormatInfo)nfi.MemberwiseClone();
		obj._isReadOnly = true;
		return obj;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void ValidateParseStyleInteger(NumberStyles style)
	{
		if ((style & ~NumberStyles.Any) != NumberStyles.None && (style & ~NumberStyles.HexNumber) != NumberStyles.None && (style & ~NumberStyles.BinaryNumber) != NumberStyles.None)
		{
			ThrowInvalid(style);
		}
		static void ThrowInvalid(NumberStyles value)
		{
			throw new ArgumentException(((value & ~(NumberStyles.Any | NumberStyles.AllowHexSpecifier | NumberStyles.AllowBinarySpecifier)) != NumberStyles.None) ? SR.Argument_InvalidNumberStyles : SR.Arg_InvalidHexBinaryStyle, "style");
		}
	}

	internal static void ValidateParseStyleFloatingPoint(NumberStyles style)
	{
		if ((style & ~NumberStyles.Any) != NumberStyles.None)
		{
			ThrowInvalid(style);
		}
		static void ThrowInvalid(NumberStyles value)
		{
			throw new ArgumentException(((value & ~(NumberStyles.Any | NumberStyles.AllowHexSpecifier | NumberStyles.AllowBinarySpecifier)) != NumberStyles.None) ? SR.Argument_InvalidNumberStyles : SR.Arg_HexBinaryStylesNotSupported, "style");
		}
	}
}
