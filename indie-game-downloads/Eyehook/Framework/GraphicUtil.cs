using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Eyehook.Framework;

public static class GraphicUtil
{
	public static DepthStencilState Stencil0Always;

	public static DepthStencilState Stencil1Always;

	public static DepthStencilState Stencil1Match;

	public static DepthStencilState Stencil1Increment;

	public static DepthStencilState Stencil2Match;

	public static SamplerState PointFilter;

	public static SamplerState Wrap;

	public static BlendState NoColorWrite;

	public static AlphaTestEffect AlphaGreaterThan0;

	public static void Initialize(Game game)
	{
		Stencil0Always = new DepthStencilState();
		Stencil0Always.StencilEnable = true;
		Stencil0Always.DepthBufferEnable = false;
		Stencil0Always.StencilFunction = CompareFunction.Always;
		Stencil0Always.StencilPass = StencilOperation.Replace;
		Stencil0Always.ReferenceStencil = 0;
		Stencil1Always = new DepthStencilState();
		Stencil1Always.StencilEnable = true;
		Stencil1Always.DepthBufferEnable = false;
		Stencil1Always.StencilFunction = CompareFunction.Always;
		Stencil1Always.StencilPass = StencilOperation.Replace;
		Stencil1Always.ReferenceStencil = 1;
		Stencil1Match = new DepthStencilState();
		Stencil1Match.StencilEnable = true;
		Stencil1Match.DepthBufferEnable = false;
		Stencil1Match.StencilFunction = CompareFunction.Equal;
		Stencil1Match.ReferenceStencil = 1;
		Stencil2Match = new DepthStencilState();
		Stencil2Match.StencilEnable = true;
		Stencil2Match.DepthBufferEnable = false;
		Stencil2Match.StencilFunction = CompareFunction.Equal;
		Stencil2Match.ReferenceStencil = 2;
		Stencil1Increment = new DepthStencilState();
		Stencil1Increment.StencilEnable = true;
		Stencil1Increment.DepthBufferEnable = false;
		Stencil1Increment.StencilFunction = CompareFunction.Always;
		Stencil1Increment.StencilPass = StencilOperation.Increment;
		Stencil1Increment.ReferenceStencil = 1;
		NoColorWrite = new BlendState();
		NoColorWrite.ColorWriteChannels = ColorWriteChannels.None;
		PointFilter = new SamplerState();
		PointFilter.Filter = TextureFilter.Point;
		PointFilter.AddressU = TextureAddressMode.Clamp;
		PointFilter.AddressV = TextureAddressMode.Clamp;
		Wrap = new SamplerState();
		Wrap.Filter = TextureFilter.Point;
		Wrap.AddressU = TextureAddressMode.Wrap;
		Wrap.AddressV = TextureAddressMode.Wrap;
		AlphaGreaterThan0 = new AlphaTestEffect(game.GraphicsDevice);
		AlphaGreaterThan0.AlphaFunction = CompareFunction.Greater;
		AlphaGreaterThan0.ReferenceAlpha = 0;
		AlphaGreaterThan0.VertexColorEnabled = true;
		AlphaGreaterThan0.World = Matrix.Identity;
		AlphaGreaterThan0.View = Matrix.Identity;
		AlphaGreaterThan0.Projection = Matrix.CreateOrthographicOffCenter(0f, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
	}
}
