using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class Symbols
{
	internal enum UserDefinedOperator : sbyte
	{
		UNDEF,
		Narrow,
		Widen,
		IsTrue,
		IsFalse,
		Negate,
		Not,
		UnaryPlus,
		Plus,
		Minus,
		Multiply,
		Divide,
		Power,
		IntegralDivide,
		Concatenate,
		ShiftLeft,
		ShiftRight,
		Modulus,
		Or,
		Xor,
		And,
		Like,
		Equal,
		NotEqual,
		Less,
		LessEqual,
		GreaterEqual,
		Greater,
		MAX
	}

	internal sealed class Container
	{
		private sealed class InheritanceSorter : IComparer<MemberInfo>
		{
			internal static readonly InheritanceSorter Instance = new InheritanceSorter();

			private InheritanceSorter()
			{
			}

			private int Compare(MemberInfo left, MemberInfo right)
			{
				Type declaringType = left.DeclaringType;
				Type declaringType2 = right.DeclaringType;
				if ((object)declaringType == declaringType2)
				{
					return 0;
				}
				if (declaringType.IsSubclassOf(declaringType2))
				{
					return -1;
				}
				return 1;
			}

			int IComparer<MemberInfo>.Compare(MemberInfo left, MemberInfo right)
			{
				//ILSpy generated this explicit interface implementation from .override directive in Compare
				return this.Compare(left, right);
			}
		}

		private readonly object _instance;

		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
		private readonly Type _type;

		private static readonly MemberInfo[] s_noMembers = Array.Empty<MemberInfo>();

		internal bool IsWindowsRuntimeObject
		{
			get
			{
				Type type = _type;
				while ((object)type != null)
				{
					if ((type.Attributes & TypeAttributes.WindowsRuntime) == TypeAttributes.WindowsRuntime)
					{
						return true;
					}
					if ((type.Attributes & TypeAttributes.Import) == TypeAttributes.Import)
					{
						return false;
					}
					type = type.BaseType;
				}
				return false;
			}
		}

		internal bool IsCOMObject => _type.IsCOMObject;

		internal string VBFriendlyName => Utils.VBFriendlyName(_type, _instance);

		internal bool IsArray
		{
			get
			{
				if (IsArrayType(_type))
				{
					return _instance != null;
				}
				return false;
			}
		}

		internal bool IsValueType
		{
			get
			{
				if (IsValueType(_type))
				{
					return _instance != null;
				}
				return false;
			}
		}

		[RequiresUnreferencedCode("Calls Object.GetType() which cannot be statically analyzed")]
		internal Container(object instance)
		{
			if (instance == null)
			{
				throw ExceptionUtils.VbMakeObjNotSetException();
			}
			_instance = instance;
			_type = instance.GetType();
		}

		internal Container([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
		{
			if ((object)type == null)
			{
				throw ExceptionUtils.VbMakeObjNotSetException();
			}
			_instance = null;
			_type = type;
		}

		private static MemberInfo[] FilterInvalidMembers(MemberInfo[] members)
		{
			if (members == null || members.Length == 0)
			{
				return null;
			}
			int num = 0;
			int num2 = 0;
			checked
			{
				int num3 = members.Length - 1;
				for (num2 = 0; num2 <= num3; num2++)
				{
					ParameterInfo[] array = null;
					Type returnType = null;
					switch (members[num2].MemberType)
					{
					case MemberTypes.Constructor:
					case MemberTypes.Method:
					{
						MethodInfo obj = (MethodInfo)members[num2];
						array = obj.GetParameters();
						returnType = obj.ReturnType;
						break;
					}
					case MemberTypes.Property:
					{
						PropertyInfo propertyInfo = (PropertyInfo)members[num2];
						MethodInfo getMethod = propertyInfo.GetGetMethod();
						if ((object)getMethod != null)
						{
							array = getMethod.GetParameters();
						}
						else
						{
							ParameterInfo[] parameters = propertyInfo.GetSetMethod().GetParameters();
							array = new ParameterInfo[parameters.Length - 2 + 1];
							Array.Copy(parameters, array, array.Length);
						}
						returnType = propertyInfo.PropertyType;
						break;
					}
					case MemberTypes.Field:
						returnType = ((FieldInfo)members[num2]).FieldType;
						break;
					}
					if (AreParametersAndReturnTypesValid(array, returnType))
					{
						num++;
					}
					else
					{
						members[num2] = null;
					}
				}
				if (num == members.Length)
				{
					return members;
				}
				if (num > 0)
				{
					MemberInfo[] array2 = new MemberInfo[num - 1 + 1];
					int num4 = 0;
					int num5 = members.Length - 1;
					for (num2 = 0; num2 <= num5; num2++)
					{
						if ((object)members[num2] != null)
						{
							array2[num4] = members[num2];
							num4++;
						}
					}
					return array2;
				}
				return null;
			}
		}

		internal List<MemberInfo> LookupWinRTCollectionInterfaceMembers(string memberName)
		{
			List<MemberInfo> list = new List<MemberInfo>();
			Type[] interfaces = _type.GetInterfaces();
			foreach (Type type in interfaces)
			{
				if (IsCollectionInterface(type))
				{
					MemberInfo[] member = type.GetMember(memberName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					if (member != null)
					{
						list.AddRange(member);
					}
				}
			}
			return list;
		}

		internal MemberInfo[] LookupNamedMembers(string memberName)
		{
			MemberInfo[] array = ((!IsGenericParameter(_type)) ? _type.GetMember(memberName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).ToArray() : GetClassConstraint(_type)?.GetMember(memberName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).ToArray());
			if (IsWindowsRuntimeObject)
			{
				List<MemberInfo> list = LookupWinRTCollectionInterfaceMembers(memberName);
				if (array != null)
				{
					list.AddRange(array);
				}
				array = list.ToArray();
			}
			array = FilterInvalidMembers(array);
			if (array == null)
			{
				array = s_noMembers;
			}
			else if (array.Length > 1)
			{
				Array.Sort(array, InheritanceSorter.Instance);
			}
			return array;
		}

		[RequiresUnreferencedCode("Calls Container.LookupDefaultMembers")]
		private List<MemberInfo> LookupWinRTCollectionDefaultMembers(ref string defaultMemberName)
		{
			List<MemberInfo> list = new List<MemberInfo>();
			Type[] interfaces = _type.GetInterfaces();
			foreach (Type type in interfaces)
			{
				if (IsCollectionInterface(type))
				{
					MemberInfo[] array = LookupDefaultMembers(ref defaultMemberName, type);
					if (array != null)
					{
						list.AddRange(array);
					}
				}
			}
			return list;
		}

		[RequiresUnreferencedCode("Calls Type.GetMember() on a type that cannot be statically analyzed")]
		private MemberInfo[] LookupDefaultMembers(ref string defaultMemberName, Type searchType)
		{
			string text = null;
			Type type = searchType;
			do
			{
				object[] array = type.GetCustomAttributes(typeof(DefaultMemberAttribute), inherit: false).ToArray();
				if (array != null && array.Length > 0)
				{
					text = ((DefaultMemberAttribute)array[0]).MemberName;
					break;
				}
				type = type.BaseType;
			}
			while ((object)type != null && !IsRootObjectType(type));
			if (text != null)
			{
				MemberInfo[] members = type.GetMember(text, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).ToArray();
				members = FilterInvalidMembers(members);
				if (members != null)
				{
					defaultMemberName = text;
					if (members.Length > 1)
					{
						Array.Sort(members, InheritanceSorter.Instance);
					}
					return members;
				}
			}
			return s_noMembers;
		}

		[RequiresUnreferencedCode("Calls LookupDefaultMembers")]
		internal MemberInfo[] GetMembers(ref string memberName, bool reportErrors)
		{
			if (memberName == null)
			{
				memberName = "";
			}
			MemberInfo[] array;
			if (Operators.CompareString(memberName, "", TextCompare: false) == 0)
			{
				array = LookupDefaultMembers(ref memberName, _type);
				if (IsWindowsRuntimeObject)
				{
					List<MemberInfo> list = LookupWinRTCollectionDefaultMembers(ref memberName);
					if (array != null)
					{
						list.AddRange(array);
					}
					array = list.ToArray();
				}
				if (array.Length == 0)
				{
					if (reportErrors)
					{
						throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_NoDefaultMemberFound1, VBFriendlyName));
					}
					return array;
				}
			}
			else
			{
				array = LookupNamedMembers(memberName);
				if (array.Length == 0)
				{
					if (reportErrors)
					{
						throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_MemberNotFoundOnType2, memberName, VBFriendlyName));
					}
					return array;
				}
			}
			return array;
		}

		internal object GetFieldValue(FieldInfo field)
		{
			if (_instance == null && !IsShared(field))
			{
				throw new NullReferenceException(System.SR.Format(System.SR.NullReference_InstanceReqToAccessMember1, Utils.FieldToString(field)));
			}
			if (IsNonPublicRuntimeMember(field))
			{
				throw new MissingMemberException();
			}
			return field.GetValue(_instance);
		}

		[RequiresUnreferencedCode("Cannot statically analyze the type of FieldInfo.FieldType")]
		internal void SetFieldValue(FieldInfo field, object value)
		{
			if (field.IsInitOnly)
			{
				throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_ReadOnlyField2, field.Name, VBFriendlyName));
			}
			if (_instance == null && !IsShared(field))
			{
				throw new NullReferenceException(System.SR.Format(System.SR.NullReference_InstanceReqToAccessMember1, Utils.FieldToString(field)));
			}
			if (IsNonPublicRuntimeMember(field))
			{
				throw new MissingMemberException();
			}
			field.SetValue(_instance, Conversions.ChangeType(value, field.FieldType));
		}

		[RequiresUnreferencedCode("Calls Conversions.ChangeType")]
		internal object GetArrayValue(object[] indices)
		{
			Array array = (Array)_instance;
			int rank = array.Rank;
			if (indices.Length != rank)
			{
				throw new RankException();
			}
			int num = (int)Conversions.ChangeType(indices[0], typeof(int));
			if (rank == 1)
			{
				return array.GetValue(num);
			}
			int num2 = (int)Conversions.ChangeType(indices[1], typeof(int));
			if (rank == 2)
			{
				return array.GetValue(num, num2);
			}
			int num3 = (int)Conversions.ChangeType(indices[2], typeof(int));
			if (rank == 3)
			{
				return array.GetValue(num, num2, num3);
			}
			checked
			{
				int[] array2 = new int[rank - 1 + 1];
				array2[0] = num;
				array2[1] = num2;
				array2[2] = num3;
				int num4 = rank - 1;
				for (int i = 3; i <= num4; i++)
				{
					array2[i] = (int)Conversions.ChangeType(indices[i], typeof(int));
				}
				return array.GetValue(array2);
			}
		}

		[RequiresUnreferencedCode("Uses _type.GetElementType which cannot be statically analyzed.")]
		internal void SetArrayValue(object[] arguments)
		{
			Array array = (Array)_instance;
			int rank = array.Rank;
			checked
			{
				if (arguments.Length - 1 != rank)
				{
					throw new RankException();
				}
				object expression = arguments[arguments.Length - 1];
				Type elementType = _type.GetElementType();
				int num = (int)Conversions.ChangeType(arguments[0], typeof(int));
				if (rank == 1)
				{
					array.SetValue(Conversions.ChangeType(expression, elementType), num);
					return;
				}
				int num2 = (int)Conversions.ChangeType(arguments[1], typeof(int));
				if (rank == 2)
				{
					array.SetValue(Conversions.ChangeType(expression, elementType), num, num2);
					return;
				}
				int num3 = (int)Conversions.ChangeType(arguments[2], typeof(int));
				if (rank == 3)
				{
					array.SetValue(Conversions.ChangeType(expression, elementType), num, num2, num3);
					return;
				}
				int[] array2 = new int[rank - 1 + 1];
				array2[0] = num;
				array2[1] = num2;
				array2[2] = num3;
				int num4 = rank - 1;
				for (int i = 3; i <= num4; i++)
				{
					array2[i] = (int)Conversions.ChangeType(arguments[i], typeof(int));
				}
				array.SetValue(Conversions.ChangeType(expression, elementType), array2);
			}
		}

		[RequiresUnreferencedCode("Calls ConstructCallArguments which is unsafe")]
		internal object InvokeMethod(Method targetProcedure, object[] arguments, bool[] copyBack, BindingFlags flags)
		{
			MethodBase callTarget = NewLateBinding.GetCallTarget(targetProcedure, flags);
			object[] array = NewLateBinding.ConstructCallArguments(targetProcedure, arguments, flags);
			if (_instance == null && !IsShared(callTarget))
			{
				throw new NullReferenceException(System.SR.Format(System.SR.NullReference_InstanceReqToAccessMember1, targetProcedure.ToString()));
			}
			if (IsNonPublicRuntimeMember(callTarget))
			{
				throw new MissingMemberException();
			}
			object result;
			try
			{
				result = callTarget.Invoke(_instance, array);
			}
			catch (TargetInvocationException ex) when (ex.InnerException != null)
			{
				throw ex.InnerException;
			}
			OverloadResolution.ReorderArgumentArray(targetProcedure, array, arguments, copyBack, flags);
			return result;
		}
	}

	internal sealed class Method
	{
		private MemberInfo _item;

		private MethodBase _rawItem;

		private ParameterInfo[] _parameters;

		private ParameterInfo[] _rawParameters;

		private ParameterInfo[] _rawParametersFromType;

		private Type _rawDeclaringType;

		internal readonly int ParamArrayIndex;

		internal readonly bool ParamArrayExpanded;

		internal bool NotCallable;

		internal bool RequiresNarrowingConversion;

		internal bool AllNarrowingIsFromObject;

		internal bool LessSpecific;

		internal bool ArgumentsValidated;

		internal int[] NamedArgumentMapping;

		internal Type[] TypeArguments;

		internal bool ArgumentMatchingDone;

		internal ParameterInfo[] Parameters => _parameters;

		internal ParameterInfo[] RawParameters => _rawParameters;

		internal ParameterInfo[] RawParametersFromType
		{
			[RequiresUnreferencedCode("Cannot statically analyze the type of _item")]
			get
			{
				if (_rawParametersFromType == null)
				{
					if (!IsProperty)
					{
						MethodInfo methodInfo = (MethodInfo)_item;
						if (methodInfo.IsGenericMethod)
						{
							methodInfo = methodInfo.GetGenericMethodDefinition();
						}
						Type type = methodInfo.DeclaringType;
						if (type.IsConstructedGenericType)
						{
							type = type.GetGenericTypeDefinition();
						}
						MethodInfo methodInfo2 = null;
						foreach (MethodInfo declaredMethod in type.GetTypeInfo().GetDeclaredMethods(methodInfo.Name))
						{
							if (declaredMethod.HasSameMetadataDefinitionAs(methodInfo))
							{
								methodInfo2 = declaredMethod;
								break;
							}
						}
						_rawParametersFromType = methodInfo2.GetParameters();
					}
					else
					{
						_rawParametersFromType = _rawParameters;
					}
				}
				return _rawParametersFromType;
			}
		}

		internal Type DeclaringType => _item.DeclaringType;

		internal Type RawDeclaringType
		{
			get
			{
				if ((object)_rawDeclaringType == null)
				{
					Type type = _item.DeclaringType;
					if (type.IsConstructedGenericType)
					{
						type = type.GetGenericTypeDefinition();
					}
					_rawDeclaringType = type;
				}
				return _rawDeclaringType;
			}
		}

		internal bool HasParamArray => ParamArrayIndex > -1;

		internal bool HasByRefParameter
		{
			get
			{
				ParameterInfo[] parameters = Parameters;
				for (int i = 0; i < parameters.Length; i = checked(i + 1))
				{
					if (parameters[i].ParameterType.IsByRef)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal bool IsProperty => _item.MemberType == MemberTypes.Property;

		internal bool IsMethod
		{
			get
			{
				if (_item.MemberType != MemberTypes.Method)
				{
					return _item.MemberType == MemberTypes.Constructor;
				}
				return true;
			}
		}

		internal bool IsGeneric => IsGeneric(_item);

		internal Type[] TypeParameters => GetTypeParameters(_item);

		private Method(ParameterInfo[] parameters, int paramArrayIndex, bool paramArrayExpanded)
		{
			_parameters = parameters;
			_rawParameters = parameters;
			ParamArrayIndex = paramArrayIndex;
			ParamArrayExpanded = paramArrayExpanded;
			AllNarrowingIsFromObject = true;
		}

		internal Method(MethodBase method, ParameterInfo[] parameters, int paramArrayIndex, bool paramArrayExpanded)
			: this(parameters, paramArrayIndex, paramArrayExpanded)
		{
			_item = method;
			_rawItem = method;
		}

		internal Method(PropertyInfo property, ParameterInfo[] parameters, int paramArrayIndex, bool paramArrayExpanded)
			: this(parameters, paramArrayIndex, paramArrayExpanded)
		{
			_item = property;
		}

		[RequiresUnreferencedCode("Cannot statically infer which method _rawItem is or if it has annotations")]
		internal bool BindGenericArguments()
		{
			try
			{
				_item = ((MethodInfo)_rawItem).MakeGenericMethod(TypeArguments);
				_parameters = AsMethod().GetParameters();
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		internal MethodBase AsMethod()
		{
			return _item as MethodBase;
		}

		internal PropertyInfo AsProperty()
		{
			return _item as PropertyInfo;
		}

		public static bool operator ==(Method left, Method right)
		{
			return (object)left._item == right._item;
		}

		public static bool operator !=(Method left, Method right)
		{
			return (object)left._item != right._item;
		}

		public static bool operator ==(MemberInfo left, Method right)
		{
			return (object)left == right._item;
		}

		public static bool operator !=(MemberInfo left, Method right)
		{
			return (object)left != right._item;
		}

		public override string ToString()
		{
			return Utils.MemberToString(_item);
		}
	}

	internal sealed class TypedNothing
	{
		internal readonly Type Type;

		internal TypedNothing(Type type)
		{
			Type = type;
		}
	}

	internal static readonly object[] NoArguments;

	internal static readonly string[] NoArgumentNames;

	internal static readonly Type[] NoTypeArguments;

	internal static readonly Type[] NoTypeParameters;

	internal static readonly string[] OperatorCLSNames;

	internal static readonly string[] OperatorNames;

	static Symbols()
	{
		NoArguments = Array.Empty<object>();
		NoArgumentNames = Array.Empty<string>();
		NoTypeArguments = Array.Empty<Type>();
		NoTypeParameters = Array.Empty<Type>();
		OperatorCLSNames = new string[28];
		OperatorCLSNames[1] = "op_Explicit";
		OperatorCLSNames[2] = "op_Implicit";
		OperatorCLSNames[3] = "op_True";
		OperatorCLSNames[4] = "op_False";
		OperatorCLSNames[5] = "op_UnaryNegation";
		OperatorCLSNames[6] = "op_OnesComplement";
		OperatorCLSNames[7] = "op_UnaryPlus";
		OperatorCLSNames[8] = "op_Addition";
		OperatorCLSNames[9] = "op_Subtraction";
		OperatorCLSNames[10] = "op_Multiply";
		OperatorCLSNames[11] = "op_Division";
		OperatorCLSNames[12] = "op_Exponent";
		OperatorCLSNames[13] = "op_IntegerDivision";
		OperatorCLSNames[14] = "op_Concatenate";
		OperatorCLSNames[15] = "op_LeftShift";
		OperatorCLSNames[16] = "op_RightShift";
		OperatorCLSNames[17] = "op_Modulus";
		OperatorCLSNames[18] = "op_BitwiseOr";
		OperatorCLSNames[19] = "op_ExclusiveOr";
		OperatorCLSNames[20] = "op_BitwiseAnd";
		OperatorCLSNames[21] = "op_Like";
		OperatorCLSNames[22] = "op_Equality";
		OperatorCLSNames[23] = "op_Inequality";
		OperatorCLSNames[24] = "op_LessThan";
		OperatorCLSNames[25] = "op_LessThanOrEqual";
		OperatorCLSNames[26] = "op_GreaterThanOrEqual";
		OperatorCLSNames[27] = "op_GreaterThan";
		OperatorNames = new string[28];
		OperatorNames[1] = "CType";
		OperatorNames[2] = "CType";
		OperatorNames[3] = "IsTrue";
		OperatorNames[4] = "IsFalse";
		OperatorNames[5] = "-";
		OperatorNames[6] = "Not";
		OperatorNames[7] = "+";
		OperatorNames[8] = "+";
		OperatorNames[9] = "-";
		OperatorNames[10] = "*";
		OperatorNames[11] = "/";
		OperatorNames[12] = "^";
		OperatorNames[13] = "\\";
		OperatorNames[14] = "&";
		OperatorNames[15] = "<<";
		OperatorNames[16] = ">>";
		OperatorNames[17] = "Mod";
		OperatorNames[18] = "Or";
		OperatorNames[19] = "Xor";
		OperatorNames[20] = "And";
		OperatorNames[21] = "Like";
		OperatorNames[22] = "=";
		OperatorNames[23] = "<>";
		OperatorNames[24] = "<";
		OperatorNames[25] = "<=";
		OperatorNames[26] = ">=";
		OperatorNames[27] = ">";
	}

	internal static bool IsUnaryOperator(UserDefinedOperator op)
	{
		UserDefinedOperator userDefinedOperator = op;
		if ((uint)(userDefinedOperator - 1) <= 6u)
		{
			return true;
		}
		return false;
	}

	internal static bool IsBinaryOperator(UserDefinedOperator op)
	{
		UserDefinedOperator userDefinedOperator = op;
		if ((uint)(userDefinedOperator - 8) <= 19u)
		{
			return true;
		}
		return false;
	}

	internal static bool IsUserDefinedOperator(MethodBase method)
	{
		if (method.IsSpecialName)
		{
			return method.Name.StartsWith("op_", StringComparison.Ordinal);
		}
		return false;
	}

	internal static bool IsNarrowingConversionOperator(MethodBase method)
	{
		if (method.IsSpecialName)
		{
			return method.Name.Equals(OperatorCLSNames[1]);
		}
		return false;
	}

	internal static UserDefinedOperator MapToUserDefinedOperator(MethodBase method)
	{
		int num = 1;
		do
		{
			if (method.Name.Equals(OperatorCLSNames[num]))
			{
				int num2 = method.GetParameters().Length;
				UserDefinedOperator userDefinedOperator = (UserDefinedOperator)checked((sbyte)num);
				if ((num2 == 1 && IsUnaryOperator(userDefinedOperator)) || (num2 == 2 && IsBinaryOperator(userDefinedOperator)))
				{
					return userDefinedOperator;
				}
			}
			num = checked(num + 1);
		}
		while (num <= 27);
		return UserDefinedOperator.UNDEF;
	}

	internal static TypeCode GetTypeCode(Type type)
	{
		return ReflectionExtensions.GetTypeCode(type);
	}

	[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
	internal static Type MapTypeCodeToType(TypeCode typeCode)
	{
		return typeCode switch
		{
			TypeCode.Boolean => typeof(bool), 
			TypeCode.SByte => typeof(sbyte), 
			TypeCode.Byte => typeof(byte), 
			TypeCode.Int16 => typeof(short), 
			TypeCode.UInt16 => typeof(ushort), 
			TypeCode.Int32 => typeof(int), 
			TypeCode.UInt32 => typeof(uint), 
			TypeCode.Int64 => typeof(long), 
			TypeCode.UInt64 => typeof(ulong), 
			TypeCode.Decimal => typeof(decimal), 
			TypeCode.Single => typeof(float), 
			TypeCode.Double => typeof(double), 
			TypeCode.DateTime => typeof(DateTime), 
			TypeCode.Char => typeof(char), 
			TypeCode.String => typeof(string), 
			TypeCode.Object => typeof(object), 
			TypeCode.DBNull => typeof(DBNull), 
			_ => null, 
		};
	}

	internal static bool IsRootObjectType(Type type)
	{
		return (object)type == typeof(object);
	}

	internal static bool IsRootEnumType(Type type)
	{
		return (object)type == typeof(Enum);
	}

	internal static bool IsValueType(Type type)
	{
		return type.IsValueType;
	}

	internal static bool IsEnum(Type type)
	{
		return type.IsEnum;
	}

	internal static bool IsArrayType(Type type)
	{
		return type.IsArray;
	}

	internal static bool IsStringType(Type type)
	{
		return (object)type == typeof(string);
	}

	internal static bool IsCharArrayRankOne(Type type)
	{
		return (object)type == typeof(char[]);
	}

	internal static bool IsIntegralType(TypeCode typeCode)
	{
		switch (typeCode)
		{
		case TypeCode.SByte:
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.UInt16:
		case TypeCode.Int32:
		case TypeCode.UInt32:
		case TypeCode.Int64:
		case TypeCode.UInt64:
			return true;
		default:
			return false;
		}
	}

	internal static bool IsNumericType(TypeCode typeCode)
	{
		switch (typeCode)
		{
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
			return true;
		default:
			return false;
		}
	}

	internal static bool IsNumericType(Type type)
	{
		return IsNumericType(GetTypeCode(type));
	}

	internal static bool IsIntrinsicType(TypeCode typeCode)
	{
		switch (typeCode)
		{
		case TypeCode.Boolean:
		case TypeCode.Char:
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
		case TypeCode.DateTime:
		case TypeCode.String:
			return true;
		default:
			return false;
		}
	}

	internal static bool IsIntrinsicType(Type type)
	{
		if (IsIntrinsicType(GetTypeCode(type)))
		{
			return !IsEnum(type);
		}
		return false;
	}

	internal static bool IsClass(Type type)
	{
		if (!type.IsClass)
		{
			return IsRootEnumType(type);
		}
		return true;
	}

	internal static bool IsClassOrValueType(Type type)
	{
		if (!IsValueType(type))
		{
			return IsClass(type);
		}
		return true;
	}

	internal static bool IsInterface(Type type)
	{
		return type.IsInterface;
	}

	internal static bool IsClassOrInterface(Type type)
	{
		if (!IsClass(type))
		{
			return IsInterface(type);
		}
		return true;
	}

	internal static bool IsReferenceType(Type type)
	{
		if (!IsClass(type))
		{
			return IsInterface(type);
		}
		return true;
	}

	internal static bool IsGenericParameter(Type type)
	{
		return type.IsGenericParameter;
	}

	internal static bool IsCollectionInterface(Type type)
	{
		if (type.IsInterface && ((type.IsGenericType && ((object)type.GetGenericTypeDefinition() == typeof(IList<>) || (object)type.GetGenericTypeDefinition() == typeof(ICollection<>) || (object)type.GetGenericTypeDefinition() == typeof(IEnumerable<>) || (object)type.GetGenericTypeDefinition() == typeof(IReadOnlyList<>) || (object)type.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>) || (object)type.GetGenericTypeDefinition() == typeof(IDictionary<, >) || (object)type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<, >))) || (object)type == typeof(IList) || (object)type == typeof(ICollection) || (object)type == typeof(IEnumerable) || (object)type == typeof(INotifyPropertyChanged) || (object)type == typeof(INotifyCollectionChanged)))
		{
			return true;
		}
		return false;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070:UnrecognizedReflectionPattern", Justification = "Calls GetInterfaces but only looks for existing interface type by reference equality. Trimmer guarantees that if the interface type is kept it is also kept on all the types which implement it.")]
	internal static bool Implements(Type implementor, Type @interface)
	{
		Type[] interfaces = implementor.GetInterfaces();
		for (int i = 0; i < interfaces.Length; i = checked(i + 1))
		{
			if ((object)interfaces[i] == @interface)
			{
				return true;
			}
		}
		return false;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070:UnrecognizedReflectionPattern", Justification = "Calls GetInterfaces but only looks for existing interface type by reference equality. Trimmer guarantees that if the interface type is kept it is also kept on all the types which implement it.")]
	internal static bool IsOrInheritsFrom(Type derived, Type @base)
	{
		if ((object)derived == @base)
		{
			return true;
		}
		checked
		{
			if (derived.IsGenericParameter)
			{
				if (IsClass(@base) && (derived.GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) != GenericParameterAttributes.None && IsOrInheritsFrom(typeof(ValueType), @base))
				{
					return true;
				}
				Type[] genericParameterConstraints = derived.GetGenericParameterConstraints();
				for (int i = 0; i < genericParameterConstraints.Length; i++)
				{
					if (IsOrInheritsFrom(genericParameterConstraints[i], @base))
					{
						return true;
					}
				}
			}
			else if (IsInterface(derived))
			{
				if (IsInterface(@base))
				{
					Type[] interfaces = derived.GetInterfaces();
					for (int j = 0; j < interfaces.Length; j++)
					{
						if ((object)interfaces[j] == @base)
						{
							return true;
						}
					}
				}
			}
			else if (IsClass(@base) && IsClassOrValueType(derived))
			{
				return derived.IsSubclassOf(@base);
			}
			return false;
		}
	}

	internal static bool IsGeneric(Type type)
	{
		return type.IsGenericType;
	}

	internal static bool IsInstantiatedGeneric(Type type)
	{
		if (type.IsGenericType)
		{
			return !type.IsGenericTypeDefinition;
		}
		return false;
	}

	internal static bool IsGeneric(MethodBase method)
	{
		return method.IsGenericMethod;
	}

	internal static bool IsGeneric(MemberInfo member)
	{
		if (!(member is MethodBase method))
		{
			return false;
		}
		return IsGeneric(method);
	}

	internal static bool IsRawGeneric(MethodBase method)
	{
		if (method.IsGenericMethod)
		{
			return method.IsGenericMethodDefinition;
		}
		return false;
	}

	internal static Type[] GetTypeParameters(MemberInfo member)
	{
		if (!(member is MethodBase methodBase))
		{
			return NoTypeParameters;
		}
		return methodBase.GetGenericArguments();
	}

	internal static Type[] GetTypeArguments(Type type)
	{
		return type.GetGenericArguments();
	}

	internal static Type[] GetInterfaceConstraints([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type genericParameter)
	{
		return genericParameter.GetInterfaces().ToArray();
	}

	[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
	internal static Type GetClassConstraint([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type genericParameter)
	{
		Type baseType = genericParameter.BaseType;
		if (IsRootObjectType(baseType))
		{
			return null;
		}
		return baseType;
	}

	internal static int IndexIn(Type possibleGenericParameter, MethodBase genericMethodDef)
	{
		if (IsGenericParameter(possibleGenericParameter) && (object)possibleGenericParameter.DeclaringMethod != null && AreGenericMethodDefsEqual(possibleGenericParameter.DeclaringMethod, genericMethodDef))
		{
			return possibleGenericParameter.GenericParameterPosition;
		}
		return -1;
	}

	internal static bool RefersToGenericParameter(Type referringType, MethodBase method)
	{
		if (!IsRawGeneric(method))
		{
			return false;
		}
		if (referringType.IsByRef)
		{
			referringType = GetElementType(referringType);
		}
		if (IsGenericParameter(referringType))
		{
			if (AreGenericMethodDefsEqual(referringType.DeclaringMethod, method))
			{
				return true;
			}
		}
		else if (IsGeneric(referringType))
		{
			Type[] typeArguments = GetTypeArguments(referringType);
			for (int i = 0; i < typeArguments.Length; i = checked(i + 1))
			{
				if (RefersToGenericParameter(typeArguments[i], method))
				{
					return true;
				}
			}
		}
		else if (IsArrayType(referringType))
		{
			return RefersToGenericParameter(referringType.GetElementType(), method);
		}
		return false;
	}

	internal static bool RefersToGenericParameterCLRSemantics(Type referringType, Type typ)
	{
		if (referringType.IsByRef)
		{
			referringType = GetElementType(referringType);
		}
		if (IsGenericParameter(referringType))
		{
			if ((object)referringType.DeclaringType == typ)
			{
				return true;
			}
		}
		else if (IsGeneric(referringType))
		{
			Type[] typeArguments = GetTypeArguments(referringType);
			for (int i = 0; i < typeArguments.Length; i = checked(i + 1))
			{
				if (RefersToGenericParameterCLRSemantics(typeArguments[i], typ))
				{
					return true;
				}
			}
		}
		else if (IsArrayType(referringType))
		{
			return RefersToGenericParameterCLRSemantics(referringType.GetElementType(), typ);
		}
		return false;
	}

	internal static bool AreGenericMethodDefsEqual(MethodBase method1, MethodBase method2)
	{
		if ((object)method1 != method2)
		{
			return method1.HasSameMetadataDefinitionAs(method2);
		}
		return true;
	}

	internal static bool IsShadows(MethodBase method)
	{
		if (method.IsHideBySig)
		{
			return false;
		}
		if (method.IsVirtual && (method.Attributes & MethodAttributes.VtableLayoutMask) == 0 && (((MethodInfo)method).GetRuntimeBaseDefinition().Attributes & MethodAttributes.VtableLayoutMask) == 0)
		{
			return false;
		}
		return true;
	}

	internal static bool IsShared(MemberInfo member)
	{
		return member.MemberType switch
		{
			MemberTypes.Method => ((MethodInfo)member).IsStatic, 
			MemberTypes.Field => ((FieldInfo)member).IsStatic, 
			MemberTypes.Constructor => ((ConstructorInfo)member).IsStatic, 
			MemberTypes.Property => ((PropertyInfo)member).GetGetMethod().IsStatic, 
			_ => false, 
		};
	}

	internal static bool IsParamArray(ParameterInfo parameter)
	{
		if (IsArrayType(parameter.ParameterType))
		{
			return parameter.IsDefined(typeof(ParamArrayAttribute), inherit: false);
		}
		return false;
	}

	internal static Type GetElementType(Type type)
	{
		return type.GetElementType();
	}

	internal static bool AreParametersAndReturnTypesValid(ParameterInfo[] parameters, Type returnType)
	{
		if ((object)returnType != null && (returnType.IsPointer || returnType.IsByRef))
		{
			return false;
		}
		if (parameters != null)
		{
			for (int i = 0; i < parameters.Length; i = checked(i + 1))
			{
				if (parameters[i].ParameterType.IsPointer)
				{
					return false;
				}
			}
		}
		return true;
	}

	internal static void GetAllParameterCounts(ParameterInfo[] parameters, ref int requiredParameterCount, ref int maximumParameterCount, ref int paramArrayIndex)
	{
		maximumParameterCount = parameters.Length;
		checked
		{
			for (int i = maximumParameterCount - 1; i >= 0; i += -1)
			{
				if (!parameters[i].IsOptional)
				{
					requiredParameterCount = i + 1;
					break;
				}
			}
			if (maximumParameterCount != 0 && IsParamArray(parameters[maximumParameterCount - 1]))
			{
				paramArrayIndex = maximumParameterCount - 1;
				requiredParameterCount--;
			}
		}
	}

	internal static bool IsNonPublicRuntimeMember(MemberInfo member)
	{
		Type declaringType = member.DeclaringType;
		if (!declaringType.IsPublic)
		{
			return (object)declaringType.Assembly == Utils.VBRuntimeAssembly;
		}
		return false;
	}

	internal static bool HasFlag(BindingFlags flags, BindingFlags flagToTest)
	{
		return (flags & flagToTest) != 0;
	}
}
