using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace X;

internal class t
{
	protected class DA_0018
	{
		internal SystemStatistic _3A_0018 = SystemConsole.GetStatistic("Renderer_BatchSamplerChanges", SystemStatisticCategory.Rendering);
	}

	protected DA_0018 Statistics = new DA_0018();

	private bool _3A_0018 = true;

	private SamplerState _3AL;

	private static Dictionary<int, SamplerState> _3A_0019 = new Dictionary<int, SamplerState>(16);

	internal void _6F()
	{
		_3A_0018 = true;
	}

	internal static void _6c(GraphicsDevice P_0)
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

	internal void _6g(GraphicsDevice P_0, SamplerState P_1)
	{
		if (!_3A_0018 && _3AL == P_1)
		{
			return;
		}
		Statistics._3A_0018.AccumulationValue++;
		_3A_0018 = false;
		_3AL = P_1;
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

	internal SamplerState _6I(GraphicsDevice P_0, TextureAddressMode P_1, TextureAddressMode P_2, TextureAddressMode P_3, TextureFilter P_4, int P_5)
	{
		int key = (int)(P_1 + ((int)P_2 << 2) + ((int)P_3 << 4) + ((int)P_4 << 6) + (P_5 << 10));
		if (!_3A_0019.TryGetValue(key, out var value))
		{
			value = new SamplerState();
			value.AddressU = P_1;
			value.AddressV = P_2;
			value.AddressW = P_3;
			value.Filter = P_4;
			value.MaxAnisotropy = P_5;
			_3A_0019.Add(key, value);
		}
		return value;
	}
}
