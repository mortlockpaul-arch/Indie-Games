using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace u;

internal class y
{
	protected class _0001CB
	{
		internal SystemStatistic HCB = SystemConsole.GetStatistic("Renderer_BatchSamplerChanges", SystemStatisticCategory.Rendering);
	}

	protected _0001CB Statistics = new _0001CB();

	private bool HCB = true;

	private SamplerState HC_0002;

	private static Dictionary<int, SamplerState> HC_0012 = new Dictionary<int, SamplerState>(16);

	internal void _76()
	{
		HCB = true;
	}

	internal static void _7D(GraphicsDevice P_0)
	{
		for (int i = 0; i < 16; i++)
		{
			P_0.SamplerStates[i] = SamplerState.PointClamp;
		}
		for (int j = 0; j < 2; j++)
		{
			P_0.VertexSamplerStates[j] = SamplerState.PointWrap;
		}
	}

	internal void _7_0011(GraphicsDevice P_0, SamplerState P_1)
	{
		if (!HCB && HC_0002 == P_1)
		{
			return;
		}
		Statistics.HCB.AccumulationValue++;
		HCB = false;
		HC_0002 = P_1;
		for (int i = 0; i < 16; i++)
		{
			Texture texture = P_0.Textures[i];
			if (texture != null && texture.Format >= SurfaceFormat.Single)
			{
				P_0.SamplerStates[i] = SamplerState.PointClamp;
			}
			else
			{
				P_0.SamplerStates[i] = P_1;
			}
		}
		for (int j = 0; j < 2; j++)
		{
			P_0.VertexSamplerStates[j] = SamplerState.PointWrap;
		}
	}

	internal SamplerState _7K(GraphicsDevice P_0, TextureAddressMode P_1, TextureAddressMode P_2, TextureAddressMode P_3, TextureFilter P_4, int P_5)
	{
		int key = (int)(P_1 + ((int)P_2 << 2) + ((int)P_3 << 4) + ((int)P_4 << 6) + (P_5 << 10));
		if (!HC_0012.TryGetValue(key, out var value))
		{
			value = new SamplerState();
			value.AddressU = P_1;
			value.AddressV = P_2;
			value.AddressW = P_3;
			value.Filter = P_4;
			value.MaxAnisotropy = P_5;
			HC_0012.Add(key, value);
		}
		return value;
	}
}
