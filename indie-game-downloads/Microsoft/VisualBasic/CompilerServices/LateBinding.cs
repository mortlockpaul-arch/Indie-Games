using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class LateBinding
{
	private static MemberInfo GetMostDerivedMemberInfo([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] IReflect objIReflect, string name, BindingFlags flags)
	{
		MemberInfo[] nonGenericMembers = GetNonGenericMembers(objIReflect.GetMember(name, flags));
		if (nonGenericMembers == null || nonGenericMembers.Length == 0)
		{
			return null;
		}
		MemberInfo memberInfo = nonGenericMembers[0];
		int upperBound = nonGenericMembers.GetUpperBound(0);
		for (int i = 1; i <= upperBound; i = checked(i + 1))
		{
			if (nonGenericMembers[i].DeclaringType.IsSubclassOf(memberInfo.DeclaringType))
			{
				memberInfo = nonGenericMembers[i];
			}
		}
		return memberInfo;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static object LateGet(object o, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type objType, string name, object[] args, string[] paramnames, bool[] CopyBack)
	{
		BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.OptionalParamBinding;
		if ((object)objType == null)
		{
			if (o == null)
			{
				throw ExceptionUtils.VbMakeException(91);
			}
			objType = o.GetType();
		}
		IReflect correctIReflect = GetCorrectIReflect(o, objType);
		if (name == null)
		{
			name = "";
		}
		if (objType.IsCOMObject)
		{
			CheckForClassExtendingCOMClass(objType);
		}
		else
		{
			MemberInfo mostDerivedMemberInfo = GetMostDerivedMemberInfo(correctIReflect, name, bindingFlags | BindingFlags.GetField);
			if ((object)mostDerivedMemberInfo != null && mostDerivedMemberInfo.MemberType == MemberTypes.Field)
			{
				VBBinder.SecurityCheckForLateboundCalls(mostDerivedMemberInfo, objType, correctIReflect);
				object obj;
				if (objType == correctIReflect || ((FieldInfo)mostDerivedMemberInfo).IsStatic || DoesTargetObjectMatch(o, mostDerivedMemberInfo))
				{
					VerifyObjRefPresentForInstanceCall(o, mostDerivedMemberInfo);
					obj = ((FieldInfo)mostDerivedMemberInfo).GetValue(o);
				}
				else
				{
					obj = InvokeMemberOnIReflect(correctIReflect, mostDerivedMemberInfo, BindingFlags.GetField, o, null);
				}
				if (args == null || args.Length == 0)
				{
					return obj;
				}
				return LateIndexGet(obj, args, paramnames);
			}
		}
		VBBinder vBBinder = new VBBinder(CopyBack);
		try
		{
			return vBBinder.InvokeMember(name, bindingFlags, objType, correctIReflect, o, args, paramnames);
		}
		catch (Exception ex) when (IsMissingMemberException(ex))
		{
			if (objType.IsCOMObject || (args != null && args.Length > 0))
			{
				bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.OptionalParamBinding;
				if (!objType.IsCOMObject)
				{
					bindingFlags |= BindingFlags.GetField;
				}
				object obj2;
				try
				{
					obj2 = vBBinder.InvokeMember(name, bindingFlags, objType, correctIReflect, o, null, null);
				}
				catch (AccessViolationException ex2)
				{
					throw ex2;
				}
				catch (StackOverflowException ex3)
				{
					throw ex3;
				}
				catch (OutOfMemoryException ex4)
				{
					throw ex4;
				}
				catch (Exception)
				{
					obj2 = null;
				}
				if (obj2 == null)
				{
					throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_MemberNotFoundOnType2, name, Utils.VBFriendlyName(objType, o)));
				}
				try
				{
					return LateIndexGet(obj2, args, paramnames);
				}
				catch (Exception ex6) when (IsMissingMemberException(ex6) && ex is MissingMemberException)
				{
					throw ex;
				}
			}
			throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_MemberNotFoundOnType2, name, Utils.VBFriendlyName(objType, o)));
		}
		catch (TargetInvocationException ex7)
		{
			throw ex7.InnerException;
		}
	}

	private static bool IsMissingMemberException(Exception ex)
	{
		if (ex is MissingMemberException)
		{
			return true;
		}
		if (ex is MemberAccessException)
		{
			return true;
		}
		if (ex is COMException ex2)
		{
			if (ex2.ErrorCode == -2147352570)
			{
				return true;
			}
			if (ex2.ErrorCode == -2146827850)
			{
				return true;
			}
		}
		else if (ex is TargetInvocationException && ex.InnerException is COMException && ((COMException)ex.InnerException).ErrorCode == -2147352559)
		{
			return true;
		}
		return false;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void LateSetComplex(object o, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type objType, string name, object[] args, string[] paramnames, bool OptimisticSet, bool RValueBase)
	{
		try
		{
			InternalLateSet(o, ref objType, name, args, paramnames, OptimisticSet, (CallType)0);
			if (RValueBase && objType.IsValueType)
			{
				throw new Exception(System.SR.Format(System.SR.RValueBaseForValueType, Utils.VBFriendlyName(objType, o), Utils.VBFriendlyName(objType, o)));
			}
		}
		catch (MissingMemberException) when (OptimisticSet)
		{
		}
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void LateSet(object o, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type objType, string name, object[] args, string[] paramnames)
	{
		InternalLateSet(o, ref objType, name, args, paramnames, OptimisticSet: false, (CallType)0);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	internal static void InternalLateSet(object o, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] ref Type objType, string name, object[] args, string[] paramnames, bool OptimisticSet, CallType UseCallType)
	{
		BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.OptionalParamBinding;
		if ((object)objType == null)
		{
			if (o == null)
			{
				throw ExceptionUtils.VbMakeException(91);
			}
			objType = o.GetType();
		}
		IReflect correctIReflect = GetCorrectIReflect(o, objType);
		if (name == null)
		{
			name = "";
		}
		if (objType.IsCOMObject)
		{
			CheckForClassExtendingCOMClass(objType);
			switch (UseCallType)
			{
			case CallType.Set:
				bindingFlags |= BindingFlags.PutRefDispProperty;
				if (args[args.GetUpperBound(0)] == null)
				{
					args[args.GetUpperBound(0)] = new DispatchWrapper(null);
				}
				break;
			case CallType.Let:
				bindingFlags |= BindingFlags.PutDispProperty;
				break;
			default:
				bindingFlags |= GetPropertyPutFlags(args[args.GetUpperBound(0)]);
				break;
			}
		}
		else
		{
			bindingFlags |= BindingFlags.SetProperty;
			MemberInfo mostDerivedMemberInfo = GetMostDerivedMemberInfo(correctIReflect, name, bindingFlags | BindingFlags.SetField);
			if ((object)mostDerivedMemberInfo != null && mostDerivedMemberInfo.MemberType == MemberTypes.Field)
			{
				FieldInfo fieldInfo = (FieldInfo)mostDerivedMemberInfo;
				if (fieldInfo.IsInitOnly)
				{
					throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_ReadOnlyField2, name, Utils.VBFriendlyName(objType, o)));
				}
				if (args == null || args.Length == 0)
				{
					throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_MemberNotFoundOnType2, name, Utils.VBFriendlyName(objType, o)));
				}
				if (args.Length == 1)
				{
					object obj = args[0];
					VBBinder.SecurityCheckForLateboundCalls(fieldInfo, objType, correctIReflect);
					object obj2 = ((obj != null) ? ObjectType.CTypeHelper(args[0], fieldInfo.FieldType) : null);
					if (objType == correctIReflect || fieldInfo.IsStatic || DoesTargetObjectMatch(o, fieldInfo))
					{
						VerifyObjRefPresentForInstanceCall(o, fieldInfo);
						fieldInfo.SetValue(o, obj2);
					}
					else
					{
						InvokeMemberOnIReflect(correctIReflect, fieldInfo, BindingFlags.SetField, o, new object[1] { obj2 });
					}
					return;
				}
				if (args.Length > 1)
				{
					VBBinder.SecurityCheckForLateboundCalls(mostDerivedMemberInfo, objType, correctIReflect);
					object obj3 = null;
					if (objType == correctIReflect || ((FieldInfo)mostDerivedMemberInfo).IsStatic || DoesTargetObjectMatch(o, mostDerivedMemberInfo))
					{
						VerifyObjRefPresentForInstanceCall(o, mostDerivedMemberInfo);
						obj3 = ((FieldInfo)mostDerivedMemberInfo).GetValue(o);
					}
					else
					{
						obj3 = InvokeMemberOnIReflect(correctIReflect, mostDerivedMemberInfo, BindingFlags.GetField, o, new object[1] { obj3 });
					}
					LateIndexSet(obj3, args, paramnames);
					return;
				}
			}
		}
		VBBinder vBBinder = new VBBinder(null);
		checked
		{
			if (OptimisticSet && args.GetUpperBound(0) > 0)
			{
				BindingFlags bindingAttr = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty | BindingFlags.OptionalParamBinding;
				Type[] array = new Type[args.GetUpperBound(0) - 1 + 1];
				int upperBound = array.GetUpperBound(0);
				for (int i = 0; i <= upperBound; i++)
				{
					object obj4 = args[i];
					if (obj4 == null)
					{
						array[i] = null;
					}
					else
					{
						array[i] = obj4.GetType();
					}
				}
				try
				{
					PropertyInfo property = correctIReflect.GetProperty(name, bindingAttr, vBBinder, typeof(int), array, null);
					if ((object)property == null || !property.CanWrite)
					{
						return;
					}
				}
				catch (MissingMemberException)
				{
					return;
				}
			}
			try
			{
				vBBinder.InvokeMember(name, bindingFlags, objType, correctIReflect, o, args, paramnames);
			}
			catch (Exception ex2) when (IsMissingMemberException(ex2))
			{
				if (args != null && args.Length > 1)
				{
					bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty | BindingFlags.OptionalParamBinding;
					if (!objType.IsCOMObject)
					{
						bindingFlags |= BindingFlags.GetField;
					}
					object obj5;
					try
					{
						obj5 = vBBinder.InvokeMember(name, bindingFlags, objType, correctIReflect, o, null, null);
					}
					catch (Exception ex3) when (IsMissingMemberException(ex3) && ex2 is MissingMemberException)
					{
						throw ex2;
					}
					catch (AccessViolationException ex4)
					{
						throw ex4;
					}
					catch (StackOverflowException ex5)
					{
						throw ex5;
					}
					catch (OutOfMemoryException ex6)
					{
						throw ex6;
					}
					catch (Exception)
					{
						obj5 = null;
					}
					if (obj5 == null)
					{
						throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_MemberNotFoundOnType2, name, Utils.VBFriendlyName(objType, o)));
					}
					try
					{
						LateIndexSet(obj5, args, paramnames);
						return;
					}
					catch (Exception ex8) when (IsMissingMemberException(ex8) && ex2 is MissingMemberException)
					{
						throw ex2;
					}
				}
				throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_MemberNotFoundOnType2, name, Utils.VBFriendlyName(objType, o)));
			}
			catch (TargetInvocationException ex9)
			{
				if (ex9.InnerException == null)
				{
					throw ex9;
				}
				if (ex9.InnerException is TargetParameterCountException)
				{
					if ((bindingFlags & BindingFlags.PutRefDispProperty) != BindingFlags.Default)
					{
						throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_MemberSetNotFoundOnType2, name, Utils.VBFriendlyName(objType, o)));
					}
					throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_MemberLetNotFoundOnType2, name, Utils.VBFriendlyName(objType, o)));
				}
				throw ex9.InnerException;
			}
		}
	}

	private static void CheckForClassExtendingCOMClass(Type objType)
	{
		if (objType.IsCOMObject && Operators.CompareString(objType.FullName, "System.__ComObject", TextCompare: false) != 0 && Operators.CompareString(objType.BaseType.FullName, "System.__ComObject", TextCompare: false) != 0)
		{
			throw new InvalidOperationException(System.SR.LateboundCallToInheritedComClass);
		}
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static object LateIndexGet(object o, object[] args, string[] paramnames)
	{
		string DefaultName = null;
		if (o == null)
		{
			throw ExceptionUtils.VbMakeException(91);
		}
		Type type = o.GetType();
		IReflect correctIReflect = GetCorrectIReflect(o, type);
		checked
		{
			if (type.IsArray)
			{
				if (paramnames != null && paramnames.Length != 0)
				{
					throw new ArgumentException(System.SR.Argument_InvalidNamedArgs);
				}
				Array array = (Array)o;
				int num = args.Length;
				if (num != array.Rank)
				{
					throw new RankException();
				}
				switch (num)
				{
				case 1:
					return array.GetValue(Conversions.ToInteger(args[0]));
				case 2:
					return array.GetValue(Conversions.ToInteger(args[0]), Conversions.ToInteger(args[1]));
				default:
				{
					int[] array2 = new int[num - 1 + 1];
					int num2 = num - 1;
					for (int i = 0; i <= num2; i++)
					{
						array2[i] = Conversions.ToInteger(args[i]);
					}
					return array.GetValue(array2);
				}
				}
			}
			MethodBase[] array3 = null;
			BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.OptionalParamBinding;
			if (!type.IsCOMObject)
			{
				if (args == null || args.Length == 0)
				{
					bindingFlags |= BindingFlags.GetField;
				}
				MemberInfo[] defaultMembers = GetDefaultMembers(type, correctIReflect, ref DefaultName);
				int num3 = default(int);
				if (defaultMembers != null)
				{
					int upperBound = defaultMembers.GetUpperBound(0);
					for (int j = 0; j <= upperBound; j++)
					{
						MemberInfo memberInfo = defaultMembers[j];
						if (memberInfo.MemberType == MemberTypes.Property)
						{
							memberInfo = ((PropertyInfo)memberInfo).GetGetMethod();
						}
						if ((object)memberInfo != null && memberInfo.MemberType != MemberTypes.Field)
						{
							defaultMembers[num3] = memberInfo;
							num3++;
						}
					}
				}
				if ((defaultMembers == null) | (num3 == 0))
				{
					throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_NoDefaultMemberFound1, Utils.VBFriendlyName(type, o)));
				}
				array3 = new MethodBase[num3 - 1 + 1];
				int num4 = num3 - 1;
				for (int j = 0; j <= num4; j++)
				{
					try
					{
						array3[j] = (MethodBase)defaultMembers[j];
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
					}
				}
			}
			else
			{
				CheckForClassExtendingCOMClass(type);
			}
			VBBinder vBBinder = new VBBinder(null);
			try
			{
				if (type.IsCOMObject)
				{
					return vBBinder.InvokeMember("", bindingFlags, type, correctIReflect, o, args, paramnames);
				}
				object ObjState = null;
				vBBinder.m_BindToName = DefaultName;
				vBBinder.m_objType = type;
				MethodBase methodBase = vBBinder.BindToMethod(bindingFlags, array3, ref args, null, null, paramnames, ref ObjState);
				VBBinder.SecurityCheckForLateboundCalls(methodBase, type, correctIReflect);
				object result;
				if (type == correctIReflect || methodBase.IsStatic || DoesTargetObjectMatch(o, methodBase))
				{
					VerifyObjRefPresentForInstanceCall(o, methodBase);
					result = methodBase.Invoke(o, args);
				}
				else
				{
					result = InvokeMemberOnIReflect(correctIReflect, methodBase, BindingFlags.InvokeMethod, o, args);
				}
				vBBinder.ReorderArgumentArray(ref args, ObjState);
				return result;
			}
			catch (Exception ex4) when (IsMissingMemberException(ex4))
			{
				throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_NoDefaultMemberFound1, Utils.VBFriendlyName(type, o)));
			}
			catch (TargetInvocationException ex5)
			{
				throw ex5.InnerException;
			}
		}
	}

	private static MemberInfo[] GetDefaultMembers([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type typ, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] IReflect objIReflect, ref string DefaultName)
	{
		MemberInfo[] member;
		if (typ == objIReflect)
		{
			do
			{
				object[] customAttributes = typ.GetCustomAttributes(typeof(DefaultMemberAttribute), inherit: false);
				if (customAttributes != null && customAttributes.Length != 0)
				{
					DefaultName = ((DefaultMemberAttribute)customAttributes[0]).MemberName;
					member = typ.GetMember(DefaultName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					member = GetNonGenericMembers(member);
					if (member == null || member.Length == 0)
					{
						DefaultName = "";
						return null;
					}
					return member;
				}
				typ = typ.BaseType;
			}
			while ((object)typ != null);
			DefaultName = "";
			return null;
		}
		member = objIReflect.GetMember("", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		member = GetNonGenericMembers(member);
		if (member == null || member.Length == 0)
		{
			DefaultName = "";
			return null;
		}
		DefaultName = member[0].Name;
		return member;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void LateIndexSetComplex(object o, object[] args, string[] paramnames, bool OptimisticSet, bool RValueBase)
	{
		try
		{
			LateIndexSet(o, args, paramnames);
			if (RValueBase && o.GetType().IsValueType)
			{
				throw new Exception(System.SR.Format(System.SR.RValueBaseForValueType, o.GetType().Name, o.GetType().Name));
			}
		}
		catch (MissingMemberException) when (OptimisticSet)
		{
		}
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void LateIndexSet(object o, object[] args, string[] paramnames)
	{
		string DefaultName = null;
		if (o == null)
		{
			throw ExceptionUtils.VbMakeException(91);
		}
		Type type = o.GetType();
		IReflect correctIReflect = GetCorrectIReflect(o, type);
		checked
		{
			if (type.IsArray)
			{
				if (paramnames != null && paramnames.Length != 0)
				{
					throw new ArgumentException(System.SR.Argument_InvalidNamedArgs);
				}
				Array array = (Array)o;
				int num = args.Length - 1;
				object obj = args[num];
				if (obj != null)
				{
					Type elementType = type.GetElementType();
					if ((object)obj.GetType() != elementType)
					{
						obj = ObjectType.CTypeHelper(obj, elementType);
					}
				}
				if (num != array.Rank)
				{
					throw new RankException();
				}
				switch (num)
				{
				case 1:
					array.SetValue(obj, Conversions.ToInteger(args[0]));
					return;
				case 2:
					array.SetValue(obj, Conversions.ToInteger(args[0]), Conversions.ToInteger(args[1]));
					return;
				}
				int[] array2 = new int[num - 1 + 1];
				int num2 = num - 1;
				for (int i = 0; i <= num2; i++)
				{
					array2[i] = Conversions.ToInteger(args[i]);
				}
				array.SetValue(obj, array2);
				return;
			}
			MethodBase[] array3 = null;
			BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.OptionalParamBinding;
			if (type.IsCOMObject)
			{
				CheckForClassExtendingCOMClass(type);
				bindingFlags |= GetPropertyPutFlags(args[args.GetUpperBound(0)]);
			}
			else
			{
				bindingFlags |= BindingFlags.SetProperty;
				if (args.Length == 1)
				{
					bindingFlags |= BindingFlags.SetField;
				}
				MemberInfo[] defaultMembers = GetDefaultMembers(type, correctIReflect, ref DefaultName);
				int num3 = default(int);
				if (defaultMembers != null)
				{
					int upperBound = defaultMembers.GetUpperBound(0);
					for (int j = 0; j <= upperBound; j++)
					{
						MemberInfo memberInfo = defaultMembers[j];
						if (memberInfo.MemberType == MemberTypes.Property)
						{
							memberInfo = ((PropertyInfo)memberInfo).GetSetMethod();
						}
						if ((object)memberInfo != null && memberInfo.MemberType != MemberTypes.Field)
						{
							defaultMembers[num3] = memberInfo;
							num3++;
						}
					}
				}
				if ((defaultMembers == null) | (num3 == 0))
				{
					throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_NoDefaultMemberFound1, Utils.VBFriendlyName(type, o)));
				}
				array3 = new MethodBase[num3 - 1 + 1];
				int num4 = num3 - 1;
				for (int j = 0; j <= num4; j++)
				{
					try
					{
						array3[j] = (MethodBase)defaultMembers[j];
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
					}
				}
			}
			VBBinder vBBinder = new VBBinder(null);
			try
			{
				if (type.IsCOMObject)
				{
					vBBinder.InvokeMember("", bindingFlags, type, correctIReflect, o, args, paramnames);
					return;
				}
				object ObjState = null;
				vBBinder.m_BindToName = DefaultName;
				vBBinder.m_objType = type;
				MethodBase methodBase = vBBinder.BindToMethod(bindingFlags, array3, ref args, null, null, paramnames, ref ObjState);
				VBBinder.SecurityCheckForLateboundCalls(methodBase, type, correctIReflect);
				if (type == correctIReflect || methodBase.IsStatic || DoesTargetObjectMatch(o, methodBase))
				{
					VerifyObjRefPresentForInstanceCall(o, methodBase);
					methodBase.Invoke(o, args);
				}
				else
				{
					InvokeMemberOnIReflect(correctIReflect, methodBase, BindingFlags.InvokeMethod, o, args);
				}
				vBBinder.ReorderArgumentArray(ref args, ObjState);
			}
			catch (Exception ex4) when (IsMissingMemberException(ex4))
			{
				throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_NoDefaultMemberFound1, Utils.VBFriendlyName(type, o)));
			}
			catch (TargetInvocationException ex5)
			{
				throw ex5.InnerException;
			}
		}
	}

	private static BindingFlags GetPropertyPutFlags(object NewValue)
	{
		if (NewValue == null)
		{
			return BindingFlags.SetProperty;
		}
		if (NewValue is ValueType || NewValue is string || NewValue is DBNull || NewValue is Missing || NewValue is Array || NewValue is CurrencyWrapper)
		{
			return BindingFlags.PutDispProperty;
		}
		return BindingFlags.PutRefDispProperty;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void LateCall(object o, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type objType, string name, object[] args, string[] paramnames, bool[] CopyBack)
	{
		InternalLateCall(o, objType, name, args, paramnames, CopyBack, IgnoreReturn: true);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	internal static object InternalLateCall(object o, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type objType, string name, object[] args, string[] paramnames, bool[] CopyBack, bool IgnoreReturn)
	{
		BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.OptionalParamBinding;
		if (IgnoreReturn)
		{
			bindingFlags |= BindingFlags.IgnoreReturn;
		}
		if ((object)objType == null)
		{
			if (o == null)
			{
				throw ExceptionUtils.VbMakeException(91);
			}
			objType = o.GetType();
		}
		IReflect correctIReflect = GetCorrectIReflect(o, objType);
		if (objType.IsCOMObject)
		{
			CheckForClassExtendingCOMClass(objType);
		}
		if (name == null)
		{
			name = "";
		}
		VBBinder vBBinder = new VBBinder(CopyBack);
		if (!objType.IsCOMObject)
		{
			MemberInfo[] membersByName = GetMembersByName(correctIReflect, name, bindingFlags);
			if (membersByName == null || membersByName.Length == 0)
			{
				throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_MemberNotFoundOnType2, name, Utils.VBFriendlyName(objType, o)));
			}
			if (MemberIsField(membersByName))
			{
				throw new ArgumentException(System.SR.Format(System.SR.ExpressionNotProcedure, name, Utils.VBFriendlyName(objType, o)));
			}
			if (membersByName.Length == 1 && (paramnames == null || paramnames.Length == 0))
			{
				MemberInfo memberInfo = membersByName[0];
				if (memberInfo.MemberType == MemberTypes.Property)
				{
					memberInfo = ((PropertyInfo)memberInfo).GetGetMethod();
					if ((object)memberInfo == null)
					{
						throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_MemberNotFoundOnType2, name, Utils.VBFriendlyName(objType, o)));
					}
				}
				MethodBase methodBase = (MethodBase)memberInfo;
				ParameterInfo[] parameters = methodBase.GetParameters();
				int num = args.Length;
				int num2 = parameters.Length;
				if (num2 == num)
				{
					if (num2 == 0)
					{
						return FastCall(o, methodBase, parameters, args, objType, correctIReflect);
					}
					if (CopyBack == null && NoByrefs(parameters))
					{
						ParameterInfo parameterInfo = parameters[checked(num2 - 1)];
						if (!parameterInfo.ParameterType.IsArray)
						{
							return FastCall(o, methodBase, parameters, args, objType, correctIReflect);
						}
						object[] customAttributes = parameterInfo.GetCustomAttributes(typeof(ParamArrayAttribute), inherit: false);
						if (customAttributes == null || customAttributes.Length == 0)
						{
							return FastCall(o, methodBase, parameters, args, objType, correctIReflect);
						}
					}
				}
			}
		}
		try
		{
			return vBBinder.InvokeMember(name, bindingFlags, objType, correctIReflect, o, args, paramnames);
		}
		catch (MissingMemberException)
		{
			throw;
		}
		catch (Exception ex2) when (IsMissingMemberException(ex2))
		{
			throw new MissingMemberException(System.SR.Format(System.SR.MissingMember_MemberNotFoundOnType2, name, Utils.VBFriendlyName(objType, o)));
		}
		catch (TargetInvocationException ex3)
		{
			throw ex3.InnerException;
		}
	}

	private static bool NoByrefs(ParameterInfo[] parameters)
	{
		checked
		{
			int num = parameters.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (parameters[i].ParameterType.IsByRef)
				{
					return false;
				}
			}
			return true;
		}
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	private static object FastCall(object o, MethodBase method, ParameterInfo[] Parameters, object[] args, Type objType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] IReflect objIReflect)
	{
		int upperBound = args.GetUpperBound(0);
		for (int i = 0; i <= upperBound; i = checked(i + 1))
		{
			ParameterInfo parameterInfo = Parameters[i];
			object obj = args[i];
			if (obj is Missing && parameterInfo.IsOptional)
			{
				obj = parameterInfo.DefaultValue;
			}
			args[i] = ObjectType.CTypeHelper(obj, parameterInfo.ParameterType);
		}
		VBBinder.SecurityCheckForLateboundCalls(method, objType, objIReflect);
		if (objType == objIReflect || method.IsStatic || DoesTargetObjectMatch(o, method))
		{
			VerifyObjRefPresentForInstanceCall(o, method);
			return method.Invoke(o, args);
		}
		return InvokeMemberOnIReflect(objIReflect, method, BindingFlags.InvokeMethod, o, args);
	}

	private static MemberInfo[] GetMembersByName([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] IReflect objIReflect, string name, BindingFlags flags)
	{
		MemberInfo[] array = GetNonGenericMembers(objIReflect.GetMember(name, flags));
		if (array != null && array.Length == 0)
		{
			array = null;
		}
		return array;
	}

	private static bool MemberIsField(MemberInfo[] mi)
	{
		int upperBound = mi.GetUpperBound(0);
		checked
		{
			for (int i = 0; i <= upperBound; i++)
			{
				MemberInfo memberInfo = mi[i];
				if ((object)memberInfo == null || memberInfo.MemberType != MemberTypes.Field)
				{
					continue;
				}
				int upperBound2 = mi.GetUpperBound(0);
				for (int j = 0; j <= upperBound2; j++)
				{
					if (i != j && (object)mi[j] != null && memberInfo.DeclaringType.IsSubclassOf(mi[j].DeclaringType))
					{
						mi[j] = null;
					}
				}
			}
			foreach (MemberInfo memberInfo in mi)
			{
				if ((object)memberInfo != null && memberInfo.MemberType != MemberTypes.Field)
				{
					return false;
				}
			}
			return true;
		}
	}

	internal static bool DoesTargetObjectMatch(object Value, MemberInfo Member)
	{
		if (Value == null || Member.DeclaringType.IsAssignableFrom(Value.GetType()))
		{
			return true;
		}
		return false;
	}

	internal static object InvokeMemberOnIReflect([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] IReflect objIReflect, MemberInfo member, BindingFlags flags, object target, object[] args)
	{
		VBBinder vBBinder = new VBBinder(null);
		vBBinder.CacheMember(member);
		return objIReflect.InvokeMember(member.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.OptionalParamBinding | flags, vBBinder, target, args, null, null, null);
	}

	[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
	private static IReflect GetCorrectIReflect(object o, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type objType)
	{
		return objType;
	}

	internal static void VerifyObjRefPresentForInstanceCall(object Value, MemberInfo Member)
	{
		if (Value == null)
		{
			bool flag = true;
			switch (Member.MemberType)
			{
			case MemberTypes.Method:
				flag = ((MethodInfo)Member).IsStatic;
				break;
			case MemberTypes.Field:
				flag = ((FieldInfo)Member).IsStatic;
				break;
			case MemberTypes.Constructor:
				flag = ((ConstructorInfo)Member).IsStatic;
				break;
			}
			if (!flag)
			{
				throw new NullReferenceException(System.SR.Format(System.SR.NullReference_InstanceReqToAccessMember1, Utils.MemberToString(Member)));
			}
		}
	}

	internal static MemberInfo[] GetNonGenericMembers(MemberInfo[] Members)
	{
		checked
		{
			if (Members != null && Members.Length > 0)
			{
				int num = 0;
				int upperBound = Members.GetUpperBound(0);
				for (int i = 0; i <= upperBound; i++)
				{
					if (LegacyIsGeneric(Members[i]))
					{
						Members[i] = null;
					}
					else
					{
						num++;
					}
				}
				if (num == Members.GetUpperBound(0) + 1)
				{
					return Members;
				}
				if (num > 0)
				{
					MemberInfo[] array = new MemberInfo[num - 1 + 1];
					int num2 = 0;
					int upperBound2 = Members.GetUpperBound(0);
					for (int j = 0; j <= upperBound2; j++)
					{
						if ((object)Members[j] != null)
						{
							array[num2] = Members[j];
							num2++;
						}
					}
					return array;
				}
			}
			return null;
		}
	}

	internal static bool LegacyIsGeneric(MemberInfo Member)
	{
		if (!(Member is MethodBase { IsGenericMethod: var isGenericMethod }))
		{
			return false;
		}
		return isGenericMethod;
	}
}
