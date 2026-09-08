using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Provides basic skinned animation rendering support.
/// </summary>
public abstract class BaseSkinnedEffect : BaseRenderableEffect, ISkinnedEffect
{
	private bool _3A_0018;

	private Matrix[] _3AL;

	private EffectParameter _3A_0019;

	private Matrix[] _3A3 = new Matrix[1];

	/// <summary>
	/// Array of bone transforms for the skeleton's current pose. The matrix index is the
	/// same as the bone order used in the model or vertex buffer.
	/// </summary>
	public Matrix[] SkinBones
	{
		get
		{
			return _3AL;
		}
		set
		{
			if (value != null)
			{
				_UpdatedByBatch = true;
				EffectHelper._0019_000E(value, ref _3AL, ref _3A_0019);
			}
			else
			{
				if (!_3A_0018 || _3A_0019 == null)
				{
					return;
				}
				if (_3A3.Length < _3A_0019.Elements.Count)
				{
					_3A3 = new Matrix[_3A_0019.Elements.Count];
					for (int i = 0; i < _3A3.Length; i++)
					{
						ref Matrix reference = ref _3A3[i];
						reference = Matrix.Identity;
					}
				}
				if (_3AL != _3A3)
				{
					_UpdatedByBatch = true;
					_3AL = _3A3;
					_3A_0019.SetValue(_3AL);
				}
			}
		}
	}

	/// <summary>
	/// Determines if the effect is currently rendering skinned objects.
	/// </summary>
	public bool Skinned
	{
		get
		{
			return _3A_0018;
		}
		set
		{
			if (value != _3A_0018)
			{
				_3A_0018 = value;
				SetTechnique();
				if (_3A_0018 && _3AL == null)
				{
					SkinBones = null;
				}
			}
		}
	}

	internal BaseSkinnedEffect(GraphicsDevice P_0, string P_1)
		: base(P_0, P_1)
	{
		_3A_0019 = base.Parameters["_SkinBones"];
	}
}
