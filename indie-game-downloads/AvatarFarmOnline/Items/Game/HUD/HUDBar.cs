using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Meshes;
using Quasar.Textures;

namespace AvatarFarmOnline.Items.Game.HUD;

internal class HUDBar : RenderItem
{
	private Sized2DRectangleMesh iconMesh;

	private BorderedRectangle bar;

	private BorderedRectangle bgBar;

	private Sized2DRectangleMesh fgBar;

	public HUDBar(string icon, string barColor)
	{
		iconMesh = new Sized2DRectangleMesh(new Vector2(48f), TextureManager.Textures[icon]);
		iconMesh.Offset = new Vector2(24f, 0f);
		addMesh(iconMesh);
		bgBar = new BorderedRectangle(TextureManager.Textures["HUD/BarBG"], new Vector2(256f, 48f), new float[4] { 17f, 24f, 17f, 24f });
		bgBar.Offset = new Vector2(176f, 0f);
		addMesh(bgBar);
		bar = new BorderedRectangle(TextureManager.Textures[barColor], new Vector2(256f, 48f), new float[4] { 17f, 24f, 17f, 24f });
		bar.Offset = new Vector2(176f, 0f);
		addMesh(bar);
		fgBar = new Sized2DRectangleMesh(new Vector2(256f, 48f), TextureManager.Textures["HUD/BarFG"]);
		fgBar.Offset = new Vector2(176f, 0f);
		addMesh(fgBar);
	}

	protected void SetTextures(string icon, string color)
	{
		iconMesh.Texture = TextureManager.Textures[icon];
		bar.Texture = TextureManager.Textures[color];
	}

	protected void SetValue(float percent)
	{
		float num = 222f * percent;
		if (num > 0f)
		{
			num += 34f;
		}
		bar.Size = new Vector2(num, 48f);
		bar.Offset = new Vector2(48f + num * 0.5f, 0f);
	}
}
