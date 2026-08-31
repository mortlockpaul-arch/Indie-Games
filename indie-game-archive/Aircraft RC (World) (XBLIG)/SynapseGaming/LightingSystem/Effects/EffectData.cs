using System;
using System.Runtime.CompilerServices;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Provides direct access to effect byte code.
///
/// Used to fix issues with XNA 4.0 effects.
/// </summary>
public class EffectData : IDisposable
{
	[CompilerGenerated]
	private byte[] HCB;

	/// <summary>
	/// Effect byte code used to construct new effect objects.
	/// </summary>
	public byte[] ByteCode
	{
		[CompilerGenerated]
		get
		{
			return HCB;
		}
		[CompilerGenerated]
		protected set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// Creates an EffectData instance.
	/// </summary>
	/// <param name="bytecode">Effect byte code used to construct new effect objects.</param>
	public EffectData(byte[] bytecode)
	{
		ByteCode = bytecode;
	}

	/// <summary>
	/// Releases unmanaged resources used by the EffectData.
	/// </summary>
	public void Dispose()
	{
	}
}
