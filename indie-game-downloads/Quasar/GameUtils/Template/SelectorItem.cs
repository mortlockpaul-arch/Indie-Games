using Quasar.GUI.Controls.GroupControls;

namespace Quasar.GameUtils.Template;

public class SelectorItem : ButtonItem
{
	private Selector selector;

	public SelectorItem(Selector selector)
		: base(selector)
	{
		selector.OnChange += OnChange;
		this.selector = selector;
	}

	private void OnChange(Selector selectorChanged, int selection, string value)
	{
		GameTemplate.SelectorAudio.Start();
	}

	protected override void DoUpdate()
	{
		base.DoUpdate();
		text.StringBuilder.Append(selector.CanSelectPrevious ? "< " : " ");
		text.StringBuilder.Append(selector.CurrentOptionValue);
		text.StringBuilder.Append(selector.CanSelectNext ? " >" : "");
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
