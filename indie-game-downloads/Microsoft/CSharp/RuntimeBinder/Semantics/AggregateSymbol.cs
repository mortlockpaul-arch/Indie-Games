using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class AggregateSymbol : NamespaceOrAggregateSymbol
{
	public Type AssociatedSystemType;

	public Assembly AssociatedAssembly;

	private AggregateType _atsInst;

	private AggregateType _pBaseClass;

	private AggregateType _pUnderlyingType;

	private TypeArray _ifaces;

	private TypeArray _ifacesAll;

	private TypeArray _typeVarsThis;

	private TypeArray _typeVarsAll;

	private MethodSymbol _pConvFirst;

	private AggKindEnum _aggKind;

	private bool _isPredefined;

	private PredefinedType _iPredef;

	private bool _isAbstract;

	private bool _isSealed;

	private bool _hasPubNoArgCtor;

	private bool _isSkipUDOps;

	private bool? _hasConversion;

	public AggregateSymbol GetBaseAgg()
	{
		return _pBaseClass?.OwningAggregate;
	}

	public AggregateType getThisType()
	{
		if (_atsInst == null)
		{
			AggregateType atsOuter = (isNested() ? GetOuterAgg().getThisType() : null);
			_atsInst = TypeManager.GetAggregate(this, atsOuter, GetTypeVars());
		}
		return _atsInst;
	}

	public bool FindBaseAgg(AggregateSymbol agg)
	{
		for (AggregateSymbol aggregateSymbol = this; aggregateSymbol != null; aggregateSymbol = aggregateSymbol.GetBaseAgg())
		{
			if (aggregateSymbol == agg)
			{
				return true;
			}
		}
		return false;
	}

	public bool isNested()
	{
		return parent is AggregateSymbol;
	}

	public AggregateSymbol GetOuterAgg()
	{
		return parent as AggregateSymbol;
	}

	public bool isPredefAgg(PredefinedType pt)
	{
		if (_isPredefined)
		{
			return _iPredef == pt;
		}
		return false;
	}

	public AggKindEnum AggKind()
	{
		return _aggKind;
	}

	public void SetAggKind(AggKindEnum aggKind)
	{
		_aggKind = aggKind;
		if (aggKind == AggKindEnum.Interface)
		{
			SetAbstract(@abstract: true);
		}
	}

	public bool IsClass()
	{
		return AggKind() == AggKindEnum.Class;
	}

	public bool IsDelegate()
	{
		return AggKind() == AggKindEnum.Delegate;
	}

	public bool IsInterface()
	{
		return AggKind() == AggKindEnum.Interface;
	}

	public bool IsStruct()
	{
		return AggKind() == AggKindEnum.Struct;
	}

	public bool IsEnum()
	{
		return AggKind() == AggKindEnum.Enum;
	}

	public bool IsValueType()
	{
		if (AggKind() != AggKindEnum.Struct)
		{
			return AggKind() == AggKindEnum.Enum;
		}
		return true;
	}

	public bool IsRefType()
	{
		if (AggKind() != AggKindEnum.Class && AggKind() != AggKindEnum.Interface)
		{
			return AggKind() == AggKindEnum.Delegate;
		}
		return true;
	}

	public bool IsStatic()
	{
		if (_isAbstract)
		{
			return _isSealed;
		}
		return false;
	}

	public bool IsAbstract()
	{
		return _isAbstract;
	}

	public void SetAbstract(bool @abstract)
	{
		_isAbstract = @abstract;
	}

	public bool IsPredefined()
	{
		return _isPredefined;
	}

	public void SetPredefined(bool predefined)
	{
		_isPredefined = predefined;
	}

	public PredefinedType GetPredefType()
	{
		return _iPredef;
	}

	public void SetPredefType(PredefinedType predef)
	{
		_iPredef = predef;
	}

	public bool IsSealed()
	{
		return _isSealed;
	}

	public void SetSealed(bool @sealed)
	{
		_isSealed = @sealed;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	public bool HasConversion()
	{
		SymbolTable.AddConversionsForType(AssociatedSystemType);
		if (!_hasConversion.HasValue)
		{
			_hasConversion = GetBaseAgg() != null && GetBaseAgg().HasConversion();
		}
		return _hasConversion.Value;
	}

	public void SetHasConversion()
	{
		_hasConversion = true;
	}

	public bool HasPubNoArgCtor()
	{
		return _hasPubNoArgCtor;
	}

	public void SetHasPubNoArgCtor(bool hasPubNoArgCtor)
	{
		_hasPubNoArgCtor = hasPubNoArgCtor;
	}

	public bool IsSkipUDOps()
	{
		return _isSkipUDOps;
	}

	public void SetSkipUDOps(bool skipUDOps)
	{
		_isSkipUDOps = skipUDOps;
	}

	public TypeArray GetTypeVars()
	{
		return _typeVarsThis;
	}

	public void SetTypeVars(TypeArray typeVars)
	{
		if (typeVars == null)
		{
			_typeVarsThis = null;
			_typeVarsAll = null;
		}
		else
		{
			TypeArray pta = ((GetOuterAgg() == null) ? TypeArray.Empty : GetOuterAgg().GetTypeVarsAll());
			_typeVarsThis = typeVars;
			_typeVarsAll = TypeArray.Concat(pta, typeVars);
		}
	}

	public TypeArray GetTypeVarsAll()
	{
		return _typeVarsAll;
	}

	public AggregateType GetBaseClass()
	{
		return _pBaseClass;
	}

	public void SetBaseClass(AggregateType baseClass)
	{
		_pBaseClass = baseClass;
	}

	public AggregateType GetUnderlyingType()
	{
		return _pUnderlyingType;
	}

	public void SetUnderlyingType(AggregateType underlyingType)
	{
		_pUnderlyingType = underlyingType;
	}

	public TypeArray GetIfaces()
	{
		return _ifaces;
	}

	public void SetIfaces(TypeArray ifaces)
	{
		_ifaces = ifaces;
	}

	public TypeArray GetIfacesAll()
	{
		return _ifacesAll;
	}

	public void SetIfacesAll(TypeArray ifacesAll)
	{
		_ifacesAll = ifacesAll;
	}

	public MethodSymbol GetFirstUDConversion()
	{
		return _pConvFirst;
	}

	public void SetFirstUDConversion(MethodSymbol conv)
	{
		_pConvFirst = conv;
	}

	public bool InternalsVisibleTo(Assembly assembly)
	{
		return TypeManager.InternalsVisibleTo(AssociatedAssembly, assembly);
	}
}
