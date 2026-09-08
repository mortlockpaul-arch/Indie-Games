using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Collections;

[Serializable]
[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public struct DictionaryEntry(object key, object? value)
{
	private object _key = key;

	private object _value = value;

	public object Key
	{
		get
		{
			return _key;
		}
		set
		{
			_key = value;
		}
	}

	public object? Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Deconstruct(out object key, out object? value)
	{
		key = Key;
		value = Value;
	}

	public override string ToString()
	{
		return KeyValuePair.PairToString(_key, _value);
	}
}
