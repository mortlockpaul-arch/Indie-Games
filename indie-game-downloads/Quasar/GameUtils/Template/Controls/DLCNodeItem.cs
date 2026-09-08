using Microsoft.Xna.Framework;
using Quasar.GameUtils.Meshes;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template.Controls;

internal class DLCNodeItem : RenderItem
{
	private const int PROGRESS_WIDTH = 300;

	private const int PROGRESS_HEIGHT = 30;

	private const int PROGRESS_PADDING_X = 16;

	private const int PROGRESS_PADDING_Y = 12;

	private const int PROGRESS_BAR_WIDTH = 268;

	private const int PROGRESS_BAR_HEIGHT = 6;

	private const int PROGRESS_Y = -25;

	private TextMesh name;

	private TextMesh description;

	private TextMesh unlocked;

	public DLCNodeItem()
	{
		addMesh(new HUDDetailRectangle(new Vector2(800f, 80f)));
		name = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(HorizontalAlignment.Left), 40, useStringBuilder: false);
		name.Offset = new Vector2(-380f, 35f);
		name.Diffuse = GameTemplate.DialogTitleColor;
		addMesh(name);
		description = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-380f, 4f), HorizontalAlignment.Left), 100, useStringBuilder: false);
		addMesh(description);
		unlocked = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(HorizontalAlignment.Right), 30, useStringBuilder: false);
		unlocked.Offset = new Vector2(380f, 35f);
		addMesh(unlocked);
	}

	public void SetData(DLC.DLCItem item)
	{
		name.Text = item.Title;
		description.Text = item.Description;
		name.Diffuse = (item.Unlocked ? GameTemplate.DialogTitleColor : GameTemplate.DialogTitleColorDisabled);
		if (item.Unlocked)
		{
			unlocked.Text = LanguageManager.Texts["DLC_UNLOCKED"];
		}
		else
		{
			unlocked.Text = GameMath.IntToString(item.TargetSales, "#,0") + " " + "DLC_SALES".Translate();
		}
		Visible = true;
	}

	public void Reset()
	{
		Visible = false;
	}
}
