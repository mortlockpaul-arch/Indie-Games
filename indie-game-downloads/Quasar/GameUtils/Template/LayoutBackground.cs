using Quasar.Global;
using Quasar.Items._2D;
using Quasar.Textures;

namespace Quasar.GameUtils.Template;

internal class LayoutBackground : Element
{
	private Rectangle background;

	public LayoutBackground()
	{
		background = new Rectangle(TextureManager.Textures["GUI/Background"], Engine.BackBufferSize);
		addChild(background);
	}

	protected override void DoUpdate()
	{
		base.DoUpdate();
	}
}
