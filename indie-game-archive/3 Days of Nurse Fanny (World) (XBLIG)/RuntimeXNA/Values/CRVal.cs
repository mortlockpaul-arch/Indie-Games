using RuntimeXNA.Expressions;
using RuntimeXNA.OI;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Values;

public class CRVal
{
	public const int VALUES_NUMBEROF_ALTERABLE = 26;

	public const int STRINGS_NUMBEROF_ALTERABLE = 10;

	public int rvValueFlags;

	public CValue[] rvValues;

	public string[] rvStrings;

	public void init(CObject ho, CObjectCommon ocPtr, CCreateObjectInfo cob)
	{
		rvValueFlags = 0;
		rvValues = new CValue[26];
		rvStrings = new string[10];
		for (int i = 0; i < 26; i++)
		{
			rvValues[i] = null;
		}
		for (int i = 0; i < 10; i++)
		{
			rvStrings[i] = null;
		}
		if (ocPtr.ocValues != null)
		{
			for (int i = 0; i < ocPtr.ocValues.nValues; i++)
			{
				CValue value = getValue(i);
				value.forceInt(ocPtr.ocValues.values[i]);
			}
		}
		if (ocPtr.ocStrings != null)
		{
			for (int i = 0; i < ocPtr.ocStrings.nStrings; i++)
			{
				rvStrings[i] = ocPtr.ocStrings.strings[i];
			}
		}
	}

	public void kill(bool bFast)
	{
		for (int i = 0; i < 26; i++)
		{
			rvValues[i] = null;
		}
		for (int i = 0; i < 10; i++)
		{
			rvStrings[i] = null;
		}
	}

	public CValue getValue(int n)
	{
		if (rvValues[n] == null)
		{
			rvValues[n] = new CValue();
		}
		return rvValues[n];
	}

	public string getString(int n)
	{
		if (rvStrings[n] == null)
		{
			rvStrings[n] = "";
		}
		return rvStrings[n];
	}

	public void setString(int n, string s)
	{
		rvStrings[n] = s;
	}
}
