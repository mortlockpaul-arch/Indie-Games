using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.CSharp.RuntimeBinder.Errors;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal readonly struct ExpressionBinder
{
	private sealed class BinOpArgInfo
	{
		public Expr arg1;

		public Expr arg2;

		public PredefinedType pt1;

		public PredefinedType pt2;

		public PredefinedType ptRaw1;

		public PredefinedType ptRaw2;

		public CType type1;

		public CType type2;

		public CType typeRaw1;

		public CType typeRaw2;

		public BinOpKind binopKind;

		public BinOpMask mask;

		public BinOpArgInfo(Expr op1, Expr op2)
		{
			arg1 = op1;
			arg2 = op2;
			type1 = arg1.Type;
			type2 = arg2.Type;
			typeRaw1 = type1.StripNubs();
			typeRaw2 = type2.StripNubs();
			pt1 = (type1.IsPredefined ? type1.PredefinedType : PredefinedType.PT_COUNT);
			pt2 = (type2.IsPredefined ? type2.PredefinedType : PredefinedType.PT_COUNT);
			ptRaw1 = (typeRaw1.IsPredefined ? typeRaw1.PredefinedType : PredefinedType.PT_COUNT);
			ptRaw2 = (typeRaw2.IsPredefined ? typeRaw2.PredefinedType : PredefinedType.PT_COUNT);
		}

		public bool ValidForDelegate()
		{
			return (mask & BinOpMask.Delegate) != 0;
		}

		public bool ValidForEnumAndUnderlyingType()
		{
			return (mask & BinOpMask.EnumUnder) != 0;
		}

		public bool ValidForUnderlyingTypeAndEnum()
		{
			return (mask & BinOpMask.Add) != 0;
		}

		public bool ValidForEnum()
		{
			return (mask & BinOpMask.Enum) != 0;
		}
	}

	private class BinOpSig
	{
		public PredefinedType pt1;

		public PredefinedType pt2;

		public BinOpMask mask;

		public int cbosSkip;

		public PfnBindBinOp pfn;

		public OpSigFlags grfos;

		public BinOpFuncKind fnkind;

		protected BinOpSig()
		{
		}

		public BinOpSig(PredefinedType pt1, PredefinedType pt2, BinOpMask mask, int cbosSkip, PfnBindBinOp pfn, OpSigFlags grfos, BinOpFuncKind fnkind)
		{
			this.pt1 = pt1;
			this.pt2 = pt2;
			this.mask = mask;
			this.cbosSkip = cbosSkip;
			this.pfn = pfn;
			this.grfos = grfos;
			this.fnkind = fnkind;
		}

		public bool ConvertOperandsBeforeBinding()
		{
			return (grfos & OpSigFlags.Convert) != 0;
		}

		public bool CanLift()
		{
			return (grfos & OpSigFlags.CanLift) != 0;
		}

		public bool AutoLift()
		{
			return (grfos & OpSigFlags.AutoLift) != 0;
		}
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private sealed class BinOpFullSig : BinOpSig
	{
		private readonly LiftFlags _grflt;

		private readonly CType _type1;

		private readonly CType _type2;

		public BinOpFullSig(CType type1, CType type2, PfnBindBinOp pfn, OpSigFlags grfos, LiftFlags grflt, BinOpFuncKind fnkind)
		{
			pt1 = PredefinedType.PT_UNDEFINEDINDEX;
			pt2 = PredefinedType.PT_UNDEFINEDINDEX;
			mask = BinOpMask.None;
			cbosSkip = 0;
			base.pfn = pfn;
			base.grfos = grfos;
			_type1 = type1;
			_type2 = type2;
			_grflt = grflt;
			base.fnkind = fnkind;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		public BinOpFullSig(BinOpSig bos)
		{
			pt1 = bos.pt1;
			pt2 = bos.pt2;
			mask = bos.mask;
			cbosSkip = bos.cbosSkip;
			pfn = bos.pfn;
			grfos = bos.grfos;
			fnkind = bos.fnkind;
			_type1 = ((pt1 != PredefinedType.PT_UNDEFINEDINDEX) ? GetPredefindType(pt1) : null);
			_type2 = ((pt2 != PredefinedType.PT_UNDEFINEDINDEX) ? GetPredefindType(pt2) : null);
			_grflt = LiftFlags.None;
		}

		public bool FPreDef()
		{
			return pt1 != PredefinedType.PT_UNDEFINEDINDEX;
		}

		public bool isLifted()
		{
			if (_grflt == LiftFlags.None)
			{
				return false;
			}
			return true;
		}

		public bool ConvertFirst()
		{
			return (_grflt & LiftFlags.Convert1) != 0;
		}

		public bool ConvertSecond()
		{
			return (_grflt & LiftFlags.Convert2) != 0;
		}

		public CType Type1()
		{
			return _type1;
		}

		public CType Type2()
		{
			return _type2;
		}
	}

	private delegate bool ConversionFunc(Expr pSourceExpr, CType pSourceType, CType pDestinationType, bool needsExprDest, out Expr ppDestinationExpr, CONVERTTYPE flags);

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private sealed class ExplicitConversion
	{
		private readonly ExpressionBinder _binder;

		private readonly Expr _exprSrc;

		private readonly CType _typeSrc;

		private readonly CType _typeDest;

		private Expr _exprDest;

		private readonly bool _needsExprDest;

		private readonly CONVERTTYPE _flags;

		public Expr ExprDest => _exprDest;

		public ExplicitConversion(ExpressionBinder binder, Expr exprSrc, CType typeSrc, CType typeDest, bool needsExprDest, CONVERTTYPE flags)
		{
			_binder = binder;
			_exprSrc = exprSrc;
			_typeSrc = typeSrc;
			_typeDest = typeDest;
			_needsExprDest = needsExprDest;
			_flags = flags;
			_exprDest = null;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		public bool Bind()
		{
			if (_binder.BindImplicitConversion(_exprSrc, _typeSrc, _typeDest, _needsExprDest, out _exprDest, _flags | CONVERTTYPE.ISEXPLICIT))
			{
				return true;
			}
			if (_typeSrc == null || _typeDest == null || _typeDest is MethodGroupType)
			{
				return false;
			}
			if (_typeDest is NullableType)
			{
				return false;
			}
			if (_typeSrc is NullableType)
			{
				return bindExplicitConversionFromNub();
			}
			if (bindExplicitConversionFromArrayToIList())
			{
				return true;
			}
			switch (_typeDest.TypeKind)
			{
			default:
				return false;
			case TypeKind.TK_VoidType:
				return false;
			case TypeKind.TK_NullType:
				return false;
			case TypeKind.TK_ArrayType:
				if (bindExplicitConversionToArray((ArrayType)_typeDest))
				{
					return true;
				}
				break;
			case TypeKind.TK_PointerType:
				if (bindExplicitConversionToPointer())
				{
					return true;
				}
				break;
			case TypeKind.TK_AggregateType:
				switch (bindExplicitConversionToAggregate(_typeDest as AggregateType))
				{
				case AggCastResult.Success:
					return true;
				case AggCastResult.Abort:
					return false;
				}
				break;
			}
			if ((_flags & CONVERTTYPE.NOUDC) == 0)
			{
				return _binder.bindUserDefinedConversion(_exprSrc, _typeSrc, _typeDest, _needsExprDest, out _exprDest, fImplicitOnly: false);
			}
			return false;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindExplicitConversionFromNub()
		{
			if (_typeDest.IsValueType && _binder.BindExplicitConversion(null, _typeSrc.StripNubs(), _typeDest, _flags | CONVERTTYPE.NOUDC))
			{
				if (_needsExprDest)
				{
					Expr expr = _exprSrc;
					if (expr.Type is NullableType)
					{
						expr = BindNubValue(expr);
					}
					if (!_binder.BindExplicitConversion(expr, expr.Type, _typeDest, _needsExprDest, out _exprDest, _flags | CONVERTTYPE.NOUDC))
					{
						return false;
					}
					if (_exprDest is ExprUserDefinedConversion exprUserDefinedConversion)
					{
						exprUserDefinedConversion.Argument = _exprSrc;
					}
				}
				return true;
			}
			if ((_flags & CONVERTTYPE.NOUDC) == 0)
			{
				return _binder.bindUserDefinedConversion(_exprSrc, _typeSrc, _typeDest, _needsExprDest, out _exprDest, fImplicitOnly: false);
			}
			return false;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindExplicitConversionFromArrayToIList()
		{
			if (!(_typeSrc is ArrayType { IsSZArray: not false } arrayType) || !(_typeDest is AggregateType { IsInterfaceType: not false } aggregateType) || aggregateType.TypeArgsAll.Count != 1)
			{
				return false;
			}
			AggregateSymbol predefAgg = SymbolLoader.GetPredefAgg(PredefinedType.PT_G_ILIST);
			AggregateSymbol predefAgg2 = SymbolLoader.GetPredefAgg(PredefinedType.PT_G_IREADONLYLIST);
			if ((predefAgg == null || !SymbolLoader.IsBaseAggregate(predefAgg, aggregateType.OwningAggregate)) && (predefAgg2 == null || !SymbolLoader.IsBaseAggregate(predefAgg2, aggregateType.OwningAggregate)))
			{
				return false;
			}
			CType elementType = arrayType.ElementType;
			CType typeDst = aggregateType.TypeArgsAll[0];
			if (!CConversions.FExpRefConv(elementType, typeDst))
			{
				return false;
			}
			if (_needsExprDest)
			{
				_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest, EXPRFLAG.EXF_OPERATOR);
			}
			return true;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindExplicitConversionFromIListToArray(ArrayType arrayDest)
		{
			if (!arrayDest.IsSZArray || !(_typeSrc is AggregateType { IsInterfaceType: not false } aggregateType) || aggregateType.TypeArgsAll.Count != 1)
			{
				return false;
			}
			AggregateSymbol predefAgg = SymbolLoader.GetPredefAgg(PredefinedType.PT_G_ILIST);
			AggregateSymbol predefAgg2 = SymbolLoader.GetPredefAgg(PredefinedType.PT_G_IREADONLYLIST);
			if ((predefAgg == null || !SymbolLoader.IsBaseAggregate(predefAgg, aggregateType.OwningAggregate)) && (predefAgg2 == null || !SymbolLoader.IsBaseAggregate(predefAgg2, aggregateType.OwningAggregate)))
			{
				return false;
			}
			CType elementType = arrayDest.ElementType;
			CType cType = aggregateType.TypeArgsAll[0];
			if (elementType != cType && !CConversions.FExpRefConv(elementType, cType))
			{
				return false;
			}
			if (_needsExprDest)
			{
				_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest, EXPRFLAG.EXF_OPERATOR);
			}
			return true;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindExplicitConversionFromArrayToArray(ArrayType arraySrc, ArrayType arrayDest)
		{
			if (arraySrc.Rank != arrayDest.Rank || arraySrc.IsSZArray != arrayDest.IsSZArray)
			{
				return false;
			}
			if (CConversions.FExpRefConv(arraySrc.ElementType, arrayDest.ElementType))
			{
				if (_needsExprDest)
				{
					_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest, EXPRFLAG.EXF_OPERATOR);
				}
				return true;
			}
			return false;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindExplicitConversionToArray(ArrayType arrayDest)
		{
			if (_typeSrc is ArrayType arraySrc)
			{
				return bindExplicitConversionFromArrayToArray(arraySrc, arrayDest);
			}
			if (bindExplicitConversionFromIListToArray(arrayDest))
			{
				return true;
			}
			if (_binder.canConvert(GetPredefindType(PredefinedType.PT_ARRAY), _typeSrc, CONVERTTYPE.NOUDC))
			{
				if (_needsExprDest)
				{
					_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest, EXPRFLAG.EXF_OPERATOR);
				}
				return true;
			}
			return false;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindExplicitConversionToPointer()
		{
			if (_typeSrc is PointerType || (_typeSrc.FundamentalType <= FUNDTYPE.FT_U8 && _typeSrc.IsNumericType))
			{
				if (_needsExprDest)
				{
					_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest);
				}
				return true;
			}
			return false;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private AggCastResult bindExplicitConversionFromEnumToAggregate(AggregateType aggTypeDest)
		{
			if (!_typeSrc.IsEnumType)
			{
				return AggCastResult.Failure;
			}
			AggregateSymbol owningAggregate = aggTypeDest.OwningAggregate;
			if (owningAggregate.isPredefAgg(PredefinedType.PT_DECIMAL))
			{
				return bindExplicitConversionFromEnumToDecimal(aggTypeDest);
			}
			if (!owningAggregate.getThisType().IsNumericType && !owningAggregate.IsEnum() && (!owningAggregate.IsPredefined() || owningAggregate.GetPredefType() != PredefinedType.PT_CHAR))
			{
				return AggCastResult.Failure;
			}
			if (_exprSrc.GetConst() != null)
			{
				switch (_binder.bindConstantCast(_exprSrc, _typeDest, _needsExprDest, out _exprDest, explicitConversion: true))
				{
				case ConstCastResult.Success:
					return AggCastResult.Success;
				case ConstCastResult.CheckFailure:
					return AggCastResult.Abort;
				}
			}
			if (_needsExprDest)
			{
				_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest);
			}
			return AggCastResult.Success;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private AggCastResult bindExplicitConversionFromDecimalToEnum(AggregateType aggTypeDest)
		{
			if (_exprSrc.GetConst() != null)
			{
				switch (_binder.bindConstantCast(_exprSrc, _typeDest, _needsExprDest, out _exprDest, explicitConversion: true))
				{
				case ConstCastResult.Success:
					return AggCastResult.Success;
				case ConstCastResult.CheckFailure:
					if ((_flags & CONVERTTYPE.CHECKOVERFLOW) == 0)
					{
						return AggCastResult.Abort;
					}
					break;
				}
			}
			bool flag = true;
			if (_needsExprDest)
			{
				CType underlyingEnumType = aggTypeDest.UnderlyingEnumType;
				flag = _binder.bindUserDefinedConversion(_exprSrc, _typeSrc, underlyingEnumType, _needsExprDest, out _exprDest, fImplicitOnly: false);
				if (flag)
				{
					_binder.bindSimpleCast(_exprDest, _typeDest, out _exprDest);
				}
			}
			if (!flag)
			{
				return AggCastResult.Failure;
			}
			return AggCastResult.Success;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private AggCastResult bindExplicitConversionFromEnumToDecimal(AggregateType aggTypeDest)
		{
			AggregateType underlyingEnumType = _typeSrc.UnderlyingEnumType;
			Expr pexprDest;
			if (_exprSrc == null)
			{
				pexprDest = null;
			}
			else
			{
				_binder.bindSimpleCast(_exprSrc, underlyingEnumType, out pexprDest);
			}
			if (pexprDest.GetConst() != null)
			{
				switch (_binder.bindConstantCast(pexprDest, _typeDest, _needsExprDest, out _exprDest, explicitConversion: true))
				{
				case ConstCastResult.Success:
					return AggCastResult.Success;
				case ConstCastResult.CheckFailure:
					if ((_flags & CONVERTTYPE.CHECKOVERFLOW) == 0)
					{
						return AggCastResult.Abort;
					}
					break;
				}
			}
			if (_needsExprDest)
			{
				_binder.bindUserDefinedConversion(pexprDest, underlyingEnumType, aggTypeDest, _needsExprDest, out _exprDest, fImplicitOnly: false);
			}
			return AggCastResult.Success;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private AggCastResult bindExplicitConversionToEnum(AggregateType aggTypeDest)
		{
			if (!aggTypeDest.OwningAggregate.IsEnum())
			{
				return AggCastResult.Failure;
			}
			if (_typeSrc.IsPredefType(PredefinedType.PT_DECIMAL))
			{
				return bindExplicitConversionFromDecimalToEnum(aggTypeDest);
			}
			if (_typeSrc.IsNumericType || (_typeSrc.IsPredefined && _typeSrc.PredefinedType == PredefinedType.PT_CHAR))
			{
				if (_exprSrc.GetConst() != null)
				{
					switch (_binder.bindConstantCast(_exprSrc, _typeDest, _needsExprDest, out _exprDest, explicitConversion: true))
					{
					case ConstCastResult.Success:
						return AggCastResult.Success;
					case ConstCastResult.CheckFailure:
						return AggCastResult.Abort;
					}
				}
				if (_needsExprDest)
				{
					_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest);
				}
				return AggCastResult.Success;
			}
			if (_typeSrc.IsPredefined && (_typeSrc.IsPredefType(PredefinedType.PT_OBJECT) || _typeSrc.IsPredefType(PredefinedType.PT_VALUE) || _typeSrc.IsPredefType(PredefinedType.PT_ENUM)))
			{
				if (_needsExprDest)
				{
					_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest, EXPRFLAG.EXF_INDEXER);
				}
				return AggCastResult.Success;
			}
			return AggCastResult.Failure;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
		private AggCastResult bindExplicitConversionBetweenSimpleTypes(AggregateType aggTypeDest)
		{
			if (!_typeSrc.IsSimpleType || !aggTypeDest.IsSimpleType)
			{
				return AggCastResult.Failure;
			}
			AggregateSymbol owningAggregate = aggTypeDest.OwningAggregate;
			PredefinedType predefinedType = _typeSrc.PredefinedType;
			PredefinedType predefType = owningAggregate.GetPredefType();
			if (GetConvKind(predefinedType, predefType) != ConvKind.Explicit)
			{
				return AggCastResult.Failure;
			}
			if (_exprSrc.GetConst() != null)
			{
				switch (_binder.bindConstantCast(_exprSrc, _typeDest, _needsExprDest, out _exprDest, explicitConversion: true))
				{
				case ConstCastResult.Success:
					return AggCastResult.Success;
				case ConstCastResult.CheckFailure:
					if ((_flags & CONVERTTYPE.CHECKOVERFLOW) == 0)
					{
						return AggCastResult.Abort;
					}
					break;
				}
			}
			bool flag = true;
			if (_needsExprDest)
			{
				if (isUserDefinedConversion(predefinedType, predefType))
				{
					flag = _binder.bindUserDefinedConversion(_exprSrc, _typeSrc, aggTypeDest, _needsExprDest, out _exprDest, fImplicitOnly: false);
				}
				else
				{
					_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest, ((_flags & CONVERTTYPE.CHECKOVERFLOW) != 0) ? EXPRFLAG.EXF_CHECKOVERFLOW : ((EXPRFLAG)0));
				}
			}
			if (!flag)
			{
				return AggCastResult.Failure;
			}
			return AggCastResult.Success;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private AggCastResult bindExplicitConversionBetweenAggregates(AggregateType aggTypeDest)
		{
			if (!(_typeSrc is AggregateType { OwningAggregate: var owningAggregate } aggregateType))
			{
				return AggCastResult.Failure;
			}
			AggregateSymbol owningAggregate2 = aggTypeDest.OwningAggregate;
			if (SymbolLoader.HasBaseConversion(aggTypeDest, aggregateType))
			{
				if (_needsExprDest)
				{
					ref readonly ExpressionBinder binder = ref _binder;
					Expr exprSrc = _exprSrc;
					CType typeDest = _typeDest;
					ref Expr exprDest = ref _exprDest;
					int exprFlags;
					if (!owningAggregate2.IsValueType() || owningAggregate.getThisType().FundamentalType != FUNDTYPE.FT_REF)
					{
						Expr exprSrc2 = _exprSrc;
						exprFlags = (int)(EXPRFLAG.EXF_OPERATOR | ((exprSrc2 != null) ? (exprSrc2.Flags & EXPRFLAG.EXF_CANTBENULL) : ((EXPRFLAG)0)));
					}
					else
					{
						exprFlags = 4;
					}
					binder.bindSimpleCast(exprSrc, typeDest, out exprDest, (EXPRFLAG)exprFlags);
				}
				return AggCastResult.Success;
			}
			if ((owningAggregate.IsClass() && !owningAggregate.IsSealed() && owningAggregate2.IsInterface()) || (owningAggregate.IsInterface() && owningAggregate2.IsClass() && !owningAggregate2.IsSealed()) || (owningAggregate.IsInterface() && owningAggregate2.IsInterface()) || CConversions.HasGenericDelegateExplicitReferenceConversion(_typeSrc, aggTypeDest))
			{
				if (_needsExprDest)
				{
					ref readonly ExpressionBinder binder2 = ref _binder;
					Expr exprSrc3 = _exprSrc;
					CType typeDest2 = _typeDest;
					ref Expr exprDest2 = ref _exprDest;
					Expr exprSrc4 = _exprSrc;
					binder2.bindSimpleCast(exprSrc3, typeDest2, out exprDest2, EXPRFLAG.EXF_OPERATOR | ((exprSrc4 != null) ? (exprSrc4.Flags & EXPRFLAG.EXF_CANTBENULL) : ((EXPRFLAG)0)));
				}
				return AggCastResult.Success;
			}
			return AggCastResult.Failure;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private AggCastResult bindExplicitConversionFromPointerToInt(AggregateType aggTypeDest)
		{
			if (!(_typeSrc is PointerType) || aggTypeDest.FundamentalType > FUNDTYPE.FT_U8 || !aggTypeDest.IsNumericType)
			{
				return AggCastResult.Failure;
			}
			if (_needsExprDest)
			{
				_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest);
			}
			return AggCastResult.Success;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private AggCastResult bindExplicitConversionToAggregate(AggregateType aggTypeDest)
		{
			AggCastResult aggCastResult = bindExplicitConversionFromEnumToAggregate(aggTypeDest);
			if (aggCastResult != AggCastResult.Failure)
			{
				return aggCastResult;
			}
			aggCastResult = bindExplicitConversionToEnum(aggTypeDest);
			if (aggCastResult != AggCastResult.Failure)
			{
				return aggCastResult;
			}
			aggCastResult = bindExplicitConversionBetweenSimpleTypes(aggTypeDest);
			if (aggCastResult != AggCastResult.Failure)
			{
				return aggCastResult;
			}
			aggCastResult = bindExplicitConversionBetweenAggregates(aggTypeDest);
			if (aggCastResult != AggCastResult.Failure)
			{
				return aggCastResult;
			}
			aggCastResult = bindExplicitConversionFromPointerToInt(aggTypeDest);
			if (aggCastResult != AggCastResult.Failure)
			{
				return aggCastResult;
			}
			if (_typeSrc is VoidType)
			{
				return AggCastResult.Abort;
			}
			return AggCastResult.Failure;
		}
	}

	private delegate Expr PfnBindBinOp(ExpressionBinder binder, ExpressionKind ek, EXPRFLAG flags, Expr op1, Expr op2);

	private delegate Expr PfnBindUnaOp(ExpressionBinder binder, ExpressionKind ek, EXPRFLAG flags, Expr op);

	public enum NamedArgumentsKind
	{
		None,
		Positioning,
		NonTrailing
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal sealed class GroupToArgsBinder
	{
		private enum Result
		{
			Success,
			Failure_SearchForExpanded,
			Failure_NoSearchForExpanded
		}

		private readonly ExpressionBinder _pExprBinder;

		private bool _fCandidatesUnsupported;

		private readonly BindingFlag _fBindFlags;

		private readonly ExprMemberGroup _pGroup;

		private readonly ArgInfos _pArguments;

		private readonly ArgInfos _pOriginalArguments;

		private readonly NamedArgumentsKind _namedArgumentsKind;

		private AggregateType _pCurrentType;

		private MethodOrPropertySymbol _pCurrentSym;

		private TypeArray _pCurrentTypeArgs;

		private TypeArray _pCurrentParameters;

		private int _nArgBest;

		private readonly GroupToArgsBinderResult _results;

		private readonly List<CandidateFunctionMember> _methList;

		private readonly MethPropWithInst _mpwiParamTypeConstraints;

		private readonly MethPropWithInst _mpwiBogus;

		private readonly MethPropWithInst _misnamed;

		private readonly MethPropWithInst _mpwiCantInferInstArg;

		private readonly MethWithType _mwtBadArity;

		private Name _pInvalidSpecifiedName;

		private Name _pNameUsedInPositionalArgument;

		private Name _pDuplicateSpecifiedName;

		private readonly List<CType> _HiddenTypes;

		private bool _bArgumentsChangedForNamedOrOptionalArguments;

		public GroupToArgsBinder(ExpressionBinder exprBinder, BindingFlag bindFlags, ExprMemberGroup grp, ArgInfos args, ArgInfos originalArgs, NamedArgumentsKind namedArgumentsKind)
		{
			_pExprBinder = exprBinder;
			_fCandidatesUnsupported = false;
			_fBindFlags = bindFlags;
			_pGroup = grp;
			_pArguments = args;
			_pOriginalArguments = originalArgs;
			_namedArgumentsKind = namedArgumentsKind;
			_pCurrentType = null;
			_pCurrentSym = null;
			_pCurrentTypeArgs = null;
			_pCurrentParameters = null;
			_nArgBest = -1;
			_results = new GroupToArgsBinderResult();
			_methList = new List<CandidateFunctionMember>();
			_mpwiParamTypeConstraints = new MethPropWithInst();
			_mpwiBogus = new MethPropWithInst();
			_misnamed = new MethPropWithInst();
			_mpwiCantInferInstArg = new MethPropWithInst();
			_mwtBadArity = new MethWithType();
			_HiddenTypes = new List<CType>();
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		public void Bind()
		{
			LookForCandidates();
			if (!GetResultOfBind())
			{
				throw ReportErrorsOnFailure();
			}
		}

		public GroupToArgsBinderResult GetResultsOfBind()
		{
			return _results;
		}

		private static CType GetTypeQualifier(ExprMemberGroup pGroup)
		{
			if ((pGroup.Flags & EXPRFLAG.EXF_CTOR) == 0)
			{
				return pGroup.OptionalObject?.Type;
			}
			return pGroup.ParentType;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private void LookForCandidates()
		{
			bool flag = false;
			bool flag2 = true;
			bool flag3 = true;
			bool flag4 = false;
			symbmask_t mask = (symbmask_t)(1 << (int)_pGroup.SymKind);
			CMemberLookupResults.CMethodIterator methodIterator = _pGroup.MemberLookupResults.GetMethodIterator(GetTypeQualifier(_pGroup), _pExprBinder.ContextForMemberLookup, _pGroup.TypeArgs.Count, _pGroup.Flags, mask, (_namedArgumentsKind == NamedArgumentsKind.NonTrailing) ? _pOriginalArguments : null);
			while (true)
			{
				bool flag5 = false;
				if (flag2 && !flag)
				{
					flag5 = (flag = ConstructExpandedParameters());
				}
				if (!flag5)
				{
					flag = false;
					if (!GetNextSym(methodIterator))
					{
						break;
					}
					_pCurrentParameters = _pCurrentSym.Params;
					flag2 = true;
				}
				if (_bArgumentsChangedForNamedOrOptionalArguments)
				{
					_bArgumentsChangedForNamedOrOptionalArguments = false;
					CopyArgInfos(_pOriginalArguments, _pArguments);
				}
				if (_namedArgumentsKind == NamedArgumentsKind.Positioning)
				{
					if (!ReOrderArgsForNamedArguments())
					{
						continue;
					}
				}
				else if (HasOptionalParameters() && !AddArgumentsForOptionalParameters())
				{
					continue;
				}
				if (!flag5)
				{
					flag4 = true;
					flag3 &= CSemanticChecker.CheckBogus(_pCurrentSym);
					if (_pCurrentParameters.Count != _pArguments.carg)
					{
						flag2 = true;
						continue;
					}
				}
				if (!methodIterator.CanUseCurrentSymbol)
				{
					continue;
				}
				Result result = DetermineCurrentTypeArgs();
				if (result != Result.Success)
				{
					flag2 = result == Result.Failure_SearchForExpanded;
					continue;
				}
				bool flag6 = !methodIterator.IsCurrentSymbolInaccessible;
				if (!flag6 && (!_methList.IsEmpty() || (bool)_results.InaccessibleResult))
				{
					flag2 = false;
					continue;
				}
				bool flag7 = flag6 && methodIterator.IsCurrentSymbolMisnamed;
				if (flag7 && (!_methList.IsEmpty() || (bool)_results.InaccessibleResult || (bool)_misnamed))
				{
					flag2 = false;
					continue;
				}
				bool flag8 = flag6 && !flag7 && methodIterator.IsCurrentSymbolBogus;
				if (flag8 && (!_methList.IsEmpty() || (bool)_results.InaccessibleResult || (bool)_mpwiBogus || (bool)_misnamed))
				{
					flag2 = false;
					continue;
				}
				if (!ArgumentsAreConvertible())
				{
					flag2 = true;
					continue;
				}
				if (!flag6)
				{
					_results.InaccessibleResult.Set(_pCurrentSym, _pCurrentType, _pCurrentTypeArgs);
				}
				else if (flag7)
				{
					_misnamed.Set(_pCurrentSym, _pCurrentType, _pCurrentTypeArgs);
				}
				else if (flag8)
				{
					_mpwiBogus.Set(_pCurrentSym, _pCurrentType, _pCurrentTypeArgs);
				}
				else
				{
					_methList.Add(new CandidateFunctionMember(new MethPropWithInst(_pCurrentSym, _pCurrentType, _pCurrentTypeArgs), _pCurrentParameters, 0, flag));
					if (_pCurrentType.IsInterfaceType)
					{
						CType[] items = _pCurrentType.IfacesAll.Items;
						for (int i = 0; i < items.Length; i++)
						{
							AggregateType item = (AggregateType)items[i];
							_HiddenTypes.Add(item);
						}
						AggregateType predefindType = SymbolLoader.GetPredefindType(PredefinedType.PT_OBJECT);
						_HiddenTypes.Add(predefindType);
					}
				}
				flag2 = false;
			}
			_fCandidatesUnsupported = flag3 & flag4;
			if (_bArgumentsChangedForNamedOrOptionalArguments)
			{
				CopyArgInfos(_pOriginalArguments, _pArguments);
			}
		}

		private static void CopyArgInfos(ArgInfos src, ArgInfos dst)
		{
			dst.carg = src.carg;
			dst.types = src.types;
			dst.prgexpr.Clear();
			for (int i = 0; i < src.prgexpr.Count; i++)
			{
				dst.prgexpr.Add(src.prgexpr[i]);
			}
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool GetResultOfBind()
		{
			if (!_methList.IsEmpty())
			{
				CandidateFunctionMember candidateFunctionMember;
				if (_methList.Count == 1)
				{
					candidateFunctionMember = _methList.Head();
				}
				else
				{
					CType pTypeThrough = _pGroup.OptionalObject?.Type;
					candidateFunctionMember = _pExprBinder.FindBestMethod(_methList, pTypeThrough, _pArguments, out var methAmbig, out var methAmbig2);
					if (candidateFunctionMember == null)
					{
						if (methAmbig.@params != methAmbig2.@params || methAmbig.mpwi.MethProp().Params.Count != methAmbig2.mpwi.MethProp().Params.Count || methAmbig.mpwi.TypeArgs != methAmbig2.mpwi.TypeArgs || methAmbig.mpwi.GetType() != methAmbig2.mpwi.GetType() || methAmbig.mpwi.MethProp().Params == methAmbig2.mpwi.MethProp().Params)
						{
							throw ErrorHandling.Error(ErrorCode.ERR_AmbigCall, methAmbig.mpwi, methAmbig2.mpwi);
						}
						throw ErrorHandling.Error(ErrorCode.ERR_AmbigCall, methAmbig.mpwi.MethProp(), methAmbig2.mpwi.MethProp());
					}
				}
				_results.BestResult = candidateFunctionMember.mpwi;
				ReportErrorsOnSuccess();
				return true;
			}
			return false;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool ReOrderArgsForNamedArguments()
		{
			MethodOrPropertySymbol methodOrPropertySymbol = FindMostDerivedMethod(_pCurrentSym, _pGroup.OptionalObject);
			if (methodOrPropertySymbol == null)
			{
				return false;
			}
			int count = _pCurrentParameters.Count;
			if (count == 0 || count < _pArguments.carg)
			{
				return false;
			}
			if (!NamedArgumentNamesAppearInParameterList(methodOrPropertySymbol))
			{
				return false;
			}
			_bArgumentsChangedForNamedOrOptionalArguments = ReOrderArgsForNamedArguments(methodOrPropertySymbol, _pCurrentParameters, _pCurrentType, _pGroup, _pArguments);
			return _bArgumentsChangedForNamedOrOptionalArguments;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		internal static bool ReOrderArgsForNamedArguments(MethodOrPropertySymbol methprop, TypeArray pCurrentParameters, AggregateType pCurrentType, ExprMemberGroup pGroup, ArgInfos pArguments)
		{
			int count = pCurrentParameters.Count;
			Expr[] array = new Expr[count];
			int num = 0;
			Expr expr = null;
			TypeArray typeArray = TypeManager.SubstTypeArray(pCurrentParameters, pCurrentType, pGroup.TypeArgs);
			foreach (Name parameterName in methprop.ParameterNames)
			{
				if (num >= pCurrentParameters.Count)
				{
					break;
				}
				if (methprop.isParamArray && num < pArguments.carg && pArguments.prgexpr[num] is ExprArrayInit { GeneratedForParamArray: not false })
				{
					expr = pArguments.prgexpr[num];
				}
				if (num < pArguments.carg && !(pArguments.prgexpr[num] is ExprNamedArgumentSpecification) && !(pArguments.prgexpr[num] is ExprArrayInit { GeneratedForParamArray: not false }))
				{
					array[num] = pArguments.prgexpr[num++];
					continue;
				}
				Expr expr2 = FindArgumentWithName(pArguments, parameterName);
				if (expr2 == null)
				{
					if (methprop.IsParameterOptional(num))
					{
						expr2 = GenerateOptionalArgument(methprop, typeArray[num], num);
					}
					else
					{
						if (expr == null || num != methprop.Params.Count - 1)
						{
							return false;
						}
						expr2 = expr;
					}
				}
				array[num++] = expr2;
			}
			CType[] array2 = new CType[pCurrentParameters.Count];
			for (int i = 0; i < count; i++)
			{
				if (i < pArguments.prgexpr.Count)
				{
					pArguments.prgexpr[i] = array[i];
				}
				else
				{
					pArguments.prgexpr.Add(array[i]);
				}
				array2[i] = pArguments.prgexpr[i].Type;
			}
			pArguments.carg = pCurrentParameters.Count;
			pArguments.types = TypeArray.Allocate(array2);
			return true;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private static Expr GenerateOptionalArgument(MethodOrPropertySymbol methprop, CType type, int index)
		{
			CType cType = type.StripNubs();
			Expr expr;
			if (methprop.HasDefaultParameterValue(index))
			{
				CType defaultParameterValueConstValType = methprop.GetDefaultParameterValueConstValType(index);
				ConstVal defaultParameterValue = methprop.GetDefaultParameterValue(index);
				expr = ((defaultParameterValueConstValType.IsPredefType(PredefinedType.PT_DATETIME) && (cType.IsPredefType(PredefinedType.PT_DATETIME) || cType.IsPredefType(PredefinedType.PT_OBJECT) || cType.IsPredefType(PredefinedType.PT_VALUE))) ? ExprFactory.CreateConstant(SymbolLoader.GetPredefindType(PredefinedType.PT_DATETIME), ConstVal.Get(DateTime.FromBinary(defaultParameterValue.Int64Val))) : (defaultParameterValueConstValType.IsSimpleOrEnumOrString ? ExprFactory.CreateConstant((cType.IsEnumType && defaultParameterValueConstValType == cType.UnderlyingEnumType) ? cType : defaultParameterValueConstValType, defaultParameterValue) : (((!type.IsReferenceType && !(type is NullableType)) || !defaultParameterValue.IsNullRef) ? ExprFactory.CreateZeroInit(type) : ExprFactory.CreateNull())));
			}
			else if (type.IsPredefType(PredefinedType.PT_OBJECT))
			{
				if (methprop.MarshalAsObject(index))
				{
					expr = ExprFactory.CreateNull();
				}
				else
				{
					AggregateSymbol predefAgg = SymbolLoader.GetPredefAgg(PredefinedType.PT_MISSING);
					FieldWithType field = new FieldWithType(SymbolLoader.LookupAggMember(NameManager.GetPredefinedName(PredefinedName.PN_CAP_VALUE), predefAgg, symbmask_t.MASK_FieldSymbol) as FieldSymbol, predefAgg.getThisType());
					ExprField argument = ExprFactory.CreateField(predefAgg.getThisType(), null, field);
					expr = ExprFactory.CreateCast(type, argument);
				}
			}
			else
			{
				expr = ExprFactory.CreateZeroInit(type);
			}
			expr.IsOptionalArgument = true;
			return expr;
		}

		private static MethodOrPropertySymbol FindMostDerivedMethod(MethodOrPropertySymbol pMethProp, Expr pObject)
		{
			return FindMostDerivedMethod(pMethProp, pObject?.Type);
		}

		public static MethodOrPropertySymbol FindMostDerivedMethod(MethodOrPropertySymbol pMethProp, CType pType)
		{
			bool flag = false;
			MethodSymbol methodSymbol = pMethProp as MethodSymbol;
			if (methodSymbol == null)
			{
				PropertySymbol propertySymbol = (PropertySymbol)pMethProp;
				methodSymbol = propertySymbol.GetterMethod ?? propertySymbol.SetterMethod;
				if (methodSymbol == null)
				{
					return null;
				}
				flag = propertySymbol is IndexerSymbol;
			}
			if (!methodSymbol.isVirtual || pType == null)
			{
				return methodSymbol;
			}
			MethodSymbol methodSymbol2 = methodSymbol.swtSlot?.Meth();
			if (methodSymbol2 != null)
			{
				methodSymbol = methodSymbol2;
			}
			if (!(pType is AggregateType { OwningAggregate: var aggregateSymbol }))
			{
				return methodSymbol;
			}
			while (aggregateSymbol?.GetBaseAgg() != null)
			{
				for (MethodOrPropertySymbol methodOrPropertySymbol = SymbolLoader.LookupAggMember(methodSymbol.name, aggregateSymbol, symbmask_t.MASK_MethodSymbol | symbmask_t.MASK_PropertySymbol) as MethodOrPropertySymbol; methodOrPropertySymbol != null; methodOrPropertySymbol = methodOrPropertySymbol.LookupNext(symbmask_t.MASK_MethodSymbol | symbmask_t.MASK_PropertySymbol) as MethodOrPropertySymbol)
				{
					if (methodOrPropertySymbol.isOverride && methodOrPropertySymbol.swtSlot.Sym != null && methodOrPropertySymbol.swtSlot.Sym == methodSymbol)
					{
						if (flag)
						{
							return ((MethodSymbol)methodOrPropertySymbol).getProperty();
						}
						return methodOrPropertySymbol;
					}
				}
				aggregateSymbol = aggregateSymbol.GetBaseAgg();
			}
			return methodSymbol;
		}

		private bool HasOptionalParameters()
		{
			return FindMostDerivedMethod(_pCurrentSym, _pGroup.OptionalObject)?.HasOptionalParameters() ?? false;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool AddArgumentsForOptionalParameters()
		{
			if (_pCurrentParameters.Count <= _pArguments.carg)
			{
				return true;
			}
			MethodOrPropertySymbol methodOrPropertySymbol = FindMostDerivedMethod(_pCurrentSym, _pGroup.OptionalObject);
			if (methodOrPropertySymbol == null)
			{
				return false;
			}
			int num = _pArguments.carg;
			int num2 = 0;
			TypeArray typeArray = TypeManager.SubstTypeArray(_pCurrentParameters, _pCurrentType, _pGroup.TypeArgs);
			Expr[] array = new Expr[_pCurrentParameters.Count - num];
			while (num < typeArray.Count)
			{
				if (!methodOrPropertySymbol.IsParameterOptional(num))
				{
					return false;
				}
				array[num2] = GenerateOptionalArgument(methodOrPropertySymbol, typeArray[num], num);
				num++;
				num2++;
			}
			for (int i = 0; i < num2; i++)
			{
				_pArguments.prgexpr.Add(array[i]);
			}
			CType[] array2 = new CType[typeArray.Count];
			for (int j = 0; j < typeArray.Count; j++)
			{
				array2[j] = _pArguments.prgexpr[j].Type;
			}
			_pArguments.types = TypeArray.Allocate(array2);
			_pArguments.carg = typeArray.Count;
			_bArgumentsChangedForNamedOrOptionalArguments = true;
			return true;
		}

		private static Expr FindArgumentWithName(ArgInfos pArguments, Name pName)
		{
			List<Expr> prgexpr = pArguments.prgexpr;
			for (int i = 0; i < pArguments.carg; i++)
			{
				Expr expr = prgexpr[i];
				if (expr is ExprNamedArgumentSpecification exprNamedArgumentSpecification && exprNamedArgumentSpecification.Name == pName)
				{
					return expr;
				}
			}
			return null;
		}

		private bool NamedArgumentNamesAppearInParameterList(MethodOrPropertySymbol methprop)
		{
			List<Name> list = methprop.ParameterNames;
			HashSet<Name> hashSet = new HashSet<Name>();
			for (int i = 0; i < _pArguments.carg; i++)
			{
				if (!(_pArguments.prgexpr[i] is ExprNamedArgumentSpecification { Name: var name }))
				{
					if (!list.IsEmpty())
					{
						list = list.Tail();
					}
					continue;
				}
				if (!methprop.ParameterNames.Contains(name))
				{
					if (_pInvalidSpecifiedName == null)
					{
						_pInvalidSpecifiedName = name;
					}
					return false;
				}
				if (!list.Contains(name))
				{
					if (_pNameUsedInPositionalArgument == null)
					{
						_pNameUsedInPositionalArgument = name;
					}
					return false;
				}
				if (!hashSet.Add(name))
				{
					if (_pDuplicateSpecifiedName == null)
					{
						_pDuplicateSpecifiedName = name;
					}
					return false;
				}
			}
			return true;
		}

		private bool GetNextSym(CMemberLookupResults.CMethodIterator iterator)
		{
			if (!iterator.MoveNext())
			{
				return false;
			}
			_pCurrentSym = iterator.CurrentSymbol;
			AggregateType currentType = iterator.CurrentType;
			if (_pCurrentType != currentType && _pCurrentType != null && !_methList.IsEmpty() && !_methList.Head().mpwi.GetType().IsInterfaceType)
			{
				return false;
			}
			_pCurrentType = currentType;
			while (_HiddenTypes.Contains(_pCurrentType))
			{
				while (iterator.CurrentType == _pCurrentType)
				{
					iterator.MoveNext();
				}
				_pCurrentSym = iterator.CurrentSymbol;
				_pCurrentType = iterator.CurrentType;
				if (iterator.AtEnd)
				{
					return false;
				}
			}
			return true;
		}

		private bool ConstructExpandedParameters()
		{
			if (_pCurrentSym == null || _pArguments == null || _pCurrentParameters == null)
			{
				return false;
			}
			if ((_fBindFlags & BindingFlag.BIND_NOPARAMS) != 0)
			{
				return false;
			}
			if (!_pCurrentSym.isParamArray)
			{
				return false;
			}
			int num = 0;
			for (int i = _pArguments.carg; i < _pCurrentSym.Params.Count; i++)
			{
				if (_pCurrentSym.IsParameterOptional(i))
				{
					num++;
				}
			}
			if (_pArguments.carg + num < _pCurrentParameters.Count - 1)
			{
				return false;
			}
			return TryGetExpandedParams(_pCurrentSym.Params, _pArguments.carg, out _pCurrentParameters);
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private Result DetermineCurrentTypeArgs()
		{
			TypeArray typeArgs = _pGroup.TypeArgs;
			if (_pCurrentSym is MethodSymbol methodSymbol && methodSymbol.typeVars.Count != typeArgs.Count)
			{
				if (typeArgs.Count > 0)
				{
					if (!_mwtBadArity)
					{
						_mwtBadArity.Set(methodSymbol, _pCurrentType);
					}
					return Result.Failure_NoSearchForExpanded;
				}
				if (!MethodTypeInferrer.Infer(_pExprBinder, methodSymbol, _pCurrentParameters, _pArguments, out _pCurrentTypeArgs))
				{
					if (_results.IsBetterUninferableResult(_pCurrentTypeArgs))
					{
						TypeArray typeVars = methodSymbol.typeVars;
						if (typeVars != null && _pCurrentTypeArgs != null && typeVars.Count == _pCurrentTypeArgs.Count)
						{
							_mpwiCantInferInstArg.Set(methodSymbol, _pCurrentType, _pCurrentTypeArgs);
						}
						else
						{
							_mpwiCantInferInstArg.Set(methodSymbol, _pCurrentType, typeVars);
						}
					}
					return Result.Failure_SearchForExpanded;
				}
			}
			else
			{
				_pCurrentTypeArgs = typeArgs;
			}
			return Result.Success;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool ArgumentsAreConvertible()
		{
			bool flag = false;
			if (_pArguments.carg != 0)
			{
				UpdateArguments();
				for (int i = 0; i < _pArguments.carg; i++)
				{
					CType cType = _pCurrentParameters[i];
					if (!TypeBind.CheckConstraints(cType, CheckConstraintsFlags.NoErrors) && !DoesTypeArgumentsContainErrorSym(cType))
					{
						_mpwiParamTypeConstraints.Set(_pCurrentSym, _pCurrentType, _pCurrentTypeArgs);
						return false;
					}
				}
				for (int j = 0; j < _pArguments.carg; j++)
				{
					CType cType2 = _pCurrentParameters[j];
					flag |= DoesTypeArgumentsContainErrorSym(cType2);
					Expr expr = _pArguments.prgexpr[j];
					if (expr is ExprNamedArgumentSpecification exprNamedArgumentSpecification)
					{
						expr = exprNamedArgumentSpecification.Value;
					}
					if (_pExprBinder.canConvert(expr, cType2) || flag)
					{
						continue;
					}
					if (j > _nArgBest)
					{
						_nArgBest = j;
						if (!_results.BestResult)
						{
							_results.BestResult.Set(_pCurrentSym, _pCurrentType, _pCurrentTypeArgs);
						}
					}
					else if (j == _nArgBest && _pArguments.types[j] != cType2)
					{
						CType obj = ((_pArguments.types[j] is ParameterModifierType parameterModifierType) ? parameterModifierType.ParameterType : _pArguments.types[j]);
						CType cType3 = ((cType2 is ParameterModifierType parameterModifierType2) ? parameterModifierType2.ParameterType : cType2);
						if (obj == cType3 && !_results.BestResult)
						{
							_results.BestResult.Set(_pCurrentSym, _pCurrentType, _pCurrentTypeArgs);
						}
					}
					return false;
				}
			}
			if (flag)
			{
				if (_results.IsBetterUninferableResult(_pCurrentTypeArgs) && _pCurrentSym is MethodSymbol mps)
				{
					_results.UninferableResult.Set(mps, _pCurrentType, _pCurrentTypeArgs);
				}
				return false;
			}
			return true;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private void UpdateArguments()
		{
			_pCurrentParameters = TypeManager.SubstTypeArray(_pCurrentParameters, _pCurrentType, _pCurrentTypeArgs);
			if (_pArguments.prgexpr == null || _pArguments.prgexpr.Count == 0)
			{
				return;
			}
			MethodOrPropertySymbol methodOrPropertySymbol = null;
			for (int i = 0; i < _pCurrentParameters.Count; i++)
			{
				Expr expr = _pArguments.prgexpr[i];
				if (expr.IsOptionalArgument && _pCurrentParameters[i] != expr.Type)
				{
					if (methodOrPropertySymbol == null)
					{
						methodOrPropertySymbol = FindMostDerivedMethod(_pCurrentSym, _pGroup.OptionalObject);
					}
					Expr value = GenerateOptionalArgument(methodOrPropertySymbol, _pCurrentParameters[i], i);
					_pArguments.prgexpr[i] = value;
				}
			}
		}

		private static bool DoesTypeArgumentsContainErrorSym(CType var)
		{
			if (!(var is AggregateType { TypeArgsAll: var typeArgsAll }))
			{
				return false;
			}
			for (int i = 0; i < typeArgsAll.Count; i++)
			{
				CType cType = typeArgsAll[i];
				if (cType == null)
				{
					return true;
				}
				if (cType is AggregateType && DoesTypeArgumentsContainErrorSym(cType))
				{
					return true;
				}
			}
			return false;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private void ReportErrorsOnSuccess()
		{
			if (_pGroup.SymKind == SYMKIND.SK_MethodSymbol && _results.BestResult.TypeArgs.Count > 0)
			{
				TypeBind.CheckMethConstraints(new MethWithInst(_results.BestResult));
			}
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private RuntimeBinderException ReportErrorsOnFailure()
		{
			if (_pDuplicateSpecifiedName != null)
			{
				return ErrorHandling.Error(ErrorCode.ERR_DuplicateNamedArgument, _pDuplicateSpecifiedName);
			}
			if ((bool)_results.InaccessibleResult)
			{
				return CSemanticChecker.ReportAccessError(_results.InaccessibleResult, _pExprBinder.ContextForMemberLookup, GetTypeQualifier(_pGroup));
			}
			if ((bool)_misnamed)
			{
				List<Name> parameterNames = FindMostDerivedMethod(_misnamed.MethProp(), _pGroup.OptionalObject).ParameterNames;
				for (int i = 0; i != _pOriginalArguments.carg; i++)
				{
					if (_pOriginalArguments.prgexpr[i] is ExprNamedArgumentSpecification { Name: var name } && parameterNames[i] != name)
					{
						if (parameterNames.Contains(name))
						{
							return ErrorHandling.Error(ErrorCode.ERR_BadNonTrailingNamedArgument, name);
						}
						_pInvalidSpecifiedName = name;
						break;
					}
				}
			}
			else if ((bool)_mpwiBogus)
			{
				return ErrorHandling.Error(ErrorCode.ERR_BindToBogus, _mpwiBogus);
			}
			bool flag = false;
			Name name2 = _pGroup.Name;
			if (_pGroup.OptionalObject?.Type != null && _pGroup.OptionalObject.Type.IsDelegateType && _pGroup.Name == NameManager.GetPredefinedName(PredefinedName.PN_INVOKE))
			{
				flag = true;
				name2 = ((AggregateType)_pGroup.OptionalObject.Type).OwningAggregate.name;
			}
			if ((bool)_results.BestResult)
			{
				return ReportErrorsForBestMatching(flag);
			}
			if ((bool)_results.UninferableResult || (bool)_mpwiCantInferInstArg)
			{
				if (!_results.UninferableResult)
				{
					_results.UninferableResult.Set(_mpwiCantInferInstArg.Sym as MethodSymbol, _mpwiCantInferInstArg.GetType(), _mpwiCantInferInstArg.TypeArgs);
				}
				MethWithType methWithType = new MethWithType(_results.UninferableResult.Meth(), _results.UninferableResult.GetType());
				return ErrorHandling.Error(ErrorCode.ERR_CantInferMethTypeArgs, methWithType);
			}
			if ((bool)_mwtBadArity)
			{
				return ErrorHandling.Error((_mwtBadArity.Meth().typeVars.Count > 0) ? ErrorCode.ERR_BadArity : ErrorCode.ERR_HasNoTypeVars, _mwtBadArity, new ErrArgSymKind(_mwtBadArity.Meth()), _pArguments.carg);
			}
			if ((bool)_mpwiParamTypeConstraints)
			{
				TypeBind.CheckMethConstraints(new MethWithInst(_mpwiParamTypeConstraints));
				return null;
			}
			if (_pInvalidSpecifiedName != null)
			{
				if (!(_pGroup.OptionalObject?.Type is AggregateType aggregateType) || !aggregateType.OwningAggregate.IsDelegate())
				{
					return ErrorHandling.Error(ErrorCode.ERR_BadNamedArgument, _pGroup.Name, _pInvalidSpecifiedName);
				}
				return ErrorHandling.Error(ErrorCode.ERR_BadNamedArgumentForDelegateInvoke, aggregateType.OwningAggregate.name, _pInvalidSpecifiedName);
			}
			if (_pNameUsedInPositionalArgument != null)
			{
				return ErrorHandling.Error(ErrorCode.ERR_NamedArgumentUsedInPositional, _pNameUsedInPositionalArgument);
			}
			if (_fCandidatesUnsupported)
			{
				return ErrorHandling.Error(ErrorCode.ERR_BindToBogus, name2);
			}
			if (flag)
			{
				return ErrorHandling.Error(ErrorCode.ERR_BadDelArgCount, name2, _pArguments.carg);
			}
			if ((_pGroup.Flags & EXPRFLAG.EXF_CTOR) != 0)
			{
				return ErrorHandling.Error(ErrorCode.ERR_BadCtorArgCount, _pGroup.ParentType, _pArguments.carg);
			}
			return ErrorHandling.Error(ErrorCode.ERR_BadArgCount, name2, _pArguments.carg);
		}

		private RuntimeBinderException ReportErrorsForBestMatching(bool bUseDelegateErrors)
		{
			if (bUseDelegateErrors)
			{
				return ErrorHandling.Error(ErrorCode.ERR_BadDelArgTypes, _results.BestResult.GetType());
			}
			return ErrorHandling.Error(ErrorCode.ERR_BadArgTypes, _results.BestResult);
		}
	}

	internal sealed class GroupToArgsBinderResult
	{
		public MethPropWithInst BestResult { get; set; }

		public MethPropWithInst InaccessibleResult { get; }

		public MethPropWithInst UninferableResult { get; }

		public GroupToArgsBinderResult()
		{
			BestResult = new MethPropWithInst();
			InaccessibleResult = new MethPropWithInst();
			UninferableResult = new MethPropWithInst();
		}

		private static int NumberOfErrorTypes(TypeArray pTypeArgs)
		{
			int num = 0;
			for (int i = 0; i < pTypeArgs.Count; i++)
			{
				if (pTypeArgs[i] == null)
				{
					num++;
				}
			}
			return num;
		}

		private static bool IsBetterThanCurrent(TypeArray pTypeArgs1, TypeArray pTypeArgs2)
		{
			int num = NumberOfErrorTypes(pTypeArgs1);
			int num2 = NumberOfErrorTypes(pTypeArgs2);
			if (num == num2)
			{
				int num3 = ((pTypeArgs1.Count > pTypeArgs2.Count) ? pTypeArgs2.Count : pTypeArgs1.Count);
				for (int i = 0; i < num3; i++)
				{
					if (pTypeArgs1[i] is AggregateType aggregateType)
					{
						num += NumberOfErrorTypes(aggregateType.TypeArgsAll);
					}
					if (pTypeArgs2[i] is AggregateType aggregateType2)
					{
						num2 += NumberOfErrorTypes(aggregateType2.TypeArgsAll);
					}
				}
			}
			return num2 < num;
		}

		public bool IsBetterUninferableResult(TypeArray pTypeArguments)
		{
			if (UninferableResult.Sym == null)
			{
				return true;
			}
			if (pTypeArguments == null)
			{
				return false;
			}
			return IsBetterThanCurrent(UninferableResult.TypeArgs, pTypeArguments);
		}
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private sealed class ImplicitConversion
	{
		private Expr _exprDest;

		private readonly ExpressionBinder _binder;

		private readonly Expr _exprSrc;

		private readonly CType _typeSrc;

		private readonly CType _typeDest;

		private readonly bool _needsExprDest;

		private CONVERTTYPE _flags;

		public Expr ExprDest => _exprDest;

		public ImplicitConversion(ExpressionBinder binder, Expr exprSrc, CType typeSrc, CType typeDest, bool needsExprDest, CONVERTTYPE flags)
		{
			_binder = binder;
			_exprSrc = exprSrc;
			_typeSrc = typeSrc;
			_typeDest = typeDest;
			_needsExprDest = needsExprDest;
			_flags = flags;
			_exprDest = null;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		public bool Bind()
		{
			if (_typeSrc == null || _typeDest == null || _typeDest is MethodGroupType)
			{
				return false;
			}
			switch (_typeDest.TypeKind)
			{
			case TypeKind.TK_NullType:
				if (!(_typeSrc is NullType))
				{
					return false;
				}
				if (_needsExprDest)
				{
					_exprDest = _exprSrc;
				}
				return true;
			case TypeKind.TK_ArgumentListType:
				return _typeSrc == _typeDest;
			case TypeKind.TK_VoidType:
				return false;
			default:
			{
				if (_typeSrc == _typeDest && ((_flags & CONVERTTYPE.ISEXPLICIT) == 0 || (!_typeSrc.IsPredefType(PredefinedType.PT_FLOAT) && !_typeSrc.IsPredefType(PredefinedType.PT_DOUBLE))))
				{
					if (_needsExprDest)
					{
						_exprDest = _exprSrc;
					}
					return true;
				}
				if (_typeDest is NullableType nubDst)
				{
					return BindNubConversion(nubDst);
				}
				if (_typeSrc is NullableType nubSrc)
				{
					return bindImplicitConversionFromNullable(nubSrc);
				}
				if ((_flags & CONVERTTYPE.ISEXPLICIT) != 0)
				{
					_flags |= CONVERTTYPE.NOUDC;
				}
				_ = _typeDest.FundamentalType;
				switch (_typeSrc.TypeKind)
				{
				case TypeKind.TK_VoidType:
				case TypeKind.TK_ArgumentListType:
				case TypeKind.TK_ParameterModifierType:
					return false;
				case TypeKind.TK_NullType:
					if (bindImplicitConversionFromNull())
					{
						return true;
					}
					break;
				case TypeKind.TK_ArrayType:
					if (bindImplicitConversionFromArray())
					{
						return true;
					}
					break;
				case TypeKind.TK_PointerType:
					if (bindImplicitConversionFromPointer())
					{
						return true;
					}
					break;
				case TypeKind.TK_AggregateType:
					if (bindImplicitConversionFromAgg(_typeSrc as AggregateType))
					{
						return true;
					}
					break;
				}
				object obj = _exprSrc?.RuntimeObject;
				if (obj != null && _typeDest.AssociatedSystemType.IsInstanceOfType(obj) && CSemanticChecker.CheckTypeAccess(_typeDest, _binder.Context.ContextForMemberLookup))
				{
					if (_needsExprDest)
					{
						_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest, _exprSrc.Flags & EXPRFLAG.EXF_CANTBENULL);
					}
					return true;
				}
				if ((_flags & CONVERTTYPE.NOUDC) == 0)
				{
					return _binder.bindUserDefinedConversion(_exprSrc, _typeSrc, _typeDest, _needsExprDest, out _exprDest, fImplicitOnly: true);
				}
				return false;
			}
			}
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
		private bool BindNubConversion(NullableType nubDst)
		{
			nubDst.GetAts();
			if (SymbolLoader.HasBaseConversion(nubDst.UnderlyingType, _typeSrc) && !CConversions.FWrappingConv(_typeSrc, nubDst))
			{
				if ((_flags & CONVERTTYPE.ISEXPLICIT) == 0)
				{
					return false;
				}
				if (_needsExprDest)
				{
					_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest, EXPRFLAG.EXF_INDEXER);
				}
				return true;
			}
			CType cType = nubDst.StripNubs(out var wasNullable);
			CType cType2 = _typeSrc.StripNubs(out var wasNullable2);
			ConversionFunc conversionFunc = (((_flags & CONVERTTYPE.ISEXPLICIT) != 0) ? new ConversionFunc(_binder.BindExplicitConversion) : new ConversionFunc(_binder.BindImplicitConversion));
			if (!wasNullable2)
			{
				if (_typeSrc is NullType)
				{
					if (_needsExprDest)
					{
						_exprDest = ((_exprSrc is ExprConstant) ? ExprFactory.CreateZeroInit(nubDst) : ExprFactory.CreateCast(_typeDest, _exprSrc));
					}
					return true;
				}
				Expr ppDestinationExpr = _exprSrc;
				if (_typeSrc == cType || conversionFunc(_exprSrc, _typeSrc, cType, _needsExprDest, out ppDestinationExpr, _flags | CONVERTTYPE.NOUDC))
				{
					if (_needsExprDest)
					{
						ExprUserDefinedConversion exprUserDefinedConversion = ppDestinationExpr as ExprUserDefinedConversion;
						if (exprUserDefinedConversion != null)
						{
							ppDestinationExpr = exprUserDefinedConversion.UserDefinedCall;
						}
						if (wasNullable)
						{
							((ExprCall)(ppDestinationExpr = BindNubNew(ppDestinationExpr))).NullableCallLiftKind = NullableCallLiftKind.NullableConversionConstructor;
						}
						if (exprUserDefinedConversion != null)
						{
							exprUserDefinedConversion.UserDefinedCall = ppDestinationExpr;
							ppDestinationExpr = exprUserDefinedConversion;
						}
						_exprDest = ppDestinationExpr;
					}
					return true;
				}
				if ((_flags & CONVERTTYPE.NOUDC) == 0)
				{
					return _binder.bindUserDefinedConversion(_exprSrc, _typeSrc, nubDst, _needsExprDest, out _exprDest, (_flags & CONVERTTYPE.ISEXPLICIT) == 0);
				}
				return false;
			}
			if (cType2 != cType && !conversionFunc(null, cType2, cType, needsExprDest: false, out _exprDest, _flags | CONVERTTYPE.NOUDC))
			{
				if ((_flags & CONVERTTYPE.NOUDC) == 0)
				{
					return _binder.bindUserDefinedConversion(_exprSrc, _typeSrc, nubDst, _needsExprDest, out _exprDest, (_flags & CONVERTTYPE.ISEXPLICIT) == 0);
				}
				return false;
			}
			if (_needsExprDest)
			{
				MethWithInst method = new MethWithInst(null, null);
				ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, method);
				ExprCall exprCall = ExprFactory.CreateCall((EXPRFLAG)0, nubDst, _exprSrc, memberGroup, null);
				Expr ppDestinationExpr2 = _binder.mustCast(_exprSrc, cType2);
				if (!(((_flags & CONVERTTYPE.ISEXPLICIT) != 0) ? _binder.BindExplicitConversion(ppDestinationExpr2, ppDestinationExpr2.Type, cType, out ppDestinationExpr2, _flags | CONVERTTYPE.NOUDC) : _binder.BindImplicitConversion(ppDestinationExpr2, ppDestinationExpr2.Type, cType, out ppDestinationExpr2, _flags | CONVERTTYPE.NOUDC)))
				{
					return false;
				}
				exprCall.CastOfNonLiftedResultToLiftedType = _binder.mustCast(ppDestinationExpr2, nubDst, (CONVERTTYPE)0);
				exprCall.NullableCallLiftKind = NullableCallLiftKind.NullableConversion;
				exprCall.PConversions = exprCall.CastOfNonLiftedResultToLiftedType;
				_exprDest = exprCall;
			}
			return true;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindImplicitConversionFromNull()
		{
			FUNDTYPE fundamentalType = _typeDest.FundamentalType;
			if (fundamentalType != FUNDTYPE.FT_REF && fundamentalType != FUNDTYPE.FT_PTR && !_typeDest.IsPredefType(PredefinedType.PT_G_OPTIONAL))
			{
				return false;
			}
			if (_needsExprDest)
			{
				_exprDest = ((_exprSrc is ExprConstant) ? ExprFactory.CreateZeroInit(_typeDest) : ExprFactory.CreateCast(_typeDest, _exprSrc));
			}
			return true;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindImplicitConversionFromNullable(NullableType nubSrc)
		{
			if (nubSrc.GetAts() == _typeDest)
			{
				if (_needsExprDest)
				{
					_exprDest = _exprSrc;
				}
				return true;
			}
			if (SymbolLoader.HasBaseConversion(nubSrc.UnderlyingType, _typeDest) && !CConversions.FUnwrappingConv(nubSrc, _typeDest))
			{
				if (_needsExprDest)
				{
					_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest, EXPRFLAG.EXF_CTOR);
					if (!_typeDest.IsPredefType(PredefinedType.PT_OBJECT))
					{
						_binder.bindSimpleCast(_exprDest, _typeDest, out _exprDest, EXPRFLAG.EXF_ASFINALLYLEAVE);
					}
				}
				return true;
			}
			if ((_flags & CONVERTTYPE.NOUDC) == 0)
			{
				return _binder.bindUserDefinedConversion(_exprSrc, nubSrc, _typeDest, _needsExprDest, out _exprDest, fImplicitOnly: true);
			}
			return false;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindImplicitConversionFromArray()
		{
			if (!SymbolLoader.HasBaseConversion(_typeSrc, _typeDest))
			{
				return false;
			}
			EXPRFLAG exprFlags = (EXPRFLAG)0;
			if ((_typeDest is ArrayType || (_typeDest is AggregateType { IsInterfaceType: not false } aggregateType && aggregateType.TypeArgsAll.Count == 1 && (aggregateType.TypeArgsAll[0] != ((ArrayType)_typeSrc).ElementType || (_flags & CONVERTTYPE.FORCECAST) != 0))) && ((_flags & CONVERTTYPE.FORCECAST) != 0 || TypeManager.TypeContainsTyVars(_typeSrc, null) || TypeManager.TypeContainsTyVars(_typeDest, null)))
			{
				exprFlags = EXPRFLAG.EXF_OPERATOR;
			}
			if (_needsExprDest)
			{
				_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest, exprFlags);
			}
			return true;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindImplicitConversionFromPointer()
		{
			if (_typeDest is PointerType pointerType && pointerType.ReferentType == VoidType.Instance)
			{
				if (_needsExprDest)
				{
					_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest);
				}
				return true;
			}
			return false;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindImplicitConversionFromAgg(AggregateType aggTypeSrc)
		{
			AggregateSymbol owningAggregate = aggTypeSrc.OwningAggregate;
			if (owningAggregate.IsEnum())
			{
				return bindImplicitConversionFromEnum(aggTypeSrc);
			}
			if (_typeDest.IsEnumType)
			{
				if (bindImplicitConversionToEnum(aggTypeSrc))
				{
					return true;
				}
			}
			else if (owningAggregate.getThisType().IsSimpleType && _typeDest.IsSimpleType && bindImplicitConversionBetweenSimpleTypes(aggTypeSrc))
			{
				return true;
			}
			return bindImplicitConversionToBase(aggTypeSrc);
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindImplicitConversionToBase(AggregateType pSource)
		{
			if (!(_typeDest is AggregateType) || !SymbolLoader.HasBaseConversion(pSource, _typeDest))
			{
				return false;
			}
			EXPRFLAG exprFlags = (EXPRFLAG)0;
			if (pSource.OwningAggregate.IsStruct() && _typeDest.FundamentalType == FUNDTYPE.FT_REF)
			{
				exprFlags = EXPRFLAG.EXF_CTOR | EXPRFLAG.EXF_CANTBENULL;
			}
			else if (_exprSrc != null)
			{
				exprFlags = _exprSrc.Flags & EXPRFLAG.EXF_CANTBENULL;
			}
			if (_needsExprDest)
			{
				_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest, exprFlags);
			}
			return true;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		private bool bindImplicitConversionFromEnum(AggregateType aggTypeSrc)
		{
			if (_typeDest is AggregateType pDest && SymbolLoader.HasBaseConversion(aggTypeSrc, pDest))
			{
				if (_needsExprDest)
				{
					_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest, EXPRFLAG.EXF_CTOR | EXPRFLAG.EXF_CANTBENULL);
				}
				return true;
			}
			return false;
		}

		private bool bindImplicitConversionToEnum(AggregateType aggTypeSrc)
		{
			if (aggTypeSrc.OwningAggregate.GetPredefType() != PredefinedType.PT_BOOL && _exprSrc != null && _exprSrc.IsZero() && _exprSrc.Type.IsNumericType && (_flags & CONVERTTYPE.STANDARD) == 0)
			{
				if (_needsExprDest)
				{
					_exprDest = ExprFactory.CreateConstant(_typeDest, ConstVal.GetDefaultValue(_typeDest.ConstValKind));
				}
				return true;
			}
			return false;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
		private bool bindImplicitConversionBetweenSimpleTypes(AggregateType aggTypeSrc)
		{
			PredefinedType predefType = aggTypeSrc.OwningAggregate.GetPredefType();
			PredefinedType predefinedType = _typeDest.PredefinedType;
			ConvKind convKind = ((_exprSrc is ExprConstant exprSrc && ((predefType == PredefinedType.PT_INT && predefinedType != PredefinedType.PT_BOOL && predefinedType != PredefinedType.PT_CHAR) || (predefType == PredefinedType.PT_LONG && predefinedType == PredefinedType.PT_ULONG)) && isConstantInRange(exprSrc, _typeDest)) ? ConvKind.Implicit : ((predefType != predefinedType) ? GetConvKind(predefType, predefinedType) : ConvKind.Implicit));
			if (convKind != ConvKind.Implicit)
			{
				return false;
			}
			if (_exprSrc.GetConst() != null && _binder.bindConstantCast(_exprSrc, _typeDest, _needsExprDest, out _exprDest, explicitConversion: false) == ConstCastResult.Success)
			{
				return true;
			}
			if (isUserDefinedConversion(predefType, predefinedType))
			{
				if (!_needsExprDest)
				{
					return true;
				}
				return _binder.bindUserDefinedConversion(_exprSrc, aggTypeSrc, _typeDest, _needsExprDest, out _exprDest, fImplicitOnly: true);
			}
			if (_needsExprDest)
			{
				_binder.bindSimpleCast(_exprSrc, _typeDest, out _exprDest);
			}
			return true;
		}
	}

	private class UnaOpSig
	{
		public PredefinedType pt;

		public UnaOpMask grfuom;

		public int cuosSkip;

		public PfnBindUnaOp pfn;

		public UnaOpFuncKind fnkind;

		protected UnaOpSig()
		{
		}

		public UnaOpSig(PredefinedType pt, UnaOpMask grfuom, int cuosSkip, PfnBindUnaOp pfn, UnaOpFuncKind fnkind)
		{
			this.pt = pt;
			this.grfuom = grfuom;
			this.cuosSkip = cuosSkip;
			this.pfn = pfn;
			this.fnkind = fnkind;
		}
	}

	private sealed class UnaOpFullSig : UnaOpSig
	{
		private readonly LiftFlags _grflt;

		private readonly CType _type;

		public UnaOpFullSig(CType type, PfnBindUnaOp pfn, LiftFlags grflt, UnaOpFuncKind fnkind)
		{
			pt = PredefinedType.PT_UNDEFINEDINDEX;
			grfuom = UnaOpMask.None;
			cuosSkip = 0;
			base.pfn = pfn;
			_type = type;
			_grflt = grflt;
			base.fnkind = fnkind;
		}

		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
		public UnaOpFullSig(UnaOpSig uos)
		{
			pt = uos.pt;
			grfuom = uos.grfuom;
			cuosSkip = uos.cuosSkip;
			pfn = uos.pfn;
			fnkind = uos.fnkind;
			_type = ((pt != PredefinedType.PT_UNDEFINEDINDEX) ? GetPredefindType(pt) : null);
			_grflt = LiftFlags.None;
		}

		public bool FPreDef()
		{
			return pt != PredefinedType.PT_UNDEFINEDINDEX;
		}

		public bool isLifted()
		{
			if (_grflt == LiftFlags.None)
			{
				return false;
			}
			return true;
		}

		public bool Convert()
		{
			return (_grflt & LiftFlags.Convert1) != 0;
		}

		public new CType GetType()
		{
			return _type;
		}
	}

	private static readonly byte[][] s_betterConversionTable = new byte[16][]
	{
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 2,
			3, 3, 3, 3, 3, 3
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			1, 1, 1, 3, 3, 3
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 1, 1, 3, 3, 3
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 1, 3, 3, 3
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3
		},
		new byte[16]
		{
			1, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			1, 1, 1, 3, 3, 3
		},
		new byte[16]
		{
			3, 2, 3, 3, 3, 3, 3, 3, 3, 2,
			3, 3, 3, 3, 3, 3
		},
		new byte[16]
		{
			3, 2, 2, 3, 3, 3, 3, 3, 3, 2,
			3, 3, 3, 3, 3, 3
		},
		new byte[16]
		{
			3, 2, 2, 2, 3, 3, 3, 3, 3, 2,
			3, 3, 3, 3, 3, 3
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3
		}
	};

	private static readonly byte[][] s_simpleTypeConversions = new byte[13][]
	{
		new byte[13]
		{
			1, 2, 2, 2, 2, 2, 66, 3, 5, 3,
			2, 2, 2
		},
		new byte[13]
		{
			3, 1, 2, 2, 2, 2, 66, 3, 5, 3,
			3, 3, 3
		},
		new byte[13]
		{
			3, 3, 1, 2, 2, 2, 66, 3, 5, 3,
			3, 3, 3
		},
		new byte[13]
		{
			3, 3, 3, 1, 2, 2, 66, 3, 5, 3,
			3, 3, 3
		},
		new byte[13]
		{
			3, 3, 3, 3, 1, 2, 67, 3, 5, 3,
			3, 3, 3
		},
		new byte[13]
		{
			3, 3, 3, 3, 3, 1, 67, 3, 5, 3,
			3, 3, 3
		},
		new byte[13]
		{
			67, 67, 67, 67, 67, 67, 1, 67, 5, 67,
			67, 67, 67
		},
		new byte[13]
		{
			3, 3, 2, 2, 2, 2, 66, 1, 5, 3,
			2, 2, 2
		},
		new byte[13]
		{
			5, 5, 5, 5, 5, 5, 5, 5, 1, 5,
			5, 5, 5
		},
		new byte[13]
		{
			3, 2, 2, 2, 2, 2, 66, 3, 5, 1,
			3, 3, 3
		},
		new byte[13]
		{
			3, 3, 2, 2, 2, 2, 66, 3, 5, 3,
			1, 2, 2
		},
		new byte[13]
		{
			3, 3, 3, 2, 2, 2, 66, 3, 5, 3,
			3, 1, 2
		},
		new byte[13]
		{
			3, 3, 3, 3, 2, 2, 66, 3, 5, 3,
			3, 3, 1
		}
	};

	private static readonly byte[][] s_simpleTypeBetter = new byte[16][]
	{
		new byte[16]
		{
			0, 1, 1, 1, 1, 1, 1, 3, 3, 2,
			1, 1, 1, 3, 3, 1
		},
		new byte[16]
		{
			2, 0, 1, 1, 1, 1, 1, 3, 3, 2,
			1, 1, 1, 3, 3, 1
		},
		new byte[16]
		{
			2, 2, 0, 1, 1, 1, 1, 2, 3, 2,
			2, 1, 1, 3, 3, 1
		},
		new byte[16]
		{
			2, 2, 2, 0, 1, 1, 1, 2, 3, 2,
			2, 2, 1, 3, 3, 1
		},
		new byte[16]
		{
			2, 2, 2, 2, 0, 1, 3, 2, 3, 2,
			2, 2, 2, 3, 3, 1
		},
		new byte[16]
		{
			2, 2, 2, 2, 2, 0, 3, 2, 3, 2,
			2, 2, 2, 3, 3, 1
		},
		new byte[16]
		{
			2, 2, 2, 2, 3, 3, 0, 2, 3, 2,
			2, 2, 2, 3, 3, 1
		},
		new byte[16]
		{
			3, 3, 1, 1, 1, 1, 1, 0, 3, 3,
			1, 1, 1, 3, 3, 1
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 0, 3,
			3, 3, 3, 3, 3, 1
		},
		new byte[16]
		{
			1, 1, 1, 1, 1, 1, 1, 3, 3, 0,
			1, 1, 1, 3, 3, 1
		},
		new byte[16]
		{
			2, 2, 1, 1, 1, 1, 1, 2, 3, 2,
			0, 1, 1, 3, 3, 1
		},
		new byte[16]
		{
			2, 2, 2, 1, 1, 1, 1, 2, 3, 2,
			2, 0, 1, 3, 3, 1
		},
		new byte[16]
		{
			2, 2, 2, 2, 1, 1, 1, 2, 3, 2,
			2, 2, 0, 3, 3, 1
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 0, 3, 1
		},
		new byte[16]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 0, 1
		},
		new byte[16]
		{
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 0
		}
	};

	private static readonly int[] s_zero = new int[1];

	private static readonly BinOpSig[] s_binopSignatures = InitBinopSignatures();

	private static readonly UnaOpSig[] s_rguos = InitUnaOpSignatures();

	public BindingContext Context { get; }

	private static ReadOnlySpan<PredefinedType> RgptIntOp => new PredefinedType[4]
	{
		PredefinedType.PT_INT,
		PredefinedType.PT_UINT,
		PredefinedType.PT_LONG,
		PredefinedType.PT_ULONG
	};

	private static ReadOnlySpan<PredefinedName> EK2NAME => new PredefinedName[26]
	{
		PredefinedName.PN_OPEQUALS,
		PredefinedName.PN_OPCOMPARE,
		PredefinedName.PN_OPTRUE,
		PredefinedName.PN_OPFALSE,
		PredefinedName.PN_OPINCREMENT,
		PredefinedName.PN_OPDECREMENT,
		PredefinedName.PN_OPNEGATION,
		PredefinedName.PN_OPEQUALITY,
		PredefinedName.PN_OPINEQUALITY,
		PredefinedName.PN_OPLESSTHAN,
		PredefinedName.PN_OPLESSTHANOREQUAL,
		PredefinedName.PN_OPGREATERTHAN,
		PredefinedName.PN_OPGREATERTHANOREQUAL,
		PredefinedName.PN_OPPLUS,
		PredefinedName.PN_OPMINUS,
		PredefinedName.PN_OPMULTIPLY,
		PredefinedName.PN_OPDIVISION,
		PredefinedName.PN_OPMODULUS,
		PredefinedName.PN_OPUNARYMINUS,
		PredefinedName.PN_OPUNARYPLUS,
		PredefinedName.PN_OPBITWISEAND,
		PredefinedName.PN_OPBITWISEOR,
		PredefinedName.PN_OPXOR,
		PredefinedName.PN_OPCOMPLEMENT,
		PredefinedName.PN_OPLEFTSHIFT,
		PredefinedName.PN_OPRIGHTSHIFT
	};

	private AggregateSymbol ContextForMemberLookup => Context.ContextForMemberLookup;

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static BetterType WhichMethodIsBetterTieBreaker(CandidateFunctionMember node1, CandidateFunctionMember node2, CType pTypeThrough, ArgInfos args)
	{
		MethPropWithInst mpwi = node1.mpwi;
		MethPropWithInst mpwi2 = node2.mpwi;
		if (node1.ctypeLift != node2.ctypeLift)
		{
			if (node1.ctypeLift >= node2.ctypeLift)
			{
				return BetterType.Right;
			}
			return BetterType.Left;
		}
		if (mpwi.TypeArgs.Count != 0)
		{
			if (mpwi2.TypeArgs.Count == 0)
			{
				return BetterType.Right;
			}
		}
		else if (mpwi2.TypeArgs.Count != 0)
		{
			return BetterType.Left;
		}
		if (node1.fExpanded)
		{
			if (!node2.fExpanded)
			{
				return BetterType.Right;
			}
		}
		else if (node2.fExpanded)
		{
			return BetterType.Left;
		}
		BetterType betterType = CompareTypes(RearrangeNamedArguments(mpwi.MethProp().Params, mpwi, pTypeThrough, args), RearrangeNamedArguments(mpwi2.MethProp().Params, mpwi2, pTypeThrough, args));
		if (betterType == BetterType.Left || betterType == BetterType.Right)
		{
			return betterType;
		}
		if (mpwi.MethProp().modOptCount != mpwi2.MethProp().modOptCount)
		{
			if (mpwi.MethProp().modOptCount >= mpwi2.MethProp().modOptCount)
			{
				return BetterType.Right;
			}
			return BetterType.Left;
		}
		return BetterType.Neither;
	}

	private static BetterType CompareTypes(TypeArray ta1, TypeArray ta2)
	{
		if (ta1 == ta2)
		{
			return BetterType.Same;
		}
		if (ta1.Count != ta2.Count)
		{
			if (ta1.Count <= ta2.Count)
			{
				return BetterType.Right;
			}
			return BetterType.Left;
		}
		BetterType betterType = BetterType.Neither;
		for (int i = 0; i < ta1.Count; i++)
		{
			CType cType = ta1[i];
			CType cType2 = ta2[i];
			BetterType betterType2 = BetterType.Neither;
			while (true)
			{
				if (cType.TypeKind != cType2.TypeKind)
				{
					if (cType is TypeParameterType)
					{
						betterType2 = BetterType.Right;
					}
					else if (cType2 is TypeParameterType)
					{
						betterType2 = BetterType.Left;
					}
				}
				else
				{
					switch (cType.TypeKind)
					{
					case TypeKind.TK_ArrayType:
					case TypeKind.TK_PointerType:
					case TypeKind.TK_ParameterModifierType:
					case TypeKind.TK_NullableType:
						goto IL_00a3;
					case TypeKind.TK_AggregateType:
						betterType2 = CompareTypes(((AggregateType)cType).TypeArgsAll, ((AggregateType)cType2).TypeArgsAll);
						break;
					}
				}
				break;
				IL_00a3:
				cType = cType.BaseOrParameterOrElementType;
				cType2 = cType2.BaseOrParameterOrElementType;
			}
			if (betterType2 == BetterType.Right || betterType2 == BetterType.Left)
			{
				if (betterType == BetterType.Same || betterType == BetterType.Neither)
				{
					betterType = betterType2;
				}
				else if (betterType2 != betterType)
				{
					return BetterType.Neither;
				}
			}
		}
		return betterType;
	}

	private static int FindName(List<Name> names, Name name)
	{
		return names.IndexOf(name);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static TypeArray RearrangeNamedArguments(TypeArray pta, MethPropWithInst mpwi, CType pTypeThrough, ArgInfos args)
	{
		if (args.carg == 0 || !(args.prgexpr[args.carg - 1] is ExprNamedArgumentSpecification))
		{
			return pta;
		}
		CType pType = pTypeThrough ?? mpwi.GetType();
		CType[] array = new CType[pta.Count];
		MethodOrPropertySymbol methodOrPropertySymbol = GroupToArgsBinder.FindMostDerivedMethod(mpwi.MethProp(), pType);
		for (int i = 0; i < pta.Count; i++)
		{
			array[i] = pta[i];
		}
		List<Expr> prgexpr = args.prgexpr;
		for (int j = 0; j < args.carg; j++)
		{
			if (prgexpr[j] is ExprNamedArgumentSpecification exprNamedArgumentSpecification)
			{
				int num = FindName(methodOrPropertySymbol.ParameterNames, exprNamedArgumentSpecification.Name);
				CType cType = pta[num];
				for (int k = j; k < num; k++)
				{
					array[k + 1] = array[k];
				}
				array[j] = cType;
			}
		}
		return TypeArray.Allocate(array);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private BetterType WhichMethodIsBetter(CandidateFunctionMember node1, CandidateFunctionMember node2, CType pTypeThrough, ArgInfos args)
	{
		MethPropWithInst mpwi = node1.mpwi;
		MethPropWithInst mpwi2 = node2.mpwi;
		TypeArray typeArray = RearrangeNamedArguments(node1.@params, mpwi, pTypeThrough, args);
		TypeArray typeArray2 = RearrangeNamedArguments(node2.@params, mpwi2, pTypeThrough, args);
		if (typeArray == typeArray2)
		{
			return WhichMethodIsBetterTieBreaker(node1, node2, pTypeThrough, args);
		}
		BetterType betterType = BetterType.Neither;
		int carg = args.carg;
		for (int i = 0; i < carg; i++)
		{
			Expr expr = args.prgexpr[i];
			CType p = typeArray[i];
			CType p2 = typeArray2[i];
			CType argType = expr?.RuntimeObjectActualType ?? args.types[i];
			BetterType betterType2 = WhichConversionIsBetter(argType, p, p2);
			switch (betterType)
			{
			case BetterType.Right:
				if (betterType2 != BetterType.Left)
				{
					continue;
				}
				betterType = BetterType.Neither;
				break;
			case BetterType.Left:
				if (betterType2 != BetterType.Right)
				{
					continue;
				}
				betterType = BetterType.Neither;
				break;
			default:
				if (betterType2 == BetterType.Right || betterType2 == BetterType.Left)
				{
					betterType = betterType2;
				}
				continue;
			}
			break;
		}
		if (typeArray.Count != typeArray2.Count && betterType == BetterType.Neither)
		{
			if (node1.fExpanded)
			{
				if (!node2.fExpanded)
				{
					return BetterType.Right;
				}
			}
			else if (node2.fExpanded)
			{
				return BetterType.Left;
			}
			if (typeArray.Count == carg)
			{
				return BetterType.Left;
			}
			if (typeArray2.Count == carg)
			{
				return BetterType.Right;
			}
			return BetterType.Neither;
		}
		return betterType;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private BetterType WhichConversionIsBetter(CType argType, CType p1, CType p2)
	{
		if (p1 == p2)
		{
			return BetterType.Same;
		}
		if (argType == p1)
		{
			return BetterType.Left;
		}
		if (argType == p2)
		{
			return BetterType.Right;
		}
		bool flag = canConvert(p1, p2);
		bool flag2 = canConvert(p2, p1);
		if (flag != flag2)
		{
			if (!flag)
			{
				return BetterType.Right;
			}
			return BetterType.Left;
		}
		if (p1.IsPredefined && p2.IsPredefined)
		{
			PredefinedType predefinedType = p1.PredefinedType;
			if (predefinedType <= PredefinedType.PT_OBJECT)
			{
				PredefinedType predefinedType2 = p2.PredefinedType;
				if (predefinedType2 <= PredefinedType.PT_OBJECT)
				{
					return (BetterType)s_betterConversionTable[(uint)predefinedType][(uint)predefinedType2];
				}
			}
		}
		return BetterType.Neither;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private CandidateFunctionMember FindBestMethod(List<CandidateFunctionMember> list, CType pTypeThrough, ArgInfos args, out CandidateFunctionMember methAmbig1, out CandidateFunctionMember methAmbig2)
	{
		CandidateFunctionMember candidateFunctionMember = null;
		CandidateFunctionMember candidateFunctionMember2 = null;
		bool flag = false;
		CandidateFunctionMember candidateFunctionMember3 = list[0];
		for (int i = 1; i < list.Count; i++)
		{
			CandidateFunctionMember candidateFunctionMember4 = list[i];
			switch (WhichMethodIsBetter(candidateFunctionMember3, candidateFunctionMember4, pTypeThrough, args))
			{
			case BetterType.Left:
				flag = false;
				continue;
			case BetterType.Right:
				flag = false;
				candidateFunctionMember3 = candidateFunctionMember4;
				continue;
			}
			candidateFunctionMember = candidateFunctionMember3;
			candidateFunctionMember2 = candidateFunctionMember4;
			i++;
			if (i < list.Count)
			{
				candidateFunctionMember4 = list[i];
				candidateFunctionMember3 = candidateFunctionMember4;
			}
			else
			{
				flag = true;
			}
		}
		if (!flag)
		{
			foreach (CandidateFunctionMember item in list)
			{
				if (item == candidateFunctionMember3)
				{
					methAmbig1 = null;
					methAmbig2 = null;
					return candidateFunctionMember3;
				}
				switch (WhichMethodIsBetter(item, candidateFunctionMember3, pTypeThrough, args))
				{
				case BetterType.Same:
				case BetterType.Neither:
					candidateFunctionMember = candidateFunctionMember3;
					candidateFunctionMember2 = item;
					break;
				case BetterType.Right:
					continue;
				case BetterType.Left:
					break;
				}
				break;
			}
		}
		if ((candidateFunctionMember != null) & (candidateFunctionMember2 != null))
		{
			methAmbig1 = candidateFunctionMember;
			methAmbig2 = candidateFunctionMember2;
		}
		else
		{
			methAmbig1 = list[0];
			methAmbig2 = list[1];
		}
		return null;
	}

	private static void RoundToFloat(double d, out float f)
	{
		f = (float)d;
	}

	private static long I64(long x)
	{
		return x;
	}

	private static ConvKind GetConvKind(PredefinedType ptSrc, PredefinedType ptDst)
	{
		if ((int)ptSrc < 13 && (int)ptDst < 13)
		{
			return (ConvKind)(s_simpleTypeConversions[(uint)ptSrc][(uint)ptDst] & 0xF);
		}
		if (ptSrc == ptDst || (ptDst == PredefinedType.PT_OBJECT && ptSrc < PredefinedType.PT_COUNT))
		{
			return ConvKind.Implicit;
		}
		if (ptSrc == PredefinedType.PT_OBJECT && ptDst < PredefinedType.PT_COUNT)
		{
			return ConvKind.Explicit;
		}
		return ConvKind.Unknown;
	}

	private static bool isUserDefinedConversion(PredefinedType ptSrc, PredefinedType ptDst)
	{
		if ((int)ptSrc < 13 && (int)ptDst < 13)
		{
			return (s_simpleTypeConversions[(uint)ptSrc][(uint)ptDst] & 0x40) != 0;
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private BetterType WhichSimpleConversionIsBetter(PredefinedType pt1, PredefinedType pt2)
	{
		return (BetterType)s_simpleTypeBetter[(uint)pt1][(uint)pt2];
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private BetterType WhichTypeIsBetter(PredefinedType pt1, PredefinedType pt2, CType typeGiven)
	{
		if (pt1 == pt2)
		{
			return BetterType.Same;
		}
		if (typeGiven.IsPredefType(pt1))
		{
			return BetterType.Left;
		}
		if (typeGiven.IsPredefType(pt2))
		{
			return BetterType.Right;
		}
		if ((int)pt1 < 16 && (int)pt2 < 16)
		{
			return WhichSimpleConversionIsBetter(pt1, pt2);
		}
		if (pt2 == PredefinedType.PT_OBJECT && pt1 < PredefinedType.PT_COUNT)
		{
			return BetterType.Left;
		}
		if (pt1 == PredefinedType.PT_OBJECT && pt2 < PredefinedType.PT_COUNT)
		{
			return BetterType.Right;
		}
		return WhichTypeIsBetter(GetPredefindType(pt1), GetPredefindType(pt2), typeGiven);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private BetterType WhichTypeIsBetter(CType type1, CType type2, CType typeGiven)
	{
		if (type1 == type2)
		{
			return BetterType.Same;
		}
		if (typeGiven == type1)
		{
			return BetterType.Left;
		}
		if (typeGiven == type2)
		{
			return BetterType.Right;
		}
		bool flag = canConvert(type1, type2);
		bool flag2 = canConvert(type2, type1);
		if (flag != flag2)
		{
			if (!flag)
			{
				return BetterType.Right;
			}
			return BetterType.Left;
		}
		if (!(type1 is NullableType nullableType) || !(type2 is NullableType nullableType2) || !nullableType.UnderlyingType.IsPredefined || !nullableType2.UnderlyingType.IsPredefined)
		{
			return BetterType.Neither;
		}
		PredefinedType predefinedType = (type1 as NullableType).UnderlyingType.PredefinedType;
		PredefinedType predefinedType2 = (type2 as NullableType).UnderlyingType.PredefinedType;
		if ((int)predefinedType < 16 && (int)predefinedType2 < 16)
		{
			return WhichSimpleConversionIsBetter(predefinedType, predefinedType2);
		}
		return BetterType.Neither;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool canConvert(CType src, CType dest, CONVERTTYPE flags)
	{
		return BindImplicitConversion(null, src, dest, flags);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public bool canConvert(CType src, CType dest)
	{
		return canConvert(src, dest, (CONVERTTYPE)0);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool canConvert(Expr expr, CType dest)
	{
		return canConvert(expr, dest, (CONVERTTYPE)0);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool canConvert(Expr expr, CType dest, CONVERTTYPE flags)
	{
		return BindImplicitConversion(expr, expr.Type, dest, flags);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr mustConvertCore(Expr expr, CType destExpr)
	{
		return mustConvertCore(expr, destExpr, (CONVERTTYPE)0);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr mustConvertCore(Expr expr, CType dest, CONVERTTYPE flags)
	{
		if (BindImplicitConversion(expr, expr.Type, dest, out var ppDestinationExpr, flags))
		{
			CheckUnsafe(expr.Type);
			CheckUnsafe(dest);
			return ppDestinationExpr;
		}
		FUNDTYPE fundamentalType = expr.Type.FundamentalType;
		FUNDTYPE fundamentalType2 = dest.FundamentalType;
		if (expr is ExprConstant exprConstant && expr.Type.IsSimpleType && dest.IsSimpleType && ((fundamentalType == FUNDTYPE.FT_I4 && (fundamentalType2 <= FUNDTYPE.FT_U4 || fundamentalType2 == FUNDTYPE.FT_U8)) || (fundamentalType == FUNDTYPE.FT_I8 && fundamentalType2 == FUNDTYPE.FT_U8)))
		{
			string text = exprConstant.Int64Value.ToString(CultureInfo.InvariantCulture);
			throw ErrorHandling.Error(ErrorCode.ERR_ConstOutOfRange, text, dest);
		}
		if (expr.Type is NullType && dest.FundamentalType != FUNDTYPE.FT_REF)
		{
			throw ErrorHandling.Error(ErrorCode.ERR_ValueCantBeNull, dest);
		}
		throw ErrorHandling.Error(canCast(expr.Type, dest, flags) ? ErrorCode.ERR_NoImplicitConvCast : ErrorCode.ERR_NoImplicitConv, new ErrArg(expr.Type, ErrArgFlags.Unique), new ErrArg(dest, ErrArgFlags.Unique));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public Expr tryConvert(Expr expr, CType dest)
	{
		return tryConvert(expr, dest, (CONVERTTYPE)0);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr tryConvert(Expr expr, CType dest, CONVERTTYPE flags)
	{
		if (BindImplicitConversion(expr, expr.Type, dest, out var ppDestinationExpr, flags))
		{
			CheckUnsafe(expr.Type);
			CheckUnsafe(dest);
			return ppDestinationExpr;
		}
		return null;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public Expr mustConvert(Expr expr, CType dest)
	{
		return mustConvert(expr, dest, (CONVERTTYPE)0);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr mustConvert(Expr expr, CType dest, CONVERTTYPE flags)
	{
		return mustConvertCore(expr, dest, flags);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr mustCastCore(Expr expr, CType dest, CONVERTTYPE flags)
	{
		CSemanticChecker.CheckForStaticClass(dest);
		if (BindExplicitConversion(expr, expr.Type, dest, out var ppDestinationExpr, flags))
		{
			CheckUnsafe(expr.Type);
			CheckUnsafe(dest);
			return ppDestinationExpr;
		}
		Expr expr2 = expr.GetConst();
		if (expr2 != null && expr.Type.IsSimpleOrEnum && dest.IsSimpleOrEnum)
		{
			FUNDTYPE fundamentalType = expr.Type.FundamentalType;
			if (fundamentalType == FUNDTYPE.FT_STRUCT)
			{
				throw ErrorHandling.Error(ErrorCode.ERR_ConstOutOfRange, ((ExprConstant)expr2).Val.DecimalVal.ToString(CultureInfo.InvariantCulture), dest);
			}
			if (Context.Checked)
			{
				if (!CanExplicitConversionBeBoundInUncheckedContext(expr, expr.Type, dest, flags | CONVERTTYPE.NOUDC))
				{
					throw CantConvert(expr, dest);
				}
				string text;
				switch (fundamentalType)
				{
				case FUNDTYPE.FT_U1:
				case FUNDTYPE.FT_U2:
				case FUNDTYPE.FT_U4:
				case FUNDTYPE.FT_U8:
					text = ((ulong)((ExprConstant)expr2).Int64Value).ToString(CultureInfo.InvariantCulture);
					break;
				case FUNDTYPE.FT_I1:
				case FUNDTYPE.FT_I2:
				case FUNDTYPE.FT_I4:
				case FUNDTYPE.FT_I8:
					text = ((ExprConstant)expr2).Int64Value.ToString(CultureInfo.InvariantCulture);
					break;
				default:
					text = ((ExprConstant)expr2).Val.DoubleVal.ToString(CultureInfo.InvariantCulture);
					break;
				}
				throw ErrorHandling.Error(ErrorCode.ERR_ConstOutOfRangeChecked, text, dest);
			}
		}
		if (expr.Type is NullType && dest.FundamentalType != FUNDTYPE.FT_REF)
		{
			throw ErrorHandling.Error(ErrorCode.ERR_ValueCantBeNull, dest);
		}
		throw CantConvert(expr, dest);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static RuntimeBinderException CantConvert(Expr expr, CType dest)
	{
		return ErrorHandling.Error(ErrorCode.ERR_NoExplicitConv, new ErrArg(expr.Type, ErrArgFlags.Unique), new ErrArg(dest, ErrArgFlags.Unique));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public Expr mustCast(Expr expr, CType dest)
	{
		return mustCast(expr, dest, (CONVERTTYPE)0);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public Expr mustCast(Expr expr, CType dest, CONVERTTYPE flags)
	{
		return mustCastCore(expr, dest, flags);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr MustCastInUncheckedContext(Expr expr, CType dest, CONVERTTYPE flags)
	{
		return new ExpressionBinder(new BindingContext(Context)).mustCast(expr, dest, flags);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool canCast(CType src, CType dest, CONVERTTYPE flags)
	{
		return BindExplicitConversion(null, src, dest, flags);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool BindImplicitConversion(Expr pSourceExpr, CType pSourceType, CType destinationType, CONVERTTYPE flags)
	{
		return new ImplicitConversion(this, pSourceExpr, pSourceType, destinationType, needsExprDest: false, flags).Bind();
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool BindImplicitConversion(Expr pSourceExpr, CType pSourceType, CType destinationType, out Expr ppDestinationExpr, CONVERTTYPE flags)
	{
		ImplicitConversion implicitConversion = new ImplicitConversion(this, pSourceExpr, pSourceType, destinationType, needsExprDest: true, flags);
		bool result = implicitConversion.Bind();
		ppDestinationExpr = implicitConversion.ExprDest;
		return result;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool BindImplicitConversion(Expr pSourceExpr, CType pSourceType, CType destinationType, bool needsExprDest, out Expr ppDestinationExpr, CONVERTTYPE flags)
	{
		ImplicitConversion implicitConversion = new ImplicitConversion(this, pSourceExpr, pSourceType, destinationType, needsExprDest, flags);
		bool result = implicitConversion.Bind();
		ppDestinationExpr = (needsExprDest ? implicitConversion.ExprDest : null);
		return result;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool BindExplicitConversion(Expr pSourceExpr, CType pSourceType, CType destinationType, bool needsExprDest, out Expr ppDestinationExpr, CONVERTTYPE flags)
	{
		ExplicitConversion explicitConversion = new ExplicitConversion(this, pSourceExpr, pSourceType, destinationType, needsExprDest, flags);
		bool result = explicitConversion.Bind();
		ppDestinationExpr = (needsExprDest ? explicitConversion.ExprDest : null);
		return result;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool BindExplicitConversion(Expr pSourceExpr, CType pSourceType, CType destinationType, out Expr ppDestinationExpr, CONVERTTYPE flags)
	{
		ExplicitConversion explicitConversion = new ExplicitConversion(this, pSourceExpr, pSourceType, destinationType, needsExprDest: true, flags);
		bool result = explicitConversion.Bind();
		ppDestinationExpr = explicitConversion.ExprDest;
		return result;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool BindExplicitConversion(Expr pSourceExpr, CType pSourceType, CType destinationType, CONVERTTYPE flags)
	{
		return new ExplicitConversion(this, pSourceExpr, pSourceType, destinationType, needsExprDest: false, flags).Bind();
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool bindUserDefinedConversion(Expr exprSrc, CType typeSrc, CType typeDst, bool needExprDest, out Expr pexprDst, bool fImplicitOnly)
	{
		pexprDst = null;
		if (typeSrc == null || typeDst == null || typeSrc.IsInterfaceType || typeDst.IsInterfaceType)
		{
			return false;
		}
		CType cType = typeSrc.StripNubs();
		CType cType2 = typeDst.StripNubs();
		bool flag = cType != typeSrc;
		bool flag2 = cType2 != typeDst;
		bool flag3 = flag2 || typeDst.IsReferenceType || typeDst is PointerType;
		AggregateType[] array = new AggregateType[2];
		int num = 0;
		bool flag4 = fImplicitOnly;
		bool flag5 = false;
		if (cType is AggregateType aggregateType && aggregateType.OwningAggregate.HasConversion())
		{
			array[num++] = aggregateType;
			flag5 = aggregateType.IsPredefType(PredefinedType.FirstNonSimpleType) || aggregateType.IsPredefType(PredefinedType.PT_UINTPTR);
		}
		if (cType2 is AggregateType aggregateType2)
		{
			if (aggregateType2.OwningAggregate.HasConversion())
			{
				array[num++] = aggregateType2;
			}
			if (flag5 && !cType2.IsPredefType(PredefinedType.PT_LONG) && !cType2.IsPredefType(PredefinedType.PT_ULONG))
			{
				flag5 = false;
			}
		}
		else
		{
			flag5 = false;
		}
		if (num == 0)
		{
			return false;
		}
		List<UdConvInfo> list = new List<UdConvInfo>();
		CType cType3 = null;
		CType cType4 = null;
		bool flag6 = false;
		bool flag7 = false;
		int num2 = -1;
		int num3 = -1;
		CType cType5;
		CType cType6;
		for (int i = 0; i < num; i++)
		{
			AggregateType aggregateType3 = array[i];
			while (aggregateType3 != null && aggregateType3.OwningAggregate.HasConversion())
			{
				AggregateSymbol owningAggregate = aggregateType3.OwningAggregate;
				PredefinedType predefType = owningAggregate.GetPredefType();
				bool flag8 = owningAggregate.IsPredefined() && (predefType == PredefinedType.FirstNonSimpleType || predefType == PredefinedType.PT_UINTPTR || predefType == PredefinedType.PT_DECIMAL);
				for (MethodSymbol methodSymbol = owningAggregate.GetFirstUDConversion(); methodSymbol != null; methodSymbol = methodSymbol.ConvNext())
				{
					if (methodSymbol.Params.Count != 1 || (fImplicitOnly && !methodSymbol.isImplicit()))
					{
						continue;
					}
					cType5 = TypeManager.SubstType(methodSymbol.Params[0], aggregateType3);
					cType6 = TypeManager.SubstType(methodSymbol.RetType, aggregateType3);
					bool flag9 = fImplicitOnly;
					if (flag4 && !flag9 && cType5.StripNubs() != cType)
					{
						if (!methodSymbol.isImplicit())
						{
							continue;
						}
						flag9 = true;
					}
					FUNDTYPE fundamentalType;
					FUNDTYPE fundamentalType2;
					if (((fundamentalType = cType6.FundamentalType) <= FUNDTYPE.FT_R8 && fundamentalType > FUNDTYPE.FT_NONE && (fundamentalType2 = cType5.FundamentalType) <= FUNDTYPE.FT_R8 && fundamentalType2 > FUNDTYPE.FT_NONE) || (flag5 && (cType6.IsPredefType(PredefinedType.PT_INT) || cType6.IsPredefType(PredefinedType.PT_UINT))))
					{
						continue;
					}
					if (flag && (flag3 || !flag9) && cType5.IsNonNullableValueType)
					{
						cType5 = TypeManager.GetNullable(cType5);
					}
					if (flag2 && cType6.IsNonNullableValueType)
					{
						cType6 = TypeManager.GetNullable(cType6);
					}
					bool flag10 = ((exprSrc != null) ? canConvert(exprSrc, cType5, CONVERTTYPE.STANDARDANDNOUDC) : canConvert(typeSrc, cType5, CONVERTTYPE.STANDARDANDNOUDC));
					if (!flag10 && (flag9 || (!canConvert(cType5, typeSrc, CONVERTTYPE.STANDARDANDNOUDC) && (!flag8 || typeSrc is PointerType || cType5 is PointerType || !canCast(typeSrc, cType5, CONVERTTYPE.NOUDC)))))
					{
						continue;
					}
					bool flag11 = canConvert(cType6, typeDst, CONVERTTYPE.STANDARDANDNOUDC);
					if ((!flag11 && (flag9 || (!canConvert(typeDst, cType6, CONVERTTYPE.STANDARDANDNOUDC) && (!flag8 || typeDst is PointerType || cType6 is PointerType || !canCast(cType6, typeDst, CONVERTTYPE.NOUDC))))) || IsConvInTable(list, methodSymbol, aggregateType3, flag10, flag11))
					{
						continue;
					}
					list.Add(new UdConvInfo(new MethWithType(methodSymbol, aggregateType3), flag10, flag11));
					if (!flag6)
					{
						if (cType5 == typeSrc)
						{
							cType3 = cType5;
							num2 = list.Count - 1;
							flag6 = true;
						}
						else if (cType3 == null)
						{
							cType3 = cType5;
							num2 = list.Count - 1;
						}
						else if (cType3 != cType5 && CompareSrcTypesBased(cType3, list[num2].SrcImplicit, cType5, flag10) > 0)
						{
							cType3 = cType5;
							num2 = list.Count - 1;
						}
					}
					if (!flag7)
					{
						if (cType6 == typeDst)
						{
							cType4 = cType6;
							num3 = list.Count - 1;
							flag7 = true;
						}
						else if (cType4 == null)
						{
							cType4 = cType6;
							num3 = list.Count - 1;
						}
						else if (cType4 != cType6 && CompareDstTypesBased(cType4, list[num3].DstImplicit, cType6, flag11) > 0)
						{
							cType4 = cType6;
							num3 = list.Count - 1;
						}
					}
				}
				aggregateType3 = aggregateType3.BaseClass;
			}
		}
		if (cType3 == null)
		{
			return false;
		}
		int num4 = 3;
		int num5 = -1;
		int num6 = -1;
		for (int j = 0; j < list.Count; j++)
		{
			UdConvInfo udConvInfo = list[j];
			cType5 = TypeManager.SubstType(udConvInfo.Meth.Meth().Params[0], udConvInfo.Meth.GetType());
			cType6 = TypeManager.SubstType(udConvInfo.Meth.Meth().RetType, udConvInfo.Meth.GetType());
			int num7 = 0;
			if (flag && cType5.IsNonNullableValueType)
			{
				cType5 = TypeManager.GetNullable(cType5);
				num7++;
			}
			if (flag2 && cType6.IsNonNullableValueType)
			{
				cType6 = TypeManager.GetNullable(cType6);
				num7++;
			}
			if (cType5 == cType3 && cType6 == cType4)
			{
				if (num4 > num7)
				{
					num5 = j;
					num6 = -1;
					num4 = num7;
				}
				else if (num4 >= num7 && num6 < 0)
				{
					num6 = j;
					if (num7 == 0)
					{
						break;
					}
				}
				continue;
			}
			if (!flag6 && cType5 != cType3 && CompareSrcTypesBased(cType3, list[num2].SrcImplicit, cType5, udConvInfo.SrcImplicit) >= 0)
			{
				if (needExprDest)
				{
					throw HandleAmbiguity(typeSrc, typeDst, list, num2, j);
				}
				return true;
			}
			if (!flag7 && cType6 != cType4 && CompareDstTypesBased(cType4, list[num3].DstImplicit, cType6, udConvInfo.DstImplicit) >= 0)
			{
				if (needExprDest)
				{
					throw HandleAmbiguity(typeSrc, typeDst, list, num2, j);
				}
				return true;
			}
		}
		if (!needExprDest)
		{
			return true;
		}
		if (num5 < 0)
		{
			throw HandleAmbiguity(typeSrc, typeDst, list, num2, num3);
		}
		if (num6 >= 0)
		{
			throw HandleAmbiguity(typeSrc, typeDst, list, num5, num6);
		}
		MethWithInst methWithInst = new MethWithInst(list[num5].Meth.Meth(), list[num5].Meth.GetType(), null);
		cType5 = TypeManager.SubstType(methWithInst.Meth().Params[0], methWithInst.GetType());
		cType6 = TypeManager.SubstType(methWithInst.Meth().RetType, methWithInst.GetType());
		Expr ppTransformedArgument = exprSrc;
		Expr call;
		if ((num4 > 0 && !(cType5 is NullableType)) & flag3)
		{
			ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, methWithInst);
			ExprCall exprCall = ExprFactory.CreateCall((EXPRFLAG)0, typeDst, exprSrc, memberGroup, methWithInst);
			call = exprCall;
			Expr expr = mustCast(exprSrc, cType5);
			MarkAsIntermediateConversion(expr);
			Expr expr2 = BindUDConversionCore(expr, cType5, cType6, typeDst, methWithInst);
			exprCall.CastOfNonLiftedResultToLiftedType = mustCast(expr2, typeDst);
			exprCall.NullableCallLiftKind = NullableCallLiftKind.UserDefinedConversion;
			if (flag)
			{
				Expr expr3;
				if (cType5 == cType)
				{
					expr3 = ((!(cType6 is NullableType)) ? exprSrc : mustCast(exprSrc, cType5));
				}
				else
				{
					NullableType nullable = TypeManager.GetNullable(cType5);
					expr3 = mustCast(exprSrc, nullable);
					MarkAsIntermediateConversion(expr3);
				}
				ExprCall exprCall2 = ExprFactory.CreateCall((EXPRFLAG)0, typeDst, expr3, memberGroup, methWithInst);
				exprCall2.NullableCallLiftKind = NullableCallLiftKind.NotLiftedIntermediateConversion;
				exprCall.PConversions = exprCall2;
			}
			else
			{
				Expr expr4 = BindUDConversionCore(expr, cType5, cType6, typeDst, methWithInst);
				MarkAsIntermediateConversion(expr4);
				exprCall.PConversions = expr4;
			}
		}
		else
		{
			call = BindUDConversionCore(exprSrc, cType5, cType6, typeDst, methWithInst, out ppTransformedArgument);
		}
		pexprDst = ExprFactory.CreateUserDefinedConversion(ppTransformedArgument, call, methWithInst);
		return true;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static RuntimeBinderException HandleAmbiguity(CType typeSrc, CType typeDst, List<UdConvInfo> prguci, int iuciBestSrc, int iuciBestDst)
	{
		return ErrorHandling.Error(ErrorCode.ERR_AmbigUDConv, prguci[iuciBestSrc].Meth, prguci[iuciBestDst].Meth, typeSrc, typeDst);
	}

	private static void MarkAsIntermediateConversion(Expr pExpr)
	{
		while (true)
		{
			if (pExpr is ExprCall { NullableCallLiftKind: var nullableCallLiftKind } exprCall)
			{
				switch (nullableCallLiftKind)
				{
				default:
					return;
				case NullableCallLiftKind.NotLifted:
					exprCall.NullableCallLiftKind = NullableCallLiftKind.NotLiftedIntermediateConversion;
					return;
				case NullableCallLiftKind.NullableConversion:
					exprCall.NullableCallLiftKind = NullableCallLiftKind.NullableIntermediateConversion;
					return;
				case NullableCallLiftKind.NullableConversionConstructor:
					pExpr = exprCall.OptionalArguments;
					break;
				}
			}
			else
			{
				if (!(pExpr is ExprUserDefinedConversion exprUserDefinedConversion))
				{
					break;
				}
				pExpr = exprUserDefinedConversion.UserDefinedCall;
			}
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr BindUDConversionCore(Expr pFrom, CType pTypeFrom, CType pTypeTo, CType pTypeDestination, MethWithInst mwiBest)
	{
		Expr ppTransformedArgument;
		return BindUDConversionCore(pFrom, pTypeFrom, pTypeTo, pTypeDestination, mwiBest, out ppTransformedArgument);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr BindUDConversionCore(Expr pFrom, CType pTypeFrom, CType pTypeTo, CType pTypeDestination, MethWithInst mwiBest, out Expr ppTransformedArgument)
	{
		Expr expr = mustCastCore(pFrom, pTypeFrom, CONVERTTYPE.NOUDC);
		ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, mwiBest);
		ExprCall expr2 = ExprFactory.CreateCall((EXPRFLAG)0, pTypeTo, expr, memberGroup, mwiBest);
		Expr result = mustCastCore(expr2, pTypeDestination, CONVERTTYPE.NOUDC);
		ppTransformedArgument = expr;
		return result;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ConstCastResult bindConstantCast(Expr exprSrc, CType typeDest, bool needExprDest, out Expr pexprDest, bool explicitConversion)
	{
		pexprDest = null;
		long num = 0L;
		double num2 = 0.0;
		FUNDTYPE fundamentalType = exprSrc.Type.FundamentalType;
		FUNDTYPE fundamentalType2 = typeDest.FundamentalType;
		bool flag = fundamentalType <= FUNDTYPE.FT_U8;
		bool flag2 = fundamentalType <= FUNDTYPE.FT_R8;
		ExprConstant exprConstant = (ExprConstant)exprSrc.GetConst();
		if (fundamentalType == FUNDTYPE.FT_STRUCT || fundamentalType2 == FUNDTYPE.FT_STRUCT)
		{
			Expr expr = BindDecimalConstCast(typeDest, exprSrc.Type, exprConstant);
			if (expr == null)
			{
				if (explicitConversion)
				{
					return ConstCastResult.CheckFailure;
				}
				return ConstCastResult.Failure;
			}
			if (needExprDest)
			{
				pexprDest = expr;
			}
			return ConstCastResult.Success;
		}
		if (explicitConversion && Context.Checked && !isConstantInRange(exprConstant, typeDest, realsOk: true))
		{
			return ConstCastResult.CheckFailure;
		}
		if (!needExprDest)
		{
			return ConstCastResult.Success;
		}
		if (flag)
		{
			if (exprConstant.Type.FundamentalType == FUNDTYPE.FT_U8)
			{
				if (fundamentalType2 == FUNDTYPE.FT_U8)
				{
					ConstVal constVal = ConstVal.Get(exprConstant.UInt64Value);
					pexprDest = ExprFactory.CreateConstant(typeDest, constVal);
					return ConstCastResult.Success;
				}
				num = (long)exprConstant.UInt64Value & -1L;
			}
			else
			{
				num = exprConstant.Int64Value;
			}
		}
		else
		{
			if (!flag2)
			{
				return ConstCastResult.Failure;
			}
			num2 = exprConstant.Val.DoubleVal;
		}
		switch (fundamentalType2)
		{
		case FUNDTYPE.FT_I1:
			if (!flag)
			{
				num = (long)num2;
			}
			num = (sbyte)(num & 0xFF);
			break;
		case FUNDTYPE.FT_I2:
			if (!flag)
			{
				num = (long)num2;
			}
			num = (short)(num & 0xFFFF);
			break;
		case FUNDTYPE.FT_I4:
			if (!flag)
			{
				num = (long)num2;
			}
			num = (int)(num & 0xFFFFFFFFu);
			break;
		case FUNDTYPE.FT_I8:
			if (!flag)
			{
				num = (long)num2;
			}
			break;
		case FUNDTYPE.FT_U1:
			if (!flag)
			{
				num = (long)num2;
			}
			num &= 0xFF;
			break;
		case FUNDTYPE.FT_U2:
			if (!flag)
			{
				num = (long)num2;
			}
			num &= 0xFFFF;
			break;
		case FUNDTYPE.FT_U4:
			if (!flag)
			{
				num = (long)num2;
			}
			num &= 0xFFFFFFFFu;
			break;
		case FUNDTYPE.FT_U8:
			if (!flag)
			{
				num = ((!(num2 < 9.223372036854776E+18)) ? ((long)(num2 - 9.223372036854776E+18) + long.MinValue) : ((long)num2));
			}
			break;
		case FUNDTYPE.FT_R4:
		case FUNDTYPE.FT_R8:
			if (flag)
			{
				num2 = ((fundamentalType != FUNDTYPE.FT_U8) ? ((double)num) : ((double)(ulong)num));
			}
			if (fundamentalType2 == FUNDTYPE.FT_R4)
			{
				RoundToFloat(num2, out var f);
				num2 = f;
			}
			break;
		}
		ConstVal constVal2 = ((fundamentalType2 == FUNDTYPE.FT_U4) ? ConstVal.Get((uint)num) : ((fundamentalType2 <= FUNDTYPE.FT_U4) ? ConstVal.Get((int)num) : ((fundamentalType2 > FUNDTYPE.FT_U8) ? ConstVal.Get(num2) : ConstVal.Get(num))));
		ExprConstant exprConstant2 = ExprFactory.CreateConstant(typeDest, constVal2);
		pexprDest = exprConstant2;
		return ConstCastResult.Success;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private int CompareSrcTypesBased(CType type1, bool fImplicit1, CType type2, bool fImplicit2)
	{
		if (fImplicit1 != fImplicit2)
		{
			if (!fImplicit1)
			{
				return 1;
			}
			return -1;
		}
		bool flag = canConvert(type1, type2, CONVERTTYPE.NOUDC);
		bool flag2 = canConvert(type2, type1, CONVERTTYPE.NOUDC);
		if (flag == flag2)
		{
			return 0;
		}
		if (fImplicit1 != flag)
		{
			return 1;
		}
		return -1;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private int CompareDstTypesBased(CType type1, bool fImplicit1, CType type2, bool fImplicit2)
	{
		if (fImplicit1 != fImplicit2)
		{
			if (!fImplicit1)
			{
				return 1;
			}
			return -1;
		}
		bool flag = canConvert(type1, type2, CONVERTTYPE.NOUDC);
		bool flag2 = canConvert(type2, type1, CONVERTTYPE.NOUDC);
		if (flag == flag2)
		{
			return 0;
		}
		if (fImplicit1 != flag)
		{
			return -1;
		}
		return 1;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static Expr BindDecimalConstCast(CType destType, CType srcType, ExprConstant src)
	{
		CType predefindType = SymbolLoader.GetPredefindType(PredefinedType.PT_DECIMAL);
		if (predefindType == null)
		{
			return null;
		}
		if (destType == predefindType)
		{
			decimal value;
			switch (srcType.FundamentalType)
			{
			case FUNDTYPE.FT_I1:
			case FUNDTYPE.FT_I2:
			case FUNDTYPE.FT_I4:
				value = Convert.ToDecimal(src.Val.Int32Val);
				break;
			case FUNDTYPE.FT_U1:
			case FUNDTYPE.FT_U2:
			case FUNDTYPE.FT_U4:
				value = Convert.ToDecimal(src.Val.UInt32Val);
				break;
			case FUNDTYPE.FT_R4:
				value = Convert.ToDecimal((float)src.Val.DoubleVal);
				break;
			case FUNDTYPE.FT_R8:
				value = Convert.ToDecimal(src.Val.DoubleVal);
				break;
			case FUNDTYPE.FT_U8:
				value = Convert.ToDecimal((ulong)src.Val.Int64Val);
				break;
			case FUNDTYPE.FT_I8:
				value = Convert.ToDecimal(src.Val.Int64Val);
				break;
			default:
				return null;
			}
			ConstVal constVal = ConstVal.Get(value);
			return ExprFactory.CreateConstant(predefindType, constVal);
		}
		if (srcType == predefindType)
		{
			decimal value2 = 0m;
			FUNDTYPE fundamentalType = destType.FundamentalType;
			ConstVal constVal;
			try
			{
				if (fundamentalType != FUNDTYPE.FT_R4 && fundamentalType != FUNDTYPE.FT_R8)
				{
					value2 = decimal.Truncate(src.Val.DecimalVal);
				}
				switch (fundamentalType)
				{
				case FUNDTYPE.FT_I1:
					constVal = ConstVal.Get(Convert.ToSByte(value2));
					break;
				case FUNDTYPE.FT_U1:
					constVal = ConstVal.Get((uint)Convert.ToByte(value2));
					break;
				case FUNDTYPE.FT_I2:
					constVal = ConstVal.Get(Convert.ToInt16(value2));
					break;
				case FUNDTYPE.FT_U2:
					constVal = ConstVal.Get((uint)Convert.ToUInt16(value2));
					break;
				case FUNDTYPE.FT_I4:
					constVal = ConstVal.Get(Convert.ToInt32(value2));
					break;
				case FUNDTYPE.FT_U4:
					constVal = ConstVal.Get(Convert.ToUInt32(value2));
					break;
				case FUNDTYPE.FT_I8:
					constVal = ConstVal.Get(Convert.ToInt64(value2));
					break;
				case FUNDTYPE.FT_U8:
					constVal = ConstVal.Get(Convert.ToUInt64(value2));
					break;
				case FUNDTYPE.FT_R4:
					constVal = ConstVal.Get(Convert.ToSingle(src.Val.DecimalVal));
					break;
				case FUNDTYPE.FT_R8:
					constVal = ConstVal.Get(Convert.ToDouble(src.Val.DecimalVal));
					break;
				default:
					return null;
				}
			}
			catch (OverflowException)
			{
				return null;
			}
			return ExprFactory.CreateConstant(destType, constVal);
		}
		return null;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool CanExplicitConversionBeBoundInUncheckedContext(Expr exprSrc, CType typeSrc, CType typeDest, CONVERTTYPE flags)
	{
		return new ExpressionBinder(new BindingContext(Context)).BindExplicitConversion(exprSrc, typeSrc, typeDest, flags);
	}

	public ExpressionBinder(BindingContext context)
	{
		Context = context;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static AggregateType GetPredefindType(PredefinedType pt)
	{
		return SymbolLoader.GetPredefindType(pt);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr GenerateAssignmentConversion(Expr op1, Expr op2, bool allowExplicit)
	{
		if (!allowExplicit)
		{
			return mustConvertCore(op2, op1.Type);
		}
		return mustCastCore(op2, op1.Type, (CONVERTTYPE)0);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public Expr BindAssignment(Expr op1, Expr op2, bool allowExplicit)
	{
		CheckLvalue(op1, CheckLvalueKind.Assignment);
		op2 = GenerateAssignmentConversion(op1, op2, allowExplicit);
		return GenerateOptimizedAssignment(op1, op2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal Expr BindArrayIndexCore(Expr pOp1, Expr pOp2)
	{
		CType pIntType = GetPredefindType(PredefinedType.PT_INT);
		CType elementType = (pOp1.Type as ArrayType).ElementType;
		CheckUnsafe(elementType);
		CType pDestType = ChooseArrayIndexType(pOp2);
		ExpressionBinder binder = this;
		Expr index = pOp2.Map(delegate(Expr x)
		{
			Expr expr = binder.MustConvertWithSuppressedMessage(x, pDestType);
			return (pDestType != pIntType) ? ExprFactoryCreateCastWithSuppressedMessage(EXPRFLAG.EXF_LITERALCONST, pDestType, expr) : expr;
		});
		return ExprFactory.CreateArrayIndex(elementType, pOp1, index);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Workarounds https://github.com/mono/linker/issues/1416. All usages are marked as unsafe.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr MustConvertWithSuppressedMessage(Expr x, CType pDestType)
	{
		return mustConvert(x, pDestType);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Workarounds https://github.com/mono/linker/issues/1416. All usages are marked as unsafe.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprCast ExprFactoryCreateCastWithSuppressedMessage(EXPRFLAG flags, CType type, Expr argument)
	{
		return ExprFactory.CreateCast(flags, type, argument);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void bindSimpleCast(Expr exprSrc, CType typeDest, out Expr pexprDest)
	{
		bindSimpleCast(exprSrc, typeDest, out pexprDest, (EXPRFLAG)0);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void bindSimpleCast(Expr exprSrc, CType typeDest, out Expr pexprDest, EXPRFLAG exprFlags)
	{
		Expr expr = exprSrc.GetConst();
		ExprCast exprCast = ExprFactory.CreateCast(exprFlags, typeDest, exprSrc);
		if (Context.Checked)
		{
			exprCast.Flags |= EXPRFLAG.EXF_CHECKOVERFLOW;
		}
		if (expr is ExprConstant exprConstant && exprFlags == (EXPRFLAG)0 && exprSrc.Type.FundamentalType == typeDest.FundamentalType && (!exprSrc.Type.IsPredefType(PredefinedType.PT_STRING) || exprConstant.Val.IsNullRef))
		{
			ExprConstant exprConstant2 = ExprFactory.CreateConstant(typeDest, exprConstant.Val);
			pexprDest = exprConstant2;
		}
		else
		{
			pexprDest = exprCast;
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprCall BindToMethod(MethWithInst mwi, Expr pArguments, ExprMemberGroup pMemGroup, MemLookFlags flags)
	{
		Expr optionalObject = pMemGroup.OptionalObject;
		CType callingObjectType = optionalObject?.Type;
		PostBindMethod(mwi);
		optionalObject = AdjustMemberObject(mwi, optionalObject);
		pMemGroup.OptionalObject = optionalObject;
		CType type = (((flags & (MemLookFlags.Ctor | MemLookFlags.NewObj)) != (MemLookFlags.Ctor | MemLookFlags.NewObj)) ? TypeManager.SubstType(mwi.Meth().RetType, mwi.GetType(), mwi.TypeArgs) : mwi.Ats);
		ExprCall exprCall = ExprFactory.CreateCall((EXPRFLAG)0, type, pArguments, pMemGroup, mwi);
		if ((flags & MemLookFlags.Ctor) != MemLookFlags.None && (flags & MemLookFlags.NewObj) != MemLookFlags.None)
		{
			exprCall.Flags |= EXPRFLAG.EXF_LITERALCONST | EXPRFLAG.EXF_CANTBENULL;
		}
		verifyMethodArgs(exprCall, callingObjectType);
		return exprCall;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal Expr BindToField(Expr pOptionalObject, FieldWithType fwt, BindingFlag bindFlags)
	{
		CType type = TypeManager.SubstType(fwt.Field().GetType(), fwt.GetType());
		pOptionalObject = AdjustMemberObject(fwt, pOptionalObject);
		CheckUnsafe(type);
		ExprField exprField = ExprFactory.CreateField(type, pOptionalObject, fwt);
		exprField.Flags |= (EXPRFLAG)(bindFlags & BindingFlag.BIND_MEMBERSET);
		return exprField;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal ExprProperty BindToProperty(Expr pObject, PropWithType pwt, BindingFlag bindFlags, Expr args, ExprMemberGroup pMemGroup)
	{
		Expr expr = pObject;
		PostBindProperty(pwt, out var pmwtGet, out var pmwtSet);
		pObject = (((bool)pmwtGet && (!pmwtSet || pmwtSet.GetType() == pmwtGet.GetType() || SymbolLoader.HasBaseConversion(pmwtGet.GetType(), pmwtSet.GetType()))) ? AdjustMemberObject(pmwtGet, pObject) : ((!pmwtSet) ? AdjustMemberObject(pwt, pObject) : AdjustMemberObject(pmwtSet, pObject)));
		pMemGroup.OptionalObject = pObject;
		CType type = TypeManager.SubstType(pwt.Prop().RetType, pwt.GetType());
		if ((bindFlags & BindingFlag.BIND_RVALUEREQUIRED) != 0)
		{
			if (!pmwtGet)
			{
				throw ErrorHandling.Error(ErrorCode.ERR_PropertyLacksGet, pwt);
			}
			CType cType = null;
			if (expr != null)
			{
				cType = expr.Type;
			}
			switch (CSemanticChecker.CheckAccess2(pmwtGet.Meth(), pmwtGet.GetType(), ContextForMemberLookup, cType))
			{
			case ACCESSERROR.ACCESSERROR_NOACCESSTHRU:
				throw ErrorHandling.Error(ErrorCode.ERR_BadProtectedAccess, pwt, cType, ContextForMemberLookup);
			default:
				throw ErrorHandling.Error(ErrorCode.ERR_InaccessibleGetter, pwt);
			case ACCESSERROR.ACCESSERROR_NOERROR:
				break;
			}
		}
		ExprProperty exprProperty = ExprFactory.CreateProperty(type, expr, args, pMemGroup, pwt, pmwtSet);
		if (exprProperty.OptionalArguments != null)
		{
			verifyMethodArgs(exprProperty, expr?.Type);
		}
		return exprProperty;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal Expr bindUDUnop(ExpressionKind ek, Expr arg)
	{
		Name name = ExpressionKindName(ek);
		CType cType = arg.Type;
		while (true)
		{
			switch (cType.TypeKind)
			{
			case TypeKind.TK_NullableType:
				break;
			case TypeKind.TK_AggregateType:
			{
				if ((!cType.IsClassType && !cType.IsStructType) || ((AggregateType)cType).OwningAggregate.IsSkipUDOps())
				{
					return null;
				}
				ArgInfos argInfos = new ArgInfos();
				argInfos.carg = 1;
				FillInArgInfoFromArgList(argInfos, arg);
				List<CandidateFunctionMember> list = new List<CandidateFunctionMember>();
				MethodSymbol methodSymbol = null;
				AggregateType aggregateType = (AggregateType)cType;
				while (true)
				{
					methodSymbol = ((methodSymbol == null) ? SymbolLoader.LookupAggMember(name, aggregateType.OwningAggregate, symbmask_t.MASK_MethodSymbol) : methodSymbol.LookupNext(symbmask_t.MASK_MethodSymbol)) as MethodSymbol;
					if (methodSymbol == null)
					{
						if (!list.IsEmpty())
						{
							break;
						}
						aggregateType = aggregateType.BaseClass;
						if (aggregateType == null)
						{
							break;
						}
					}
					else if (methodSymbol.isOperator && methodSymbol.Params.Count == 1)
					{
						TypeArray typeArray = TypeManager.SubstTypeArray(methodSymbol.Params, aggregateType);
						CType cType2 = typeArray[0];
						NullableType nullable;
						if (canConvert(arg, cType2))
						{
							list.Add(new CandidateFunctionMember(new MethPropWithInst(methodSymbol, aggregateType, TypeArray.Empty), typeArray, 0, fExpanded: false));
						}
						else if (cType2.IsNonNullableValueType && TypeManager.SubstType(methodSymbol.RetType, aggregateType).IsNonNullableValueType && canConvert(arg, nullable = TypeManager.GetNullable(cType2)))
						{
							list.Add(new CandidateFunctionMember(new MethPropWithInst(methodSymbol, aggregateType, TypeArray.Empty), TypeArray.Allocate(nullable), 1, fExpanded: false));
						}
					}
				}
				if (list.IsEmpty())
				{
					return null;
				}
				CandidateFunctionMember candidateFunctionMember = FindBestMethod(list, null, argInfos, out var methAmbig, out var methAmbig2);
				if (candidateFunctionMember == null)
				{
					throw ErrorHandling.Error(ErrorCode.ERR_AmbigCall, methAmbig.mpwi, methAmbig2.mpwi);
				}
				ExprCall exprCall = ((candidateFunctionMember.ctypeLift == 0) ? BindUDUnopCall(arg, candidateFunctionMember.@params[0], candidateFunctionMember.mpwi) : BindLiftedUDUnop(arg, candidateFunctionMember.@params[0], candidateFunctionMember.mpwi));
				return ExprFactory.CreateUserDefinedUnaryOperator(ek, exprCall.Type, arg, exprCall, candidateFunctionMember.mpwi);
			}
			default:
				return null;
			}
			cType = cType.StripNubs();
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprCall BindLiftedUDUnop(Expr arg, CType typeArg, MethPropWithInst mpwi)
	{
		CType cType = typeArg.StripNubs();
		if (!(arg.Type is NullableType) || !canConvert(arg.Type.StripNubs(), cType, CONVERTTYPE.NOUDC))
		{
			arg = mustConvert(arg, typeArg);
		}
		CType cType2 = TypeManager.SubstType(mpwi.Meth().RetType, mpwi.GetType());
		if (!(cType2 is NullableType))
		{
			cType2 = TypeManager.GetNullable(cType2);
		}
		Expr arg2 = mustCast(arg, cType);
		ExprCall expr = BindUDUnopCall(arg2, cType, mpwi);
		ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, mpwi);
		ExprCall exprCall = ExprFactory.CreateCall((EXPRFLAG)0, cType2, arg, memberGroup, null);
		exprCall.MethWithInst = new MethWithInst(mpwi);
		exprCall.CastOfNonLiftedResultToLiftedType = mustCast(expr, cType2, (CONVERTTYPE)0);
		exprCall.NullableCallLiftKind = NullableCallLiftKind.Operator;
		return exprCall;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprCall BindUDUnopCall(Expr arg, CType typeArg, MethPropWithInst mpwi)
	{
		CType type = TypeManager.SubstType(mpwi.Meth().RetType, mpwi.GetType());
		CheckUnsafe(type);
		ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, mpwi);
		ExprCall exprCall = ExprFactory.CreateCall((EXPRFLAG)0, type, mustConvert(arg, typeArg), memberGroup, null);
		exprCall.MethWithInst = new MethWithInst(mpwi);
		verifyMethodArgs(exprCall, mpwi.GetType());
		return exprCall;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private GroupToArgsBinderResult BindMethodGroupToArgumentsCore(BindingFlag bindFlags, ExprMemberGroup grp, Expr args, int carg, NamedArgumentsKind namedArgumentsKind)
	{
		ArgInfos argInfos = new ArgInfos
		{
			carg = carg
		};
		FillInArgInfoFromArgList(argInfos, args);
		ArgInfos argInfos2 = new ArgInfos
		{
			carg = carg
		};
		FillInArgInfoFromArgList(argInfos2, args);
		GroupToArgsBinder groupToArgsBinder = new GroupToArgsBinder(this, bindFlags, grp, argInfos, argInfos2, namedArgumentsKind);
		groupToArgsBinder.Bind();
		return groupToArgsBinder.GetResultsOfBind();
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal ExprWithArgs BindMethodGroupToArguments(BindingFlag bindFlags, ExprMemberGroup grp, Expr args)
	{
		int carg = CountArguments(args);
		NamedArgumentsKind namedArgumentsKind = FindNamedArgumentsType(args);
		MethPropWithInst bestResult = BindMethodGroupToArgumentsCore(bindFlags, grp, args, carg, namedArgumentsKind).BestResult;
		if (grp.SymKind == SYMKIND.SK_PropertySymbol)
		{
			return BindToProperty(grp.OptionalObject, new PropWithType(bestResult), bindFlags, args, grp);
		}
		return BindToMethod(new MethWithInst(bestResult), args, grp, (MemLookFlags)grp.Flags);
	}

	private static NamedArgumentsKind FindNamedArgumentsType(Expr args)
	{
		Expr expr = args;
		while (expr != null)
		{
			Expr expr2;
			if (expr is ExprList exprList)
			{
				expr2 = exprList.OptionalElement;
				expr = exprList.OptionalNextListNode;
			}
			else
			{
				expr2 = expr;
				expr = null;
			}
			if (!(expr2 is ExprNamedArgumentSpecification))
			{
				continue;
			}
			while (expr != null)
			{
				if (expr is ExprList exprList2)
				{
					expr2 = exprList2.OptionalElement;
					expr = exprList2.OptionalNextListNode;
				}
				else
				{
					expr2 = expr;
					expr = null;
				}
				if (!(expr2 is ExprNamedArgumentSpecification))
				{
					return NamedArgumentsKind.NonTrailing;
				}
			}
			return NamedArgumentsKind.Positioning;
		}
		return NamedArgumentsKind.None;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static RuntimeBinderException BadOperatorTypesError(Expr pOperand1, Expr pOperand2)
	{
		string errorString = pOperand1.ErrorString;
		if (pOperand2 != null)
		{
			return ErrorHandling.Error(ErrorCode.ERR_BadBinaryOps, errorString, pOperand1.Type, pOperand2.Type);
		}
		return ErrorHandling.Error(ErrorCode.ERR_BadUnaryOp, errorString, pOperand1.Type);
	}

	private static ErrorCode GetStandardLvalueError(CheckLvalueKind kind)
	{
		if (kind != CheckLvalueKind.Increment)
		{
			return ErrorCode.ERR_AssgLvalueExpected;
		}
		return ErrorCode.ERR_IncrementLvalueExpected;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void CheckLvalueProp(ExprProperty prop)
	{
		CType type = null;
		if (prop.OptionalObjectThrough != null)
		{
			type = prop.OptionalObjectThrough.Type;
		}
		CheckPropertyAccess(prop.MethWithTypeSet, prop.PropWithTypeSlot, type);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void CheckPropertyAccess(MethWithType mwt, PropWithType pwtSlot, CType type)
	{
		switch (CSemanticChecker.CheckAccess2(mwt.Meth(), mwt.GetType(), ContextForMemberLookup, type))
		{
		case ACCESSERROR.ACCESSERROR_NOACCESSTHRU:
			throw ErrorHandling.Error(ErrorCode.ERR_BadProtectedAccess, pwtSlot, type, ContextForMemberLookup);
		case ACCESSERROR.ACCESSERROR_NOACCESS:
			throw ErrorHandling.Error(mwt.Meth().isSetAccessor() ? ErrorCode.ERR_InaccessibleSetter : ErrorCode.ERR_InaccessibleGetter, pwtSlot);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void CheckLvalue(Expr expr, CheckLvalueKind kind)
	{
		if (expr.isLvalue())
		{
			if (expr is ExprProperty prop)
			{
				CheckLvalueProp(prop);
			}
			return;
		}
		switch (expr.Kind)
		{
		case ExpressionKind.Property:
		{
			ExprProperty exprProperty = (ExprProperty)expr;
			throw ErrorHandling.Error(ErrorCode.ERR_AssgReadonlyProp, exprProperty.PropWithTypeSlot);
		}
		case ExpressionKind.Field:
			throw ErrorHandling.Error(((ExprField)expr).FieldWithType.Field().isStatic ? ErrorCode.ERR_AssgReadonlyStatic : ErrorCode.ERR_AssgReadonly);
		default:
			throw ErrorHandling.Error(GetStandardLvalueError(kind));
		}
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void PostBindMethod(MethWithInst pMWI)
	{
		MethodSymbol methodSymbol = pMWI.Meth();
		if (methodSymbol.RetType != null)
		{
			CheckUnsafe(methodSymbol.RetType);
			CType[] items = methodSymbol.Params.Items;
			for (int i = 0; i < items.Length; i++)
			{
				CheckUnsafe(items[i]);
			}
		}
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void PostBindProperty(PropWithType pwt, out MethWithType pmwtGet, out MethWithType pmwtSet)
	{
		PropertySymbol propertySymbol = pwt.Prop();
		pmwtGet = ((propertySymbol.GetterMethod != null) ? new MethWithType(propertySymbol.GetterMethod, pwt.GetType()) : new MethWithType());
		pmwtSet = ((propertySymbol.SetterMethod != null) ? new MethWithType(propertySymbol.SetterMethod, pwt.GetType()) : new MethWithType());
		if (propertySymbol.RetType != null)
		{
			CheckUnsafe(propertySymbol.RetType);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr AdjustMemberObject(SymWithType swt, Expr pObject)
	{
		bool num = IsMatchingStatic(swt, pObject);
		bool isStatic = swt.Sym.isStatic;
		if (!num)
		{
			if (isStatic)
			{
				if ((pObject.Flags & EXPRFLAG.EXF_UNREALIZEDGOTO) != 0)
				{
					return null;
				}
				throw ErrorHandling.Error(ErrorCode.ERR_ObjectProhibited, swt);
			}
			throw ErrorHandling.Error(ErrorCode.ERR_ObjectRequired, swt);
		}
		if (isStatic)
		{
			return null;
		}
		if (swt.Sym is MethodSymbol && swt.Meth().IsConstructor())
		{
			return pObject;
		}
		if (pObject == null)
		{
			return null;
		}
		CType cType = pObject.Type;
		CType ats;
		if (cType is NullableType nullableType && (ats = nullableType.GetAts()) != swt.GetType())
		{
			cType = ats;
		}
		if (cType is TypeParameterType || cType is AggregateType)
		{
			_ = swt.Sym.parent;
			pObject = tryConvert(pObject, swt.GetType(), CONVERTTYPE.NOUDC);
		}
		return pObject;
	}

	private static bool IsMatchingStatic(SymWithType swt, Expr pObject)
	{
		if (swt.Sym is MethodSymbol methodSymbol && methodSymbol.IsConstructor())
		{
			return !methodSymbol.isStatic;
		}
		if (swt.Sym.isStatic)
		{
			if (pObject == null)
			{
				return true;
			}
			if ((pObject.Flags & EXPRFLAG.EXF_SAMENAMETYPE) == 0)
			{
				return false;
			}
		}
		else if (pObject == null)
		{
			return false;
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void verifyMethodArgs(ExprWithArgs call, CType callingObjectType)
	{
		Expr optionalArguments = call.OptionalArguments;
		SymWithType symWithType = call.GetSymWithType();
		MethodOrPropertySymbol mp = symWithType.Sym as MethodOrPropertySymbol;
		TypeArray pTypeArgs = (call as ExprCall)?.MethWithInst.TypeArgs;
		AdjustCallArgumentsForParams(callingObjectType, symWithType.GetType(), mp, pTypeArgs, optionalArguments, out var newArgs);
		call.OptionalArguments = newArgs;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void AdjustCallArgumentsForParams(CType callingObjectType, CType type, MethodOrPropertySymbol mp, TypeArray pTypeArgs, Expr argsPtr, out Expr newArgs)
	{
		newArgs = null;
		Expr last = null;
		MethodOrPropertySymbol methodOrPropertySymbol = GroupToArgsBinder.FindMostDerivedMethod(mp, callingObjectType);
		int num = mp.Params.Count;
		TypeArray typeArray = mp.Params;
		int num2 = 0;
		int num3 = ExpressionIterator.Count(argsPtr);
		bool flag = false;
		ExpressionIterator expressionIterator = new ExpressionIterator(argsPtr);
		if (argsPtr == null)
		{
			if (!mp.isParamArray)
			{
				return;
			}
		}
		else
		{
			while (true)
			{
				if (expressionIterator.AtEnd())
				{
					return;
				}
				Expr expr = expressionIterator.Current();
				if (expr.Type is ParameterModifierType)
				{
					if (num != 0)
					{
						num--;
					}
					ExprFactory.AppendItemToList(expr, ref newArgs, ref last);
				}
				else if (num != 0)
				{
					if (num == 1 && mp.isParamArray && num3 > mp.Params.Count)
					{
						break;
					}
					Expr expr2 = expr;
					Expr expr3;
					if (expr2 is ExprNamedArgumentSpecification exprNamedArgumentSpecification)
					{
						int num4 = 0;
						using (List<Name>.Enumerator enumerator = methodOrPropertySymbol.ParameterNames.GetEnumerator())
						{
							while (enumerator.MoveNext() && enumerator.Current != exprNamedArgumentSpecification.Name)
							{
								num4++;
							}
						}
						CType dest = TypeManager.SubstType(typeArray[num4], type, pTypeArgs);
						if (!canConvert(exprNamedArgumentSpecification.Value, dest) && mp.isParamArray && num4 == mp.Params.Count - 1)
						{
							ExprArrayInit exprArrayInit = ExprFactory.CreateArrayInit((ArrayType)TypeManager.SubstType(mp.Params[mp.Params.Count - 1], type, pTypeArgs), null, null, s_zero);
							exprArrayInit.GeneratedForParamArray = true;
							exprArrayInit.OptionalArguments = exprNamedArgumentSpecification.Value;
							exprNamedArgumentSpecification.Value = exprArrayInit;
							flag = true;
						}
						else
						{
							exprNamedArgumentSpecification.Value = tryConvert(exprNamedArgumentSpecification.Value, dest);
						}
						expr3 = expr2;
					}
					else
					{
						CType dest2 = TypeManager.SubstType(typeArray[num2], type, pTypeArgs);
						expr3 = tryConvert(expr, dest2);
					}
					if (expr3 == null)
					{
						if (mp.isParamArray && num == 1 && num3 >= mp.Params.Count)
						{
							break;
						}
						return;
					}
					expr = expr3;
					ExprFactory.AppendItemToList(expr3, ref newArgs, ref last);
					num--;
				}
				num2++;
				if (num != 0 && mp.isParamArray && num2 == num3)
				{
					expr = null;
					expressionIterator.MoveNext();
					break;
				}
				expressionIterator.MoveNext();
			}
		}
		if (flag)
		{
			return;
		}
		CType cType = TypeManager.SubstType(mp.Params[mp.Params.Count - 1], type, pTypeArgs);
		if (!(cType is ArrayType { IsSZArray: not false, ElementType: var elementType }))
		{
			return;
		}
		ExprArrayInit exprArrayInit2 = ExprFactory.CreateArrayInit(cType, null, null, s_zero);
		exprArrayInit2.GeneratedForParamArray = true;
		if (expressionIterator.AtEnd())
		{
			exprArrayInit2.DimensionSizes[0] = 0;
			exprArrayInit2.OptionalArguments = null;
			argsPtr = ((argsPtr != null) ? ((Expr)ExprFactory.CreateList(argsPtr, exprArrayInit2)) : ((Expr)exprArrayInit2));
			ExprFactory.AppendItemToList(exprArrayInit2, ref newArgs, ref last);
			return;
		}
		Expr first = null;
		Expr last2 = null;
		int num5 = 0;
		while (!expressionIterator.AtEnd())
		{
			Expr expr4 = expressionIterator.Current();
			num5++;
			if (expr4 is ExprNamedArgumentSpecification exprNamedArgumentSpecification2)
			{
				exprNamedArgumentSpecification2.Value = tryConvert(exprNamedArgumentSpecification2.Value, elementType);
			}
			else
			{
				expr4 = tryConvert(expr4, elementType);
			}
			ExprFactory.AppendItemToList(expr4, ref first, ref last2);
			expressionIterator.MoveNext();
		}
		exprArrayInit2.DimensionSizes[0] = num5;
		exprArrayInit2.OptionalArguments = first;
		ExprFactory.AppendItemToList(exprArrayInit2, ref newArgs, ref last);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal CType ChooseArrayIndexType(Expr args)
	{
		ReadOnlySpan<PredefinedType> rgptIntOp = RgptIntOp;
		for (int i = 0; i < rgptIntOp.Length; i++)
		{
			CType predefindType = GetPredefindType(rgptIntOp[i]);
			using IEnumerator<Expr> enumerator = args.ToEnumerable().GetEnumerator();
			Expr current;
			do
			{
				if (enumerator.MoveNext())
				{
					current = enumerator.Current;
					continue;
				}
				return predefindType;
			}
			while (canConvert(current, predefindType));
		}
		return GetPredefindType(PredefinedType.PT_INT);
	}

	internal static void FillInArgInfoFromArgList(ArgInfos argInfo, Expr args)
	{
		CType[] array = new CType[argInfo.carg];
		argInfo.prgexpr = new List<Expr>();
		int num = 0;
		Expr expr = args;
		while (expr != null)
		{
			Expr expr2;
			if (expr is ExprList exprList)
			{
				expr2 = exprList.OptionalElement;
				expr = exprList.OptionalNextListNode;
			}
			else
			{
				expr2 = expr;
				expr = null;
			}
			array[num] = expr2.Type;
			argInfo.prgexpr.Add(expr2);
			num++;
		}
		argInfo.types = TypeArray.Allocate(array);
	}

	private static bool TryGetExpandedParams(TypeArray @params, int count, out TypeArray ppExpandedParams)
	{
		CType[] array;
		if (count < @params.Count - 1)
		{
			array = new CType[@params.Count - 1];
			@params.CopyItems(0, @params.Count - 1, array);
			ppExpandedParams = TypeArray.Allocate(array);
			return true;
		}
		array = new CType[count];
		@params.CopyItems(0, @params.Count - 1, array);
		if (!(@params[@params.Count - 1] is ArrayType { ElementType: var elementType }))
		{
			ppExpandedParams = null;
			return false;
		}
		for (int i = @params.Count - 1; i < count; i++)
		{
			array[i] = elementType;
		}
		ppExpandedParams = TypeArray.Allocate(array);
		return true;
	}

	public static bool IsMethPropCallable(MethodOrPropertySymbol sym, bool requireUC)
	{
		if (!sym.isOverride || sym.isHideByName)
		{
			if (requireUC)
			{
				return sym.isUserCallable();
			}
			return true;
		}
		return false;
	}

	private static bool IsConvInTable(List<UdConvInfo> convTable, MethodSymbol meth, AggregateType ats, bool fSrc, bool fDst)
	{
		foreach (UdConvInfo item in convTable)
		{
			if (item.Meth.Meth() == meth && item.Meth.GetType() == ats && item.SrcImplicit == fSrc && item.DstImplicit == fDst)
			{
				return true;
			}
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool isConstantInRange(ExprConstant exprSrc, CType typeDest)
	{
		return isConstantInRange(exprSrc, typeDest, realsOk: false);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool isConstantInRange(ExprConstant exprSrc, CType typeDest, bool realsOk)
	{
		FUNDTYPE fundamentalType = exprSrc.Type.FundamentalType;
		FUNDTYPE fundamentalType2 = typeDest.FundamentalType;
		if (fundamentalType > FUNDTYPE.FT_U8 || fundamentalType2 > FUNDTYPE.FT_U8)
		{
			if (!realsOk)
			{
				return false;
			}
			if (fundamentalType > FUNDTYPE.FT_R8 || fundamentalType2 > FUNDTYPE.FT_R8)
			{
				return false;
			}
		}
		if (fundamentalType2 > FUNDTYPE.FT_U8)
		{
			return true;
		}
		if (fundamentalType > FUNDTYPE.FT_U8)
		{
			double doubleVal = exprSrc.Val.DoubleVal;
			switch (fundamentalType2)
			{
			case FUNDTYPE.FT_I1:
				if (doubleVal > -129.0 && doubleVal < 128.0)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_I2:
				if (doubleVal > -32769.0 && doubleVal < 32768.0)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_I4:
				if (doubleVal > -2147483649.0 && doubleVal < (double)I64(2147483648L))
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_I8:
				if (doubleVal >= -9.223372036854776E+18 && doubleVal < 9.223372036854776E+18)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_U1:
				if (doubleVal > -1.0 && doubleVal < 256.0)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_U2:
				if (doubleVal > -1.0 && doubleVal < 65536.0)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_U4:
				if (doubleVal > -1.0 && doubleVal < 4294967296.0)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_U8:
				if (doubleVal > -1.0 && doubleVal < 1.8446744073709552E+19)
				{
					return true;
				}
				break;
			}
			return false;
		}
		if (fundamentalType == FUNDTYPE.FT_U8)
		{
			ulong uInt64Value = exprSrc.UInt64Value;
			switch (fundamentalType2)
			{
			case FUNDTYPE.FT_I1:
				if (uInt64Value <= 127)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_I2:
				if (uInt64Value <= 32767)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_I4:
				if (uInt64Value <= int.MaxValue)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_I8:
				if (uInt64Value <= long.MaxValue)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_U1:
				if (uInt64Value <= 255)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_U2:
				if (uInt64Value <= 65535)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_U4:
				if (uInt64Value <= uint.MaxValue)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_U8:
				return true;
			}
		}
		else
		{
			long int64Value = exprSrc.Int64Value;
			switch (fundamentalType2)
			{
			case FUNDTYPE.FT_I1:
				if (int64Value >= -128 && int64Value <= 127)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_I2:
				if (int64Value >= -32768 && int64Value <= 32767)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_I4:
				if (int64Value >= I64(-2147483648L) && int64Value <= I64(2147483647L))
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_I8:
				return true;
			case FUNDTYPE.FT_U1:
				if (int64Value >= 0 && int64Value <= 255)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_U2:
				if (int64Value >= 0 && int64Value <= 65535)
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_U4:
				if (int64Value >= 0 && int64Value <= I64(4294967295L))
				{
					return true;
				}
				break;
			case FUNDTYPE.FT_U8:
				if (int64Value >= 0)
				{
					return true;
				}
				break;
			}
		}
		return false;
	}

	private static Name ExpressionKindName(ExpressionKind ek)
	{
		return NameManager.GetPredefinedName(EK2NAME[(int)(ek - 29)]);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void CheckUnsafe(CType type)
	{
		if (type == null || type.IsUnsafe())
		{
			throw ErrorHandling.Error(ErrorCode.ERR_UnsafeNeeded);
		}
	}

	private static ExprWrap WrapShortLivedExpression(Expr expr)
	{
		return ExprFactory.CreateWrap(expr);
	}

	private static ExprAssignment GenerateOptimizedAssignment(Expr op1, Expr op2)
	{
		return ExprFactory.CreateAssignment(op1, op2);
	}

	internal static int CountArguments(Expr args)
	{
		int num = 0;
		Expr expr = args;
		while (expr != null)
		{
			if (expr is ExprList exprList)
			{
				_ = exprList.OptionalElement;
				expr = exprList.OptionalNextListNode;
			}
			else
			{
				expr = null;
			}
			num++;
		}
		return num;
	}

	private static bool IsNullableConstructor(Expr expr, out ExprCall call)
	{
		if (expr is ExprCall exprCall && exprCall.MemberGroup.OptionalObject == null)
		{
			MethWithInst methWithInst = exprCall.MethWithInst;
			if ((object)methWithInst != null && methWithInst.Meth().IsNullableConstructor())
			{
				call = exprCall;
				return true;
			}
		}
		call = null;
		return false;
	}

	private static Expr StripNullableConstructor(Expr pExpr)
	{
		ExprCall call;
		while (IsNullableConstructor(pExpr, out call))
		{
			pExpr = call.OptionalArguments;
		}
		return pExpr;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static Expr BindNubValue(Expr exprSrc)
	{
		if (IsNullableConstructor(exprSrc, out var call))
		{
			return call.OptionalArguments;
		}
		NullableType obj = (NullableType)exprSrc.Type;
		CType underlyingType = obj.UnderlyingType;
		AggregateType ats = obj.GetAts();
		PropertySymbol property = PredefinedMembers.GetProperty(PREDEFPROP.PP_G_OPTIONAL_VALUE);
		PropWithType property2 = new PropWithType(property, ats);
		MethPropWithInst method = new MethPropWithInst(property, ats);
		ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(exprSrc, method);
		return ExprFactory.CreateProperty(underlyingType, null, null, memberGroup, property2, null);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprCall BindNubNew(Expr exprSrc)
	{
		NullableType nullable = TypeManager.GetNullable(exprSrc.Type);
		AggregateType ats = nullable.GetAts();
		MethWithInst method = new MethWithInst(PredefinedMembers.GetMethod(PREDEFMETH.PM_G_OPTIONAL_CTOR), ats, TypeArray.Empty);
		ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, method);
		return ExprFactory.CreateCall(EXPRFLAG.EXF_LITERALCONST | EXPRFLAG.EXF_CANTBENULL, nullable, exprSrc, memberGroup, method);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050:RequiresDynamicCode", Justification = "We can't annotate static constructors on structs, but everything else in this type is annotated.")]
	private static BinOpSig[] InitBinopSignatures()
	{
		return new BinOpSig[20]
		{
			new BinOpSig(PredefinedType.PT_INT, PredefinedType.PT_INT, BinOpMask.Integer, 8, BindIntBinOp, OpSigFlags.Value, BinOpFuncKind.IntBinOp),
			new BinOpSig(PredefinedType.PT_UINT, PredefinedType.PT_UINT, BinOpMask.Integer, 7, BindIntBinOp, OpSigFlags.Value, BinOpFuncKind.IntBinOp),
			new BinOpSig(PredefinedType.PT_LONG, PredefinedType.PT_LONG, BinOpMask.Integer, 6, BindIntBinOp, OpSigFlags.Value, BinOpFuncKind.IntBinOp),
			new BinOpSig(PredefinedType.PT_ULONG, PredefinedType.PT_ULONG, BinOpMask.Integer, 5, BindIntBinOp, OpSigFlags.Value, BinOpFuncKind.IntBinOp),
			new BinOpSig(PredefinedType.PT_ULONG, PredefinedType.PT_LONG, BinOpMask.Integer, 4, null, OpSigFlags.Value, BinOpFuncKind.None),
			new BinOpSig(PredefinedType.PT_LONG, PredefinedType.PT_ULONG, BinOpMask.Integer, 3, null, OpSigFlags.Value, BinOpFuncKind.None),
			new BinOpSig(PredefinedType.PT_FLOAT, PredefinedType.PT_FLOAT, BinOpMask.Real, 1, BindRealBinOp, OpSigFlags.Value, BinOpFuncKind.RealBinOp),
			new BinOpSig(PredefinedType.PT_DOUBLE, PredefinedType.PT_DOUBLE, BinOpMask.Real, 0, BindRealBinOp, OpSigFlags.Value, BinOpFuncKind.RealBinOp),
			new BinOpSig(PredefinedType.PT_DECIMAL, PredefinedType.PT_DECIMAL, BinOpMask.Real, 0, BindDecBinOp, OpSigFlags.Value, BinOpFuncKind.DecBinOp),
			new BinOpSig(PredefinedType.PT_STRING, PredefinedType.PT_STRING, BinOpMask.Equal, 0, BindStrCmpOp, OpSigFlags.Convert, BinOpFuncKind.StrCmpOp),
			new BinOpSig(PredefinedType.PT_STRING, PredefinedType.PT_STRING, BinOpMask.Add, 2, BindStrBinOp, OpSigFlags.Convert, BinOpFuncKind.StrBinOp),
			new BinOpSig(PredefinedType.PT_STRING, PredefinedType.PT_OBJECT, BinOpMask.Add, 1, BindStrBinOp, OpSigFlags.Convert, BinOpFuncKind.StrBinOp),
			new BinOpSig(PredefinedType.PT_OBJECT, PredefinedType.PT_STRING, BinOpMask.Add, 0, BindStrBinOp, OpSigFlags.Convert, BinOpFuncKind.StrBinOp),
			new BinOpSig(PredefinedType.PT_INT, PredefinedType.PT_INT, BinOpMask.Shift, 3, BindShiftOp, OpSigFlags.Value, BinOpFuncKind.ShiftOp),
			new BinOpSig(PredefinedType.PT_UINT, PredefinedType.PT_INT, BinOpMask.Shift, 2, BindShiftOp, OpSigFlags.Value, BinOpFuncKind.ShiftOp),
			new BinOpSig(PredefinedType.PT_LONG, PredefinedType.PT_INT, BinOpMask.Shift, 1, BindShiftOp, OpSigFlags.Value, BinOpFuncKind.ShiftOp),
			new BinOpSig(PredefinedType.PT_ULONG, PredefinedType.PT_INT, BinOpMask.Shift, 0, BindShiftOp, OpSigFlags.Value, BinOpFuncKind.ShiftOp),
			new BinOpSig(PredefinedType.PT_BOOL, PredefinedType.PT_BOOL, BinOpMask.BoolNorm, 0, BindBoolBinOp, OpSigFlags.Value, BinOpFuncKind.BoolBinOp),
			new BinOpSig(PredefinedType.PT_BOOL, PredefinedType.PT_BOOL, BinOpMask.Logical, 0, BindBoolBinOp, OpSigFlags.BoolBit, BinOpFuncKind.BoolBinOp),
			new BinOpSig(PredefinedType.PT_BOOL, PredefinedType.PT_BOOL, BinOpMask.Bitwise, 0, BindLiftedBoolBitwiseOp, OpSigFlags.BoolBit, BinOpFuncKind.BoolBitwiseOp)
		};
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050:RequiresDynamicCode", Justification = "We can't annotate static constructors on structs, but everything else in this type is annotated.")]
	private static UnaOpSig[] InitUnaOpSignatures()
	{
		return new UnaOpSig[16]
		{
			new UnaOpSig(PredefinedType.PT_INT, UnaOpMask.Signed, 7, BindIntUnaOp, UnaOpFuncKind.IntUnaOp),
			new UnaOpSig(PredefinedType.PT_UINT, UnaOpMask.Unsigned, 6, BindIntUnaOp, UnaOpFuncKind.IntUnaOp),
			new UnaOpSig(PredefinedType.PT_LONG, UnaOpMask.Signed, 5, BindIntUnaOp, UnaOpFuncKind.IntUnaOp),
			new UnaOpSig(PredefinedType.PT_ULONG, UnaOpMask.Unsigned, 4, BindIntUnaOp, UnaOpFuncKind.IntUnaOp),
			new UnaOpSig(PredefinedType.PT_ULONG, UnaOpMask.Minus, 3, null, UnaOpFuncKind.None),
			new UnaOpSig(PredefinedType.PT_FLOAT, UnaOpMask.Real, 1, BindRealUnaOp, UnaOpFuncKind.RealUnaOp),
			new UnaOpSig(PredefinedType.PT_DOUBLE, UnaOpMask.Real, 0, BindRealUnaOp, UnaOpFuncKind.RealUnaOp),
			new UnaOpSig(PredefinedType.PT_DECIMAL, UnaOpMask.Real, 0, BindDecUnaOp, UnaOpFuncKind.DecUnaOp),
			new UnaOpSig(PredefinedType.PT_BOOL, UnaOpMask.Bang, 0, BindBoolUnaOp, UnaOpFuncKind.BoolUnaOp),
			new UnaOpSig(PredefinedType.PT_INT, UnaOpMask.IncDec, 6, null, UnaOpFuncKind.None),
			new UnaOpSig(PredefinedType.PT_UINT, UnaOpMask.IncDec, 5, null, UnaOpFuncKind.None),
			new UnaOpSig(PredefinedType.PT_LONG, UnaOpMask.IncDec, 4, null, UnaOpFuncKind.None),
			new UnaOpSig(PredefinedType.PT_ULONG, UnaOpMask.IncDec, 3, null, UnaOpFuncKind.None),
			new UnaOpSig(PredefinedType.PT_FLOAT, UnaOpMask.IncDec, 1, null, UnaOpFuncKind.None),
			new UnaOpSig(PredefinedType.PT_DOUBLE, UnaOpMask.IncDec, 0, null, UnaOpFuncKind.None),
			new UnaOpSig(PredefinedType.PT_DECIMAL, UnaOpMask.IncDec, 0, null, UnaOpFuncKind.None)
		};
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprBinOp BindUserDefinedBinOp(ExpressionKind ek, BinOpArgInfo info)
	{
		if (info.pt1 <= PredefinedType.PT_ULONG && info.pt2 <= PredefinedType.PT_ULONG)
		{
			return null;
		}
		MethPropWithInst ppmpwi;
		Expr expr;
		if (info.binopKind == BinOpKind.Logical)
		{
			ExprCall exprCall = BindUDBinop(ek - 55 + 49, info.arg1, info.arg2, fDontLift: true, out ppmpwi);
			if (exprCall == null)
			{
				return null;
			}
			expr = BindUserBoolOp(ek, exprCall);
		}
		else
		{
			expr = BindUDBinop(ek, info.arg1, info.arg2, fDontLift: false, out ppmpwi);
		}
		if (expr == null)
		{
			return null;
		}
		return ExprFactory.CreateUserDefinedBinop(ek, expr.Type, info.arg1, info.arg2, expr, ppmpwi);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool GetSpecialBinopSignatures(List<BinOpFullSig> prgbofs, BinOpArgInfo info)
	{
		if (info.pt1 <= PredefinedType.PT_ULONG && info.pt2 <= PredefinedType.PT_ULONG)
		{
			return false;
		}
		if (!GetDelBinOpSigs(prgbofs, info) && !GetEnumBinOpSigs(prgbofs, info))
		{
			return GetRefEqualSigs(prgbofs, info);
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool GetStandardAndLiftedBinopSignatures(List<BinOpFullSig> rgbofs, BinOpArgInfo info)
	{
		int num = 0;
		for (int i = 0; i < s_binopSignatures.Length; i++)
		{
			BinOpSig binOpSig = s_binopSignatures[i];
			if ((binOpSig.mask & info.mask) == 0)
			{
				continue;
			}
			CType cType = GetPredefindType(binOpSig.pt1);
			CType cType2 = GetPredefindType(binOpSig.pt2);
			if (cType == null || cType2 == null)
			{
				continue;
			}
			ConvKind convKind = GetConvKind(info.pt1, binOpSig.pt1);
			ConvKind convKind2 = GetConvKind(info.pt2, binOpSig.pt2);
			LiftFlags liftFlags = LiftFlags.None;
			switch (convKind)
			{
			case ConvKind.Explicit:
				if (!(info.arg1 is ExprConstant expr))
				{
					continue;
				}
				if (!canConvert(expr, cType))
				{
					if (i < num || !binOpSig.CanLift())
					{
						continue;
					}
					cType = TypeManager.GetNullable(cType);
					if (!canConvert(expr, cType))
					{
						continue;
					}
					ConvKind convKind3 = GetConvKind(info.ptRaw1, binOpSig.pt1);
					liftFlags = (((uint)(convKind3 - 1) <= 1u) ? (liftFlags | LiftFlags.Lift1) : (liftFlags | LiftFlags.Convert1));
				}
				break;
			case ConvKind.Unknown:
				if (!canConvert(info.arg1, cType))
				{
					if (i < num || !binOpSig.CanLift())
					{
						continue;
					}
					cType = TypeManager.GetNullable(cType);
					if (!canConvert(info.arg1, cType))
					{
						continue;
					}
					ConvKind convKind3 = GetConvKind(info.ptRaw1, binOpSig.pt1);
					liftFlags = (((uint)(convKind3 - 1) <= 1u) ? (liftFlags | LiftFlags.Lift1) : (liftFlags | LiftFlags.Convert1));
				}
				break;
			case ConvKind.Identity:
				if (convKind2 == ConvKind.Identity)
				{
					BinOpFullSig binOpFullSig = new BinOpFullSig(binOpSig);
					if (binOpFullSig.Type1() != null && binOpFullSig.Type2() != null)
					{
						rgbofs.Add(binOpFullSig);
						return true;
					}
				}
				break;
			case ConvKind.Implicit:
				break;
			default:
				continue;
			}
			switch (convKind2)
			{
			case ConvKind.Explicit:
				if (!(info.arg2 is ExprConstant expr2))
				{
					continue;
				}
				if (!canConvert(expr2, cType2))
				{
					if (i < num || !binOpSig.CanLift())
					{
						continue;
					}
					cType2 = TypeManager.GetNullable(cType2);
					if (!canConvert(expr2, cType2))
					{
						continue;
					}
					ConvKind convKind3 = GetConvKind(info.ptRaw2, binOpSig.pt2);
					liftFlags = (((uint)(convKind3 - 1) <= 1u) ? (liftFlags | LiftFlags.Lift2) : (liftFlags | LiftFlags.Convert2));
				}
				break;
			case ConvKind.Unknown:
				if (!canConvert(info.arg2, cType2))
				{
					if (i < num || !binOpSig.CanLift())
					{
						continue;
					}
					cType2 = TypeManager.GetNullable(cType2);
					if (!canConvert(info.arg2, cType2))
					{
						continue;
					}
					ConvKind convKind3 = GetConvKind(info.ptRaw2, binOpSig.pt2);
					liftFlags = (((uint)(convKind3 - 1) <= 1u) ? (liftFlags | LiftFlags.Lift2) : (liftFlags | LiftFlags.Convert2));
				}
				break;
			case ConvKind.Identity:
			case ConvKind.Implicit:
				break;
			default:
				continue;
			}
			if (liftFlags != LiftFlags.None)
			{
				rgbofs.Add(new BinOpFullSig(cType, cType2, binOpSig.pfn, binOpSig.grfos, liftFlags, binOpSig.fnkind));
				num = i + binOpSig.cbosSkip + 1;
			}
			else
			{
				rgbofs.Add(new BinOpFullSig(binOpSig));
				i += binOpSig.cbosSkip;
			}
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private int FindBestSignatureInList(List<BinOpFullSig> binopSignatures, BinOpArgInfo info)
	{
		if (binopSignatures.Count == 1)
		{
			return 0;
		}
		int num = 0;
		for (int i = 1; i < binopSignatures.Count; i++)
		{
			if (num < 0)
			{
				num = i;
				continue;
			}
			int num2 = WhichBofsIsBetter(binopSignatures[num], binopSignatures[i], info.type1, info.type2);
			if (num2 == 0)
			{
				num = -1;
			}
			else if (num2 > 0)
			{
				num = i;
			}
		}
		if (num == -1)
		{
			return -1;
		}
		for (int i = 0; i < binopSignatures.Count; i++)
		{
			if (i != num && WhichBofsIsBetter(binopSignatures[num], binopSignatures[i], info.type1, info.type2) >= 0)
			{
				return -1;
			}
		}
		return num;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprBinOp BindNullEqualityComparison(ExpressionKind ek, BinOpArgInfo info)
	{
		Expr arg = info.arg1;
		Expr right = info.arg2;
		if (info.binopKind == BinOpKind.Equal)
		{
			CType predefindType = GetPredefindType(PredefinedType.PT_BOOL);
			ExprBinOp exprBinOp = null;
			if (info.type1 is NullableType && info.type2 is NullType)
			{
				right = ExprFactory.CreateZeroInit(info.type1);
				exprBinOp = ExprFactory.CreateBinop(ek, predefindType, arg, right);
			}
			if (info.type1 is NullType && info.type2 is NullableType)
			{
				arg = ExprFactory.CreateZeroInit(info.type2);
				exprBinOp = ExprFactory.CreateBinop(ek, predefindType, arg, right);
			}
			if (exprBinOp != null)
			{
				exprBinOp.IsLifted = true;
				return exprBinOp;
			}
		}
		throw BadOperatorTypesError(info.arg1, info.arg2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public Expr BindStandardBinop(ExpressionKind ek, Expr arg1, Expr arg2)
	{
		(BinOpKind, EXPRFLAG) binopKindAndFlags = GetBinopKindAndFlags(ek);
		BinOpKind item = binopKindAndFlags.Item1;
		EXPRFLAG item2 = binopKindAndFlags.Item2;
		BinOpArgInfo binOpArgInfo = new BinOpArgInfo(arg1, arg2)
		{
			binopKind = item
		};
		binOpArgInfo.mask = (BinOpMask)(1 << (int)binOpArgInfo.binopKind);
		List<BinOpFullSig> list = new List<BinOpFullSig>();
		ExprBinOp exprBinOp = BindUserDefinedBinOp(ek, binOpArgInfo);
		if (exprBinOp != null)
		{
			return exprBinOp;
		}
		bool flag = GetSpecialBinopSignatures(list, binOpArgInfo);
		if (!flag)
		{
			flag = GetStandardAndLiftedBinopSignatures(list, binOpArgInfo);
		}
		int num;
		if (flag)
		{
			num = list.Count - 1;
		}
		else
		{
			if (list.Count == 0)
			{
				return BindNullEqualityComparison(ek, binOpArgInfo);
			}
			num = FindBestSignatureInList(list, binOpArgInfo);
			if (num < 0)
			{
				throw AmbiguousOperatorError(arg1, arg2);
			}
		}
		return BindStandardBinopCore(binOpArgInfo, list[num], ek, item2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr BindStandardBinopCore(BinOpArgInfo info, BinOpFullSig bofs, ExpressionKind ek, EXPRFLAG flags)
	{
		if (bofs.pfn == null)
		{
			throw BadOperatorTypesError(info.arg1, info.arg2);
		}
		if (!bofs.isLifted() || !bofs.AutoLift())
		{
			Expr expr = info.arg1;
			Expr expr2 = info.arg2;
			if (bofs.ConvertOperandsBeforeBinding())
			{
				expr = mustConvert(expr, bofs.Type1());
				expr2 = mustConvert(expr2, bofs.Type2());
			}
			if (bofs.fnkind == BinOpFuncKind.BoolBitwiseOp)
			{
				return BindBoolBitwiseOp(ek, flags, expr, expr2);
			}
			return bofs.pfn(this, ek, flags, expr, expr2);
		}
		if (IsEnumArithmeticBinOp(ek, info))
		{
			Expr expr3 = info.arg1;
			Expr expr4 = info.arg2;
			if (bofs.ConvertOperandsBeforeBinding())
			{
				expr3 = mustConvert(expr3, bofs.Type1());
				expr4 = mustConvert(expr4, bofs.Type2());
			}
			return BindLiftedEnumArithmeticBinOp(ek, flags, expr3, expr4);
		}
		return BindLiftedStandardBinOp(info, bofs, ek, flags);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprBinOp BindLiftedStandardBinOp(BinOpArgInfo info, BinOpFullSig bofs, ExpressionKind ek, EXPRFLAG flags)
	{
		Expr arg = info.arg1;
		Expr arg2 = info.arg2;
		Expr expr = null;
		LiftArgument(arg, bofs.Type1(), bofs.ConvertFirst(), out var ppLiftedArgument, out var ppNonLiftedArgument);
		LiftArgument(arg2, bofs.Type2(), bofs.ConvertSecond(), out var ppLiftedArgument2, out var ppNonLiftedArgument2);
		if (!ppNonLiftedArgument.isNull() && !ppNonLiftedArgument2.isNull())
		{
			expr = bofs.pfn(this, ek, flags, ppNonLiftedArgument, ppNonLiftedArgument2);
		}
		CType cType;
		if (info.binopKind == BinOpKind.Compare || info.binopKind == BinOpKind.Equal)
		{
			cType = GetPredefindType(PredefinedType.PT_BOOL);
		}
		else
		{
			cType = ((bofs.fnkind == BinOpFuncKind.EnumBinOp) ? GetEnumBinOpType(ek, ppNonLiftedArgument.Type, ppNonLiftedArgument2.Type, out var _) : ppLiftedArgument.Type);
			if (!(cType is NullableType))
			{
				cType = TypeManager.GetNullable(cType);
			}
		}
		ExprBinOp exprBinOp = ExprFactory.CreateBinop(ek, cType, ppLiftedArgument, ppLiftedArgument2);
		mustCast(expr, cType, (CONVERTTYPE)0);
		exprBinOp.IsLifted = true;
		exprBinOp.Flags |= flags;
		return exprBinOp;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void LiftArgument(Expr pArgument, CType pParameterType, bool bConvertBeforeLift, out Expr ppLiftedArgument, out Expr ppNonLiftedArgument)
	{
		Expr expr = mustConvert(pArgument, pParameterType);
		if (expr != pArgument)
		{
			MarkAsIntermediateConversion(expr);
		}
		Expr expr2 = pArgument;
		if (pParameterType is NullableType nullableType)
		{
			if (expr2.isNull())
			{
				expr2 = mustCast(expr2, pParameterType);
			}
			expr2 = mustCast(expr2, nullableType.UnderlyingType);
			if (bConvertBeforeLift)
			{
				MarkAsIntermediateConversion(expr2);
			}
		}
		else
		{
			expr2 = expr;
		}
		ppLiftedArgument = expr;
		ppNonLiftedArgument = expr2;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool GetDelBinOpSigs(List<BinOpFullSig> prgbofs, BinOpArgInfo info)
	{
		if (!info.ValidForDelegate() || (!info.type1.IsDelegateType && !info.type2.IsDelegateType))
		{
			return false;
		}
		if (info.type1 == info.type2)
		{
			prgbofs.Add(new BinOpFullSig(info.type1, info.type2, BindDelBinOp, OpSigFlags.Convert, LiftFlags.None, BinOpFuncKind.DelBinOp));
			return true;
		}
		bool num = info.type2.IsDelegateType && canConvert(info.arg1, info.type2);
		bool flag = info.type1.IsDelegateType && canConvert(info.arg2, info.type1);
		if (num)
		{
			prgbofs.Add(new BinOpFullSig(info.type2, info.type2, BindDelBinOp, OpSigFlags.Convert, LiftFlags.None, BinOpFuncKind.DelBinOp));
		}
		if (flag)
		{
			prgbofs.Add(new BinOpFullSig(info.type1, info.type1, BindDelBinOp, OpSigFlags.Convert, LiftFlags.None, BinOpFuncKind.DelBinOp));
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool CanConvertArg1(BinOpArgInfo info, CType typeDst, out LiftFlags pgrflt, out CType ptypeSig1, out CType ptypeSig2)
	{
		ptypeSig1 = null;
		ptypeSig2 = null;
		if (canConvert(info.arg1, typeDst))
		{
			pgrflt = LiftFlags.None;
		}
		else
		{
			pgrflt = LiftFlags.None;
			typeDst = TypeManager.GetNullable(typeDst);
			if (!canConvert(info.arg1, typeDst))
			{
				return false;
			}
			pgrflt = LiftFlags.Convert1;
		}
		ptypeSig1 = typeDst;
		if (info.type2 is NullableType)
		{
			pgrflt |= LiftFlags.Lift2;
			ptypeSig2 = TypeManager.GetNullable(info.typeRaw2);
		}
		else
		{
			ptypeSig2 = info.typeRaw2;
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool CanConvertArg2(BinOpArgInfo info, CType typeDst, out LiftFlags pgrflt, out CType ptypeSig1, out CType ptypeSig2)
	{
		ptypeSig1 = null;
		ptypeSig2 = null;
		if (canConvert(info.arg2, typeDst))
		{
			pgrflt = LiftFlags.None;
		}
		else
		{
			pgrflt = LiftFlags.None;
			typeDst = TypeManager.GetNullable(typeDst);
			if (!canConvert(info.arg2, typeDst))
			{
				return false;
			}
			pgrflt = LiftFlags.Convert2;
		}
		ptypeSig2 = typeDst;
		if (info.type1 is NullableType)
		{
			pgrflt |= LiftFlags.Lift1;
			ptypeSig1 = TypeManager.GetNullable(info.typeRaw1);
		}
		else
		{
			ptypeSig1 = info.typeRaw1;
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void RecordBinOpSigFromArgs(List<BinOpFullSig> prgbofs, BinOpArgInfo info)
	{
		LiftFlags liftFlags = LiftFlags.None;
		CType type;
		if (info.type1 != info.typeRaw1)
		{
			liftFlags |= LiftFlags.Lift1;
			type = TypeManager.GetNullable(info.typeRaw1);
		}
		else
		{
			type = info.typeRaw1;
		}
		CType type2;
		if (info.type2 != info.typeRaw2)
		{
			liftFlags |= LiftFlags.Lift2;
			type2 = TypeManager.GetNullable(info.typeRaw2);
		}
		else
		{
			type2 = info.typeRaw2;
		}
		prgbofs.Add(new BinOpFullSig(type, type2, BindEnumBinOp, OpSigFlags.Value, liftFlags, BinOpFuncKind.EnumBinOp));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool GetEnumBinOpSigs(List<BinOpFullSig> prgbofs, BinOpArgInfo info)
	{
		if (!info.typeRaw1.IsEnumType && !info.typeRaw2.IsEnumType)
		{
			return false;
		}
		CType ptypeSig = null;
		CType ptypeSig2 = null;
		LiftFlags pgrflt = LiftFlags.None;
		if (info.typeRaw1 == info.typeRaw2)
		{
			if (!info.ValidForEnum())
			{
				return false;
			}
			RecordBinOpSigFromArgs(prgbofs, info);
			return true;
		}
		bool num;
		if (!info.typeRaw1.IsEnumType)
		{
			if (info.typeRaw1 == info.typeRaw2.UnderlyingEnumType)
			{
				num = info.ValidForUnderlyingTypeAndEnum();
				goto IL_008a;
			}
		}
		else if (info.typeRaw2 == info.typeRaw1.UnderlyingEnumType)
		{
			num = info.ValidForEnumAndUnderlyingType();
			goto IL_008a;
		}
		goto IL_0097;
		IL_0097:
		if ((!info.typeRaw1.IsEnumType) ? ((info.ValidForEnum() && CanConvertArg1(info, info.typeRaw2, out pgrflt, out ptypeSig, out ptypeSig2)) || (info.ValidForEnumAndUnderlyingType() && CanConvertArg1(info, info.typeRaw2.UnderlyingEnumType, out pgrflt, out ptypeSig, out ptypeSig2))) : ((info.ValidForEnum() && CanConvertArg2(info, info.typeRaw1, out pgrflt, out ptypeSig, out ptypeSig2)) || (info.ValidForEnumAndUnderlyingType() && CanConvertArg2(info, info.typeRaw1.UnderlyingEnumType, out pgrflt, out ptypeSig, out ptypeSig2))))
		{
			prgbofs.Add(new BinOpFullSig(ptypeSig, ptypeSig2, BindEnumBinOp, OpSigFlags.Value, pgrflt, BinOpFuncKind.EnumBinOp));
		}
		return false;
		IL_008a:
		if (num)
		{
			RecordBinOpSigFromArgs(prgbofs, info);
			return true;
		}
		goto IL_0097;
	}

	private static bool IsEnumArithmeticBinOp(ExpressionKind ek, BinOpArgInfo info)
	{
		return ek switch
		{
			ExpressionKind.Add => info.typeRaw1.IsEnumType ^ info.typeRaw2.IsEnumType, 
			ExpressionKind.Subtract => info.typeRaw1.IsEnumType | info.typeRaw2.IsEnumType, 
			_ => false, 
		};
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool GetRefEqualSigs(List<BinOpFullSig> prgbofs, BinOpArgInfo info)
	{
		if (info.mask != BinOpMask.Equal)
		{
			return false;
		}
		if (info.type1 != info.typeRaw1 || info.type2 != info.typeRaw2)
		{
			return false;
		}
		bool result = false;
		CType cType = info.type1;
		CType cType2 = info.type2;
		CType predefindType = GetPredefindType(PredefinedType.PT_OBJECT);
		CType cType3 = null;
		if (cType is NullType && cType2 is NullType)
		{
			cType3 = predefindType;
			result = true;
		}
		else
		{
			CType predefindType2 = GetPredefindType(PredefinedType.PT_DELEGATE);
			if (canConvert(info.arg1, predefindType2) && canConvert(info.arg2, predefindType2) && !cType.IsDelegateType && !cType2.IsDelegateType)
			{
				prgbofs.Add(new BinOpFullSig(predefindType2, predefindType2, BindDelBinOp, OpSigFlags.Convert, LiftFlags.None, BinOpFuncKind.DelBinOp));
			}
			if (cType.FundamentalType != FUNDTYPE.FT_REF)
			{
				return false;
			}
			if (cType2 is NullType)
			{
				result = true;
				cType3 = predefindType;
			}
			else
			{
				if (cType2.FundamentalType != FUNDTYPE.FT_REF)
				{
					return false;
				}
				if (cType is NullType)
				{
					result = true;
					cType3 = predefindType;
				}
				else
				{
					if (!canCast(cType, cType2, CONVERTTYPE.NOUDC) && !canCast(cType2, cType, CONVERTTYPE.NOUDC))
					{
						return false;
					}
					if (cType.IsInterfaceType || cType.IsPredefType(PredefinedType.PT_STRING) || SymbolLoader.HasBaseConversion(cType, predefindType2))
					{
						cType = predefindType;
					}
					else if (cType is ArrayType)
					{
						cType = GetPredefindType(PredefinedType.PT_ARRAY);
					}
					else if (!cType.IsClassType)
					{
						return false;
					}
					if (cType2.IsInterfaceType || cType2.IsPredefType(PredefinedType.PT_STRING) || SymbolLoader.HasBaseConversion(cType2, predefindType2))
					{
						cType2 = predefindType;
					}
					else if (cType2 is ArrayType)
					{
						cType2 = GetPredefindType(PredefinedType.PT_ARRAY);
					}
					else if (!cType2.IsClassType)
					{
						return false;
					}
					if (SymbolLoader.HasBaseConversion(cType2, cType))
					{
						cType3 = cType;
					}
					else if (SymbolLoader.HasBaseConversion(cType, cType2))
					{
						cType3 = cType2;
					}
				}
			}
		}
		prgbofs.Add(new BinOpFullSig(cType3, cType3, BindRefCmpOp, OpSigFlags.None, LiftFlags.None, BinOpFuncKind.RefCmpOp));
		return result;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private int WhichBofsIsBetter(BinOpFullSig bofs1, BinOpFullSig bofs2, CType type1, CType type2)
	{
		BetterType betterType;
		BetterType betterType2;
		if (bofs1.FPreDef() && bofs2.FPreDef())
		{
			betterType = WhichTypeIsBetter(bofs1.pt1, bofs2.pt1, type1);
			betterType2 = WhichTypeIsBetter(bofs1.pt2, bofs2.pt2, type2);
		}
		else
		{
			betterType = WhichTypeIsBetter(bofs1.Type1(), bofs2.Type1(), type1);
			betterType2 = WhichTypeIsBetter(bofs1.Type2(), bofs2.Type2(), type2);
		}
		int num = betterType switch
		{
			BetterType.Left => -1, 
			BetterType.Right => 1, 
			_ => 0, 
		};
		switch (betterType2)
		{
		case BetterType.Left:
			num--;
			break;
		case BetterType.Right:
			num++;
			break;
		}
		return num;
	}

	private static (ExpressionKind, UnaOpKind, EXPRFLAG) CalculateExprAndUnaryOpKinds(OperatorKind op, bool bChecked)
	{
		EXPRFLAG eXPRFLAG = (EXPRFLAG)0;
		UnaOpKind item;
		ExpressionKind item2;
		switch (op)
		{
		case OperatorKind.OP_UPLUS:
			item = UnaOpKind.Plus;
			item2 = ExpressionKind.UnaryPlus;
			break;
		case OperatorKind.OP_NEG:
			if (bChecked)
			{
				eXPRFLAG = EXPRFLAG.EXF_CHECKOVERFLOW;
			}
			item = UnaOpKind.Minus;
			item2 = ExpressionKind.Negate;
			break;
		case OperatorKind.OP_BITNOT:
			item = UnaOpKind.Tilde;
			item2 = ExpressionKind.BitwiseNot;
			break;
		case OperatorKind.OP_LOGNOT:
			item = UnaOpKind.Bang;
			item2 = ExpressionKind.LogicalNot;
			break;
		case OperatorKind.OP_POSTINC:
			eXPRFLAG = EXPRFLAG.EXF_OPERATOR;
			if (bChecked)
			{
				eXPRFLAG |= EXPRFLAG.EXF_CHECKOVERFLOW;
			}
			item = UnaOpKind.IncDec;
			item2 = ExpressionKind.Add;
			break;
		case OperatorKind.OP_PREINC:
			if (bChecked)
			{
				eXPRFLAG = EXPRFLAG.EXF_CHECKOVERFLOW;
			}
			item = UnaOpKind.IncDec;
			item2 = ExpressionKind.Add;
			break;
		case OperatorKind.OP_POSTDEC:
			eXPRFLAG = EXPRFLAG.EXF_OPERATOR;
			if (bChecked)
			{
				eXPRFLAG |= EXPRFLAG.EXF_CHECKOVERFLOW;
			}
			item = UnaOpKind.IncDec;
			item2 = ExpressionKind.Subtract;
			break;
		case OperatorKind.OP_PREDEC:
			if (bChecked)
			{
				eXPRFLAG = EXPRFLAG.EXF_CHECKOVERFLOW;
			}
			item = UnaOpKind.IncDec;
			item2 = ExpressionKind.Subtract;
			break;
		default:
			throw Error.InternalCompilerError();
		}
		return (item2, item, eXPRFLAG);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public Expr BindStandardUnaryOperator(OperatorKind op, Expr pArgument)
	{
		CType type = pArgument.Type;
		if (type is NullableType { UnderlyingType: var underlyingType } nullableType && underlyingType.IsEnumType)
		{
			return mustCast(BindStandardUnaryOperator(op, mustCast(pArgument, TypeManager.GetNullable(GetPredefindType(underlyingType.FundamentalType switch
			{
				FUNDTYPE.FT_U4 => PredefinedType.PT_UINT, 
				FUNDTYPE.FT_I8 => PredefinedType.PT_LONG, 
				FUNDTYPE.FT_U8 => PredefinedType.PT_ULONG, 
				_ => PredefinedType.PT_INT, 
			})))), nullableType);
		}
		(ExpressionKind, UnaOpKind, EXPRFLAG) tuple = CalculateExprAndUnaryOpKinds(op, Context.Checked);
		ExpressionKind item = tuple.Item1;
		UnaOpKind item2 = tuple.Item2;
		EXPRFLAG item3 = tuple.Item3;
		UnaOpMask unaryOpMask = (UnaOpMask)(1 << (int)item2);
		List<UnaOpFullSig> list = new List<UnaOpFullSig>();
		UnaryOperatorSignatureFindResult unaryOperatorSignatureFindResult = PopulateSignatureList(pArgument, item2, unaryOpMask, item, item3, list, out var ppResult);
		int num = list.Count - 1;
		switch (unaryOperatorSignatureFindResult)
		{
		case UnaryOperatorSignatureFindResult.Return:
			return ppResult;
		default:
			if (!FindApplicableSignatures(pArgument, unaryOpMask, list))
			{
				if (list.Count == 0)
				{
					throw BadOperatorTypesError(pArgument, null);
				}
				num = 0;
				if (list.Count == 1)
				{
					break;
				}
				for (int i = 1; i < list.Count; i++)
				{
					if (num < 0)
					{
						num = i;
						continue;
					}
					int num2 = WhichUofsIsBetter(list[num], list[i], type);
					if (num2 == 0)
					{
						num = -1;
					}
					else if (num2 > 0)
					{
						num = i;
					}
				}
				if (num < 0)
				{
					throw AmbiguousOperatorError(pArgument, null);
				}
				for (int j = 0; j < list.Count; j++)
				{
					if (j != num && WhichUofsIsBetter(list[num], list[j], type) >= 0)
					{
						throw AmbiguousOperatorError(pArgument, null);
					}
				}
			}
			else
			{
				num = list.Count - 1;
			}
			break;
		case UnaryOperatorSignatureFindResult.Match:
			break;
		}
		UnaOpFullSig unaOpFullSig = list[num];
		if (unaOpFullSig.pfn == null)
		{
			if (item2 == UnaOpKind.IncDec)
			{
				return BindIncOp(item, item3, pArgument, unaOpFullSig);
			}
			throw BadOperatorTypesError(pArgument, null);
		}
		if (unaOpFullSig.isLifted())
		{
			return BindLiftedStandardUnop(item, item3, pArgument, unaOpFullSig);
		}
		if (pArgument is ExprConstant)
		{
			pArgument = ExprFactory.CreateCast(pArgument.Type, pArgument);
		}
		Expr op2 = tryConvert(pArgument, unaOpFullSig.GetType()) ?? mustCast(pArgument, unaOpFullSig.GetType(), CONVERTTYPE.NOUDC);
		return unaOpFullSig.pfn(this, item, item3, op2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private UnaryOperatorSignatureFindResult PopulateSignatureList(Expr pArgument, UnaOpKind unaryOpKind, UnaOpMask unaryOpMask, ExpressionKind exprKind, EXPRFLAG flags, List<UnaOpFullSig> pSignatures, out Expr ppResult)
	{
		ppResult = null;
		CType type = pArgument.Type;
		CType cType = type.StripNubs();
		if ((cType.IsPredefined ? cType.PredefinedType : PredefinedType.PT_COUNT) > PredefinedType.PT_ULONG)
		{
			if (cType.IsEnumType)
			{
				if ((unaryOpMask & (UnaOpMask.Tilde | UnaOpMask.IncDec)) != UnaOpMask.None)
				{
					if (unaryOpKind == UnaOpKind.Tilde)
					{
						pSignatures.Add(new UnaOpFullSig(((AggregateType)type).OwningAggregate.GetUnderlyingType(), BindEnumUnaOp, LiftFlags.None, UnaOpFuncKind.EnumUnaOp));
					}
					else
					{
						pSignatures.Add(new UnaOpFullSig(((AggregateType)type).OwningAggregate.GetUnderlyingType(), null, LiftFlags.None, UnaOpFuncKind.None));
					}
					return UnaryOperatorSignatureFindResult.Match;
				}
			}
			else if (unaryOpKind == UnaOpKind.IncDec)
			{
				ExprMultiGet exprMultiGet = ExprFactory.CreateMultiGet((EXPRFLAG)0, type, null);
				Expr expr = bindUDUnop(exprKind - 42 + 33, exprMultiGet);
				if (expr != null)
				{
					if (expr.Type != null && expr.Type != type)
					{
						expr = mustConvert(expr, type);
					}
					ExprMulti exprMulti = (exprMultiGet.OptionalMulti = ExprFactory.CreateMulti(EXPRFLAG.EXF_ASSGOP | flags, type, pArgument, expr));
					CheckLvalue(pArgument, CheckLvalueKind.Increment);
					ppResult = exprMulti;
					return UnaryOperatorSignatureFindResult.Return;
				}
			}
			else
			{
				Expr expr2 = bindUDUnop(exprKind, pArgument);
				if (expr2 != null)
				{
					ppResult = expr2;
					return UnaryOperatorSignatureFindResult.Return;
				}
			}
		}
		return UnaryOperatorSignatureFindResult.Continue;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool FindApplicableSignatures(Expr pArgument, UnaOpMask unaryOpMask, List<UnaOpFullSig> pSignatures)
	{
		long num = 0L;
		CType type = pArgument.Type;
		CType cType = type.StripNubs();
		PredefinedType ptSrc = (type.IsPredefined ? type.PredefinedType : PredefinedType.PT_COUNT);
		PredefinedType ptSrc2 = (cType.IsPredefined ? cType.PredefinedType : PredefinedType.PT_COUNT);
		for (int i = 0; i < s_rguos.Length; i++)
		{
			UnaOpSig unaOpSig = s_rguos[i];
			if ((unaOpSig.grfuom & unaryOpMask) == 0)
			{
				continue;
			}
			ConvKind convKind = GetConvKind(ptSrc, s_rguos[i].pt);
			CType cType2 = null;
			switch (convKind)
			{
			case ConvKind.Explicit:
				if (!(pArgument is ExprConstant))
				{
					continue;
				}
				if (!canConvert(pArgument, cType2 = GetPredefindType(unaOpSig.pt)))
				{
					if (i < num)
					{
						continue;
					}
					cType2 = TypeManager.GetNullable(cType2);
					if (!canConvert(pArgument, cType2))
					{
						continue;
					}
				}
				break;
			case ConvKind.Unknown:
				if (!canConvert(pArgument, cType2 = GetPredefindType(unaOpSig.pt)))
				{
					if (i < num)
					{
						continue;
					}
					cType2 = TypeManager.GetNullable(cType2);
					if (!canConvert(pArgument, cType2))
					{
						continue;
					}
				}
				break;
			case ConvKind.Identity:
			{
				UnaOpFullSig unaOpFullSig = new UnaOpFullSig(unaOpSig);
				if (unaOpFullSig.GetType() != null)
				{
					pSignatures.Add(unaOpFullSig);
					return true;
				}
				break;
			}
			case ConvKind.Implicit:
				break;
			default:
				continue;
			}
			if (cType2 is NullableType)
			{
				LiftFlags liftFlags = LiftFlags.None;
				ConvKind convKind2 = GetConvKind(ptSrc2, unaOpSig.pt);
				liftFlags = (((uint)(convKind2 - 1) <= 1u) ? (liftFlags | LiftFlags.Lift1) : (liftFlags | LiftFlags.Convert1));
				pSignatures.Add(new UnaOpFullSig(cType2, unaOpSig.pfn, liftFlags, unaOpSig.fnkind));
				num = i + unaOpSig.cuosSkip + 1;
			}
			else
			{
				UnaOpFullSig unaOpFullSig2 = new UnaOpFullSig(unaOpSig);
				if (unaOpFullSig2.GetType() != null)
				{
					pSignatures.Add(unaOpFullSig2);
				}
				i += unaOpSig.cuosSkip;
			}
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprOperator BindLiftedStandardUnop(ExpressionKind ek, EXPRFLAG flags, Expr arg, UnaOpFullSig uofs)
	{
		NullableType nullableType = uofs.GetType() as NullableType;
		if (arg.Type is NullType)
		{
			throw BadOperatorTypesError(arg, null);
		}
		LiftArgument(arg, uofs.GetType(), uofs.Convert(), out var ppLiftedArgument, out var ppNonLiftedArgument);
		Expr expr = uofs.pfn(this, ek, flags, ppNonLiftedArgument);
		ExprUnaryOp exprUnaryOp = ExprFactory.CreateUnaryOp(ek, nullableType, ppLiftedArgument);
		mustCast(expr, nullableType, (CONVERTTYPE)0);
		exprUnaryOp.Flags |= flags;
		return exprUnaryOp;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private int WhichUofsIsBetter(UnaOpFullSig uofs1, UnaOpFullSig uofs2, CType typeArg)
	{
		return ((!uofs1.FPreDef() || !uofs2.FPreDef()) ? WhichTypeIsBetter(uofs1.GetType(), uofs2.GetType(), typeArg) : WhichTypeIsBetter(uofs1.pt, uofs2.pt, typeArg)) switch
		{
			BetterType.Left => -1, 
			BetterType.Right => 1, 
			_ => 0, 
		};
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprOperator BindIntBinOp(ExpressionBinder binder, ExpressionKind ek, EXPRFLAG flags, Expr arg1, Expr arg2)
	{
		return binder.BindIntOp(ek, flags, arg1, arg2, arg1.Type.PredefinedType);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprOperator BindIntUnaOp(ExpressionBinder binder, ExpressionKind ek, EXPRFLAG flags, Expr arg)
	{
		return binder.BindIntOp(ek, flags, arg, null, arg.Type.PredefinedType);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprOperator BindRealBinOp(ExpressionBinder binder, ExpressionKind ek, EXPRFLAG _, Expr arg1, Expr arg2)
	{
		return BindFloatOp(ek, arg1, arg2);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprOperator BindRealUnaOp(ExpressionBinder binder, ExpressionKind ek, EXPRFLAG _, Expr arg)
	{
		return BindFloatOp(ek, arg, null);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr BindIncOp(ExpressionKind ek, EXPRFLAG flags, Expr arg, UnaOpFullSig uofs)
	{
		CheckLvalue(arg, CheckLvalueKind.Increment);
		FUNDTYPE fundamentalType = uofs.GetType().StripNubs().FundamentalType;
		if (fundamentalType == FUNDTYPE.FT_R8 || fundamentalType == FUNDTYPE.FT_R4)
		{
			flags &= ~EXPRFLAG.EXF_CHECKOVERFLOW;
		}
		if (uofs.isLifted())
		{
			return BindLiftedIncOp(ek, flags, arg, uofs);
		}
		return BindNonliftedIncOp(ek, flags, arg, uofs);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr BindIncOpCore(ExpressionKind ek, EXPRFLAG flags, Expr exprVal, CType type)
	{
		if (type.IsEnumType && type.FundamentalType > FUNDTYPE.FT_U8)
		{
			type = GetPredefindType(PredefinedType.PT_INT);
		}
		ConstVal cv;
		switch (type.FundamentalType)
		{
		default:
		{
			PREDEFMETH predefMeth;
			if (ek == ExpressionKind.Add)
			{
				ek = ExpressionKind.DecimalInc;
				predefMeth = PREDEFMETH.PM_DECIMAL_OPINCREMENT;
			}
			else
			{
				ek = ExpressionKind.DecimalDec;
				predefMeth = PREDEFMETH.PM_DECIMAL_OPDECREMENT;
			}
			return CreateUnaryOpForPredefMethodCall(ek, predefMeth, type, exprVal);
		}
		case FUNDTYPE.FT_I1:
		case FUNDTYPE.FT_I2:
		case FUNDTYPE.FT_U1:
		case FUNDTYPE.FT_U2:
			type = GetPredefindType(PredefinedType.PT_INT);
			cv = ConstVal.Get(1);
			break;
		case FUNDTYPE.FT_I4:
		case FUNDTYPE.FT_U4:
			cv = ConstVal.Get(1);
			break;
		case FUNDTYPE.FT_I8:
		case FUNDTYPE.FT_U8:
			cv = ConstVal.Get(1L);
			break;
		case FUNDTYPE.FT_R4:
		case FUNDTYPE.FT_R8:
			cv = ConstVal.Get(1.0);
			break;
		}
		return LScalar(ek, flags, exprVal, type, cv, type);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr LScalar(ExpressionKind ek, EXPRFLAG flags, Expr exprVal, CType type, ConstVal cv, CType typeTmp)
	{
		CType cType = type;
		if (cType.IsEnumType)
		{
			cType = cType.UnderlyingEnumType;
		}
		ExprBinOp exprBinOp = ExprFactory.CreateBinop(ek, typeTmp, exprVal, ExprFactory.CreateConstant(cType, cv));
		exprBinOp.Flags |= flags;
		if (typeTmp == type)
		{
			return exprBinOp;
		}
		return mustCast(exprBinOp, type, CONVERTTYPE.NOUDC);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprMulti BindNonliftedIncOp(ExpressionKind ek, EXPRFLAG flags, Expr arg, UnaOpFullSig uofs)
	{
		Expr expr2;
		Expr expr = (expr2 = ExprFactory.CreateMultiGet(EXPRFLAG.EXF_ASSGOP, arg.Type, null));
		CType type = uofs.GetType();
		expr2 = mustCast(expr2, type);
		expr2 = BindIncOpCore(ek, flags, expr2, type);
		return ((ExprMultiGet)expr).OptionalMulti = ExprFactory.CreateMulti(op: mustCast(expr2, arg.Type, CONVERTTYPE.NOUDC), flags: EXPRFLAG.EXF_ASSGOP | flags, type: arg.Type, left: arg);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprMulti BindLiftedIncOp(ExpressionKind ek, EXPRFLAG flags, Expr arg, UnaOpFullSig uofs)
	{
		NullableType nullableType = uofs.GetType() as NullableType;
		Expr expr2;
		Expr expr = (expr2 = ExprFactory.CreateMultiGet(EXPRFLAG.EXF_ASSGOP, arg.Type, null));
		Expr expr3 = expr2;
		expr3 = mustCast(expr3, nullableType.UnderlyingType);
		Expr expr4 = BindIncOpCore(ek, flags, expr3, nullableType.UnderlyingType);
		ExprUnaryOp exprUnaryOp = ExprFactory.CreateUnaryOp(operand: mustCast(expr2, nullableType), exprKind: (ek == ExpressionKind.Add) ? ExpressionKind.Inc : ExpressionKind.Dec, type: arg.Type);
		mustCast(mustCast(expr4, nullableType), arg.Type);
		exprUnaryOp.Flags |= flags;
		return ((ExprMultiGet)expr).OptionalMulti = ExprFactory.CreateMulti(EXPRFLAG.EXF_ASSGOP | flags, arg.Type, arg, exprUnaryOp);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "All types used here are builtin and will not be trimmed.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprBinOp BindDecBinOp(ExpressionBinder _, ExpressionKind ek, EXPRFLAG flags, Expr arg1, Expr arg2)
	{
		CType predefindType = GetPredefindType(PredefinedType.PT_DECIMAL);
		CType type;
		switch (ek)
		{
		default:
			type = null;
			break;
		case ExpressionKind.Add:
		case ExpressionKind.Subtract:
		case ExpressionKind.Multiply:
		case ExpressionKind.Divide:
		case ExpressionKind.Modulo:
			type = predefindType;
			break;
		case ExpressionKind.Eq:
		case ExpressionKind.NotEq:
		case ExpressionKind.LessThan:
		case ExpressionKind.LessThanOrEqual:
		case ExpressionKind.GreaterThan:
		case ExpressionKind.GreaterThanOrEqual:
			type = GetPredefindType(PredefinedType.PT_BOOL);
			break;
		}
		return ExprFactory.CreateBinop(ek, type, arg1, arg2);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "All types used here are builtin and will not be trimmed.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprUnaryOp BindDecUnaOp(ExpressionBinder _, ExpressionKind ek, EXPRFLAG flags, Expr arg)
	{
		CType predefindType = GetPredefindType(PredefinedType.PT_DECIMAL);
		if (ek == ExpressionKind.Negate)
		{
			PREDEFMETH predefMeth = PREDEFMETH.PM_DECIMAL_OPUNARYMINUS;
			return CreateUnaryOpForPredefMethodCall(ExpressionKind.DecimalNegate, predefMeth, predefindType, arg);
		}
		return ExprFactory.CreateUnaryOp(ExpressionKind.UnaryPlus, predefindType, arg);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static Expr BindStrBinOp(ExpressionBinder _, ExpressionKind ek, EXPRFLAG flags, Expr arg1, Expr arg2)
	{
		return BindStringConcat(arg1, arg2);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprBinOp BindShiftOp(ExpressionBinder _, ExpressionKind ek, EXPRFLAG flags, Expr arg1, Expr arg2)
	{
		PredefinedType predefinedType = arg1.Type.PredefinedType;
		return ExprFactory.CreateBinop(ek, arg1.Type, arg1, arg2);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "All types used here are builtin and will not be trimmed.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprBinOp BindBoolBinOp(ExpressionBinder _, ExpressionKind ek, EXPRFLAG flags, Expr arg1, Expr arg2)
	{
		return ExprFactory.CreateBinop(ek, GetPredefindType(PredefinedType.PT_BOOL), arg1, arg2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprOperator BindBoolBitwiseOp(ExpressionKind ek, EXPRFLAG flags, Expr expr1, Expr expr2)
	{
		if (expr1.Type is NullableType || expr2.Type is NullableType)
		{
			CType nullable = TypeManager.GetNullable(GetPredefindType(PredefinedType.PT_BOOL));
			Expr expr3 = StripNullableConstructor(expr1);
			Expr expr4 = StripNullableConstructor(expr2);
			Expr expr5 = null;
			if (!(expr3.Type is NullableType) && !(expr4.Type is NullableType))
			{
				expr5 = BindBoolBinOp(this, ek, flags, expr3, expr4);
			}
			ExprBinOp exprBinOp = ExprFactory.CreateBinop(ek, nullable, expr1, expr2);
			if (expr5 != null)
			{
				mustCast(expr5, nullable, (CONVERTTYPE)0);
			}
			exprBinOp.IsLifted = true;
			exprBinOp.Flags |= flags;
			return exprBinOp;
		}
		return BindBoolBinOp(this, ek, flags, expr1, expr2);
	}

	private static Expr BindLiftedBoolBitwiseOp(ExpressionBinder _, ExpressionKind ek, EXPRFLAG flags, Expr expr1, Expr expr2)
	{
		return null;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "All types used here are builtin and will not be trimmed.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static Expr BindBoolUnaOp(ExpressionBinder _, ExpressionKind ek, EXPRFLAG flags, Expr arg)
	{
		CType predefindType = GetPredefindType(PredefinedType.PT_BOOL);
		Expr expr = arg.GetConst();
		if (expr == null)
		{
			return ExprFactory.CreateUnaryOp(ExpressionKind.LogicalNot, predefindType, arg);
		}
		return ExprFactory.CreateConstant(predefindType, ConstVal.Get(((ExprConstant)expr).Val.Int32Val == 0));
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "All types used here are builtin and will not be trimmed.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprBinOp BindStrCmpOp(ExpressionBinder _, ExpressionKind ek, EXPRFLAG flags, Expr arg1, Expr arg2)
	{
		PREDEFMETH predefMeth = ((ek == ExpressionKind.Eq) ? PREDEFMETH.PM_STRING_OPEQUALITY : PREDEFMETH.PM_STRING_OPINEQUALITY);
		ek = ((ek == ExpressionKind.Eq) ? ExpressionKind.StringEq : ExpressionKind.StringNotEq);
		return CreateBinopForPredefMethodCall(ek, predefMeth, GetPredefindType(PredefinedType.PT_BOOL), arg1, arg2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprBinOp BindRefCmpOp(ExpressionBinder binder, ExpressionKind ek, EXPRFLAG flags, Expr arg1, Expr arg2)
	{
		arg1 = binder.mustConvert(arg1, GetPredefindType(PredefinedType.PT_OBJECT), CONVERTTYPE.NOUDC);
		arg2 = binder.mustConvert(arg2, GetPredefindType(PredefinedType.PT_OBJECT), CONVERTTYPE.NOUDC);
		return ExprFactory.CreateBinop(ek, GetPredefindType(PredefinedType.PT_BOOL), arg1, arg2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static Expr BindDelBinOp(ExpressionBinder _, ExpressionKind ek, EXPRFLAG flags, Expr arg1, Expr arg2)
	{
		PREDEFMETH predefMeth = PREDEFMETH.PM_DECIMAL_OPDECREMENT;
		CType retType = null;
		switch (ek)
		{
		case ExpressionKind.Add:
			predefMeth = PREDEFMETH.PM_DELEGATE_COMBINE;
			retType = arg1.Type;
			ek = ExpressionKind.DelegateAdd;
			break;
		case ExpressionKind.Subtract:
			predefMeth = PREDEFMETH.PM_DELEGATE_REMOVE;
			retType = arg1.Type;
			ek = ExpressionKind.DelegateSubtract;
			break;
		case ExpressionKind.Eq:
			predefMeth = PREDEFMETH.PM_DELEGATE_OPEQUALITY;
			retType = GetPredefindType(PredefinedType.PT_BOOL);
			ek = ExpressionKind.DelegateEq;
			break;
		case ExpressionKind.NotEq:
			predefMeth = PREDEFMETH.PM_DELEGATE_OPINEQUALITY;
			retType = GetPredefindType(PredefinedType.PT_BOOL);
			ek = ExpressionKind.DelegateNotEq;
			break;
		}
		return CreateBinopForPredefMethodCall(ek, predefMeth, retType, arg1, arg2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static Expr BindEnumBinOp(ExpressionBinder binder, ExpressionKind ek, EXPRFLAG flags, Expr arg1, Expr arg2)
	{
		AggregateType enumBinOpType = GetEnumBinOpType(ek, arg1.Type, arg2.Type, out var ppEnumType);
		PredefinedType predefinedType = ppEnumType.FundamentalType switch
		{
			FUNDTYPE.FT_U4 => PredefinedType.PT_UINT, 
			FUNDTYPE.FT_I8 => PredefinedType.PT_LONG, 
			FUNDTYPE.FT_U8 => PredefinedType.PT_ULONG, 
			_ => PredefinedType.PT_INT, 
		};
		CType predefindType = GetPredefindType(predefinedType);
		arg1 = binder.mustCast(arg1, predefindType, CONVERTTYPE.NOUDC);
		arg2 = binder.mustCast(arg2, predefindType, CONVERTTYPE.NOUDC);
		Expr expr = binder.BindIntOp(ek, flags, arg1, arg2, predefinedType);
		if (expr.Type != enumBinOpType)
		{
			expr = binder.mustCast(expr, enumBinOpType, CONVERTTYPE.NOUDC);
		}
		return expr;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr BindLiftedEnumArithmeticBinOp(ExpressionKind ek, EXPRFLAG flags, Expr arg1, Expr arg2)
	{
		CType cType = ((arg1.Type is NullableType nullableType) ? nullableType.UnderlyingType : arg1.Type);
		CType cType2 = ((arg2.Type is NullableType nullableType2) ? nullableType2.UnderlyingType : arg2.Type);
		if (cType is NullType)
		{
			cType = cType2.UnderlyingEnumType;
		}
		else if (cType2 is NullType)
		{
			cType2 = cType.UnderlyingEnumType;
		}
		NullableType nullable = TypeManager.GetNullable(GetEnumBinOpType(ek, cType, cType2, out var ppEnumType));
		NullableType nullable2 = TypeManager.GetNullable(GetPredefindType(ppEnumType.FundamentalType switch
		{
			FUNDTYPE.FT_U4 => PredefinedType.PT_UINT, 
			FUNDTYPE.FT_I8 => PredefinedType.PT_LONG, 
			FUNDTYPE.FT_U8 => PredefinedType.PT_ULONG, 
			_ => PredefinedType.PT_INT, 
		}));
		arg1 = mustCast(arg1, nullable2, CONVERTTYPE.NOUDC);
		arg2 = mustCast(arg2, nullable2, CONVERTTYPE.NOUDC);
		ExprBinOp exprBinOp = ExprFactory.CreateBinop(ek, nullable2, arg1, arg2);
		exprBinOp.IsLifted = true;
		exprBinOp.Flags |= flags;
		if (exprBinOp.Type != nullable)
		{
			return mustCast(exprBinOp, nullable, CONVERTTYPE.NOUDC);
		}
		return exprBinOp;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static Expr BindEnumUnaOp(ExpressionBinder binder, ExpressionKind ek, EXPRFLAG flags, Expr arg)
	{
		CType type = ((ExprCast)arg).Argument.Type;
		PredefinedType predefinedType = type.FundamentalType switch
		{
			FUNDTYPE.FT_U4 => PredefinedType.PT_UINT, 
			FUNDTYPE.FT_I8 => PredefinedType.PT_LONG, 
			FUNDTYPE.FT_U8 => PredefinedType.PT_ULONG, 
			_ => PredefinedType.PT_INT, 
		};
		CType predefindType = GetPredefindType(predefinedType);
		arg = binder.mustCast(arg, predefindType, CONVERTTYPE.NOUDC);
		Expr expr = binder.BindIntOp(ek, flags, arg, null, predefinedType);
		return binder.MustCastInUncheckedContext(expr, type, CONVERTTYPE.NOUDC);
	}

	private (BinOpKind, EXPRFLAG) GetBinopKindAndFlags(ExpressionKind ek)
	{
		EXPRFLAG eXPRFLAG = (EXPRFLAG)0;
		BinOpKind item;
		switch (ek)
		{
		case ExpressionKind.Add:
			if (Context.Checked)
			{
				eXPRFLAG = EXPRFLAG.EXF_CHECKOVERFLOW;
			}
			item = BinOpKind.Add;
			break;
		case ExpressionKind.Subtract:
			if (Context.Checked)
			{
				eXPRFLAG = EXPRFLAG.EXF_CHECKOVERFLOW;
			}
			item = BinOpKind.Sub;
			break;
		case ExpressionKind.Divide:
		case ExpressionKind.Modulo:
			eXPRFLAG = EXPRFLAG.EXF_ASSGOP;
			if (Context.Checked)
			{
				eXPRFLAG |= EXPRFLAG.EXF_CHECKOVERFLOW;
			}
			item = BinOpKind.Mul;
			break;
		case ExpressionKind.Multiply:
			if (Context.Checked)
			{
				eXPRFLAG = EXPRFLAG.EXF_CHECKOVERFLOW;
			}
			item = BinOpKind.Mul;
			break;
		case ExpressionKind.BitwiseAnd:
		case ExpressionKind.BitwiseOr:
			item = BinOpKind.Bitwise;
			break;
		case ExpressionKind.BitwiseExclusiveOr:
			item = BinOpKind.BitXor;
			break;
		case ExpressionKind.LeftShirt:
		case ExpressionKind.RightShift:
			item = BinOpKind.Shift;
			break;
		case ExpressionKind.LogicalAnd:
		case ExpressionKind.LogicalOr:
			item = BinOpKind.Logical;
			break;
		case ExpressionKind.LessThan:
		case ExpressionKind.LessThanOrEqual:
		case ExpressionKind.GreaterThan:
		case ExpressionKind.GreaterThanOrEqual:
			item = BinOpKind.Compare;
			break;
		case ExpressionKind.Eq:
		case ExpressionKind.NotEq:
			item = BinOpKind.Equal;
			break;
		default:
			throw Error.InternalCompilerError();
		}
		return (item, eXPRFLAG);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "All types used here are builtin and will not be trimmed.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprOperator BindIntOp(ExpressionKind kind, EXPRFLAG flags, Expr op1, Expr op2, PredefinedType ptOp)
	{
		CType predefindType = GetPredefindType(ptOp);
		if (kind == ExpressionKind.Negate)
		{
			return BindIntegerNeg(flags, op1, ptOp);
		}
		CType type = (kind.IsRelational() ? GetPredefindType(PredefinedType.PT_BOOL) : predefindType);
		ExprOperator exprOperator = ExprFactory.CreateOperator(kind, type, op1, op2);
		exprOperator.Flags |= flags;
		return exprOperator;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "All types used here are builtin and will not be trimmed.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprOperator BindIntegerNeg(EXPRFLAG flags, Expr op, PredefinedType ptOp)
	{
		GetPredefindType(ptOp);
		switch (ptOp)
		{
		case PredefinedType.PT_ULONG:
			throw BadOperatorTypesError(op, null);
		case PredefinedType.PT_UINT:
			if (op.Type.FundamentalType == FUNDTYPE.FT_U4)
			{
				op = mustConvertCore(op, GetPredefindType(PredefinedType.PT_LONG), CONVERTTYPE.NOUDC);
			}
			break;
		}
		return ExprFactory.CreateNeg(flags, op);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "All types used here are builtin and will not be trimmed.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprOperator BindFloatOp(ExpressionKind kind, Expr op1, Expr op2)
	{
		CType type = (kind.IsRelational() ? GetPredefindType(PredefinedType.PT_BOOL) : op1.Type);
		ExprOperator exprOperator = ExprFactory.CreateOperator(kind, type, op1, op2);
		exprOperator.Flags &= ~EXPRFLAG.EXF_CHECKOVERFLOW;
		return exprOperator;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "All types used here are builtin and will not be trimmed.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprConcat BindStringConcat(Expr op1, Expr op2)
	{
		return ExprFactory.CreateConcat(op1, op2);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static RuntimeBinderException AmbiguousOperatorError(Expr op1, Expr op2)
	{
		string errorString = op1.ErrorString;
		if (op2 == null)
		{
			return ErrorHandling.Error(ErrorCode.ERR_AmbigUnaryOp, errorString, op1.Type);
		}
		return ErrorHandling.Error(ErrorCode.ERR_AmbigBinaryOps, errorString, op1.Type, op2.Type);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr BindUserBoolOp(ExpressionKind kind, ExprCall pCall)
	{
		CType type = pCall.Type;
		if (!TypeManager.SubstEqualTypes(type, pCall.MethWithInst.Meth().Params[0], type) || !TypeManager.SubstEqualTypes(type, pCall.MethWithInst.Meth().Params[1], type))
		{
			throw ErrorHandling.Error(ErrorCode.ERR_BadBoolOp, pCall.MethWithInst);
		}
		ExprList obj = (ExprList)pCall.OptionalArguments;
		ExprWrap exprWrap = (ExprWrap)(obj.OptionalElement = WrapShortLivedExpression(obj.OptionalElement));
		SymbolTable.PopulateSymbolTableWithName("op_True", null, exprWrap.Type.AssociatedSystemType);
		SymbolTable.PopulateSymbolTableWithName("op_False", null, exprWrap.Type.AssociatedSystemType);
		Expr expr2 = bindUDUnop(ExpressionKind.True, exprWrap);
		Expr expr3 = bindUDUnop(ExpressionKind.False, exprWrap);
		if (expr2 == null || expr3 == null)
		{
			throw ErrorHandling.Error(ErrorCode.ERR_MustHaveOpTF, type);
		}
		expr2 = mustConvert(expr2, GetPredefindType(PredefinedType.PT_BOOL));
		expr3 = mustConvert(expr3, GetPredefindType(PredefinedType.PT_BOOL));
		return ExprFactory.CreateUserLogOp(type, (kind == ExpressionKind.LogicalAnd) ? expr3 : expr2, pCall);
	}

	private static AggregateType GetUserDefinedBinopArgumentType(CType type)
	{
		if (type is NullableType nullableType)
		{
			type = nullableType.UnderlyingType;
		}
		if (type is AggregateType aggregateType && (aggregateType.IsClassType || aggregateType.IsStructType) && !aggregateType.OwningAggregate.IsSkipUDOps())
		{
			return aggregateType;
		}
		return null;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static int GetUserDefinedBinopArgumentTypes(CType type1, CType type2, AggregateType[] rgats)
	{
		int num = 0;
		rgats[0] = GetUserDefinedBinopArgumentType(type1);
		if (rgats[0] != null)
		{
			num++;
		}
		rgats[num] = GetUserDefinedBinopArgumentType(type2);
		if (rgats[num] != null)
		{
			num++;
		}
		if (num == 2 && rgats[0] == rgats[1])
		{
			num = 1;
		}
		return num;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool UserDefinedBinaryOperatorCanBeLifted(ExpressionKind ek, MethodSymbol method, AggregateType ats, TypeArray Params)
	{
		if (!Params[0].IsNonNullableValueType || !Params[1].IsNonNullableValueType)
		{
			return false;
		}
		CType cType = TypeManager.SubstType(method.RetType, ats);
		if (!cType.IsNonNullableValueType)
		{
			return false;
		}
		switch (ek)
		{
		case ExpressionKind.Eq:
		case ExpressionKind.NotEq:
			if (!cType.IsPredefType(PredefinedType.PT_BOOL))
			{
				return false;
			}
			if (Params[0] != Params[1])
			{
				return false;
			}
			return true;
		case ExpressionKind.LessThan:
		case ExpressionKind.LessThanOrEqual:
		case ExpressionKind.GreaterThan:
		case ExpressionKind.GreaterThanOrEqual:
			if (!cType.IsPredefType(PredefinedType.PT_BOOL))
			{
				return false;
			}
			return true;
		default:
			return true;
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool UserDefinedBinaryOperatorIsApplicable(List<CandidateFunctionMember> candidateList, ExpressionKind ek, MethodSymbol method, AggregateType ats, Expr arg1, Expr arg2, bool fDontLift)
	{
		if (!method.isOperator || method.Params.Count != 2)
		{
			return false;
		}
		TypeArray typeArray = TypeManager.SubstTypeArray(method.Params, ats);
		if (canConvert(arg1, typeArray[0]) && canConvert(arg2, typeArray[1]))
		{
			candidateList.Add(new CandidateFunctionMember(new MethPropWithInst(method, ats, TypeArray.Empty), typeArray, 0, fExpanded: false));
			return true;
		}
		if (fDontLift || !UserDefinedBinaryOperatorCanBeLifted(ek, method, ats, typeArray))
		{
			return false;
		}
		CType[] array = new CType[2]
		{
			TypeManager.GetNullable(typeArray[0]),
			TypeManager.GetNullable(typeArray[1])
		};
		if (!canConvert(arg1, array[0]) || !canConvert(arg2, array[1]))
		{
			return false;
		}
		candidateList.Add(new CandidateFunctionMember(new MethPropWithInst(method, ats, TypeArray.Empty), TypeArray.Allocate(array), 2, fExpanded: false));
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool GetApplicableUserDefinedBinaryOperatorCandidates(List<CandidateFunctionMember> candidateList, ExpressionKind ek, AggregateType type, Expr arg1, Expr arg2, bool fDontLift)
	{
		Name name = ExpressionKindName(ek);
		bool result = false;
		for (MethodSymbol methodSymbol = SymbolLoader.LookupAggMember(name, type.OwningAggregate, symbmask_t.MASK_MethodSymbol) as MethodSymbol; methodSymbol != null; methodSymbol = methodSymbol.LookupNext(symbmask_t.MASK_MethodSymbol) as MethodSymbol)
		{
			if (UserDefinedBinaryOperatorIsApplicable(candidateList, ek, methodSymbol, type, arg1, arg2, fDontLift))
			{
				result = true;
			}
		}
		return result;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private AggregateType GetApplicableUserDefinedBinaryOperatorCandidatesInBaseTypes(List<CandidateFunctionMember> candidateList, ExpressionKind ek, AggregateType type, Expr arg1, Expr arg2, bool fDontLift, AggregateType atsStop)
	{
		AggregateType aggregateType = type;
		while (aggregateType != null && aggregateType != atsStop)
		{
			if (GetApplicableUserDefinedBinaryOperatorCandidates(candidateList, ek, aggregateType, arg1, arg2, fDontLift))
			{
				return aggregateType;
			}
			aggregateType = aggregateType.BaseClass;
		}
		return null;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprCall BindUDBinop(ExpressionKind ek, Expr arg1, Expr arg2, bool fDontLift, out MethPropWithInst ppmpwi)
	{
		List<CandidateFunctionMember> list = new List<CandidateFunctionMember>();
		ppmpwi = null;
		AggregateType[] array = new AggregateType[2];
		switch (GetUserDefinedBinopArgumentTypes(arg1.Type, arg2.Type, array))
		{
		case 0:
			return null;
		case 1:
			GetApplicableUserDefinedBinaryOperatorCandidatesInBaseTypes(list, ek, array[0], arg1, arg2, fDontLift, null);
			break;
		default:
		{
			AggregateType applicableUserDefinedBinaryOperatorCandidatesInBaseTypes = GetApplicableUserDefinedBinaryOperatorCandidatesInBaseTypes(list, ek, array[0], arg1, arg2, fDontLift, null);
			GetApplicableUserDefinedBinaryOperatorCandidatesInBaseTypes(list, ek, array[1], arg1, arg2, fDontLift, applicableUserDefinedBinaryOperatorCandidatesInBaseTypes);
			break;
		}
		}
		if (list.IsEmpty())
		{
			return null;
		}
		ExprList args = ExprFactory.CreateList(arg1, arg2);
		ArgInfos argInfos = new ArgInfos();
		argInfos.carg = 2;
		FillInArgInfoFromArgList(argInfos, args);
		CandidateFunctionMember candidateFunctionMember = FindBestMethod(list, null, argInfos, out var methAmbig, out var methAmbig2);
		if (candidateFunctionMember == null)
		{
			throw ErrorHandling.Error(ErrorCode.ERR_AmbigCall, methAmbig.mpwi, methAmbig2.mpwi);
		}
		ppmpwi = candidateFunctionMember.mpwi;
		if (candidateFunctionMember.ctypeLift != 0)
		{
			return BindLiftedUDBinop(ek, arg1, arg2, candidateFunctionMember.@params, candidateFunctionMember.mpwi);
		}
		CType typeRet = TypeManager.SubstType(candidateFunctionMember.mpwi.Meth().RetType, candidateFunctionMember.mpwi.GetType());
		return BindUDBinopCall(arg1, arg2, candidateFunctionMember.@params, typeRet, candidateFunctionMember.mpwi);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprCall BindUDBinopCall(Expr arg1, Expr arg2, TypeArray Params, CType typeRet, MethPropWithInst mpwi)
	{
		arg1 = mustConvert(arg1, Params[0]);
		arg2 = mustConvert(arg2, Params[1]);
		ExprList arguments = ExprFactory.CreateList(arg1, arg2);
		CheckUnsafe(arg1.Type);
		CheckUnsafe(arg2.Type);
		CheckUnsafe(typeRet);
		ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, mpwi);
		ExprCall exprCall = ExprFactory.CreateCall((EXPRFLAG)0, typeRet, arguments, memberGroup, null);
		exprCall.MethWithInst = new MethWithInst(mpwi);
		verifyMethodArgs(exprCall, mpwi.GetType());
		return exprCall;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprCall BindLiftedUDBinop(ExpressionKind ek, Expr arg1, Expr arg2, TypeArray Params, MethPropWithInst mpwi)
	{
		Expr expr = arg1;
		Expr expr2 = arg2;
		CType cType = TypeManager.SubstType(mpwi.Meth().RetType, mpwi.GetType());
		TypeArray typeArray = TypeManager.SubstTypeArray(mpwi.Meth().Params, mpwi.GetType());
		if (!canConvert(arg1.Type.StripNubs(), typeArray[0], CONVERTTYPE.NOUDC))
		{
			expr = mustConvert(arg1, Params[0]);
		}
		if (!canConvert(arg2.Type.StripNubs(), typeArray[1], CONVERTTYPE.NOUDC))
		{
			expr2 = mustConvert(arg2, Params[1]);
		}
		Expr arg3 = mustCast(expr, typeArray[0]);
		Expr arg4 = mustCast(expr2, typeArray[1]);
		CType cType2;
		switch (ek)
		{
		default:
			cType2 = TypeManager.GetNullable(cType);
			break;
		case ExpressionKind.Eq:
		case ExpressionKind.NotEq:
			cType2 = cType;
			break;
		case ExpressionKind.LessThan:
		case ExpressionKind.LessThanOrEqual:
		case ExpressionKind.GreaterThan:
		case ExpressionKind.GreaterThanOrEqual:
			cType2 = cType;
			break;
		}
		ExprCall expr3 = BindUDBinopCall(arg3, arg4, typeArray, cType, mpwi);
		ExprList arguments = ExprFactory.CreateList(expr, expr2);
		ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, mpwi);
		ExprCall exprCall = ExprFactory.CreateCall((EXPRFLAG)0, cType2, arguments, memberGroup, null);
		exprCall.MethWithInst = new MethWithInst(mpwi);
		ExprCall exprCall2 = exprCall;
		exprCall2.NullableCallLiftKind = ek switch
		{
			ExpressionKind.Eq => NullableCallLiftKind.EqualityOperator, 
			ExpressionKind.NotEq => NullableCallLiftKind.InequalityOperator, 
			_ => NullableCallLiftKind.Operator, 
		};
		exprCall.CastOfNonLiftedResultToLiftedType = mustCast(expr3, cType2, (CONVERTTYPE)0);
		return exprCall;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static AggregateType GetEnumBinOpType(ExpressionKind ek, CType argType1, CType argType2, out AggregateType ppEnumType)
	{
		AggregateType aggregateType = argType1 as AggregateType;
		AggregateType aggregateType2 = argType2 as AggregateType;
		AggregateType aggregateType3 = (aggregateType.IsEnumType ? aggregateType : aggregateType2);
		AggregateType result = aggregateType3;
		switch (ek)
		{
		case ExpressionKind.Subtract:
			if (aggregateType == aggregateType2)
			{
				result = aggregateType3.UnderlyingEnumType;
			}
			break;
		default:
			result = GetPredefindType(PredefinedType.PT_BOOL);
			break;
		case ExpressionKind.Add:
		case ExpressionKind.BitwiseAnd:
		case ExpressionKind.BitwiseOr:
		case ExpressionKind.BitwiseExclusiveOr:
			break;
		}
		ppEnumType = aggregateType3;
		return result;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprBinOp CreateBinopForPredefMethodCall(ExpressionKind ek, PREDEFMETH predefMeth, CType RetType, Expr arg1, Expr arg2)
	{
		MethodSymbol method = PredefinedMembers.GetMethod(predefMeth);
		ExprBinOp exprBinOp = ExprFactory.CreateBinop(ek, RetType, arg1, arg2);
		AggregateType aggregate = TypeManager.GetAggregate(method.getClass(), TypeArray.Empty);
		exprBinOp.PredefinedMethodToCall = new MethWithInst(method, aggregate, null);
		exprBinOp.UserDefinedCallMethod = exprBinOp.PredefinedMethodToCall;
		return exprBinOp;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprUnaryOp CreateUnaryOpForPredefMethodCall(ExpressionKind ek, PREDEFMETH predefMeth, CType pRetType, Expr pArg)
	{
		MethodSymbol method = PredefinedMembers.GetMethod(predefMeth);
		ExprUnaryOp exprUnaryOp = ExprFactory.CreateUnaryOp(ek, pRetType, pArg);
		AggregateType aggregate = TypeManager.GetAggregate(method.getClass(), TypeArray.Empty);
		exprUnaryOp.PredefinedMethodToCall = new MethWithInst(method, aggregate, null);
		exprUnaryOp.UserDefinedCallMethod = exprUnaryOp.PredefinedMethodToCall;
		return exprUnaryOp;
	}
}
