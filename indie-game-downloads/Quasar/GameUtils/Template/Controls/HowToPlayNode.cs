using Microsoft.Xna.Framework;
using Quasar.GameUtils.Meshes;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Textures;

namespace Quasar.GameUtils.Template.Controls;

internal class HowToPlayNode : RenderItem
{
	private HowToPlay howToPlay;

	private TextBoxMesh tbm;

	private Sized2DRectangleMesh rMesh;

	public HowToPlayNode(HowToPlay howToPlay)
	{
		this.howToPlay = howToPlay;
		howToPlay.OnItemChanged += OnItemChanged;
		transform.Translation = new Vector3(howToPlay.Position + new Vector2(0.5f), 0f);
		HUDDetailRectangle m = new HUDDetailRectangle(howToPlay.Size);
		addMesh(m);
		tbm = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(Vector2.Zero, howToPlay.Size, 1f, HorizontalAlignment.Center, VerticalAlignment.Center), 400, useStringBuilder: false);
		addMesh(tbm);
		rMesh = new Sized2DRectangleMesh(new Vector2(128f), null);
		addMesh(rMesh);
		OnItemChanged();
	}

	private void OnItemChanged()
	{
		HowToPlay.HowToPlayItem currentItem = howToPlay.CurrentItem;
		tbm.Text = currentItem.Text;
		if (currentItem.HasImage)
		{
			rMesh.Size = new Vector2(howToPlay.Size.Y - 24f);
			rMesh.Offset = new Vector2((0f - howToPlay.Size.X) * 0.5f + 12f + howToPlay.Size.Y * 0.5f, 0f);
			rMesh.Texture = TextureManager.Textures["HowToPlay/" + currentItem.Image];
			tbm.Size = new Vector2(howToPlay.Size.X - howToPlay.Size.Y - 24f, howToPlay.Size.Y - 24f);
			tbm.Offset = new Vector2(howToPlay.Size.Y * 0.5f, 0f);
		}
		else
		{
			rMesh.Size = new Vector2(0f);
			tbm.Size = new Vector2(howToPlay.Size.X - 24f, howToPlay.Size.Y - 24f);
			tbm.Offset = Vector2.Zero;
		}
	}

	protected override void DoUpdate()
	{
		base.DoUpdate();
	}
}
