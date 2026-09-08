using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class AggregateType : CType
{
	private AggregateType _baseType;

	private TypeArray _ifacesAll;

	private Type _associatedSystemType;

	public bool? ConstraintError;

	public bool AllHidden;

	public bool DiffHidden;

	public AggregateType OuterType { get; }

	public AggregateSymbol OwningAggregate { get; }

	public AggregateType BaseClass
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			if (_baseType == null)
			{
				Type baseType = AssociatedSystemType.BaseType;
				if (baseType == null)
				{
					return null;
				}
				AggregateType typeSrc = SymbolTable.GetCTypeFromType(baseType) as AggregateType;
				_baseType = TypeManager.SubstType(typeSrc, TypeArgsAll);
			}
			return _baseType;
		}
	}

	public IEnumerable<AggregateType> TypeHierarchy
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			if (IsInterfaceType)
			{
				yield return this;
				CType[] items = IfacesAll.Items;
				for (int i = 0; i < items.Length; i++)
				{
					yield return (AggregateType)items[i];
				}
				yield return GetPredefinedAggregateGetThisTypeWithSuppressedMessage();
			}
			else
			{
				for (AggregateType agg = this; agg != null; agg = agg.BaseClassWithSuppressedMessage)
				{
					yield return agg;
				}
			}
		}
	}

	private AggregateType BaseClassWithSuppressedMessage
	{
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Workarounds https://github.com/mono/linker/issues/1906. All usages are marked as unsafe.")]
		get
		{
			return BaseClass;
		}
	}

	public TypeArray TypeArgsThis { get; }

	public TypeArray TypeArgsAll { get; }

	public TypeArray IfacesAll => _ifacesAll ?? (_ifacesAll = TypeManager.SubstTypeArray(OwningAggregate.GetIfacesAll(), TypeArgsAll));

	public override bool IsReferenceType => OwningAggregate.IsRefType();

	public override bool IsNonNullableValueType => IsValueType;

	public override bool IsValueType => OwningAggregate.IsValueType();

	public override bool IsStaticClass => OwningAggregate.IsStatic();

	public override bool IsPredefined => OwningAggregate.IsPredefined();

	public override PredefinedType PredefinedType => OwningAggregate.GetPredefType();

	public override bool IsDelegateType => OwningAggregate.IsDelegate();

	public override bool IsSimpleType
	{
		get
		{
			AggregateSymbol owningAggregate = OwningAggregate;
			if (owningAggregate.IsPredefined())
			{
				return PredefinedTypeFacts.IsSimpleType(owningAggregate.GetPredefType());
			}
			return false;
		}
	}

	public override bool IsSimpleOrEnum
	{
		get
		{
			AggregateSymbol owningAggregate = OwningAggregate;
			if (!owningAggregate.IsPredefined())
			{
				return owningAggregate.IsEnum();
			}
			return PredefinedTypeFacts.IsSimpleType(owningAggregate.GetPredefType());
		}
	}

	public override bool IsSimpleOrEnumOrString
	{
		get
		{
			AggregateSymbol owningAggregate = OwningAggregate;
			if (owningAggregate.IsPredefined())
			{
				PredefinedType predefType = owningAggregate.GetPredefType();
				if (!PredefinedTypeFacts.IsSimpleType(predefType))
				{
					return predefType == PredefinedType.PT_STRING;
				}
				return true;
			}
			return owningAggregate.IsEnum();
		}
	}

	public override bool IsNumericType
	{
		get
		{
			AggregateSymbol owningAggregate = OwningAggregate;
			if (owningAggregate.IsPredefined())
			{
				return PredefinedTypeFacts.IsNumericType(owningAggregate.GetPredefType());
			}
			return false;
		}
	}

	public override bool IsStructType => OwningAggregate.IsStruct();

	public override bool IsEnumType => OwningAggregate.IsEnum();

	public override bool IsInterfaceType => OwningAggregate.IsInterface();

	public override bool IsClassType => OwningAggregate.IsClass();

	public override AggregateType UnderlyingEnumType => OwningAggregate.GetUnderlyingType();

	public override Type AssociatedSystemType
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			return _associatedSystemType ?? (_associatedSystemType = CalculateAssociatedSystemType());
		}
	}

	public override FUNDTYPE FundamentalType
	{
		get
		{
			AggregateSymbol owningAggregate = OwningAggregate;
			if (owningAggregate.IsEnum())
			{
				owningAggregate = owningAggregate.GetUnderlyingType().OwningAggregate;
			}
			else if (!owningAggregate.IsStruct())
			{
				return FUNDTYPE.FT_REF;
			}
			if (!owningAggregate.IsPredefined())
			{
				return FUNDTYPE.FT_STRUCT;
			}
			return PredefinedTypeFacts.GetFundType(owningAggregate.GetPredefType());
		}
	}

	public override ConstValKind ConstValKind
	{
		get
		{
			if (IsPredefType(PredefinedType.FirstNonSimpleType) || IsPredefType(PredefinedType.PT_UINTPTR))
			{
				return ConstValKind.IntPtr;
			}
			switch (FundamentalType)
			{
			case FUNDTYPE.FT_I8:
			case FUNDTYPE.FT_U8:
				return ConstValKind.Long;
			case FUNDTYPE.FT_STRUCT:
				if (!IsPredefined || PredefinedType != PredefinedType.PT_DATETIME)
				{
					return ConstValKind.Decimal;
				}
				return ConstValKind.Long;
			case FUNDTYPE.FT_REF:
				if (!IsPredefined || PredefinedType != PredefinedType.PT_STRING)
				{
					return ConstValKind.IntPtr;
				}
				return ConstValKind.String;
			case FUNDTYPE.FT_R4:
				return ConstValKind.Float;
			case FUNDTYPE.FT_R8:
				return ConstValKind.Double;
			case FUNDTYPE.FT_I1:
				return ConstValKind.Boolean;
			default:
				return ConstValKind.Int;
			}
		}
	}

	public AggregateType(AggregateSymbol parent, TypeArray typeArgsThis, AggregateType outerType)
		: base(TypeKind.TK_AggregateType)
	{
		OuterType = outerType;
		OwningAggregate = parent;
		TypeArgsThis = typeArgsThis;
		TypeArgsAll = ((outerType != null) ? TypeArray.Concat(outerType.TypeArgsAll, typeArgsThis) : typeArgsThis);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Workarounds https://github.com/mono/linker/issues/1906. All usages are marked as unsafe.")]
	private static AggregateType GetPredefinedAggregateGetThisTypeWithSuppressedMessage()
	{
		return PredefinedTypes.GetPredefinedAggregate(PredefinedType.PT_OBJECT).getThisType();
	}

	public override bool IsPredefType(PredefinedType pt)
	{
		AggregateSymbol owningAggregate = OwningAggregate;
		if (owningAggregate.IsPredefined())
		{
			return owningAggregate.GetPredefType() == pt;
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Type CalculateAssociatedSystemType()
	{
		Type associatedSystemType = OwningAggregate.AssociatedSystemType;
		if (associatedSystemType.IsGenericType)
		{
			TypeArray typeArgsAll = TypeArgsAll;
			Type[] array = new Type[typeArgsAll.Count];
			for (int i = 0; i < array.Length; i++)
			{
				CType cType = typeArgsAll[i];
				if (cType is TypeParameterType typeParameterType && typeParameterType.Symbol.name == null)
				{
					return null;
				}
				array[i] = cType.AssociatedSystemType;
			}
			try
			{
				return associatedSystemType.MakeGenericType(array);
			}
			catch (ArgumentException)
			{
			}
		}
		return associatedSystemType;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	public override AggregateType GetAts()
	{
		return this;
	}
}
