using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices.Marshalling;

[StructLayout(LayoutKind.Explicit)]
public struct ComVariant : IDisposable
{
	private struct TypeUnion
	{
		public ushort _vt;

		public ushort _wReserved1;

		public ushort _wReserved2;

		public ushort _wReserved3;

		public UnionTypes _unionTypes;
	}

	private struct Record
	{
		public nint _record;

		public nint _recordInfo;
	}

	private struct Blob
	{
		public int _size;

		public nint _data;
	}

	private struct Vector<T> where T : unmanaged
	{
		public int _numElements;

		public unsafe T* _data;
	}

	private struct VersionedStream
	{
		public Guid _version;

		public nint _stream;
	}

	private struct ClipboardData
	{
		public uint _size;

		public int _format;

		public nint _data;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct UnionTypes
	{
		[FieldOffset(0)]
		public sbyte _i1;

		[FieldOffset(0)]
		public short _i2;

		[FieldOffset(0)]
		public int _i4;

		[FieldOffset(0)]
		public long _i8;

		[FieldOffset(0)]
		public byte _ui1;

		[FieldOffset(0)]
		public ushort _ui2;

		[FieldOffset(0)]
		public uint _ui4;

		[FieldOffset(0)]
		public ulong _ui8;

		[FieldOffset(0)]
		public int _int;

		[FieldOffset(0)]
		public uint _uint;

		[FieldOffset(0)]
		public short _bool;

		[FieldOffset(0)]
		public int _error;

		[FieldOffset(0)]
		public float _r4;

		[FieldOffset(0)]
		public double _r8;

		[FieldOffset(0)]
		public long _cy;

		[FieldOffset(0)]
		public double _date;

		[FieldOffset(0)]
		public nint _bstr;

		[FieldOffset(0)]
		public nint _unknown;

		[FieldOffset(0)]
		public nint _dispatch;

		[FieldOffset(0)]
		public nint _pvarVal;

		[FieldOffset(0)]
		public nint _byref;

		[FieldOffset(0)]
		public Record _record;

		[FieldOffset(0)]
		public Blob _blob;

		[FieldOffset(0)]
		public unsafe VersionedStream* _versionedStream;

		[FieldOffset(0)]
		public unsafe ClipboardData* clipboardData;
	}

	[FieldOffset(0)]
	private TypeUnion _typeUnion;

	[FieldOffset(0)]
	private decimal _decimal;

	public static ComVariant Null { get; } = new ComVariant
	{
		VarType = VarEnum.VT_NULL
	};

	public VarEnum VarType
	{
		readonly get
		{
			return (VarEnum)_typeUnion._vt;
		}
		private set
		{
			_typeUnion._vt = (ushort)value;
		}
	}

	public unsafe void Dispose()
	{
		fixed (ComVariant* ptr = &this)
		{
			void* pObject = ptr;
			Interop.Ole32.PropVariantClear((nint)pObject);
		}
	}

	public static ComVariant Create<T>([DisallowNull] T value)
	{
		ComVariant result = default(ComVariant);
		if (typeof(T) == typeof(DBNull))
		{
			return Null;
		}
		if (typeof(T) == typeof(short))
		{
			result.VarType = VarEnum.VT_I2;
			result._typeUnion._unionTypes._i2 = (short)(object)value;
		}
		else if (typeof(T) == typeof(int))
		{
			result.VarType = VarEnum.VT_I4;
			result._typeUnion._unionTypes._i4 = (int)(object)value;
		}
		else if (typeof(T) == typeof(float))
		{
			result.VarType = VarEnum.VT_R4;
			result._typeUnion._unionTypes._r4 = (float)(object)value;
		}
		else if (typeof(T) == typeof(double))
		{
			result.VarType = VarEnum.VT_R8;
			result._typeUnion._unionTypes._r8 = (double)(object)value;
		}
		else if (typeof(T) == typeof(CurrencyWrapper))
		{
			result.VarType = VarEnum.VT_CY;
			result._typeUnion._unionTypes._cy = decimal.ToOACurrency(((CurrencyWrapper)(object)value).WrappedObject);
		}
		else if (typeof(T) == typeof(DateTime))
		{
			result.VarType = VarEnum.VT_DATE;
			result._typeUnion._unionTypes._date = ((DateTime)(object)value).ToOADate();
		}
		else if (typeof(T) == typeof(BStrWrapper))
		{
			result.VarType = VarEnum.VT_BSTR;
			result._typeUnion._unionTypes._bstr = Marshal.StringToBSTR(((BStrWrapper)(object)value).WrappedObject);
		}
		else if (typeof(T) == typeof(string))
		{
			result.VarType = VarEnum.VT_BSTR;
			result._typeUnion._unionTypes._bstr = Marshal.StringToBSTR((string)(object)value);
		}
		else if (typeof(T) == typeof(ErrorWrapper))
		{
			result.VarType = VarEnum.VT_ERROR;
			result._typeUnion._unionTypes._error = ((ErrorWrapper)(object)value).ErrorCode;
		}
		else if (typeof(T) == typeof(bool))
		{
			result.VarType = VarEnum.VT_BOOL;
			result._typeUnion._unionTypes._bool = (short)(((bool)(object)value) ? (-1) : 0);
		}
		else if (typeof(T) == typeof(decimal))
		{
			result._decimal = (decimal)(object)value;
			result.VarType = VarEnum.VT_DECIMAL;
		}
		else if (typeof(T) == typeof(sbyte))
		{
			result.VarType = VarEnum.VT_I1;
			result._typeUnion._unionTypes._i1 = (sbyte)(object)value;
		}
		else if (typeof(T) == typeof(byte))
		{
			result.VarType = VarEnum.VT_UI1;
			result._typeUnion._unionTypes._ui1 = (byte)(object)value;
		}
		else if (typeof(T) == typeof(ushort))
		{
			result.VarType = VarEnum.VT_UI2;
			result._typeUnion._unionTypes._ui2 = (ushort)(object)value;
		}
		else if (typeof(T) == typeof(uint))
		{
			result.VarType = VarEnum.VT_UI4;
			result._typeUnion._unionTypes._ui4 = (uint)(object)value;
		}
		else if (typeof(T) == typeof(long))
		{
			result.VarType = VarEnum.VT_I8;
			result._typeUnion._unionTypes._i8 = (long)(object)value;
		}
		else
		{
			if (!(typeof(T) == typeof(ulong)))
			{
				throw new ArgumentException(SR.UnsupportedType, "T");
			}
			result.VarType = VarEnum.VT_UI8;
			result._typeUnion._unionTypes._ui8 = (ulong)(object)value;
		}
		return result;
	}

	public unsafe static ComVariant CreateRaw<T>(VarEnum vt, T rawValue) where T : unmanaged
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(sizeof(T), sizeof(UnionTypes), "T");
		switch (vt)
		{
		case VarEnum.VT_DECIMAL:
			throw new ArgumentException(SR.ComVariant_VT_DECIMAL_NotSupported_CreateRaw, "vt");
		case VarEnum.VT_VARIANT:
			throw new ArgumentException(SR.ComVariant_VT_VARIANT_In_Variant, "vt");
		default:
		{
			if (!vt.HasFlag(VarEnum.VT_ARRAY) || 1 == 0)
			{
			}
			ComVariant result = new ComVariant
			{
				VarType = vt
			};
			ref T rawDataRef = ref result.GetRawDataRef<T>();
			(VarEnum, int) tuple = (vt, sizeof(T));
			var (varEnum, _) = tuple;
			T val;
			int num;
			switch (varEnum)
			{
			case VarEnum.VT_I1:
			case VarEnum.VT_UI1:
				if (tuple.Item2 != 1)
				{
					goto IL_0206;
				}
				val = rawValue;
				break;
			case VarEnum.VT_I2:
			case VarEnum.VT_BOOL:
			case VarEnum.VT_UI2:
				if (tuple.Item2 != 2)
				{
					goto IL_0206;
				}
				val = rawValue;
				break;
			case VarEnum.VT_I4:
			case VarEnum.VT_R4:
			case VarEnum.VT_ERROR:
			case VarEnum.VT_UI4:
			case VarEnum.VT_INT:
			case VarEnum.VT_UINT:
			case VarEnum.VT_HRESULT:
				if (tuple.Item2 != 4)
				{
					goto IL_0206;
				}
				val = rawValue;
				break;
			case VarEnum.VT_R8:
			case VarEnum.VT_DATE:
			case VarEnum.VT_I8:
			case VarEnum.VT_UI8:
				if (tuple.Item2 != 8)
				{
					goto IL_0206;
				}
				val = rawValue;
				break;
			case VarEnum.VT_CY:
			case VarEnum.VT_FILETIME:
				if (tuple.Item2 != 8)
				{
					goto IL_0206;
				}
				val = rawValue;
				break;
			case VarEnum.VT_BSTR:
			case VarEnum.VT_DISPATCH:
			case VarEnum.VT_UNKNOWN:
			case VarEnum.VT_SAFEARRAY:
			case VarEnum.VT_LPSTR:
			case VarEnum.VT_LPWSTR:
			case VarEnum.VT_STREAM:
			case VarEnum.VT_STORAGE:
			case VarEnum.VT_STREAMED_OBJECT:
			case VarEnum.VT_STORED_OBJECT:
			case VarEnum.VT_CF:
			case VarEnum.VT_CLSID:
			case (VarEnum)73:
				if (sizeof(T) == 8)
				{
					val = rawValue;
					break;
				}
				goto IL_0206;
			case VarEnum.VT_RECORD:
			case (VarEnum)16420:
				if (sizeof(T) == sizeof(Record))
				{
					val = rawValue;
					break;
				}
				goto IL_0206;
			default:
				{
					num = 5;
					goto IL_020c;
				}
				IL_0206:
				num = 2;
				goto IL_020c;
				IL_020c:
				if (vt.HasFlag(VarEnum.VT_BYREF) && sizeof(T) == 8)
				{
					goto IL_0238;
				}
				if (num != 2)
				{
					if (num != 5)
					{
						goto IL_0238;
					}
					num = 6;
				}
				else
				{
					num = 3;
				}
				if (vt.HasFlag(VarEnum.VT_VECTOR) && sizeof(T) == sizeof(Vector<byte>))
				{
					goto IL_0272;
				}
				if (num != 3)
				{
					if (num != 6)
					{
						goto IL_0272;
					}
					num = 7;
				}
				else
				{
					num = 4;
				}
				if (!vt.HasFlag(VarEnum.VT_ARRAY) || sizeof(T) != 8)
				{
					if (num != 4)
					{
						if (num != 7)
						{
							goto IL_02ab;
						}
						if ((varEnum == VarEnum.VT_BLOB || varEnum == VarEnum.VT_BLOB_OBJECT) && sizeof(T) == sizeof(Blob))
						{
							val = rawValue;
							break;
						}
					}
					throw new ArgumentException(SR.Format(SR.ComVariant_SizeMustMatchVariantSize, "T", "vt"));
				}
				goto IL_02ab;
				IL_0272:
				val = rawValue;
				break;
				IL_02ab:
				val = rawValue;
				break;
				IL_0238:
				val = rawValue;
				break;
			}
			rawDataRef = val;
			return result;
		}
		}
	}

	private readonly void ThrowIfNotVarType(params VarEnum[] requiredType)
	{
		if (Array.IndexOf(requiredType, VarType) == -1)
		{
			throw new InvalidOperationException(SR.Format(SR.ComVariant_TypeIsNotSupportedType, VarType, string.Join(", ", requiredType)));
		}
	}

	public readonly T? As<T>()
	{
		if (VarType == VarEnum.VT_EMPTY)
		{
			return default(T);
		}
		if (typeof(T) == typeof(DBNull))
		{
			ThrowIfNotVarType(VarEnum.VT_NULL);
			return (T)(object)DBNull.Value;
		}
		if (typeof(T) == typeof(short))
		{
			ThrowIfNotVarType(VarEnum.VT_I2);
			return (T)(object)_typeUnion._unionTypes._i2;
		}
		if (typeof(T) == typeof(int))
		{
			ThrowIfNotVarType(VarEnum.VT_I4, VarEnum.VT_ERROR, VarEnum.VT_INT);
			return (T)(object)_typeUnion._unionTypes._i4;
		}
		if (typeof(T) == typeof(float))
		{
			ThrowIfNotVarType(VarEnum.VT_R4);
			return (T)(object)_typeUnion._unionTypes._r4;
		}
		if (typeof(T) == typeof(double))
		{
			ThrowIfNotVarType(VarEnum.VT_R8);
			return (T)(object)_typeUnion._unionTypes._r8;
		}
		if (typeof(T) == typeof(CurrencyWrapper))
		{
			ThrowIfNotVarType(VarEnum.VT_CY);
			return (T)(object)new CurrencyWrapper(decimal.FromOACurrency(_typeUnion._unionTypes._cy));
		}
		if (typeof(T) == typeof(DateTime))
		{
			ThrowIfNotVarType(VarEnum.VT_DATE);
			return (T)(object)DateTime.FromOADate(_typeUnion._unionTypes._date);
		}
		if (typeof(T) == typeof(BStrWrapper))
		{
			ThrowIfNotVarType(VarEnum.VT_BSTR);
			return (T)(object)new BStrWrapper(Marshal.PtrToStringBSTR(_typeUnion._unionTypes._bstr));
		}
		if (typeof(T) == typeof(string))
		{
			ThrowIfNotVarType(VarEnum.VT_BSTR);
			if (_typeUnion._unionTypes._bstr == IntPtr.Zero)
			{
				return default(T);
			}
			return (T)(object)Marshal.PtrToStringBSTR(_typeUnion._unionTypes._bstr);
		}
		if (typeof(T) == typeof(ErrorWrapper))
		{
			ThrowIfNotVarType(VarEnum.VT_ERROR);
			return (T)(object)new ErrorWrapper(_typeUnion._unionTypes._error);
		}
		if (typeof(T) == typeof(bool))
		{
			ThrowIfNotVarType(VarEnum.VT_BOOL);
			return (T)(object)(_typeUnion._unionTypes._bool != 0);
		}
		if (typeof(T) == typeof(decimal))
		{
			ThrowIfNotVarType(VarEnum.VT_DECIMAL);
			ComVariant comVariant = this;
			comVariant.VarType = VarEnum.VT_EMPTY;
			return (T)(object)comVariant._decimal;
		}
		if (typeof(T) == typeof(sbyte))
		{
			ThrowIfNotVarType(VarEnum.VT_I1);
			return (T)(object)_typeUnion._unionTypes._i1;
		}
		if (typeof(T) == typeof(byte))
		{
			ThrowIfNotVarType(VarEnum.VT_UI1);
			return (T)(object)_typeUnion._unionTypes._ui1;
		}
		if (typeof(T) == typeof(ushort))
		{
			ThrowIfNotVarType(VarEnum.VT_UI2);
			return (T)(object)_typeUnion._unionTypes._ui2;
		}
		if (typeof(T) == typeof(uint))
		{
			ThrowIfNotVarType(VarEnum.VT_UI4, VarEnum.VT_UINT);
			return (T)(object)_typeUnion._unionTypes._ui4;
		}
		if (typeof(T) == typeof(long))
		{
			ThrowIfNotVarType(VarEnum.VT_I8);
			return (T)(object)_typeUnion._unionTypes._i8;
		}
		if (typeof(T) == typeof(ulong))
		{
			ThrowIfNotVarType(VarEnum.VT_UI8);
			return (T)(object)_typeUnion._unionTypes._ui8;
		}
		throw new ArgumentException(SR.UnsupportedType, "T");
	}

	[UnscopedRef]
	public unsafe ref T GetRawDataRef<T>() where T : unmanaged
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(sizeof(T), sizeof(UnionTypes), "T");
		if (typeof(T) == typeof(decimal))
		{
			throw new ArgumentException(SR.ComVariant_VT_DECIMAL_NotSupported_RawDataRef, "T");
		}
		return ref Unsafe.As<UnionTypes, T>(ref _typeUnion._unionTypes);
	}
}
