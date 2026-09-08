using System;
using AvatarFarmOnline.Logic.Stage;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Textures;

namespace AvatarFarmOnline.Items.Game.HUD;

internal class SavingMessage : RenderItem
{
	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private Sized2DRectangleMesh rm;

	public SavingMessage(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		Layout2D.LayoutData layout = new Layout2D.LayoutData(new Vector2(Engine.GUIWidth * 0.45f - 64f, (0f - Engine.GUIHeight) * 0.45f + 120f + 44f + 20f + 32f), new Vector2(64f));
		rm = new Sized2DRectangleMesh(layout, TextureManager.Textures["HUD/HUDBigIcons"]);
		rm.SetTile(12, 4, 4);
		addMesh(rm);
	}

	protected override void DoUpdate()
	{
		Visible = stage.LastSave != 0 && stage.Timer.TimeSince(stage.LastSave) < 3000;
		if (Visible)
		{
			transform.Translation = new Vector3(0f, (float)Math.Abs(Math.Sin(Timer.DefaultTimer.TotalTimeSeconds * 8f)) * 30f, 0f);
		}
		base.DoUpdate();
	}
}
