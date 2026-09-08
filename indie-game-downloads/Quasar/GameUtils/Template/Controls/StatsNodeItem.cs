using Microsoft.Xna.Framework;
using Quasar.GameUtils.Stats;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template.Controls;

internal class StatsNodeItem : RenderItem
{
	private TextMesh name;

	private TextMesh progress;

	public StatsNodeItem()
	{
		name = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(HorizontalAlignment.Left), 40, useStringBuilder: false);
		name.Offset = new Vector2(-300f, 0f);
		name.Diffuse = GameTemplate.DialogTitleColor;
		addMesh(name);
		progress = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(300f, 0f), HorizontalAlignment.Right), 80, useStringBuilder: true);
		addMesh(progress);
	}

	public void SetData(StatProgress item)
	{
		name.Text = item.Stat.Name;
		progress.StringBuilder.Length = 0;
		item.Stat.GetProgressText(progress.StringBuilder, item.Progress, item.Count);
		name.Diffuse = GameTemplate.DialogTitleColor;
		Visible = true;
	}

	public void Reset()
	{
		Visible = false;
	}
}
