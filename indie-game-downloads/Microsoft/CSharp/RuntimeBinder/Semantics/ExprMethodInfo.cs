using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class ExprMethodInfo : ExprWithType
{
	public MethWithInst Method { get; }

	public MethodInfo MethodInfo
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			AggregateType ats = Method.Ats;
			MethodSymbol methodSymbol = Method.Meth();
			TypeArray typeArray = TypeManager.SubstTypeArray(methodSymbol.Params, ats, methodSymbol.typeVars);
			TypeManager.SubstType(methodSymbol.RetType, ats, methodSymbol.typeVars);
			Type type = ats.AssociatedSystemType;
			MethodInfo methodInfo = methodSymbol.AssociatedMemberInfo as MethodInfo;
			if (!type.IsGenericType && !type.IsNested)
			{
				type = methodInfo.DeclaringType;
			}
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo2 in methods)
			{
				if (!methodInfo2.HasSameMetadataDefinitionAs(methodInfo))
				{
					continue;
				}
				bool flag = true;
				ParameterInfo[] parameters = methodInfo2.GetParameters();
				for (int j = 0; j < typeArray.Count; j++)
				{
					if (!ExprWithType.TypesAreEqual(parameters[j].ParameterType, typeArray[j].AssociatedSystemType))
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				if (methodInfo2.IsGenericMethod)
				{
					int num = Method.TypeArgs?.Count ?? 0;
					Type[] array = new Type[num];
					if (num > 0)
					{
						for (int k = 0; k < Method.TypeArgs.Count; k++)
						{
							array[k] = Method.TypeArgs[k].AssociatedSystemType;
						}
					}
					return methodInfo2.MakeGenericMethod(array);
				}
				return methodInfo2;
			}
			throw Error.InternalCompilerError();
		}
	}

	public ConstructorInfo ConstructorInfo
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			AggregateType ats = Method.Ats;
			MethodSymbol methodSymbol = Method.Meth();
			TypeArray typeArray = TypeManager.SubstTypeArray(methodSymbol.Params, ats);
			Type type = ats.AssociatedSystemType;
			ConstructorInfo constructorInfo = (ConstructorInfo)methodSymbol.AssociatedMemberInfo;
			if (!type.IsGenericType && !type.IsNested)
			{
				type = constructorInfo.DeclaringType;
			}
			ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (ConstructorInfo constructorInfo2 in constructors)
			{
				if (!constructorInfo2.HasSameMetadataDefinitionAs(constructorInfo))
				{
					continue;
				}
				bool flag = true;
				ParameterInfo[] parameters = constructorInfo2.GetParameters();
				for (int j = 0; j < typeArray.Count; j++)
				{
					if (!ExprWithType.TypesAreEqual(parameters[j].ParameterType, typeArray[j].AssociatedSystemType))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return constructorInfo2;
				}
			}
			throw Error.InternalCompilerError();
		}
	}

	public override object Object
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			return MethodInfo;
		}
	}

	public ExprMethodInfo(CType type, MethodSymbol method, AggregateType methodType, TypeArray methodParameters)
		: base(ExpressionKind.MethodInfo, type)
	{
		Method = new MethWithInst(method, methodType, methodParameters);
	}
}
