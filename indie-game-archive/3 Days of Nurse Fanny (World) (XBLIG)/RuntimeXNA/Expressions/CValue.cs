using System;

namespace RuntimeXNA.Expressions;

public class CValue
{
	public const byte TYPE_INT = 0;

	public const byte TYPE_DOUBLE = 1;

	public const byte TYPE_STRING = 2;

	public byte type;

	public int intValue;

	public double doubleValue;

	public string stringValue;

	public CValue()
	{
		type = 0;
		intValue = 0;
	}

	public CValue(CValue value)
	{
		switch (value.type)
		{
		case 0:
			intValue = value.intValue;
			break;
		case 1:
			doubleValue = value.doubleValue;
			break;
		case 2:
			stringValue = string.Concat(value.stringValue);
			break;
		}
		type = value.type;
	}

	public CValue(int i)
	{
		type = 0;
		intValue = i;
	}

	public CValue(double d)
	{
		type = 1;
		doubleValue = d;
	}

	public CValue(string s)
	{
		type = 2;
		stringValue = string.Concat(s);
	}

	public byte getType()
	{
		return type;
	}

	public int getInt()
	{
		return type switch
		{
			0 => intValue, 
			1 => (int)doubleValue, 
			_ => 0, 
		};
	}

	public double getDouble()
	{
		return type switch
		{
			0 => intValue, 
			1 => doubleValue, 
			_ => 0.0, 
		};
	}

	public string getString()
	{
		if (type == 2)
		{
			return stringValue;
		}
		return "";
	}

	public void forceInt(int value)
	{
		type = 0;
		intValue = value;
	}

	public void forceDouble(double value)
	{
		type = 1;
		doubleValue = value;
	}

	public void forceString(string value)
	{
		type = 2;
		stringValue = string.Concat(value);
	}

	public void forceValue(CValue value)
	{
		type = value.type;
		switch (type)
		{
		case 0:
			intValue = value.intValue;
			break;
		case 1:
			doubleValue = value.doubleValue;
			break;
		case 2:
			stringValue = string.Concat(value.stringValue);
			break;
		}
	}

	public void setValue(CValue value)
	{
		switch (type)
		{
		case 0:
			intValue = value.getInt();
			break;
		case 1:
			doubleValue = value.getDouble();
			break;
		case 2:
			stringValue = string.Concat(value.stringValue);
			break;
		}
	}

	public void getCompatibleTypes(CValue value)
	{
		if (type == 0 && value.type == 1)
		{
			convertToDouble();
		}
		else if (type == 1 && value.type == 0)
		{
			value.convertToDouble();
		}
	}

	public void convertToDouble()
	{
		if (type == 0)
		{
			doubleValue = intValue;
			type = 1;
		}
	}

	public void convertToInt()
	{
		if (type == 1)
		{
			intValue = (int)doubleValue;
			type = 0;
		}
	}

	public void add(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		switch (type)
		{
		case 0:
			intValue += value.intValue;
			break;
		case 1:
			doubleValue += value.doubleValue;
			break;
		case 2:
			stringValue += value.stringValue;
			break;
		}
	}

	public void sub(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		switch (type)
		{
		case 0:
			intValue -= value.intValue;
			break;
		case 1:
			doubleValue -= value.doubleValue;
			break;
		}
	}

	public void negate()
	{
		switch (type)
		{
		case 0:
			intValue = -intValue;
			break;
		case 1:
			doubleValue = 0.0 - doubleValue;
			break;
		}
	}

	public void mul(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		switch (type)
		{
		case 0:
			intValue *= value.intValue;
			break;
		case 1:
			doubleValue *= value.doubleValue;
			break;
		}
	}

	public void div(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		switch (type)
		{
		case 0:
			if (value.intValue != 0)
			{
				intValue /= value.intValue;
			}
			else
			{
				intValue = 0;
			}
			break;
		case 1:
			if (value.doubleValue != 0.0)
			{
				doubleValue /= value.doubleValue;
			}
			else
			{
				doubleValue = 0.0;
			}
			break;
		}
	}

	public void pow(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		switch (type)
		{
		case 0:
			doubleValue = Math.Pow(getDouble(), value.getDouble());
			type = 1;
			break;
		case 1:
			doubleValue = Math.Pow(doubleValue, value.doubleValue);
			break;
		}
	}

	public void mod(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		switch (type)
		{
		case 0:
			if (value.intValue == 0)
			{
				intValue = 0;
			}
			else
			{
				intValue %= value.intValue;
			}
			break;
		case 1:
			if (value.doubleValue == 0.0)
			{
				doubleValue = 0.0;
			}
			else
			{
				doubleValue %= value.doubleValue;
			}
			break;
		}
	}

	public void andLog(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		switch (type)
		{
		case 0:
			intValue &= value.intValue;
			break;
		case 1:
			forceInt(getInt() & value.getInt());
			break;
		}
	}

	public void orLog(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		switch (type)
		{
		case 0:
			intValue |= value.intValue;
			break;
		case 1:
			forceInt(getInt() | value.getInt());
			break;
		}
	}

	public void xorLog(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		switch (type)
		{
		case 0:
			intValue ^= value.intValue;
			break;
		case 1:
			forceInt(getInt() ^ value.getInt());
			break;
		}
	}

	public bool equal(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		return type switch
		{
			0 => intValue == value.intValue, 
			1 => doubleValue == value.doubleValue, 
			2 => stringValue == value.stringValue, 
			_ => false, 
		};
	}

	public bool greater(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		return type switch
		{
			0 => intValue >= value.intValue, 
			1 => doubleValue >= value.doubleValue, 
			2 => stringValue.CompareTo(value.stringValue) >= 0, 
			_ => false, 
		};
	}

	public bool lower(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		return type switch
		{
			0 => intValue <= value.intValue, 
			1 => doubleValue <= value.doubleValue, 
			2 => stringValue.CompareTo(value.stringValue) <= 0, 
			_ => false, 
		};
	}

	public bool greaterThan(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		return type switch
		{
			0 => intValue > value.intValue, 
			1 => doubleValue > value.doubleValue, 
			2 => stringValue.CompareTo(value.stringValue) > 0, 
			_ => false, 
		};
	}

	public bool lowerThan(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		return type switch
		{
			0 => intValue < value.intValue, 
			1 => doubleValue < value.doubleValue, 
			2 => stringValue.CompareTo(value.stringValue) < 0, 
			_ => false, 
		};
	}

	public bool notEqual(CValue value)
	{
		if (type != value.type)
		{
			getCompatibleTypes(value);
		}
		return type switch
		{
			0 => intValue != value.intValue, 
			1 => doubleValue != value.doubleValue, 
			2 => stringValue != value.stringValue, 
			_ => false, 
		};
	}
}
