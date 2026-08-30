using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Library.Graphics;

public class TextureRenderer
{
	private RenderTarget2D m_RenderTarget;

	private DepthStencilBuffer m_DepthStencilBuffer;

	private DepthStencilBuffer m_DefaultDepthStencilBuffer;

	private GraphicsDevice m_GraphicsDevice;

	public TextureRenderer(GraphicsDevice graphicsDevice, int width, int height)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		base._002Ector();
		PresentationParameters presentationParameters = graphicsDevice.PresentationParameters;
		m_RenderTarget = new RenderTarget2D(graphicsDevice, width, height, 1, presentationParameters.BackBufferFormat, presentationParameters.MultiSampleType, presentationParameters.MultiSampleQuality);
		m_DepthStencilBuffer = new DepthStencilBuffer(graphicsDevice, width, height, presentationParameters.AutoDepthStencilFormat, presentationParameters.MultiSampleType, presentationParameters.MultiSampleQuality);
		m_GraphicsDevice = graphicsDevice;
	}

	public void Begin(Color clearColor)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Begin();
		m_GraphicsDevice.Clear(clearColor);
	}

	public void Begin()
	{
		m_DefaultDepthStencilBuffer = m_GraphicsDevice.DepthStencilBuffer;
		m_GraphicsDevice.DepthStencilBuffer = m_DepthStencilBuffer;
		m_GraphicsDevice.SetRenderTarget(0, m_RenderTarget);
	}

	public void End()
	{
		m_GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
		m_GraphicsDevice.DepthStencilBuffer = m_DefaultDepthStencilBuffer;
	}

	public Texture2D GetTexture()
	{
		return m_RenderTarget.GetTexture();
	}
}
