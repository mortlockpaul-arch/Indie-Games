using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class MethodTypeInferrer
{
	private enum NewInferenceResult
	{
		InferenceFailed,
		MadeProgress,
		NoProgress,
		Success
	}

	[Flags]
	private enum Dependency
	{
		Unknown = 0,
		NotDependent = 1,
		DependsMask = 0x10,
		Indirect = 0x12
	}

	private readonly ExpressionBinder _binder;

	private readonly TypeArray _pMethodTypeParameters;

	private readonly TypeArray _pMethodFormalParameterTypes;

	private readonly ArgInfos _pMethodArguments;

	private readonly List<CType>[] _pExactBounds;

	private readonly List<CType>[] _pUpperBounds;

	private readonly List<CType>[] _pLowerBounds;

	private readonly CType[] _pFixedResults;

	private Dependency[][] _ppDependencies;

	private bool _dependenciesDirty;

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	public static bool Infer(ExpressionBinder binder, MethodSymbol pMethod, TypeArray pMethodFormalParameterTypes, ArgInfos pMethodArguments, out TypeArray ppInferredTypeArguments)
	{
		ppInferredTypeArguments = null;
		if (pMethodFormalParameterTypes.Count == 0 || pMethod.InferenceMustFail())
		{
			return false;
		}
		MethodTypeInferrer methodTypeInferrer = new MethodTypeInferrer(binder, pMethodFormalParameterTypes, pMethodArguments, pMethod.typeVars);
		bool result = methodTypeInferrer.InferTypeArgs();
		ppInferredTypeArguments = methodTypeInferrer.GetResults();
		return result;
	}

	private MethodTypeInferrer(ExpressionBinder exprBinder, TypeArray pMethodFormalParameterTypes, ArgInfos pMethodArguments, TypeArray pMethodTypeParameters)
	{
		_binder = exprBinder;
		_pMethodFormalParameterTypes = pMethodFormalParameterTypes;
		_pMethodArguments = pMethodArguments;
		_pMethodTypeParameters = pMethodTypeParameters;
		_pFixedResults = new CType[pMethodTypeParameters.Count];
		_pLowerBounds = new List<CType>[pMethodTypeParameters.Count];
		_pUpperBounds = new List<CType>[pMethodTypeParameters.Count];
		_pExactBounds = new List<CType>[pMethodTypeParameters.Count];
		for (int i = 0; i < pMethodTypeParameters.Count; i++)
		{
			_pLowerBounds[i] = new List<CType>();
			_pUpperBounds[i] = new List<CType>();
			_pExactBounds[i] = new List<CType>();
		}
		_ppDependencies = null;
	}

	private TypeArray GetResults()
	{
		return TypeArray.Allocate(_pFixedResults);
	}

	private bool IsUnfixed(int iParam)
	{
		return _pFixedResults[iParam] == null;
	}

	private bool IsUnfixed(TypeParameterType pParam)
	{
		int indexInTotalParameters = pParam.IndexInTotalParameters;
		return IsUnfixed(indexInTotalParameters);
	}

	private bool AllFixed()
	{
		for (int i = 0; i < _pMethodTypeParameters.Count; i++)
		{
			if (IsUnfixed(i))
			{
				return false;
			}
		}
		return true;
	}

	private void AddLowerBound(TypeParameterType pParam, CType pBound)
	{
		int indexInTotalParameters = pParam.IndexInTotalParameters;
		if (!_pLowerBounds[indexInTotalParameters].Contains(pBound))
		{
			_pLowerBounds[indexInTotalParameters].Add(pBound);
		}
	}

	private void AddUpperBound(TypeParameterType pParam, CType pBound)
	{
		int indexInTotalParameters = pParam.IndexInTotalParameters;
		if (!_pUpperBounds[indexInTotalParameters].Contains(pBound))
		{
			_pUpperBounds[indexInTotalParameters].Add(pBound);
		}
	}

	private void AddExactBound(TypeParameterType pParam, CType pBound)
	{
		int indexInTotalParameters = pParam.IndexInTotalParameters;
		if (!_pExactBounds[indexInTotalParameters].Contains(pBound))
		{
			_pExactBounds[indexInTotalParameters].Add(pBound);
		}
	}

	private bool HasBound(int iParam)
	{
		if (_pLowerBounds[iParam].IsEmpty() && _pExactBounds[iParam].IsEmpty())
		{
			return !_pUpperBounds[iParam].IsEmpty();
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool InferTypeArgs()
	{
		InferTypeArgsFirstPhase();
		return InferTypeArgsSecondPhase();
	}

	private static bool IsReallyAType(CType pType)
	{
		if (!(pType is NullType) && !(pType is VoidType))
		{
			return !(pType is MethodGroupType);
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private void InferTypeArgsFirstPhase()
	{
		for (int i = 0; i < _pMethodArguments.carg; i++)
		{
			Expr expr = _pMethodArguments.prgexpr[i];
			if (expr.IsOptionalArgument)
			{
				continue;
			}
			CType cType = _pMethodFormalParameterTypes[i];
			CType cType2 = expr.RuntimeObjectActualType ?? _pMethodArguments.types[i];
			bool flag = false;
			if (cType is ParameterModifierType parameterModifierType)
			{
				cType = parameterModifierType.ParameterType;
				flag = true;
			}
			if (cType2 is ParameterModifierType parameterModifierType2)
			{
				cType2 = parameterModifierType2.ParameterType;
			}
			if (IsReallyAType(cType2))
			{
				if (flag)
				{
					ExactInference(cType2, cType);
				}
				else
				{
					LowerBoundInference(cType2, cType);
				}
			}
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool InferTypeArgsSecondPhase()
	{
		InitializeDependencies();
		while (true)
		{
			switch (DoSecondPhase())
			{
			case NewInferenceResult.InferenceFailed:
				return false;
			case NewInferenceResult.Success:
				return true;
			}
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private NewInferenceResult DoSecondPhase()
	{
		if (AllFixed())
		{
			return NewInferenceResult.Success;
		}
		NewInferenceResult newInferenceResult = FixNondependentParameters();
		if (newInferenceResult != NewInferenceResult.NoProgress)
		{
			return newInferenceResult;
		}
		newInferenceResult = FixDependentParameters();
		if (newInferenceResult != NewInferenceResult.NoProgress)
		{
			return newInferenceResult;
		}
		return NewInferenceResult.InferenceFailed;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private NewInferenceResult FixNondependentParameters()
	{
		bool[] array = new bool[_pMethodTypeParameters.Count];
		NewInferenceResult result = NewInferenceResult.NoProgress;
		for (int i = 0; i < _pMethodTypeParameters.Count; i++)
		{
			if (IsUnfixed(i) && HasBound(i) && !DependsOnAny(i))
			{
				array[i] = true;
				result = NewInferenceResult.MadeProgress;
			}
		}
		for (int i = 0; i < _pMethodTypeParameters.Count; i++)
		{
			if (array[i] && !Fix(i))
			{
				result = NewInferenceResult.InferenceFailed;
			}
		}
		return result;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private NewInferenceResult FixDependentParameters()
	{
		bool[] array = new bool[_pMethodTypeParameters.Count];
		NewInferenceResult result = NewInferenceResult.NoProgress;
		for (int i = 0; i < _pMethodTypeParameters.Count; i++)
		{
			if (IsUnfixed(i) && HasBound(i) && AnyDependsOn(i))
			{
				array[i] = true;
				result = NewInferenceResult.MadeProgress;
			}
		}
		for (int i = 0; i < _pMethodTypeParameters.Count; i++)
		{
			if (array[i] && !Fix(i))
			{
				result = NewInferenceResult.InferenceFailed;
			}
		}
		return result;
	}

	private void InitializeDependencies()
	{
		_ppDependencies = new Dependency[_pMethodTypeParameters.Count][];
		for (int i = 0; i < _pMethodTypeParameters.Count; i++)
		{
			_ppDependencies[i] = new Dependency[_pMethodTypeParameters.Count];
		}
		DeduceAllDependencies();
	}

	private bool DependsOn(int iParam, int jParam)
	{
		if (_dependenciesDirty)
		{
			SetIndirectsToUnknown();
			DeduceAllDependencies();
		}
		return (_ppDependencies[iParam][jParam] & Dependency.DependsMask) != 0;
	}

	private bool DependsTransitivelyOn(int iParam, int jParam)
	{
		for (int i = 0; i < _pMethodTypeParameters.Count; i++)
		{
			if ((_ppDependencies[iParam][i] & Dependency.DependsMask) != Dependency.Unknown && (_ppDependencies[i][jParam] & Dependency.DependsMask) != Dependency.Unknown)
			{
				return true;
			}
		}
		return false;
	}

	private void DeduceAllDependencies()
	{
		while (DeduceDependencies())
		{
		}
		SetUnknownsToNotDependent();
		_dependenciesDirty = false;
	}

	private bool DeduceDependencies()
	{
		bool result = false;
		for (int i = 0; i < _pMethodTypeParameters.Count; i++)
		{
			for (int j = 0; j < _pMethodTypeParameters.Count; j++)
			{
				if (_ppDependencies[i][j] == Dependency.Unknown && DependsTransitivelyOn(i, j))
				{
					_ppDependencies[i][j] = Dependency.Indirect;
					result = true;
				}
			}
		}
		return result;
	}

	private void SetUnknownsToNotDependent()
	{
		for (int i = 0; i < _pMethodTypeParameters.Count; i++)
		{
			for (int j = 0; j < _pMethodTypeParameters.Count; j++)
			{
				if (_ppDependencies[i][j] == Dependency.Unknown)
				{
					_ppDependencies[i][j] = Dependency.NotDependent;
				}
			}
		}
	}

	private void SetIndirectsToUnknown()
	{
		for (int i = 0; i < _pMethodTypeParameters.Count; i++)
		{
			for (int j = 0; j < _pMethodTypeParameters.Count; j++)
			{
				if (_ppDependencies[i][j] == Dependency.Indirect)
				{
					_ppDependencies[i][j] = Dependency.Unknown;
				}
			}
		}
	}

	private void UpdateDependenciesAfterFix(int iParam)
	{
		if (_ppDependencies != null)
		{
			for (int i = 0; i < _pMethodTypeParameters.Count; i++)
			{
				_ppDependencies[iParam][i] = Dependency.NotDependent;
				_ppDependencies[i][iParam] = Dependency.NotDependent;
			}
			_dependenciesDirty = true;
		}
	}

	private bool DependsOnAny(int iParam)
	{
		for (int i = 0; i < _pMethodTypeParameters.Count; i++)
		{
			if (DependsOn(iParam, i))
			{
				return true;
			}
		}
		return false;
	}

	private bool AnyDependsOn(int iParam)
	{
		for (int i = 0; i < _pMethodTypeParameters.Count; i++)
		{
			if (DependsOn(i, iParam))
			{
				return true;
			}
		}
		return false;
	}

	private void ExactInference(CType pSource, CType pDest)
	{
		if (!ExactTypeParameterInference(pSource, pDest) && !ExactArrayInference(pSource, pDest) && !ExactNullableInference(pSource, pDest))
		{
			ExactConstructedInference(pSource, pDest);
		}
	}

	private bool ExactTypeParameterInference(CType pSource, CType pDest)
	{
		if (pDest is TypeParameterType { IsMethodTypeParameter: not false } typeParameterType && IsUnfixed(typeParameterType))
		{
			AddExactBound(typeParameterType, pSource);
			return true;
		}
		return false;
	}

	private bool ExactArrayInference(CType pSource, CType pDest)
	{
		if (!(pSource is ArrayType arrayType) || !(pDest is ArrayType arrayType2))
		{
			return false;
		}
		if (arrayType.Rank != arrayType2.Rank || arrayType.IsSZArray != arrayType2.IsSZArray)
		{
			return false;
		}
		ExactInference(arrayType.ElementType, arrayType2.ElementType);
		return true;
	}

	private bool ExactNullableInference(CType pSource, CType pDest)
	{
		if (!(pSource is NullableType nullableType) || !(pDest is NullableType nullableType2))
		{
			return false;
		}
		ExactInference(nullableType.UnderlyingType, nullableType2.UnderlyingType);
		return true;
	}

	private bool ExactConstructedInference(CType pSource, CType pDest)
	{
		if (!(pSource is AggregateType aggregateType) || !(pDest is AggregateType aggregateType2) || aggregateType.OwningAggregate != aggregateType2.OwningAggregate)
		{
			return false;
		}
		ExactTypeArgumentInference(aggregateType, aggregateType2);
		return true;
	}

	private void ExactTypeArgumentInference(AggregateType pSource, AggregateType pDest)
	{
		TypeArray typeArgsAll = pSource.TypeArgsAll;
		TypeArray typeArgsAll2 = pDest.TypeArgsAll;
		for (int i = 0; i < typeArgsAll.Count; i++)
		{
			ExactInference(typeArgsAll[i], typeArgsAll2[i]);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private void LowerBoundInference(CType pSource, CType pDest)
	{
		if (!LowerBoundTypeParameterInference(pSource, pDest) && !LowerBoundArrayInference(pSource, pDest) && !ExactNullableInference(pSource, pDest))
		{
			LowerBoundConstructedInference(pSource, pDest);
		}
	}

	private bool LowerBoundTypeParameterInference(CType pSource, CType pDest)
	{
		if (pDest is TypeParameterType { IsMethodTypeParameter: not false } typeParameterType && IsUnfixed(typeParameterType))
		{
			AddLowerBound(typeParameterType, pSource);
			return true;
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool LowerBoundArrayInference(CType pSource, CType pDest)
	{
		if (!(pSource is ArrayType { ElementType: var elementType } arrayType))
		{
			return false;
		}
		CType pDest2;
		if (pDest is ArrayType arrayType2)
		{
			if (arrayType2.Rank != arrayType.Rank || arrayType2.IsSZArray != arrayType.IsSZArray)
			{
				return false;
			}
			pDest2 = arrayType2.ElementType;
		}
		else
		{
			if (!pDest.IsPredefType(PredefinedType.PT_G_IENUMERABLE) && !pDest.IsPredefType(PredefinedType.PT_G_ICOLLECTION) && !pDest.IsPredefType(PredefinedType.PT_G_ILIST) && !pDest.IsPredefType(PredefinedType.PT_G_IREADONLYCOLLECTION) && !pDest.IsPredefType(PredefinedType.PT_G_IREADONLYLIST))
			{
				return false;
			}
			if (!arrayType.IsSZArray)
			{
				return false;
			}
			pDest2 = ((AggregateType)pDest).TypeArgsThis[0];
		}
		if (elementType.IsReferenceType)
		{
			LowerBoundInference(elementType, pDest2);
		}
		else
		{
			ExactInference(elementType, pDest2);
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool LowerBoundConstructedInference(CType pSource, CType pDest)
	{
		if (!(pDest is AggregateType aggregateType))
		{
			return false;
		}
		if (aggregateType.TypeArgsAll.Count == 0)
		{
			return false;
		}
		if (pSource is AggregateType aggregateType2 && aggregateType2.OwningAggregate == aggregateType.OwningAggregate)
		{
			if (aggregateType2.IsInterfaceType || aggregateType2.IsDelegateType)
			{
				LowerBoundTypeArgumentInference(aggregateType2, aggregateType);
			}
			else
			{
				ExactTypeArgumentInference(aggregateType2, aggregateType);
			}
			return true;
		}
		if (LowerBoundClassInference(pSource, aggregateType))
		{
			return true;
		}
		if (LowerBoundInterfaceInference(pSource, aggregateType))
		{
			return true;
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool LowerBoundClassInference(CType pSource, AggregateType pDest)
	{
		if (!pDest.IsClassType)
		{
			return false;
		}
		AggregateType aggregateType = null;
		if (pSource.IsClassType)
		{
			aggregateType = (pSource as AggregateType).BaseClass;
		}
		while (aggregateType != null)
		{
			if (aggregateType.OwningAggregate == pDest.OwningAggregate)
			{
				ExactTypeArgumentInference(aggregateType, pDest);
				return true;
			}
			aggregateType = aggregateType.BaseClass;
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool LowerBoundInterfaceInference(CType pSource, AggregateType pDest)
	{
		if (!pDest.IsInterfaceType)
		{
			return false;
		}
		if (pSource is AggregateType aggregateType && (aggregateType.IsStructType || aggregateType.IsClassType || aggregateType.IsInterfaceType))
		{
			AggregateType aggregateType2 = null;
			CType[] items = aggregateType.IfacesAll.Items;
			for (int i = 0; i < items.Length; i++)
			{
				AggregateType aggregateType3 = (AggregateType)items[i];
				if (aggregateType3.OwningAggregate == pDest.OwningAggregate)
				{
					if (aggregateType2 == null)
					{
						aggregateType2 = aggregateType3;
					}
					else if (aggregateType2 != aggregateType3)
					{
						return false;
					}
				}
			}
			if (aggregateType2 != null)
			{
				LowerBoundTypeArgumentInference(aggregateType2, pDest);
				return true;
			}
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private void LowerBoundTypeArgumentInference(AggregateType pSource, AggregateType pDest)
	{
		TypeArray typeVarsAll = pSource.OwningAggregate.GetTypeVarsAll();
		TypeArray typeArgsAll = pSource.TypeArgsAll;
		TypeArray typeArgsAll2 = pDest.TypeArgsAll;
		for (int i = 0; i < typeArgsAll.Count; i++)
		{
			TypeParameterType typeParameterType = (TypeParameterType)typeVarsAll[i];
			CType cType = typeArgsAll[i];
			CType pDest2 = typeArgsAll2[i];
			if (cType.IsReferenceType)
			{
				if (typeParameterType.Covariant)
				{
					LowerBoundInference(cType, pDest2);
					continue;
				}
				if (typeParameterType.Contravariant)
				{
					UpperBoundInference(typeArgsAll[i], typeArgsAll2[i]);
					continue;
				}
			}
			ExactInference(typeArgsAll[i], typeArgsAll2[i]);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private void UpperBoundInference(CType pSource, CType pDest)
	{
		if (!UpperBoundTypeParameterInference(pSource, pDest) && !UpperBoundArrayInference(pSource, pDest) && !ExactNullableInference(pSource, pDest))
		{
			UpperBoundConstructedInference(pSource, pDest);
		}
	}

	private bool UpperBoundTypeParameterInference(CType pSource, CType pDest)
	{
		if (pDest is TypeParameterType { IsMethodTypeParameter: not false } typeParameterType && IsUnfixed(typeParameterType))
		{
			AddUpperBound(typeParameterType, pSource);
			return true;
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool UpperBoundArrayInference(CType pSource, CType pDest)
	{
		if (!(pDest is ArrayType { ElementType: var elementType } arrayType))
		{
			return false;
		}
		CType cType;
		if (pSource is ArrayType arrayType2)
		{
			if (arrayType.Rank != arrayType2.Rank || arrayType.IsSZArray != arrayType2.IsSZArray)
			{
				return false;
			}
			cType = arrayType2.ElementType;
		}
		else
		{
			if (!pSource.IsPredefType(PredefinedType.PT_G_IENUMERABLE) && !pSource.IsPredefType(PredefinedType.PT_G_ICOLLECTION) && !pSource.IsPredefType(PredefinedType.PT_G_ILIST) && !pSource.IsPredefType(PredefinedType.PT_G_IREADONLYLIST) && !pSource.IsPredefType(PredefinedType.PT_G_IREADONLYCOLLECTION))
			{
				return false;
			}
			if (!arrayType.IsSZArray)
			{
				return false;
			}
			cType = ((AggregateType)pSource).TypeArgsThis[0];
		}
		if (cType.IsReferenceType)
		{
			UpperBoundInference(cType, elementType);
		}
		else
		{
			ExactInference(cType, elementType);
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool UpperBoundConstructedInference(CType pSource, CType pDest)
	{
		if (!(pSource is AggregateType aggregateType))
		{
			return false;
		}
		if (aggregateType.TypeArgsAll.Count == 0)
		{
			return false;
		}
		if (pDest is AggregateType aggregateType2 && aggregateType.OwningAggregate == aggregateType2.OwningAggregate)
		{
			if (aggregateType2.IsInterfaceType || aggregateType2.IsDelegateType)
			{
				UpperBoundTypeArgumentInference(aggregateType, aggregateType2);
			}
			else
			{
				ExactTypeArgumentInference(aggregateType, aggregateType2);
			}
			return true;
		}
		if (UpperBoundClassInference(aggregateType, pDest))
		{
			return true;
		}
		if (UpperBoundInterfaceInference(aggregateType, pDest))
		{
			return true;
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool UpperBoundClassInference(AggregateType pSource, CType pDest)
	{
		if (!pSource.IsClassType || !pDest.IsClassType)
		{
			return false;
		}
		for (AggregateType baseClass = ((AggregateType)pDest).BaseClass; baseClass != null; baseClass = baseClass.BaseClass)
		{
			if (baseClass.OwningAggregate == pSource.OwningAggregate)
			{
				ExactTypeArgumentInference(pSource, baseClass);
				return true;
			}
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool UpperBoundInterfaceInference(AggregateType pSource, CType pDest)
	{
		if (!pSource.IsInterfaceType)
		{
			return false;
		}
		if (pDest is AggregateType aggregateType && (aggregateType.IsStructType || aggregateType.IsClassType || aggregateType.IsInterfaceType))
		{
			AggregateType aggregateType2 = null;
			CType[] items = aggregateType.IfacesAll.Items;
			for (int i = 0; i < items.Length; i++)
			{
				AggregateType aggregateType3 = (AggregateType)items[i];
				if (aggregateType3.OwningAggregate == pSource.OwningAggregate)
				{
					if (aggregateType2 == null)
					{
						aggregateType2 = aggregateType3;
					}
					else if (aggregateType2 != aggregateType3)
					{
						return false;
					}
				}
			}
			if (aggregateType2 != null)
			{
				UpperBoundTypeArgumentInference(aggregateType2, pDest as AggregateType);
				return true;
			}
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private void UpperBoundTypeArgumentInference(AggregateType pSource, AggregateType pDest)
	{
		TypeArray typeVarsAll = pSource.OwningAggregate.GetTypeVarsAll();
		TypeArray typeArgsAll = pSource.TypeArgsAll;
		TypeArray typeArgsAll2 = pDest.TypeArgsAll;
		for (int i = 0; i < typeArgsAll.Count; i++)
		{
			TypeParameterType typeParameterType = (TypeParameterType)typeVarsAll[i];
			CType cType = typeArgsAll[i];
			CType pDest2 = typeArgsAll2[i];
			if (cType.IsReferenceType)
			{
				if (typeParameterType.Covariant)
				{
					UpperBoundInference(cType, pDest2);
					continue;
				}
				if (typeParameterType.Contravariant)
				{
					LowerBoundInference(typeArgsAll[i], typeArgsAll2[i]);
					continue;
				}
			}
			ExactInference(typeArgsAll[i], typeArgsAll2[i]);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool Fix(int iParam)
	{
		if (_pExactBounds[iParam].Count >= 2)
		{
			return false;
		}
		List<CType> list = new List<CType>();
		if (_pExactBounds[iParam].IsEmpty())
		{
			HashSet<CType> hashSet = new HashSet<CType>();
			foreach (CType item in _pLowerBounds[iParam])
			{
				if (hashSet.Add(item))
				{
					list.Add(item);
				}
			}
			foreach (CType item2 in _pUpperBounds[iParam])
			{
				if (hashSet.Add(item2))
				{
					list.Add(item2);
				}
			}
		}
		else
		{
			list.Add(_pExactBounds[iParam].Head());
		}
		if (list.IsEmpty())
		{
			return false;
		}
		foreach (CType item3 in _pLowerBounds[iParam])
		{
			List<CType> list2 = new List<CType>();
			foreach (CType item4 in list)
			{
				if (item3 != item4 && !_binder.canConvert(item3, item4))
				{
					list2.Add(item4);
				}
			}
			foreach (CType item5 in list2)
			{
				list.Remove(item5);
			}
		}
		foreach (CType item6 in _pUpperBounds[iParam])
		{
			List<CType> list3 = new List<CType>();
			foreach (CType item7 in list)
			{
				if (item6 != item7 && !_binder.canConvert(item7, item6))
				{
					list3.Add(item7);
				}
			}
			foreach (CType item8 in list3)
			{
				list.Remove(item8);
			}
		}
		CType cType = null;
		foreach (CType item9 in list)
		{
			foreach (CType item10 in list)
			{
				if (item9 == item10 || _binder.canConvert(item10, item9))
				{
					continue;
				}
				goto IL_02bb;
			}
			if (cType != null)
			{
				return false;
			}
			cType = item9;
			IL_02bb:;
		}
		if (cType == null)
		{
			return false;
		}
		_pFixedResults[iParam] = TypeManager.GetBestAccessibleType(_binder.Context.ContextForMemberLookup, cType);
		UpdateDependenciesAfterFix(iParam);
		return true;
	}
}
