using Microsoft.Xna.Framework;
using Quasar.GUI.Controls.GroupControls;
using Quasar.GameUtils.Template;

namespace AvatarFarmOnline.Template;

public class MenuSelectorItem : MenuButtonItem
{
	private Selector selector;

	public MenuSelectorItem(Selector selector)
		: base(selector)
	{
		selector.OnChange += OnChange;
		this.selector = selector;
		updateData();
	}

	private void OnChange(Selector selectorChanged, int selection, string value)
	{
		GameTemplate.SelectorAudio.Start();
	}

	protected override void OnInteraction(Button buttonPressed, PlayerIndex whoPressed)
	{
		GameTemplate.SelectorAudio.Start();
	}

	private void updateData()
	{
		text.StringBuilder.Length = 0;
		text.StringBuilder.Append(selector.Text);
		text.StringBuilder.Append(": ");
		text.StringBuilder.Append(selector.CurrentOptionValue);
		updateBg(AvatarFarmOnline.Template.ExtendedGameTemplate.MenuFont.MeasureString(text.StringBuilder));
	}

	protected override void DoUpdate()
	{
		base.DoUpdate();
		updateData();
	}

	public override void Dispose()
	{
		if (selector != null)
		{
			selector.OnChange -= OnChange;
		}
		selector = null;
		base.Dispose();
	}
}
