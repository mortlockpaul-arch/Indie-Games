using System.Reflection;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class MethodSymbol : MethodOrPropertySymbol
{
	private MethodKindEnum _methKind;

	private bool _inferenceMustFail;

	private bool _checkedInfMustFail;

	private MethodSymbol _convNext;

	private PropertySymbol _prop;

	private EventSymbol _evt;

	public bool isVirtual;

	public MemberInfo AssociatedMemberInfo;

	public TypeArray typeVars;

	public MethodKindEnum MethKind => _methKind;

	public bool InferenceMustFail()
	{
		if (_checkedInfMustFail)
		{
			return _inferenceMustFail;
		}
		_checkedInfMustFail = true;
		for (int i = 0; i < typeVars.Count; i++)
		{
			TypeParameterType typeFind = (TypeParameterType)typeVars[i];
			int num = 0;
			while (true)
			{
				if (num >= base.Params.Count)
				{
					_inferenceMustFail = true;
					return true;
				}
				if (TypeManager.TypeContainsType(base.Params[num], typeFind))
				{
					break;
				}
				num++;
			}
		}
		return false;
	}

	public bool IsConstructor()
	{
		return _methKind == MethodKindEnum.Constructor;
	}

	public bool IsNullableConstructor()
	{
		if (getClass().isPredefAgg(PredefinedType.PT_G_OPTIONAL) && base.Params.Count == 1 && base.Params[0] is TypeParameterType)
		{
			return IsConstructor();
		}
		return false;
	}

	public bool isPropertyAccessor()
	{
		return _methKind == MethodKindEnum.PropAccessor;
	}

	public bool isEventAccessor()
	{
		return _methKind == MethodKindEnum.EventAccessor;
	}

	public bool isImplicit()
	{
		return _methKind == MethodKindEnum.ImplicitConv;
	}

	public void SetMethKind(MethodKindEnum mk)
	{
		_methKind = mk;
	}

	public MethodSymbol ConvNext()
	{
		return _convNext;
	}

	public void SetConvNext(MethodSymbol conv)
	{
		_convNext = conv;
	}

	public PropertySymbol getProperty()
	{
		return _prop;
	}

	public void SetProperty(PropertySymbol prop)
	{
		_prop = prop;
	}

	public EventSymbol getEvent()
	{
		return _evt;
	}

	public void SetEvent(EventSymbol evt)
	{
		_evt = evt;
	}

	public new bool isUserCallable()
	{
		if (!isOperator)
		{
			return !isAnyAccessor();
		}
		return false;
	}

	private bool isAnyAccessor()
	{
		if (!isPropertyAccessor())
		{
			return isEventAccessor();
		}
		return true;
	}

	public bool isSetAccessor()
	{
		if (!isPropertyAccessor())
		{
			return false;
		}
		PropertySymbol property = getProperty();
		if (property == null)
		{
			return false;
		}
		return this == property.SetterMethod;
	}
}
