namespace Renderer;

public class RenderText : DrawableComponent
{
	private textData m_data;

	public RenderText()
		: base(0f)
	{
	}

	public void Initialize(textData data, float depth)
	{
		m_data = data;
		base.Depth = depth;
	}

	public textData GetTextData()
	{
		return m_data;
	}
}
