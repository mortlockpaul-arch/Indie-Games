using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Provides methods that help with effect setup and data synchronization.
/// </summary>
public class EffectHelper
{
	internal static void _0012I(Matrix P_0, ref Matrix P_1, ref EffectParameter P_2)
	{
		if (P_2 == null || P_0 == P_1)
		{
			P_1 = P_0;
			return;
		}
		P_1 = P_0;
		P_2.SetValue(P_1);
	}

	internal static void _0012Q(Matrix[] P_0, ref Matrix[] P_1, ref EffectParameter P_2)
	{
		P_1 = P_0;
		if (P_2 != null && P_0 != null)
		{
			Math.Min(P_0.Length, P_2.Elements.Count);
			P_2.SetValue(P_0);
		}
	}

	internal static void _0012_0016(Matrix P_0, ref Matrix P_1, ref Matrix P_2, ref EffectParameter P_3, ref EffectParameter P_4)
	{
		if (!(P_0 == P_1))
		{
			P_1 = P_0;
			if (P_3 != null)
			{
				P_3.SetValue(P_1);
			}
			if (P_4 != null)
			{
				P_2 = Matrix.Invert(P_1);
				P_4.SetValue(P_2);
			}
		}
	}

	internal static void _0012v(Vector4 P_0, ref Vector4 P_1, ref EffectParameter P_2)
	{
		if (P_2 != null && !(P_0 == P_1))
		{
			P_1 = P_0;
			P_2.SetValue(P_1);
		}
	}

	internal static void _00122(Vector3 P_0, ref Vector3 P_1, ref EffectParameter P_2)
	{
		if (P_2 != null && !(P_0 == P_1))
		{
			P_1 = P_0;
			P_2.SetValue(P_1);
		}
	}

	internal static void _0012_0005(int P_0, ref int P_1, ref EffectParameter P_2)
	{
		if (P_2 != null && P_0 != P_1)
		{
			P_1 = P_0;
			P_2.SetValue(P_1);
		}
	}

	internal static void _00124(float P_0, ref float P_1, ref EffectParameter P_2)
	{
		if (P_2 != null && P_0 != P_1)
		{
			P_1 = P_0;
			P_2.SetValue(P_1);
		}
	}

	internal static void _0012x(Vector2 P_0, ref Vector2 P_1, ref EffectParameter P_2)
	{
		if (P_2 != null && !(P_0 == P_1))
		{
			P_1 = P_0;
			P_2.SetValue(P_1);
		}
	}

	internal static void _00120(Texture2D P_0, ref Texture2D P_1, ref EffectParameter P_2)
	{
		if (P_2 != null && P_0 != P_1)
		{
			P_1 = P_0;
			P_2.SetValue(P_1);
		}
	}

	internal static void _00120(Texture2D P_0, Texture2D P_1, ref Texture2D P_2, ref EffectParameter P_3)
	{
		if (P_0 == null)
		{
			P_0 = P_1;
		}
		if (P_3 != null && P_0 != P_2)
		{
			P_2 = P_0;
			P_3.SetValue(P_2);
		}
	}

	internal static void _0012v(List<EffectParameter> P_0, Vector4 P_1)
	{
		if (P_0 == null || P_0.Count < 1)
		{
			return;
		}
		for (int i = 0; i < P_0.Count; i++)
		{
			EffectParameter effectParameter = P_0[i];
			if (effectParameter.ParameterType == EffectParameterType.Int32 && effectParameter.RowCount == 1)
			{
				effectParameter.SetValue((int)P_1.X);
			}
			if (effectParameter.ParameterType == EffectParameterType.Single && effectParameter.RowCount <= 1)
			{
				if (effectParameter.ColumnCount == 1)
				{
					effectParameter.SetValue(P_1.X);
				}
				else if (effectParameter.ColumnCount == 2)
				{
					effectParameter.SetValue(new Vector2(P_1.X, P_1.Y));
				}
				else if (effectParameter.ColumnCount == 3)
				{
					effectParameter.SetValue(new Vector3(P_1.X, P_1.Y, P_1.Z));
				}
				else
				{
					effectParameter.SetValue(P_1);
				}
			}
		}
	}

	internal static void _0012I(List<EffectParameter> P_0, Matrix P_1)
	{
		if (P_0 != null && P_0.Count >= 1)
		{
			for (int i = 0; i < P_0.Count; i++)
			{
				P_0[i].SetValue(P_1);
			}
		}
	}

	internal static void _0012m(List<EffectParameter> P_0, Matrix P_1)
	{
		if (P_0 != null && P_0.Count >= 1)
		{
			_0012I(P_0, Matrix.Transpose(P_1));
		}
	}

	/// <summary>
	/// Synchronizes all recognized object effect properties with the shadow effect.
	/// Allows shadow effects to support material transparency.
	/// </summary>
	/// <param name="objeffect">The object's effect</param>
	/// <param name="shadoweffect">The shadow effect</param>
	public static void SyncObjectAndShadowEffects(Effect objeffect, Effect shadoweffect)
	{
		EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(objeffect);
		EffectTypeCaster effectTypeCaster2 = OptimizationSystem.EffectTypeCasters.Get(shadoweffect);
		_0012C(effectTypeCaster, effectTypeCaster2);
	}

	internal static void _0012C(EffectTypeCaster P_0, EffectTypeCaster P_1)
	{
		if (P_1.RenderableEffect != null)
		{
			if (P_0.RenderableEffect != null)
			{
				P_1.RenderableEffect.DoubleSided = P_0.RenderableEffect.DoubleSided;
			}
			else
			{
				P_1.RenderableEffect.DoubleSided = false;
			}
		}
		if (P_1.TransparentEffect != null)
		{
			ITransparentEffect transparentEffect = P_1.TransparentEffect;
			if (P_0.TransparentEffect != null)
			{
				ITransparentEffect transparentEffect2 = P_0.TransparentEffect;
				if (transparentEffect2.TransparencyMode != TransparencyMode.None)
				{
					transparentEffect.SetTransparencyModeAndMap(transparentEffect2.TransparencyMode, transparentEffect2.TransparencyThreshold, transparentEffect2.TransparencyMap);
				}
				else
				{
					transparentEffect.SetTransparencyModeAndMap(TransparencyMode.None, transparentEffect.TransparencyThreshold, null);
				}
			}
			else
			{
				transparentEffect.SetTransparencyModeAndMap(TransparencyMode.None, transparentEffect.TransparencyThreshold, null);
			}
		}
		if (P_1.AddressableEffect != null)
		{
			IAddressableEffect addressableEffect = P_1.AddressableEffect;
			if (P_0.AddressableEffect != null)
			{
				IAddressableEffect addressableEffect2 = P_0.AddressableEffect;
				addressableEffect.AddressModeU = addressableEffect2.AddressModeU;
				addressableEffect.AddressModeV = addressableEffect2.AddressModeV;
				addressableEffect.AddressModeW = addressableEffect2.AddressModeW;
			}
			else
			{
				addressableEffect.AddressModeU = TextureAddressMode.Wrap;
				addressableEffect.AddressModeV = TextureAddressMode.Wrap;
				addressableEffect.AddressModeW = TextureAddressMode.Wrap;
			}
		}
		if (P_1.TerrainEffect != null)
		{
			ITerrainEffect terrainEffect = P_1.TerrainEffect;
			if (P_0.TerrainEffect != null)
			{
				ITerrainEffect terrainEffect2 = P_0.TerrainEffect;
				terrainEffect.HeightMapTexture = terrainEffect2.HeightMapTexture;
				terrainEffect.HeightScale = terrainEffect2.HeightScale;
				terrainEffect.MeshSegments = terrainEffect2.MeshSegments;
				terrainEffect.TileRepeatCount = terrainEffect2.TileRepeatCount;
				terrainEffect.Tiling = terrainEffect2.Tiling;
			}
			else
			{
				terrainEffect.HeightMapTexture = null;
			}
		}
	}
}
