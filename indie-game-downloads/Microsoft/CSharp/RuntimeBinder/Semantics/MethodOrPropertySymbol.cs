using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal abstract class MethodOrPropertySymbol : ParentSymbol
{
	public uint modOptCount;

	public new bool isStatic;

	public bool isOverride;

	public bool isOperator;

	public bool isParamArray;

	public bool isHideByName;

	private bool[] _optionalParameterIndex;

	private bool[] _defaultParameterIndex;

	private ConstVal[] _defaultParameters;

	private CType[] _defaultParameterConstValTypes;

	private bool[] _marshalAsIndex;

	private UnmanagedType[] _marshalAsBuffer;

	public SymWithType swtSlot;

	public CType RetType;

	private TypeArray _Params;

	public List<Name> ParameterNames { get; private set; }

	public TypeArray Params
	{
		get
		{
			return _Params;
		}
		set
		{
			_Params = value;
			int count = value.Count;
			if (count == 0)
			{
				_optionalParameterIndex = (_defaultParameterIndex = (_marshalAsIndex = Array.Empty<bool>()));
				_defaultParameters = Array.Empty<ConstVal>();
				_defaultParameterConstValTypes = Array.Empty<CType>();
				_marshalAsBuffer = Array.Empty<UnmanagedType>();
			}
			else
			{
				_optionalParameterIndex = new bool[count];
				_defaultParameterIndex = new bool[count];
				_defaultParameters = new ConstVal[count];
				_defaultParameterConstValTypes = new CType[count];
				_marshalAsIndex = new bool[count];
				_marshalAsBuffer = new UnmanagedType[count];
			}
		}
	}

	public MethodOrPropertySymbol()
	{
		ParameterNames = new List<Name>();
	}

	public bool IsParameterOptional(int index)
	{
		if (_optionalParameterIndex == null)
		{
			return false;
		}
		return _optionalParameterIndex[index];
	}

	public void SetOptionalParameter(int index)
	{
		_optionalParameterIndex[index] = true;
	}

	public bool HasOptionalParameters()
	{
		if (_optionalParameterIndex == null)
		{
			return false;
		}
		bool[] optionalParameterIndex = _optionalParameterIndex;
		for (int i = 0; i < optionalParameterIndex.Length; i++)
		{
			if (optionalParameterIndex[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool HasDefaultParameterValue(int index)
	{
		return _defaultParameterIndex[index];
	}

	public void SetDefaultParameterValue(int index, CType type, ConstVal cv)
	{
		_defaultParameterIndex[index] = true;
		_defaultParameters[index] = cv;
		_defaultParameterConstValTypes[index] = type;
	}

	public ConstVal GetDefaultParameterValue(int index)
	{
		return _defaultParameters[index];
	}

	public CType GetDefaultParameterValueConstValType(int index)
	{
		return _defaultParameterConstValTypes[index];
	}

	private bool IsMarshalAsParameter(int index)
	{
		return _marshalAsIndex[index];
	}

	public void SetMarshalAsParameter(int index, UnmanagedType umt)
	{
		_marshalAsIndex[index] = true;
		_marshalAsBuffer[index] = umt;
	}

	private UnmanagedType GetMarshalAsParameterValue(int index)
	{
		return _marshalAsBuffer[index];
	}

	public bool MarshalAsObject(int index)
	{
		if (IsMarshalAsParameter(index))
		{
			UnmanagedType marshalAsParameterValue = GetMarshalAsParameterValue(index);
			if (marshalAsParameterValue != UnmanagedType.Interface && marshalAsParameterValue != UnmanagedType.IUnknown)
			{
				return marshalAsParameterValue == UnmanagedType.IDispatch;
			}
			return true;
		}
		return false;
	}

	public AggregateSymbol getClass()
	{
		return parent as AggregateSymbol;
	}

	public bool IsExpImpl()
	{
		return name == null;
	}
}
