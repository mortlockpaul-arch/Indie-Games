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
	private byte[] _3A_0018;

	/// <summary>
	/// Effect byte code used to construct new effect objects.
	/// </summary>
	public byte[] ByteCode
	{
		[CompilerGenerated]
		get
		{
			return _3A_0018;
		}
		[CompilerGenerated]
		protected set
		{
			_3A_0018 = value;
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
