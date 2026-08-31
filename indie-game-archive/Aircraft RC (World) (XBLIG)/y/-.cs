using System;
using System.Collections;
using System.Globalization;

namespace Y;

[Serializable]
internal sealed class _0002 : IComparer
{
	public static readonly _0002 Default = new _0002();

	internal static readonly _0002 HCB = new _0002(CultureInfo.InvariantCulture);

	private CompareInfo HC_0002;

	private _0002()
	{
	}

	internal _0002(CultureInfo P_0)
	{
		if (P_0 == null)
		{
			throw new ArgumentNullException("culture");
		}
		HC_0002 = P_0.CompareInfo;
	}

	public int Compare(object a, object b)
	{
		if (a == b)
		{
			return 0;
		}
		if (a == null)
		{
			return -1;
		}
		if (b == null)
		{
			return 1;
		}
		if (HC_0002 != null)
		{
			string text = a as string;
			string text2 = b as string;
			if (text != null && text2 != null)
			{
				return HC_0002.Compare(text, text2);
			}
		}
		if (a is IComparable)
		{
			return (a as IComparable).CompareTo(b);
		}
		if (b is IComparable)
		{
			return -(b as IComparable).CompareTo(a);
		}
		throw new ArgumentException("Neither 'a' nor 'b' implements IComparable.");
	}
}
