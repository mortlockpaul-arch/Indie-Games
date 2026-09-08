using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class CMemberLookupResults
{
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public sealed class CMethodIterator
	{
		private readonly AggregateSymbol _context;

		private readonly TypeArray _containingTypes;

		private readonly CType _qualifyingType;

		private readonly Name _name;

		private readonly int _arity;

		private readonly symbmask_t _mask;

		private readonly EXPRFLAG _flags;

		private readonly ArgInfos _nonTrailingNamedArguments;

		private int _currentTypeIndex;

		public MethodOrPropertySymbol CurrentSymbol { get; private set; }

		public AggregateType CurrentType { get; private set; }

		public bool IsCurrentSymbolInaccessible { get; private set; }

		public bool IsCurrentSymbolBogus { get; private set; }

		public bool IsCurrentSymbolMisnamed { get; private set; }

		public bool AtEnd => CurrentSymbol == null;

		public bool CanUseCurrentSymbol
		{
			[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
			get
			{
				if ((_mask == symbmask_t.MASK_MethodSymbol && ((_flags & EXPRFLAG.EXF_CTOR) == 0 != !((MethodSymbol)CurrentSymbol).IsConstructor() || (_flags & EXPRFLAG.EXF_OPERATOR) == 0 != !((MethodSymbol)CurrentSymbol).isOperator)) || (_mask == symbmask_t.MASK_PropertySymbol && !(CurrentSymbol is IndexerSymbol)))
				{
					return false;
				}
				if (((_arity > 0) & (_mask == symbmask_t.MASK_MethodSymbol)) && ((MethodSymbol)CurrentSymbol).typeVars.Count != _arity)
				{
					return false;
				}
				if (!ExpressionBinder.IsMethPropCallable(CurrentSymbol, (_flags & EXPRFLAG.EXF_USERCALLABLE) != 0))
				{
					return false;
				}
				IsCurrentSymbolInaccessible = !CSemanticChecker.CheckAccess(CurrentSymbol, CurrentType, _context, _qualifyingType);
				IsCurrentSymbolBogus = CSemanticChecker.CheckBogus(CurrentSymbol);
				IsCurrentSymbolMisnamed = CheckArgumentNames();
				return true;
			}
		}

		public CMethodIterator(Name name, TypeArray containingTypes, CType qualifyingType, AggregateSymbol context, int arity, EXPRFLAG flags, symbmask_t mask, ArgInfos nonTrailingNamedArguments)
		{
			_name = name;
			_containingTypes = containingTypes;
			_qualifyingType = qualifyingType;
			_context = context;
			_arity = arity;
			_flags = flags;
			_mask = mask;
			_nonTrailingNamedArguments = nonTrailingNamedArguments;
		}

		public bool MoveNext()
		{
			if (CurrentType != null || FindNextTypeForInstanceMethods())
			{
				return FindNextMethod();
			}
			return false;
		}

		private bool CheckArgumentNames()
		{
			ArgInfos nonTrailingNamedArguments = _nonTrailingNamedArguments;
			if (nonTrailingNamedArguments != null)
			{
				List<Name> parameterNames = ExpressionBinder.GroupToArgsBinder.FindMostDerivedMethod(CurrentSymbol, _qualifyingType).ParameterNames;
				List<Expr> prgexpr = nonTrailingNamedArguments.prgexpr;
				for (int i = 0; i < nonTrailingNamedArguments.carg; i++)
				{
					if (prgexpr[i] is ExprNamedArgumentSpecification exprNamedArgumentSpecification && (parameterNames[i] != exprNamedArgumentSpecification.Name || (i == parameterNames.Count - 1 && i != nonTrailingNamedArguments.carg - 1)))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool FindNextMethod()
		{
			while (true)
			{
				CurrentSymbol = ((CurrentSymbol == null) ? SymbolLoader.LookupAggMember(_name, CurrentType.OwningAggregate, _mask) : CurrentSymbol.LookupNext(_mask)) as MethodOrPropertySymbol;
				if (CurrentSymbol != null)
				{
					break;
				}
				if (!FindNextTypeForInstanceMethods())
				{
					return false;
				}
			}
			return true;
		}

		private bool FindNextTypeForInstanceMethods()
		{
			if (_currentTypeIndex >= _containingTypes.Count)
			{
				CurrentType = null;
				return false;
			}
			CurrentType = _containingTypes[_currentTypeIndex++] as AggregateType;
			return true;
		}
	}

	private readonly Name _pName;

	private TypeArray ContainingTypes { get; }

	public CMemberLookupResults(TypeArray containingTypes, Name name)
	{
		_pName = name;
		ContainingTypes = containingTypes;
	}

	public CMethodIterator GetMethodIterator(CType qualifyingType, AggregateSymbol context, int arity, EXPRFLAG flags, symbmask_t mask, ArgInfos nonTrailingNamedArguments)
	{
		return new CMethodIterator(_pName, ContainingTypes, qualifyingType, context, arity, flags, mask, nonTrailingNamedArguments);
	}
}
