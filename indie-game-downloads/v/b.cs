using System;

namespace v;

internal abstract class b : h
{
	public new static b Create()
	{
		return new _000E();
	}

	public b()
	{
	}

	public abstract byte[] EncryptValue(byte[] rgb);

	public abstract byte[] DecryptValue(byte[] rgb);

	public abstract _0001 ExportParameters(bool include);

	public abstract void ImportParameters(_0001 parameters);

	internal void J(_0001 P_0)
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
internal interface B : IDisposable
{
	bool CanReuseTransform { get; }

	bool CanTransformMultipleBlocks { get; }

	int InputBlockSize { get; }

	int OutputBlockSize { get; }

	int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset);

	byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount);
}
