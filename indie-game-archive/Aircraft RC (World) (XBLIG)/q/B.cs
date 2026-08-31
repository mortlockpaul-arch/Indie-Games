using Microsoft.Xna.Framework.Graphics;

namespace q;

internal class B
{
	internal static BlendState HCB;

	internal static BlendState HC_0002;

	internal static BlendState HC_0012;

	internal static BlendState HCH;

	internal static BlendState HC7;

	internal static DepthStencilState HC_0001;

	internal static DepthStencilState HCw;

	internal static DepthStencilState HCZ;

	internal static DepthStencilState HC_000F;

	internal static DepthStencilState HCy;

	internal static BlendState HC6;

	internal static DepthStencilState HCD;

	internal static DepthStencilState HC_0011;

	internal static DepthStencilState HCK;

	internal static DepthStencilState HC_0003;

	internal static BlendState HCk;

	internal static RasterizerState[,] HCs;

	internal static RasterizerState[,] HC_0013;

	static B()
	{
		HCB = new BlendState
		{
			ColorSourceBlend = Blend.One,
			AlphaSourceBlend = Blend.One,
			ColorDestinationBlend = Blend.One,
			AlphaDestinationBlend = Blend.One
		};
		HC_0002 = new BlendState();
		HC_0012 = new BlendState
		{
			ColorSourceBlend = Blend.One,
			ColorDestinationBlend = Blend.Zero,
			ColorWriteChannels = ColorWriteChannels.Alpha
		};
		HCH = new BlendState
		{
			ColorSourceBlend = Blend.DestinationAlpha,
			AlphaSourceBlend = Blend.DestinationAlpha,
			ColorDestinationBlend = Blend.One,
			AlphaDestinationBlend = Blend.One,
			ColorWriteChannels = (ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue)
		};
		HC7 = new BlendState
		{
			ColorSourceBlend = Blend.DestinationAlpha,
			ColorDestinationBlend = Blend.One,
			AlphaSourceBlend = Blend.One,
			AlphaDestinationBlend = Blend.Zero
		};
		HC_0001 = new DepthStencilState
		{
			DepthBufferEnable = true,
			DepthBufferWriteEnable = false,
			DepthBufferFunction = CompareFunction.Equal
		};
		HCw = new DepthStencilState
		{
			DepthBufferEnable = true,
			DepthBufferWriteEnable = false,
			DepthBufferFunction = CompareFunction.Greater
		};
		HCZ = new DepthStencilState
		{
			DepthBufferEnable = true,
			DepthBufferWriteEnable = false,
			DepthBufferFunction = CompareFunction.Less
		};
		HC_000F = new DepthStencilState
		{
			DepthBufferEnable = true,
			DepthBufferWriteEnable = false,
			DepthBufferFunction = CompareFunction.LessEqual
		};
		HCy = new DepthStencilState
		{
			DepthBufferEnable = true,
			DepthBufferWriteEnable = false,
			DepthBufferFunction = CompareFunction.Equal,
			StencilEnable = true,
			StencilFunction = CompareFunction.NotEqual,
			StencilPass = StencilOperation.Replace,
			StencilDepthBufferFail = StencilOperation.Keep,
			StencilFail = StencilOperation.Keep,
			StencilMask = int.MaxValue,
			StencilWriteMask = int.MaxValue,
			TwoSidedStencilMode = false
		};
		HC6 = new BlendState
		{
			ColorWriteChannels = ColorWriteChannels.None
		};
		HCD = new DepthStencilState
		{
			DepthBufferWriteEnable = false,
			StencilEnable = true,
			StencilFunction = CompareFunction.Always,
			StencilPass = StencilOperation.Replace,
			StencilFail = StencilOperation.Keep,
			StencilDepthBufferFail = StencilOperation.Keep,
			StencilMask = int.MaxValue,
			StencilWriteMask = int.MaxValue
		};
		HC_0011 = new DepthStencilState
		{
			DepthBufferWriteEnable = true,
			StencilEnable = true,
			StencilFunction = CompareFunction.Equal,
			StencilPass = StencilOperation.Keep,
			StencilFail = StencilOperation.Keep,
			StencilDepthBufferFail = StencilOperation.Keep,
			StencilMask = int.MaxValue,
			StencilWriteMask = int.MaxValue
		};
		HCK = new DepthStencilState
		{
			StencilEnable = true,
			StencilFunction = CompareFunction.Always,
			StencilPass = StencilOperation.Replace,
			StencilFail = StencilOperation.Keep,
			StencilDepthBufferFail = StencilOperation.Keep,
			StencilMask = int.MaxValue,
			StencilWriteMask = int.MaxValue
		};
		HC_0003 = new DepthStencilState
		{
			DepthBufferEnable = false,
			StencilEnable = true,
			StencilFunction = CompareFunction.Equal,
			StencilPass = StencilOperation.Keep,
			StencilFail = StencilOperation.Keep,
			StencilDepthBufferFail = StencilOperation.Keep,
			StencilMask = int.MaxValue,
			StencilWriteMask = int.MaxValue
		};
		HCk = new BlendState
		{
			ColorSourceBlend = Blend.SourceAlpha,
			ColorDestinationBlend = Blend.InverseSourceAlpha
		};
		HCs = new RasterizerState[3, 2];
		HC_0013 = new RasterizerState[3, 2];
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				RasterizerState rasterizerState = new RasterizerState
				{
					CullMode = (CullMode)i,
					FillMode = (FillMode)j
				};
				RasterizerState rasterizerState2 = new RasterizerState
				{
					CullMode = (CullMode)i,
					FillMode = (FillMode)j,
					ScissorTestEnable = true
				};
				HCs[i, j] = rasterizerState;
				HC_0013[i, j] = rasterizerState2;
			}
		}
	}

	internal static void Hm(GraphicsDevice P_0, FillMode P_1, CullMode P_2, bool P_3, bool P_4, bool P_5)
	{
		RasterizerState[,] array = ((!P_5) ? HCs : HC_0013);
		if (P_4)
		{
			P_0.RasterizerState = array[0, (int)P_1];
		}
		else if (P_2 == CullMode.None || !P_3)
		{
			P_0.RasterizerState = array[(int)P_2, (int)P_1];
		}
		else if (P_2 == CullMode.CullCounterClockwiseFace)
		{
			P_0.RasterizerState = array[1, (int)P_1];
		}
		else
		{
			P_0.RasterizerState = array[2, (int)P_1];
		}
	}
}
