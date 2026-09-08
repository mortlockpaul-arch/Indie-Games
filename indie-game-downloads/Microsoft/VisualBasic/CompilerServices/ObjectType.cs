using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ObjectType
{
	private enum VType
	{
		t_bad,
		t_bool,
		t_ui1,
		t_i2,
		t_i4,
		t_i8,
		t_dec,
		t_r4,
		t_r8,
		t_char,
		t_str,
		t_date
	}

	private enum VType2
	{
		t_bad,
		t_bool,
		t_ui1,
		t_char,
		t_i2,
		t_i4,
		t_i8,
		t_r4,
		t_r8,
		t_date,
		t_dec,
		t_ref,
		t_str
	}

	private enum CC : byte
	{
		Err,
		Same,
		Narr,
		Wide
	}

	private static readonly VType[,] WiderType = new VType[12, 12]
	{
		{
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad
		},
		{
			VType.t_bad,
			VType.t_bool,
			VType.t_bool,
			VType.t_i2,
			VType.t_i4,
			VType.t_i8,
			VType.t_dec,
			VType.t_r4,
			VType.t_r8,
			VType.t_bad,
			VType.t_r8,
			VType.t_bad
		},
		{
			VType.t_bad,
			VType.t_bool,
			VType.t_ui1,
			VType.t_i2,
			VType.t_i4,
			VType.t_i8,
			VType.t_dec,
			VType.t_r4,
			VType.t_r8,
			VType.t_bad,
			VType.t_r8,
			VType.t_bad
		},
		{
			VType.t_bad,
			VType.t_i2,
			VType.t_i2,
			VType.t_i2,
			VType.t_i4,
			VType.t_i8,
			VType.t_dec,
			VType.t_r4,
			VType.t_r8,
			VType.t_bad,
			VType.t_r8,
			VType.t_bad
		},
		{
			VType.t_bad,
			VType.t_i4,
			VType.t_i4,
			VType.t_i4,
			VType.t_i4,
			VType.t_i8,
			VType.t_dec,
			VType.t_r4,
			VType.t_r8,
			VType.t_bad,
			VType.t_r8,
			VType.t_bad
		},
		{
			VType.t_bad,
			VType.t_i8,
			VType.t_i8,
			VType.t_i8,
			VType.t_i8,
			VType.t_i8,
			VType.t_dec,
			VType.t_r4,
			VType.t_r8,
			VType.t_bad,
			VType.t_r8,
			VType.t_bad
		},
		{
			VType.t_bad,
			VType.t_dec,
			VType.t_dec,
			VType.t_dec,
			VType.t_dec,
			VType.t_dec,
			VType.t_dec,
			VType.t_r4,
			VType.t_r8,
			VType.t_bad,
			VType.t_r8,
			VType.t_bad
		},
		{
			VType.t_bad,
			VType.t_r4,
			VType.t_r4,
			VType.t_r4,
			VType.t_r4,
			VType.t_r4,
			VType.t_r4,
			VType.t_r4,
			VType.t_r8,
			VType.t_bad,
			VType.t_r8,
			VType.t_bad
		},
		{
			VType.t_bad,
			VType.t_r8,
			VType.t_r8,
			VType.t_r8,
			VType.t_r8,
			VType.t_r8,
			VType.t_r8,
			VType.t_r8,
			VType.t_r8,
			VType.t_bad,
			VType.t_r8,
			VType.t_bad
		},
		{
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_char,
			VType.t_str,
			VType.t_bad
		},
		{
			VType.t_bad,
			VType.t_r8,
			VType.t_r8,
			VType.t_r8,
			VType.t_r8,
			VType.t_r8,
			VType.t_r8,
			VType.t_r8,
			VType.t_r8,
			VType.t_str,
			VType.t_str,
			VType.t_date
		},
		{
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_bad,
			VType.t_date,
			VType.t_date
		}
	};

	private static readonly CC[,] ConversionClassTable = new CC[13, 13]
	{
		{
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err
		},
		{
			CC.Err,
			CC.Same,
			CC.Narr,
			CC.Err,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Err,
			CC.Narr,
			CC.Err,
			CC.Narr
		},
		{
			CC.Err,
			CC.Narr,
			CC.Same,
			CC.Err,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Err,
			CC.Narr,
			CC.Err,
			CC.Narr
		},
		{
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Same,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Narr
		},
		{
			CC.Err,
			CC.Narr,
			CC.Wide,
			CC.Err,
			CC.Same,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Err,
			CC.Narr,
			CC.Err,
			CC.Narr
		},
		{
			CC.Err,
			CC.Narr,
			CC.Wide,
			CC.Err,
			CC.Wide,
			CC.Same,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Err,
			CC.Narr,
			CC.Err,
			CC.Narr
		},
		{
			CC.Err,
			CC.Narr,
			CC.Wide,
			CC.Err,
			CC.Wide,
			CC.Wide,
			CC.Same,
			CC.Narr,
			CC.Narr,
			CC.Err,
			CC.Narr,
			CC.Err,
			CC.Narr
		},
		{
			CC.Err,
			CC.Narr,
			CC.Wide,
			CC.Err,
			CC.Wide,
			CC.Wide,
			CC.Wide,
			CC.Same,
			CC.Narr,
			CC.Err,
			CC.Wide,
			CC.Err,
			CC.Narr
		},
		{
			CC.Err,
			CC.Narr,
			CC.Wide,
			CC.Err,
			CC.Wide,
			CC.Wide,
			CC.Wide,
			CC.Wide,
			CC.Same,
			CC.Err,
			CC.Wide,
			CC.Err,
			CC.Narr
		},
		{
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Same,
			CC.Err,
			CC.Err,
			CC.Narr
		},
		{
			CC.Err,
			CC.Narr,
			CC.Wide,
			CC.Err,
			CC.Wide,
			CC.Wide,
			CC.Wide,
			CC.Narr,
			CC.Narr,
			CC.Err,
			CC.Same,
			CC.Err,
			CC.Narr
		},
		{
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err,
			CC.Err
		},
		{
			CC.Err,
			CC.Narr,
			CC.Narr,
			CC.Wide,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Narr,
			CC.Err,
			CC.Same
		}
	};

	private static VType VTypeFromTypeCode(TypeCode typ)
	{
		return typ switch
		{
			TypeCode.Boolean => VType.t_bool, 
			TypeCode.Byte => VType.t_ui1, 
			TypeCode.Int16 => VType.t_i2, 
			TypeCode.Int32 => VType.t_i4, 
			TypeCode.Int64 => VType.t_i8, 
			TypeCode.Decimal => VType.t_dec, 
			TypeCode.Single => VType.t_r4, 
			TypeCode.Double => VType.t_r8, 
			TypeCode.Char => VType.t_char, 
			TypeCode.String => VType.t_str, 
			TypeCode.DateTime => VType.t_date, 
			_ => VType.t_bad, 
		};
	}

	private static VType2 VType2FromTypeCode(TypeCode typ)
	{
		return typ switch
		{
			TypeCode.Boolean => VType2.t_bool, 
			TypeCode.Byte => VType2.t_ui1, 
			TypeCode.Int16 => VType2.t_i2, 
			TypeCode.Int32 => VType2.t_i4, 
			TypeCode.Int64 => VType2.t_i8, 
			TypeCode.Decimal => VType2.t_dec, 
			TypeCode.Single => VType2.t_r4, 
			TypeCode.Double => VType2.t_r8, 
			TypeCode.Char => VType2.t_char, 
			TypeCode.String => VType2.t_str, 
			TypeCode.DateTime => VType2.t_date, 
			_ => VType2.t_bad, 
		};
	}

	private static TypeCode TypeCodeFromVType(VType vartyp)
	{
		return vartyp switch
		{
			VType.t_bool => TypeCode.Boolean, 
			VType.t_ui1 => TypeCode.Byte, 
			VType.t_i2 => TypeCode.Int16, 
			VType.t_i4 => TypeCode.Int32, 
			VType.t_i8 => TypeCode.Int64, 
			VType.t_dec => TypeCode.Decimal, 
			VType.t_r4 => TypeCode.Single, 
			VType.t_r8 => TypeCode.Double, 
			VType.t_char => TypeCode.Char, 
			VType.t_str => TypeCode.String, 
			VType.t_date => TypeCode.DateTime, 
			_ => TypeCode.Object, 
		};
	}

	internal static Type TypeFromTypeCode(TypeCode vartyp)
	{
		return vartyp switch
		{
			TypeCode.Boolean => typeof(bool), 
			TypeCode.Byte => typeof(byte), 
			TypeCode.Int16 => typeof(short), 
			TypeCode.Int32 => typeof(int), 
			TypeCode.Int64 => typeof(long), 
			TypeCode.Decimal => typeof(decimal), 
			TypeCode.Single => typeof(float), 
			TypeCode.Double => typeof(double), 
			TypeCode.Char => typeof(char), 
			TypeCode.String => typeof(string), 
			TypeCode.DateTime => typeof(DateTime), 
			TypeCode.SByte => typeof(sbyte), 
			TypeCode.UInt16 => typeof(ushort), 
			TypeCode.UInt32 => typeof(uint), 
			TypeCode.UInt64 => typeof(ulong), 
			TypeCode.Object => typeof(object), 
			TypeCode.DBNull => typeof(DBNull), 
			_ => null, 
		};
	}

	internal static bool IsWiderNumeric(Type Type1, Type Type2)
	{
		TypeCode typeCode = Type.GetTypeCode(Type1);
		TypeCode typeCode2 = Type.GetTypeCode(Type2);
		if (Information.IsOldNumericTypeCode(typeCode) && Information.IsOldNumericTypeCode(typeCode2))
		{
			if (typeCode == TypeCode.Boolean || typeCode2 == TypeCode.Boolean)
			{
				return false;
			}
			if (Type1.IsEnum)
			{
				return false;
			}
			return WiderType[(int)VTypeFromTypeCode(typeCode), (int)VTypeFromTypeCode(typeCode2)] == VTypeFromTypeCode(typeCode);
		}
		return false;
	}

	internal static bool IsWideningConversion(Type FromType, Type ToType)
	{
		TypeCode typeCode = Type.GetTypeCode(FromType);
		TypeCode typeCode2 = Type.GetTypeCode(ToType);
		if (typeCode == TypeCode.Object)
		{
			if ((object)FromType == typeof(char[]) && (typeCode2 == TypeCode.String || (object)ToType == typeof(char[])))
			{
				return true;
			}
			if (typeCode2 == TypeCode.Object)
			{
				if (FromType.IsArray && ToType.IsArray)
				{
					if (FromType.GetArrayRank() == ToType.GetArrayRank())
					{
						return ToType.GetElementType().IsAssignableFrom(FromType.GetElementType());
					}
					return false;
				}
				return ToType.IsAssignableFrom(FromType);
			}
			return false;
		}
		if (typeCode2 == TypeCode.Object)
		{
			if ((object)ToType == typeof(char[]) && typeCode == TypeCode.String)
			{
				return false;
			}
			return ToType.IsAssignableFrom(FromType);
		}
		if (ToType.IsEnum)
		{
			return false;
		}
		CC cC = ConversionClassTable[(int)VType2FromTypeCode(typeCode2), (int)VType2FromTypeCode(typeCode)];
		return cC == CC.Wide || cC == CC.Same;
	}

	internal static TypeCode GetWidestType(object obj1, object obj2, bool IsAdd = false)
	{
		IConvertible convertible = obj1 as IConvertible;
		IConvertible convertible2 = obj2 as IConvertible;
		TypeCode typeCode = (TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((obj1 != null) ? ((!(obj1 is char[]) || ((Array)obj1).Rank != 1) ? 1 : 18) : 0));
		TypeCode typeCode2 = (TypeCode)(((int?)convertible2?.GetTypeCode()) ?? ((obj2 != null) ? ((!(obj2 is char[]) || ((Array)obj2).Rank != 1) ? 1 : 18) : 0));
		if (obj1 == null)
		{
			return typeCode2;
		}
		if (obj2 == null)
		{
			return typeCode;
		}
		if (IsAdd && ((typeCode == TypeCode.DBNull && typeCode2 == TypeCode.String) || (typeCode == TypeCode.String && typeCode2 == TypeCode.DBNull)))
		{
			return TypeCode.DBNull;
		}
		return TypeCodeFromVType(WiderType[(int)VTypeFromTypeCode(typeCode), (int)VTypeFromTypeCode(typeCode2)]);
	}

	public static int ObjTst(object o1, object o2, bool TextCompare)
	{
		IConvertible convertible = o1 as IConvertible;
		TypeCode typeCode = (TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((o1 != null) ? 1 : 0));
		IConvertible convertible2 = o2 as IConvertible;
		TypeCode typeCode2 = (TypeCode)(((int?)convertible2?.GetTypeCode()) ?? ((o2 != null) ? 1 : 0));
		if (typeCode == TypeCode.Object && o1 is char[] && (typeCode2 == TypeCode.String || typeCode2 == TypeCode.Empty || (typeCode2 == TypeCode.Object && o2 is char[])))
		{
			o1 = new string(CharArrayType.FromObject(o1));
			convertible = (IConvertible)o1;
			typeCode = TypeCode.String;
		}
		if (typeCode2 == TypeCode.Object && o2 is char[] && (typeCode == TypeCode.String || typeCode == TypeCode.Empty))
		{
			o2 = new string(CharArrayType.FromObject(o2));
			convertible2 = (IConvertible)o2;
			typeCode2 = TypeCode.String;
		}
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 18:
			return ObjTstStringString(null, o2.ToString(), TextCompare);
		case 342:
			return ObjTstStringString(o1.ToString(), null, TextCompare);
		case 0:
			return 0;
		case 114:
			return ObjTstByte(convertible.ToByte(null), 0);
		case 6:
			return ObjTstByte(0, convertible2.ToByte(null));
		case 57:
			return ObjTstInt32(ToVBBool(convertible), 0);
		case 3:
			return ObjTstInt32(0, ToVBBool(convertible2));
		case 133:
			return ObjTstInt16(convertible.ToInt16(null), 0);
		case 7:
			return ObjTstInt16(0, convertible2.ToInt16(null));
		case 171:
			return ObjTstInt32(convertible.ToInt32(null), 0);
		case 9:
			return ObjTstInt32(0, convertible2.ToInt32(null));
		case 209:
			return ObjTstInt64(convertible.ToInt64(null), 0L);
		case 11:
			return ObjTstInt64(0L, convertible2.ToInt64(null));
		case 247:
			return ObjTstSingle(convertible.ToSingle(null), 0f);
		case 13:
			return ObjTstSingle(0f, convertible2.ToSingle(null));
		case 266:
			return ObjTstDouble(convertible.ToDouble(null), 0.0);
		case 14:
			return ObjTstDouble(0.0, convertible2.ToDouble(null));
		case 285:
			return ObjTstDecimal(convertible, 0);
		case 15:
			return ObjTstDecimal(0, convertible2);
		case 76:
			return ObjTstChar(convertible.ToChar(null), '\0');
		case 4:
			return ObjTstChar('\0', convertible2.ToChar(null));
		case 304:
			return ObjTstDateTime(convertible.ToDateTime(null), DateType.FromObject(null));
		case 16:
			return ObjTstDateTime(DateType.FromObject(null), convertible2.ToDateTime(null));
		case 129:
		case 148:
		case 186:
		case 224:
		case 291:
		case 292:
		case 294:
		case 296:
		case 300:
			return ObjTstDecimal(convertible, convertible2);
		case 72:
			return ObjTstDecimal(ToVBBool(convertible), convertible2);
		case 288:
			return ObjTstDecimal(convertible, ToVBBool(convertible2));
		case 348:
		case 349:
		case 351:
		case 353:
		case 355:
		case 356:
		case 357:
			return ObjTstString(convertible, typeCode, convertible2, typeCode2);
		case 132:
		case 151:
		case 189:
		case 227:
		case 265:
		case 284:
		case 303:
			return ObjTstString(convertible, typeCode, convertible2, typeCode2);
		case 320:
			return ObjTstDateTime(convertible.ToDateTime(null), convertible2.ToDateTime(null));
		case 358:
			return ObjTstDateTime(DateType.FromString(convertible.ToString(null), Utils.GetCultureInfo()), convertible2.ToDateTime(null));
		case 322:
			return ObjTstDateTime(convertible.ToDateTime(null), DateType.FromString(convertible2.ToString(null), Utils.GetCultureInfo()));
		case 360:
			return ObjTstStringString(convertible.ToString(null), convertible2.ToString(null), TextCompare);
		case 75:
			return ObjTstBoolean(convertible.ToBoolean(null), BooleanType.FromString(convertible2.ToString(null)));
		case 345:
			return ObjTstBoolean(BooleanType.FromString(convertible.ToString(null)), convertible2.ToBoolean(null));
		case 94:
		case 346:
			return ObjTstStringString(convertible.ToString(null), convertible2.ToString(null), TextCompare);
		case 128:
		case 147:
		case 185:
		case 223:
		case 261:
		case 272:
		case 273:
		case 275:
		case 277:
		case 279:
		case 280:
		case 281:
		case 299:
			return ObjTstDouble(convertible.ToDouble(null), convertible2.ToDouble(null));
		case 269:
			return ObjTstDouble(convertible.ToDouble(null), ToVBBool(convertible2));
		case 71:
			return ObjTstDouble(ToVBBool(convertible), convertible2.ToDouble(null));
		case 127:
		case 146:
		case 184:
		case 222:
		case 253:
		case 254:
		case 256:
		case 258:
		case 260:
		case 262:
		case 298:
			return ObjTstSingle(convertible.ToSingle(null), convertible2.ToSingle(null));
		case 250:
			return ObjTstSingle(convertible.ToSingle(null), ToVBBool(convertible2));
		case 70:
			return ObjTstSingle(ToVBBool(convertible), convertible2.ToSingle(null));
		case 125:
		case 144:
		case 182:
		case 215:
		case 216:
		case 218:
		case 220:
			return ObjTstInt64(convertible.ToInt64(null), convertible2.ToInt64(null));
		case 212:
			return ObjTstInt64(convertible.ToInt64(null), ToVBBool(convertible2));
		case 68:
			return ObjTstInt64(ToVBBool(convertible), convertible2.ToInt64(null));
		case 123:
		case 142:
		case 177:
		case 178:
		case 180:
			return ObjTstInt32(convertible.ToInt32(null), convertible2.ToInt32(null));
		case 174:
			return ObjTstInt32(convertible.ToInt32(null), ToVBBool(convertible2));
		case 66:
			return ObjTstInt32(ToVBBool(convertible), convertible2.ToInt32(null));
		case 121:
		case 139:
		case 140:
			return ObjTstInt16(convertible.ToInt16(null), convertible2.ToInt16(null));
		case 63:
		case 64:
			return ObjTstInt16(checked((short)ToVBBool(convertible)), convertible2.ToInt16(null));
		case 117:
		case 136:
			return ObjTstInt16(convertible.ToInt16(null), checked((short)ToVBBool(convertible2)));
		case 60:
			return checked(ObjTstInt16((short)ToVBBool(convertible), (short)ToVBBool(convertible2)));
		case 120:
			return ObjTstByte(convertible.ToByte(null), convertible2.ToByte(null));
		case 80:
			return ObjTstChar(convertible.ToChar(null), convertible2.ToChar(null));
		default:
			throw GetNoValidOperatorException(o1, o2);
		}
	}

	private static int ObjTstDateTime(DateTime var1, DateTime var2)
	{
		long ticks = var1.Ticks;
		long ticks2 = var2.Ticks;
		if (ticks < ticks2)
		{
			return -1;
		}
		if (ticks > ticks2)
		{
			return 1;
		}
		return 0;
	}

	private static int ObjTstBoolean(bool b1, bool b2)
	{
		if (b1 == b2)
		{
			return 0;
		}
		if ((b1 ? 1 : 0) < (b2 ? 1 : 0))
		{
			return 1;
		}
		return -1;
	}

	private static int ObjTstDouble(double d1, double d2)
	{
		if (d1 < d2)
		{
			return -1;
		}
		if (d1 > d2)
		{
			return 1;
		}
		return 0;
	}

	private static int ObjTstChar(char ch1, char ch2)
	{
		if (ch1 < ch2)
		{
			return -1;
		}
		if (ch1 > ch2)
		{
			return 1;
		}
		return 0;
	}

	private static int ObjTstByte(byte by1, byte by2)
	{
		if ((uint)by1 < (uint)by2)
		{
			return -1;
		}
		if ((uint)by1 > (uint)by2)
		{
			return 1;
		}
		return 0;
	}

	private static int ObjTstSingle(float d1, float d2)
	{
		if (d1 < d2)
		{
			return -1;
		}
		if (d1 > d2)
		{
			return 1;
		}
		return 0;
	}

	private static int ObjTstInt16(short d1, short d2)
	{
		if (d1 < d2)
		{
			return -1;
		}
		if (d1 > d2)
		{
			return 1;
		}
		return 0;
	}

	private static int ObjTstInt32(int d1, int d2)
	{
		if (d1 < d2)
		{
			return -1;
		}
		if (d1 > d2)
		{
			return 1;
		}
		return 0;
	}

	private static int ObjTstInt64(long d1, long d2)
	{
		if (d1 < d2)
		{
			return -1;
		}
		if (d1 > d2)
		{
			return 1;
		}
		return 0;
	}

	private static int ObjTstDecimal(IConvertible i1, IConvertible i2)
	{
		decimal d = i1.ToDecimal(null);
		decimal d2 = i2.ToDecimal(null);
		if (decimal.Compare(d, d2) < 0)
		{
			return -1;
		}
		if (decimal.Compare(d, d2) > 0)
		{
			return 1;
		}
		return 0;
	}

	private static int ObjTstString(IConvertible conv1, TypeCode tc1, IConvertible conv2, TypeCode tc2)
	{
		return ObjTstDouble(tc1 switch
		{
			TypeCode.String => DoubleType.FromString(conv1.ToString(null)), 
			TypeCode.Boolean => ToVBBool(conv1), 
			_ => conv1.ToDouble(null), 
		}, tc2 switch
		{
			TypeCode.String => DoubleType.FromString(conv2.ToString(null)), 
			TypeCode.Boolean => ToVBBool(conv2), 
			_ => conv2.ToDouble(null), 
		});
	}

	private static int ObjTstStringString(string s1, string s2, bool TextCompare)
	{
		if (s1 == null)
		{
			if (s2.Length > 0)
			{
				return -1;
			}
			return 0;
		}
		if (s2 == null)
		{
			if (s1.Length > 0)
			{
				return 1;
			}
			return 0;
		}
		if (TextCompare)
		{
			return Utils.GetCultureInfo().CompareInfo.Compare(s1, s2, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
		}
		return string.CompareOrdinal(s1, s2);
	}

	public static object PlusObj(object obj)
	{
		if (obj == null)
		{
			return 0;
		}
		IConvertible convertible = obj as IConvertible;
		switch ((TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((obj != null) ? 1 : 0)))
		{
		case TypeCode.Boolean:
			if (obj is bool)
			{
				return (short)(0 - (((bool)obj) ? 1 : 0));
			}
			return (short)(0 - (convertible.ToBoolean(null) ? 1 : 0));
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.Int32:
		case TypeCode.Int64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			return obj;
		case TypeCode.String:
			return DoubleType.FromObject(obj);
		case TypeCode.Empty:
			return 0;
		default:
			throw GetNoValidOperatorException(obj);
		}
	}

	public static object NegObj(object obj)
	{
		IConvertible convertible = obj as IConvertible;
		TypeCode tc = (TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((obj != null) ? 1 : 0));
		return InternalNegObj(obj, convertible, tc);
	}

	private static object InternalNegObj(object obj, IConvertible conv, TypeCode tc)
	{
		checked
		{
			short num5;
			decimal num3;
			double num;
			switch (tc)
			{
			case TypeCode.Empty:
				return 0;
			case TypeCode.Boolean:
				num5 = ((!(obj is bool)) ? ((short)unchecked(-(short)(0 - (conv.ToBoolean(null) ? 1 : 0)))) : ((short)unchecked(-(short)(0 - (((bool)obj) ? 1 : 0)))));
				goto IL_0263;
			case TypeCode.Byte:
				num5 = ((!(obj is byte)) ? ((short)unchecked(-conv.ToByte(null))) : ((short)unchecked(-(byte)obj)));
				goto IL_0263;
			case TypeCode.Int16:
			{
				int num4 = ((!(obj is short)) ? (-conv.ToInt16(null)) : (-(short)obj));
				if (num4 < -32768 || num4 > 32767)
				{
					return num4;
				}
				return (short)num4;
			}
			case TypeCode.Int32:
			{
				long num2 = ((!(obj is int)) ? (-conv.ToInt32(null)) : (-(int)obj));
				if (num2 < int.MinValue || num2 > int.MaxValue)
				{
					return num2;
				}
				return (int)num2;
			}
			case TypeCode.Int64:
			{
				long num2;
				try
				{
					num2 = ((!(obj is long)) ? (-conv.ToInt64(null)) : (-(long)obj));
				}
				catch (StackOverflowException ex4)
				{
					throw ex4;
				}
				catch (OutOfMemoryException ex5)
				{
					throw ex5;
				}
				catch (Exception)
				{
					num3 = decimal.Negate(conv.ToDecimal(null));
					goto IL_027f;
				}
				return num2;
			}
			case TypeCode.Decimal:
				try
				{
					num3 = ((!(obj is decimal)) ? decimal.Negate(conv.ToDecimal(null)) : decimal.Negate((decimal)obj));
					return num3;
				}
				catch (StackOverflowException ex)
				{
					throw ex;
				}
				catch (OutOfMemoryException ex2)
				{
					throw ex2;
				}
				catch (Exception)
				{
					num = 0.0 - conv.ToDouble(null);
				}
				goto IL_0275;
			case TypeCode.Single:
				if (obj is float)
				{
					return 0f - (float)obj;
				}
				return 0f - conv.ToSingle(null);
			case TypeCode.Double:
				num = ((!(obj is double)) ? (0.0 - conv.ToDouble(null)) : (0.0 - (double)obj));
				goto IL_0275;
			case TypeCode.String:
				num = ((!(obj is string value)) ? (0.0 - DoubleType.FromString(conv.ToString(null))) : (0.0 - DoubleType.FromString(value)));
				goto IL_0275;
			default:
				{
					throw GetNoValidOperatorException(obj);
				}
				IL_027f:
				return num3;
				IL_0263:
				return num5;
				IL_0275:
				return num;
			}
		}
	}

	public static object NotObj(object obj)
	{
		if (obj == null)
		{
			return -1;
		}
		IConvertible convertible = obj as IConvertible;
		switch (convertible?.GetTypeCode() ?? TypeCode.Object)
		{
		case TypeCode.Boolean:
			return !convertible.ToBoolean(null);
		case TypeCode.Byte:
		{
			Type type = obj.GetType();
			byte b = (byte)(~convertible.ToByte(null));
			if (type.IsEnum)
			{
				return Enum.ToObject(type, b);
			}
			return b;
		}
		case TypeCode.Int16:
		{
			Type type = obj.GetType();
			short num2 = (short)(~convertible.ToInt16(null));
			if (type.IsEnum)
			{
				return Enum.ToObject(type, num2);
			}
			return num2;
		}
		case TypeCode.Int32:
		{
			Type type = obj.GetType();
			int num3 = ~convertible.ToInt32(null);
			if (type.IsEnum)
			{
				return Enum.ToObject(type, num3);
			}
			return num3;
		}
		case TypeCode.Int64:
		{
			Type type = obj.GetType();
			long num = ~convertible.ToInt64(null);
			if (type.IsEnum)
			{
				return Enum.ToObject(type, num);
			}
			return num;
		}
		case TypeCode.Decimal:
			return ~Convert.ToInt64(convertible.ToDecimal(null));
		case TypeCode.Single:
			return ~Convert.ToInt64(convertible.ToDecimal(null));
		case TypeCode.Double:
			return ~Convert.ToInt64(convertible.ToDecimal(null));
		case TypeCode.String:
			return ~LongType.FromString(convertible.ToString(null));
		default:
			throw GetNoValidOperatorException(obj);
		}
	}

	public static object BitAndObj(object obj1, object obj2)
	{
		if (obj1 == null && obj2 == null)
		{
			return 0;
		}
		Type type = null;
		Type type2 = null;
		bool isEnum = default(bool);
		if (obj1 != null)
		{
			type = obj1.GetType();
			isEnum = type.IsEnum;
		}
		bool isEnum2 = default(bool);
		if (obj2 != null)
		{
			type2 = obj2.GetType();
			isEnum2 = type2.IsEnum;
		}
		switch (GetWidestType(obj1, obj2))
		{
		case TypeCode.Boolean:
			if ((object)type == type2)
			{
				return BooleanType.FromObject(obj1) & BooleanType.FromObject(obj2);
			}
			return (short)(ShortType.FromObject(obj1) & ShortType.FromObject(obj2));
		case TypeCode.Byte:
		{
			byte b = (byte)(ByteType.FromObject(obj1) & ByteType.FromObject(obj2));
			if ((isEnum && isEnum2 && (object)type != type2) || !isEnum || !isEnum2)
			{
				return b;
			}
			if (isEnum)
			{
				return Enum.ToObject(type, b);
			}
			if (isEnum2)
			{
				return Enum.ToObject(type2, b);
			}
			break;
		}
		case TypeCode.Int16:
		{
			short num2 = (short)(ShortType.FromObject(obj1) & ShortType.FromObject(obj2));
			if ((isEnum && isEnum2 && (object)type != type2) || !isEnum || !isEnum2)
			{
				return num2;
			}
			if (isEnum)
			{
				return Enum.ToObject(type, num2);
			}
			if (isEnum2)
			{
				return Enum.ToObject(type2, num2);
			}
			break;
		}
		case TypeCode.Int32:
		{
			int num3 = IntegerType.FromObject(obj1) & IntegerType.FromObject(obj2);
			if ((isEnum && isEnum2 && (object)type != type2) || !isEnum || !isEnum2)
			{
				return num3;
			}
			if (isEnum)
			{
				return Enum.ToObject(type, num3);
			}
			if (isEnum2)
			{
				return Enum.ToObject(type2, num3);
			}
			break;
		}
		case TypeCode.Int64:
		{
			long num = LongType.FromObject(obj1) & LongType.FromObject(obj2);
			if ((isEnum && isEnum2 && (object)type != type2) || !isEnum || !isEnum2)
			{
				return num;
			}
			if (isEnum)
			{
				return Enum.ToObject(type, num);
			}
			if (isEnum2)
			{
				return Enum.ToObject(type2, num);
			}
			break;
		}
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
		case TypeCode.String:
			return LongType.FromObject(obj1) & LongType.FromObject(obj2);
		}
		throw GetNoValidOperatorException(obj1, obj2);
	}

	public static object BitOrObj(object obj1, object obj2)
	{
		if (obj1 == null && obj2 == null)
		{
			return 0;
		}
		Type type = null;
		Type type2 = null;
		bool isEnum = default(bool);
		if (obj1 != null)
		{
			type = obj1.GetType();
			isEnum = type.IsEnum;
		}
		bool isEnum2 = default(bool);
		if (obj2 != null)
		{
			type2 = obj2.GetType();
			isEnum2 = type2.IsEnum;
		}
		switch (GetWidestType(obj1, obj2))
		{
		case TypeCode.Boolean:
			if ((object)type == type2)
			{
				return BooleanType.FromObject(obj1) | BooleanType.FromObject(obj2);
			}
			return (short)(ShortType.FromObject(obj1) | ShortType.FromObject(obj2));
		case TypeCode.Byte:
		{
			byte b = (byte)(ByteType.FromObject(obj1) | ByteType.FromObject(obj2));
			if ((isEnum && isEnum2 && (object)type != type2) || !isEnum || !isEnum2)
			{
				return b;
			}
			if (isEnum)
			{
				return Enum.ToObject(type, b);
			}
			if (isEnum2)
			{
				return Enum.ToObject(type2, b);
			}
			break;
		}
		case TypeCode.Int16:
		{
			short num2 = (short)(ShortType.FromObject(obj1) | ShortType.FromObject(obj2));
			if ((isEnum && isEnum2 && (object)type != type2) || !isEnum || !isEnum2)
			{
				return num2;
			}
			if (isEnum)
			{
				return Enum.ToObject(type, num2);
			}
			if (isEnum2)
			{
				return Enum.ToObject(type2, num2);
			}
			break;
		}
		case TypeCode.Int32:
		{
			int num3 = IntegerType.FromObject(obj1) | IntegerType.FromObject(obj2);
			if ((isEnum && isEnum2 && (object)type != type2) || !isEnum || !isEnum2)
			{
				return num3;
			}
			if (isEnum)
			{
				return Enum.ToObject(type, num3);
			}
			if (isEnum2)
			{
				return Enum.ToObject(type2, num3);
			}
			break;
		}
		case TypeCode.Int64:
		{
			long num = LongType.FromObject(obj1) | LongType.FromObject(obj2);
			if ((isEnum && isEnum2 && (object)type != type2) || !isEnum || !isEnum2)
			{
				return num;
			}
			if (isEnum)
			{
				return Enum.ToObject(type, num);
			}
			if (isEnum2)
			{
				return Enum.ToObject(type2, num);
			}
			break;
		}
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
		case TypeCode.String:
			return LongType.FromObject(obj1) | LongType.FromObject(obj2);
		}
		throw GetNoValidOperatorException(obj1, obj2);
	}

	public static object BitXorObj(object obj1, object obj2)
	{
		if (obj1 == null && obj2 == null)
		{
			return 0;
		}
		Type type = null;
		Type type2 = null;
		bool isEnum = default(bool);
		if (obj1 != null)
		{
			type = obj1.GetType();
			isEnum = type.IsEnum;
		}
		bool isEnum2 = default(bool);
		if (obj2 != null)
		{
			type2 = obj2.GetType();
			isEnum2 = type2.IsEnum;
		}
		switch (GetWidestType(obj1, obj2))
		{
		case TypeCode.Boolean:
			if ((object)type == type2)
			{
				return BooleanType.FromObject(obj1) ^ BooleanType.FromObject(obj2);
			}
			return (short)(ShortType.FromObject(obj1) ^ ShortType.FromObject(obj2));
		case TypeCode.Byte:
		{
			byte b = (byte)(ByteType.FromObject(obj1) ^ ByteType.FromObject(obj2));
			if ((isEnum && isEnum2 && (object)type != type2) || !isEnum || !isEnum2)
			{
				return b;
			}
			if (isEnum)
			{
				return Enum.ToObject(type, b);
			}
			if (isEnum2)
			{
				return Enum.ToObject(type2, b);
			}
			break;
		}
		case TypeCode.Int16:
		{
			short num2 = (short)(ShortType.FromObject(obj1) ^ ShortType.FromObject(obj2));
			if ((isEnum && isEnum2 && (object)type != type2) || !isEnum || !isEnum2)
			{
				return num2;
			}
			if (isEnum)
			{
				return Enum.ToObject(type, num2);
			}
			if (isEnum2)
			{
				return Enum.ToObject(type2, num2);
			}
			break;
		}
		case TypeCode.Int32:
		{
			int num3 = IntegerType.FromObject(obj1) ^ IntegerType.FromObject(obj2);
			if ((isEnum && isEnum2 && (object)type != type2) || !isEnum || !isEnum2)
			{
				return num3;
			}
			if (isEnum)
			{
				return Enum.ToObject(type, num3);
			}
			if (isEnum2)
			{
				return Enum.ToObject(type2, num3);
			}
			break;
		}
		case TypeCode.Int64:
		{
			long num = LongType.FromObject(obj1) ^ LongType.FromObject(obj2);
			if ((isEnum && isEnum2 && (object)type != type2) || !isEnum || !isEnum2)
			{
				return num;
			}
			if (isEnum)
			{
				return Enum.ToObject(type, num);
			}
			if (isEnum2)
			{
				return Enum.ToObject(type2, num);
			}
			break;
		}
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
		case TypeCode.String:
			return LongType.FromObject(obj1) ^ LongType.FromObject(obj2);
		}
		throw GetNoValidOperatorException(obj1, obj2);
	}

	public static object AddObj(object o1, object o2)
	{
		IConvertible convertible = o1 as IConvertible;
		TypeCode typeCode = (TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((o1 != null) ? 1 : 0));
		IConvertible convertible2 = o2 as IConvertible;
		TypeCode typeCode2 = (TypeCode)(((int?)convertible2?.GetTypeCode()) ?? ((o2 != null) ? 1 : 0));
		if (typeCode == TypeCode.Object && o1 is char[] && (typeCode2 == TypeCode.String || typeCode2 == TypeCode.Empty || (typeCode2 == TypeCode.Object && o2 is char[])))
		{
			o1 = new string(CharArrayType.FromObject(o1));
			convertible = (IConvertible)o1;
			typeCode = TypeCode.String;
		}
		if (typeCode2 == TypeCode.Object && o2 is char[] && (typeCode == TypeCode.String || typeCode == TypeCode.Empty))
		{
			o2 = new string(CharArrayType.FromObject(o2));
			convertible2 = (IConvertible)o2;
			typeCode2 = TypeCode.String;
		}
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 342:
		case 344:
			return o1;
		case 18:
		case 56:
			return o2;
		case 348:
		case 349:
		case 351:
		case 353:
		case 355:
		case 356:
		case 357:
			return AddString(convertible, typeCode, convertible2, typeCode2);
		case 132:
		case 151:
		case 189:
		case 227:
		case 265:
		case 284:
		case 303:
			return AddString(convertible, typeCode, convertible2, typeCode2);
		case 80:
		case 94:
		case 320:
		case 322:
		case 346:
		case 358:
		case 360:
			return StringType.FromObject(o1) + StringType.FromObject(o2);
		case 75:
		case 345:
			return AddString(convertible, typeCode, convertible2, typeCode2);
		case 0:
			return 0;
		case 57:
		case 114:
		case 133:
		case 171:
		case 209:
		case 247:
		case 266:
		case 285:
			return o1;
		case 3:
		case 6:
		case 7:
		case 9:
		case 11:
		case 13:
		case 14:
		case 15:
			return o2;
		case 129:
		case 148:
		case 186:
		case 224:
		case 291:
		case 292:
		case 294:
		case 296:
		case 300:
			return AddDecimal(convertible, convertible2);
		case 72:
			return AddDecimal(ToVBBoolConv(convertible), convertible2);
		case 288:
			return AddDecimal(convertible, ToVBBoolConv(convertible2));
		case 128:
		case 147:
		case 185:
		case 223:
		case 261:
		case 272:
		case 273:
		case 275:
		case 277:
		case 279:
		case 280:
		case 281:
		case 299:
			return AddDouble(convertible.ToDouble(null), convertible2.ToDouble(null));
		case 269:
			return AddDouble(convertible.ToDouble(null), ToVBBool(convertible2));
		case 71:
			return AddDouble(ToVBBool(convertible), convertible2.ToDouble(null));
		case 127:
		case 146:
		case 184:
		case 222:
		case 253:
		case 254:
		case 256:
		case 258:
		case 260:
		case 262:
		case 298:
			return AddSingle(convertible.ToSingle(null), convertible2.ToSingle(null));
		case 250:
			return AddSingle(convertible.ToSingle(null), ToVBBool(convertible2));
		case 70:
			return AddSingle(ToVBBool(convertible), convertible2.ToSingle(null));
		case 125:
		case 144:
		case 182:
		case 215:
		case 216:
		case 218:
		case 220:
			return AddInt64(convertible.ToInt64(null), convertible2.ToInt64(null));
		case 212:
			return AddInt64(convertible.ToInt64(null), ToVBBool(convertible2));
		case 68:
			return AddInt64(ToVBBool(convertible), convertible2.ToInt64(null));
		case 123:
		case 142:
		case 177:
		case 178:
		case 180:
			return AddInt32(convertible.ToInt32(null), convertible2.ToInt32(null));
		case 174:
			return AddInt32(convertible.ToInt32(null), ToVBBool(convertible2));
		case 66:
			return AddInt32(ToVBBool(convertible), convertible2.ToInt32(null));
		case 121:
		case 139:
		case 140:
			return AddInt16(convertible.ToInt16(null), convertible2.ToInt16(null));
		case 63:
		case 64:
			return AddInt16(checked((short)ToVBBool(convertible)), convertible2.ToInt16(null));
		case 117:
		case 136:
			return AddInt16(convertible.ToInt16(null), checked((short)ToVBBool(convertible2)));
		case 60:
			return checked(AddInt16((short)ToVBBool(convertible), (short)ToVBBool(convertible2)));
		case 120:
			return AddByte(convertible.ToByte(null), convertible2.ToByte(null));
		default:
			throw GetNoValidOperatorException(o1, o2);
		}
	}

	private static object AddString(IConvertible conv1, TypeCode tc1, IConvertible conv2, TypeCode tc2)
	{
		return tc1 switch
		{
			TypeCode.String => DoubleType.FromString(conv1.ToString(null)), 
			TypeCode.Boolean => ToVBBool(conv1), 
			_ => conv1.ToDouble(null), 
		} + tc2 switch
		{
			TypeCode.String => DoubleType.FromString(conv2.ToString(null)), 
			TypeCode.Boolean => ToVBBool(conv2), 
			_ => conv2.ToDouble(null), 
		};
	}

	private static object AddByte(byte i1, byte i2)
	{
		checked
		{
			short num = (short)unchecked(i1 + i2);
			if (num >= 0 && num <= 255)
			{
				return (byte)num;
			}
			return num;
		}
	}

	private static object AddInt16(short i1, short i2)
	{
		checked
		{
			int num = i1 + i2;
			if (num >= -32768 && num <= 32767)
			{
				return (short)num;
			}
			return num;
		}
	}

	private static object AddInt32(int i1, int i2)
	{
		checked
		{
			long num = unchecked((long)i1) + unchecked((long)i2);
			if (num >= int.MinValue && num <= int.MaxValue)
			{
				return (int)num;
			}
			return num;
		}
	}

	private static object AddInt64(long i1, long i2)
	{
		try
		{
			return checked(i1 + i2);
		}
		catch (OverflowException)
		{
			return decimal.Add(new decimal(i1), new decimal(i2));
		}
	}

	private static object AddSingle(float f1, float f2)
	{
		double num = (double)f1 + (double)f2;
		if (num <= 3.4028234663852886E+38 && num >= -3.4028234663852886E+38)
		{
			return (float)num;
		}
		if (double.IsInfinity(num) && (float.IsInfinity(f1) || float.IsInfinity(f2)))
		{
			return (float)num;
		}
		return num;
	}

	private static object AddDouble(double d1, double d2)
	{
		return d1 + d2;
	}

	private static object AddDecimal(IConvertible conv1, IConvertible conv2)
	{
		decimal num = default(decimal);
		if (conv1 != null)
		{
			num = conv1.ToDecimal(null);
		}
		decimal num2 = conv2.ToDecimal(null);
		try
		{
			return decimal.Add(num, num2);
		}
		catch (OverflowException)
		{
			return Convert.ToDouble(num) + Convert.ToDouble(num2);
		}
	}

	private static int ToVBBool(IConvertible conv)
	{
		if (conv.ToBoolean(null))
		{
			return -1;
		}
		return 0;
	}

	private static IConvertible ToVBBoolConv(IConvertible conv)
	{
		if (conv.ToBoolean(null))
		{
			return -1;
		}
		return 0;
	}

	public static object SubObj(object o1, object o2)
	{
		IConvertible convertible = o1 as IConvertible;
		TypeCode typeCode = (TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((o1 != null) ? 1 : 0));
		IConvertible convertible2 = o2 as IConvertible;
		TypeCode typeCode2 = (TypeCode)(((int?)convertible2?.GetTypeCode()) ?? ((o2 != null) ? 1 : 0));
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 0:
			return 0;
		case 18:
			return SubStringString(null, convertible2.ToString(null));
		case 342:
			return SubStringString(convertible.ToString(null), null);
		case 57:
		case 114:
		case 133:
		case 171:
		case 209:
		case 247:
		case 266:
		case 285:
			return o1;
		case 3:
		case 6:
		case 7:
		case 9:
		case 11:
		case 13:
		case 14:
		case 15:
			return InternalNegObj(o2, convertible2, typeCode2);
		case 129:
		case 148:
		case 186:
		case 224:
		case 291:
		case 292:
		case 294:
		case 296:
		case 300:
			return SubDecimal(convertible, convertible2);
		case 72:
			return SubDecimal(ToVBBoolConv(convertible), convertible2);
		case 288:
			return SubDecimal(convertible, ToVBBoolConv(convertible2));
		case 75:
		case 132:
		case 151:
		case 189:
		case 227:
		case 265:
		case 284:
		case 303:
		case 345:
		case 348:
		case 349:
		case 351:
		case 353:
		case 355:
		case 356:
		case 357:
			return SubString(convertible, typeCode, convertible2, typeCode2);
		case 360:
			return SubStringString(convertible.ToString(null), convertible2.ToString(null));
		case 128:
		case 147:
		case 185:
		case 223:
		case 261:
		case 272:
		case 273:
		case 275:
		case 277:
		case 279:
		case 280:
		case 281:
		case 299:
			return SubDouble(convertible.ToDouble(null), convertible2.ToDouble(null));
		case 269:
			return SubDouble(convertible.ToDouble(null), ToVBBool(convertible2));
		case 71:
			return SubDouble(ToVBBool(convertible), convertible2.ToDouble(null));
		case 127:
		case 146:
		case 184:
		case 222:
		case 253:
		case 254:
		case 256:
		case 258:
		case 260:
		case 262:
		case 298:
			return SubSingle(convertible.ToSingle(null), convertible2.ToSingle(null));
		case 250:
			return SubSingle(convertible.ToSingle(null), ToVBBool(convertible2));
		case 70:
			return SubSingle(ToVBBool(convertible), convertible2.ToSingle(null));
		case 125:
		case 144:
		case 182:
		case 215:
		case 216:
		case 218:
		case 220:
			return SubInt64(convertible.ToInt64(null), convertible2.ToInt64(null));
		case 212:
			return SubInt64(convertible.ToInt64(null), ToVBBool(convertible2));
		case 68:
			return SubInt64(ToVBBool(convertible), convertible2.ToInt64(null));
		case 123:
		case 142:
		case 177:
		case 178:
		case 180:
			return SubInt32(convertible.ToInt32(null), convertible2.ToInt32(null));
		case 174:
			return SubInt32(convertible.ToInt32(null), ToVBBool(convertible2));
		case 66:
			return SubInt32(ToVBBool(convertible), convertible2.ToInt32(null));
		case 121:
		case 139:
		case 140:
			return SubInt16(convertible.ToInt16(null), convertible2.ToInt16(null));
		case 63:
		case 64:
			return SubInt16(checked((short)ToVBBool(convertible)), convertible2.ToInt16(null));
		case 117:
		case 136:
			return SubInt16(convertible.ToInt16(null), checked((short)ToVBBool(convertible2)));
		case 60:
			return checked(SubInt16((short)ToVBBool(convertible), (short)ToVBBool(convertible2)));
		case 120:
			return SubByte(convertible.ToByte(null), convertible2.ToByte(null));
		default:
			throw GetNoValidOperatorException(o1, o2);
		}
	}

	private static object SubString(IConvertible conv1, TypeCode tc1, IConvertible conv2, TypeCode tc2)
	{
		return tc1 switch
		{
			TypeCode.String => DoubleType.FromString(conv1.ToString(null)), 
			TypeCode.Boolean => ToVBBool(conv1), 
			_ => conv1.ToDouble(null), 
		} - tc2 switch
		{
			TypeCode.String => DoubleType.FromString(conv2.ToString(null)), 
			TypeCode.Boolean => ToVBBool(conv2), 
			_ => conv2.ToDouble(null), 
		};
	}

	private static object SubStringString(string s1, string s2)
	{
		double num = default(double);
		if (s1 != null)
		{
			num = DoubleType.FromString(s1);
		}
		double num2 = default(double);
		if (s2 != null)
		{
			num2 = DoubleType.FromString(s2);
		}
		return num - num2;
	}

	private static object SubByte(byte i1, byte i2)
	{
		checked
		{
			short num = (short)unchecked(i1 - i2);
			if (num >= 0 && num <= 255)
			{
				return (byte)num;
			}
			return num;
		}
	}

	private static object SubInt16(short i1, short i2)
	{
		checked
		{
			int num = i1 - i2;
			if (num >= -32768 && num <= 32767)
			{
				return (short)num;
			}
			return num;
		}
	}

	private static object SubInt32(int i1, int i2)
	{
		checked
		{
			long num = unchecked((long)i1) - unchecked((long)i2);
			if (num >= int.MinValue && num <= int.MaxValue)
			{
				return (int)num;
			}
			return num;
		}
	}

	private static object SubInt64(long i1, long i2)
	{
		try
		{
			return checked(i1 - i2);
		}
		catch (StackOverflowException ex)
		{
			throw ex;
		}
		catch (OutOfMemoryException ex2)
		{
			throw ex2;
		}
		catch (Exception)
		{
			return decimal.Subtract(new decimal(i1), new decimal(i2));
		}
	}

	private static object SubSingle(float f1, float f2)
	{
		double num = (double)f1 - (double)f2;
		if (num <= 3.4028234663852886E+38 && num >= -3.4028234663852886E+38)
		{
			return (float)num;
		}
		if (double.IsInfinity(num) && (float.IsInfinity(f1) || float.IsInfinity(f2)))
		{
			return (float)num;
		}
		return num;
	}

	private static object SubDouble(double d1, double d2)
	{
		return d1 - d2;
	}

	private static object SubDecimal(IConvertible conv1, IConvertible conv2)
	{
		decimal num = conv1.ToDecimal(null);
		decimal num2 = conv2.ToDecimal(null);
		try
		{
			return decimal.Subtract(num, num2);
		}
		catch (OverflowException)
		{
			return Convert.ToDouble(num) - Convert.ToDouble(num2);
		}
	}

	public static object MulObj(object o1, object o2)
	{
		IConvertible convertible = o1 as IConvertible;
		TypeCode typeCode = (TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((o1 != null) ? 1 : 0));
		IConvertible convertible2 = o2 as IConvertible;
		TypeCode typeCode2 = (TypeCode)(((int?)convertible2?.GetTypeCode()) ?? ((o2 != null) ? 1 : 0));
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 18:
		case 342:
			return 0.0;
		case 6:
		case 114:
			return (byte)0;
		case 3:
		case 7:
		case 57:
		case 133:
			return (short)0;
		case 0:
		case 9:
		case 171:
			return 0;
		case 11:
		case 209:
			return 0L;
		case 13:
		case 247:
			return 0f;
		case 14:
		case 266:
			return 0.0;
		case 15:
		case 285:
			return 0m;
		case 129:
		case 148:
		case 186:
		case 224:
		case 291:
		case 292:
		case 294:
		case 296:
		case 300:
			return MulDecimal(convertible, convertible2);
		case 72:
			return MulDecimal(ToVBBoolConv(convertible), convertible2);
		case 288:
			return MulDecimal(convertible, ToVBBoolConv(convertible2));
		case 75:
		case 132:
		case 151:
		case 189:
		case 227:
		case 265:
		case 284:
		case 303:
		case 345:
		case 348:
		case 349:
		case 351:
		case 353:
		case 355:
		case 356:
		case 357:
			return MulString(convertible, typeCode, convertible2, typeCode2);
		case 360:
			return MulStringString(convertible.ToString(null), convertible2.ToString(null));
		case 128:
		case 147:
		case 185:
		case 223:
		case 261:
		case 272:
		case 273:
		case 275:
		case 277:
		case 279:
		case 280:
		case 281:
		case 299:
			return MulDouble(convertible.ToDouble(null), convertible2.ToDouble(null));
		case 269:
			return MulDouble(convertible.ToDouble(null), ToVBBool(convertible2));
		case 71:
			return MulDouble(ToVBBool(convertible), convertible2.ToDouble(null));
		case 127:
		case 146:
		case 184:
		case 222:
		case 253:
		case 254:
		case 256:
		case 258:
		case 260:
		case 262:
		case 298:
			return MulSingle(convertible.ToSingle(null), convertible2.ToSingle(null));
		case 250:
			return MulSingle(convertible.ToSingle(null), ToVBBool(convertible2));
		case 70:
			return MulSingle(ToVBBool(convertible), convertible2.ToSingle(null));
		case 125:
		case 144:
		case 182:
		case 215:
		case 216:
		case 218:
		case 220:
			return MulInt64(convertible.ToInt64(null), convertible2.ToInt64(null));
		case 212:
			return MulInt64(convertible.ToInt64(null), ToVBBool(convertible2));
		case 68:
			return MulInt64(ToVBBool(convertible), convertible2.ToInt64(null));
		case 123:
		case 142:
		case 177:
		case 178:
		case 180:
			return MulInt32(convertible.ToInt32(null), convertible2.ToInt32(null));
		case 174:
			return MulInt32(convertible.ToInt32(null), ToVBBool(convertible2));
		case 66:
			return MulInt32(ToVBBool(convertible), convertible2.ToInt32(null));
		case 121:
		case 139:
		case 140:
			return MulInt16(convertible.ToInt16(null), convertible2.ToInt16(null));
		case 63:
		case 64:
			return MulInt16(checked((short)ToVBBool(convertible)), convertible2.ToInt16(null));
		case 117:
		case 136:
			return MulInt16(convertible.ToInt16(null), checked((short)ToVBBool(convertible2)));
		case 60:
			return checked(MulInt16((short)ToVBBool(convertible), (short)ToVBBool(convertible2)));
		case 120:
			return MulByte(convertible.ToByte(null), convertible2.ToByte(null));
		default:
			throw GetNoValidOperatorException(o1, o2);
		}
	}

	private static object MulString(IConvertible conv1, TypeCode tc1, IConvertible conv2, TypeCode tc2)
	{
		return tc1 switch
		{
			TypeCode.String => DoubleType.FromString(conv1.ToString(null)), 
			TypeCode.Boolean => ToVBBool(conv1), 
			_ => conv1.ToDouble(null), 
		} * tc2 switch
		{
			TypeCode.String => DoubleType.FromString(conv2.ToString(null)), 
			TypeCode.Boolean => ToVBBool(conv2), 
			_ => conv2.ToDouble(null), 
		};
	}

	private static object MulStringString(string s1, string s2)
	{
		double num = default(double);
		if (s1 != null)
		{
			num = DoubleType.FromString(s1);
		}
		double num2 = default(double);
		if (s2 != null)
		{
			num2 = DoubleType.FromString(s2);
		}
		return num * num2;
	}

	private static object MulByte(byte i1, byte i2)
	{
		checked
		{
			int num = i1 * i2;
			if (num >= 0 && num <= 255)
			{
				return (byte)num;
			}
			if (num >= -32768 && num <= 32767)
			{
				return (short)num;
			}
			return num;
		}
	}

	private static object MulInt16(short i1, short i2)
	{
		checked
		{
			int num = i1 * i2;
			if (num >= -32768 && num <= 32767)
			{
				return (short)num;
			}
			return num;
		}
	}

	private static object MulInt32(int i1, int i2)
	{
		checked
		{
			long num = unchecked((long)i1) * unchecked((long)i2);
			if (num >= int.MinValue && num <= int.MaxValue)
			{
				return (int)num;
			}
			return num;
		}
	}

	private static object MulInt64(long i1, long i2)
	{
		try
		{
			return checked(i1 * i2);
		}
		catch (OverflowException)
		{
			try
			{
				return decimal.Multiply(new decimal(i1), new decimal(i2));
			}
			catch (OverflowException)
			{
				return (double)i1 * (double)i2;
			}
		}
	}

	private static object MulSingle(float f1, float f2)
	{
		double num = (double)f1 * (double)f2;
		if (num <= 3.4028234663852886E+38 && num >= -3.4028234663852886E+38)
		{
			return (float)num;
		}
		if (double.IsInfinity(num) && (float.IsInfinity(f1) || float.IsInfinity(f2)))
		{
			return (float)num;
		}
		return num;
	}

	private static object MulDouble(double d1, double d2)
	{
		return d1 * d2;
	}

	private static object MulDecimal(IConvertible conv1, IConvertible conv2)
	{
		decimal num = conv1.ToDecimal(null);
		decimal num2 = conv2.ToDecimal(null);
		try
		{
			return decimal.Multiply(num, num2);
		}
		catch (OverflowException)
		{
			return Convert.ToDouble(num) * Convert.ToDouble(num2);
		}
	}

	public static object DivObj(object o1, object o2)
	{
		IConvertible convertible = o1 as IConvertible;
		TypeCode typeCode = (TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((o1 != null) ? 1 : 0));
		IConvertible convertible2 = o2 as IConvertible;
		TypeCode typeCode2 = (TypeCode)(((int?)convertible2?.GetTypeCode()) ?? ((o2 != null) ? 1 : 0));
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 18:
			return DivString(convertible, typeCode, convertible2, typeCode2);
		case 342:
			return DivString(convertible, typeCode, convertible2, typeCode2);
		case 345:
			return DivString(convertible, typeCode, convertible2, typeCode2);
		case 75:
			return DivString(convertible, typeCode, convertible2, typeCode2);
		case 348:
		case 349:
		case 351:
		case 353:
		case 355:
		case 356:
		case 357:
			return DivString(convertible, typeCode, convertible2, typeCode2);
		case 132:
		case 151:
		case 189:
		case 227:
		case 265:
		case 284:
		case 303:
			return DivString(convertible, typeCode, convertible2, typeCode2);
		case 360:
			return DivStringString(convertible.ToString(null), convertible2.ToString(null));
		case 0:
			return DivDouble(0.0, 0.0);
		case 57:
			return DivDouble(ToVBBool(convertible), 0.0);
		case 114:
		case 133:
		case 171:
		case 209:
		case 247:
		case 266:
		case 285:
			return DivDouble(convertible.ToDouble(null), 0.0);
		case 3:
		case 6:
		case 7:
		case 9:
		case 11:
		case 13:
		case 14:
		case 15:
			return DivDouble(0.0, convertible2.ToDouble(null));
		case 63:
		case 64:
		case 66:
		case 68:
		case 71:
			return DivDouble(ToVBBool(convertible), convertible2.ToDouble(null));
		case 72:
			return DivDecimal(ToVBBoolConv(convertible), convertible2.ToDecimal(null));
		case 288:
			return DivDecimal(convertible, ToVBBoolConv(convertible2));
		case 60:
			return DivDouble(ToVBBool(convertible), ToVBBool(convertible2));
		case 70:
			return DivSingle(ToVBBool(convertible), convertible2.ToSingle(null));
		case 250:
			return DivSingle(convertible.ToSingle(null), ToVBBool(convertible2));
		case 117:
		case 136:
		case 174:
		case 212:
		case 269:
			return DivDouble(convertible.ToDouble(null), ToVBBool(convertible2));
		case 129:
		case 148:
		case 186:
		case 224:
		case 291:
		case 292:
		case 294:
		case 296:
		case 300:
			return DivDecimal(convertible, convertible2);
		case 127:
		case 146:
		case 184:
		case 222:
		case 253:
		case 254:
		case 256:
		case 258:
		case 260:
		case 262:
		case 298:
			return DivSingle(convertible.ToSingle(null), convertible2.ToSingle(null));
		case 120:
		case 121:
		case 123:
		case 125:
		case 128:
		case 139:
		case 140:
		case 142:
		case 144:
		case 147:
		case 177:
		case 178:
		case 180:
		case 182:
		case 185:
		case 215:
		case 216:
		case 218:
		case 220:
		case 223:
		case 261:
		case 272:
		case 273:
		case 275:
		case 277:
		case 279:
		case 280:
		case 281:
		case 299:
			return DivDouble(convertible.ToDouble(null), convertible2.ToDouble(null));
		default:
			throw GetNoValidOperatorException(o1, o2);
		}
	}

	private static object DivString(IConvertible conv1, TypeCode tc1, IConvertible conv2, TypeCode tc2)
	{
		return tc1 switch
		{
			TypeCode.String => DoubleType.FromString(conv1.ToString(null)), 
			TypeCode.Boolean => ToVBBool(conv1), 
			_ => conv1.ToDouble(null), 
		} / tc2 switch
		{
			TypeCode.String => DoubleType.FromString(conv2.ToString(null)), 
			TypeCode.Boolean => ToVBBool(conv2), 
			_ => conv2.ToDouble(null), 
		};
	}

	private static object DivStringString(string s1, string s2)
	{
		double num = default(double);
		if (s1 != null)
		{
			num = DoubleType.FromString(s1);
		}
		double num2 = default(double);
		if (s2 != null)
		{
			num2 = DoubleType.FromString(s2);
		}
		return num / num2;
	}

	private static object DivDouble(double d1, double d2)
	{
		return d1 / d2;
	}

	private static object DivSingle(float sng1, float sng2)
	{
		float num = sng1 / sng2;
		if (float.IsInfinity(num))
		{
			if (float.IsInfinity(sng1) || float.IsInfinity(sng2))
			{
				return num;
			}
			return (double)sng1 / (double)sng2;
		}
		return num;
	}

	private static object DivDecimal(IConvertible conv1, IConvertible conv2)
	{
		decimal num = default(decimal);
		if (conv1 != null)
		{
			num = conv1.ToDecimal(null);
		}
		decimal num2 = default(decimal);
		if (conv2 != null)
		{
			num2 = conv2.ToDecimal(null);
		}
		try
		{
			return decimal.Divide(num, num2);
		}
		catch (OverflowException)
		{
			return Convert.ToSingle(num) / Convert.ToSingle(num2);
		}
	}

	public static object PowObj(object obj1, object obj2)
	{
		if (obj1 == null && obj2 == null)
		{
			return 1.0;
		}
		switch (GetWidestType(obj1, obj2))
		{
		case TypeCode.Boolean:
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.Int32:
		case TypeCode.Int64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
		case TypeCode.String:
			return Math.Pow(DoubleType.FromObject(obj1), DoubleType.FromObject(obj2));
		default:
			throw GetNoValidOperatorException(obj1, obj2);
		}
	}

	public static object ModObj(object o1, object o2)
	{
		IConvertible convertible = o1 as IConvertible;
		IConvertible convertible2 = o2 as IConvertible;
		TypeCode typeCode = (TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((o1 != null) ? 1 : 0));
		TypeCode typeCode2;
		if (convertible2 != null)
		{
			typeCode2 = convertible2.GetTypeCode();
		}
		else
		{
			convertible2 = null;
			typeCode2 = ((o2 != null) ? TypeCode.Object : TypeCode.Empty);
		}
		checked
		{
			switch (unchecked((int)checked(unchecked((int)typeCode) * 19 + typeCode2)))
			{
			case 18:
				return ModString(convertible, typeCode, convertible2, typeCode2);
			case 342:
				return ModString(convertible, typeCode, convertible2, typeCode2);
			case 348:
			case 349:
			case 351:
			case 353:
			case 355:
			case 356:
			case 357:
				return ModString(convertible, typeCode, convertible2, typeCode2);
			case 132:
			case 151:
			case 189:
			case 227:
			case 265:
			case 284:
			case 303:
				return ModString(convertible, typeCode, convertible2, typeCode2);
			case 360:
				return ModStringString(convertible.ToString(null), convertible2.ToString(null));
			case 75:
				return ModString(convertible, typeCode, convertible2, typeCode2);
			case 345:
				return ModString(convertible, typeCode, convertible2, typeCode2);
			case 0:
				return ModInt32(0, 0);
			case 114:
				return ModByte(convertible.ToByte(null), 0);
			case 57:
				return ModInt16((short)ToVBBool(convertible), 0);
			case 133:
				return ModInt16(convertible.ToInt16(null), 0);
			case 171:
				return ModInt32(convertible.ToInt32(null), 0);
			case 209:
				return ModInt64(convertible.ToInt64(null), 0L);
			case 247:
				return ModSingle(convertible.ToSingle(null), 0f);
			case 266:
				return ModDouble(convertible.ToDouble(null), 0.0);
			case 285:
				return ModDecimal(convertible, null);
			case 3:
				return ModInt16(0, (short)ToVBBool(convertible2));
			case 6:
				return ModByte(0, convertible2.ToByte(null));
			case 7:
				return ModInt16(0, (short)ToVBBool(convertible2));
			case 9:
				return ModInt32(0, convertible2.ToInt32(null));
			case 11:
				return ModInt64(0L, convertible2.ToInt64(null));
			case 13:
				return ModSingle(0f, convertible2.ToSingle(null));
			case 14:
				return ModDouble(0.0, convertible2.ToDouble(null));
			case 15:
				return ModDecimal(null, convertible2);
			case 129:
			case 148:
			case 186:
			case 224:
			case 291:
			case 292:
			case 294:
			case 296:
			case 300:
				return ModDecimal(convertible, convertible2);
			case 72:
				return ModDecimal(ToVBBoolConv(convertible), convertible2);
			case 288:
				return ModDecimal(convertible, ToVBBoolConv(convertible2));
			case 128:
			case 147:
			case 185:
			case 223:
			case 261:
			case 272:
			case 273:
			case 275:
			case 277:
			case 279:
			case 280:
			case 281:
			case 299:
				return ModDouble(convertible.ToDouble(null), convertible2.ToDouble(null));
			case 269:
				return ModDouble(convertible.ToDouble(null), ToVBBool(convertible2));
			case 71:
				return ModDouble(ToVBBool(convertible), convertible2.ToDouble(null));
			case 127:
			case 146:
			case 184:
			case 222:
			case 253:
			case 254:
			case 256:
			case 258:
			case 260:
			case 262:
			case 298:
				return ModSingle(convertible.ToSingle(null), convertible2.ToSingle(null));
			case 250:
				return ModSingle(convertible.ToSingle(null), ToVBBool(convertible2));
			case 70:
				return ModSingle(ToVBBool(convertible), convertible2.ToSingle(null));
			case 125:
			case 144:
			case 182:
			case 215:
			case 216:
			case 218:
			case 220:
				return ModInt64(convertible.ToInt64(null), convertible2.ToInt64(null));
			case 212:
				return ModInt64(convertible.ToInt64(null), ToVBBool(convertible2));
			case 68:
				return ModInt64(ToVBBool(convertible), convertible2.ToInt64(null));
			case 123:
			case 142:
			case 177:
			case 178:
			case 180:
				return ModInt32(convertible.ToInt32(null), convertible2.ToInt32(null));
			case 174:
				return ModInt32(convertible.ToInt32(null), ToVBBool(convertible2));
			case 66:
				return ModInt32(ToVBBool(convertible), convertible2.ToInt32(null));
			case 121:
			case 139:
			case 140:
				return ModInt16(convertible.ToInt16(null), convertible2.ToInt16(null));
			case 63:
			case 64:
				return ModInt16((short)ToVBBool(convertible), convertible2.ToInt16(null));
			case 117:
			case 136:
				return ModInt16(convertible.ToInt16(null), (short)ToVBBool(convertible2));
			case 60:
				return ModInt16((short)ToVBBool(convertible), (short)ToVBBool(convertible2));
			case 120:
				return ModByte(convertible.ToByte(null), convertible2.ToByte(null));
			default:
				throw GetNoValidOperatorException(o1, o2);
			}
		}
	}

	private static object ModString(IConvertible conv1, TypeCode tc1, IConvertible conv2, TypeCode tc2)
	{
		return tc1 switch
		{
			TypeCode.String => DoubleType.FromString(conv1.ToString(null)), 
			TypeCode.Boolean => ToVBBool(conv1), 
			_ => conv1.ToDouble(null), 
		} % tc2 switch
		{
			TypeCode.String => DoubleType.FromString(conv2.ToString(null)), 
			TypeCode.Boolean => ToVBBool(conv2), 
			_ => conv2.ToDouble(null), 
		};
	}

	private static object ModStringString(string s1, string s2)
	{
		double num = default(double);
		if (s1 != null)
		{
			num = DoubleType.FromString(s1);
		}
		double num2 = default(double);
		if (s2 != null)
		{
			num2 = DoubleType.FromString(s2);
		}
		return num % num2;
	}

	private static object ModByte(byte i1, byte i2)
	{
		checked
		{
			return (byte)unchecked((uint)i1 % (uint)i2);
		}
	}

	private static object ModInt16(short i1, short i2)
	{
		int num = i1 % i2;
		if (num < -32768 || num > 32767)
		{
			return num;
		}
		return checked((short)num);
	}

	private static object ModInt32(int i1, int i2)
	{
		long num = (long)i1 % (long)i2;
		if (num < int.MinValue || num > int.MaxValue)
		{
			return num;
		}
		return checked((int)num);
	}

	private static object ModInt64(long i1, long i2)
	{
		try
		{
			return i1 % i2;
		}
		catch (OverflowException)
		{
			decimal num = decimal.Remainder(new decimal(i1), new decimal(i2));
			if (decimal.Compare(num, -9223372036854775808m) < 0 || decimal.Compare(num, 9223372036854775807m) > 0)
			{
				return num;
			}
			return Convert.ToInt64(num);
		}
	}

	private static object ModSingle(float sng1, float sng2)
	{
		return sng1 % sng2;
	}

	private static object ModDouble(double d1, double d2)
	{
		return d1 % d2;
	}

	private static object ModDecimal(IConvertible conv1, IConvertible conv2)
	{
		decimal d = default(decimal);
		if (conv1 != null)
		{
			d = conv1.ToDecimal(null);
		}
		decimal d2 = default(decimal);
		if (conv2 != null)
		{
			d2 = conv2.ToDecimal(null);
		}
		return decimal.Remainder(d, d2);
	}

	public static object IDivObj(object o1, object o2)
	{
		IConvertible convertible = o1 as IConvertible;
		TypeCode typeCode = (TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((o1 != null) ? 1 : 0));
		IConvertible convertible2 = o2 as IConvertible;
		TypeCode typeCode2 = (TypeCode)(((int?)convertible2?.GetTypeCode()) ?? ((o2 != null) ? 1 : 0));
		checked
		{
			switch (unchecked((int)checked(unchecked((int)typeCode) * 19 + typeCode2)))
			{
			case 18:
				return IDivideInt64(0L, LongType.FromString(convertible2.ToString(null)));
			case 342:
				return IDivideInt64(LongType.FromString(convertible.ToString(null)), 0L);
			case 132:
			case 151:
			case 189:
			case 227:
			case 265:
			case 284:
			case 303:
				return IDivideString(convertible, typeCode, convertible2, typeCode2);
			case 360:
				return IDivideStringString(convertible.ToString(null), convertible2.ToString(null));
			case 345:
				return IDivideInt64(LongType.FromString(convertible.ToString(null)), ToVBBool(convertible2));
			case 348:
			case 349:
			case 351:
			case 353:
			case 355:
			case 356:
			case 357:
				return IDivideInt64(LongType.FromString(convertible.ToString(null)), convertible2.ToInt64(null));
			case 0:
				return IDivideInt32(0, 0);
			case 57:
				return IDivideInt16((short)ToVBBool(convertible), 0);
			case 114:
				return IDivideByte(convertible.ToByte(null), 0);
			case 133:
				return IDivideInt16(convertible.ToInt16(null), 0);
			case 171:
				return IDivideInt32(convertible.ToInt32(null), 0);
			case 209:
			case 247:
			case 266:
			case 285:
				return IDivideInt64(convertible.ToInt64(null), 0L);
			case 3:
				return IDivideInt64(0L, ToVBBool(convertible2));
			case 6:
				return IDivideByte(0, convertible2.ToByte(null));
			case 7:
				return IDivideInt16(0, convertible2.ToInt16(null));
			case 9:
				return IDivideInt32(0, convertible2.ToInt32(null));
			case 11:
			case 13:
			case 14:
			case 15:
				return IDivideInt64(0L, convertible2.ToInt64(null));
			case 63:
			case 64:
				return IDivideInt16((short)ToVBBool(convertible), convertible2.ToInt16(null));
			case 66:
				return IDivideInt32(ToVBBool(convertible), convertible2.ToInt32(null));
			case 68:
			case 70:
			case 71:
			case 72:
				return IDivideInt64(ToVBBool(convertible), convertible2.ToInt64(null));
			case 60:
				return IDivideInt16((short)ToVBBool(convertible), (short)ToVBBool(convertible2));
			case 75:
				return IDivideInt64(ToVBBool(convertible), LongType.FromString(convertible2.ToString(null)));
			case 117:
			case 136:
				return IDivideInt16(convertible.ToInt16(null), (short)ToVBBool(convertible2));
			case 174:
				return IDivideInt32(convertible.ToInt32(null), ToVBBool(convertible2));
			case 212:
			case 250:
			case 269:
			case 288:
				return IDivideInt64(convertible.ToInt64(null), ToVBBool(convertible2));
			case 120:
				return IDivideByte(convertible.ToByte(null), convertible2.ToByte(null));
			case 121:
			case 139:
			case 140:
				return IDivideInt16(convertible.ToInt16(null), convertible2.ToInt16(null));
			case 123:
			case 142:
			case 177:
			case 178:
			case 180:
				return IDivideInt32(convertible.ToInt32(null), convertible2.ToInt32(null));
			case 125:
			case 127:
			case 128:
			case 129:
			case 144:
			case 146:
			case 147:
			case 148:
			case 182:
			case 184:
			case 185:
			case 186:
			case 215:
			case 216:
			case 218:
			case 220:
			case 222:
			case 223:
			case 224:
			case 253:
			case 254:
			case 256:
			case 258:
			case 260:
			case 261:
			case 262:
			case 272:
			case 273:
			case 275:
			case 277:
			case 279:
			case 280:
			case 281:
			case 291:
			case 292:
			case 294:
			case 296:
			case 298:
			case 299:
			case 300:
				return IDivideInt64(convertible.ToInt64(null), convertible2.ToInt64(null));
			default:
				throw GetNoValidOperatorException(o1, o2);
			}
		}
	}

	private static object IDivideString(IConvertible conv1, TypeCode tc1, IConvertible conv2, TypeCode tc2)
	{
		long num;
		switch (tc1)
		{
		case TypeCode.String:
			try
			{
				num = LongType.FromString(conv1.ToString(null));
			}
			catch (StackOverflowException ex)
			{
				throw ex;
			}
			catch (OutOfMemoryException ex2)
			{
				throw ex2;
			}
			catch (Exception)
			{
				throw GetNoValidOperatorException(conv1, conv2);
			}
			break;
		case TypeCode.Boolean:
			num = ToVBBool(conv1);
			break;
		default:
			num = conv1.ToInt64(null);
			break;
		}
		long num2;
		switch (tc2)
		{
		case TypeCode.String:
			try
			{
				num2 = LongType.FromString(conv2.ToString(null));
			}
			catch (StackOverflowException ex4)
			{
				throw ex4;
			}
			catch (OutOfMemoryException ex5)
			{
				throw ex5;
			}
			catch (Exception)
			{
				throw GetNoValidOperatorException(conv1, conv2);
			}
			break;
		case TypeCode.Boolean:
			num2 = ToVBBool(conv2);
			break;
		default:
			num2 = conv2.ToInt64(null);
			break;
		}
		return num / num2;
	}

	private static object IDivideStringString(string s1, string s2)
	{
		long num = default(long);
		if (s1 != null)
		{
			num = LongType.FromString(s1);
		}
		long num2 = default(long);
		if (s2 != null)
		{
			num2 = LongType.FromString(s2);
		}
		return num / num2;
	}

	private static object IDivideByte(byte d1, byte d2)
	{
		checked
		{
			return (byte)unchecked((uint)d1 / (uint)d2);
		}
	}

	private static object IDivideInt16(short d1, short d2)
	{
		checked
		{
			return (short)unchecked(d1 / d2);
		}
	}

	private static object IDivideInt32(int d1, int d2)
	{
		return d1 / d2;
	}

	private static object IDivideInt64(long d1, long d2)
	{
		return d1 / d2;
	}

	public static object ShiftLeftObj(object o1, int amount)
	{
		IConvertible convertible = o1 as IConvertible;
		switch ((TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((o1 != null) ? 1 : 0)))
		{
		case TypeCode.Empty:
			return 0 << amount;
		case TypeCode.Boolean:
			return (short)((short)(0 - (convertible.ToBoolean(null) ? 1 : 0)) << (amount & 0xF));
		case TypeCode.Byte:
			return (byte)(convertible.ToByte(null) << (amount & 7));
		case TypeCode.Int16:
			return (short)(convertible.ToInt16(null) << (amount & 0xF));
		case TypeCode.Int32:
			return convertible.ToInt32(null) << amount;
		case TypeCode.Int64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			return convertible.ToInt64(null) << amount;
		case TypeCode.String:
			return LongType.FromString(convertible.ToString(null)) << amount;
		default:
			throw GetNoValidOperatorException(o1);
		}
	}

	public static object ShiftRightObj(object o1, int amount)
	{
		IConvertible convertible = o1 as IConvertible;
		switch ((TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((o1 != null) ? 1 : 0)))
		{
		case TypeCode.Empty:
			return 0 >> amount;
		case TypeCode.Boolean:
			return (short)((short)(0 - (convertible.ToBoolean(null) ? 1 : 0)) >> (amount & 0xF));
		case TypeCode.Byte:
			return (byte)((uint)convertible.ToByte(null) >> (amount & 7));
		case TypeCode.Int16:
			return (short)(convertible.ToInt16(null) >> (amount & 0xF));
		case TypeCode.Int32:
			return convertible.ToInt32(null) >> amount;
		case TypeCode.Int64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			return convertible.ToInt64(null) >> amount;
		case TypeCode.String:
			return LongType.FromString(convertible.ToString(null)) >> amount;
		default:
			throw GetNoValidOperatorException(o1);
		}
	}

	public static object XorObj(object obj1, object obj2)
	{
		if (obj1 == null && obj2 == null)
		{
			return false;
		}
		switch (GetWidestType(obj1, obj2))
		{
		case TypeCode.Boolean:
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.Int32:
		case TypeCode.Int64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
		case TypeCode.String:
			return BooleanType.FromObject(obj1) ^ BooleanType.FromObject(obj2);
		default:
			throw GetNoValidOperatorException(obj1, obj2);
		}
	}

	public static bool LikeObj(object vLeft, object vRight, CompareMethod CompareOption)
	{
		return StringType.StrLike(StringType.FromObject(vLeft), StringType.FromObject(vRight), CompareOption);
	}

	public static object StrCatObj(object vLeft, object vRight)
	{
		bool flag = vLeft is DBNull;
		bool flag2 = vRight is DBNull;
		if (flag & flag2)
		{
			return vLeft;
		}
		if (flag & !flag2)
		{
			vLeft = "";
		}
		else if (flag2 & !flag)
		{
			vRight = "";
		}
		return StringType.FromObject(vLeft) + StringType.FromObject(vRight);
	}

	internal static object CTypeHelper(object obj, TypeCode toType)
	{
		if (obj == null)
		{
			return null;
		}
		return toType switch
		{
			TypeCode.Boolean => BooleanType.FromObject(obj), 
			TypeCode.Byte => ByteType.FromObject(obj), 
			TypeCode.Int16 => ShortType.FromObject(obj), 
			TypeCode.Int32 => IntegerType.FromObject(obj), 
			TypeCode.Int64 => LongType.FromObject(obj), 
			TypeCode.Decimal => DecimalType.FromObject(obj), 
			TypeCode.Single => SingleType.FromObject(obj), 
			TypeCode.Double => DoubleType.FromObject(obj), 
			TypeCode.String => StringType.FromObject(obj), 
			TypeCode.Char => CharType.FromObject(obj), 
			TypeCode.DateTime => DateType.FromObject(obj), 
			_ => throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(obj), Utils.VBFriendlyName(TypeFromTypeCode(toType)))), 
		};
	}

	internal static object CTypeHelper(object obj, Type toType)
	{
		if (obj == null)
		{
			return null;
		}
		if ((object)toType == typeof(object))
		{
			return obj;
		}
		Type type = obj.GetType();
		bool flag = default(bool);
		if (toType.IsByRef)
		{
			toType = toType.GetElementType();
			flag = true;
		}
		if (type.IsByRef)
		{
			type = type.GetElementType();
		}
		object obj2;
		if ((object)type == toType || (object)toType == typeof(object))
		{
			if (!flag)
			{
				return obj;
			}
			obj2 = GetObjectValuePrimitive(obj);
		}
		else
		{
			TypeCode typeCode = Type.GetTypeCode(toType);
			if (typeCode == TypeCode.Object)
			{
				if ((object)toType == typeof(object) || toType.IsInstanceOfType(obj))
				{
					return obj;
				}
				if (obj is string value && (object)toType == typeof(char[]))
				{
					return CharArrayType.FromString(value);
				}
				throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(type), Utils.VBFriendlyName(toType)));
			}
			obj2 = CTypeHelper(obj, typeCode);
		}
		if (toType.IsEnum)
		{
			return Enum.ToObject(toType, obj2);
		}
		return obj2;
	}

	private static Exception GetNoValidOperatorException(object Operand)
	{
		return new InvalidCastException(System.SR.Format(System.SR.NoValidOperator_OneOperand, Utils.VBFriendlyName(Operand)));
	}

	private static Exception GetNoValidOperatorException(object Left, object Right)
	{
		return new InvalidCastException(System.SR.Format(p1: (Left == null) ? "'Nothing'" : ((!(Left is string str)) ? System.SR.Format(System.SR.NoValidOperator_NonStringType1, Utils.VBFriendlyName(Left)) : System.SR.Format(System.SR.NoValidOperator_StringType1, Strings.Left(str, 32))), p2: (Right == null) ? "'Nothing'" : ((!(Right is string str2)) ? System.SR.Format(System.SR.NoValidOperator_NonStringType1, Utils.VBFriendlyName(Right)) : System.SR.Format(System.SR.NoValidOperator_StringType1, Strings.Left(str2, 32))), resourceFormat: System.SR.NoValidOperator_TwoOperands));
	}

	public static object GetObjectValuePrimitive(object o)
	{
		if (o == null)
		{
			return null;
		}
		if (!(o is IConvertible convertible))
		{
			return o;
		}
		return convertible.GetTypeCode() switch
		{
			TypeCode.Char => convertible.ToChar(null), 
			TypeCode.String => o, 
			TypeCode.Boolean => convertible.ToBoolean(null), 
			TypeCode.Byte => convertible.ToByte(null), 
			TypeCode.SByte => convertible.ToSByte(null), 
			TypeCode.Int16 => convertible.ToInt16(null), 
			TypeCode.UInt16 => convertible.ToUInt16(null), 
			TypeCode.Int32 => convertible.ToInt32(null), 
			TypeCode.UInt32 => convertible.ToUInt32(null), 
			TypeCode.Int64 => convertible.ToInt64(null), 
			TypeCode.UInt64 => convertible.ToUInt64(null), 
			TypeCode.Single => convertible.ToSingle(null), 
			TypeCode.Double => convertible.ToDouble(null), 
			TypeCode.Decimal => convertible.ToDecimal(null), 
			TypeCode.DateTime => convertible.ToDateTime(null), 
			_ => o, 
		};
	}
}
