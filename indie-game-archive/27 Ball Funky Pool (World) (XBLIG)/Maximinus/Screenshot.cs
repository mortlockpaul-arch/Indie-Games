using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class Screenshot
{
	private GraphicsDevice device;

	public Screenshot(GraphicsDevice d)
		: this(d, "", "")
	{
	}

	public Screenshot(GraphicsDevice d, string gameName)
		: this(d, gameName, "")
	{
	}

	public Screenshot(GraphicsDevice d, string gameName, string shotName)
	{
		device = d;
	}

	private void saveCallback()
	{
	}

	private void killThread(int taskId)
	{
	}
}
