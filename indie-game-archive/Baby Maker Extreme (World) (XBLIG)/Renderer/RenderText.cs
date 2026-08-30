namespace Renderer;

public class RenderText : DrawableComponent
{
	private SceneRenderer.textData m_data;

	public RenderText()
		: base(0f)
	{
	}

	public void Initialize(SceneRenderer.textData data, float depth)
	{
		m_data = data;
		base.Depth = depth;
	}

	public SceneRenderer.textData GetTextData()
	{
		return m_data;
	}
}
