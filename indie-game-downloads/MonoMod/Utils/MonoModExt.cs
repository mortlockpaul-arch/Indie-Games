using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace MonoMod.Utils;

[MonoMod__OldName__("MonoMod.MonoModExt")]
public static class MonoModExt
{
	public static IDictionary<string, object> SharedData = new Dictionary<string, object>
	{
		{
			"Platform",
			(PlatformHelper.Current & ~Platform.X64).ToString()
		},
		{
			"PlatformPrefix",
			(PlatformHelper.Current & ~Platform.X64).ToString().ToLowerInvariant() + "_"
		},
		{
			"Arch",
			(PlatformHelper.Current & Platform.X64).ToString()
		},
		{
			"Architecture",
			(PlatformHelper.Current & Platform.X64).ToString()
		},
		{
			"ArchPrefix",
			(PlatformHelper.Current & Platform.X64).ToString().ToLowerInvariant() + "_"
		},
		{
			"ArchitecturePrefix",
			(PlatformHelper.Current & Platform.X64).ToString().ToLowerInvariant() + "_"
		}
	};

	private static readonly Regex TypeGenericParamRegex = new Regex("\\!\\d");

	private static readonly Regex MethodGenericParamRegex = new Regex("\\!\\!\\d");

	private static Type t_ParamArrayAttribute = typeof(ParamArrayAttribute);

	public static readonly FieldInfo f_GenericParameter_position = typeof(GenericParameter).GetField("position", BindingFlags.Instance | BindingFlags.NonPublic);

	public static readonly FieldInfo f_GenericParameter_type = typeof(GenericParameter).GetField("type", BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly Type t_Code = typeof(Code);

	private static readonly Type t_OpCodes = typeof(OpCodes);

	private static readonly Dictionary<int, OpCode> _ShortToLongOp = new Dictionary<int, OpCode>();

	private static readonly Dictionary<int, OpCode> _LongToShortOp = new Dictionary<int, OpCode>();

	public static ModuleDefinition ReadModule(string path, ReaderParameters rp)
	{
		while (true)
		{
			try
			{
				return ModuleDefinition.ReadModule(path, rp);
			}
			catch
			{
				if (rp.ReadSymbols)
				{
					rp.ReadSymbols = false;
					continue;
				}
				throw;
			}
		}
	}

	public static ModuleDefinition ReadModule(Stream input, ReaderParameters rp)
	{
		while (true)
		{
			try
			{
				return ModuleDefinition.ReadModule(input, rp);
			}
			catch
			{
				if (rp.ReadSymbols)
				{
					rp.ReadSymbols = false;
					continue;
				}
				throw;
			}
		}
	}

	public static Mono.Cecil.Cil.MethodBody Clone(this Mono.Cecil.Cil.MethodBody o, MethodDefinition m)
	{
		if (o == null)
		{
			return null;
		}
		Mono.Cecil.Cil.MethodBody obj = new Mono.Cecil.Cil.MethodBody(m)
		{
			MaxStackSize = o.MaxStackSize,
			InitLocals = o.InitLocals,
			LocalVarToken = o.LocalVarToken
		};
		obj.Instructions.AddRange(o.Instructions);
		obj.ExceptionHandlers.AddRange(o.ExceptionHandlers);
		obj.Variables.AddRange(o.Variables);
		m.CustomDebugInformations.AddRange(o.Method.CustomDebugInformations);
		m.DebugInformation.SequencePoints.AddRange(o.Method.DebugInformation.SequencePoints);
		return obj;
	}

	public static GenericParameter Update(this GenericParameter param, GenericParameter other)
	{
		return param.Update(other.Position, other.Type);
	}

	public static GenericParameter Update(this GenericParameter param, int position, GenericParameterType type)
	{
		f_GenericParameter_position.SetValue(param, position);
		f_GenericParameter_type.SetValue(param, type);
		return param;
	}

	public static void AddAttribute(this Mono.Cecil.ICustomAttributeProvider cap, MethodReference constructor)
	{
		cap.AddAttribute(new CustomAttribute(constructor));
	}

	public static void AddAttribute(this Mono.Cecil.ICustomAttributeProvider cap, CustomAttribute attr)
	{
		cap.CustomAttributes.Add(attr);
	}

	public static bool HasMMAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
	{
		return cap.HasCustomAttribute("MonoMod.MonoMod" + attribute);
	}

	public static CustomAttribute GetMMAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
	{
		return cap.GetCustomAttribute("MonoMod.MonoMod" + attribute);
	}

	public static CustomAttribute GetNextMMAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
	{
		return cap.GetNextCustomAttribute("MonoMod.MonoMod" + attribute);
	}

	public static CustomAttribute GetCustomAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
	{
		if (cap == null || !cap.HasCustomAttributes)
		{
			return null;
		}
		foreach (CustomAttribute customAttribute in cap.CustomAttributes)
		{
			if (customAttribute.AttributeType.FullName == attribute)
			{
				return customAttribute;
			}
		}
		return null;
	}

	public static CustomAttribute GetNextCustomAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
	{
		if (cap == null || !cap.HasCustomAttributes)
		{
			return null;
		}
		bool flag = false;
		for (int i = 0; i < cap.CustomAttributes.Count; i++)
		{
			CustomAttribute customAttribute = cap.CustomAttributes[i];
			if (!(customAttribute.AttributeType.FullName != attribute))
			{
				if (flag)
				{
					return customAttribute;
				}
				cap.CustomAttributes.RemoveAt(i);
				i--;
				flag = true;
			}
		}
		return null;
	}

	public static bool HasCustomAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
	{
		return cap.GetCustomAttribute(attribute) != null;
	}

	public static string GetOriginalName(this MethodDefinition method)
	{
		foreach (CustomAttribute customAttribute in method.CustomAttributes)
		{
			if (customAttribute.AttributeType.FullName == "MonoMod.MonoModOriginalName")
			{
				return (string)customAttribute.ConstructorArguments[0].Value;
			}
		}
		if (method.HasMMAttribute("Constructor"))
		{
			return "orig_ctor_" + ((Mono.Cecil.ICustomAttributeProvider)method.DeclaringType).GetPatchName();
		}
		return "orig_" + method.Name;
	}

	public static bool MatchingConditionals(this Mono.Cecil.ICustomAttributeProvider cap, ModuleDefinition module)
	{
		return cap.MatchingConditionals(module.Assembly.Name);
	}

	public static bool MatchingConditionals(this Mono.Cecil.ICustomAttributeProvider cap, AssemblyNameReference asmName = null)
	{
		if (cap == null)
		{
			return true;
		}
		if (!cap.HasCustomAttributes)
		{
			return true;
		}
		_ = PlatformHelper.Current;
		bool flag = true;
		foreach (CustomAttribute customAttribute in cap.CustomAttributes)
		{
			if (customAttribute.AttributeType.FullName == "MonoMod.MonoModOnPlatform")
			{
				CustomAttributeArgument[] array = (CustomAttributeArgument[])customAttribute.ConstructorArguments[0].Value;
				for (int i = 0; i < array.Length; i++)
				{
					PlatformHelper.Is((Platform)array[i].Value);
				}
				flag &= array.Length == 0;
			}
			else if (customAttribute.AttributeType.FullName == "MonoMod.MonoModIfFlag")
			{
				string key = (string)customAttribute.ConstructorArguments[0].Value;
				bool flag2 = ((SharedData.TryGetValue(key, out var value) && value is bool) ? ((bool)value) : (customAttribute.ConstructorArguments.Count != 2 || (bool)customAttribute.ConstructorArguments[1].Value));
				flag &= flag2;
			}
			else if (customAttribute.AttributeType.FullName == "MonoMod.MonoModTargetModule")
			{
				string text = ((string)customAttribute.ConstructorArguments[0].Value).Inject(SharedData);
				flag &= asmName.Name == text || asmName.FullName == text;
			}
		}
		return flag;
	}

	public static string GetFindableID(this MethodReference method, string name = null, string type = null, bool withType = true, bool simple = false)
	{
		while (method.IsGenericInstance)
		{
			method = ((GenericInstanceMethod)method).ElementMethod;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (simple)
		{
			if (withType)
			{
				stringBuilder.Append(type ?? method.DeclaringType.GetPatchFullName()).Append("::");
			}
			stringBuilder.Append(name ?? method.Name);
			return stringBuilder.ToString();
		}
		stringBuilder.Append(method.ReturnType.GetPatchFullName()).Append(" ");
		if (withType)
		{
			stringBuilder.Append(type ?? method.DeclaringType.GetPatchFullName()).Append("::");
		}
		stringBuilder.Append(name ?? method.Name);
		if (method.GenericParameters.Count != 0)
		{
			stringBuilder.Append("<");
			Collection<GenericParameter> genericParameters = method.GenericParameters;
			for (int i = 0; i < genericParameters.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(genericParameters[i].Name);
			}
			stringBuilder.Append(">");
		}
		stringBuilder.Append("(");
		if (method.HasParameters)
		{
			Collection<ParameterDefinition> parameters = method.Parameters;
			for (int j = 0; j < parameters.Count; j++)
			{
				ParameterDefinition parameterDefinition = parameters[j];
				if (j > 0)
				{
					stringBuilder.Append(",");
				}
				if (parameterDefinition.ParameterType.IsSentinel)
				{
					stringBuilder.Append("...,");
				}
				stringBuilder.Append(parameterDefinition.ParameterType.GetPatchFullName());
			}
		}
		stringBuilder.Append(")");
		return stringBuilder.ToString();
	}

	public static string GetFindableID(this MethodBase method, string name = null, string type = null, bool withType = true, bool proxyMethod = false, bool simple = false)
	{
		while (method is MethodInfo && method.IsGenericMethod && !method.IsGenericMethodDefinition)
		{
			method = ((MethodInfo)method).GetGenericMethodDefinition();
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (simple)
		{
			if (withType)
			{
				stringBuilder.Append(type ?? method.DeclaringType.FullName).Append("::");
			}
			stringBuilder.Append(name ?? method.Name);
			return stringBuilder.ToString();
		}
		stringBuilder.Append((method as MethodInfo)?.ReturnType?.FullName ?? "System.Void").Append(" ");
		if (withType)
		{
			stringBuilder.Append(type ?? method.DeclaringType.FullName.Replace("+", "/")).Append("::");
		}
		stringBuilder.Append(name ?? method.Name);
		if (method.ContainsGenericParameters)
		{
			stringBuilder.Append("<");
			Type[] genericArguments = method.GetGenericArguments();
			for (int i = 0; i < genericArguments.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(genericArguments[i].Name);
			}
			stringBuilder.Append(">");
		}
		stringBuilder.Append("(");
		ParameterInfo[] parameters = method.GetParameters();
		for (int j = (proxyMethod ? 1 : 0); j < parameters.Length; j++)
		{
			ParameterInfo parameterInfo = parameters[j];
			if (j > (proxyMethod ? 1 : 0))
			{
				stringBuilder.Append(",");
			}
			if (Attribute.IsDefined(parameterInfo, t_ParamArrayAttribute))
			{
				stringBuilder.Append("...,");
			}
			stringBuilder.Append(parameterInfo.ParameterType.FullName);
		}
		stringBuilder.Append(")");
		return stringBuilder.ToString();
	}

	public static void UpdateOffsets(this Mono.Cecil.Cil.MethodBody body, int instri, int delta)
	{
		int num = body.Instructions.Count - 1;
		while (instri <= num)
		{
			body.Instructions[num].Offset += delta;
			num--;
		}
	}

	public static int GetInt(this Instruction instr)
	{
		OpCode opCode = instr.OpCode;
		if (opCode == OpCodes.Ldc_I4_M1)
		{
			return -1;
		}
		if (opCode == OpCodes.Ldc_I4_0)
		{
			return 0;
		}
		if (opCode == OpCodes.Ldc_I4_1)
		{
			return 1;
		}
		if (opCode == OpCodes.Ldc_I4_2)
		{
			return 2;
		}
		if (opCode == OpCodes.Ldc_I4_3)
		{
			return 3;
		}
		if (opCode == OpCodes.Ldc_I4_4)
		{
			return 4;
		}
		if (opCode == OpCodes.Ldc_I4_5)
		{
			return 5;
		}
		if (opCode == OpCodes.Ldc_I4_6)
		{
			return 6;
		}
		if (opCode == OpCodes.Ldc_I4_7)
		{
			return 7;
		}
		if (opCode == OpCodes.Ldc_I4_8)
		{
			return 8;
		}
		if (opCode == OpCodes.Ldc_I4_S)
		{
			return (sbyte)instr.Operand;
		}
		return (int)instr.Operand;
	}

	public static int? GetIntOrNull(this Instruction instr)
	{
		OpCode opCode = instr.OpCode;
		if (opCode == OpCodes.Ldc_I4_M1)
		{
			return -1;
		}
		if (opCode == OpCodes.Ldc_I4_0)
		{
			return 0;
		}
		if (opCode == OpCodes.Ldc_I4_1)
		{
			return 1;
		}
		if (opCode == OpCodes.Ldc_I4_2)
		{
			return 2;
		}
		if (opCode == OpCodes.Ldc_I4_3)
		{
			return 3;
		}
		if (opCode == OpCodes.Ldc_I4_4)
		{
			return 4;
		}
		if (opCode == OpCodes.Ldc_I4_5)
		{
			return 5;
		}
		if (opCode == OpCodes.Ldc_I4_6)
		{
			return 6;
		}
		if (opCode == OpCodes.Ldc_I4_7)
		{
			return 7;
		}
		if (opCode == OpCodes.Ldc_I4_8)
		{
			return 8;
		}
		if (opCode == OpCodes.Ldc_I4_S)
		{
			return (sbyte)instr.Operand;
		}
		if (opCode == OpCodes.Ldc_I4)
		{
			return (int)instr.Operand;
		}
		return null;
	}

	public static ParameterDefinition GetParam(this Instruction instr, MethodDefinition method)
	{
		OpCode opCode = instr.OpCode;
		int num = (method.HasThis ? (-1) : 0);
		if (opCode == OpCodes.Ldarg_0)
		{
			if (!method.HasThis)
			{
				return method.Parameters[num];
			}
			return null;
		}
		if (opCode == OpCodes.Ldarg_1)
		{
			return method.Parameters[num + 1];
		}
		if (opCode == OpCodes.Ldarg_2)
		{
			return method.Parameters[num + 2];
		}
		if (opCode == OpCodes.Ldarg_3)
		{
			return method.Parameters[num + 3];
		}
		if (opCode == OpCodes.Ldarg_S)
		{
			return (ParameterDefinition)instr.Operand;
		}
		if (opCode == OpCodes.Ldarg)
		{
			return (ParameterDefinition)instr.Operand;
		}
		return null;
	}

	public static VariableDefinition GetLocal(this Instruction instr, MethodDefinition method)
	{
		OpCode opCode = instr.OpCode;
		if (opCode == OpCodes.Ldloc_0)
		{
			return method.Body.Variables[0];
		}
		if (opCode == OpCodes.Ldloc_1)
		{
			return method.Body.Variables[1];
		}
		if (opCode == OpCodes.Ldloc_2)
		{
			return method.Body.Variables[2];
		}
		if (opCode == OpCodes.Ldloc_3)
		{
			return method.Body.Variables[3];
		}
		if (opCode == OpCodes.Ldloca_S)
		{
			return (VariableDefinition)instr.Operand;
		}
		if (opCode == OpCodes.Ldloc)
		{
			return (VariableDefinition)instr.Operand;
		}
		return null;
	}

	public static void AddRange<T>(this Collection<T> list, Collection<T> other)
	{
		for (int i = 0; i < other.Count; i++)
		{
			list.Add(other[i]);
		}
	}

	public static void AddRange(this IDictionary dict, IDictionary other)
	{
		foreach (DictionaryEntry item in other)
		{
			dict.Add(item.Key, item.Value);
		}
	}

	public static void AddRange<K, V>(this IDictionary<K, V> dict, IDictionary<K, V> other)
	{
		foreach (KeyValuePair<K, V> item in other)
		{
			dict.Add(item.Key, item.Value);
		}
	}

	public static void AddRange<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> other)
	{
		foreach (KeyValuePair<K, V> item in other)
		{
			dict.Add(item.Key, item.Value);
		}
	}

	public static void PushRange<T>(this Stack<T> stack, T[] other)
	{
		foreach (T item in other)
		{
			stack.Push(item);
		}
	}

	public static void PopRange<T>(this Stack<T> stack, int n)
	{
		for (int i = 0; i < n; i++)
		{
			stack.Pop();
		}
	}

	public static void EnqueueRange<T>(this Queue<T> queue, T[] other)
	{
		foreach (T item in other)
		{
			queue.Enqueue(item);
		}
	}

	public static void DequeueRange<T>(this Queue<T> queue, int n)
	{
		for (int i = 0; i < n; i++)
		{
			queue.Dequeue();
		}
	}

	public static T[] Clone<T>(this T[] array, int length)
	{
		T[] array2 = new T[length];
		Array.Copy(array, array2, length);
		return array2;
	}

	public static GenericParameter GetGenericParameter(this IGenericParameterProvider provider, GenericParameter orig)
	{
		if (provider is GenericParameter && ((GenericParameter)provider).Name == orig.Name)
		{
			return (GenericParameter)provider;
		}
		foreach (GenericParameter genericParameter in provider.GenericParameters)
		{
			if (genericParameter.Name == orig.Name)
			{
				return genericParameter;
			}
		}
		int position = orig.Position;
		if (provider is MethodReference && orig.DeclaringMethod != null)
		{
			if (position < provider.GenericParameters.Count)
			{
				return provider.GenericParameters[position];
			}
			return new GenericParameter(orig.Name, provider).Update(position, GenericParameterType.Method);
		}
		if (provider is TypeReference && orig.DeclaringType != null)
		{
			if (position < provider.GenericParameters.Count)
			{
				return provider.GenericParameters[position];
			}
			return new GenericParameter(orig.Name, provider).Update(position, GenericParameterType.Type);
		}
		object obj = (provider as TypeSpecification)?.ElementType.GetGenericParameter(orig);
		if (obj == null)
		{
			MemberReference obj2 = provider as MemberReference;
			if (obj2 == null)
			{
				return null;
			}
			TypeReference declaringType = obj2.DeclaringType;
			if (declaringType == null)
			{
				return null;
			}
			obj = declaringType.GetGenericParameter(orig);
		}
		return (GenericParameter)obj;
	}

	public static IMetadataTokenProvider Relink(this IMetadataTokenProvider mtp, Relinker relinker, IGenericParameterProvider context)
	{
		if (mtp is TypeReference)
		{
			return ((TypeReference)mtp).Relink(relinker, context);
		}
		if (mtp is MethodReference)
		{
			return ((MethodReference)mtp).Relink(relinker, context);
		}
		if (mtp is FieldReference)
		{
			return ((FieldReference)mtp).Relink(relinker, context);
		}
		if (mtp is ParameterDefinition)
		{
			return ((ParameterDefinition)mtp).Relink(relinker, context);
		}
		throw new InvalidOperationException($"MonoMod can't handle metadata token providers of the type {mtp.GetType()}");
	}

	public static TypeReference Relink(this TypeReference type, Relinker relinker, IGenericParameterProvider context)
	{
		if (type == null)
		{
			return null;
		}
		if (type is TypeSpecification)
		{
			TypeReference type2 = ((TypeSpecification)type).ElementType.Relink(relinker, context);
			if (type.IsSentinel)
			{
				return new SentinelType(type2);
			}
			if (type.IsByReference)
			{
				return new ByReferenceType(type2);
			}
			if (type.IsPointer)
			{
				return new PointerType(type2);
			}
			if (type.IsPinned)
			{
				return new PinnedType(type2);
			}
			if (type.IsArray)
			{
				return new ArrayType(type2, ((ArrayType)type).Dimensions.Count);
			}
			if (type.IsRequiredModifier)
			{
				return new RequiredModifierType(((RequiredModifierType)type).ModifierType.Relink(relinker, context), type2);
			}
			if (type.IsOptionalModifier)
			{
				return new OptionalModifierType(((OptionalModifierType)type).ModifierType.Relink(relinker, context), type2);
			}
			if (type.IsGenericInstance)
			{
				GenericInstanceType genericInstanceType = new GenericInstanceType(type2);
				{
					foreach (TypeReference genericArgument in ((GenericInstanceType)type).GenericArguments)
					{
						genericInstanceType.GenericArguments.Add(genericArgument?.Relink(relinker, context));
					}
					return genericInstanceType;
				}
			}
			if (type.IsFunctionPointer)
			{
				FunctionPointerType functionPointerType = (FunctionPointerType)type;
				functionPointerType.ReturnType = functionPointerType.ReturnType.Relink(relinker, context);
				for (int i = 0; i < functionPointerType.Parameters.Count; i++)
				{
					functionPointerType.Parameters[i].ParameterType = functionPointerType.Parameters[i].ParameterType.Relink(relinker, context);
				}
				return functionPointerType;
			}
			throw new NotSupportedException($"MonoMod can't handle TypeSpecification: {type.FullName} ({type.GetType()})");
		}
		if (type.IsGenericParameter)
		{
			GenericParameter genericParameter = context.GetGenericParameter((GenericParameter)type);
			if (genericParameter == null)
			{
				throw new RelinkTargetNotFoundException(string.Format("{0} {1} (context: {2})", "MonoMod relinker failed finding", type.FullName, context), type, context);
			}
			for (int j = 0; j < genericParameter.Constraints.Count; j++)
			{
				if (!genericParameter.Constraints[j].IsGenericInstance)
				{
					genericParameter.Constraints[j] = genericParameter.Constraints[j].Relink(relinker, context);
				}
			}
			return genericParameter;
		}
		return (TypeReference)relinker(type, context);
	}

	public static IMetadataTokenProvider Relink(this MethodReference method, Relinker relinker, IGenericParameterProvider context)
	{
		if (method.IsGenericInstance)
		{
			GenericInstanceMethod obj = (GenericInstanceMethod)method;
			GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod((MethodReference)obj.ElementMethod.Relink(relinker, context));
			foreach (TypeReference genericArgument in obj.GenericArguments)
			{
				genericInstanceMethod.GenericArguments.Add(genericArgument.Relink(relinker, context));
			}
			return (MethodReference)relinker(genericInstanceMethod, context);
		}
		MethodReference methodReference = new MethodReference(method.Name, method.ReturnType, method.DeclaringType.Relink(relinker, context));
		methodReference.CallingConvention = method.CallingConvention;
		methodReference.ExplicitThis = method.ExplicitThis;
		methodReference.HasThis = method.HasThis;
		foreach (GenericParameter genericParameter2 in method.GenericParameters)
		{
			GenericParameter genericParameter = new GenericParameter(genericParameter2.Name, genericParameter2.Owner)
			{
				Attributes = genericParameter2.Attributes
			}.Update(genericParameter2);
			methodReference.GenericParameters.Add(genericParameter);
			foreach (TypeReference constraint in genericParameter2.Constraints)
			{
				genericParameter.Constraints.Add(constraint.Relink(relinker, methodReference));
			}
		}
		methodReference.ReturnType = method.ReturnType?.Relink(relinker, methodReference);
		foreach (ParameterDefinition parameter in method.Parameters)
		{
			parameter.ParameterType = parameter.ParameterType.Relink(relinker, method);
			methodReference.Parameters.Add(parameter);
		}
		return relinker(methodReference, context);
	}

	public static IMetadataTokenProvider Relink(this FieldReference field, Relinker relinker, IGenericParameterProvider context)
	{
		TypeReference typeReference = field.DeclaringType.Relink(relinker, context);
		return relinker(new FieldReference(field.Name, field.FieldType.Relink(relinker, typeReference), typeReference), context);
	}

	public static IMetadataTokenProvider Relink(this ParameterDefinition param, Relinker relinker, IGenericParameterProvider context)
	{
		param = ((MethodReference)param.Method).Parameters[param.Index];
		param.ParameterType = param.ParameterType.Relink(relinker, context);
		for (int i = 0; i < param.CustomAttributes.Count; i++)
		{
			param.CustomAttributes[i] = param.CustomAttributes[i].Relink(relinker, context);
		}
		return param;
	}

	public static ParameterDefinition Clone(this ParameterDefinition param)
	{
		ParameterDefinition parameterDefinition = new ParameterDefinition(param.Name, param.Attributes, param.ParameterType)
		{
			IsIn = param.IsIn,
			IsLcid = param.IsLcid,
			IsOptional = param.IsOptional,
			IsOut = param.IsOut,
			IsReturnValue = param.IsReturnValue
		};
		if (param.HasConstant)
		{
			parameterDefinition.Constant = param.Constant;
		}
		foreach (CustomAttribute customAttribute in param.CustomAttributes)
		{
			parameterDefinition.CustomAttributes.Add(customAttribute.Clone());
		}
		return parameterDefinition;
	}

	public static CustomAttribute Relink(this CustomAttribute attrib, Relinker relinker, IGenericParameterProvider context)
	{
		attrib.Constructor = (MethodReference)attrib.Constructor.Relink(relinker, context);
		for (int i = 0; i < attrib.ConstructorArguments.Count; i++)
		{
			CustomAttributeArgument customAttributeArgument = attrib.ConstructorArguments[i];
			attrib.ConstructorArguments[i] = new CustomAttributeArgument(customAttributeArgument.Type.Relink(relinker, context), customAttributeArgument.Value);
		}
		for (int j = 0; j < attrib.Fields.Count; j++)
		{
			Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = attrib.Fields[j];
			attrib.Fields[j] = new Mono.Cecil.CustomAttributeNamedArgument(customAttributeNamedArgument.Name, new CustomAttributeArgument(customAttributeNamedArgument.Argument.Type.Relink(relinker, context), customAttributeNamedArgument.Argument.Value));
		}
		for (int k = 0; k < attrib.Properties.Count; k++)
		{
			Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument2 = attrib.Properties[k];
			attrib.Properties[k] = new Mono.Cecil.CustomAttributeNamedArgument(customAttributeNamedArgument2.Name, new CustomAttributeArgument(customAttributeNamedArgument2.Argument.Type.Relink(relinker, context), customAttributeNamedArgument2.Argument.Value));
		}
		return attrib;
	}

	public static CustomAttribute Clone(this CustomAttribute attrib)
	{
		CustomAttribute customAttribute = new CustomAttribute(attrib.Constructor, attrib.GetBlob());
		foreach (CustomAttributeArgument constructorArgument in attrib.ConstructorArguments)
		{
			customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(constructorArgument.Type, constructorArgument.Value));
		}
		foreach (Mono.Cecil.CustomAttributeNamedArgument field in attrib.Fields)
		{
			customAttribute.Fields.Add(new Mono.Cecil.CustomAttributeNamedArgument(field.Name, new CustomAttributeArgument(field.Argument.Type, field.Argument.Value)));
		}
		foreach (Mono.Cecil.CustomAttributeNamedArgument property in attrib.Properties)
		{
			customAttribute.Properties.Add(new Mono.Cecil.CustomAttributeNamedArgument(property.Name, new CustomAttributeArgument(property.Argument.Type, property.Argument.Value)));
		}
		return customAttribute;
	}

	public static GenericParameter Relink(this GenericParameter param, Relinker relinker, IGenericParameterProvider context)
	{
		GenericParameter genericParameter = new GenericParameter(param.Name, param.Owner)
		{
			Attributes = param.Attributes
		}.Update(param);
		foreach (TypeReference constraint in param.Constraints)
		{
			genericParameter.Constraints.Add(constraint.Relink(relinker, context));
		}
		return genericParameter;
	}

	public static GenericParameter Clone(this GenericParameter param)
	{
		GenericParameter genericParameter = new GenericParameter(param.Name, param.Owner)
		{
			Attributes = param.Attributes
		}.Update(param);
		foreach (TypeReference constraint in param.Constraints)
		{
			genericParameter.Constraints.Add(constraint);
		}
		return genericParameter;
	}

	public static MethodDefinition FindMethod(this TypeDefinition type, string findableID, bool simple = true)
	{
		if (simple && !findableID.Contains(" "))
		{
			foreach (MethodDefinition method in type.Methods)
			{
				if (method.GetFindableID(null, null, withType: true, simple: true) == findableID)
				{
					return method;
				}
			}
			foreach (MethodDefinition method2 in type.Methods)
			{
				if (method2.GetFindableID(null, null, withType: false, simple: true) == findableID)
				{
					return method2;
				}
			}
		}
		foreach (MethodDefinition method3 in type.Methods)
		{
			if (method3.GetFindableID() == findableID)
			{
				return method3;
			}
		}
		foreach (MethodDefinition method4 in type.Methods)
		{
			if (method4.GetFindableID(null, null, withType: false) == findableID)
			{
				return method4;
			}
		}
		return null;
	}

	public static MethodInfo FindMethod(this Type type, string findableID, bool simple = true)
	{
		MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		MethodInfo[] array = methods;
		foreach (MethodInfo methodInfo in array)
		{
			if (methodInfo.GetFindableID() == findableID)
			{
				return methodInfo;
			}
		}
		array = methods;
		foreach (MethodInfo methodInfo2 in array)
		{
			if (methodInfo2.GetFindableID(null, null, withType: false) == findableID)
			{
				return methodInfo2;
			}
		}
		if (!simple)
		{
			return null;
		}
		array = methods;
		foreach (MethodInfo methodInfo3 in array)
		{
			if (methodInfo3.GetFindableID(null, null, withType: true, proxyMethod: false, simple: true) == findableID)
			{
				return methodInfo3;
			}
		}
		array = methods;
		foreach (MethodInfo methodInfo4 in array)
		{
			if (methodInfo4.GetFindableID(null, null, withType: false, proxyMethod: false, simple: true) == findableID)
			{
				return methodInfo4;
			}
		}
		return null;
	}

	public static PropertyDefinition FindProperty(this TypeDefinition type, string name)
	{
		foreach (PropertyDefinition property in type.Properties)
		{
			if (property.Name == name)
			{
				return property;
			}
		}
		return null;
	}

	public static FieldDefinition FindField(this TypeDefinition type, string name)
	{
		foreach (FieldDefinition field in type.Fields)
		{
			if (field.Name == name)
			{
				return field;
			}
		}
		return null;
	}

	public static EventDefinition FindEvent(this TypeDefinition type, string name)
	{
		foreach (EventDefinition @event in type.Events)
		{
			if (@event.Name == name)
			{
				return @event;
			}
		}
		return null;
	}

	public static bool HasMethod(this TypeDefinition type, MethodDefinition method)
	{
		return type.FindMethod(method.GetFindableID(null, null, withType: false)) != null;
	}

	public static bool HasProperty(this TypeDefinition type, PropertyDefinition prop)
	{
		return type.FindProperty(prop.Name) != null;
	}

	public static bool HasField(this TypeDefinition type, FieldDefinition field)
	{
		return type.FindField(field.Name) != null;
	}

	public static bool HasEvent(this TypeDefinition type, EventDefinition eventDef)
	{
		return type.FindEvent(eventDef.Name) != null;
	}

	public static void SetPublic(this IMetadataTokenProvider mtp, bool p)
	{
		if (mtp is TypeDefinition)
		{
			((TypeDefinition)mtp).SetPublic(p);
			return;
		}
		if (mtp is FieldDefinition)
		{
			((FieldDefinition)mtp).SetPublic(p);
			return;
		}
		if (mtp is MethodDefinition)
		{
			((MethodDefinition)mtp).SetPublic(p);
			return;
		}
		throw new InvalidOperationException($"MonoMod can't set metadata token providers of the type {mtp.GetType()} public.");
	}

	public static void SetPublic(this FieldDefinition o, bool p)
	{
		if (o.IsDefinition && !(o.DeclaringType.Name == "<PrivateImplementationDetails>"))
		{
			o.IsPrivate = !p;
			o.IsPublic = p;
			if (p)
			{
				o.DeclaringType.SetPublic(p: true);
			}
		}
	}

	public static void SetPublic(this MethodDefinition o, bool p)
	{
		if (o.IsDefinition && !(o.DeclaringType.Name == "<PrivateImplementationDetails>"))
		{
			o.IsPrivate = !p;
			o.IsPublic = p;
			if (p)
			{
				o.DeclaringType.SetPublic(p: true);
			}
		}
	}

	public static void SetPublic(this TypeDefinition o, bool p)
	{
		if (!o.IsDefinition || o.Name == "<PrivateImplementationDetails>" || (o.DeclaringType != null && o.DeclaringType.Name == "<PrivateImplementationDetails>"))
		{
			return;
		}
		if (o.DeclaringType == null)
		{
			o.IsNotPublic = !p;
			o.IsPublic = p;
			return;
		}
		o.IsNestedPrivate = !p;
		o.IsNestedPublic = p;
		if (p)
		{
			o.DeclaringType.SetPublic(p: true);
		}
	}

	public static Collection<T> Filtered<T>(this Collection<T> mtps, List<string> with, List<string> without) where T : IMetadataTokenProvider
	{
		if (with.Count == 0 && without.Count == 0)
		{
			return mtps;
		}
		Collection<T> collection = new Collection<T>();
		for (int i = 0; i < mtps.Count; i++)
		{
			IMetadataTokenProvider metadataTokenProvider = mtps[i];
			string item = null;
			string item2 = null;
			if (metadataTokenProvider is TypeReference)
			{
				item = ((TypeReference)metadataTokenProvider).FullName;
			}
			else if (metadataTokenProvider is MethodReference)
			{
				item = ((MethodReference)metadataTokenProvider).GetFindableID();
				item2 = ((MethodReference)metadataTokenProvider).GetFindableID(null, null, withType: true, simple: true);
			}
			else if (metadataTokenProvider is FieldReference)
			{
				item = ((FieldReference)metadataTokenProvider).Name;
			}
			if ((without.Count == 0 || (!without.Contains(item) && !without.Contains(item2))) && (with.Count == 0 || with.Contains(item) || with.Contains(item2)))
			{
				collection.Add((T)metadataTokenProvider);
			}
		}
		return collection;
	}

	public static IMetadataTokenProvider ImportReference(this ModuleDefinition mod, IMetadataTokenProvider mtp)
	{
		if (mtp is TypeReference)
		{
			return mod.ImportReference((TypeReference)mtp);
		}
		if (mtp is FieldReference)
		{
			return mod.ImportReference((FieldReference)mtp);
		}
		if (mtp is MethodReference)
		{
			return mod.ImportReference((MethodReference)mtp);
		}
		return mtp;
	}

	public static IMemberDefinition SafeResolve(this MemberReference r)
	{
		try
		{
			return r.Resolve();
		}
		catch
		{
			return null;
		}
	}

	public static TypeDefinition SafeResolve(this TypeReference r)
	{
		try
		{
			return r.Resolve();
		}
		catch
		{
			return null;
		}
	}

	public static FieldDefinition SafeResolve(this FieldReference r)
	{
		try
		{
			return r.Resolve();
		}
		catch
		{
			return null;
		}
	}

	public static MethodDefinition SafeResolve(this MethodReference r)
	{
		try
		{
			return r.Resolve();
		}
		catch
		{
			return null;
		}
	}

	public static PropertyDefinition SafeResolve(this PropertyReference r)
	{
		try
		{
			return r.Resolve();
		}
		catch
		{
			return null;
		}
	}

	public static string GetPatchName(this MemberReference mr)
	{
		Mono.Cecil.ICustomAttributeProvider obj = mr as Mono.Cecil.ICustomAttributeProvider;
		return ((obj != null) ? obj.GetPatchName() : null) ?? mr.Name;
	}

	public static string GetPatchFullName(this MemberReference mr)
	{
		Mono.Cecil.ICustomAttributeProvider obj = mr as Mono.Cecil.ICustomAttributeProvider;
		return ((obj != null) ? obj.GetPatchFullName(mr) : null) ?? mr.FullName;
	}

	private static string GetPatchName(this Mono.Cecil.ICustomAttributeProvider cap)
	{
		CustomAttribute mMAttribute = cap.GetMMAttribute("Patch");
		if (mMAttribute != null)
		{
			return ((string)mMAttribute.ConstructorArguments[0].Value).Inject(SharedData);
		}
		string name = ((MemberReference)cap).Name;
		if (!name.StartsWith("patch_"))
		{
			return name;
		}
		return name.Substring(6);
	}

	private static string GetPatchFullName(this Mono.Cecil.ICustomAttributeProvider cap, MemberReference mr)
	{
		if (cap is TypeReference)
		{
			TypeReference typeReference = (TypeReference)cap;
			string text = cap.GetPatchName();
			if (text.StartsWith("global::"))
			{
				text = text.Substring(8);
			}
			else if (!text.Contains(".") && !text.Contains("/"))
			{
				if (!string.IsNullOrEmpty(typeReference.Namespace))
				{
					text = typeReference.Namespace + "." + text;
				}
				else if (typeReference.IsNested)
				{
					text = typeReference.DeclaringType.GetPatchFullName() + "/" + text;
				}
			}
			if (mr is TypeSpecification)
			{
				List<TypeSpecification> list = new List<TypeSpecification>();
				TypeSpecification typeSpecification = (TypeSpecification)mr;
				do
				{
					list.Add(typeSpecification);
				}
				while ((typeSpecification = typeSpecification.ElementType as TypeSpecification) != null);
				StringBuilder stringBuilder = new StringBuilder(text.Length + list.Count * 4);
				stringBuilder.Append(text);
				for (int num = list.Count - 1; num > -1; num--)
				{
					typeSpecification = list[num];
					if (typeSpecification.IsByReference)
					{
						stringBuilder.Append("&");
					}
					else if (typeSpecification.IsPointer)
					{
						stringBuilder.Append("*");
					}
					else if (!typeSpecification.IsPinned && !typeSpecification.IsSentinel)
					{
						if (typeSpecification.IsArray)
						{
							ArrayType arrayType = (ArrayType)typeSpecification;
							if (arrayType.IsVector)
							{
								stringBuilder.Append("[]");
							}
							else
							{
								stringBuilder.Append("[");
								for (int i = 0; i < arrayType.Dimensions.Count; i++)
								{
									if (i > 0)
									{
										stringBuilder.Append(",");
									}
									stringBuilder.Append(arrayType.Dimensions[i].ToString());
								}
								stringBuilder.Append("]");
							}
						}
						else if (typeSpecification.IsRequiredModifier)
						{
							stringBuilder.Append("modreq(").Append(((RequiredModifierType)typeSpecification).ModifierType).Append(")");
						}
						else if (typeSpecification.IsOptionalModifier)
						{
							stringBuilder.Append("modopt(").Append(((OptionalModifierType)typeSpecification).ModifierType).Append(")");
						}
						else if (typeSpecification.IsGenericInstance)
						{
							GenericInstanceType genericInstanceType = (GenericInstanceType)typeSpecification;
							stringBuilder.Append("<");
							for (int j = 0; j < genericInstanceType.GenericArguments.Count; j++)
							{
								if (j > 0)
								{
									stringBuilder.Append(",");
								}
								stringBuilder.Append(genericInstanceType.GenericArguments[j].GetPatchFullName());
							}
							stringBuilder.Append(">");
						}
						else
						{
							if (!typeSpecification.IsFunctionPointer)
							{
								throw new NotSupportedException($"MonoMod can't handle TypeSpecification: {typeReference.FullName} ({typeReference.GetType()})");
							}
							FunctionPointerType functionPointerType = (FunctionPointerType)typeSpecification;
							stringBuilder.Append(" ").Append(functionPointerType.ReturnType.GetPatchFullName()).Append(" *(");
							if (functionPointerType.HasParameters)
							{
								for (int k = 0; k < functionPointerType.Parameters.Count; k++)
								{
									ParameterDefinition parameterDefinition = functionPointerType.Parameters[k];
									if (k > 0)
									{
										stringBuilder.Append(",");
									}
									if (parameterDefinition.ParameterType.IsSentinel)
									{
										stringBuilder.Append("...,");
									}
									stringBuilder.Append(parameterDefinition.ParameterType.FullName);
								}
							}
							stringBuilder.Append(")");
						}
					}
				}
				text = stringBuilder.ToString();
			}
			return text;
		}
		if (cap is FieldReference)
		{
			FieldReference fieldReference = (FieldReference)cap;
			return $"{fieldReference.FieldType.GetPatchFullName()} {fieldReference.DeclaringType.GetPatchFullName()}::{cap.GetPatchName()}";
		}
		if (cap is MethodReference)
		{
			throw new InvalidOperationException("GetPatchFullName not supported on MethodReferences - use GetFindableID instead");
		}
		throw new InvalidOperationException($"GetPatchFullName not supported on type {cap.GetType()}");
	}

	public static bool IsBaseMethodCall(this Instruction instr, Mono.Cecil.Cil.MethodBody body)
	{
		if (instr.OpCode != OpCodes.Call)
		{
			return false;
		}
		MethodDefinition method = body.Method;
		if (!(instr.Operand is MethodReference { DeclaringType: var typeReference }))
		{
			return false;
		}
		while (typeReference is TypeSpecification)
		{
			typeReference = ((TypeSpecification)typeReference).ElementType;
		}
		string patchFullName = typeReference.GetPatchFullName();
		bool flag = false;
		try
		{
			TypeDefinition typeDefinition = method.DeclaringType;
			while ((typeDefinition = typeDefinition.BaseType?.SafeResolve()) != null)
			{
				if (typeDefinition.GetPatchFullName() == patchFullName)
				{
					flag = true;
					break;
				}
			}
		}
		catch
		{
			flag = method.DeclaringType.GetPatchFullName() == patchFullName;
		}
		if (!flag)
		{
			return false;
		}
		return true;
	}

	public static bool IsCallvirt(this MethodReference method)
	{
		if (!method.HasThis)
		{
			return false;
		}
		if (method.DeclaringType.IsValueType)
		{
			return false;
		}
		return true;
	}

	public static bool IsStruct(this TypeReference type)
	{
		if (!type.IsValueType)
		{
			return false;
		}
		if (type.IsPrimitive)
		{
			return false;
		}
		return true;
	}

	public static void RecalculateILOffsets(this MethodDefinition method)
	{
		if (method.HasBody)
		{
			int num = 0;
			for (int i = 0; i < method.Body.Instructions.Count; i++)
			{
				Instruction instruction = method.Body.Instructions[i];
				instruction.Offset = num;
				num += instruction.GetSize();
			}
		}
	}

	public static void AppendGetAddr(this Mono.Cecil.Cil.MethodBody body, Instruction instr, TypeReference type, IDictionary<TypeReference, VariableDefinition> localMap = null)
	{
		if (localMap == null || !localMap.TryGetValue(type, out var value))
		{
			value = new VariableDefinition(type);
			body.Variables.Add(value);
			if (localMap != null)
			{
				localMap[type] = value;
			}
		}
		ILProcessor iLProcessor = body.GetILProcessor();
		Instruction target = instr;
		iLProcessor.InsertAfter(target, target = iLProcessor.Create(OpCodes.Stloc, value));
		iLProcessor.InsertAfter(target, target = iLProcessor.Create(OpCodes.Ldloca, value));
	}

	public static void PrependGetValue(this Mono.Cecil.Cil.MethodBody body, Instruction instr, TypeReference type)
	{
		ILProcessor iLProcessor = body.GetILProcessor();
		iLProcessor.InsertBefore(instr, iLProcessor.Create(OpCodes.Ldobj, type));
	}

	public static void ConvertShortLongOps(this MethodDefinition method)
	{
		if (!method.HasBody)
		{
			return;
		}
		for (int i = 0; i < method.Body.Instructions.Count; i++)
		{
			Instruction instruction = method.Body.Instructions[i];
			if (instruction.Operand is Instruction)
			{
				int num = ((Instruction)instruction.Operand).Offset - instruction.Offset;
				if (num <= -127 || num >= 127)
				{
					instruction.OpCode = instruction.OpCode.ShortToLongOp();
				}
				else
				{
					instruction.OpCode = instruction.OpCode.LongToShortOp();
				}
			}
		}
	}

	public static OpCode ShortToLongOp(this OpCode op)
	{
		string name = Enum.GetName(t_Code, op.Code);
		if (!name.EndsWith("_S"))
		{
			return op;
		}
		if (_ShortToLongOp.TryGetValue((int)op.Code, out var value))
		{
			return value;
		}
		return _ShortToLongOp[(int)op.Code] = ((OpCode?)t_OpCodes.GetField(name.Substring(0, name.Length - 2))?.GetValue(null)) ?? op;
	}

	public static OpCode LongToShortOp(this OpCode op)
	{
		string name = Enum.GetName(t_Code, op.Code);
		if (name.EndsWith("_S"))
		{
			return op;
		}
		if (_LongToShortOp.TryGetValue((int)op.Code, out var value))
		{
			return value;
		}
		return _LongToShortOp[(int)op.Code] = ((OpCode?)t_OpCodes.GetField(name + "_S")?.GetValue(null)) ?? op;
	}

	public static bool IsMatchingSignature(this MethodDefinition method, MethodReference candidate)
	{
		if (method.Parameters.Count != candidate.Parameters.Count)
		{
			return false;
		}
		if (method.Name != candidate.Name)
		{
			return false;
		}
		if (!method.ReturnType._InflateGenericType(method).IsMatchingSignature(candidate.ReturnType._InflateGenericType(candidate)))
		{
			return false;
		}
		if (method.GenericParameters.Count != candidate.GenericParameters.Count)
		{
			return false;
		}
		if (method.HasParameters)
		{
			for (int i = 0; i < method.Parameters.Count; i++)
			{
				if (!method.Parameters[i].ParameterType._InflateGenericType(method).IsMatchingSignature(candidate.Parameters[i].ParameterType._InflateGenericType(candidate)))
				{
					return false;
				}
			}
		}
		MethodDefinition methodDefinition = candidate.SafeResolve();
		if (methodDefinition != null && !methodDefinition.IsVirtual)
		{
			return false;
		}
		return true;
	}

	public static bool IsMatchingSignature(this IModifierType a, IModifierType b)
	{
		if (!a.ModifierType.IsMatchingSignature(b.ModifierType))
		{
			return false;
		}
		return a.ElementType.IsMatchingSignature(b.ElementType);
	}

	public static bool IsMatchingSignature(this TypeSpecification a, TypeSpecification b)
	{
		if (a.GetType() != b.GetType())
		{
			return false;
		}
		if (a is GenericInstanceType a2)
		{
			return a2.IsMatchingSignature((GenericInstanceType)b);
		}
		if (a is IModifierType a3)
		{
			return a3.IsMatchingSignature((IModifierType)b);
		}
		return a.ElementType.IsMatchingSignature(b.ElementType);
	}

	public static bool IsMatchingSignature(this GenericInstanceType a, GenericInstanceType b)
	{
		if (!a.ElementType.IsMatchingSignature(b.ElementType))
		{
			return false;
		}
		if (a.HasGenericArguments != b.HasGenericArguments)
		{
			return false;
		}
		if (!a.HasGenericArguments)
		{
			return true;
		}
		if (a.GenericArguments.Count != b.GenericArguments.Count)
		{
			return false;
		}
		for (int i = 0; i < a.GenericArguments.Count; i++)
		{
			if (!a.GenericArguments[i].IsMatchingSignature(b.GenericArguments[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsMatchingSignature(this GenericParameter a, GenericParameter b)
	{
		if (a.Position != b.Position)
		{
			return false;
		}
		if (a.Type != b.Type)
		{
			return false;
		}
		return true;
	}

	public static bool IsMatchingSignature(this TypeReference a, TypeReference b)
	{
		if (a is TypeSpecification || b is TypeSpecification)
		{
			if (a is TypeSpecification && b is TypeSpecification)
			{
				return ((TypeSpecification)a).IsMatchingSignature((TypeSpecification)b);
			}
			return false;
		}
		if (a is GenericParameter && b is GenericParameter)
		{
			return ((GenericParameter)a).IsMatchingSignature((GenericParameter)b);
		}
		return a.FullName == b.FullName;
	}

	private static TypeReference _InflateGenericType(this TypeReference type, MethodReference method)
	{
		if (!(method.DeclaringType is GenericInstanceType))
		{
			return type;
		}
		return _InflateGenericType(method.DeclaringType as GenericInstanceType, type);
	}

	private static TypeReference _InflateGenericType(GenericInstanceType genericInstanceProvider, TypeReference typeToInflate)
	{
		if (typeToInflate is ArrayType arrayType)
		{
			TypeReference typeReference = _InflateGenericType(genericInstanceProvider, arrayType.ElementType);
			if (typeReference != arrayType.ElementType)
			{
				return new ArrayType(typeReference, arrayType.Rank);
			}
			return arrayType;
		}
		if (typeToInflate is GenericInstanceType genericInstanceType)
		{
			GenericInstanceType genericInstanceType2 = new GenericInstanceType(genericInstanceType.ElementType);
			for (int i = 0; i < genericInstanceType.GenericArguments.Count; i++)
			{
				genericInstanceType2.GenericArguments.Add(_InflateGenericType(genericInstanceProvider, genericInstanceType.GenericArguments[i]));
			}
			return genericInstanceType2;
		}
		if (typeToInflate is GenericParameter genericParameter)
		{
			if (genericParameter.Owner is MethodReference)
			{
				return genericParameter;
			}
			GenericParameter genericParameter2 = genericInstanceProvider.ElementType.Resolve().GetGenericParameter(genericParameter);
			return genericInstanceProvider.GenericArguments[genericParameter2.Position];
		}
		if (typeToInflate is FunctionPointerType functionPointerType)
		{
			FunctionPointerType functionPointerType2 = new FunctionPointerType();
			functionPointerType2.ReturnType = _InflateGenericType(genericInstanceProvider, functionPointerType.ReturnType);
			for (int j = 0; j < functionPointerType.Parameters.Count; j++)
			{
				functionPointerType2.Parameters.Add(new ParameterDefinition(_InflateGenericType(genericInstanceProvider, functionPointerType.Parameters[j].ParameterType)));
			}
			return functionPointerType2;
		}
		if (typeToInflate is IModifierType modifierType)
		{
			TypeReference modifierType2 = _InflateGenericType(genericInstanceProvider, modifierType.ModifierType);
			TypeReference type = _InflateGenericType(genericInstanceProvider, modifierType.ElementType);
			if (modifierType is OptionalModifierType)
			{
				return new OptionalModifierType(modifierType2, type);
			}
			return new RequiredModifierType(modifierType2, type);
		}
		if (typeToInflate is PinnedType pinnedType)
		{
			TypeReference typeReference2 = _InflateGenericType(genericInstanceProvider, pinnedType.ElementType);
			if (typeReference2 != pinnedType.ElementType)
			{
				return new PinnedType(typeReference2);
			}
			return pinnedType;
		}
		if (typeToInflate is PointerType pointerType)
		{
			TypeReference typeReference3 = _InflateGenericType(genericInstanceProvider, pointerType.ElementType);
			if (typeReference3 != pointerType.ElementType)
			{
				return new PointerType(typeReference3);
			}
			return pointerType;
		}
		if (typeToInflate is ByReferenceType byReferenceType)
		{
			TypeReference typeReference4 = _InflateGenericType(genericInstanceProvider, byReferenceType.ElementType);
			if (typeReference4 != byReferenceType.ElementType)
			{
				return new ByReferenceType(typeReference4);
			}
			return byReferenceType;
		}
		if (typeToInflate is SentinelType sentinelType)
		{
			TypeReference typeReference5 = _InflateGenericType(genericInstanceProvider, sentinelType.ElementType);
			if (typeReference5 != sentinelType.ElementType)
			{
				return new SentinelType(typeReference5);
			}
			return sentinelType;
		}
		return typeToInflate;
	}
}
