using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class VBBinder : Binder
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class VBBinderState
	{
		internal object[] m_OriginalArgs;

		internal bool[] m_ByRefFlags;

		internal int[] m_OriginalParamOrder;

		internal VBBinderState()
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public enum BindScore
	{
		Exact,
		Widening0,
		Widening1,
		Narrowing,
		Unknown
	}

	internal string m_BindToName;

	internal Type m_objType;

	private VBBinderState m_state;

	private MemberInfo m_CachedMember;

	private bool[] m_ByRefFlags;

	private void ThrowInvalidCast(Type ArgType, Type ParmType, int ParmIndex)
	{
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromToArg4, CalledMethodName(), Conversions.ToString(checked(ParmIndex + 1)), Utils.VBFriendlyName(ArgType), Utils.VBFriendlyName(ParmType)));
	}

	public VBBinder(bool[] CopyBack)
	{
		m_ByRefFlags = CopyBack;
	}

	public override void ReorderArgumentArray(ref object[] args, object objState)
	{
		VBBinderState vBBinderState = (VBBinderState)objState;
		checked
		{
			if (args != null && vBBinderState != null)
			{
				if (vBBinderState.m_OriginalParamOrder != null)
				{
					if (m_ByRefFlags != null)
					{
						if (vBBinderState.m_ByRefFlags == null)
						{
							int upperBound = m_ByRefFlags.GetUpperBound(0);
							for (int i = 0; i <= upperBound; i++)
							{
								m_ByRefFlags[i] = false;
							}
						}
						else
						{
							int upperBound2 = vBBinderState.m_OriginalParamOrder.GetUpperBound(0);
							for (int i = 0; i <= upperBound2; i++)
							{
								int num = vBBinderState.m_OriginalParamOrder[i];
								if (num >= 0 && num <= args.GetUpperBound(0))
								{
									m_ByRefFlags[num] = vBBinderState.m_ByRefFlags[num];
									vBBinderState.m_OriginalArgs[num] = args[i];
								}
							}
						}
					}
				}
				else if (m_ByRefFlags != null)
				{
					if (vBBinderState.m_ByRefFlags == null)
					{
						int upperBound3 = m_ByRefFlags.GetUpperBound(0);
						for (int i = 0; i <= upperBound3; i++)
						{
							m_ByRefFlags[i] = false;
						}
					}
					else
					{
						int upperBound4 = m_ByRefFlags.GetUpperBound(0);
						for (int i = 0; i <= upperBound4; i++)
						{
							if (m_ByRefFlags[i])
							{
								bool flag = vBBinderState.m_ByRefFlags[i];
								m_ByRefFlags[i] = flag;
								if (flag)
								{
									vBBinderState.m_OriginalArgs[i] = args[i];
								}
							}
						}
					}
				}
			}
			if (vBBinderState != null)
			{
				vBBinderState.m_OriginalParamOrder = null;
				vBBinderState.m_ByRefFlags = null;
			}
		}
	}

	public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, ref object ObjState)
	{
		Type type = null;
		Type type2 = null;
		Type type3 = null;
		if (match == null || match.Length == 0)
		{
			throw ExceptionUtils.VbMakeException(438);
		}
		if ((object)m_CachedMember != null && m_CachedMember.MemberType == MemberTypes.Method && (object)match[0] != null && Operators.CompareString(match[0].Name, m_CachedMember.Name, TextCompare: false) == 0)
		{
			return (MethodBase)m_CachedMember;
		}
		bool flag = (bindingAttr & BindingFlags.SetProperty) != 0;
		if (names != null && names.Length == 0)
		{
			names = null;
		}
		int num = match.Length;
		checked
		{
			if (num > 1)
			{
				int upperBound = match.GetUpperBound(0);
				for (int i = 0; i <= upperBound; i++)
				{
					MethodBase methodBase = match[i];
					if ((object)methodBase == null || methodBase.IsHideBySig)
					{
						continue;
					}
					if (methodBase.IsVirtual)
					{
						if ((methodBase.Attributes & MethodAttributes.VtableLayoutMask) == 0)
						{
							continue;
						}
						int upperBound2 = match.GetUpperBound(0);
						for (int j = 0; j <= upperBound2; j++)
						{
							if (i != j && (object)match[j] != null && methodBase.DeclaringType.IsSubclassOf(match[j].DeclaringType))
							{
								match[j] = null;
								num--;
							}
						}
						continue;
					}
					int upperBound3 = match.GetUpperBound(0);
					for (int k = 0; k <= upperBound3; k++)
					{
						if (i != k && (object)match[k] != null && methodBase.DeclaringType.IsSubclassOf(match[k].DeclaringType))
						{
							match[k] = null;
							num--;
						}
					}
				}
			}
			int num2 = num;
			int num4 = default(int);
			int m = default(int);
			if (names != null)
			{
				int upperBound4 = match.GetUpperBound(0);
				for (int i = 0; i <= upperBound4; i++)
				{
					MethodBase methodBase = match[i];
					if ((object)methodBase == null)
					{
						continue;
					}
					ParameterInfo[] parameters = methodBase.GetParameters();
					int num3 = parameters.GetUpperBound(0);
					if (flag)
					{
						num3--;
					}
					if (num3 >= 0)
					{
						ParameterInfo parameterInfo = parameters[num3];
						num4 = -1;
						if (parameterInfo.ParameterType.IsArray)
						{
							object[] customAttributes = parameterInfo.GetCustomAttributes(typeof(ParamArrayAttribute), inherit: false);
							num4 = ((customAttributes == null || customAttributes.Length <= 0) ? (-1) : num3);
						}
					}
					int upperBound5 = names.GetUpperBound(0);
					for (int l = 0; l <= upperBound5; l++)
					{
						int num5 = num3;
						for (m = 0; m <= num5; m++)
						{
							if (Strings.StrComp(names[l], parameters[m].Name, CompareMethod.Text) != 0)
							{
								continue;
							}
							if (m == num4 && num == 1)
							{
								throw ExceptionUtils.VbMakeExceptionEx(446, System.SR.NamedArgumentOnParamArray);
							}
							if (m == num4)
							{
								m = num3 + 1;
							}
							break;
						}
						if (m > num3)
						{
							if (num == 1)
							{
								throw new MissingMemberException(System.SR.Format(System.SR.Argument_InvalidNamedArg2, names[l], CalledMethodName()));
							}
							match[i] = null;
							num--;
							break;
						}
					}
				}
			}
			int[] array = new int[match.Length - 1 + 1];
			int upperBound6 = match.GetUpperBound(0);
			for (int i = 0; i <= upperBound6; i++)
			{
				MethodBase methodBase = match[i];
				if ((object)methodBase == null)
				{
					continue;
				}
				num4 = -1;
				ParameterInfo[] parameters = methodBase.GetParameters();
				int num3 = parameters.GetUpperBound(0);
				if (flag)
				{
					num3--;
				}
				if (num3 >= 0)
				{
					ParameterInfo parameterInfo = parameters[num3];
					if (parameterInfo.ParameterType.IsArray)
					{
						object[] customAttributes2 = parameterInfo.GetCustomAttributes(typeof(ParamArrayAttribute), inherit: false);
						if (customAttributes2 != null && customAttributes2.Length > 0)
						{
							num4 = num3;
						}
					}
				}
				array[i] = num4;
				if (num4 == -1 && args.Length > parameters.Length)
				{
					if (num == 1)
					{
						throw new MissingMemberException(System.SR.Format(System.SR.NoMethodTakingXArguments2, CalledMethodName(), Conversions.ToString(GetPropArgCount(args, flag))));
					}
					match[i] = null;
					num--;
				}
				int num6 = num3;
				if (num4 != -1)
				{
					num6--;
				}
				if (args.Length >= num6)
				{
					continue;
				}
				int num7 = args.Length;
				int num8 = num6 - 1;
				int n;
				for (n = num7; n <= num8 && parameters[n].DefaultValue != DBNull.Value; n++)
				{
				}
				if (n != num6)
				{
					if (num == 1)
					{
						throw new MissingMemberException(System.SR.Format(System.SR.NoMethodTakingXArguments2, CalledMethodName(), Conversions.ToString(GetPropArgCount(args, flag))));
					}
					match[i] = null;
					num--;
				}
			}
			object[] array2 = new object[match.Length - 1 + 1];
			int upperBound7 = match.GetUpperBound(0);
			int[] array3;
			for (int i = 0; i <= upperBound7; i++)
			{
				MethodBase methodBase = match[i];
				if ((object)methodBase == null)
				{
					continue;
				}
				ParameterInfo[] parameters = methodBase.GetParameters();
				array3 = (int[])(array2[i] = ((args.Length <= parameters.Length) ? new int[parameters.Length - 1 + 1] : new int[args.Length - 1 + 1]));
				if (names == null)
				{
					int num9 = args.GetUpperBound(0);
					if (flag)
					{
						num9--;
					}
					int num10 = num9;
					int num11;
					for (num11 = 0; num11 <= num10; num11++)
					{
						if (args[num11] is Missing && (num11 > parameters.GetUpperBound(0) || parameters[num11].IsOptional))
						{
							array3[num11] = -1;
						}
						else
						{
							array3[num11] = num11;
						}
					}
					num9 = array3.GetUpperBound(0);
					int num12 = num11;
					int num13 = num9;
					for (num11 = num12; num11 <= num13; num11++)
					{
						array3[num11] = -1;
					}
					if (flag)
					{
						array3[num9] = args.GetUpperBound(0);
					}
					continue;
				}
				Exception ex = CreateParamOrder(flag, array3, methodBase.GetParameters(), args, names);
				if (ex != null)
				{
					if (num == 1)
					{
						throw ex;
					}
					match[i] = null;
					num--;
				}
			}
			Type[] array4 = new Type[args.Length - 1 + 1];
			int upperBound8 = args.GetUpperBound(0);
			for (int num11 = 0; num11 <= upperBound8; num11++)
			{
				if (args[num11] != null)
				{
					array4[num11] = args[num11].GetType();
				}
			}
			int upperBound9 = match.GetUpperBound(0);
			for (int i = 0; i <= upperBound9; i++)
			{
				MethodBase methodBase = match[i];
				if ((object)methodBase == null)
				{
					continue;
				}
				ParameterInfo[] parameters = methodBase.GetParameters();
				array3 = (int[])array2[i];
				int num3 = array3.GetUpperBound(0);
				if (flag)
				{
					num3--;
				}
				num4 = array[i];
				if (num4 != -1)
				{
					type3 = parameters[num4].ParameterType.GetElementType();
				}
				else if (array3.Length > parameters.Length)
				{
					goto IL_0876;
				}
				int num14 = num3;
				for (m = 0; m <= num14; m++)
				{
					int num11 = array3[m];
					if (num11 == -1)
					{
						if (parameters[m].IsOptional || m == array[i])
						{
							continue;
						}
						if (num == 1)
						{
							throw new MissingMemberException(System.SR.Format(System.SR.NoMethodTakingXArguments2, CalledMethodName(), Conversions.ToString(GetPropArgCount(args, flag))));
						}
					}
					else
					{
						type = array4[num11];
						if ((object)type == null)
						{
							continue;
						}
						if (num4 != -1 && m > num4)
						{
							type2 = parameters[num4].ParameterType.GetElementType();
						}
						else
						{
							type2 = parameters[m].ParameterType;
							if (type2.IsByRef)
							{
								type2 = type2.GetElementType();
							}
							if (m == num4)
							{
								if (type2.IsInstanceOfType(args[num11]) && m == num3)
								{
									continue;
								}
								type2 = type3;
							}
						}
						if ((object)type2 == type || (type == Type.Missing && parameters[m].IsOptional))
						{
							continue;
						}
						if (args[num11] != Missing.Value)
						{
							if ((object)type2 == typeof(object) || type2.IsInstanceOfType(args[num11]))
							{
								continue;
							}
							TypeCode typeCode = Type.GetTypeCode(type2);
							TypeCode typeCode2 = (((object)type != null) ? Type.GetTypeCode(type) : TypeCode.Empty);
							switch (typeCode)
							{
							case TypeCode.Boolean:
							case TypeCode.Byte:
							case TypeCode.Int16:
							case TypeCode.Int32:
							case TypeCode.Int64:
							case TypeCode.Single:
							case TypeCode.Double:
							case TypeCode.Decimal:
								switch (typeCode2)
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
									continue;
								}
								break;
							case TypeCode.Char:
								if (typeCode2 == TypeCode.String)
								{
									continue;
								}
								break;
							case TypeCode.String:
								switch (typeCode2)
								{
								case TypeCode.Empty:
								case TypeCode.Boolean:
								case TypeCode.Char:
								case TypeCode.Byte:
								case TypeCode.Int16:
								case TypeCode.Int32:
								case TypeCode.Int64:
								case TypeCode.Single:
								case TypeCode.Double:
								case TypeCode.Decimal:
								case TypeCode.String:
									continue;
								}
								if ((object)type == typeof(char[]))
								{
									continue;
								}
								break;
							case TypeCode.DateTime:
								if (typeCode2 == TypeCode.String)
								{
									continue;
								}
								break;
							default:
								if ((object)type2 != typeof(char[]))
								{
									break;
								}
								switch (typeCode2)
								{
								case TypeCode.Object:
									if ((object)type == typeof(char[]))
									{
										continue;
									}
									break;
								case TypeCode.String:
									continue;
								}
								break;
							}
						}
					}
					goto IL_0876;
				}
				continue;
				IL_0876:
				if (num == 1)
				{
					if (num2 != 1)
					{
						throw new AmbiguousMatchException(System.SR.Format(System.SR.AmbiguousMatch_NarrowingConversion1, CalledMethodName()));
					}
					ThrowInvalidCast(type, type2, m);
				}
				match[i] = null;
				num--;
			}
			num = 0;
			int upperBound10 = match.GetUpperBound(0);
			for (int i = 0; i <= upperBound10; i++)
			{
				MethodBase methodBase = match[i];
				if ((object)methodBase == null)
				{
					continue;
				}
				array3 = (int[])array2[i];
				ParameterInfo[] parameters = methodBase.GetParameters();
				bool flag2 = false;
				int num3 = parameters.GetUpperBound(0);
				if (flag)
				{
					num3--;
				}
				int upperBound11 = args.GetUpperBound(0);
				if (flag)
				{
					upperBound11--;
				}
				num4 = array[i];
				if (num4 != -1)
				{
					type3 = parameters[num3].ParameterType.GetElementType();
				}
				int num15 = num3;
				for (m = 0; m <= num15; m++)
				{
					type2 = ((m != num4) ? parameters[m].ParameterType : type3);
					if (type2.IsByRef)
					{
						flag2 = true;
						type2 = type2.GetElementType();
					}
					int num11 = array3[m];
					if ((num11 == -1 && parameters[m].IsOptional) || m == array[i])
					{
						continue;
					}
					type = array4[num11];
					if ((object)type == null || (type == Type.Missing && parameters[m].IsOptional) || (object)type2 == type || (object)type2 == typeof(object))
					{
						continue;
					}
					TypeCode typeCode3 = Type.GetTypeCode(type2);
					TypeCode typeCode4 = (((object)type != null) ? Type.GetTypeCode(type) : TypeCode.Empty);
					switch (typeCode3)
					{
					case TypeCode.Boolean:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						switch (typeCode4)
						{
						default:
							if (num == 0)
							{
								ThrowInvalidCast(type, type2, m);
							}
							break;
						case TypeCode.Boolean:
						case TypeCode.Byte:
						case TypeCode.Int16:
						case TypeCode.Int32:
						case TypeCode.Int64:
						case TypeCode.Single:
						case TypeCode.Double:
						case TypeCode.Decimal:
						case TypeCode.String:
							break;
						}
						break;
					}
				}
				if (m > num3)
				{
					if (i != num)
					{
						match[num] = match[i];
						array2[num] = array2[i];
						array[num] = array[i];
						match[i] = null;
					}
					num++;
					if (flag2)
					{
						bool flag3 = true;
					}
				}
				else
				{
					match[i] = null;
				}
			}
			if (num == 0)
			{
				throw new MissingMemberException(System.SR.Format(System.SR.NoMethodTakingXArguments2, CalledMethodName(), Conversions.ToString(GetPropArgCount(args, flag))));
			}
			VBBinderState vBBinderState = (VBBinderState)(ObjState = (m_state = new VBBinderState()));
			vBBinderState.m_OriginalArgs = args;
			int num16;
			if (num == 1)
			{
				num16 = 0;
			}
			else
			{
				num16 = 0;
				BindScore bindScore = BindScore.Unknown;
				int num17 = 0;
				int num18 = num - 1;
				for (int i = 0; i <= num18; i++)
				{
					MethodBase methodBase = match[i];
					if ((object)methodBase == null)
					{
						continue;
					}
					array3 = (int[])array2[i];
					BindScore bindScore2 = BindingScore(methodBase.GetParameters(), array3, array4, flag, array[i]);
					if (bindScore2 < bindScore)
					{
						if (i != 0)
						{
							match[0] = match[i];
							array2[0] = array2[i];
							array[0] = array[i];
							match[i] = null;
						}
						num17 = 1;
						bindScore = bindScore2;
					}
					else if (bindScore2 == bindScore)
					{
						if (bindScore2 == BindScore.Exact || bindScore2 == BindScore.Widening1)
						{
							switch (GetMostSpecific(match[0], methodBase, array3, array2, flag, array[0], array[i], args))
							{
							case -1:
								if (num17 != i)
								{
									match[num17] = match[i];
									array2[num17] = array2[i];
									array[num17] = array[i];
									match[i] = null;
								}
								num17++;
								continue;
							case 0:
								continue;
							}
							bool flag4 = true;
							int num19 = num17 - 1;
							for (int num20 = 1; num20 <= num19; num20++)
							{
								if (GetMostSpecific(match[num20], methodBase, array3, array2, flag, array[num20], array[i], args) != 1)
								{
									flag4 = false;
									break;
								}
							}
							if (flag4)
							{
								num17 = 0;
							}
							if (i != num17)
							{
								match[num17] = match[i];
								array2[num17] = array2[i];
								array[num17] = array[i];
								match[i] = null;
							}
							num17++;
						}
						else
						{
							if (num17 != i)
							{
								match[num17] = match[i];
								array2[num17] = array2[i];
								array[num17] = array[i];
								match[i] = null;
							}
							num17++;
						}
					}
					else
					{
						match[i] = null;
					}
				}
				if (num17 > 1)
				{
					int upperBound12 = match.GetUpperBound(0);
					for (int i = 0; i <= upperBound12; i++)
					{
						MethodBase methodBase = match[i];
						if ((object)methodBase == null)
						{
							continue;
						}
						int upperBound13 = match.GetUpperBound(0);
						for (int num21 = 0; num21 <= upperBound13; num21++)
						{
							if (i != num21 && (object)match[num21] != null && ((object)methodBase == match[num21] || (methodBase.DeclaringType.IsSubclassOf(match[num21].DeclaringType) && MethodsDifferOnlyByReturnType(methodBase, match[num21]))))
							{
								match[num21] = null;
								num17--;
							}
						}
					}
					int upperBound14 = match.GetUpperBound(0);
					for (int i = 0; i <= upperBound14; i++)
					{
						if ((object)match[i] != null)
						{
							continue;
						}
						int num22 = i + 1;
						int upperBound15 = match.GetUpperBound(0);
						for (int num23 = num22; num23 <= upperBound15; num23++)
						{
							MethodBase methodBase2 = match[num23];
							if ((object)methodBase2 != null)
							{
								match[i] = methodBase2;
								array2[i] = array2[num23];
								array[i] = array[num23];
								match[num23] = null;
							}
						}
					}
				}
				if (num17 > 1)
				{
					string text = "\r\n    " + Utils.MethodToString(match[0]);
					int num24 = num17 - 1;
					for (int i = 1; i <= num24; i++)
					{
						text = text + "\r\n    " + Utils.MethodToString(match[i]);
					}
					switch (bindScore)
					{
					case BindScore.Exact:
						throw new AmbiguousMatchException(System.SR.Format(System.SR.AmbiguousCall_ExactMatch2, CalledMethodName(), text));
					case BindScore.Widening0:
					case BindScore.Widening1:
						throw new AmbiguousMatchException(System.SR.Format(System.SR.AmbiguousCall_WideningConversion2, CalledMethodName(), text));
					default:
						throw new AmbiguousMatchException(System.SR.Format(System.SR.AmbiguousCall2, CalledMethodName(), text));
					}
				}
			}
			MethodBase methodBase3 = match[num16];
			array3 = (int[])array2[num16];
			if (names != null)
			{
				ReorderParams(array3, args, vBBinderState);
			}
			ParameterInfo[] parameters2 = methodBase3.GetParameters();
			if (args.Length > 0)
			{
				vBBinderState.m_ByRefFlags = new bool[args.GetUpperBound(0) + 1];
				bool flag3 = false;
				int upperBound16 = parameters2.GetUpperBound(0);
				for (m = 0; m <= upperBound16; m++)
				{
					if (!parameters2[m].ParameterType.IsByRef)
					{
						continue;
					}
					if (vBBinderState.m_OriginalParamOrder == null)
					{
						if (m < vBBinderState.m_ByRefFlags.Length)
						{
							vBBinderState.m_ByRefFlags[m] = true;
						}
					}
					else if (m < vBBinderState.m_OriginalParamOrder.Length)
					{
						int num25 = vBBinderState.m_OriginalParamOrder[m];
						if (num25 >= 0)
						{
							vBBinderState.m_ByRefFlags[num25] = true;
						}
					}
					flag3 = true;
				}
				if (!flag3)
				{
					vBBinderState.m_ByRefFlags = null;
				}
			}
			else
			{
				vBBinderState.m_ByRefFlags = null;
			}
			num4 = array[num16];
			if (num4 != -1)
			{
				int num3 = parameters2.GetUpperBound(0);
				if (flag)
				{
					num3--;
				}
				int upperBound11 = args.GetUpperBound(0);
				if (flag)
				{
					upperBound11--;
				}
				object[] array5 = new object[parameters2.Length - 1 + 1];
				int num26 = Math.Min(upperBound11, num4) - 1;
				for (m = 0; m <= num26; m++)
				{
					array5[m] = ObjectType.CTypeHelper(args[m], parameters2[m].ParameterType);
				}
				if (upperBound11 < num4)
				{
					int num27 = upperBound11 + 1;
					int num28 = num4 - 1;
					for (m = num27; m <= num28; m++)
					{
						array5[m] = ObjectType.CTypeHelper(parameters2[m].DefaultValue, parameters2[m].ParameterType);
					}
				}
				if (flag)
				{
					int upperBound17 = array5.GetUpperBound(0);
					array5[upperBound17] = ObjectType.CTypeHelper(args[args.GetUpperBound(0)], parameters2[upperBound17].ParameterType);
				}
				if (upperBound11 == -1)
				{
					array5[num4] = Array.CreateInstance(type3, 0);
				}
				else
				{
					type3 = parameters2[num3].ParameterType.GetElementType();
					int num29 = args.Length - parameters2.Length + 1;
					type2 = parameters2[num3].ParameterType;
					if (num29 == 1 && type2.IsArray && (args[num4] == null || type2.IsInstanceOfType(args[num4])))
					{
						array5[num4] = args[num4];
					}
					else if ((object)type3 == typeof(object))
					{
						object[] array6 = new object[num29 - 1 + 1];
						int num30 = num29 - 1;
						for (int num11 = 0; num11 <= num30; num11++)
						{
							array6[num11] = ObjectType.CTypeHelper(args[num11 + num4], type3);
						}
						array5[num4] = array6;
					}
					else
					{
						Array array7 = Array.CreateInstance(type3, num29);
						int num31 = num29 - 1;
						for (int num11 = 0; num11 <= num31; num11++)
						{
							array7.SetValue(ObjectType.CTypeHelper(args[num11 + num4], type3), num11);
						}
						array5[num4] = array7;
					}
				}
				args = array5;
			}
			else
			{
				object[] array8 = new object[parameters2.Length - 1 + 1];
				int upperBound18 = array8.GetUpperBound(0);
				int num11;
				for (num11 = 0; num11 <= upperBound18; num11++)
				{
					int num32 = array3[num11];
					if (num32 >= 0 && num32 <= args.GetUpperBound(0))
					{
						array8[num11] = ObjectType.CTypeHelper(args[num32], parameters2[num11].ParameterType);
					}
					else
					{
						array8[num11] = ObjectType.CTypeHelper(parameters2[num11].DefaultValue, parameters2[num11].ParameterType);
					}
				}
				int num33 = num11;
				int upperBound19 = parameters2.GetUpperBound(0);
				for (m = num33; m <= upperBound19; m++)
				{
					array8[m] = ObjectType.CTypeHelper(parameters2[m].DefaultValue, parameters2[m].ParameterType);
				}
				args = array8;
			}
			if ((object)methodBase3 == null)
			{
				throw new MissingMemberException(System.SR.Format(System.SR.NoMethodTakingXArguments2, CalledMethodName(), Conversions.ToString(GetPropArgCount(args, flag))));
			}
			return methodBase3;
		}
	}

	private int GetPropArgCount(object[] args, bool IsPropertySet)
	{
		if (IsPropertySet)
		{
			return checked(args.Length - 1);
		}
		return args.Length;
	}

	private int GetMostSpecific(MethodBase match0, MethodBase ThisMethod, int[] ArgIndexes, object[] ParamOrder, bool IsPropertySet, int ParamArrayIndex0, int ParamArrayIndex1, object[] args)
	{
		int num = -1;
		Type type = null;
		Type type2 = null;
		int num2 = args.GetUpperBound(0);
		ParameterInfo[] parameters = ThisMethod.GetParameters();
		ParameterInfo[] parameters2 = match0.GetParameters();
		int[] array = (int[])ParamOrder[0];
		num = -1;
		int num3 = args.GetUpperBound(0);
		int num4 = parameters2.GetUpperBound(0);
		int num5 = parameters.GetUpperBound(0);
		checked
		{
			if (IsPropertySet)
			{
				num4--;
				num5--;
				num3--;
				num2--;
			}
			bool flag;
			if (ParamArrayIndex0 == -1)
			{
				flag = false;
			}
			else
			{
				type = parameters2[ParamArrayIndex0].ParameterType.GetElementType();
				flag = true;
				if (num3 != -1 && num3 == num4)
				{
					object obj = args[num3];
					if (obj == null || parameters2[num4].ParameterType.IsInstanceOfType(obj))
					{
						flag = false;
					}
				}
			}
			bool flag2;
			if (ParamArrayIndex1 == -1)
			{
				flag2 = false;
			}
			else
			{
				type2 = parameters[ParamArrayIndex1].ParameterType.GetElementType();
				flag2 = true;
				if (num3 != -1 && num3 == num5)
				{
					object obj2 = args[num3];
					if (obj2 == null || parameters[num5].ParameterType.IsInstanceOfType(obj2))
					{
						flag2 = false;
					}
				}
			}
			int num6 = Math.Min(num2, Math.Max(num4, num5));
			for (int i = 0; i <= num6; i++)
			{
				int num7 = ((i > num4) ? (-1) : array[i]);
				int num8 = ((i > num5) ? (-1) : ArgIndexes[i]);
				if (num7 == -1 && num8 == -1)
				{
					continue;
				}
				Type type3;
				if (flag2 && ParamArrayIndex1 != -1 && i >= ParamArrayIndex1)
				{
					if (flag && ParamArrayIndex0 != -1 && i >= ParamArrayIndex0)
					{
						type3 = type;
					}
					else
					{
						type3 = parameters2[num7].ParameterType;
						if (type3.IsByRef)
						{
							type3 = type3.GetElementType();
						}
					}
					if ((object)type2 == type3)
					{
						if (num == -1 && ParamArrayIndex0 == -1 && i == num4 && args[num4] != null)
						{
							num = 0;
						}
					}
					else if (ObjectType.IsWideningConversion(type3, type2))
					{
						if (num == 1)
						{
							num = -1;
							break;
						}
						num = 0;
					}
					else if (ObjectType.IsWideningConversion(type2, type3))
					{
						if (num == 0)
						{
							num = -1;
							break;
						}
						num = 1;
					}
					continue;
				}
				Type type4;
				if (flag && ParamArrayIndex0 != -1 && i >= ParamArrayIndex0)
				{
					if (flag2 && ParamArrayIndex1 != -1 && i >= ParamArrayIndex1)
					{
						type4 = type2;
					}
					else
					{
						type4 = parameters[num8].ParameterType;
						if (type4.IsByRef)
						{
							type4 = type4.GetElementType();
						}
					}
					if ((object)type == type4)
					{
						if (num == -1 && ParamArrayIndex1 == -1 && i == num5 && args[num5] != null)
						{
							num = 1;
						}
					}
					else if (ObjectType.IsWideningConversion(type, type4))
					{
						if (num == 1)
						{
							num = -1;
							break;
						}
						num = 0;
					}
					else if (ObjectType.IsWideningConversion(type4, type))
					{
						if (num == 0)
						{
							num = -1;
							break;
						}
						num = 1;
					}
					continue;
				}
				type3 = parameters2[array[i]].ParameterType;
				type4 = parameters[ArgIndexes[i]].ParameterType;
				if ((object)type3 == type4)
				{
					continue;
				}
				if (ObjectType.IsWideningConversion(type3, type4))
				{
					if (num == 1)
					{
						num = -1;
						break;
					}
					num = 0;
				}
				else if (ObjectType.IsWideningConversion(type4, type3))
				{
					if (num == 0)
					{
						num = -1;
						break;
					}
					num = 1;
				}
				else if (ObjectType.IsWiderNumeric(type3, type4))
				{
					if (num == 0)
					{
						num = -1;
						break;
					}
					num = 1;
				}
				else if (ObjectType.IsWiderNumeric(type4, type3))
				{
					if (num == 1)
					{
						num = -1;
						break;
					}
					num = 0;
				}
				else
				{
					num = -1;
				}
			}
			if (num == -1)
			{
				if ((ParamArrayIndex0 == -1 || !flag) && ParamArrayIndex1 != -1)
				{
					if (flag2 && MatchesParamArraySignature(parameters2, parameters, ParamArrayIndex1, IsPropertySet, num2))
					{
						num = 0;
					}
				}
				else if ((ParamArrayIndex1 == -1 || !flag2) && ParamArrayIndex0 != -1 && flag && MatchesParamArraySignature(parameters, parameters2, ParamArrayIndex0, IsPropertySet, num2))
				{
					num = 1;
				}
			}
			return num;
		}
	}

	private bool MatchesParamArraySignature(ParameterInfo[] param0, ParameterInfo[] param1, int ParamArrayIndex1, bool IsPropertySet, int ArgCountUpperBound)
	{
		int num = param0.GetUpperBound(0);
		checked
		{
			if (IsPropertySet)
			{
				num--;
			}
			num = Math.Min(num, ArgCountUpperBound);
			int num2 = num;
			for (int i = 0; i <= num2; i++)
			{
				Type type = param0[i].ParameterType;
				if (type.IsByRef)
				{
					type = type.GetElementType();
				}
				Type parameterType;
				if (i >= ParamArrayIndex1)
				{
					parameterType = param1[ParamArrayIndex1].ParameterType;
					parameterType = parameterType.GetElementType();
				}
				else
				{
					parameterType = param1[i].ParameterType;
					if (parameterType.IsByRef)
					{
						parameterType = parameterType.GetElementType();
					}
				}
				if ((object)type != parameterType)
				{
					return false;
				}
			}
			return true;
		}
	}

	private bool MethodsDifferOnlyByReturnType(MethodBase match1, MethodBase match2)
	{
		ParameterInfo[] parameters = match1.GetParameters();
		ParameterInfo[] parameters2 = match2.GetParameters();
		int num = Math.Min(parameters.GetUpperBound(0), parameters2.GetUpperBound(0));
		int num2 = num;
		checked
		{
			for (int i = 0; i <= num2; i++)
			{
				Type type = parameters[i].ParameterType;
				if (type.IsByRef)
				{
					type = type.GetElementType();
				}
				Type type2 = parameters2[i].ParameterType;
				if (type2.IsByRef)
				{
					type2 = type2.GetElementType();
				}
				if ((object)type != type2)
				{
					return false;
				}
			}
			if (parameters.Length > parameters2.Length)
			{
				int num3 = num + 1;
				int upperBound = parameters2.GetUpperBound(0);
				for (int i = num3; i <= upperBound; i++)
				{
					if (!parameters[i].IsOptional)
					{
						return false;
					}
				}
			}
			else if (parameters2.Length > parameters.Length)
			{
				int num4 = num + 1;
				int upperBound2 = parameters.GetUpperBound(0);
				for (int i = num4; i <= upperBound2; i++)
				{
					if (!parameters2[i].IsOptional)
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture)
	{
		FieldInfo fieldInfo;
		if ((object)m_CachedMember != null && m_CachedMember.MemberType == MemberTypes.Field && (object)match[0] != null && Operators.CompareString(match[0].Name, m_CachedMember.Name, TextCompare: false) == 0)
		{
			fieldInfo = (FieldInfo)m_CachedMember;
		}
		else
		{
			fieldInfo = match[0];
			int upperBound = match.GetUpperBound(0);
			for (int i = 1; i <= upperBound; i = checked(i + 1))
			{
				if (match[i].DeclaringType.IsSubclassOf(fieldInfo.DeclaringType))
				{
					fieldInfo = match[i];
				}
			}
		}
		return fieldInfo;
	}

	public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
	{
		throw new NotSupportedException();
	}

	public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers)
	{
		BindScore bindScore = BindScore.Unknown;
		int num = 0;
		int upperBound = match.GetUpperBound(0);
		checked
		{
			for (int i = 0; i <= upperBound; i++)
			{
				PropertyInfo propertyInfo = match[i];
				if ((object)propertyInfo == null)
				{
					continue;
				}
				BindScore bindScore2 = BindingScore(propertyInfo.GetIndexParameters(), null, indexes, IsPropertySet: false, -1);
				if (bindScore2 < bindScore)
				{
					if (i != 0)
					{
						match[0] = match[i];
						match[i] = null;
					}
					num = 1;
					bindScore = bindScore2;
				}
				else if (bindScore2 == bindScore)
				{
					switch (bindScore2)
					{
					case BindScore.Widening1:
					{
						int num2 = -1;
						ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
						ParameterInfo[] indexParameters2 = match[0].GetIndexParameters();
						num2 = -1;
						int upperBound2 = indexParameters.GetUpperBound(0);
						for (int j = 0; j <= upperBound2; j++)
						{
							int num3 = j;
							int num4 = j;
							if (num3 == -1 || num4 == -1)
							{
								continue;
							}
							Type parameterType = indexParameters2[num3].ParameterType;
							Type parameterType2 = indexParameters[num4].ParameterType;
							if (ObjectType.IsWideningConversion(parameterType, parameterType2))
							{
								if (num2 == 1)
								{
									num2 = -1;
									break;
								}
								num2 = 0;
							}
							else if (ObjectType.IsWideningConversion(parameterType2, parameterType))
							{
								if (num2 == 0)
								{
									num2 = -1;
									break;
								}
								num2 = 1;
							}
						}
						switch (num2)
						{
						case -1:
							if (num != i)
							{
								match[num] = match[i];
								match[i] = null;
							}
							num++;
							break;
						case 0:
							num = 1;
							break;
						default:
							if (i != 0)
							{
								match[0] = match[i];
								match[i] = null;
							}
							num = 1;
							break;
						}
						break;
					}
					case BindScore.Exact:
						if (propertyInfo.DeclaringType.IsSubclassOf(match[0].DeclaringType))
						{
							if (i != 0)
							{
								match[0] = match[i];
								match[i] = null;
							}
							num = 1;
						}
						else if (!match[0].DeclaringType.IsSubclassOf(propertyInfo.DeclaringType))
						{
							if (num != i)
							{
								match[num] = match[i];
								match[i] = null;
							}
							num++;
						}
						break;
					default:
						if (num != i)
						{
							match[num] = match[i];
							match[i] = null;
						}
						num++;
						break;
					}
				}
				else
				{
					match[i] = null;
				}
			}
			if (num == 1)
			{
				return match[0];
			}
			return null;
		}
	}

	public override object ChangeType(object value, Type typ, CultureInfo culture)
	{
		try
		{
			if ((object)typ == typeof(object) || (typ.IsByRef && (object)typ.GetElementType() == typeof(object)))
			{
				return value;
			}
			return ObjectType.CTypeHelper(value, typ);
		}
		catch (Exception)
		{
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(value), Utils.VBFriendlyName(typ)));
		}
	}

	private BindScore BindingScore(ParameterInfo[] Parameters, int[] paramOrder, Type[] ArgTypes, bool IsPropertySet, int ParamArrayIndex)
	{
		BindScore bindScore = BindScore.Exact;
		int num = ArgTypes.GetUpperBound(0);
		int num2 = Parameters.GetUpperBound(0);
		checked
		{
			if (IsPropertySet)
			{
				num2--;
				num--;
			}
			int num3 = Math.Max(num, num2);
			for (int i = 0; i <= num3; i++)
			{
				int num4 = ((paramOrder != null) ? paramOrder[i] : i);
				Type type = ((num4 != -1) ? ArgTypes[num4] : null);
				if ((object)type == null)
				{
					continue;
				}
				Type type2 = ((i <= num2) ? Parameters[i].ParameterType : Parameters[ParamArrayIndex].ParameterType);
				if (i == ParamArrayIndex && type.IsArray && (object)type2 == type)
				{
					continue;
				}
				if (i == ParamArrayIndex && type.IsArray && (m_state.m_OriginalArgs == null || m_state.m_OriginalArgs[num4] == null || type2.IsInstanceOfType(m_state.m_OriginalArgs[num4])))
				{
					if (bindScore < BindScore.Widening1)
					{
						bindScore = BindScore.Widening1;
					}
					continue;
				}
				if ((ParamArrayIndex != -1 && i >= ParamArrayIndex) || type2.IsByRef)
				{
					type2 = type2.GetElementType();
				}
				if ((object)type == type2)
				{
					continue;
				}
				if (ObjectType.IsWideningConversion(type, type2))
				{
					if (bindScore < BindScore.Widening1)
					{
						bindScore = BindScore.Widening1;
					}
				}
				else if (type.IsArray && (m_state.m_OriginalArgs == null || m_state.m_OriginalArgs[num4] == null || type2.IsInstanceOfType(m_state.m_OriginalArgs[num4])))
				{
					if (bindScore < BindScore.Widening1)
					{
						bindScore = BindScore.Widening1;
					}
				}
				else
				{
					bindScore = BindScore.Narrowing;
				}
			}
			return bindScore;
		}
	}

	private void ReorderParams(int[] paramOrder, object[] vars, VBBinderState state)
	{
		int num = Math.Max(vars.GetUpperBound(0), paramOrder.GetUpperBound(0));
		checked
		{
			state.m_OriginalParamOrder = new int[num + 1];
			int num2 = num;
			for (int i = 0; i <= num2; i++)
			{
				state.m_OriginalParamOrder[i] = paramOrder[i];
			}
		}
	}

	private Exception CreateParamOrder(bool SetProp, int[] paramOrder, ParameterInfo[] pars, object[] args, string[] names)
	{
		checked
		{
			bool[] array = new bool[pars.Length - 1 + 1];
			int num = args.Length - names.Length - 1;
			int num2 = pars.GetUpperBound(0);
			int upperBound = pars.GetUpperBound(0);
			for (int i = 0; i <= upperBound; i++)
			{
				paramOrder[i] = -1;
			}
			if (SetProp)
			{
				paramOrder[pars.GetUpperBound(0)] = args.GetUpperBound(0);
				num--;
				num2--;
			}
			int num3 = num;
			for (int i = 0; i <= num3; i++)
			{
				paramOrder[i] = names.Length + i;
			}
			int upperBound2 = names.GetUpperBound(0);
			for (int i = 0; i <= upperBound2; i++)
			{
				int num4 = num2;
				int j;
				for (j = 0; j <= num4; j++)
				{
					if (Strings.StrComp(names[i], pars[j].Name, CompareMethod.Text) == 0)
					{
						if (paramOrder[j] != -1)
						{
							return new ArgumentException(System.SR.Format(System.SR.NamedArgumentAlreadyUsed1, pars[j].Name));
						}
						paramOrder[j] = i;
						array[i] = true;
						break;
					}
				}
				if (j > num2)
				{
					return new MissingMemberException(System.SR.Format(System.SR.Argument_InvalidNamedArg2, names[i], CalledMethodName()));
				}
			}
			return null;
		}
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	internal object InvokeMember(string name, BindingFlags invokeAttr, Type objType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] IReflect objIReflect, object target, object[] args, string[] namedParameters)
	{
		if (objType.IsCOMObject)
		{
			ParameterModifier[] modifiers = null;
			try
			{
				return objIReflect.InvokeMember(name, invokeAttr, null, target, args, modifiers, null, namedParameters);
			}
			catch (MissingMemberException)
			{
				throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_MemberNotFoundOnType2, name, Utils.VBFriendlyName(objType)));
			}
		}
		m_BindToName = name;
		m_objType = objType;
		if (name.Length == 0)
		{
			if (objType == objIReflect)
			{
				name = GetDefaultMemberName(objType);
				if (name == null)
				{
					throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_NoDefaultMemberFound1, Utils.VBFriendlyName(objType)));
				}
			}
			else
			{
				name = "";
			}
		}
		MethodBase[] methodsByName = GetMethodsByName(objType, objIReflect, name, invokeAttr);
		if (args == null)
		{
			args = Array.Empty<object>();
		}
		object ObjState = null;
		MethodBase obj = BindToMethod(invokeAttr, methodsByName, ref args, null, null, namedParameters, ref ObjState) ?? throw new MissingMemberException(System.SR.Format(System.SR.NoMethodTakingXArguments2, CalledMethodName(), Conversions.ToString(GetPropArgCount(args, (invokeAttr & BindingFlags.SetProperty) != 0))));
		SecurityCheckForLateboundCalls(obj, objType, objIReflect);
		MethodInfo methodInfo = (MethodInfo)obj;
		object result;
		if (objType == objIReflect || methodInfo.IsStatic || LateBinding.DoesTargetObjectMatch(target, methodInfo))
		{
			LateBinding.VerifyObjRefPresentForInstanceCall(target, methodInfo);
			result = methodInfo.Invoke(target, args);
		}
		else
		{
			result = LateBinding.InvokeMemberOnIReflect(objIReflect, methodInfo, BindingFlags.InvokeMethod, target, args);
		}
		if (ObjState != null)
		{
			ReorderArgumentArray(ref args, ObjState);
		}
		return result;
	}

	private string GetDefaultMemberName(Type typ)
	{
		do
		{
			object[] customAttributes = typ.GetCustomAttributes(typeof(DefaultMemberAttribute), inherit: false);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				return ((DefaultMemberAttribute)customAttributes[0]).MemberName;
			}
			typ = typ.BaseType;
		}
		while ((object)typ != null);
		return null;
	}

	private MethodBase[] GetMethodsByName(Type objType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] IReflect objIReflect, string name, BindingFlags invokeAttr)
	{
		MemberInfo[] member = objIReflect.GetMember(name, invokeAttr);
		member = LateBinding.GetNonGenericMembers(member);
		if (member == null)
		{
			return null;
		}
		int upperBound = member.GetUpperBound(0);
		checked
		{
			int num = default(int);
			for (int i = 0; i <= upperBound; i++)
			{
				MemberInfo memberInfo = member[i];
				if ((object)memberInfo == null)
				{
					continue;
				}
				if (memberInfo.MemberType == MemberTypes.Field)
				{
					Type declaringType = memberInfo.DeclaringType;
					int upperBound2 = member.GetUpperBound(0);
					for (int j = 0; j <= upperBound2; j++)
					{
						if (i != j && (object)member[j] != null && declaringType.IsSubclassOf(member[j].DeclaringType))
						{
							member[j] = null;
							num++;
						}
					}
				}
				else if (memberInfo.MemberType == MemberTypes.Method)
				{
					MethodInfo methodInfo = (MethodInfo)memberInfo;
					if (methodInfo.IsHideBySig || (methodInfo.IsVirtual && (!methodInfo.IsVirtual || (methodInfo.Attributes & MethodAttributes.VtableLayoutMask) == 0) && (!methodInfo.IsVirtual || (methodInfo.GetBaseDefinition().Attributes & MethodAttributes.VtableLayoutMask) == 0)))
					{
						continue;
					}
					Type declaringType = memberInfo.DeclaringType;
					int upperBound3 = member.GetUpperBound(0);
					for (int k = 0; k <= upperBound3; k++)
					{
						if (i != k && (object)member[k] != null && declaringType.IsSubclassOf(member[k].DeclaringType))
						{
							member[k] = null;
							num++;
						}
					}
				}
				else if (memberInfo.MemberType == MemberTypes.Property)
				{
					PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
					int num2 = 1;
					MethodInfo methodInfo;
					do
					{
						methodInfo = ((num2 != 1) ? propertyInfo.GetSetMethod() : propertyInfo.GetGetMethod());
						if ((object)methodInfo != null && !methodInfo.IsHideBySig && (!methodInfo.IsVirtual || (methodInfo.IsVirtual && (methodInfo.Attributes & MethodAttributes.VtableLayoutMask) != MethodAttributes.PrivateScope)))
						{
							Type declaringType = memberInfo.DeclaringType;
							int upperBound4 = member.GetUpperBound(0);
							for (int l = 0; l <= upperBound4; l++)
							{
								if (i != l && (object)member[l] != null && declaringType.IsSubclassOf(member[l].DeclaringType))
								{
									member[l] = null;
									num++;
								}
							}
						}
						num2++;
					}
					while (num2 <= 2);
					methodInfo = (((invokeAttr & BindingFlags.GetProperty) != BindingFlags.Default) ? propertyInfo.GetGetMethod() : (((invokeAttr & BindingFlags.SetProperty) == 0) ? null : propertyInfo.GetSetMethod()));
					if ((object)methodInfo == null)
					{
						num++;
					}
					member[i] = methodInfo;
				}
				else
				{
					if (memberInfo.MemberType != MemberTypes.NestedType)
					{
						continue;
					}
					Type declaringType = memberInfo.DeclaringType;
					int upperBound5 = member.GetUpperBound(0);
					for (int m = 0; m <= upperBound5; m++)
					{
						if (i != m && (object)member[m] != null && declaringType.IsSubclassOf(member[m].DeclaringType))
						{
							member[m] = null;
							num++;
						}
					}
					if (num == member.Length - 1)
					{
						throw new ArgumentException(System.SR.Format(System.SR.Argument_IllegalNestedType2, name, Utils.VBFriendlyName(objType)));
					}
					member[i] = null;
					num++;
				}
			}
			MethodBase[] array = new MethodBase[member.Length - num - 1 + 1];
			int num3 = 0;
			int num4 = member.Length - 1;
			for (int n = 0; n <= num4; n++)
			{
				if ((object)member[n] != null)
				{
					array[num3] = (MethodBase)member[n];
					num3++;
				}
			}
			return array;
		}
	}

	internal string CalledMethodName()
	{
		return m_objType.Name + "." + m_BindToName;
	}

	internal static void SecurityCheckForLateboundCalls(MemberInfo member, Type objType, IReflect objIReflect)
	{
		if (objType != objIReflect && !IsMemberPublic(member))
		{
			throw new MissingMethodException();
		}
		Type declaringType = member.DeclaringType;
		if (!declaringType.IsPublic && (object)declaringType.Assembly == Utils.VBRuntimeAssembly)
		{
			throw new MissingMethodException();
		}
	}

	private static bool IsMemberPublic(MemberInfo Member)
	{
		return Member.MemberType switch
		{
			MemberTypes.Method => ((MethodInfo)Member).IsPublic, 
			MemberTypes.Field => ((FieldInfo)Member).IsPublic, 
			MemberTypes.Constructor => ((ConstructorInfo)Member).IsPublic, 
			MemberTypes.Property => false, 
			_ => false, 
		};
	}

	internal void CacheMember(MemberInfo member)
	{
		m_CachedMember = member;
	}
}
