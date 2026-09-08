using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class Operators
{
	private enum CompareClass
	{
		Less = -1,
		Equal,
		Greater,
		Unordered,
		UserDefined,
		Undefined
	}

	internal static readonly object Boxed_ZeroDouble = 0.0;

	internal static readonly object Boxed_ZeroSinge = 0f;

	internal static readonly object Boxed_ZeroDecimal = 0m;

	internal static readonly object Boxed_ZeroLong = 0L;

	internal static readonly object Boxed_ZeroInteger = 0;

	internal static readonly object Boxed_ZeroShort = (short)0;

	internal static readonly object Boxed_ZeroULong = 0uL;

	internal static readonly object Boxed_ZeroUInteger = 0u;

	internal static readonly object Boxed_ZeroUShort = (ushort)0;

	internal static readonly object Boxed_ZeroSByte = (sbyte)0;

	internal static readonly object Boxed_ZeroByte = (byte)0;

	[RequiresUnreferencedCode("Calls CollectOverloadCandidates")]
	internal static List<Symbols.Method> CollectOperators(Symbols.UserDefinedOperator op, Type type1, Type type2, ref bool foundType1Operators, ref bool foundType2Operators)
	{
		bool num = (object)type2 != null;
		List<Symbols.Method> list;
		if (!Symbols.IsRootObjectType(type1) && Symbols.IsClassOrValueType(type1))
		{
			MemberInfo[] members = new Symbols.Container(type1).LookupNamedMembers(Symbols.OperatorCLSNames[(int)op]);
			int argumentCount = Interaction.IIf(Symbols.IsUnaryOperator(op), 1, 2);
			int rejectedForArgumentCount = 0;
			int rejectedForTypeArgumentCount = 0;
			list = OverloadResolution.CollectOverloadCandidates(members, null, argumentCount, null, null, collectOnlyOperators: true, null, ref rejectedForArgumentCount, ref rejectedForTypeArgumentCount, null);
			if (list.Count > 0)
			{
				foundType1Operators = true;
			}
		}
		else
		{
			list = new List<Symbols.Method>();
		}
		if (num && !Symbols.IsRootObjectType(type2) && Symbols.IsClassOrValueType(type2))
		{
			Type type3 = type1;
			while ((object)type3 != null && !Symbols.IsOrInheritsFrom(type2, type3))
			{
				type3 = type3.BaseType;
			}
			MemberInfo[] members2 = new Symbols.Container(type2).LookupNamedMembers(Symbols.OperatorCLSNames[(int)op]);
			int argumentCount2 = Interaction.IIf(Symbols.IsUnaryOperator(op), 1, 2);
			Type terminatingScope = type3;
			int rejectedForTypeArgumentCount = 0;
			int rejectedForArgumentCount = 0;
			List<Symbols.Method> list2 = OverloadResolution.CollectOverloadCandidates(members2, null, argumentCount2, null, null, collectOnlyOperators: true, terminatingScope, ref rejectedForTypeArgumentCount, ref rejectedForArgumentCount, null);
			if (list2.Count > 0)
			{
				foundType2Operators = true;
			}
			list.AddRange(list2);
		}
		return list;
	}

	[RequiresUnreferencedCode("Calls ResolveOverloadedCall")]
	internal static Symbols.Method ResolveUserDefinedOperator(Symbols.UserDefinedOperator op, object[] arguments, bool reportErrors)
	{
		arguments = (object[])arguments.Clone();
		Type type = null;
		Type type2;
		if (arguments[0] == null)
		{
			type = arguments[1].GetType();
			type2 = type;
			arguments[0] = new Symbols.TypedNothing(type2);
		}
		else
		{
			type2 = arguments[0].GetType();
			if (arguments.Length > 1)
			{
				if (arguments[1] != null)
				{
					type = arguments[1].GetType();
				}
				else
				{
					type = type2;
					arguments[1] = new Symbols.TypedNothing(type);
				}
			}
		}
		bool foundType1Operators = default(bool);
		bool foundType2Operators = default(bool);
		List<Symbols.Method> list = CollectOperators(op, type2, type, ref foundType1Operators, ref foundType2Operators);
		OverloadResolution.ResolutionFailure failure = default(OverloadResolution.ResolutionFailure);
		if (list.Count > 0)
		{
			return OverloadResolution.ResolveOverloadedCall(Symbols.OperatorNames[(int)op], list, arguments, Symbols.NoArgumentNames, Symbols.NoTypeArguments, BindingFlags.InvokeMethod, reportErrors, ref failure);
		}
		return null;
	}

	[RequiresUnreferencedCode("Calls Container.InvokeMethod")]
	internal static object InvokeUserDefinedOperator(Symbols.Method operatorMethod, bool forceArgumentValidation, params object[] arguments)
	{
		if ((!operatorMethod.ArgumentsValidated || forceArgumentValidation) && !OverloadResolution.CanMatchArguments(operatorMethod, arguments, Symbols.NoArgumentNames, Symbols.NoTypeArguments, rejectNarrowingConversions: false, null))
		{
			string text = "";
			List<string> list = new List<string>();
			OverloadResolution.CanMatchArguments(operatorMethod, arguments, Symbols.NoArgumentNames, Symbols.NoTypeArguments, rejectNarrowingConversions: false, list);
			foreach (string item in list)
			{
				text = text + "\r\n    " + item;
			}
			text = System.SR.Format(System.SR.MatchArgumentFailure2, operatorMethod.ToString(), text);
			throw new InvalidCastException(text);
		}
		return new Symbols.Container(operatorMethod.DeclaringType).InvokeMethod(operatorMethod, arguments, null, BindingFlags.InvokeMethod);
	}

	[RequiresUnreferencedCode("Calls Return IDOBinder.InvokeUserDefinedOperator")]
	internal static object InvokeUserDefinedOperator(Symbols.UserDefinedOperator op, params object[] arguments)
	{
		if (IDOUtils.TryCastToIDMOP(arguments[0]) != null)
		{
			return IDOBinder.InvokeUserDefinedOperator(op, arguments);
		}
		return InvokeObjectUserDefinedOperator(op, arguments);
	}

	[Obsolete("FallbackInvokeUserDefinedOperator has been deprecated and is not supported.", true)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object FallbackInvokeUserDefinedOperator(object vbOp, object[] arguments)
	{
		return InvokeObjectUserDefinedOperator((Symbols.UserDefinedOperator)Conversions.ToSByte(vbOp), arguments);
	}

	[RequiresUnreferencedCode("Calls InvokeUserDefinedOperator")]
	internal static object InvokeObjectUserDefinedOperator(Symbols.UserDefinedOperator op, object[] arguments)
	{
		Symbols.Method method = ResolveUserDefinedOperator(op, arguments, reportErrors: true);
		if ((object)method != null)
		{
			return InvokeUserDefinedOperator(method, forceArgumentValidation: false, arguments);
		}
		if (arguments.Length > 1)
		{
			throw GetNoValidOperatorException(op, arguments[0], arguments[1]);
		}
		throw GetNoValidOperatorException(op, arguments[0]);
	}

	[RequiresUnreferencedCode("Calls CanMatchArguments")]
	internal static Symbols.Method GetCallableUserDefinedOperator(Symbols.UserDefinedOperator op, params object[] arguments)
	{
		Symbols.Method method = ResolveUserDefinedOperator(op, arguments, reportErrors: false);
		if ((object)method != null && !method.ArgumentsValidated && !OverloadResolution.CanMatchArguments(method, arguments, Symbols.NoArgumentNames, Symbols.NoTypeArguments, rejectNarrowingConversions: false, null))
		{
			return null;
		}
		return method;
	}

	private static sbyte ToVBBool(object conv)
	{
		return (sbyte)(0 - (Convert.ToBoolean(conv) ? 1 : 0));
	}

	private static TypeCode GetTypeCode(object o)
	{
		if (o == null)
		{
			return TypeCode.Empty;
		}
		return ReflectionExtensions.GetTypeCode(o.GetType());
	}

	private static Type GetEnumResult(object left, object right)
	{
		if (left != null)
		{
			if (left is Enum)
			{
				if (right == null)
				{
					return left.GetType();
				}
				if (right is Enum)
				{
					Type type = left.GetType();
					if ((object)type == right.GetType())
					{
						return type;
					}
				}
			}
		}
		else if (right is Enum)
		{
			return right.GetType();
		}
		return null;
	}

	private static Exception GetNoValidOperatorException(Symbols.UserDefinedOperator op, object operand)
	{
		return new InvalidCastException(System.SR.Format(System.SR.UnaryOperand2, Symbols.OperatorNames[(int)op], Utils.VBFriendlyName(operand)));
	}

	private static Exception GetNoValidOperatorException(Symbols.UserDefinedOperator op, object left, object right)
	{
		return new InvalidCastException(System.SR.Format(p2: (left == null) ? "'Nothing'" : ((!(left is string str)) ? System.SR.Format(System.SR.NoValidOperator_NonStringType1, Utils.VBFriendlyName(left)) : System.SR.Format(System.SR.NoValidOperator_StringType1, Strings.Left(str, 32))), p3: (right == null) ? "'Nothing'" : ((!(right is string str2)) ? System.SR.Format(System.SR.NoValidOperator_NonStringType1, Utils.VBFriendlyName(right)) : System.SR.Format(System.SR.NoValidOperator_StringType1, Strings.Left(str2, 32))), resourceFormat: System.SR.BinaryOperands3, p1: Symbols.OperatorNames[(int)op]));
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object CompareObjectEqual(object Left, object Right, bool TextCompare)
	{
		CompareClass compareClass = CompareObject2(Left, Right, TextCompare);
		return compareClass switch
		{
			CompareClass.Unordered => false, 
			CompareClass.UserDefined => InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Equal, Left, Right), 
			CompareClass.Undefined => throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Equal, Left, Right), 
			_ => compareClass == CompareClass.Equal, 
		};
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static bool ConditionalCompareObjectEqual(object Left, object Right, bool TextCompare)
	{
		CompareClass compareClass = CompareObject2(Left, Right, TextCompare);
		return compareClass switch
		{
			CompareClass.Unordered => false, 
			CompareClass.UserDefined => Conversions.ToBoolean(InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Equal, Left, Right)), 
			CompareClass.Undefined => throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Equal, Left, Right), 
			_ => compareClass == CompareClass.Equal, 
		};
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object CompareObjectNotEqual(object Left, object Right, bool TextCompare)
	{
		CompareClass compareClass = CompareObject2(Left, Right, TextCompare);
		return compareClass switch
		{
			CompareClass.Unordered => true, 
			CompareClass.UserDefined => InvokeUserDefinedOperator(Symbols.UserDefinedOperator.NotEqual, Left, Right), 
			CompareClass.Undefined => throw GetNoValidOperatorException(Symbols.UserDefinedOperator.NotEqual, Left, Right), 
			_ => compareClass != CompareClass.Equal, 
		};
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static bool ConditionalCompareObjectNotEqual(object Left, object Right, bool TextCompare)
	{
		CompareClass compareClass = CompareObject2(Left, Right, TextCompare);
		return compareClass switch
		{
			CompareClass.Unordered => true, 
			CompareClass.UserDefined => Conversions.ToBoolean(InvokeUserDefinedOperator(Symbols.UserDefinedOperator.NotEqual, Left, Right)), 
			CompareClass.Undefined => throw GetNoValidOperatorException(Symbols.UserDefinedOperator.NotEqual, Left, Right), 
			_ => compareClass != CompareClass.Equal, 
		};
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object CompareObjectLess(object Left, object Right, bool TextCompare)
	{
		CompareClass compareClass = CompareObject2(Left, Right, TextCompare);
		return compareClass switch
		{
			CompareClass.Unordered => false, 
			CompareClass.UserDefined => InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Less, Left, Right), 
			CompareClass.Undefined => throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Less, Left, Right), 
			_ => compareClass < CompareClass.Equal, 
		};
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static bool ConditionalCompareObjectLess(object Left, object Right, bool TextCompare)
	{
		CompareClass compareClass = CompareObject2(Left, Right, TextCompare);
		return compareClass switch
		{
			CompareClass.Unordered => false, 
			CompareClass.UserDefined => Conversions.ToBoolean(InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Less, Left, Right)), 
			CompareClass.Undefined => throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Less, Left, Right), 
			_ => compareClass < CompareClass.Equal, 
		};
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object CompareObjectLessEqual(object Left, object Right, bool TextCompare)
	{
		CompareClass compareClass = CompareObject2(Left, Right, TextCompare);
		return compareClass switch
		{
			CompareClass.Unordered => false, 
			CompareClass.UserDefined => InvokeUserDefinedOperator(Symbols.UserDefinedOperator.LessEqual, Left, Right), 
			CompareClass.Undefined => throw GetNoValidOperatorException(Symbols.UserDefinedOperator.LessEqual, Left, Right), 
			_ => compareClass <= CompareClass.Equal, 
		};
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static bool ConditionalCompareObjectLessEqual(object Left, object Right, bool TextCompare)
	{
		CompareClass compareClass = CompareObject2(Left, Right, TextCompare);
		return compareClass switch
		{
			CompareClass.Unordered => false, 
			CompareClass.UserDefined => Conversions.ToBoolean(InvokeUserDefinedOperator(Symbols.UserDefinedOperator.LessEqual, Left, Right)), 
			CompareClass.Undefined => throw GetNoValidOperatorException(Symbols.UserDefinedOperator.LessEqual, Left, Right), 
			_ => compareClass <= CompareClass.Equal, 
		};
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object CompareObjectGreaterEqual(object Left, object Right, bool TextCompare)
	{
		CompareClass compareClass = CompareObject2(Left, Right, TextCompare);
		return compareClass switch
		{
			CompareClass.Unordered => false, 
			CompareClass.UserDefined => InvokeUserDefinedOperator(Symbols.UserDefinedOperator.GreaterEqual, Left, Right), 
			CompareClass.Undefined => throw GetNoValidOperatorException(Symbols.UserDefinedOperator.GreaterEqual, Left, Right), 
			_ => compareClass >= CompareClass.Equal, 
		};
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static bool ConditionalCompareObjectGreaterEqual(object Left, object Right, bool TextCompare)
	{
		CompareClass compareClass = CompareObject2(Left, Right, TextCompare);
		return compareClass switch
		{
			CompareClass.Unordered => false, 
			CompareClass.UserDefined => Conversions.ToBoolean(InvokeUserDefinedOperator(Symbols.UserDefinedOperator.GreaterEqual, Left, Right)), 
			CompareClass.Undefined => throw GetNoValidOperatorException(Symbols.UserDefinedOperator.GreaterEqual, Left, Right), 
			_ => compareClass >= CompareClass.Equal, 
		};
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object CompareObjectGreater(object Left, object Right, bool TextCompare)
	{
		CompareClass compareClass = CompareObject2(Left, Right, TextCompare);
		return compareClass switch
		{
			CompareClass.Unordered => false, 
			CompareClass.UserDefined => InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Greater, Left, Right), 
			CompareClass.Undefined => throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Greater, Left, Right), 
			_ => compareClass > CompareClass.Equal, 
		};
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static bool ConditionalCompareObjectGreater(object Left, object Right, bool TextCompare)
	{
		CompareClass compareClass = CompareObject2(Left, Right, TextCompare);
		return compareClass switch
		{
			CompareClass.Unordered => false, 
			CompareClass.UserDefined => Conversions.ToBoolean(InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Greater, Left, Right)), 
			CompareClass.Undefined => throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Greater, Left, Right), 
			_ => compareClass > CompareClass.Equal, 
		};
	}

	private static CompareClass CompareObject2(object left, object right, bool textCompare)
	{
		TypeCode typeCode = GetTypeCode(left);
		TypeCode typeCode2 = GetTypeCode(right);
		if (typeCode == TypeCode.Object && left is char[] value && (typeCode2 == TypeCode.String || typeCode2 == TypeCode.Empty || (typeCode2 == TypeCode.Object && right is char[])))
		{
			left = new string(value);
			typeCode = TypeCode.String;
		}
		if (typeCode2 == TypeCode.Object && right is char[] value2 && (typeCode == TypeCode.String || typeCode == TypeCode.Empty))
		{
			right = new string(value2);
			typeCode2 = TypeCode.String;
		}
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 0:
			return CompareClass.Equal;
		case 3:
			return CompareBoolean(left: false, Convert.ToBoolean(right));
		case 5:
			return CompareInt32(0, Convert.ToSByte(right));
		case 6:
			return CompareInt32(0, Convert.ToByte(right));
		case 7:
			return CompareInt32(0, Convert.ToInt16(right));
		case 8:
			return CompareInt32(0, Convert.ToUInt16(right));
		case 9:
			return CompareInt32(0, Convert.ToInt32(right));
		case 10:
			return CompareUInt32(0u, Convert.ToUInt32(right));
		case 11:
			return CompareInt64(0L, Convert.ToInt64(right));
		case 12:
			return CompareUInt64(0uL, Convert.ToUInt64(right));
		case 15:
			return CompareDecimal(0m, right);
		case 13:
			return CompareSingle(0f, Convert.ToSingle(right));
		case 14:
			return CompareDouble(0.0, Convert.ToDouble(right));
		case 16:
			return CompareDate(DateTime.MinValue, Convert.ToDateTime(right));
		case 4:
			return CompareChar('\0', Convert.ToChar(right));
		case 18:
			return (CompareClass)CompareString(null, Convert.ToString(right), textCompare);
		case 57:
			return CompareBoolean(Convert.ToBoolean(left), right: false);
		case 60:
			return CompareBoolean(Convert.ToBoolean(left), Convert.ToBoolean(right));
		case 62:
			return CompareInt32(ToVBBool(left), Convert.ToSByte(right));
		case 63:
		case 64:
			return CompareInt32(ToVBBool(left), Convert.ToInt16(right));
		case 65:
		case 66:
			return CompareInt32(ToVBBool(left), Convert.ToInt32(right));
		case 67:
		case 68:
			return CompareInt64(ToVBBool(left), Convert.ToInt64(right));
		case 69:
		case 72:
			return CompareDecimal(ToVBBool(left), right);
		case 70:
			return CompareSingle(ToVBBool(left), Convert.ToSingle(right));
		case 71:
			return CompareDouble(ToVBBool(left), Convert.ToDouble(right));
		case 75:
			return CompareBoolean(Convert.ToBoolean(left), Conversions.ToBoolean(Convert.ToString(right)));
		case 95:
			return CompareInt32(Convert.ToSByte(left), 0);
		case 98:
			return CompareInt32(Convert.ToSByte(left), ToVBBool(right));
		case 100:
			return CompareInt32(Convert.ToSByte(left), Convert.ToSByte(right));
		case 101:
		case 102:
		case 119:
		case 121:
		case 138:
		case 139:
		case 140:
			return CompareInt32(Convert.ToInt16(left), Convert.ToInt16(right));
		case 103:
		case 104:
		case 123:
		case 141:
		case 142:
		case 157:
		case 159:
		case 161:
		case 176:
		case 177:
		case 178:
		case 179:
		case 180:
			return CompareInt32(Convert.ToInt32(left), Convert.ToInt32(right));
		case 105:
		case 106:
		case 125:
		case 143:
		case 144:
		case 163:
		case 181:
		case 182:
		case 195:
		case 197:
		case 199:
		case 201:
		case 214:
		case 215:
		case 216:
		case 217:
		case 218:
		case 219:
		case 220:
			return CompareInt64(Convert.ToInt64(left), Convert.ToInt64(right));
		case 107:
		case 110:
		case 129:
		case 145:
		case 148:
		case 167:
		case 183:
		case 186:
		case 205:
		case 221:
		case 224:
		case 233:
		case 235:
		case 237:
		case 239:
		case 243:
		case 290:
		case 291:
		case 292:
		case 293:
		case 294:
		case 295:
		case 296:
		case 297:
		case 300:
			return CompareDecimal(left, right);
		case 108:
		case 127:
		case 146:
		case 165:
		case 184:
		case 203:
		case 222:
		case 241:
		case 252:
		case 253:
		case 254:
		case 255:
		case 256:
		case 257:
		case 258:
		case 259:
		case 260:
		case 262:
		case 298:
			return CompareSingle(Convert.ToSingle(left), Convert.ToSingle(right));
		case 109:
		case 128:
		case 147:
		case 166:
		case 185:
		case 204:
		case 223:
		case 242:
		case 261:
		case 271:
		case 272:
		case 273:
		case 274:
		case 275:
		case 276:
		case 277:
		case 278:
		case 279:
		case 280:
		case 281:
		case 299:
			return CompareDouble(Convert.ToDouble(left), Convert.ToDouble(right));
		case 113:
		case 132:
		case 151:
		case 170:
		case 189:
		case 208:
		case 227:
		case 246:
		case 265:
		case 284:
		case 303:
			return CompareDouble(Convert.ToDouble(left), Conversions.ToDouble(Convert.ToString(right)));
		case 114:
			return CompareInt32(Convert.ToByte(left), 0);
		case 117:
			return CompareInt32(Convert.ToInt16(left), ToVBBool(right));
		case 120:
			return CompareInt32(Convert.ToByte(left), Convert.ToByte(right));
		case 122:
		case 158:
		case 160:
			return CompareInt32(Convert.ToUInt16(left), Convert.ToUInt16(right));
		case 124:
		case 162:
		case 196:
		case 198:
		case 200:
			return CompareUInt32(Convert.ToUInt32(left), Convert.ToUInt32(right));
		case 126:
		case 164:
		case 202:
		case 234:
		case 236:
		case 238:
		case 240:
			return CompareUInt64(Convert.ToUInt64(left), Convert.ToUInt64(right));
		case 133:
			return CompareInt32(Convert.ToInt16(left), 0);
		case 136:
			return CompareInt32(Convert.ToInt16(left), ToVBBool(right));
		case 152:
			return CompareInt32(Convert.ToUInt16(left), 0);
		case 155:
			return CompareInt32(Convert.ToInt32(left), ToVBBool(right));
		case 171:
			return CompareInt32(Convert.ToInt32(left), 0);
		case 174:
			return CompareInt32(Convert.ToInt32(left), ToVBBool(right));
		case 190:
			return CompareUInt32(Convert.ToUInt32(left), 0u);
		case 193:
			return CompareInt64(Convert.ToInt64(left), ToVBBool(right));
		case 209:
			return CompareInt64(Convert.ToInt64(left), 0L);
		case 212:
			return CompareInt64(Convert.ToInt64(left), ToVBBool(right));
		case 228:
			return CompareUInt64(Convert.ToUInt64(left), 0uL);
		case 231:
			return CompareDecimal(left, ToVBBool(right));
		case 285:
			return CompareDecimal(left, 0m);
		case 288:
			return CompareDecimal(left, ToVBBool(right));
		case 247:
			return CompareSingle(Convert.ToSingle(left), 0f);
		case 250:
			return CompareSingle(Convert.ToSingle(left), ToVBBool(right));
		case 266:
			return CompareDouble(Convert.ToDouble(left), 0.0);
		case 269:
			return CompareDouble(Convert.ToDouble(left), ToVBBool(right));
		case 304:
			return CompareDate(Convert.ToDateTime(left), DateTime.MinValue);
		case 320:
			return CompareDate(Convert.ToDateTime(left), Convert.ToDateTime(right));
		case 322:
			return CompareDate(Convert.ToDateTime(left), Conversions.ToDate(Convert.ToString(right)));
		case 76:
			return CompareChar(Convert.ToChar(left), '\0');
		case 80:
			return CompareChar(Convert.ToChar(left), Convert.ToChar(right));
		case 94:
		case 346:
		case 360:
			return (CompareClass)CompareString(Convert.ToString(left), Convert.ToString(right), textCompare);
		case 342:
			return (CompareClass)CompareString(Convert.ToString(left), null, textCompare);
		case 345:
			return CompareBoolean(Conversions.ToBoolean(Convert.ToString(left)), Convert.ToBoolean(right));
		case 347:
		case 348:
		case 349:
		case 350:
		case 351:
		case 352:
		case 353:
		case 354:
		case 355:
		case 356:
		case 357:
			return CompareDouble(Conversions.ToDouble(Convert.ToString(left)), Convert.ToDouble(right));
		case 358:
			return CompareDate(Conversions.ToDate(Convert.ToString(left)), Convert.ToDateTime(right));
		default:
			if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
			{
				return CompareClass.UserDefined;
			}
			return CompareClass.Undefined;
		}
	}

	private static CompareClass CompareBoolean(bool left, bool right)
	{
		if (left == right)
		{
			return CompareClass.Equal;
		}
		if ((left ? 1 : 0) < (right ? 1 : 0))
		{
			return CompareClass.Greater;
		}
		return CompareClass.Less;
	}

	private static CompareClass CompareInt32(int left, int right)
	{
		if (left == right)
		{
			return CompareClass.Equal;
		}
		if (left > right)
		{
			return CompareClass.Greater;
		}
		return CompareClass.Less;
	}

	private static CompareClass CompareUInt32(uint left, uint right)
	{
		if (left == right)
		{
			return CompareClass.Equal;
		}
		if (left > right)
		{
			return CompareClass.Greater;
		}
		return CompareClass.Less;
	}

	private static CompareClass CompareInt64(long left, long right)
	{
		if (left == right)
		{
			return CompareClass.Equal;
		}
		if (left > right)
		{
			return CompareClass.Greater;
		}
		return CompareClass.Less;
	}

	private static CompareClass CompareUInt64(ulong left, ulong right)
	{
		if (left == right)
		{
			return CompareClass.Equal;
		}
		if (left > right)
		{
			return CompareClass.Greater;
		}
		return CompareClass.Less;
	}

	private static CompareClass CompareDecimal(object left, object right)
	{
		int num = decimal.Compare(Convert.ToDecimal(left), Convert.ToDecimal(right));
		if (num == 0)
		{
			return CompareClass.Equal;
		}
		if (num > 0)
		{
			return CompareClass.Greater;
		}
		return CompareClass.Less;
	}

	private static CompareClass CompareSingle(float left, float right)
	{
		if (left == right)
		{
			return CompareClass.Equal;
		}
		if (left < right)
		{
			return CompareClass.Less;
		}
		if (left > right)
		{
			return CompareClass.Greater;
		}
		return CompareClass.Unordered;
	}

	private static CompareClass CompareDouble(double left, double right)
	{
		if (left == right)
		{
			return CompareClass.Equal;
		}
		if (left < right)
		{
			return CompareClass.Less;
		}
		if (left > right)
		{
			return CompareClass.Greater;
		}
		return CompareClass.Unordered;
	}

	private static CompareClass CompareDate(DateTime left, DateTime right)
	{
		int num = DateTime.Compare(left, right);
		if (num == 0)
		{
			return CompareClass.Equal;
		}
		if (num > 0)
		{
			return CompareClass.Greater;
		}
		return CompareClass.Less;
	}

	private static CompareClass CompareChar(char left, char right)
	{
		if (left == right)
		{
			return CompareClass.Equal;
		}
		if (left > right)
		{
			return CompareClass.Greater;
		}
		return CompareClass.Less;
	}

	public static int CompareString(string Left, string Right, bool TextCompare)
	{
		if ((object)Left == Right)
		{
			return 0;
		}
		if (Left == null)
		{
			if (Right.Length == 0)
			{
				return 0;
			}
			return -1;
		}
		if (Right == null)
		{
			if (Left.Length == 0)
			{
				return 0;
			}
			return 1;
		}
		int num = ((!TextCompare) ? string.CompareOrdinal(Left, Right) : Utils.GetCultureInfo().CompareInfo.Compare(Left, Right, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth));
		if (num == 0)
		{
			return 0;
		}
		if (num > 0)
		{
			return 1;
		}
		return -1;
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object PlusObject(object Operand)
	{
		switch (GetTypeCode(Operand))
		{
		case TypeCode.Empty:
			return Boxed_ZeroInteger;
		case TypeCode.Boolean:
			return (short)(0 - (Convert.ToBoolean(Operand) ? 1 : 0));
		case TypeCode.SByte:
			return Convert.ToSByte(Operand);
		case TypeCode.Byte:
			return Convert.ToByte(Operand);
		case TypeCode.Int16:
			return Convert.ToInt16(Operand);
		case TypeCode.UInt16:
			return Convert.ToUInt16(Operand);
		case TypeCode.Int32:
			return Convert.ToInt32(Operand);
		case TypeCode.UInt32:
			return Convert.ToUInt32(Operand);
		case TypeCode.Int64:
			return Convert.ToInt64(Operand);
		case TypeCode.UInt64:
			return Convert.ToUInt64(Operand);
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			return Operand;
		case TypeCode.String:
			return Conversions.ToDouble(Convert.ToString(Operand));
		case TypeCode.Object:
			return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.UnaryPlus, Operand);
		default:
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.UnaryPlus, Operand);
		}
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object NegateObject(object Operand)
	{
		switch (GetTypeCode(Operand))
		{
		case TypeCode.Empty:
			return Boxed_ZeroInteger;
		case TypeCode.Boolean:
			if (Operand is bool)
			{
				return NegateBoolean((bool)Operand);
			}
			return NegateBoolean(Convert.ToBoolean(Operand));
		case TypeCode.SByte:
			if (Operand is sbyte)
			{
				return NegateSByte((sbyte)Operand);
			}
			return NegateSByte(Convert.ToSByte(Operand));
		case TypeCode.Byte:
			if (Operand is byte)
			{
				return NegateByte((byte)Operand);
			}
			return NegateByte(Convert.ToByte(Operand));
		case TypeCode.Int16:
			if (Operand is short)
			{
				return NegateInt16((short)Operand);
			}
			return NegateInt16(Convert.ToInt16(Operand));
		case TypeCode.UInt16:
			if (Operand is ushort)
			{
				return NegateUInt16((ushort)Operand);
			}
			return NegateUInt16(Convert.ToUInt16(Operand));
		case TypeCode.Int32:
			if (Operand is int)
			{
				return NegateInt32((int)Operand);
			}
			return NegateInt32(Convert.ToInt32(Operand));
		case TypeCode.UInt32:
			if (Operand is uint)
			{
				return NegateUInt32((uint)Operand);
			}
			return NegateUInt32(Convert.ToUInt32(Operand));
		case TypeCode.Int64:
			if (Operand is long)
			{
				return NegateInt64((long)Operand);
			}
			return NegateInt64(Convert.ToInt64(Operand));
		case TypeCode.UInt64:
			if (Operand is ulong)
			{
				return NegateUInt64((ulong)Operand);
			}
			return NegateUInt64(Convert.ToUInt64(Operand));
		case TypeCode.Decimal:
			if (Operand is decimal)
			{
				return NegateDecimal((decimal)Operand);
			}
			return NegateDecimal(Convert.ToDecimal(Operand));
		case TypeCode.Single:
			if (Operand is float)
			{
				return NegateSingle((float)Operand);
			}
			return NegateSingle(Convert.ToSingle(Operand));
		case TypeCode.Double:
			if (Operand is double)
			{
				return NegateDouble((double)Operand);
			}
			return NegateDouble(Convert.ToDouble(Operand));
		case TypeCode.String:
			if (Operand is string operand)
			{
				return NegateString(operand);
			}
			return NegateString(Convert.ToString(Operand));
		case TypeCode.Object:
			return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Negate, Operand);
		default:
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Negate, Operand);
		}
	}

	private static object NegateBoolean(bool operand)
	{
		checked
		{
			return (short)unchecked(-(short)(0 - (operand ? 1 : 0)));
		}
	}

	private static object NegateSByte(sbyte operand)
	{
		if (operand == sbyte.MinValue)
		{
			return (short)128;
		}
		checked
		{
			return (sbyte)unchecked(-operand);
		}
	}

	private static object NegateByte(byte operand)
	{
		checked
		{
			return (short)unchecked(-operand);
		}
	}

	private static object NegateInt16(short operand)
	{
		if (operand == short.MinValue)
		{
			return 32768;
		}
		checked
		{
			return (short)unchecked(-operand);
		}
	}

	private static object NegateUInt16(ushort operand)
	{
		return checked(-operand);
	}

	private static object NegateInt32(int operand)
	{
		if (operand == int.MinValue)
		{
			return 2147483648L;
		}
		return checked(-operand);
	}

	private static object NegateUInt32(uint operand)
	{
		checked
		{
			return 0L - unchecked((long)operand);
		}
	}

	private static object NegateInt64(long operand)
	{
		if (operand == long.MinValue)
		{
			return 9223372036854775808m;
		}
		return checked(-operand);
	}

	private static object NegateUInt64(ulong operand)
	{
		return decimal.Negate(new decimal(operand));
	}

	private static object NegateDecimal(decimal operand)
	{
		return decimal.Negate(operand);
	}

	private static object NegateSingle(float operand)
	{
		return 0f - operand;
	}

	private static object NegateDouble(double operand)
	{
		return 0.0 - operand;
	}

	private static object NegateString(string operand)
	{
		return 0.0 - Conversions.ToDouble(operand);
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object NotObject(object Operand)
	{
		switch (GetTypeCode(Operand))
		{
		case TypeCode.Empty:
			return -1;
		case TypeCode.Boolean:
			return NotBoolean(Convert.ToBoolean(Operand));
		case TypeCode.SByte:
			return NotSByte(Convert.ToSByte(Operand), Operand.GetType());
		case TypeCode.Byte:
			return NotByte(Convert.ToByte(Operand), Operand.GetType());
		case TypeCode.Int16:
			return NotInt16(Convert.ToInt16(Operand), Operand.GetType());
		case TypeCode.UInt16:
			return NotUInt16(Convert.ToUInt16(Operand), Operand.GetType());
		case TypeCode.Int32:
			return NotInt32(Convert.ToInt32(Operand), Operand.GetType());
		case TypeCode.UInt32:
			return NotUInt32(Convert.ToUInt32(Operand), Operand.GetType());
		case TypeCode.Int64:
			return NotInt64(Convert.ToInt64(Operand), Operand.GetType());
		case TypeCode.UInt64:
			return NotUInt64(Convert.ToUInt64(Operand), Operand.GetType());
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			return NotInt64(Convert.ToInt64(Operand));
		case TypeCode.String:
			return NotInt64(Conversions.ToLong(Convert.ToString(Operand)));
		case TypeCode.Object:
			return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Not, Operand);
		default:
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Not, Operand);
		}
	}

	private static object NotBoolean(bool operand)
	{
		return !operand;
	}

	private static object NotSByte(sbyte operand, Type operandType)
	{
		sbyte b = (sbyte)(~operand);
		if (operandType.IsEnum)
		{
			return Enum.ToObject(operandType, b);
		}
		return b;
	}

	private static object NotByte(byte operand, Type operandType)
	{
		byte b = (byte)(~operand);
		if (operandType.IsEnum)
		{
			return Enum.ToObject(operandType, b);
		}
		return b;
	}

	private static object NotInt16(short operand, Type operandType)
	{
		short num = (short)(~operand);
		if (operandType.IsEnum)
		{
			return Enum.ToObject(operandType, num);
		}
		return num;
	}

	private static object NotUInt16(ushort operand, Type operandType)
	{
		ushort num = (ushort)(~operand);
		if (operandType.IsEnum)
		{
			return Enum.ToObject(operandType, num);
		}
		return num;
	}

	private static object NotInt32(int operand, Type operandType)
	{
		int num = ~operand;
		if (operandType.IsEnum)
		{
			return Enum.ToObject(operandType, num);
		}
		return num;
	}

	private static object NotUInt32(uint operand, Type operandType)
	{
		uint num = ~operand;
		if (operandType.IsEnum)
		{
			return Enum.ToObject(operandType, num);
		}
		return num;
	}

	private static object NotInt64(long operand)
	{
		return ~operand;
	}

	private static object NotInt64(long operand, Type operandType)
	{
		long num = ~operand;
		if (operandType.IsEnum)
		{
			return Enum.ToObject(operandType, num);
		}
		return num;
	}

	private static object NotUInt64(ulong operand, Type operandType)
	{
		ulong num = ~operand;
		if (operandType.IsEnum)
		{
			return Enum.ToObject(operandType, num);
		}
		return num;
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object AndObject(object Left, object Right)
	{
		TypeCode typeCode = GetTypeCode(Left);
		TypeCode typeCode2 = GetTypeCode(Right);
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 0:
			return Boxed_ZeroInteger;
		case 3:
		case 57:
			return false;
		case 5:
		case 95:
			return AndSByte(0, 0, GetEnumResult(Left, Right));
		case 6:
		case 114:
			return AndByte(0, 0, GetEnumResult(Left, Right));
		case 7:
		case 133:
			return AndInt16(0, 0, GetEnumResult(Left, Right));
		case 8:
		case 152:
			return AndUInt16(0, 0, GetEnumResult(Left, Right));
		case 9:
		case 171:
			return AndInt32(0, 0, GetEnumResult(Left, Right));
		case 10:
		case 190:
			return AndUInt32(0u, 0u, GetEnumResult(Left, Right));
		case 11:
		case 209:
			return AndInt64(0L, 0L, GetEnumResult(Left, Right));
		case 12:
		case 228:
			return AndUInt64(0uL, 0uL, GetEnumResult(Left, Right));
		case 13:
		case 14:
		case 15:
			return AndInt64(0L, Convert.ToInt64(Right));
		case 18:
			return AndInt64(0L, Conversions.ToLong(Convert.ToString(Right)));
		case 60:
			return AndBoolean(Convert.ToBoolean(Left), Convert.ToBoolean(Right));
		case 62:
			return AndSByte(ToVBBool(Left), Convert.ToSByte(Right));
		case 63:
		case 64:
			return AndInt16(ToVBBool(Left), Convert.ToInt16(Right));
		case 65:
		case 66:
			return AndInt32(ToVBBool(Left), Convert.ToInt32(Right));
		case 67:
		case 68:
		case 69:
		case 70:
		case 71:
		case 72:
			return AndInt64(ToVBBool(Left), Convert.ToInt64(Right));
		case 75:
			return AndBoolean(Convert.ToBoolean(Left), Conversions.ToBoolean(Convert.ToString(Right)));
		case 98:
			return AndSByte(Convert.ToSByte(Left), ToVBBool(Right));
		case 100:
			return AndSByte(Convert.ToSByte(Left), Convert.ToSByte(Right), GetEnumResult(Left, Right));
		case 101:
		case 102:
		case 119:
		case 121:
		case 138:
		case 139:
			return AndInt16(Convert.ToInt16(Left), Convert.ToInt16(Right));
		case 103:
		case 104:
		case 123:
		case 141:
		case 142:
		case 157:
		case 159:
		case 161:
		case 176:
		case 177:
		case 178:
		case 179:
			return AndInt32(Convert.ToInt32(Left), Convert.ToInt32(Right));
		case 105:
		case 106:
		case 107:
		case 108:
		case 109:
		case 110:
		case 125:
		case 127:
		case 128:
		case 129:
		case 143:
		case 144:
		case 145:
		case 146:
		case 147:
		case 148:
		case 163:
		case 165:
		case 166:
		case 167:
		case 181:
		case 182:
		case 183:
		case 184:
		case 185:
		case 186:
		case 195:
		case 197:
		case 199:
		case 201:
		case 203:
		case 204:
		case 205:
		case 214:
		case 215:
		case 216:
		case 217:
		case 218:
		case 219:
		case 221:
		case 222:
		case 223:
		case 224:
		case 233:
		case 235:
		case 237:
		case 239:
		case 241:
		case 242:
		case 243:
		case 252:
		case 253:
		case 254:
		case 255:
		case 256:
		case 257:
		case 258:
		case 259:
		case 260:
		case 261:
		case 262:
		case 271:
		case 272:
		case 273:
		case 274:
		case 275:
		case 276:
		case 277:
		case 278:
		case 279:
		case 280:
		case 281:
		case 290:
		case 291:
		case 292:
		case 293:
		case 294:
		case 295:
		case 296:
		case 297:
		case 298:
		case 299:
		case 300:
			return AndInt64(Convert.ToInt64(Left), Convert.ToInt64(Right));
		case 113:
		case 132:
		case 151:
		case 170:
		case 189:
		case 208:
		case 227:
		case 246:
		case 265:
		case 284:
		case 303:
			return AndInt64(Convert.ToInt64(Left), Conversions.ToLong(Convert.ToString(Right)));
		case 117:
		case 136:
			return AndInt16(Convert.ToInt16(Left), ToVBBool(Right));
		case 120:
			return AndByte(Convert.ToByte(Left), Convert.ToByte(Right), GetEnumResult(Left, Right));
		case 122:
		case 158:
			return AndUInt16(Convert.ToUInt16(Left), Convert.ToUInt16(Right));
		case 124:
		case 162:
		case 196:
		case 198:
			return AndUInt32(Convert.ToUInt32(Left), Convert.ToUInt32(Right));
		case 126:
		case 164:
		case 202:
		case 234:
		case 236:
		case 238:
			return AndUInt64(Convert.ToUInt64(Left), Convert.ToUInt64(Right));
		case 140:
			return AndInt16(Convert.ToInt16(Left), Convert.ToInt16(Right), GetEnumResult(Left, Right));
		case 155:
		case 174:
			return AndInt32(Convert.ToInt32(Left), ToVBBool(Right));
		case 160:
			return AndUInt16(Convert.ToUInt16(Left), Convert.ToUInt16(Right), GetEnumResult(Left, Right));
		case 180:
			return AndInt32(Convert.ToInt32(Left), Convert.ToInt32(Right), GetEnumResult(Left, Right));
		case 193:
		case 212:
		case 231:
		case 250:
		case 269:
		case 288:
			return AndInt64(Convert.ToInt64(Left), ToVBBool(Right));
		case 200:
			return AndUInt32(Convert.ToUInt32(Left), Convert.ToUInt32(Right), GetEnumResult(Left, Right));
		case 220:
			return AndInt64(Convert.ToInt64(Left), Convert.ToInt64(Right), GetEnumResult(Left, Right));
		case 240:
			return AndUInt64(Convert.ToUInt64(Left), Convert.ToUInt64(Right), GetEnumResult(Left, Right));
		case 247:
		case 266:
		case 285:
			return AndInt64(Convert.ToInt64(Left), 0L);
		case 342:
			return AndInt64(Conversions.ToLong(Convert.ToString(Left)), 0L);
		case 345:
			return AndBoolean(Conversions.ToBoolean(Convert.ToString(Left)), Convert.ToBoolean(Right));
		case 347:
		case 348:
		case 349:
		case 350:
		case 351:
		case 352:
		case 353:
		case 354:
		case 355:
		case 356:
		case 357:
			return AndInt64(Conversions.ToLong(Convert.ToString(Left)), Convert.ToInt64(Right));
		case 360:
			return AndInt64(Conversions.ToLong(Convert.ToString(Left)), Conversions.ToLong(Convert.ToString(Right)));
		default:
			if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
			{
				return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.And, Left, Right);
			}
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.And, Left, Right);
		}
	}

	private static object AndBoolean(bool left, bool right)
	{
		return left & right;
	}

	private static object AndSByte(sbyte left, sbyte right, Type enumType = null)
	{
		sbyte b = (sbyte)(left & right);
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, b);
		}
		return b;
	}

	private static object AndByte(byte left, byte right, Type enumType = null)
	{
		byte b = (byte)(left & right);
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, b);
		}
		return b;
	}

	private static object AndInt16(short left, short right, Type enumType = null)
	{
		short num = (short)(left & right);
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object AndUInt16(ushort left, ushort right, Type enumType = null)
	{
		ushort num = (ushort)(left & right);
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object AndInt32(int left, int right, Type enumType = null)
	{
		int num = left & right;
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object AndUInt32(uint left, uint right, Type enumType = null)
	{
		uint num = left & right;
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object AndInt64(long left, long right, Type enumType = null)
	{
		long num = left & right;
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object AndUInt64(ulong left, ulong right, Type enumType = null)
	{
		ulong num = left & right;
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object OrObject(object Left, object Right)
	{
		TypeCode typeCode = GetTypeCode(Left);
		TypeCode typeCode2 = GetTypeCode(Right);
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 0:
			return Boxed_ZeroInteger;
		case 3:
			return OrBoolean(left: false, Convert.ToBoolean(Right));
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
			return Right;
		case 13:
		case 14:
		case 15:
			return OrInt64(0L, Convert.ToInt64(Right));
		case 18:
			return OrInt64(0L, Conversions.ToLong(Convert.ToString(Right)));
		case 57:
			return OrBoolean(Convert.ToBoolean(Left), right: false);
		case 60:
			return OrBoolean(Convert.ToBoolean(Left), Convert.ToBoolean(Right));
		case 62:
			return OrSByte(ToVBBool(Left), Convert.ToSByte(Right));
		case 63:
		case 64:
			return OrInt16(ToVBBool(Left), Convert.ToInt16(Right));
		case 65:
		case 66:
			return OrInt32(ToVBBool(Left), Convert.ToInt32(Right));
		case 67:
		case 68:
		case 69:
		case 70:
		case 71:
		case 72:
			return OrInt64(ToVBBool(Left), Convert.ToInt64(Right));
		case 75:
			return OrBoolean(Convert.ToBoolean(Left), Conversions.ToBoolean(Convert.ToString(Right)));
		case 95:
		case 114:
		case 133:
		case 152:
		case 171:
		case 190:
		case 209:
		case 228:
			return Left;
		case 98:
			return OrSByte(Convert.ToSByte(Left), ToVBBool(Right));
		case 100:
			return OrSByte(Convert.ToSByte(Left), Convert.ToSByte(Right), GetEnumResult(Left, Right));
		case 101:
		case 102:
		case 119:
		case 121:
		case 138:
		case 139:
			return OrInt16(Convert.ToInt16(Left), Convert.ToInt16(Right));
		case 103:
		case 104:
		case 123:
		case 141:
		case 142:
		case 157:
		case 159:
		case 161:
		case 176:
		case 177:
		case 178:
		case 179:
			return OrInt32(Convert.ToInt32(Left), Convert.ToInt32(Right));
		case 105:
		case 106:
		case 107:
		case 108:
		case 109:
		case 110:
		case 125:
		case 127:
		case 128:
		case 129:
		case 143:
		case 144:
		case 145:
		case 146:
		case 147:
		case 148:
		case 163:
		case 165:
		case 166:
		case 167:
		case 181:
		case 182:
		case 183:
		case 184:
		case 185:
		case 186:
		case 195:
		case 197:
		case 199:
		case 201:
		case 203:
		case 204:
		case 205:
		case 214:
		case 215:
		case 216:
		case 217:
		case 218:
		case 219:
		case 221:
		case 222:
		case 223:
		case 224:
		case 233:
		case 235:
		case 237:
		case 239:
		case 241:
		case 242:
		case 243:
		case 252:
		case 253:
		case 254:
		case 255:
		case 256:
		case 257:
		case 258:
		case 259:
		case 260:
		case 261:
		case 262:
		case 271:
		case 272:
		case 273:
		case 274:
		case 275:
		case 276:
		case 277:
		case 278:
		case 279:
		case 280:
		case 281:
		case 290:
		case 291:
		case 292:
		case 293:
		case 294:
		case 295:
		case 296:
		case 297:
		case 298:
		case 299:
		case 300:
			return OrInt64(Convert.ToInt64(Left), Convert.ToInt64(Right));
		case 113:
		case 132:
		case 151:
		case 170:
		case 189:
		case 208:
		case 227:
		case 246:
		case 265:
		case 284:
		case 303:
			return OrInt64(Convert.ToInt64(Left), Conversions.ToLong(Convert.ToString(Right)));
		case 117:
		case 136:
			return OrInt16(Convert.ToInt16(Left), ToVBBool(Right));
		case 120:
			return OrByte(Convert.ToByte(Left), Convert.ToByte(Right), GetEnumResult(Left, Right));
		case 122:
		case 158:
			return OrUInt16(Convert.ToUInt16(Left), Convert.ToUInt16(Right));
		case 124:
		case 162:
		case 196:
		case 198:
			return OrUInt32(Convert.ToUInt32(Left), Convert.ToUInt32(Right));
		case 126:
		case 164:
		case 202:
		case 234:
		case 236:
		case 238:
			return OrUInt64(Convert.ToUInt64(Left), Convert.ToUInt64(Right));
		case 140:
			return OrInt16(Convert.ToInt16(Left), Convert.ToInt16(Right), GetEnumResult(Left, Right));
		case 155:
		case 174:
			return OrInt32(Convert.ToInt32(Left), ToVBBool(Right));
		case 160:
			return OrUInt16(Convert.ToUInt16(Left), Convert.ToUInt16(Right), GetEnumResult(Left, Right));
		case 180:
			return OrInt32(Convert.ToInt32(Left), Convert.ToInt32(Right), GetEnumResult(Left, Right));
		case 193:
		case 212:
		case 231:
		case 250:
		case 269:
		case 288:
			return OrInt64(Convert.ToInt64(Left), ToVBBool(Right));
		case 200:
			return OrUInt32(Convert.ToUInt32(Left), Convert.ToUInt32(Right), GetEnumResult(Left, Right));
		case 220:
			return OrInt64(Convert.ToInt64(Left), Convert.ToInt64(Right), GetEnumResult(Left, Right));
		case 240:
			return OrUInt64(Convert.ToUInt64(Left), Convert.ToUInt64(Right), GetEnumResult(Left, Right));
		case 247:
		case 266:
		case 285:
			return OrInt64(Convert.ToInt64(Left), 0L);
		case 342:
			return OrInt64(Conversions.ToLong(Convert.ToString(Left)), 0L);
		case 345:
			return OrBoolean(Conversions.ToBoolean(Convert.ToString(Left)), Convert.ToBoolean(Right));
		case 347:
		case 348:
		case 349:
		case 350:
		case 351:
		case 352:
		case 353:
		case 354:
		case 355:
		case 356:
		case 357:
			return OrInt64(Conversions.ToLong(Convert.ToString(Left)), Convert.ToInt64(Right));
		case 360:
			return OrInt64(Conversions.ToLong(Convert.ToString(Left)), Conversions.ToLong(Convert.ToString(Right)));
		default:
			if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
			{
				return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Or, Left, Right);
			}
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Or, Left, Right);
		}
	}

	private static object OrBoolean(bool left, bool right)
	{
		return left | right;
	}

	private static object OrSByte(sbyte left, sbyte right, Type enumType = null)
	{
		sbyte b = (sbyte)(left | right);
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, b);
		}
		return b;
	}

	private static object OrByte(byte left, byte right, Type enumType = null)
	{
		byte b = (byte)(left | right);
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, b);
		}
		return b;
	}

	private static object OrInt16(short left, short right, Type enumType = null)
	{
		short num = (short)(left | right);
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object OrUInt16(ushort left, ushort right, Type enumType = null)
	{
		ushort num = (ushort)(left | right);
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object OrInt32(int left, int right, Type enumType = null)
	{
		int num = left | right;
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object OrUInt32(uint left, uint right, Type enumType = null)
	{
		uint num = left | right;
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object OrInt64(long left, long right, Type enumType = null)
	{
		long num = left | right;
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object OrUInt64(ulong left, ulong right, Type enumType = null)
	{
		ulong num = left | right;
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object XorObject(object Left, object Right)
	{
		TypeCode typeCode = GetTypeCode(Left);
		TypeCode typeCode2 = GetTypeCode(Right);
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 0:
			return Boxed_ZeroInteger;
		case 3:
			return XorBoolean(left: false, Convert.ToBoolean(Right));
		case 5:
			return XorSByte(0, Convert.ToSByte(Right), GetEnumResult(Left, Right));
		case 6:
			return XorByte(0, Convert.ToByte(Right), GetEnumResult(Left, Right));
		case 7:
			return XorInt16(0, Convert.ToInt16(Right), GetEnumResult(Left, Right));
		case 8:
			return XorUInt16(0, Convert.ToUInt16(Right), GetEnumResult(Left, Right));
		case 9:
			return XorInt32(0, Convert.ToInt32(Right), GetEnumResult(Left, Right));
		case 10:
			return XorUInt32(0u, Convert.ToUInt32(Right), GetEnumResult(Left, Right));
		case 11:
			return XorInt64(0L, Convert.ToInt64(Right), GetEnumResult(Left, Right));
		case 12:
			return XorUInt64(0uL, Convert.ToUInt64(Right), GetEnumResult(Left, Right));
		case 13:
		case 14:
		case 15:
			return XorInt64(0L, Convert.ToInt64(Right));
		case 18:
			return XorInt64(0L, Conversions.ToLong(Convert.ToString(Right)));
		case 57:
			return XorBoolean(Convert.ToBoolean(Left), right: false);
		case 60:
			return XorBoolean(Convert.ToBoolean(Left), Convert.ToBoolean(Right));
		case 62:
			return XorSByte(ToVBBool(Left), Convert.ToSByte(Right));
		case 63:
		case 64:
			return XorInt16(ToVBBool(Left), Convert.ToInt16(Right));
		case 65:
		case 66:
			return XorInt32(ToVBBool(Left), Convert.ToInt32(Right));
		case 67:
		case 68:
		case 69:
		case 70:
		case 71:
		case 72:
			return XorInt64(ToVBBool(Left), Convert.ToInt64(Right));
		case 75:
			return XorBoolean(Convert.ToBoolean(Left), Conversions.ToBoolean(Convert.ToString(Right)));
		case 95:
			return XorSByte(Convert.ToSByte(Left), 0, GetEnumResult(Left, Right));
		case 98:
			return XorSByte(Convert.ToSByte(Left), ToVBBool(Right));
		case 100:
			return XorSByte(Convert.ToSByte(Left), Convert.ToSByte(Right), GetEnumResult(Left, Right));
		case 101:
		case 102:
		case 119:
		case 121:
		case 138:
		case 139:
			return XorInt16(Convert.ToInt16(Left), Convert.ToInt16(Right));
		case 103:
		case 104:
		case 123:
		case 141:
		case 142:
		case 157:
		case 159:
		case 161:
		case 176:
		case 177:
		case 178:
		case 179:
			return XorInt32(Convert.ToInt32(Left), Convert.ToInt32(Right));
		case 105:
		case 106:
		case 107:
		case 108:
		case 109:
		case 110:
		case 125:
		case 127:
		case 128:
		case 129:
		case 143:
		case 144:
		case 145:
		case 146:
		case 147:
		case 148:
		case 163:
		case 165:
		case 166:
		case 167:
		case 181:
		case 182:
		case 183:
		case 184:
		case 185:
		case 186:
		case 195:
		case 197:
		case 199:
		case 201:
		case 203:
		case 204:
		case 205:
		case 214:
		case 215:
		case 216:
		case 217:
		case 218:
		case 219:
		case 221:
		case 222:
		case 223:
		case 224:
		case 233:
		case 235:
		case 237:
		case 239:
		case 241:
		case 242:
		case 243:
		case 252:
		case 253:
		case 254:
		case 255:
		case 256:
		case 257:
		case 258:
		case 259:
		case 260:
		case 261:
		case 262:
		case 271:
		case 272:
		case 273:
		case 274:
		case 275:
		case 276:
		case 277:
		case 278:
		case 279:
		case 280:
		case 281:
		case 290:
		case 291:
		case 292:
		case 293:
		case 294:
		case 295:
		case 296:
		case 297:
		case 298:
		case 299:
		case 300:
			return XorInt64(Convert.ToInt64(Left), Convert.ToInt64(Right));
		case 113:
		case 132:
		case 151:
		case 170:
		case 189:
		case 208:
		case 227:
		case 246:
		case 265:
		case 284:
		case 303:
			return XorInt64(Convert.ToInt64(Left), Conversions.ToLong(Convert.ToString(Right)));
		case 114:
			return XorByte(Convert.ToByte(Left), 0, GetEnumResult(Left, Right));
		case 117:
		case 136:
			return XorInt16(Convert.ToInt16(Left), ToVBBool(Right));
		case 120:
			return XorByte(Convert.ToByte(Left), Convert.ToByte(Right), GetEnumResult(Left, Right));
		case 122:
		case 158:
			return XorUInt16(Convert.ToUInt16(Left), Convert.ToUInt16(Right));
		case 124:
		case 162:
		case 196:
		case 198:
			return XorUInt32(Convert.ToUInt32(Left), Convert.ToUInt32(Right));
		case 126:
		case 164:
		case 202:
		case 234:
		case 236:
		case 238:
			return XorUInt64(Convert.ToUInt64(Left), Convert.ToUInt64(Right));
		case 133:
			return XorInt16(Convert.ToInt16(Left), 0, GetEnumResult(Left, Right));
		case 140:
			return XorInt16(Convert.ToInt16(Left), Convert.ToInt16(Right), GetEnumResult(Left, Right));
		case 152:
			return XorUInt16(Convert.ToUInt16(Left), 0, GetEnumResult(Left, Right));
		case 155:
		case 174:
			return XorInt32(Convert.ToInt32(Left), ToVBBool(Right));
		case 160:
			return XorUInt16(Convert.ToUInt16(Left), Convert.ToUInt16(Right), GetEnumResult(Left, Right));
		case 171:
			return XorInt32(Convert.ToInt32(Left), 0, GetEnumResult(Left, Right));
		case 180:
			return XorInt32(Convert.ToInt32(Left), Convert.ToInt32(Right), GetEnumResult(Left, Right));
		case 190:
			return XorUInt32(Convert.ToUInt32(Left), 0u, GetEnumResult(Left, Right));
		case 193:
		case 212:
		case 231:
		case 250:
		case 269:
		case 288:
			return XorInt64(Convert.ToInt64(Left), ToVBBool(Right));
		case 200:
			return XorUInt32(Convert.ToUInt32(Left), Convert.ToUInt32(Right), GetEnumResult(Left, Right));
		case 209:
			return XorInt64(Convert.ToInt64(Left), 0L, GetEnumResult(Left, Right));
		case 220:
			return XorInt64(Convert.ToInt64(Left), Convert.ToInt64(Right), GetEnumResult(Left, Right));
		case 228:
			return XorUInt64(Convert.ToUInt64(Left), 0uL, GetEnumResult(Left, Right));
		case 240:
			return XorUInt64(Convert.ToUInt64(Left), Convert.ToUInt64(Right), GetEnumResult(Left, Right));
		case 247:
		case 266:
		case 285:
			return XorInt64(Convert.ToInt64(Left), 0L);
		case 342:
			return XorInt64(Conversions.ToLong(Convert.ToString(Left)), 0L);
		case 345:
			return XorBoolean(Conversions.ToBoolean(Convert.ToString(Left)), Convert.ToBoolean(Right));
		case 347:
		case 348:
		case 349:
		case 350:
		case 351:
		case 352:
		case 353:
		case 354:
		case 355:
		case 356:
		case 357:
			return XorInt64(Conversions.ToLong(Convert.ToString(Left)), Convert.ToInt64(Right));
		case 360:
			return XorInt64(Conversions.ToLong(Convert.ToString(Left)), Conversions.ToLong(Convert.ToString(Right)));
		default:
			if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
			{
				return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Xor, Left, Right);
			}
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Xor, Left, Right);
		}
	}

	private static object XorBoolean(bool left, bool right)
	{
		return left ^ right;
	}

	private static object XorSByte(sbyte left, sbyte right, Type enumType = null)
	{
		sbyte b = (sbyte)(left ^ right);
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, b);
		}
		return b;
	}

	private static object XorByte(byte left, byte right, Type enumType = null)
	{
		byte b = (byte)(left ^ right);
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, b);
		}
		return b;
	}

	private static object XorInt16(short left, short right, Type enumType = null)
	{
		short num = (short)(left ^ right);
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object XorUInt16(ushort left, ushort right, Type enumType = null)
	{
		ushort num = (ushort)(left ^ right);
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object XorInt32(int left, int right, Type enumType = null)
	{
		int num = left ^ right;
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object XorUInt32(uint left, uint right, Type enumType = null)
	{
		uint num = left ^ right;
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object XorInt64(long left, long right, Type enumType = null)
	{
		long num = left ^ right;
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	private static object XorUInt64(ulong left, ulong right, Type enumType = null)
	{
		ulong num = left ^ right;
		if ((object)enumType != null)
		{
			return Enum.ToObject(enumType, num);
		}
		return num;
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object AddObject(object Left, object Right)
	{
		TypeCode typeCode = GetTypeCode(Left);
		TypeCode typeCode2 = GetTypeCode(Right);
		if (typeCode == TypeCode.Object && Left is char[] value && (typeCode2 == TypeCode.String || typeCode2 == TypeCode.Empty || (typeCode2 == TypeCode.Object && Right is char[])))
		{
			Left = new string(value);
			typeCode = TypeCode.String;
		}
		if (typeCode2 == TypeCode.Object && Right is char[] value2 && (typeCode == TypeCode.String || typeCode == TypeCode.Empty))
		{
			Right = new string(value2);
			typeCode2 = TypeCode.String;
		}
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 0:
			return Boxed_ZeroInteger;
		case 3:
			return AddInt16(0, ToVBBool(Right));
		case 5:
			return Convert.ToSByte(Right);
		case 6:
			return Convert.ToByte(Right);
		case 7:
			return Convert.ToInt16(Right);
		case 8:
			return Convert.ToUInt16(Right);
		case 9:
			return Convert.ToInt32(Right);
		case 10:
			return Convert.ToUInt32(Right);
		case 11:
			return Convert.ToInt64(Right);
		case 12:
			return Convert.ToUInt64(Right);
		case 13:
		case 14:
		case 15:
		case 18:
		case 56:
			return Right;
		case 16:
			return AddString(Conversions.ToString(DateTime.MinValue), Conversions.ToString(Convert.ToDateTime(Right)));
		case 4:
			return AddString("\0", Convert.ToString(Right));
		case 57:
			return AddInt16(ToVBBool(Left), 0);
		case 60:
			return AddInt16(ToVBBool(Left), ToVBBool(Right));
		case 62:
			return AddSByte(ToVBBool(Left), Convert.ToSByte(Right));
		case 63:
		case 64:
			return AddInt16(ToVBBool(Left), Convert.ToInt16(Right));
		case 65:
		case 66:
			return AddInt32(ToVBBool(Left), Convert.ToInt32(Right));
		case 67:
		case 68:
			return AddInt64(ToVBBool(Left), Convert.ToInt64(Right));
		case 69:
		case 72:
			return AddDecimal(ToVBBool(Left), Convert.ToDecimal(Right));
		case 70:
			return AddSingle(ToVBBool(Left), Convert.ToSingle(Right));
		case 71:
			return AddDouble(ToVBBool(Left), Convert.ToDouble(Right));
		case 75:
			return AddDouble(ToVBBool(Left), Conversions.ToDouble(Convert.ToString(Right)));
		case 95:
			return Convert.ToSByte(Left);
		case 98:
			return AddSByte(Convert.ToSByte(Left), ToVBBool(Right));
		case 100:
			return AddSByte(Convert.ToSByte(Left), Convert.ToSByte(Right));
		case 101:
		case 102:
		case 119:
		case 121:
		case 138:
		case 139:
		case 140:
			return AddInt16(Convert.ToInt16(Left), Convert.ToInt16(Right));
		case 103:
		case 104:
		case 123:
		case 141:
		case 142:
		case 157:
		case 159:
		case 161:
		case 176:
		case 177:
		case 178:
		case 179:
		case 180:
			return AddInt32(Convert.ToInt32(Left), Convert.ToInt32(Right));
		case 105:
		case 106:
		case 125:
		case 143:
		case 144:
		case 163:
		case 181:
		case 182:
		case 195:
		case 197:
		case 199:
		case 201:
		case 214:
		case 215:
		case 216:
		case 217:
		case 218:
		case 219:
		case 220:
			return AddInt64(Convert.ToInt64(Left), Convert.ToInt64(Right));
		case 107:
		case 110:
		case 129:
		case 145:
		case 148:
		case 167:
		case 183:
		case 186:
		case 205:
		case 221:
		case 224:
		case 233:
		case 235:
		case 237:
		case 239:
		case 243:
		case 290:
		case 291:
		case 292:
		case 293:
		case 294:
		case 295:
		case 296:
		case 297:
		case 300:
			return AddDecimal(Left, Right);
		case 108:
		case 127:
		case 146:
		case 165:
		case 184:
		case 203:
		case 222:
		case 241:
		case 252:
		case 253:
		case 254:
		case 255:
		case 256:
		case 257:
		case 258:
		case 259:
		case 260:
		case 262:
		case 298:
			return AddSingle(Convert.ToSingle(Left), Convert.ToSingle(Right));
		case 109:
		case 128:
		case 147:
		case 166:
		case 185:
		case 204:
		case 223:
		case 242:
		case 261:
		case 271:
		case 272:
		case 273:
		case 274:
		case 275:
		case 276:
		case 277:
		case 278:
		case 279:
		case 280:
		case 281:
		case 299:
			return AddDouble(Convert.ToDouble(Left), Convert.ToDouble(Right));
		case 113:
		case 132:
		case 151:
		case 170:
		case 189:
		case 208:
		case 227:
		case 246:
		case 265:
		case 284:
		case 303:
			return AddDouble(Convert.ToDouble(Left), Conversions.ToDouble(Convert.ToString(Right)));
		case 114:
			return Convert.ToByte(Left);
		case 117:
		case 136:
			return AddInt16(Convert.ToInt16(Left), ToVBBool(Right));
		case 120:
			return AddByte(Convert.ToByte(Left), Convert.ToByte(Right));
		case 122:
		case 158:
		case 160:
			return AddUInt16(Convert.ToUInt16(Left), Convert.ToUInt16(Right));
		case 124:
		case 162:
		case 196:
		case 198:
		case 200:
			return AddUInt32(Convert.ToUInt32(Left), Convert.ToUInt32(Right));
		case 126:
		case 164:
		case 202:
		case 234:
		case 236:
		case 238:
		case 240:
			return AddUInt64(Convert.ToUInt64(Left), Convert.ToUInt64(Right));
		case 133:
			return Convert.ToInt16(Left);
		case 152:
			return Convert.ToUInt16(Left);
		case 155:
		case 174:
			return AddInt32(Convert.ToInt32(Left), ToVBBool(Right));
		case 171:
			return Convert.ToInt32(Left);
		case 190:
			return Convert.ToUInt32(Left);
		case 193:
		case 212:
			return AddInt64(Convert.ToInt64(Left), ToVBBool(Right));
		case 209:
			return Convert.ToInt64(Left);
		case 228:
			return Convert.ToUInt64(Left);
		case 231:
		case 288:
			return AddDecimal(Left, ToVBBool(Right));
		case 247:
		case 266:
		case 285:
		case 342:
		case 344:
			return Left;
		case 250:
			return AddSingle(Convert.ToSingle(Left), ToVBBool(Right));
		case 269:
			return AddDouble(Convert.ToDouble(Left), ToVBBool(Right));
		case 304:
			return AddString(Conversions.ToString(DateTime.MinValue), Conversions.ToString(Conversions.ToDate(Left)));
		case 320:
			return AddString(Conversions.ToString(Convert.ToDateTime(Left)), Conversions.ToString(Convert.ToDateTime(Right)));
		case 322:
			return AddString(Conversions.ToString(Convert.ToDateTime(Left)), Convert.ToString(Right));
		case 76:
			return AddString(Convert.ToString(Left), "\0");
		case 80:
		case 94:
		case 346:
			return AddString(Convert.ToString(Left), Convert.ToString(Right));
		case 345:
			return AddDouble(Conversions.ToDouble(Convert.ToString(Left)), ToVBBool(Right));
		case 347:
		case 348:
		case 349:
		case 350:
		case 351:
		case 352:
		case 353:
		case 354:
		case 355:
		case 356:
		case 357:
			return AddDouble(Conversions.ToDouble(Convert.ToString(Left)), Convert.ToDouble(Right));
		case 358:
			return AddString(Convert.ToString(Left), Conversions.ToString(Convert.ToDateTime(Right)));
		case 360:
			return AddString(Convert.ToString(Left), Convert.ToString(Right));
		default:
			if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
			{
				return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Plus, Left, Right);
			}
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Plus, Left, Right);
		}
	}

	private static object AddByte(byte left, byte right)
	{
		checked
		{
			short num = (short)unchecked(left + right);
			if (num > 255)
			{
				return num;
			}
			return (byte)num;
		}
	}

	private static object AddSByte(sbyte left, sbyte right)
	{
		checked
		{
			short num = (short)unchecked(left + right);
			if (num > 127 || num < -128)
			{
				return num;
			}
			return (sbyte)num;
		}
	}

	private static object AddInt16(short left, short right)
	{
		checked
		{
			int num = left + right;
			if (num > 32767 || num < -32768)
			{
				return num;
			}
			return (short)num;
		}
	}

	private static object AddUInt16(ushort left, ushort right)
	{
		checked
		{
			int num = left + right;
			if (num > 65535)
			{
				return num;
			}
			return (ushort)num;
		}
	}

	private static object AddInt32(int left, int right)
	{
		checked
		{
			long num = unchecked((long)left) + unchecked((long)right);
			if (num > int.MaxValue || num < int.MinValue)
			{
				return num;
			}
			return (int)num;
		}
	}

	private static object AddUInt32(uint left, uint right)
	{
		checked
		{
			long num = unchecked((long)left) + unchecked((long)right);
			if (num > uint.MaxValue)
			{
				return num;
			}
			return (uint)num;
		}
	}

	private static object AddInt64(long left, long right)
	{
		try
		{
			return checked(left + right);
		}
		catch (OverflowException)
		{
			return decimal.Add(new decimal(left), new decimal(right));
		}
	}

	private static object AddUInt64(ulong left, ulong right)
	{
		try
		{
			return checked(left + right);
		}
		catch (OverflowException)
		{
			return decimal.Add(new decimal(left), new decimal(right));
		}
	}

	private static object AddDecimal(object left, object right)
	{
		decimal num = Convert.ToDecimal(left);
		decimal num2 = Convert.ToDecimal(right);
		try
		{
			return decimal.Add(num, num2);
		}
		catch (OverflowException)
		{
			return Convert.ToDouble(num) + Convert.ToDouble(num2);
		}
	}

	private static object AddSingle(float left, float right)
	{
		double num = (double)left + (double)right;
		if (num <= 3.4028234663852886E+38 && num >= -3.4028234663852886E+38)
		{
			return (float)num;
		}
		if (double.IsInfinity(num) && (float.IsInfinity(left) || float.IsInfinity(right)))
		{
			return (float)num;
		}
		return num;
	}

	private static object AddDouble(double left, double right)
	{
		return left + right;
	}

	private static object AddString(string left, string right)
	{
		return left + right;
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object SubtractObject(object Left, object Right)
	{
		TypeCode typeCode = GetTypeCode(Left);
		TypeCode typeCode2 = GetTypeCode(Right);
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 0:
			return Boxed_ZeroInteger;
		case 3:
			return SubtractInt16(0, ToVBBool(Right));
		case 5:
			return SubtractSByte(0, Convert.ToSByte(Right));
		case 6:
			return SubtractByte(0, Convert.ToByte(Right));
		case 7:
			return SubtractInt16(0, Convert.ToInt16(Right));
		case 8:
			return SubtractUInt16(0, Convert.ToUInt16(Right));
		case 9:
			return SubtractInt32(0, Convert.ToInt32(Right));
		case 10:
			return SubtractUInt32(0u, Convert.ToUInt32(Right));
		case 11:
			return SubtractInt64(0L, Convert.ToInt64(Right));
		case 12:
			return SubtractUInt64(0uL, Convert.ToUInt64(Right));
		case 15:
			return SubtractDecimal(0m, Right);
		case 13:
			return SubtractSingle(0f, Convert.ToSingle(Right));
		case 14:
			return SubtractDouble(0.0, Convert.ToDouble(Right));
		case 18:
			return SubtractDouble(0.0, Conversions.ToDouble(Convert.ToString(Right)));
		case 57:
			return SubtractInt16(ToVBBool(Left), 0);
		case 60:
			return SubtractInt16(ToVBBool(Left), ToVBBool(Right));
		case 62:
			return SubtractSByte(ToVBBool(Left), Convert.ToSByte(Right));
		case 63:
		case 64:
			return SubtractInt16(ToVBBool(Left), Convert.ToInt16(Right));
		case 65:
		case 66:
			return SubtractInt32(ToVBBool(Left), Convert.ToInt32(Right));
		case 67:
		case 68:
			return SubtractInt64(ToVBBool(Left), Convert.ToInt64(Right));
		case 69:
		case 72:
			return SubtractDecimal(ToVBBool(Left), Convert.ToDecimal(Right));
		case 70:
			return SubtractSingle(ToVBBool(Left), Convert.ToSingle(Right));
		case 71:
			return SubtractDouble(ToVBBool(Left), Convert.ToDouble(Right));
		case 75:
			return SubtractDouble(ToVBBool(Left), Conversions.ToDouble(Convert.ToString(Right)));
		case 95:
			return Convert.ToSByte(Left);
		case 98:
			return SubtractSByte(Convert.ToSByte(Left), ToVBBool(Right));
		case 100:
			return SubtractSByte(Convert.ToSByte(Left), Convert.ToSByte(Right));
		case 101:
		case 102:
		case 119:
		case 121:
		case 138:
		case 139:
		case 140:
			return SubtractInt16(Convert.ToInt16(Left), Convert.ToInt16(Right));
		case 103:
		case 104:
		case 123:
		case 141:
		case 142:
		case 157:
		case 159:
		case 161:
		case 176:
		case 177:
		case 178:
		case 179:
		case 180:
			return SubtractInt32(Convert.ToInt32(Left), Convert.ToInt32(Right));
		case 105:
		case 106:
		case 125:
		case 143:
		case 144:
		case 163:
		case 181:
		case 182:
		case 195:
		case 197:
		case 199:
		case 201:
		case 214:
		case 215:
		case 216:
		case 217:
		case 218:
		case 219:
		case 220:
			return SubtractInt64(Convert.ToInt64(Left), Convert.ToInt64(Right));
		case 107:
		case 110:
		case 129:
		case 145:
		case 148:
		case 167:
		case 183:
		case 186:
		case 205:
		case 221:
		case 224:
		case 233:
		case 235:
		case 237:
		case 239:
		case 243:
		case 290:
		case 291:
		case 292:
		case 293:
		case 294:
		case 295:
		case 296:
		case 297:
		case 300:
			return SubtractDecimal(Left, Right);
		case 108:
		case 127:
		case 146:
		case 165:
		case 184:
		case 203:
		case 222:
		case 241:
		case 252:
		case 253:
		case 254:
		case 255:
		case 256:
		case 257:
		case 258:
		case 259:
		case 260:
		case 262:
		case 298:
			return SubtractSingle(Convert.ToSingle(Left), Convert.ToSingle(Right));
		case 109:
		case 128:
		case 147:
		case 166:
		case 185:
		case 204:
		case 223:
		case 242:
		case 261:
		case 271:
		case 272:
		case 273:
		case 274:
		case 275:
		case 276:
		case 277:
		case 278:
		case 279:
		case 280:
		case 281:
		case 299:
			return SubtractDouble(Convert.ToDouble(Left), Convert.ToDouble(Right));
		case 113:
		case 132:
		case 151:
		case 170:
		case 189:
		case 208:
		case 227:
		case 246:
		case 265:
		case 284:
		case 303:
			return SubtractDouble(Convert.ToDouble(Left), Conversions.ToDouble(Convert.ToString(Right)));
		case 114:
			return Convert.ToByte(Left);
		case 117:
		case 136:
			return SubtractInt16(Convert.ToInt16(Left), ToVBBool(Right));
		case 120:
			return SubtractByte(Convert.ToByte(Left), Convert.ToByte(Right));
		case 122:
		case 158:
		case 160:
			return SubtractUInt16(Convert.ToUInt16(Left), Convert.ToUInt16(Right));
		case 124:
		case 162:
		case 196:
		case 198:
		case 200:
			return SubtractUInt32(Convert.ToUInt32(Left), Convert.ToUInt32(Right));
		case 126:
		case 164:
		case 202:
		case 234:
		case 236:
		case 238:
		case 240:
			return SubtractUInt64(Convert.ToUInt64(Left), Convert.ToUInt64(Right));
		case 133:
			return Convert.ToInt16(Left);
		case 152:
			return Convert.ToUInt16(Left);
		case 155:
		case 174:
			return SubtractInt32(Convert.ToInt32(Left), ToVBBool(Right));
		case 171:
			return Convert.ToInt32(Left);
		case 190:
			return Convert.ToUInt32(Left);
		case 193:
		case 212:
			return SubtractInt64(Convert.ToInt64(Left), ToVBBool(Right));
		case 209:
			return Convert.ToInt64(Left);
		case 228:
			return Convert.ToUInt64(Left);
		case 231:
		case 288:
			return SubtractDecimal(Left, ToVBBool(Right));
		case 247:
		case 266:
		case 285:
			return Left;
		case 250:
			return SubtractSingle(Convert.ToSingle(Left), ToVBBool(Right));
		case 269:
			return SubtractDouble(Convert.ToDouble(Left), ToVBBool(Right));
		case 342:
			return Conversions.ToDouble(Convert.ToString(Left));
		case 345:
			return SubtractDouble(Conversions.ToDouble(Convert.ToString(Left)), ToVBBool(Right));
		case 347:
		case 348:
		case 349:
		case 350:
		case 351:
		case 352:
		case 353:
		case 354:
		case 355:
		case 356:
		case 357:
			return SubtractDouble(Conversions.ToDouble(Convert.ToString(Left)), Convert.ToDouble(Right));
		case 360:
			return SubtractDouble(Conversions.ToDouble(Convert.ToString(Left)), Conversions.ToDouble(Convert.ToString(Right)));
		default:
			if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object || (typeCode == TypeCode.DateTime && typeCode2 == TypeCode.DateTime) || (typeCode == TypeCode.DateTime && typeCode2 == TypeCode.Empty) || (typeCode == TypeCode.Empty && typeCode2 == TypeCode.DateTime))
			{
				return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Minus, Left, Right);
			}
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Minus, Left, Right);
		}
	}

	private static object SubtractByte(byte left, byte right)
	{
		checked
		{
			short num = (short)unchecked(left - right);
			if (num < 0)
			{
				return num;
			}
			return (byte)num;
		}
	}

	private static object SubtractSByte(sbyte left, sbyte right)
	{
		checked
		{
			short num = (short)unchecked(left - right);
			if (num < -128 || num > 127)
			{
				return num;
			}
			return (sbyte)num;
		}
	}

	private static object SubtractInt16(short left, short right)
	{
		checked
		{
			int num = left - right;
			if (num < -32768 || num > 32767)
			{
				return num;
			}
			return (short)num;
		}
	}

	private static object SubtractUInt16(ushort left, ushort right)
	{
		checked
		{
			int num = left - right;
			if (num < 0)
			{
				return num;
			}
			return (ushort)num;
		}
	}

	private static object SubtractInt32(int left, int right)
	{
		checked
		{
			long num = unchecked((long)left) - unchecked((long)right);
			if (num < int.MinValue || num > int.MaxValue)
			{
				return num;
			}
			return (int)num;
		}
	}

	private static object SubtractUInt32(uint left, uint right)
	{
		checked
		{
			long num = unchecked((long)left) - unchecked((long)right);
			if (num < 0)
			{
				return num;
			}
			return (uint)num;
		}
	}

	private static object SubtractInt64(long left, long right)
	{
		try
		{
			return checked(left - right);
		}
		catch (OverflowException)
		{
			return decimal.Subtract(new decimal(left), new decimal(right));
		}
	}

	private static object SubtractUInt64(ulong left, ulong right)
	{
		try
		{
			return checked(left - right);
		}
		catch (OverflowException)
		{
			return decimal.Subtract(new decimal(left), new decimal(right));
		}
	}

	private static object SubtractDecimal(object left, object right)
	{
		decimal num = Convert.ToDecimal(left);
		decimal num2 = Convert.ToDecimal(right);
		try
		{
			return decimal.Subtract(num, num2);
		}
		catch (OverflowException)
		{
			return Convert.ToDouble(num) - Convert.ToDouble(num2);
		}
	}

	private static object SubtractSingle(float left, float right)
	{
		double num = (double)left - (double)right;
		if (num <= 3.4028234663852886E+38 && num >= -3.4028234663852886E+38)
		{
			return (float)num;
		}
		if (double.IsInfinity(num) && (float.IsInfinity(left) || float.IsInfinity(right)))
		{
			return (float)num;
		}
		return num;
	}

	private static object SubtractDouble(double left, double right)
	{
		return left - right;
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object MultiplyObject(object Left, object Right)
	{
		TypeCode typeCode = GetTypeCode(Left);
		TypeCode typeCode2 = GetTypeCode(Right);
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 0:
		case 9:
		case 171:
			return Boxed_ZeroInteger;
		case 3:
		case 7:
		case 57:
		case 133:
			return Boxed_ZeroShort;
		case 5:
		case 95:
			return Boxed_ZeroSByte;
		case 6:
		case 114:
			return Boxed_ZeroByte;
		case 8:
		case 152:
			return Boxed_ZeroUShort;
		case 10:
		case 190:
			return Boxed_ZeroUInteger;
		case 11:
		case 209:
			return Boxed_ZeroLong;
		case 12:
		case 228:
			return Boxed_ZeroULong;
		case 15:
		case 285:
			return Boxed_ZeroDecimal;
		case 13:
		case 247:
			return Boxed_ZeroSinge;
		case 14:
		case 266:
			return Boxed_ZeroDouble;
		case 18:
			return MultiplyDouble(0.0, Conversions.ToDouble(Convert.ToString(Right)));
		case 60:
			return MultiplyInt16(ToVBBool(Left), ToVBBool(Right));
		case 62:
			return MultiplySByte(ToVBBool(Left), Convert.ToSByte(Right));
		case 63:
		case 64:
			return MultiplyInt16(ToVBBool(Left), Convert.ToInt16(Right));
		case 65:
		case 66:
			return MultiplyInt32(ToVBBool(Left), Convert.ToInt32(Right));
		case 67:
		case 68:
			return MultiplyInt64(ToVBBool(Left), Convert.ToInt64(Right));
		case 69:
		case 72:
			return MultiplyDecimal(ToVBBool(Left), Convert.ToDecimal(Right));
		case 70:
			return MultiplySingle(ToVBBool(Left), Convert.ToSingle(Right));
		case 71:
			return MultiplyDouble(ToVBBool(Left), Convert.ToDouble(Right));
		case 75:
			return MultiplyDouble(ToVBBool(Left), Conversions.ToDouble(Convert.ToString(Right)));
		case 98:
			return MultiplySByte(Convert.ToSByte(Left), ToVBBool(Right));
		case 100:
			return MultiplySByte(Convert.ToSByte(Left), Convert.ToSByte(Right));
		case 101:
		case 102:
		case 119:
		case 121:
		case 138:
		case 139:
		case 140:
			return MultiplyInt16(Convert.ToInt16(Left), Convert.ToInt16(Right));
		case 103:
		case 104:
		case 123:
		case 141:
		case 142:
		case 157:
		case 159:
		case 161:
		case 176:
		case 177:
		case 178:
		case 179:
		case 180:
			return MultiplyInt32(Convert.ToInt32(Left), Convert.ToInt32(Right));
		case 105:
		case 106:
		case 125:
		case 143:
		case 144:
		case 163:
		case 181:
		case 182:
		case 195:
		case 197:
		case 199:
		case 201:
		case 214:
		case 215:
		case 216:
		case 217:
		case 218:
		case 219:
		case 220:
			return MultiplyInt64(Convert.ToInt64(Left), Convert.ToInt64(Right));
		case 107:
		case 110:
		case 129:
		case 145:
		case 148:
		case 167:
		case 183:
		case 186:
		case 205:
		case 221:
		case 224:
		case 233:
		case 235:
		case 237:
		case 239:
		case 243:
		case 290:
		case 291:
		case 292:
		case 293:
		case 294:
		case 295:
		case 296:
		case 297:
		case 300:
			return MultiplyDecimal(Left, Right);
		case 108:
		case 127:
		case 146:
		case 165:
		case 184:
		case 203:
		case 222:
		case 241:
		case 252:
		case 253:
		case 254:
		case 255:
		case 256:
		case 257:
		case 258:
		case 259:
		case 260:
		case 262:
		case 298:
			return MultiplySingle(Convert.ToSingle(Left), Convert.ToSingle(Right));
		case 109:
		case 128:
		case 147:
		case 166:
		case 185:
		case 204:
		case 223:
		case 242:
		case 261:
		case 271:
		case 272:
		case 273:
		case 274:
		case 275:
		case 276:
		case 277:
		case 278:
		case 279:
		case 280:
		case 281:
		case 299:
			return MultiplyDouble(Convert.ToDouble(Left), Convert.ToDouble(Right));
		case 113:
		case 132:
		case 151:
		case 170:
		case 189:
		case 208:
		case 227:
		case 246:
		case 265:
		case 284:
		case 303:
			return MultiplyDouble(Convert.ToDouble(Left), Conversions.ToDouble(Convert.ToString(Right)));
		case 117:
		case 136:
			return MultiplyInt16(Convert.ToInt16(Left), ToVBBool(Right));
		case 120:
			return MultiplyByte(Convert.ToByte(Left), Convert.ToByte(Right));
		case 122:
		case 158:
		case 160:
			return MultiplyUInt16(Convert.ToUInt16(Left), Convert.ToUInt16(Right));
		case 124:
		case 162:
		case 196:
		case 198:
		case 200:
			return MultiplyUInt32(Convert.ToUInt32(Left), Convert.ToUInt32(Right));
		case 126:
		case 164:
		case 202:
		case 234:
		case 236:
		case 238:
		case 240:
			return MultiplyUInt64(Convert.ToUInt64(Left), Convert.ToUInt64(Right));
		case 155:
		case 174:
			return MultiplyInt32(Convert.ToInt32(Left), ToVBBool(Right));
		case 193:
		case 212:
			return MultiplyInt64(Convert.ToInt64(Left), ToVBBool(Right));
		case 231:
		case 288:
			return MultiplyDecimal(Left, ToVBBool(Right));
		case 250:
			return MultiplySingle(Convert.ToSingle(Left), ToVBBool(Right));
		case 269:
			return MultiplyDouble(Convert.ToDouble(Left), ToVBBool(Right));
		case 342:
			return MultiplyDouble(Conversions.ToDouble(Convert.ToString(Left)), 0.0);
		case 345:
			return MultiplyDouble(Conversions.ToDouble(Convert.ToString(Left)), ToVBBool(Right));
		case 347:
		case 348:
		case 349:
		case 350:
		case 351:
		case 352:
		case 353:
		case 354:
		case 355:
		case 356:
		case 357:
			return MultiplyDouble(Conversions.ToDouble(Convert.ToString(Left)), Convert.ToDouble(Right));
		case 360:
			return MultiplyDouble(Conversions.ToDouble(Convert.ToString(Left)), Conversions.ToDouble(Convert.ToString(Right)));
		default:
			if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
			{
				return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Multiply, Left, Right);
			}
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Multiply, Left, Right);
		}
	}

	private static object MultiplyByte(byte left, byte right)
	{
		checked
		{
			int num = left * right;
			if (num > 255)
			{
				if (num > 32767)
				{
					return num;
				}
				return (short)num;
			}
			return (byte)num;
		}
	}

	private static object MultiplySByte(sbyte left, sbyte right)
	{
		checked
		{
			short num = (short)unchecked(left * right);
			if (num > 127 || num < -128)
			{
				return num;
			}
			return (sbyte)num;
		}
	}

	private static object MultiplyInt16(short left, short right)
	{
		checked
		{
			int num = left * right;
			if (num > 32767 || num < -32768)
			{
				return num;
			}
			return (short)num;
		}
	}

	private static object MultiplyUInt16(ushort left, ushort right)
	{
		checked
		{
			long num = unchecked((long)left) * unchecked((long)right);
			if (num > 65535)
			{
				if (num > int.MaxValue)
				{
					return num;
				}
				return (int)num;
			}
			return (ushort)num;
		}
	}

	private static object MultiplyInt32(int left, int right)
	{
		checked
		{
			long num = unchecked((long)left) * unchecked((long)right);
			if (num > int.MaxValue || num < int.MinValue)
			{
				return num;
			}
			return (int)num;
		}
	}

	private static object MultiplyUInt32(uint left, uint right)
	{
		checked
		{
			ulong num = unchecked((ulong)left) * unchecked((ulong)right);
			if (num > uint.MaxValue)
			{
				if (decimal.Compare(new decimal(num), 9223372036854775807m) > 0)
				{
					return new decimal(num);
				}
				return (long)num;
			}
			return (uint)num;
		}
	}

	private static object MultiplyInt64(long left, long right)
	{
		try
		{
			return checked(left * right);
		}
		catch (OverflowException)
		{
		}
		try
		{
			return decimal.Multiply(new decimal(left), new decimal(right));
		}
		catch (OverflowException)
		{
			return (double)left * (double)right;
		}
	}

	private static object MultiplyUInt64(ulong left, ulong right)
	{
		try
		{
			return checked(left * right);
		}
		catch (OverflowException)
		{
		}
		try
		{
			return decimal.Multiply(new decimal(left), new decimal(right));
		}
		catch (OverflowException)
		{
			return (double)left * (double)right;
		}
	}

	private static object MultiplyDecimal(object left, object right)
	{
		decimal num = Convert.ToDecimal(left);
		decimal num2 = Convert.ToDecimal(right);
		try
		{
			return decimal.Multiply(num, num2);
		}
		catch (OverflowException)
		{
			return Convert.ToDouble(num) * Convert.ToDouble(num2);
		}
	}

	private static object MultiplySingle(float left, float right)
	{
		double num = (double)left * (double)right;
		if (num <= 3.4028234663852886E+38 && num >= -3.4028234663852886E+38)
		{
			return (float)num;
		}
		if (double.IsInfinity(num) && (float.IsInfinity(left) || float.IsInfinity(right)))
		{
			return (float)num;
		}
		return num;
	}

	private static object MultiplyDouble(double left, double right)
	{
		return left * right;
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object DivideObject(object Left, object Right)
	{
		TypeCode typeCode = GetTypeCode(Left);
		TypeCode typeCode2 = GetTypeCode(Right);
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 0:
			return DivideDouble(0.0, 0.0);
		case 3:
			return DivideDouble(0.0, ToVBBool(Right));
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
		case 14:
			return DivideDouble(0.0, Convert.ToDouble(Right));
		case 15:
			return DivideDecimal(0m, Right);
		case 13:
			return DivideSingle(0f, Convert.ToSingle(Right));
		case 18:
			return DivideDouble(0.0, Conversions.ToDouble(Convert.ToString(Right)));
		case 57:
			return DivideDouble(ToVBBool(Left), 0.0);
		case 60:
			return DivideDouble(ToVBBool(Left), ToVBBool(Right));
		case 62:
		case 63:
		case 64:
		case 65:
		case 66:
		case 67:
		case 68:
		case 69:
		case 71:
			return DivideDouble(ToVBBool(Left), Convert.ToDouble(Right));
		case 72:
			return DivideDecimal(ToVBBool(Left), Right);
		case 70:
			return DivideSingle(ToVBBool(Left), Convert.ToSingle(Right));
		case 75:
			return DivideDouble(ToVBBool(Left), Conversions.ToDouble(Convert.ToString(Right)));
		case 95:
		case 114:
		case 133:
		case 152:
		case 171:
		case 190:
		case 209:
		case 228:
		case 266:
			return DivideDouble(Convert.ToDouble(Left), 0.0);
		case 98:
		case 117:
		case 136:
		case 155:
		case 174:
		case 193:
		case 212:
		case 231:
		case 269:
			return DivideDouble(Convert.ToDouble(Left), ToVBBool(Right));
		case 100:
		case 101:
		case 102:
		case 103:
		case 104:
		case 105:
		case 106:
		case 107:
		case 109:
		case 119:
		case 120:
		case 121:
		case 122:
		case 123:
		case 124:
		case 125:
		case 126:
		case 128:
		case 138:
		case 139:
		case 140:
		case 141:
		case 142:
		case 143:
		case 144:
		case 145:
		case 147:
		case 157:
		case 158:
		case 159:
		case 160:
		case 161:
		case 162:
		case 163:
		case 164:
		case 166:
		case 176:
		case 177:
		case 178:
		case 179:
		case 180:
		case 181:
		case 182:
		case 183:
		case 185:
		case 195:
		case 196:
		case 197:
		case 198:
		case 199:
		case 200:
		case 201:
		case 202:
		case 204:
		case 214:
		case 215:
		case 216:
		case 217:
		case 218:
		case 219:
		case 220:
		case 221:
		case 223:
		case 233:
		case 234:
		case 235:
		case 236:
		case 237:
		case 238:
		case 239:
		case 240:
		case 242:
		case 261:
		case 271:
		case 272:
		case 273:
		case 274:
		case 275:
		case 276:
		case 277:
		case 278:
		case 279:
		case 280:
		case 281:
		case 299:
			return DivideDouble(Convert.ToDouble(Left), Convert.ToDouble(Right));
		case 110:
		case 129:
		case 148:
		case 167:
		case 186:
		case 205:
		case 224:
		case 243:
		case 290:
		case 291:
		case 292:
		case 293:
		case 294:
		case 295:
		case 296:
		case 297:
		case 300:
			return DivideDecimal(Left, Right);
		case 108:
		case 127:
		case 146:
		case 165:
		case 184:
		case 203:
		case 222:
		case 241:
		case 252:
		case 253:
		case 254:
		case 255:
		case 256:
		case 257:
		case 258:
		case 259:
		case 260:
		case 262:
		case 298:
			return DivideSingle(Convert.ToSingle(Left), Convert.ToSingle(Right));
		case 113:
		case 132:
		case 151:
		case 170:
		case 189:
		case 208:
		case 227:
		case 246:
		case 265:
		case 284:
		case 303:
			return DivideDouble(Convert.ToDouble(Left), Conversions.ToDouble(Convert.ToString(Right)));
		case 285:
			return DivideDecimal(Left, 0m);
		case 288:
			return DivideDecimal(Left, ToVBBool(Right));
		case 247:
			return DivideSingle(Convert.ToSingle(Left), 0f);
		case 250:
			return DivideSingle(Convert.ToSingle(Left), ToVBBool(Right));
		case 342:
			return DivideDouble(Conversions.ToDouble(Convert.ToString(Left)), 0.0);
		case 345:
			return DivideDouble(Conversions.ToDouble(Convert.ToString(Left)), ToVBBool(Right));
		case 347:
		case 348:
		case 349:
		case 350:
		case 351:
		case 352:
		case 353:
		case 354:
		case 355:
		case 356:
		case 357:
			return DivideDouble(Conversions.ToDouble(Convert.ToString(Left)), Convert.ToDouble(Right));
		case 360:
			return DivideDouble(Conversions.ToDouble(Convert.ToString(Left)), Conversions.ToDouble(Convert.ToString(Right)));
		default:
			if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
			{
				return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Divide, Left, Right);
			}
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Divide, Left, Right);
		}
	}

	private static object DivideDecimal(object left, object right)
	{
		decimal num = Convert.ToDecimal(left);
		decimal num2 = Convert.ToDecimal(right);
		try
		{
			return decimal.Divide(num, num2);
		}
		catch (OverflowException)
		{
			return Convert.ToSingle(num) / Convert.ToSingle(num2);
		}
	}

	private static object DivideSingle(float left, float right)
	{
		float num = left / right;
		if (float.IsInfinity(num))
		{
			if (float.IsInfinity(left) || float.IsInfinity(right))
			{
				return num;
			}
			return (double)left / (double)right;
		}
		return num;
	}

	private static object DivideDouble(double left, double right)
	{
		return left / right;
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object ExponentObject(object Left, object Right)
	{
		TypeCode typeCode = GetTypeCode(Left);
		TypeCode typeCode2 = GetTypeCode(Right);
		double x;
		switch (typeCode)
		{
		case TypeCode.Empty:
			x = 0.0;
			break;
		case TypeCode.Boolean:
			x = ToVBBool(Left);
			break;
		case TypeCode.SByte:
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.UInt16:
		case TypeCode.Int32:
		case TypeCode.UInt32:
		case TypeCode.Int64:
		case TypeCode.UInt64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			x = Convert.ToDouble(Left);
			break;
		case TypeCode.String:
			x = Conversions.ToDouble(Convert.ToString(Left));
			break;
		case TypeCode.Object:
			return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Power, Left, Right);
		default:
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Power, Left, Right);
		}
		double y;
		switch (typeCode2)
		{
		case TypeCode.Empty:
			y = 0.0;
			break;
		case TypeCode.Boolean:
			y = ToVBBool(Right);
			break;
		case TypeCode.SByte:
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.UInt16:
		case TypeCode.Int32:
		case TypeCode.UInt32:
		case TypeCode.Int64:
		case TypeCode.UInt64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			y = Convert.ToDouble(Right);
			break;
		case TypeCode.String:
			y = Conversions.ToDouble(Convert.ToString(Right));
			break;
		case TypeCode.Object:
			return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Power, Left, Right);
		default:
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Power, Left, Right);
		}
		return Math.Pow(x, y);
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object ModObject(object Left, object Right)
	{
		TypeCode typeCode = GetTypeCode(Left);
		TypeCode typeCode2 = GetTypeCode(Right);
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 0:
			return ModInt32(0, 0);
		case 3:
			return ModInt16(0, ToVBBool(Right));
		case 5:
			return ModSByte(0, Convert.ToSByte(Right));
		case 6:
			return ModByte(0, Convert.ToByte(Right));
		case 7:
			return ModInt16(0, Convert.ToInt16(Right));
		case 8:
			return ModUInt16(0, Convert.ToUInt16(Right));
		case 9:
			return ModInt32(0, Convert.ToInt32(Right));
		case 10:
			return ModUInt32(0u, Convert.ToUInt32(Right));
		case 11:
			return ModInt64(0L, Convert.ToInt64(Right));
		case 12:
			return ModUInt64(0uL, Convert.ToUInt64(Right));
		case 15:
			return ModDecimal(0m, Convert.ToDecimal(Right));
		case 13:
			return ModSingle(0f, Convert.ToSingle(Right));
		case 14:
			return ModDouble(0.0, Convert.ToDouble(Right));
		case 18:
			return ModDouble(0.0, Conversions.ToDouble(Convert.ToString(Right)));
		case 57:
			return ModInt16(ToVBBool(Left), 0);
		case 60:
			return ModInt16(ToVBBool(Left), ToVBBool(Right));
		case 62:
			return ModSByte(ToVBBool(Left), Convert.ToSByte(Right));
		case 63:
		case 64:
			return ModInt16(ToVBBool(Left), Convert.ToInt16(Right));
		case 65:
		case 66:
			return ModInt32(ToVBBool(Left), Convert.ToInt32(Right));
		case 67:
		case 68:
			return ModInt64(ToVBBool(Left), Convert.ToInt64(Right));
		case 69:
		case 72:
			return ModDecimal(ToVBBool(Left), Convert.ToDecimal(Right));
		case 70:
			return ModSingle(ToVBBool(Left), Convert.ToSingle(Right));
		case 71:
			return ModDouble(ToVBBool(Left), Convert.ToDouble(Right));
		case 75:
			return ModDouble(ToVBBool(Left), Conversions.ToDouble(Convert.ToString(Right)));
		case 95:
			return ModSByte(Convert.ToSByte(Left), 0);
		case 98:
			return ModSByte(Convert.ToSByte(Left), ToVBBool(Right));
		case 100:
			return ModSByte(Convert.ToSByte(Left), Convert.ToSByte(Right));
		case 101:
		case 102:
		case 119:
		case 121:
		case 138:
		case 139:
		case 140:
			return ModInt16(Convert.ToInt16(Left), Convert.ToInt16(Right));
		case 103:
		case 104:
		case 123:
		case 141:
		case 142:
		case 157:
		case 159:
		case 161:
		case 176:
		case 177:
		case 178:
		case 179:
		case 180:
			return ModInt32(Convert.ToInt32(Left), Convert.ToInt32(Right));
		case 105:
		case 106:
		case 125:
		case 143:
		case 144:
		case 163:
		case 181:
		case 182:
		case 195:
		case 197:
		case 199:
		case 201:
		case 214:
		case 215:
		case 216:
		case 217:
		case 218:
		case 219:
		case 220:
			return ModInt64(Convert.ToInt64(Left), Convert.ToInt64(Right));
		case 107:
		case 110:
		case 129:
		case 145:
		case 148:
		case 167:
		case 183:
		case 186:
		case 205:
		case 221:
		case 224:
		case 233:
		case 235:
		case 237:
		case 239:
		case 243:
		case 290:
		case 291:
		case 292:
		case 293:
		case 294:
		case 295:
		case 296:
		case 297:
		case 300:
			return ModDecimal(Left, Right);
		case 108:
		case 127:
		case 146:
		case 165:
		case 184:
		case 203:
		case 222:
		case 241:
		case 252:
		case 253:
		case 254:
		case 255:
		case 256:
		case 257:
		case 258:
		case 259:
		case 260:
		case 262:
		case 298:
			return ModSingle(Convert.ToSingle(Left), Convert.ToSingle(Right));
		case 109:
		case 128:
		case 147:
		case 166:
		case 185:
		case 204:
		case 223:
		case 242:
		case 261:
		case 271:
		case 272:
		case 273:
		case 274:
		case 275:
		case 276:
		case 277:
		case 278:
		case 279:
		case 280:
		case 281:
		case 299:
			return ModDouble(Convert.ToDouble(Left), Convert.ToDouble(Right));
		case 113:
		case 132:
		case 151:
		case 170:
		case 189:
		case 208:
		case 227:
		case 246:
		case 265:
		case 284:
		case 303:
			return ModDouble(Convert.ToDouble(Left), Conversions.ToDouble(Convert.ToString(Right)));
		case 114:
			return ModByte(Convert.ToByte(Left), 0);
		case 117:
		case 136:
			return ModInt16(Convert.ToInt16(Left), ToVBBool(Right));
		case 120:
			return ModByte(Convert.ToByte(Left), Convert.ToByte(Right));
		case 122:
		case 158:
		case 160:
			return ModUInt16(Convert.ToUInt16(Left), Convert.ToUInt16(Right));
		case 124:
		case 162:
		case 196:
		case 198:
		case 200:
			return ModUInt32(Convert.ToUInt32(Left), Convert.ToUInt32(Right));
		case 126:
		case 164:
		case 202:
		case 234:
		case 236:
		case 238:
		case 240:
			return ModUInt64(Convert.ToUInt64(Left), Convert.ToUInt64(Right));
		case 133:
			return ModInt16(Convert.ToInt16(Left), 0);
		case 152:
			return ModUInt16(Convert.ToUInt16(Left), 0);
		case 155:
		case 174:
			return ModInt32(Convert.ToInt32(Left), ToVBBool(Right));
		case 171:
			return ModInt32(Convert.ToInt32(Left), 0);
		case 190:
			return ModUInt32(Convert.ToUInt32(Left), 0u);
		case 193:
		case 212:
			return ModInt64(Convert.ToInt64(Left), ToVBBool(Right));
		case 209:
			return ModInt64(Convert.ToInt64(Left), 0L);
		case 228:
			return ModUInt64(Convert.ToUInt64(Left), 0uL);
		case 231:
		case 288:
			return ModDecimal(Left, ToVBBool(Right));
		case 285:
			return ModDecimal(Left, 0m);
		case 247:
			return ModSingle(Convert.ToSingle(Left), 0f);
		case 250:
			return ModSingle(Convert.ToSingle(Left), ToVBBool(Right));
		case 266:
			return ModDouble(Convert.ToDouble(Left), 0.0);
		case 269:
			return ModDouble(Convert.ToDouble(Left), ToVBBool(Right));
		case 342:
			return ModDouble(Conversions.ToDouble(Convert.ToString(Left)), 0.0);
		case 345:
			return ModDouble(Conversions.ToDouble(Convert.ToString(Left)), ToVBBool(Right));
		case 347:
		case 348:
		case 349:
		case 350:
		case 351:
		case 352:
		case 353:
		case 354:
		case 355:
		case 356:
		case 357:
			return ModDouble(Conversions.ToDouble(Convert.ToString(Left)), Convert.ToDouble(Right));
		case 360:
			return ModDouble(Conversions.ToDouble(Convert.ToString(Left)), Conversions.ToDouble(Convert.ToString(Right)));
		default:
			if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
			{
				return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Modulus, Left, Right);
			}
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.Modulus, Left, Right);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static object ModSByte(sbyte left, sbyte right)
	{
		checked
		{
			return (sbyte)unchecked(left % right);
		}
	}

	private static object ModByte(byte left, byte right)
	{
		checked
		{
			return (byte)unchecked((uint)left % (uint)right);
		}
	}

	private static object ModInt16(short left, short right)
	{
		checked
		{
			return (short)unchecked(left % right);
		}
	}

	private static object ModUInt16(ushort left, ushort right)
	{
		checked
		{
			return (ushort)unchecked((uint)left % (uint)right);
		}
	}

	private static object ModInt32(int left, int right)
	{
		if (left == int.MinValue && right == -1)
		{
			return 0;
		}
		return left % right;
	}

	private static object ModUInt32(uint left, uint right)
	{
		return left % right;
	}

	private static object ModInt64(long left, long right)
	{
		if (left == long.MinValue && right == -1)
		{
			return 0L;
		}
		return left % right;
	}

	private static object ModUInt64(ulong left, ulong right)
	{
		return left % right;
	}

	private static object ModDecimal(object left, object right)
	{
		decimal d = Convert.ToDecimal(left);
		decimal d2 = Convert.ToDecimal(right);
		return decimal.Remainder(d, d2);
	}

	private static object ModSingle(float left, float right)
	{
		return left % right;
	}

	private static object ModDouble(double left, double right)
	{
		return left % right;
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object IntDivideObject(object Left, object Right)
	{
		TypeCode typeCode = GetTypeCode(Left);
		TypeCode typeCode2 = GetTypeCode(Right);
		switch ((int)checked(unchecked((int)typeCode) * 19 + typeCode2))
		{
		case 0:
			return IntDivideInt32(0, 0);
		case 3:
			return IntDivideInt16(0, ToVBBool(Right));
		case 5:
			return IntDivideSByte(0, Convert.ToSByte(Right));
		case 6:
			return IntDivideByte(0, Convert.ToByte(Right));
		case 7:
			return IntDivideInt16(0, Convert.ToInt16(Right));
		case 8:
			return IntDivideUInt16(0, Convert.ToUInt16(Right));
		case 9:
			return IntDivideInt32(0, Convert.ToInt32(Right));
		case 10:
			return IntDivideUInt32(0u, Convert.ToUInt32(Right));
		case 11:
			return IntDivideInt64(0L, Convert.ToInt64(Right));
		case 12:
			return IntDivideUInt64(0uL, Convert.ToUInt64(Right));
		case 13:
		case 14:
		case 15:
			return IntDivideInt64(0L, Convert.ToInt64(Right));
		case 18:
			return IntDivideInt64(0L, Conversions.ToLong(Convert.ToString(Right)));
		case 57:
			return IntDivideInt16(ToVBBool(Left), 0);
		case 60:
			return IntDivideInt16(ToVBBool(Left), ToVBBool(Right));
		case 62:
			return IntDivideSByte(ToVBBool(Left), Convert.ToSByte(Right));
		case 63:
		case 64:
			return IntDivideInt16(ToVBBool(Left), Convert.ToInt16(Right));
		case 65:
		case 66:
			return IntDivideInt32(ToVBBool(Left), Convert.ToInt32(Right));
		case 67:
		case 68:
		case 69:
		case 70:
		case 71:
		case 72:
			return IntDivideInt64(ToVBBool(Left), Convert.ToInt64(Right));
		case 75:
			return IntDivideInt64(ToVBBool(Left), Conversions.ToLong(Convert.ToString(Right)));
		case 95:
			return IntDivideSByte(Convert.ToSByte(Left), 0);
		case 98:
			return IntDivideSByte(Convert.ToSByte(Left), ToVBBool(Right));
		case 100:
			return IntDivideSByte(Convert.ToSByte(Left), Convert.ToSByte(Right));
		case 101:
		case 102:
		case 119:
		case 121:
		case 138:
		case 139:
		case 140:
			return IntDivideInt16(Convert.ToInt16(Left), Convert.ToInt16(Right));
		case 103:
		case 104:
		case 123:
		case 141:
		case 142:
		case 157:
		case 159:
		case 161:
		case 176:
		case 177:
		case 178:
		case 179:
		case 180:
			return IntDivideInt32(Convert.ToInt32(Left), Convert.ToInt32(Right));
		case 105:
		case 106:
		case 107:
		case 108:
		case 109:
		case 110:
		case 125:
		case 127:
		case 128:
		case 129:
		case 143:
		case 144:
		case 145:
		case 146:
		case 147:
		case 148:
		case 163:
		case 165:
		case 166:
		case 167:
		case 181:
		case 182:
		case 183:
		case 184:
		case 185:
		case 186:
		case 195:
		case 197:
		case 199:
		case 201:
		case 203:
		case 204:
		case 205:
		case 214:
		case 215:
		case 216:
		case 217:
		case 218:
		case 219:
		case 220:
		case 221:
		case 222:
		case 223:
		case 224:
		case 233:
		case 235:
		case 237:
		case 239:
		case 241:
		case 242:
		case 243:
		case 252:
		case 253:
		case 254:
		case 255:
		case 256:
		case 257:
		case 258:
		case 259:
		case 260:
		case 261:
		case 262:
		case 271:
		case 272:
		case 273:
		case 274:
		case 275:
		case 276:
		case 277:
		case 278:
		case 279:
		case 280:
		case 281:
		case 290:
		case 291:
		case 292:
		case 293:
		case 294:
		case 295:
		case 296:
		case 297:
		case 298:
		case 299:
		case 300:
			return IntDivideInt64(Convert.ToInt64(Left), Convert.ToInt64(Right));
		case 113:
		case 132:
		case 151:
		case 170:
		case 189:
		case 208:
		case 227:
		case 246:
		case 265:
		case 284:
		case 303:
			return IntDivideInt64(Convert.ToInt64(Left), Conversions.ToLong(Convert.ToString(Right)));
		case 114:
			return IntDivideByte(Convert.ToByte(Left), 0);
		case 117:
		case 136:
			return IntDivideInt16(Convert.ToInt16(Left), ToVBBool(Right));
		case 120:
			return IntDivideByte(Convert.ToByte(Left), Convert.ToByte(Right));
		case 122:
		case 158:
		case 160:
			return IntDivideUInt16(Convert.ToUInt16(Left), Convert.ToUInt16(Right));
		case 124:
		case 162:
		case 196:
		case 198:
		case 200:
			return IntDivideUInt32(Convert.ToUInt32(Left), Convert.ToUInt32(Right));
		case 126:
		case 164:
		case 202:
		case 234:
		case 236:
		case 238:
		case 240:
			return IntDivideUInt64(Convert.ToUInt64(Left), Convert.ToUInt64(Right));
		case 133:
			return IntDivideInt16(Convert.ToInt16(Left), 0);
		case 152:
			return IntDivideUInt16(Convert.ToUInt16(Left), 0);
		case 155:
		case 174:
			return IntDivideInt32(Convert.ToInt32(Left), ToVBBool(Right));
		case 171:
			return IntDivideInt32(Convert.ToInt32(Left), 0);
		case 190:
			return IntDivideUInt32(Convert.ToUInt32(Left), 0u);
		case 193:
		case 212:
		case 231:
		case 250:
		case 269:
		case 288:
			return IntDivideInt64(Convert.ToInt64(Left), ToVBBool(Right));
		case 209:
			return IntDivideInt64(Convert.ToInt64(Left), 0L);
		case 228:
			return IntDivideUInt64(Convert.ToUInt64(Left), 0uL);
		case 247:
		case 266:
		case 285:
			return IntDivideInt64(Convert.ToInt64(Left), 0L);
		case 342:
			return IntDivideInt64(Conversions.ToLong(Convert.ToString(Left)), 0L);
		case 345:
			return IntDivideInt64(Conversions.ToLong(Convert.ToString(Left)), ToVBBool(Right));
		case 347:
		case 348:
		case 349:
		case 350:
		case 351:
		case 352:
		case 353:
		case 354:
		case 355:
		case 356:
		case 357:
			return IntDivideInt64(Conversions.ToLong(Convert.ToString(Left)), Convert.ToInt64(Right));
		case 360:
			return IntDivideInt64(Conversions.ToLong(Convert.ToString(Left)), Conversions.ToLong(Convert.ToString(Right)));
		default:
			if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
			{
				return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.IntegralDivide, Left, Right);
			}
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.IntegralDivide, Left, Right);
		}
	}

	private static object IntDivideSByte(sbyte left, sbyte right)
	{
		if (left == sbyte.MinValue && right == -1)
		{
			return (short)128;
		}
		checked
		{
			return (sbyte)unchecked(left / right);
		}
	}

	private static object IntDivideByte(byte left, byte right)
	{
		checked
		{
			return (byte)unchecked((uint)left / (uint)right);
		}
	}

	private static object IntDivideInt16(short left, short right)
	{
		if (left == short.MinValue && right == -1)
		{
			return 32768;
		}
		checked
		{
			return (short)unchecked(left / right);
		}
	}

	private static object IntDivideUInt16(ushort left, ushort right)
	{
		checked
		{
			return (ushort)unchecked((uint)left / (uint)right);
		}
	}

	private static object IntDivideInt32(int left, int right)
	{
		if (left == int.MinValue && right == -1)
		{
			return 2147483648L;
		}
		return left / right;
	}

	private static object IntDivideUInt32(uint left, uint right)
	{
		return left / right;
	}

	private static object IntDivideInt64(long left, long right)
	{
		return left / right;
	}

	private static object IntDivideUInt64(ulong left, ulong right)
	{
		return left / right;
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object LeftShiftObject(object Operand, object Amount)
	{
		TypeCode typeCode = GetTypeCode(Operand);
		TypeCode typeCode2 = GetTypeCode(Amount);
		if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
		{
			return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.ShiftLeft, Operand, Amount);
		}
		switch (typeCode)
		{
		case TypeCode.Empty:
			return 0 << Conversions.ToInteger(Amount);
		case TypeCode.Boolean:
			return (short)((short)(0 - (Convert.ToBoolean(Operand) ? 1 : 0)) << (Conversions.ToInteger(Amount) & 0xF));
		case TypeCode.SByte:
			return (sbyte)(Convert.ToSByte(Operand) << (Conversions.ToInteger(Amount) & 7));
		case TypeCode.Byte:
			return (byte)(Convert.ToByte(Operand) << (Conversions.ToInteger(Amount) & 7));
		case TypeCode.Int16:
			return (short)(Convert.ToInt16(Operand) << (Conversions.ToInteger(Amount) & 0xF));
		case TypeCode.UInt16:
			return (ushort)(Convert.ToUInt16(Operand) << (Conversions.ToInteger(Amount) & 0xF));
		case TypeCode.Int32:
			return Convert.ToInt32(Operand) << Conversions.ToInteger(Amount);
		case TypeCode.UInt32:
			return Convert.ToUInt32(Operand) << Conversions.ToInteger(Amount);
		case TypeCode.Int64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			return Convert.ToInt64(Operand) << Conversions.ToInteger(Amount);
		case TypeCode.UInt64:
			return Convert.ToUInt64(Operand) << Conversions.ToInteger(Amount);
		case TypeCode.String:
			return Conversions.ToLong(Convert.ToString(Operand)) << Conversions.ToInteger(Amount);
		default:
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.ShiftLeft, Operand);
		}
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object RightShiftObject(object Operand, object Amount)
	{
		TypeCode typeCode = GetTypeCode(Operand);
		TypeCode typeCode2 = GetTypeCode(Amount);
		if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
		{
			return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.ShiftRight, Operand, Amount);
		}
		switch (typeCode)
		{
		case TypeCode.Empty:
			return 0 >> Conversions.ToInteger(Amount);
		case TypeCode.Boolean:
			return (short)((short)(0 - (Convert.ToBoolean(Operand) ? 1 : 0)) >> (Conversions.ToInteger(Amount) & 0xF));
		case TypeCode.SByte:
			return (sbyte)(Convert.ToSByte(Operand) >> (Conversions.ToInteger(Amount) & 7));
		case TypeCode.Byte:
			return (byte)((uint)Convert.ToByte(Operand) >> (Conversions.ToInteger(Amount) & 7));
		case TypeCode.Int16:
			return (short)(Convert.ToInt16(Operand) >> (Conversions.ToInteger(Amount) & 0xF));
		case TypeCode.UInt16:
			return (ushort)((uint)Convert.ToUInt16(Operand) >> (Conversions.ToInteger(Amount) & 0xF));
		case TypeCode.Int32:
			return Convert.ToInt32(Operand) >> Conversions.ToInteger(Amount);
		case TypeCode.UInt32:
			return Convert.ToUInt32(Operand) >> Conversions.ToInteger(Amount);
		case TypeCode.Int64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			return Convert.ToInt64(Operand) >> Conversions.ToInteger(Amount);
		case TypeCode.UInt64:
			return Convert.ToUInt64(Operand) >> Conversions.ToInteger(Amount);
		case TypeCode.String:
			return Conversions.ToLong(Convert.ToString(Operand)) >> Conversions.ToInteger(Amount);
		default:
			throw GetNoValidOperatorException(Symbols.UserDefinedOperator.ShiftRight, Operand);
		}
	}

	[RequiresUnreferencedCode("The object types cannot be statically analyzed so their operators may be trimmed")]
	public static object ConcatenateObject(object Left, object Right)
	{
		TypeCode typeCode = ((Left is IConvertible convertible) ? convertible.GetTypeCode() : ((Left != null) ? TypeCode.Object : TypeCode.Empty));
		TypeCode typeCode2 = ((Right is IConvertible convertible2) ? convertible2.GetTypeCode() : ((Right != null) ? TypeCode.Object : TypeCode.Empty));
		if (typeCode == TypeCode.Object && Left is char[])
		{
			typeCode = TypeCode.String;
		}
		if (typeCode2 == TypeCode.Object && Right is char[])
		{
			typeCode2 = TypeCode.String;
		}
		if (typeCode == TypeCode.Object || typeCode2 == TypeCode.Object)
		{
			return InvokeUserDefinedOperator(Symbols.UserDefinedOperator.Concatenate, Left, Right);
		}
		bool flag = typeCode == TypeCode.DBNull;
		bool flag2 = typeCode2 == TypeCode.DBNull;
		if (flag & flag2)
		{
			return Left;
		}
		if (flag & !flag2)
		{
			Left = "";
		}
		else if (flag2 & !flag)
		{
			Right = "";
		}
		return Conversions.ToString(Left) + Conversions.ToString(Right);
	}
}
