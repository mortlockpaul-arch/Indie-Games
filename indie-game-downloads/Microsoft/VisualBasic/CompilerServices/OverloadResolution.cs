using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class OverloadResolution
{
	internal enum ResolutionFailure
	{
		None,
		MissingMember,
		InvalidArgument,
		AmbiguousMatch,
		InvalidTarget
	}

	private enum ComparisonType
	{
		ParameterSpecificty,
		GenericSpecificityBasedOnMethodGenericParams,
		GenericSpecificityBasedOnTypeGenericParams
	}

	private delegate bool ArgumentDetector(Symbols.Method targetProcedure, object[] arguments, string[] argumentNames, Type[] typeArguments, List<string> errors);

	private delegate bool CandidateProperty(Symbols.Method candidate);

	private static bool IsExactSignatureMatch(ParameterInfo[] leftSignature, int leftTypeParameterCount, ParameterInfo[] rightSignature, int rightTypeParameterCount)
	{
		ParameterInfo[] array;
		ParameterInfo[] array2;
		if (leftSignature.Length >= rightSignature.Length)
		{
			array = leftSignature;
			array2 = rightSignature;
		}
		else
		{
			array = rightSignature;
			array2 = leftSignature;
		}
		int num = array2.Length;
		checked
		{
			int num2 = array.Length - 1;
			for (int i = num; i <= num2; i++)
			{
				if (!array[i].IsOptional)
				{
					return false;
				}
			}
			int num3 = array2.Length - 1;
			for (int j = 0; j <= num3; j++)
			{
				Type type = array2[j].ParameterType;
				Type type2 = array[j].ParameterType;
				if (type.IsByRef)
				{
					type = type.GetElementType();
				}
				if (type2.IsByRef)
				{
					type2 = type2.GetElementType();
				}
				if ((object)type != type2 && (!array2[j].IsOptional || !array[j].IsOptional))
				{
					return false;
				}
			}
			return true;
		}
	}

	private static void CompareNumericTypeSpecificity(Type leftType, Type rightType, ref bool leftWins, ref bool rightWins)
	{
		if ((object)leftType != rightType)
		{
			if (ConversionResolution.NumericSpecificityRank[(int)ReflectionExtensions.GetTypeCode(leftType)] < ConversionResolution.NumericSpecificityRank[(int)ReflectionExtensions.GetTypeCode(rightType)])
			{
				leftWins = true;
			}
			else
			{
				rightWins = true;
			}
		}
	}

	[RequiresUnreferencedCode("ClassifyConversion")]
	private static void CompareParameterSpecificity(Type argumentType, ParameterInfo leftParameter, MethodBase leftProcedure, bool expandLeftParamArray, ParameterInfo rightParameter, MethodBase rightProcedure, bool expandRightParamArray, ref bool leftWins, ref bool rightWins, ref bool bothLose)
	{
		bothLose = false;
		Type type = leftParameter.ParameterType;
		Type type2 = rightParameter.ParameterType;
		if (type.IsByRef)
		{
			type = Symbols.GetElementType(type);
		}
		if (type2.IsByRef)
		{
			type2 = Symbols.GetElementType(type2);
		}
		if (expandLeftParamArray && Symbols.IsParamArray(leftParameter))
		{
			type = Symbols.GetElementType(type);
		}
		if (expandRightParamArray && Symbols.IsParamArray(rightParameter))
		{
			type2 = Symbols.GetElementType(type2);
		}
		if (Symbols.IsNumericType(type) && Symbols.IsNumericType(type2) && !Symbols.IsEnum(type) && !Symbols.IsEnum(type2))
		{
			CompareNumericTypeSpecificity(type, type2, ref leftWins, ref rightWins);
			return;
		}
		if ((object)leftProcedure != null && (object)rightProcedure != null && Symbols.IsRawGeneric(leftProcedure) && Symbols.IsRawGeneric(rightProcedure))
		{
			if ((object)type == type2)
			{
				return;
			}
			int num = Symbols.IndexIn(type, leftProcedure);
			int num2 = Symbols.IndexIn(type2, rightProcedure);
			if (num == num2 && num >= 0)
			{
				return;
			}
		}
		Symbols.Method operatorMethod = null;
		switch (ConversionResolution.ClassifyConversion(type2, type, ref operatorMethod))
		{
		case ConversionResolution.ConversionClass.Widening:
			if ((object)operatorMethod != null && ConversionResolution.ClassifyConversion(type, type2, ref operatorMethod) == ConversionResolution.ConversionClass.Widening)
			{
				if ((object)argumentType != null && (object)argumentType == type)
				{
					leftWins = true;
				}
				else if ((object)argumentType != null && (object)argumentType == type2)
				{
					rightWins = true;
				}
				else
				{
					bothLose = true;
				}
			}
			else
			{
				leftWins = true;
			}
			break;
		default:
			if (ConversionResolution.ClassifyConversion(type, type2, ref operatorMethod) == ConversionResolution.ConversionClass.Widening)
			{
				rightWins = true;
			}
			else
			{
				bothLose = true;
			}
			break;
		case ConversionResolution.ConversionClass.Identity:
			break;
		}
	}

	private static void CompareGenericityBasedOnMethodGenericParams(ParameterInfo leftParameter, ParameterInfo rawLeftParameter, Symbols.Method leftMember, bool expandLeftParamArray, ParameterInfo rightParameter, ParameterInfo rawRightParameter, Symbols.Method rightMember, bool expandRightParamArray, ref bool leftIsLessGeneric, ref bool rightIsLessGeneric, ref bool signatureMismatch)
	{
		if (!leftMember.IsMethod || !rightMember.IsMethod)
		{
			return;
		}
		Type type = leftParameter.ParameterType;
		Type type2 = rightParameter.ParameterType;
		Type type3 = rawLeftParameter.ParameterType;
		Type type4 = rawRightParameter.ParameterType;
		if (type.IsByRef)
		{
			type = Symbols.GetElementType(type);
			type3 = Symbols.GetElementType(type3);
		}
		if (type2.IsByRef)
		{
			type2 = Symbols.GetElementType(type2);
			type4 = Symbols.GetElementType(type4);
		}
		if (expandLeftParamArray && Symbols.IsParamArray(leftParameter))
		{
			type = Symbols.GetElementType(type);
			type3 = Symbols.GetElementType(type3);
		}
		if (expandRightParamArray && Symbols.IsParamArray(rightParameter))
		{
			type2 = Symbols.GetElementType(type2);
			type4 = Symbols.GetElementType(type4);
		}
		if ((object)type != type2)
		{
			signatureMismatch = true;
			return;
		}
		MethodBase methodBase = leftMember.AsMethod();
		MethodBase methodBase2 = rightMember.AsMethod();
		if (Symbols.IsGeneric(methodBase))
		{
			methodBase = ((MethodInfo)methodBase).GetGenericMethodDefinition();
		}
		if (Symbols.IsGeneric(methodBase2))
		{
			methodBase2 = ((MethodInfo)methodBase2).GetGenericMethodDefinition();
		}
		if (Symbols.RefersToGenericParameter(type3, methodBase))
		{
			if (!Symbols.RefersToGenericParameter(type4, methodBase2))
			{
				rightIsLessGeneric = true;
			}
		}
		else if (Symbols.RefersToGenericParameter(type4, methodBase2) && !Symbols.RefersToGenericParameter(type3, methodBase))
		{
			leftIsLessGeneric = true;
		}
	}

	private static void CompareGenericityBasedOnTypeGenericParams(ParameterInfo leftParameter, ParameterInfo rawLeftParameter, Symbols.Method leftMember, bool expandLeftParamArray, ParameterInfo rightParameter, ParameterInfo rawRightParameter, Symbols.Method rightMember, bool expandRightParamArray, ref bool leftIsLessGeneric, ref bool rightIsLessGeneric, ref bool signatureMismatch)
	{
		Type type = leftParameter.ParameterType;
		Type type2 = rightParameter.ParameterType;
		Type type3 = rawLeftParameter.ParameterType;
		Type type4 = rawRightParameter.ParameterType;
		if (type.IsByRef)
		{
			type = Symbols.GetElementType(type);
			type3 = Symbols.GetElementType(type3);
		}
		if (type2.IsByRef)
		{
			type2 = Symbols.GetElementType(type2);
			type4 = Symbols.GetElementType(type4);
		}
		if (expandLeftParamArray && Symbols.IsParamArray(leftParameter))
		{
			type = Symbols.GetElementType(type);
			type3 = Symbols.GetElementType(type3);
		}
		if (expandRightParamArray && Symbols.IsParamArray(rightParameter))
		{
			type2 = Symbols.GetElementType(type2);
			type4 = Symbols.GetElementType(type4);
		}
		if ((object)type != type2)
		{
			signatureMismatch = true;
			return;
		}
		Type rawDeclaringType = leftMember.RawDeclaringType;
		Type rawDeclaringType2 = rightMember.RawDeclaringType;
		if (Symbols.RefersToGenericParameterCLRSemantics(type3, rawDeclaringType))
		{
			if (!Symbols.RefersToGenericParameterCLRSemantics(type4, rawDeclaringType2))
			{
				rightIsLessGeneric = true;
			}
		}
		else if (Symbols.RefersToGenericParameterCLRSemantics(type4, rawDeclaringType2))
		{
			leftIsLessGeneric = true;
		}
	}

	private static Symbols.Method LeastGenericProcedure(Symbols.Method left, Symbols.Method right, ComparisonType compareGenericity, ref bool signatureMismatch)
	{
		bool leftIsLessGeneric = false;
		bool rightIsLessGeneric = false;
		signatureMismatch = false;
		if (!left.IsMethod || !right.IsMethod)
		{
			return null;
		}
		int i = 0;
		int num = left.Parameters.Length;
		int num2;
		for (num2 = right.Parameters.Length; i < num && i < num2; i = checked(i + 1))
		{
			switch (compareGenericity)
			{
			case ComparisonType.GenericSpecificityBasedOnMethodGenericParams:
				CompareGenericityBasedOnMethodGenericParams(left.Parameters[i], left.RawParameters[i], left, left.ParamArrayExpanded, right.Parameters[i], right.RawParameters[i], right, expandRightParamArray: false, ref leftIsLessGeneric, ref rightIsLessGeneric, ref signatureMismatch);
				break;
			case ComparisonType.GenericSpecificityBasedOnTypeGenericParams:
				CompareGenericityBasedOnTypeGenericParams(left.Parameters[i], left.RawParameters[i], left, left.ParamArrayExpanded, right.Parameters[i], right.RawParameters[i], right, expandRightParamArray: false, ref leftIsLessGeneric, ref rightIsLessGeneric, ref signatureMismatch);
				break;
			}
			if (signatureMismatch || (leftIsLessGeneric && rightIsLessGeneric))
			{
				return null;
			}
		}
		if (i < num || i < num2)
		{
			return null;
		}
		if (leftIsLessGeneric)
		{
			return left;
		}
		if (rightIsLessGeneric)
		{
			return right;
		}
		return null;
	}

	internal static Symbols.Method LeastGenericProcedure(Symbols.Method left, Symbols.Method right)
	{
		if (!left.IsGeneric && !right.IsGeneric && !Symbols.IsGeneric(left.DeclaringType) && !Symbols.IsGeneric(right.DeclaringType))
		{
			return null;
		}
		bool signatureMismatch = false;
		Symbols.Method method = LeastGenericProcedure(left, right, ComparisonType.GenericSpecificityBasedOnMethodGenericParams, ref signatureMismatch);
		if ((object)method == null && !signatureMismatch)
		{
			method = LeastGenericProcedure(left, right, ComparisonType.GenericSpecificityBasedOnTypeGenericParams, ref signatureMismatch);
		}
		return method;
	}

	[RequiresUnreferencedCode("Calls RejectUncallableProcedure")]
	private static void InsertIfMethodAvailable(MemberInfo newCandidate, ParameterInfo[] newCandidateSignature, int newCandidateParamArrayIndex, bool expandNewCandidateParamArray, object[] arguments, int argumentCount, string[] argumentNames, Type[] typeArguments, bool collectOnlyOperators, List<Symbols.Method> candidates, Symbols.Container baseReference)
	{
		Symbols.Method method = null;
		checked
		{
			if (!collectOnlyOperators)
			{
				MethodBase methodBase = newCandidate as MethodBase;
				bool flag = false;
				if (newCandidate.MemberType == MemberTypes.Method && Symbols.IsRawGeneric(methodBase))
				{
					method = new Symbols.Method(methodBase, newCandidateSignature, newCandidateParamArrayIndex, expandNewCandidateParamArray);
					RejectUncallableProcedure(method, arguments, argumentNames, typeArguments);
					newCandidate = method.AsMethod();
					newCandidateSignature = method.Parameters;
				}
				if ((object)newCandidate != null && newCandidate.MemberType == MemberTypes.Method && Symbols.IsRawGeneric(newCandidate as MethodBase))
				{
					flag = true;
				}
				int num = candidates.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					Symbols.Method method2 = candidates[i];
					if ((object)method2 == null)
					{
						continue;
					}
					ParameterInfo[] parameters = method2.Parameters;
					MethodBase methodBase2 = ((!method2.IsMethod) ? null : method2.AsMethod());
					if (newCandidate == method2)
					{
						continue;
					}
					int num2 = 0;
					int num3 = 0;
					int num4 = 1;
					while (num4 <= argumentCount)
					{
						bool bothLose = false;
						bool leftWins = false;
						bool rightWins = false;
						CompareParameterSpecificity(null, newCandidateSignature[num2], methodBase, expandNewCandidateParamArray, parameters[num3], methodBase2, method2.ParamArrayExpanded, ref leftWins, ref rightWins, ref bothLose);
						if (!(bothLose | leftWins | rightWins))
						{
							if (num2 != newCandidateParamArrayIndex || !expandNewCandidateParamArray)
							{
								num2++;
							}
							if (num3 != method2.ParamArrayIndex || !method2.ParamArrayExpanded)
							{
								num3++;
							}
							num4++;
							continue;
						}
						goto IL_01f7;
					}
					if (!IsExactSignatureMatch(newCandidateSignature, Symbols.GetTypeParameters(newCandidate).Length, method2.Parameters, method2.TypeParameters.Length))
					{
						if (flag || ((object)methodBase2 != null && Symbols.IsRawGeneric(methodBase2)))
						{
							continue;
						}
						if (!expandNewCandidateParamArray && method2.ParamArrayExpanded)
						{
							candidates[i] = null;
							continue;
						}
						if (expandNewCandidateParamArray && !method2.ParamArrayExpanded)
						{
							return;
						}
						if (expandNewCandidateParamArray || method2.ParamArrayExpanded)
						{
							if (num2 > num3)
							{
								candidates[i] = null;
							}
							else if (num3 > num2)
							{
								return;
							}
						}
					}
					else
					{
						if ((object)newCandidate.DeclaringType == method2.DeclaringType || (baseReference != null && baseReference.IsWindowsRuntimeObject && Symbols.IsCollectionInterface(newCandidate.DeclaringType) && Symbols.IsCollectionInterface(method2.DeclaringType)))
						{
							break;
						}
						if (flag || (object)methodBase2 == null || !Symbols.IsRawGeneric(methodBase2))
						{
							return;
						}
					}
					IL_01f7:;
				}
			}
			if ((object)method != null)
			{
				candidates.Add(method);
			}
			else if (newCandidate.MemberType == MemberTypes.Property)
			{
				candidates.Add(new Symbols.Method((PropertyInfo)newCandidate, newCandidateSignature, newCandidateParamArrayIndex, expandNewCandidateParamArray));
			}
			else
			{
				candidates.Add(new Symbols.Method((MethodBase)newCandidate, newCandidateSignature, newCandidateParamArrayIndex, expandNewCandidateParamArray));
			}
		}
	}

	[RequiresUnreferencedCode("Calls InsertIfMethodAvailable")]
	internal static List<Symbols.Method> CollectOverloadCandidates(MemberInfo[] members, object[] arguments, int argumentCount, string[] argumentNames, Type[] typeArguments, bool collectOnlyOperators, Type terminatingScope, ref int rejectedForArgumentCount, ref int rejectedForTypeArgumentCount, Symbols.Container baseReference)
	{
		int num = 0;
		if (typeArguments != null)
		{
			num = typeArguments.Length;
		}
		List<Symbols.Method> list = new List<Symbols.Method>(members.Length);
		if (members.Length == 0)
		{
			return list;
		}
		bool flag = true;
		int num2 = 0;
		checked
		{
			do
			{
				Type declaringType = members[num2].DeclaringType;
				if ((object)terminatingScope != null && Symbols.IsOrInheritsFrom(terminatingScope, declaringType))
				{
					break;
				}
				do
				{
					MemberInfo memberInfo = members[num2];
					ParameterInfo[] array = null;
					int num3 = 0;
					int requiredParameterCount;
					int maximumParameterCount;
					int paramArrayIndex;
					bool flag2;
					switch (memberInfo.MemberType)
					{
					case MemberTypes.Constructor:
					case MemberTypes.Method:
					{
						MethodBase methodBase = (MethodBase)memberInfo;
						if (collectOnlyOperators && !Symbols.IsUserDefinedOperator(methodBase))
						{
							break;
						}
						array = methodBase.GetParameters();
						num3 = Symbols.GetTypeParameters(methodBase).Length;
						if (Symbols.IsShadows(methodBase))
						{
							flag = false;
						}
						goto IL_0162;
					}
					case MemberTypes.Property:
					{
						if (collectOnlyOperators)
						{
							break;
						}
						PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
						MethodInfo getMethod = propertyInfo.GetGetMethod();
						if ((object)getMethod != null)
						{
							array = getMethod.GetParameters();
							if (Symbols.IsShadows(getMethod))
							{
								flag = false;
							}
						}
						else
						{
							MethodInfo setMethod = propertyInfo.GetSetMethod();
							ParameterInfo[] parameters = setMethod.GetParameters();
							array = new ParameterInfo[parameters.Length - 2 + 1];
							Array.Copy(parameters, array, array.Length);
							if (Symbols.IsShadows(setMethod))
							{
								flag = false;
							}
						}
						goto IL_0162;
					}
					case MemberTypes.Event:
					case MemberTypes.Field:
					case MemberTypes.TypeInfo:
					case MemberTypes.Custom:
					case MemberTypes.NestedType:
						{
							if (!collectOnlyOperators)
							{
								flag = false;
							}
							break;
						}
						IL_0162:
						requiredParameterCount = 0;
						maximumParameterCount = 0;
						paramArrayIndex = -1;
						Symbols.GetAllParameterCounts(array, ref requiredParameterCount, ref maximumParameterCount, ref paramArrayIndex);
						flag2 = paramArrayIndex >= 0;
						if (argumentCount < requiredParameterCount || (!flag2 && argumentCount > maximumParameterCount))
						{
							rejectedForArgumentCount++;
							break;
						}
						if (num > 0 && num != num3)
						{
							rejectedForTypeArgumentCount++;
							break;
						}
						if (!flag2 || argumentCount == maximumParameterCount)
						{
							InsertIfMethodAvailable(memberInfo, array, paramArrayIndex, expandNewCandidateParamArray: false, arguments, argumentCount, argumentNames, typeArguments, collectOnlyOperators, list, baseReference);
						}
						if (flag2)
						{
							InsertIfMethodAvailable(memberInfo, array, paramArrayIndex, expandNewCandidateParamArray: true, arguments, argumentCount, argumentNames, typeArguments, collectOnlyOperators, list, baseReference);
						}
						break;
					}
					num2++;
				}
				while (num2 < members.Length && (object)members[num2].DeclaringType == declaringType);
			}
			while (flag && num2 < members.Length);
			for (num2 = 0; num2 < list.Count; num2++)
			{
				if ((object)list[num2] == null)
				{
					int i;
					for (i = num2 + 1; i < list.Count && (object)list[i] == null; i++)
					{
					}
					list.RemoveRange(num2, i - num2);
				}
			}
			return list;
		}
	}

	[RequiresUnreferencedCode("Calls ClassifyConversion")]
	private static bool CanConvert(Type targetType, Type sourceType, bool rejectNarrowingConversion, List<string> errors, string parameterName, bool isByRefCopyBackContext, ref bool requiresNarrowingConversion, ref bool allNarrowingIsFromObject)
	{
		Symbols.Method operatorMethod = null;
		ConversionResolution.ConversionClass conversionClass = ConversionResolution.ClassifyConversion(targetType, sourceType, ref operatorMethod);
		switch (conversionClass)
		{
		case ConversionResolution.ConversionClass.Identity:
		case ConversionResolution.ConversionClass.Widening:
			return true;
		case ConversionResolution.ConversionClass.Narrowing:
			if (rejectNarrowingConversion)
			{
				if (errors != null)
				{
					ReportError(errors, Interaction.IIf(isByRefCopyBackContext, System.SR.ArgumentNarrowingCopyBack3, System.SR.ArgumentNarrowing3), parameterName, sourceType, targetType);
				}
				return false;
			}
			requiresNarrowingConversion = true;
			if ((object)sourceType != typeof(object))
			{
				allNarrowingIsFromObject = false;
			}
			return true;
		default:
			if (errors != null)
			{
				ReportError(errors, Interaction.IIf(conversionClass == ConversionResolution.ConversionClass.Ambiguous, Interaction.IIf(isByRefCopyBackContext, System.SR.ArgumentMismatchAmbiguousCopyBack3, System.SR.ArgumentMismatchAmbiguous3), Interaction.IIf(isByRefCopyBackContext, System.SR.ArgumentMismatchCopyBack3, System.SR.ArgumentMismatch3)), parameterName, sourceType, targetType);
			}
			return false;
		}
	}

	[RequiresUnreferencedCode("Calls GetInterfaces on argument type recursively")]
	private static bool InferTypeArgumentsFromArgument(Type argumentType, Type parameterType, Type[] typeInferenceArguments, MethodBase targetProcedure, bool digThroughToBasesAndImplements)
	{
		bool flag = InferTypeArgumentsFromArgumentDirectly(argumentType, parameterType, typeInferenceArguments, targetProcedure, digThroughToBasesAndImplements);
		if (flag || !digThroughToBasesAndImplements || !Symbols.IsInstantiatedGeneric(parameterType) || (!parameterType.IsClass && !parameterType.IsInterface))
		{
			return flag;
		}
		Type genericTypeDefinition = parameterType.GetGenericTypeDefinition();
		if (Symbols.IsArrayType(argumentType))
		{
			if (argumentType.GetArrayRank() > 1 || parameterType.IsClass)
			{
				return false;
			}
			argumentType = typeof(IList<>).MakeGenericType(argumentType.GetElementType());
			if ((object)typeof(IList<>) == genericTypeDefinition)
			{
				goto IL_0149;
			}
		}
		else
		{
			if (!argumentType.IsClass && !argumentType.IsInterface)
			{
				return false;
			}
			if (Symbols.IsInstantiatedGeneric(argumentType) && (object)argumentType.GetGenericTypeDefinition() == genericTypeDefinition)
			{
				return false;
			}
		}
		if (parameterType.IsClass)
		{
			if (!argumentType.IsClass)
			{
				return false;
			}
			Type baseType = argumentType.BaseType;
			while ((object)baseType != null && (!Symbols.IsInstantiatedGeneric(baseType) || (object)baseType.GetGenericTypeDefinition() != genericTypeDefinition))
			{
				baseType = baseType.BaseType;
			}
			argumentType = baseType;
		}
		else
		{
			Type type = null;
			Type[] interfaces = argumentType.GetInterfaces();
			foreach (Type type2 in interfaces)
			{
				if (Symbols.IsInstantiatedGeneric(type2) && (object)type2.GetGenericTypeDefinition() == genericTypeDefinition)
				{
					if ((object)type != null)
					{
						return false;
					}
					type = type2;
				}
			}
			argumentType = type;
		}
		if ((object)argumentType == null)
		{
			return false;
		}
		goto IL_0149;
		IL_0149:
		return InferTypeArgumentsFromArgumentDirectly(argumentType, parameterType, typeInferenceArguments, targetProcedure, digThroughToBasesAndImplements);
	}

	[RequiresUnreferencedCode("Calls InferTypeArgumentsFromArgument")]
	private static bool InferTypeArgumentsFromArgumentDirectly(Type argumentType, Type parameterType, Type[] typeInferenceArguments, MethodBase targetProcedure, bool digThroughToBasesAndImplements)
	{
		if (!Symbols.RefersToGenericParameter(parameterType, targetProcedure))
		{
			return true;
		}
		checked
		{
			if (Symbols.IsGenericParameter(parameterType))
			{
				if (Symbols.AreGenericMethodDefsEqual(parameterType.DeclaringMethod, targetProcedure))
				{
					int genericParameterPosition = parameterType.GenericParameterPosition;
					if ((object)typeInferenceArguments[genericParameterPosition] == null)
					{
						typeInferenceArguments[genericParameterPosition] = argumentType;
					}
					else if ((object)typeInferenceArguments[genericParameterPosition] != argumentType)
					{
						return false;
					}
				}
			}
			else
			{
				if (Symbols.IsInstantiatedGeneric(parameterType))
				{
					Type type = null;
					if (Symbols.IsInstantiatedGeneric(argumentType) && (object)argumentType.GetGenericTypeDefinition() == parameterType.GetGenericTypeDefinition())
					{
						type = argumentType;
					}
					if ((object)type == null && digThroughToBasesAndImplements)
					{
						Type[] interfaces = argumentType.GetInterfaces();
						foreach (Type type2 in interfaces)
						{
							if (Symbols.IsInstantiatedGeneric(type2) && (object)type2.GetGenericTypeDefinition() == parameterType.GetGenericTypeDefinition())
							{
								if ((object)type != null)
								{
									return false;
								}
								type = type2;
							}
						}
					}
					if ((object)type != null)
					{
						Type[] typeArguments = Symbols.GetTypeArguments(parameterType);
						Type[] typeArguments2 = Symbols.GetTypeArguments(type);
						int num = typeArguments2.Length - 1;
						for (int j = 0; j <= num; j++)
						{
							if (!InferTypeArgumentsFromArgument(typeArguments2[j], typeArguments[j], typeInferenceArguments, targetProcedure, digThroughToBasesAndImplements: false))
							{
								return false;
							}
						}
						return true;
					}
					return false;
				}
				if (Symbols.IsArrayType(parameterType))
				{
					if (Symbols.IsArrayType(argumentType) && parameterType.GetArrayRank() == argumentType.GetArrayRank())
					{
						return InferTypeArgumentsFromArgument(Symbols.GetElementType(argumentType), Symbols.GetElementType(parameterType), typeInferenceArguments, targetProcedure, digThroughToBasesAndImplements);
					}
					return false;
				}
			}
			return true;
		}
	}

	[RequiresUnreferencedCode("Calls ClassifyConversion")]
	private static bool CanPassToParamArray(Symbols.Method targetProcedure, object argument, ParameterInfo parameter)
	{
		if (argument == null)
		{
			return true;
		}
		Type parameterType = parameter.ParameterType;
		Type argumentType = GetArgumentType(argument);
		Symbols.Method operatorMethod = null;
		ConversionResolution.ConversionClass conversionClass = ConversionResolution.ClassifyConversion(parameterType, argumentType, ref operatorMethod);
		return conversionClass == ConversionResolution.ConversionClass.Widening || conversionClass == ConversionResolution.ConversionClass.Identity;
	}

	[RequiresUnreferencedCode("Calls CanConvert")]
	internal static bool CanPassToParameter(Symbols.Method targetProcedure, object argument, ParameterInfo parameter, bool isExpandedParamArray, bool rejectNarrowingConversions, List<string> errors, ref bool requiresNarrowingConversion, ref bool allNarrowingIsFromObject)
	{
		if (argument == null)
		{
			return true;
		}
		Type type = parameter.ParameterType;
		bool isByRef = type.IsByRef;
		if (isByRef || isExpandedParamArray)
		{
			type = Symbols.GetElementType(type);
		}
		Type argumentType = GetArgumentType(argument);
		if (argument == Missing.Value)
		{
			if (parameter.IsOptional)
			{
				return true;
			}
			if (!Symbols.IsRootObjectType(type) || !isExpandedParamArray)
			{
				if (errors != null)
				{
					if (isExpandedParamArray)
					{
						ReportError(errors, System.SR.OmittedParamArrayArgument);
					}
					else
					{
						ReportError(errors, System.SR.OmittedArgument1, parameter.Name);
					}
				}
				return false;
			}
		}
		bool flag = CanConvert(type, argumentType, rejectNarrowingConversions, errors, parameter.Name, isByRefCopyBackContext: false, ref requiresNarrowingConversion, ref allNarrowingIsFromObject);
		if (!isByRef || !flag)
		{
			return flag;
		}
		return CanConvert(argumentType, type, rejectNarrowingConversions, errors, parameter.Name, isByRefCopyBackContext: true, ref requiresNarrowingConversion, ref allNarrowingIsFromObject);
	}

	[RequiresUnreferencedCode("Calls InferTypArgumentsFromArgument")]
	internal static bool InferTypeArgumentsFromArgument(Symbols.Method targetProcedure, object argument, ParameterInfo parameter, bool isExpandedParamArray, List<string> errors)
	{
		if (argument == null)
		{
			return true;
		}
		Type type = parameter.ParameterType;
		if (type.IsByRef || isExpandedParamArray)
		{
			type = Symbols.GetElementType(type);
		}
		if (!InferTypeArgumentsFromArgument(GetArgumentType(argument), type, targetProcedure.TypeArguments, targetProcedure.AsMethod(), digThroughToBasesAndImplements: true))
		{
			if (errors != null)
			{
				ReportError(errors, System.SR.TypeInferenceFails1, parameter.Name);
			}
			return false;
		}
		return true;
	}

	[RequiresUnreferencedCode("Uses Type.GetElementType which cannot be statically analyzed.")]
	internal static object PassToParameter(object argument, ParameterInfo parameter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type parameterType)
	{
		bool isByRef = parameterType.IsByRef;
		if (isByRef)
		{
			parameterType = parameterType.GetElementType();
		}
		if (argument is Symbols.TypedNothing)
		{
			argument = null;
		}
		if (argument == Missing.Value && parameter.IsOptional)
		{
			argument = parameter.DefaultValue;
		}
		if (isByRef)
		{
			Type argumentType = GetArgumentType(argument);
			if ((object)argumentType != null && Symbols.IsValueType(argumentType))
			{
				argument = Conversions.ForceValueCopy(argument, argumentType);
			}
		}
		return Conversions.ChangeType(argument, parameterType);
	}

	private static bool FindParameterByName(ParameterInfo[] parameters, string name, ref int index)
	{
		for (int i = 0; i < parameters.Length; i = checked(i + 1))
		{
			if (Operators.CompareString(name, parameters[i].Name, TextCompare: true) == 0)
			{
				index = i;
				return true;
			}
		}
		return false;
	}

	private static bool[] CreateMatchTable(int size, int lastPositionalMatchIndex)
	{
		checked
		{
			bool[] array = new bool[size - 1 + 1];
			for (int i = 0; i <= lastPositionalMatchIndex; i++)
			{
				array[i] = true;
			}
			return array;
		}
	}

	[RequiresUnreferencedCode("Calls InstantiateGenericMethod")]
	internal static bool CanMatchArguments(Symbols.Method targetProcedure, object[] arguments, string[] argumentNames, Type[] typeArguments, bool rejectNarrowingConversions, List<string> errors)
	{
		bool flag = errors != null;
		targetProcedure.ArgumentsValidated = true;
		checked
		{
			if (targetProcedure.IsMethod && Symbols.IsRawGeneric(targetProcedure.AsMethod()))
			{
				if (typeArguments.Length == 0)
				{
					typeArguments = new Type[targetProcedure.TypeParameters.Length - 1 + 1];
					targetProcedure.TypeArguments = typeArguments;
					if (!InferTypeArguments(targetProcedure, arguments, argumentNames, typeArguments, errors))
					{
						return false;
					}
				}
				else
				{
					targetProcedure.TypeArguments = typeArguments;
				}
				if (!InstantiateGenericMethod(targetProcedure, typeArguments, errors))
				{
					return false;
				}
			}
			ParameterInfo[] parameters = targetProcedure.Parameters;
			int i = argumentNames.Length;
			int index = 0;
			while (i < arguments.Length && index != targetProcedure.ParamArrayIndex)
			{
				if (!CanPassToParameter(targetProcedure, arguments[i], parameters[index], isExpandedParamArray: false, rejectNarrowingConversions, errors, ref targetProcedure.RequiresNarrowingConversion, ref targetProcedure.AllNarrowingIsFromObject) && !flag)
				{
					return false;
				}
				i++;
				index++;
			}
			if (targetProcedure.HasParamArray)
			{
				if (targetProcedure.ParamArrayExpanded)
				{
					if (i == arguments.Length - 1 && arguments[i] == null)
					{
						return false;
					}
					for (; i < arguments.Length; i++)
					{
						if (!CanPassToParameter(targetProcedure, arguments[i], parameters[index], isExpandedParamArray: true, rejectNarrowingConversions, errors, ref targetProcedure.RequiresNarrowingConversion, ref targetProcedure.AllNarrowingIsFromObject) && !flag)
						{
							return false;
						}
					}
				}
				else
				{
					if (arguments.Length - i != 1)
					{
						return false;
					}
					if (!CanPassToParamArray(targetProcedure, arguments[i], parameters[index]))
					{
						if (flag)
						{
							ReportError(errors, System.SR.ArgumentMismatch3, parameters[index].Name, GetArgumentType(arguments[i]), parameters[index].ParameterType);
						}
						return false;
					}
				}
				index++;
			}
			bool[] array = null;
			if (argumentNames.Length > 0 || index < parameters.Length)
			{
				array = CreateMatchTable(parameters.Length, index - 1);
			}
			if (argumentNames.Length > 0)
			{
				int[] array2 = new int[argumentNames.Length - 1 + 1];
				for (i = 0; i < argumentNames.Length; i++)
				{
					if (!FindParameterByName(parameters, argumentNames[i], ref index))
					{
						if (!flag)
						{
							return false;
						}
						ReportError(errors, System.SR.NamedParamNotFound2, argumentNames[i], targetProcedure);
						continue;
					}
					if (index == targetProcedure.ParamArrayIndex)
					{
						if (!flag)
						{
							return false;
						}
						ReportError(errors, System.SR.NamedParamArrayArgument1, argumentNames[i]);
						continue;
					}
					if (array[index])
					{
						if (!flag)
						{
							return false;
						}
						ReportError(errors, System.SR.NamedArgUsedTwice2, argumentNames[i], targetProcedure);
						continue;
					}
					if (!CanPassToParameter(targetProcedure, arguments[i], parameters[index], isExpandedParamArray: false, rejectNarrowingConversions, errors, ref targetProcedure.RequiresNarrowingConversion, ref targetProcedure.AllNarrowingIsFromObject) && !flag)
					{
						return false;
					}
					array[index] = true;
					array2[i] = index;
				}
				targetProcedure.NamedArgumentMapping = array2;
			}
			if (array != null)
			{
				int num = array.Length - 1;
				for (int j = 0; j <= num; j++)
				{
					if (!array[j] && !parameters[j].IsOptional)
					{
						if (!flag)
						{
							return false;
						}
						ReportError(errors, System.SR.OmittedArgument1, parameters[j].Name);
					}
				}
			}
			if (errors != null && errors.Count > 0)
			{
				return false;
			}
			return true;
		}
	}

	[RequiresUnreferencedCode("Calls Method.BindGenericArguments")]
	private static bool InstantiateGenericMethod(Symbols.Method targetProcedure, Type[] typeArguments, List<string> errors)
	{
		bool flag = errors != null;
		checked
		{
			int num = typeArguments.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if ((object)typeArguments[i] == null)
				{
					if (!flag)
					{
						return false;
					}
					ReportError(errors, System.SR.UnboundTypeParam1, targetProcedure.TypeParameters[i].Name);
				}
			}
			if ((errors == null || errors.Count == 0) && !targetProcedure.BindGenericArguments())
			{
				if (!flag)
				{
					return false;
				}
				ReportError(errors, System.SR.FailedTypeArgumentBinding);
			}
			if (errors != null && errors.Count > 0)
			{
				return false;
			}
			return true;
		}
	}

	[RequiresUnreferencedCode("Cannot statically analyze the parameter types of the targetProcedure")]
	internal static void MatchArguments(Symbols.Method targetProcedure, object[] arguments, object[] matchedArguments)
	{
		ParameterInfo[] parameters = targetProcedure.Parameters;
		int[] namedArgumentMapping = targetProcedure.NamedArgumentMapping;
		int num = 0;
		if (namedArgumentMapping != null)
		{
			num = namedArgumentMapping.Length;
		}
		int num2 = 0;
		checked
		{
			while (num < arguments.Length && num2 != targetProcedure.ParamArrayIndex)
			{
				matchedArguments[num2] = PassToParameter(arguments[num], parameters[num2], parameters[num2].ParameterType);
				num++;
				num2++;
			}
			if (targetProcedure.HasParamArray)
			{
				if (targetProcedure.ParamArrayExpanded)
				{
					int length = arguments.Length - num;
					ParameterInfo parameterInfo = parameters[num2];
					Type elementType = parameterInfo.ParameterType.GetElementType();
					Array array = Array.CreateInstance(elementType, length);
					int num3 = 0;
					while (num < arguments.Length)
					{
						array.SetValue(PassToParameter(arguments[num], parameterInfo, elementType), num3);
						num++;
						num3++;
					}
					matchedArguments[num2] = array;
				}
				else
				{
					matchedArguments[num2] = PassToParameter(arguments[num], parameters[num2], parameters[num2].ParameterType);
				}
				num2++;
			}
			bool[] array2 = null;
			if (namedArgumentMapping != null || num2 < parameters.Length)
			{
				array2 = CreateMatchTable(parameters.Length, num2 - 1);
			}
			if (namedArgumentMapping != null)
			{
				for (num = 0; num < namedArgumentMapping.Length; num++)
				{
					num2 = namedArgumentMapping[num];
					matchedArguments[num2] = PassToParameter(arguments[num], parameters[num2], parameters[num2].ParameterType);
					array2[num2] = true;
				}
			}
			if (array2 == null)
			{
				return;
			}
			int num4 = array2.Length - 1;
			for (int i = 0; i <= num4; i++)
			{
				if (!array2[i])
				{
					matchedArguments[i] = PassToParameter(Missing.Value, parameters[i], parameters[i].ParameterType);
				}
			}
		}
	}

	[RequiresUnreferencedCode("Calls InferTypeArgumentsFromArgument")]
	private static bool InferTypeArguments(Symbols.Method targetProcedure, object[] arguments, string[] argumentNames, Type[] typeArguments, List<string> errors)
	{
		bool flag = errors != null;
		ParameterInfo[] rawParameters = targetProcedure.RawParameters;
		int i = argumentNames.Length;
		int index = 0;
		checked
		{
			while (i < arguments.Length && index != targetProcedure.ParamArrayIndex)
			{
				if (!InferTypeArgumentsFromArgument(targetProcedure, arguments[i], rawParameters[index], isExpandedParamArray: false, errors) && !flag)
				{
					return false;
				}
				i++;
				index++;
			}
			if (targetProcedure.HasParamArray)
			{
				if (targetProcedure.ParamArrayExpanded)
				{
					for (; i < arguments.Length; i++)
					{
						if (!InferTypeArgumentsFromArgument(targetProcedure, arguments[i], rawParameters[index], isExpandedParamArray: true, errors) && !flag)
						{
							return false;
						}
					}
				}
				else
				{
					if (arguments.Length - i != 1)
					{
						return true;
					}
					if (!InferTypeArgumentsFromArgument(targetProcedure, arguments[i], rawParameters[index], isExpandedParamArray: false, errors))
					{
						return false;
					}
				}
				index++;
			}
			if (argumentNames.Length > 0)
			{
				for (i = 0; i < argumentNames.Length; i++)
				{
					if (FindParameterByName(rawParameters, argumentNames[i], ref index) && index != targetProcedure.ParamArrayIndex && !InferTypeArgumentsFromArgument(targetProcedure, arguments[i], rawParameters[index], isExpandedParamArray: false, errors) && !flag)
					{
						return false;
					}
				}
			}
			if (errors != null && errors.Count > 0)
			{
				return false;
			}
			return true;
		}
	}

	internal static void ReorderArgumentArray(Symbols.Method targetProcedure, object[] parameterResults, object[] arguments, bool[] copyBack, BindingFlags lookupFlags)
	{
		if (copyBack == null)
		{
			return;
		}
		checked
		{
			int num = copyBack.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				copyBack[i] = false;
			}
			if (Symbols.HasFlag(lookupFlags, BindingFlags.SetProperty) || !targetProcedure.HasByRefParameter)
			{
				return;
			}
			ParameterInfo[] parameters = targetProcedure.Parameters;
			int[] namedArgumentMapping = targetProcedure.NamedArgumentMapping;
			int num2 = 0;
			if (namedArgumentMapping != null)
			{
				num2 = namedArgumentMapping.Length;
			}
			int num3 = 0;
			while (num2 < arguments.Length && num3 != targetProcedure.ParamArrayIndex)
			{
				if (parameters[num3].ParameterType.IsByRef)
				{
					arguments[num2] = parameterResults[num3];
					copyBack[num2] = true;
				}
				num2++;
				num3++;
			}
			if (namedArgumentMapping == null)
			{
				return;
			}
			for (num2 = 0; num2 < namedArgumentMapping.Length; num2++)
			{
				num3 = namedArgumentMapping[num2];
				if (parameters[num3].ParameterType.IsByRef)
				{
					arguments[num2] = parameterResults[num3];
					copyBack[num2] = true;
				}
			}
		}
	}

	[RequiresUnreferencedCode("Calls RejectUncallableProcedure")]
	private static Symbols.Method RejectUncallableProcedures(List<Symbols.Method> candidates, object[] arguments, string[] argumentNames, Type[] typeArguments, ref int candidateCount, ref bool someCandidatesAreGeneric)
	{
		Symbols.Method result = null;
		checked
		{
			int num = candidates.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				Symbols.Method method = candidates[i];
				if (!method.ArgumentMatchingDone)
				{
					RejectUncallableProcedure(method, arguments, argumentNames, typeArguments);
				}
				if (method.NotCallable)
				{
					candidateCount--;
					continue;
				}
				result = method;
				if (method.IsGeneric || Symbols.IsGeneric(method.DeclaringType))
				{
					someCandidatesAreGeneric = true;
				}
			}
			return result;
		}
	}

	[RequiresUnreferencedCode("Calls CanMatchArguments")]
	private static void RejectUncallableProcedure(Symbols.Method candidate, object[] arguments, string[] argumentNames, Type[] typeArguments)
	{
		if (!CanMatchArguments(candidate, arguments, argumentNames, typeArguments, rejectNarrowingConversions: false, null))
		{
			candidate.NotCallable = true;
		}
		candidate.ArgumentMatchingDone = true;
	}

	private static Type GetArgumentType(object argument)
	{
		if (argument == null)
		{
			return null;
		}
		if (!(argument is Symbols.TypedNothing { Type: var type }))
		{
			return argument.GetType();
		}
		return type;
	}

	[RequiresUnreferencedCode("Calls Method.RawParametersFromType")]
	private static Symbols.Method MoreSpecificProcedure(Symbols.Method left, Symbols.Method right, object[] arguments, string[] argumentNames, ComparisonType compareGenericity, [Optional][DefaultParameterValue(false)] ref bool bothLose, bool continueWhenBothLose = false)
	{
		bothLose = false;
		bool leftWins = false;
		bool rightWins = false;
		MethodBase leftProcedure = ((!left.IsMethod) ? null : left.AsMethod());
		MethodBase rightProcedure = ((!right.IsMethod) ? null : right.AsMethod());
		int index = 0;
		int index2 = 0;
		checked
		{
			for (int i = argumentNames.Length; i < arguments.Length; i++)
			{
				Type argumentType = GetArgumentType(arguments[i]);
				switch (compareGenericity)
				{
				case ComparisonType.GenericSpecificityBasedOnMethodGenericParams:
					CompareGenericityBasedOnMethodGenericParams(left.Parameters[index], left.RawParameters[index], left, left.ParamArrayExpanded, right.Parameters[index2], right.RawParameters[index2], right, right.ParamArrayExpanded, ref leftWins, ref rightWins, ref bothLose);
					break;
				case ComparisonType.GenericSpecificityBasedOnTypeGenericParams:
					CompareGenericityBasedOnTypeGenericParams(left.Parameters[index], left.RawParametersFromType[index], left, left.ParamArrayExpanded, right.Parameters[index2], right.RawParametersFromType[index2], right, right.ParamArrayExpanded, ref leftWins, ref rightWins, ref bothLose);
					break;
				case ComparisonType.ParameterSpecificty:
					CompareParameterSpecificity(argumentType, left.Parameters[index], leftProcedure, left.ParamArrayExpanded, right.Parameters[index2], rightProcedure, right.ParamArrayExpanded, ref leftWins, ref rightWins, ref bothLose);
					break;
				}
				if ((bothLose && !continueWhenBothLose) || (leftWins && rightWins))
				{
					return null;
				}
				if (index != left.ParamArrayIndex)
				{
					index++;
				}
				if (index2 != right.ParamArrayIndex)
				{
					index2++;
				}
			}
			for (int i = 0; i < argumentNames.Length; i++)
			{
				bool num = FindParameterByName(left.Parameters, argumentNames[i], ref index);
				bool flag = FindParameterByName(right.Parameters, argumentNames[i], ref index2);
				if (!num || !flag)
				{
					throw new InternalErrorException();
				}
				Type argumentType2 = GetArgumentType(arguments[i]);
				switch (compareGenericity)
				{
				case ComparisonType.GenericSpecificityBasedOnMethodGenericParams:
					CompareGenericityBasedOnMethodGenericParams(left.Parameters[index], left.RawParameters[index], left, expandLeftParamArray: true, right.Parameters[index2], right.RawParameters[index2], right, expandRightParamArray: true, ref leftWins, ref rightWins, ref bothLose);
					break;
				case ComparisonType.GenericSpecificityBasedOnTypeGenericParams:
					CompareGenericityBasedOnTypeGenericParams(left.Parameters[index], left.RawParameters[index], left, expandLeftParamArray: true, right.Parameters[index2], right.RawParameters[index2], right, expandRightParamArray: true, ref leftWins, ref rightWins, ref bothLose);
					break;
				case ComparisonType.ParameterSpecificty:
					CompareParameterSpecificity(argumentType2, left.Parameters[index], leftProcedure, expandLeftParamArray: true, right.Parameters[index2], rightProcedure, expandRightParamArray: true, ref leftWins, ref rightWins, ref bothLose);
					break;
				}
				if ((bothLose && !continueWhenBothLose) || (leftWins && rightWins))
				{
					return null;
				}
			}
			if (leftWins)
			{
				return left;
			}
			if (rightWins)
			{
				return right;
			}
			return null;
		}
	}

	[RequiresUnreferencedCode("Calls MoreSpecificProcedure")]
	private static Symbols.Method MostSpecificProcedure(List<Symbols.Method> candidates, ref int candidateCount, object[] arguments, string[] argumentNames)
	{
		checked
		{
			foreach (Symbols.Method candidate in candidates)
			{
				if (candidate.NotCallable || candidate.RequiresNarrowingConversion)
				{
					continue;
				}
				bool flag = true;
				foreach (Symbols.Method candidate2 in candidates)
				{
					if (candidate2.NotCallable || candidate2.RequiresNarrowingConversion || (candidate2 == candidate && candidate2.ParamArrayExpanded == candidate.ParamArrayExpanded))
					{
						continue;
					}
					bool bothLose = false;
					Symbols.Method method = MoreSpecificProcedure(candidate, candidate2, arguments, argumentNames, ComparisonType.ParameterSpecificty, ref bothLose, continueWhenBothLose: true);
					if ((object)method == candidate)
					{
						if (!candidate2.LessSpecific)
						{
							candidate2.LessSpecific = true;
							candidateCount--;
						}
						continue;
					}
					flag = false;
					if ((object)method == candidate2 && !candidate.LessSpecific)
					{
						candidate.LessSpecific = true;
						candidateCount--;
					}
				}
				if (flag)
				{
					return candidate;
				}
			}
			return null;
		}
	}

	[RequiresUnreferencedCode("Calls MoreSpecificProcedure")]
	private static Symbols.Method RemoveRedundantGenericProcedures(List<Symbols.Method> candidates, ref int candidateCount, object[] arguments, string[] argumentNames)
	{
		checked
		{
			int num = candidates.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				Symbols.Method method = candidates[i];
				if (method.NotCallable)
				{
					continue;
				}
				int num2 = i + 1;
				int num3 = candidates.Count - 1;
				for (int j = num2; j <= num3; j++)
				{
					Symbols.Method method2 = candidates[j];
					if (method2.NotCallable || method.RequiresNarrowingConversion != method2.RequiresNarrowingConversion)
					{
						continue;
					}
					Symbols.Method method3 = null;
					bool bothLose = false;
					if (method.IsGeneric || method2.IsGeneric)
					{
						method3 = MoreSpecificProcedure(method, method2, arguments, argumentNames, ComparisonType.GenericSpecificityBasedOnMethodGenericParams, ref bothLose);
						if ((object)method3 != null)
						{
							candidateCount--;
							if (candidateCount == 1)
							{
								return method3;
							}
							if ((object)method3 != method)
							{
								method.NotCallable = true;
								break;
							}
							method2.NotCallable = true;
						}
					}
					if (bothLose || (object)method3 != null || (!Symbols.IsGeneric(method.DeclaringType) && !Symbols.IsGeneric(method2.DeclaringType)))
					{
						continue;
					}
					method3 = MoreSpecificProcedure(method, method2, arguments, argumentNames, ComparisonType.GenericSpecificityBasedOnTypeGenericParams, ref bothLose);
					if ((object)method3 != null)
					{
						candidateCount--;
						if (candidateCount == 1)
						{
							return method3;
						}
						if ((object)method3 != method)
						{
							method.NotCallable = true;
							break;
						}
						method2.NotCallable = true;
					}
				}
			}
			return null;
		}
	}

	private static void ReportError(List<string> errors, string resourceID, string substitution1, Type substitution2, Type substitution3)
	{
		errors.Add(System.SR.Format(resourceID, substitution1, Utils.VBFriendlyName(substitution2), Utils.VBFriendlyName(substitution3)));
	}

	private static void ReportError(List<string> errors, string resourceID, string substitution1, Symbols.Method substitution2)
	{
		errors.Add(System.SR.Format(resourceID, substitution1, substitution2.ToString()));
	}

	private static void ReportError(List<string> errors, string resourceID, string substitution1)
	{
		errors.Add(System.SR.Format(resourceID, substitution1));
	}

	private static void ReportError(List<string> errors, string resourceID)
	{
		errors.Add(resourceID);
	}

	private static Exception ReportOverloadResolutionFailure(string overloadedProcedureName, List<Symbols.Method> candidates, object[] arguments, string[] argumentNames, Type[] typeArguments, string errorID, ResolutionFailure failure, ArgumentDetector detector, CandidateProperty candidateFilter)
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<string> list = new List<string>();
		int num = 0;
		checked
		{
			int num2 = candidates.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				Symbols.Method method = candidates[i];
				if (!candidateFilter(method))
				{
					continue;
				}
				if (method.HasParamArray)
				{
					int num3 = i + 1;
					while (num3 < candidates.Count)
					{
						if (!candidateFilter(candidates[num3]) || !(candidates[num3] == method))
						{
							num3++;
							continue;
						}
						goto IL_00fe;
					}
				}
				num++;
				list.Clear();
				detector(method, arguments, argumentNames, typeArguments, list);
				stringBuilder.Append("\r\n    '");
				stringBuilder.Append(method.ToString());
				stringBuilder.Append("':");
				foreach (string item in list)
				{
					stringBuilder.Append("\r\n        ");
					stringBuilder.Append(item);
				}
				IL_00fe:;
			}
			string message = System.SR.Format(errorID, overloadedProcedureName, stringBuilder.ToString());
			if (num == 1)
			{
				return new InvalidCastException(message);
			}
			return new AmbiguousMatchException(message);
		}
	}

	[RequiresUnreferencedCode("Calls CanMatchArguments")]
	private static bool DetectArgumentErrors(Symbols.Method targetProcedure, object[] arguments, string[] argumentNames, Type[] typeArguments, List<string> errors)
	{
		return CanMatchArguments(targetProcedure, arguments, argumentNames, typeArguments, rejectNarrowingConversions: false, errors);
	}

	private static bool CandidateIsNotCallable(Symbols.Method candidate)
	{
		return candidate.NotCallable;
	}

	[RequiresUnreferencedCode("Calls ReportOverloadResolutionFailure")]
	private static Exception ReportUncallableProcedures(string overloadedProcedureName, List<Symbols.Method> candidates, object[] arguments, string[] argumentNames, Type[] typeArguments, ResolutionFailure failure)
	{
		return ReportOverloadResolutionFailure(overloadedProcedureName, candidates, arguments, argumentNames, typeArguments, System.SR.NoCallableOverloadCandidates2, failure, DetectArgumentErrors, CandidateIsNotCallable);
	}

	[RequiresUnreferencedCode("Calls CanMatchArguments")]
	private static bool DetectArgumentNarrowing(Symbols.Method targetProcedure, object[] arguments, string[] argumentNames, Type[] typeArguments, List<string> errors)
	{
		return CanMatchArguments(targetProcedure, arguments, argumentNames, typeArguments, rejectNarrowingConversions: true, errors);
	}

	private static bool CandidateIsNarrowing(Symbols.Method candidate)
	{
		if (!candidate.NotCallable)
		{
			return candidate.RequiresNarrowingConversion;
		}
		return false;
	}

	[RequiresUnreferencedCode("Calls ReportOverloadResolutionFailure")]
	private static Exception ReportNarrowingProcedures(string overloadedProcedureName, List<Symbols.Method> candidates, object[] arguments, string[] argumentNames, Type[] typeArguments, ResolutionFailure failure)
	{
		return ReportOverloadResolutionFailure(overloadedProcedureName, candidates, arguments, argumentNames, typeArguments, System.SR.NoNonNarrowingOverloadCandidates2, failure, DetectArgumentNarrowing, CandidateIsNarrowing);
	}

	private static bool DetectUnspecificity(Symbols.Method targetProcedure, object[] arguments, string[] argumentNames, Type[] typeArguments, List<string> errors)
	{
		ReportError(errors, System.SR.NotMostSpecificOverload);
		return false;
	}

	private static bool CandidateIsUnspecific(Symbols.Method candidate)
	{
		if (!candidate.NotCallable && !candidate.RequiresNarrowingConversion)
		{
			return !candidate.LessSpecific;
		}
		return false;
	}

	private static Exception ReportUnspecificProcedures(string overloadedProcedureName, List<Symbols.Method> candidates, ResolutionFailure failure)
	{
		return ReportOverloadResolutionFailure(overloadedProcedureName, candidates, null, null, null, System.SR.NoMostSpecificOverload2, failure, DetectUnspecificity, CandidateIsUnspecific);
	}

	[RequiresUnreferencedCode("Calls MostSpecificProcedure and RemoveRedundantGenericProcedures")]
	internal static Symbols.Method ResolveOverloadedCall(string methodName, List<Symbols.Method> candidates, object[] arguments, string[] argumentNames, Type[] typeArguments, BindingFlags lookupFlags, bool reportErrors, ref ResolutionFailure failure)
	{
		failure = ResolutionFailure.None;
		int candidateCount = candidates.Count;
		bool someCandidatesAreGeneric = false;
		Symbols.Method result = RejectUncallableProcedures(candidates, arguments, argumentNames, typeArguments, ref candidateCount, ref someCandidatesAreGeneric);
		checked
		{
			switch (candidateCount)
			{
			case 1:
				return result;
			case 0:
				failure = ResolutionFailure.InvalidArgument;
				if (reportErrors)
				{
					throw ReportUncallableProcedures(methodName, candidates, arguments, argumentNames, typeArguments, failure);
				}
				return null;
			default:
			{
				if (someCandidatesAreGeneric)
				{
					result = RemoveRedundantGenericProcedures(candidates, ref candidateCount, arguments, argumentNames);
					if (candidateCount == 1)
					{
						return result;
					}
				}
				int num = 0;
				Symbols.Method result2 = null;
				foreach (Symbols.Method candidate in candidates)
				{
					if (candidate.NotCallable)
					{
						continue;
					}
					if (candidate.RequiresNarrowingConversion)
					{
						candidateCount--;
						if (candidate.AllNarrowingIsFromObject)
						{
							num++;
							result2 = candidate;
						}
					}
					else
					{
						result = candidate;
					}
				}
				switch (candidateCount)
				{
				case 1:
					return result;
				case 0:
					if (num == 1)
					{
						return result2;
					}
					failure = ResolutionFailure.AmbiguousMatch;
					if (reportErrors)
					{
						throw ReportNarrowingProcedures(methodName, candidates, arguments, argumentNames, typeArguments, failure);
					}
					return null;
				default:
					result = MostSpecificProcedure(candidates, ref candidateCount, arguments, argumentNames);
					if ((object)result != null)
					{
						return result;
					}
					failure = ResolutionFailure.AmbiguousMatch;
					if (reportErrors)
					{
						throw ReportUnspecificProcedures(methodName, candidates, failure);
					}
					return null;
				}
			}
			}
		}
	}

	[RequiresUnreferencedCode("Calls ResolveOverloadedCall")]
	internal static Symbols.Method ResolveOverloadedCall(string methodName, MemberInfo[] members, object[] arguments, string[] argumentNames, Type[] typeArguments, BindingFlags lookupFlags, bool reportErrors, ref ResolutionFailure failure, Symbols.Container baseReference)
	{
		int rejectedForArgumentCount = 0;
		int rejectedForTypeArgumentCount = 0;
		List<Symbols.Method> list = CollectOverloadCandidates(members, arguments, arguments.Length, argumentNames, typeArguments, collectOnlyOperators: false, null, ref rejectedForArgumentCount, ref rejectedForTypeArgumentCount, baseReference);
		if (list.Count == 1 && !list[0].NotCallable)
		{
			return list[0];
		}
		if (list.Count == 0)
		{
			failure = ResolutionFailure.MissingMember;
			if (reportErrors)
			{
				string resourceFormat = System.SR.NoViableOverloadCandidates1;
				if (rejectedForArgumentCount > 0)
				{
					resourceFormat = System.SR.NoArgumentCountOverloadCandidates1;
				}
				else if (rejectedForTypeArgumentCount > 0)
				{
					resourceFormat = System.SR.NoTypeArgumentCountOverloadCandidates1;
				}
				throw new MissingMemberException(System.SR.Format(resourceFormat, methodName));
			}
			return null;
		}
		return ResolveOverloadedCall(methodName, list, arguments, argumentNames, typeArguments, lookupFlags, reportErrors, ref failure);
	}
}
