using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder.Errors;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class MemberLookup
{
	private CType _typeSrc;

	private CType _typeQual;

	private ParentSymbol _symWhere;

	private Name _name;

	private int _arity;

	private MemLookFlags _flags;

	private readonly List<AggregateType> _rgtypeStart;

	private List<AggregateType> _prgtype;

	private int _csym;

	private readonly SymWithType _swtFirst;

	private readonly List<MethPropWithType> _methPropWithTypeList;

	private readonly SymWithType _swtAmbig;

	private readonly SymWithType _swtInaccess;

	private readonly SymWithType _swtBad;

	private readonly SymWithType _swtBogus;

	private readonly SymWithType _swtBadArity;

	private bool _fMulti;

	private void RecordType(AggregateType type, Symbol sym)
	{
		if (!_prgtype.Contains(type))
		{
			_prgtype.Add(type);
		}
		_csym++;
		if (_swtFirst == null)
		{
			_swtFirst.Set(sym, type);
			_fMulti = sym is MethodSymbol || sym is IndexerSymbol;
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool SearchSingleType(AggregateType typeCur, out bool pfHideByName)
	{
		bool result = false;
		pfHideByName = false;
		bool flag = !CSemanticChecker.CheckTypeAccess(typeCur, _symWhere);
		if (flag && (_csym != 0 || _swtInaccess != null))
		{
			return false;
		}
		Symbol symbol = SymbolLoader.LookupAggMember(_name, typeCur.OwningAggregate, symbmask_t.MASK_Member);
		while (true)
		{
			if (symbol != null)
			{
				if (_arity > 0 && (!(symbol is MethodSymbol methodSymbol) || methodSymbol.typeVars.Count != _arity))
				{
					if (!_swtBadArity)
					{
						_swtBadArity.Set(symbol, typeCur);
					}
				}
				else if (!symbol.IsOverride() || symbol.IsHideByName())
				{
					MethodOrPropertySymbol methodOrPropertySymbol = symbol as MethodOrPropertySymbol;
					MethodSymbol methodSymbol2 = symbol as MethodSymbol;
					if (methodOrPropertySymbol != null && (_flags & MemLookFlags.UserCallable) != MemLookFlags.None && !methodOrPropertySymbol.isUserCallable() && (methodSymbol2 == null || !methodSymbol2.isPropertyAccessor() || ((!symbol.name.Text.StartsWith("set_", StringComparison.Ordinal) || methodSymbol2.Params.Count <= 1) && (!symbol.name.Text.StartsWith("get_", StringComparison.Ordinal) || methodSymbol2.Params.Count <= 0))))
					{
						if (!_swtInaccess)
						{
							_swtInaccess.Set(symbol, typeCur);
						}
					}
					else if (flag || !CSemanticChecker.CheckAccess(symbol, typeCur, _symWhere, _typeQual))
					{
						if (!_swtInaccess)
						{
							_swtInaccess.Set(symbol, typeCur);
						}
						if (flag)
						{
							return false;
						}
					}
					else
					{
						PropertySymbol propertySymbol = symbol as PropertySymbol;
						if ((_flags & MemLookFlags.Ctor) == 0 != (methodSymbol2 == null || !methodSymbol2.IsConstructor()) || (_flags & MemLookFlags.Operator) == 0 != (methodSymbol2 == null || !methodSymbol2.isOperator) || (_flags & MemLookFlags.Indexer) == 0 != !(propertySymbol is IndexerSymbol))
						{
							if (!_swtBad)
							{
								_swtBad.Set(symbol, typeCur);
							}
						}
						else if (!(symbol is MethodSymbol) && (_flags & MemLookFlags.Indexer) == 0 && CSemanticChecker.CheckBogus(symbol))
						{
							if (!_swtBogus)
							{
								_swtBogus.Set(symbol, typeCur);
							}
						}
						else if ((_flags & MemLookFlags.MustBeInvocable) != MemLookFlags.None && ((symbol is FieldSymbol fieldSymbol && !IsDelegateType(fieldSymbol.GetType(), typeCur) && !IsDynamicMember(symbol)) || (propertySymbol != null && !IsDelegateType(propertySymbol.RetType, typeCur) && !IsDynamicMember(symbol))))
						{
							if (!_swtBad)
							{
								_swtBad.Set(symbol, typeCur);
							}
						}
						else
						{
							if (methodOrPropertySymbol != null)
							{
								MethPropWithType item = new MethPropWithType(methodOrPropertySymbol, typeCur);
								_methPropWithTypeList.Add(item);
							}
							result = true;
							if ((bool)_swtFirst)
							{
								if (!typeCur.IsInterfaceType)
								{
									if (!_fMulti)
									{
										if ((!(_swtFirst.Sym is FieldSymbol) || !(symbol is EventSymbol) || !_swtFirst.Field().isEvent) && (!(_swtFirst.Sym is FieldSymbol) || !(symbol is EventSymbol)))
										{
											break;
										}
										goto IL_040f;
									}
									if (_swtFirst.Sym.getKind() != symbol.getKind())
									{
										if (typeCur == _prgtype[0])
										{
											break;
										}
										pfHideByName = true;
										goto IL_040f;
									}
								}
								else if (!_fMulti)
								{
									if (!(symbol is MethodSymbol))
									{
										break;
									}
									_prgtype = new List<AggregateType>();
									_csym = 0;
									_swtFirst.Clear();
									_swtAmbig.Clear();
								}
								else if (_swtFirst.Sym.getKind() != symbol.getKind())
								{
									if (!typeCur.DiffHidden && !(_swtFirst.Sym is MethodSymbol))
									{
										break;
									}
									pfHideByName = true;
									goto IL_040f;
								}
							}
							RecordType(typeCur, symbol);
							if (methodOrPropertySymbol != null && methodOrPropertySymbol.isHideByName)
							{
								pfHideByName = true;
							}
						}
					}
				}
				goto IL_040f;
			}
			return result;
			IL_040f:
			symbol = symbol.LookupNext(symbmask_t.MASK_Member);
		}
		if (!_swtAmbig)
		{
			_swtAmbig.Set(symbol, typeCur);
		}
		pfHideByName = true;
		return true;
	}

	private static bool IsDynamicMember(Symbol sym)
	{
		DynamicAttribute dynamicAttribute = null;
		if (sym is FieldSymbol fieldSymbol)
		{
			if (!fieldSymbol.getType().IsPredefType(PredefinedType.PT_OBJECT))
			{
				return false;
			}
			object[] customAttributes = fieldSymbol.AssociatedFieldInfo.GetCustomAttributes(typeof(DynamicAttribute), inherit: false);
			if (customAttributes.Length == 1)
			{
				dynamicAttribute = customAttributes[0] as DynamicAttribute;
			}
		}
		else
		{
			PropertySymbol propertySymbol = (PropertySymbol)sym;
			if (!propertySymbol.getType().IsPredefType(PredefinedType.PT_OBJECT))
			{
				return false;
			}
			object[] customAttributes2 = propertySymbol.AssociatedPropertyInfo.GetCustomAttributes(typeof(DynamicAttribute), inherit: false);
			if (customAttributes2.Length == 1)
			{
				dynamicAttribute = customAttributes2[0] as DynamicAttribute;
			}
		}
		if (dynamicAttribute == null)
		{
			return false;
		}
		if (dynamicAttribute.TransformFlags.Count != 0)
		{
			if (dynamicAttribute.TransformFlags.Count == 1)
			{
				return dynamicAttribute.TransformFlags[0];
			}
			return false;
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool LookupInClass(AggregateType typeStart, ref AggregateType ptypeEnd)
	{
		AggregateType aggregateType = ptypeEnd;
		AggregateType aggregateType2 = typeStart;
		while (aggregateType2 != aggregateType && aggregateType2 != null)
		{
			SearchSingleType(aggregateType2, out var pfHideByName);
			if ((bool)_swtFirst && !_fMulti)
			{
				return false;
			}
			if (pfHideByName)
			{
				ptypeEnd = null;
				return true;
			}
			if ((_flags & MemLookFlags.Ctor) != MemLookFlags.None)
			{
				return false;
			}
			aggregateType2 = aggregateType2.BaseClass;
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private bool LookupInInterfaces(AggregateType typeStart, TypeArray types)
	{
		if (typeStart != null)
		{
			typeStart.AllHidden = false;
			typeStart.DiffHidden = _swtFirst != null;
		}
		for (int i = 0; i < types.Count; i++)
		{
			AggregateType obj = (AggregateType)types[i];
			obj.AllHidden = false;
			obj.DiffHidden = _swtFirst;
		}
		bool flag = false;
		AggregateType aggregateType = typeStart;
		int num = 0;
		if (aggregateType == null)
		{
			aggregateType = (AggregateType)types[num++];
		}
		while (true)
		{
			if (!aggregateType.AllHidden && SearchSingleType(aggregateType, out var pfHideByName))
			{
				pfHideByName |= !_fMulti;
				CType[] items = aggregateType.IfacesAll.Items;
				for (int j = 0; j < items.Length; j++)
				{
					AggregateType aggregateType2 = (AggregateType)items[j];
					if (pfHideByName)
					{
						aggregateType2.AllHidden = true;
					}
					aggregateType2.DiffHidden = true;
				}
				if (pfHideByName)
				{
					flag = true;
				}
			}
			if (num >= types.Count)
			{
				break;
			}
			aggregateType = types[num++] as AggregateType;
		}
		return !flag;
	}

	private static RuntimeBinderException ReportBogus(SymWithType swt)
	{
		MethodSymbol getterMethod = swt.Prop().GetterMethod;
		MethodSymbol setterMethod = swt.Prop().SetterMethod;
		if (!((getterMethod == null) | (setterMethod == null)))
		{
			return ErrorHandling.Error(ErrorCode.ERR_BindToBogusProp2, swt.Sym.name, new SymWithType(getterMethod, swt.GetType()), new SymWithType(setterMethod, swt.GetType()), new ErrArgRefOnly(swt.Sym));
		}
		return ErrorHandling.Error(ErrorCode.ERR_BindToBogusProp1, swt.Sym.name, new SymWithType(getterMethod ?? setterMethod, swt.GetType()), new ErrArgRefOnly(swt.Sym));
	}

	private static bool IsDelegateType(CType pSrcType, AggregateType pAggType)
	{
		return TypeManager.SubstType(pSrcType, pAggType, pAggType.TypeArgsAll).IsDelegateType;
	}

	public MemberLookup()
	{
		_methPropWithTypeList = new List<MethPropWithType>();
		_rgtypeStart = new List<AggregateType>();
		_swtFirst = new SymWithType();
		_swtAmbig = new SymWithType();
		_swtInaccess = new SymWithType();
		_swtBad = new SymWithType();
		_swtBogus = new SymWithType();
		_swtBadArity = new SymWithType();
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	public bool Lookup(CType typeSrc, Expr obj, ParentSymbol symWhere, Name name, int arity, MemLookFlags flags)
	{
		_prgtype = _rgtypeStart;
		_typeSrc = typeSrc;
		_symWhere = symWhere;
		_name = name;
		_arity = arity;
		_flags = flags;
		_typeQual = (((_flags & MemLookFlags.Ctor) != MemLookFlags.None) ? _typeSrc : obj?.Type);
		AggregateType aggregateType;
		AggregateType aggregateType2;
		TypeArray typeArray;
		if (typeSrc.IsInterfaceType)
		{
			aggregateType = null;
			aggregateType2 = (AggregateType)typeSrc;
			typeArray = aggregateType2.IfacesAll;
		}
		else
		{
			aggregateType = (AggregateType)typeSrc;
			aggregateType2 = null;
			typeArray = TypeArray.Empty;
		}
		AggregateType ptypeEnd = ((aggregateType2 != null || typeArray.Count > 0) ? SymbolLoader.GetPredefindType(PredefinedType.PT_OBJECT) : null);
		if ((aggregateType == null || LookupInClass(aggregateType, ref ptypeEnd)) && (aggregateType2 != null || typeArray.Count > 0) && LookupInInterfaces(aggregateType2, typeArray) && ptypeEnd != null)
		{
			AggregateType ptypeEnd2 = null;
			LookupInClass(ptypeEnd, ref ptypeEnd2);
		}
		return !FError();
	}

	private bool FError()
	{
		if ((bool)_swtFirst)
		{
			return _swtAmbig;
		}
		return true;
	}

	public SymWithType SwtFirst()
	{
		return _swtFirst;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	public Exception ReportErrors()
	{
		if ((bool)_swtFirst)
		{
			return ErrorHandling.Error(ErrorCode.ERR_AmbigMember, _swtFirst, _swtAmbig);
		}
		if ((bool)_swtInaccess)
		{
			if (_swtInaccess.Sym.isUserCallable() || (_flags & MemLookFlags.UserCallable) == 0)
			{
				return CSemanticChecker.ReportAccessError(_swtInaccess, _symWhere, _typeQual);
			}
			return ErrorHandling.Error(ErrorCode.ERR_CantCallSpecialMethod, _swtInaccess);
		}
		if ((_flags & MemLookFlags.Ctor) != MemLookFlags.None)
		{
			if (_arity <= 0)
			{
				return ErrorHandling.Error(ErrorCode.ERR_NoConstructors, ((AggregateType)_typeSrc).OwningAggregate);
			}
			return ErrorHandling.Error(ErrorCode.ERR_BadCtorArgCount, ((AggregateType)_typeSrc).OwningAggregate, _arity);
		}
		if ((_flags & MemLookFlags.Operator) != MemLookFlags.None)
		{
			return ErrorHandling.Error(ErrorCode.ERR_NoSuchMember, _typeSrc, _name);
		}
		if ((_flags & MemLookFlags.Indexer) != MemLookFlags.None)
		{
			return ErrorHandling.Error(ErrorCode.ERR_BadIndexLHS, _typeSrc);
		}
		if ((bool)_swtBad)
		{
			return ErrorHandling.Error(((_flags & MemLookFlags.MustBeInvocable) != MemLookFlags.None) ? ErrorCode.ERR_NonInvocableMemberCalled : ErrorCode.ERR_CantCallSpecialMethod, _swtBad);
		}
		if ((bool)_swtBogus)
		{
			return ReportBogus(_swtBogus);
		}
		if ((bool)_swtBadArity)
		{
			if (_swtBadArity.Sym is MethodSymbol methodSymbol)
			{
				int count = methodSymbol.typeVars.Count;
				return ErrorHandling.Error((count > 0) ? ErrorCode.ERR_BadArity : ErrorCode.ERR_HasNoTypeVars, _swtBadArity, new ErrArgSymKind(_swtBadArity.Sym), count);
			}
			return ErrorHandling.Error(ErrorCode.ERR_TypeArgsNotAllowed, _swtBadArity, new ErrArgSymKind(_swtBadArity.Sym));
		}
		return ErrorHandling.Error(ErrorCode.ERR_NoSuchMember, _typeSrc, _name);
	}
}
