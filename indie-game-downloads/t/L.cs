using System;

namespace T;

internal abstract class L : _0018
{
	public new static L Create()
	{
		return new c();
	}

	public L()
	{
	}

	public abstract byte[] EncryptValue(byte[] rgb);

	public abstract byte[] DecryptValue(byte[] rgb);

	public abstract I ExportParameters(bool include);

	public abstract void ImportParameters(I parameters);

	internal void _3R(I P_0)
	{
		if (P_0.P != null)
		{
			Array.Clear(P_0.P, 0, P_0.P.Length);
		}
		if (P_0.Q != null)
		{
			Array.Clear(P_0.Q, 0, P_0.Q.Length);
		}
		if (P_0.DP != null)
		{
			Array.Clear(P_0.DP, 0, P_0.DP.Length);
		}
		if (P_0.DQ != null)
		{
			Array.Clear(P_0.DQ, 0, P_0.DQ.Length);
		}
		if (P_0.InverseQ != null)
		{
			Array.Clear(P_0.InverseQ, 0, P_0.InverseQ.Length);
		}
		if (P_0.D != null)
		{
			Array.Clear(P_0.D, 0, P_0.D.Length);
		}
	}
}
internal sealed class l
{
	private int _3A_0018;

	private int _3AL;

	private int _3A_0019;

	public int MaxSize => _3A_0018;

	public int MinSize => _3AL;

	public int SkipSize => _3A_0019;

	public l(int minSize, int maxSize, int skipSize)
	{
		_3A_0018 = maxSize;
		_3AL = minSize;
		_3A_0019 = skipSize;
	}

	internal bool _3Q(int P_0)
	{
		int num = P_0 - MinSize;
		bool flag = num >= 0 && P_0 <= MaxSize;
		if (SkipSize != 0)
		{
			if (flag)
			{
				return num % SkipSize == 0;
			}
			return false;
		}
		return flag;
	}

	internal static bool _3B(l[] P_0, int P_1)
	{
		foreach (l l2 in P_0)
		{
			if (l2._3Q(P_1))
			{
				return true;
			}
		}
		return false;
	}
}
