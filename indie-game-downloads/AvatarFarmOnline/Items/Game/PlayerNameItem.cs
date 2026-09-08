using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Template;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Behaviors;
using Quasar.Global;
using Quasar.Meshes.Text;

namespace AvatarFarmOnline.Items.Game;

internal class PlayerNameItem : RenderItem
{
	private AvatarFarmOnline.Logic.Stage.Player player;

	private TextMesh text;

	public PlayerNameItem(AvatarFarmOnline.Logic.Stage.Player player)
	{
		this.player = player;
		text = new TextMesh(AvatarFarmOnline.Template.ExtendedGameTemplate.HUDFont, new TextDrawProperties(0.002f, HorizontalAlignment.Center), 32, useStringBuilder: true);
		text.Text = player.Selection.PlayerName;
		text.FirstMaterial.RenderPriority = Material.Priority.Low;
		text.Diffuse = GameMath.RGBToVector(233, 213, 23);
		addMesh(text);
		addBehavior(new SphericalBillboardBehavior());
		Transform.Translation = new Vector3(0f, 1.8f, 0f);
	}

	protected override void DoUpdate()
	{
		if (player.Stage.IsOnShop)
		{
			Visible = false;
		}
		else
		{
			float num = Vector3.Distance(Transform.WorldTranslation, Scene.CurrentInstance.Camera.Transform.WorldTranslation);
			switch (player.Stage.LocalPlayer.CameraState)
			{
			default:
				text.Scale = num * 0.0008f;
				if (num > 40f)
				{
					Visible = false;
				}
				else if (num > 30f)
				{
					Visible = true;
					text.Alpha = 1f - (num - 30f) / 10f;
				}
				else if (num > 5f)
				{
					Visible = true;
					text.Alpha = 1f;
				}
				else
				{
					Visible = true;
					text.Alpha = num * 0.2f;
				}
				break;
			case AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.Isometric:
				Visible = true;
				text.Alpha = 1f;
				text.Scale = 0.009f;
				break;
			case AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.World:
				Visible = false;
				break;
			}
		}
		base.DoUpdate();
	}
}
