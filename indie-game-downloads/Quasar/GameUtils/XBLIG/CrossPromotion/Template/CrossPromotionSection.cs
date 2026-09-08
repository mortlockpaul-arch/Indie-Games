using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Sections;

namespace Quasar.GameUtils.XBLIG.CrossPromotion.Template;

public class CrossPromotionSection : GUISection
{
	public CrossPromotionSection(PlayerIndex playerIndex, int sectionId, int previousSectionId, string template)
		: base(sectionId, new Layout(string.Empty, "Cross Promotion", template))
	{
		base.Layout.OnCancel += delegate
		{
			BaseGame.Instance.NextGameSectionId = previousSectionId;
			return true;
		};
	}

	public override void MainLoop()
	{
		base.MainLoop();
	}
}
