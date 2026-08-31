using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Provides basic skinned animation rendering support.
/// </summary>
public abstract class BaseSkinnedEffect : BaseRenderableEffect, ISkinnedEffect
{
	private bool HCB;

	private Matrix[] HC_0002;

	private EffectParameter HC_0012;

	private Matrix[] HCH = new Matrix[1];

	/// <summary>
	/// Array of bone transforms for the skeleton's current pose. The matrix index is the
	/// same as the bone order used in the model or vertex buffer.
	/// </summary>
	public Matrix[] SkinBones
	{
		get
		{
			return HC_0002;
		}
		set
		{
			if (value != null)
			{
				_UpdatedByBatch = true;
				EffectHelper._0012Q(value, ref HC_0002, ref HC_0012);
			}
			else
			{
				if (!HCB || HC_0012 == null)
				{
					return;
				}
				if (HCH.Length < HC_0012.Elements.Count)
				{
					HCH = new Matrix[HC_0012.Elements.Count];
					for (int i = 0; i < HCH.Length; i++)
					{
						ref Matrix reference = ref HCH[i];
						reference = Matrix.Identity;
					}
				}
				if (HC_0002 != HCH)
				{
					_UpdatedByBatch = true;
					HC_0002 = HCH;
					HC_0012.SetValue(HC_0002);
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
			return HCB;
		}
		set
		{
			if (value != HCB)
			{
				HCB = value;
				SetTechnique();
				if (HCB && HC_0002 == null)
				{
					SkinBones = null;
				}
			}
		}
	}

	internal BaseSkinnedEffect(GraphicsDevice P_0, string P_1)
		: base(P_0, P_1)
	{
		HC_0012 = base.Parameters["_SkinBones"];
	}
}
