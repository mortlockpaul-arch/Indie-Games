using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GameUtils.Tasks;
using Quasar.GameUtils.Template;
using Quasar.GameUtils.Template.Controls;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Template.Controls;

internal class HowToPlayItem : RenderItem
{
	private HowToPlay howToPlay;

	private TextBoxMesh tbm;

	private Sized2DRectangleMesh rMesh;

	private BorderedRectangle imageBorderMesh;

	private global::ScrollbarItem scrollbar;

	private static object lockObject = new object();

	public HowToPlayItem(HowToPlay howToPlay)
	{
		this.howToPlay = howToPlay;
		howToPlay.OnItemChanged += OnItemChanged;
		transform.Translation = new Vector3(howToPlay.Position + new Vector2(0.5f), 0f);
		float[] coordBorder = new float[4] { 16f, 16f, 16f, 16f };
		BorderedRectangle borderedRectangle = new BorderedRectangle(TextureManager.Textures["GUI/RoundBorderTex"], howToPlay.Size + new Vector2(0f, 16f), coordBorder)
		{
			Diffuse = Vector3.Zero,
			Alpha = 0.8f
		};
		addMesh(borderedRectangle);
		tbm = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(Vector2.Zero, howToPlay.Size, 1f, HorizontalAlignment.Left, VerticalAlignment.Center), 400, useStringBuilder: false);
		addMesh(tbm);
		rMesh = new Sized2DRectangleMesh(new Vector2(128f), Vector3.Zero);
		rMesh.Diffuse = Vector3.Zero;
		addMesh(rMesh);
		imageBorderMesh = new BorderedRectangle(TextureManager.Textures["GUI/ExtBorderMask"], new Vector2(128f), coordBorder);
		imageBorderMesh.FirstMaterial.Textures.Add(TextureManager.Textures["GUI/WoodLightTileTex"]);
		imageBorderMesh.Shader = ShaderManager.Shaders["GUIMask"];
		addMesh(imageBorderMesh);
		Layout2D.LayoutData layout = new Layout2D.LayoutData(new Vector2(0f, (0f - borderedRectangle.Size.Y) * 0.5f - 30f), new Vector2(borderedRectangle.Size.X, 42f));
		scrollbar = new global::ScrollbarItem(layout, vertical: false);
		scrollbar.TotalItems = howToPlay.ItemsCount;
		addChild(scrollbar);
		OnItemChanged();
	}

	private void LoadTexture(object parameters)
	{
		lock (lockObject)
		{
			rMesh.Texture = TextureManager.Textures["HowToPlay/" + (string)parameters];
			rMesh.Diffuse = Vector3.One;
			rMesh.Shader = ShaderManager.Shaders["GUI"];
		}
	}

	private void OnItemChanged()
	{
		HowToPlay.HowToPlayItem currentItem = howToPlay.CurrentItem;
		scrollbar.BaseItem = howToPlay.CurrentIndex;
		tbm.Alpha = 0f;
		tbm.Text = currentItem.Text;
		if (currentItem.HasImage)
		{
			rMesh.Size = new Vector2(howToPlay.Size.Y - 24f);
			rMesh.Offset = new Vector2(howToPlay.Size.X * 0.5f - 12f - howToPlay.Size.Y * 0.5f, 0f);
			if (TextureManager.Textures.TryGetValue("HowToPlay/" + currentItem.Image, out var _))
			{
				LoadTexture(currentItem.Image);
			}
			else
			{
				TaskManager.Post(LoadTexture, currentItem.Image);
			}
			tbm.Size = new Vector2(howToPlay.Size.X - howToPlay.Size.Y - 24f, howToPlay.Size.Y - 24f);
			tbm.Offset = new Vector2((0f - howToPlay.Size.Y) * 0.5f, 0f);
			imageBorderMesh.Offset = rMesh.Offset;
			imageBorderMesh.Size = rMesh.Size + new Vector2(16f);
			imageBorderMesh.Alpha = 1f;
		}
		else
		{
			rMesh.Size = new Vector2(0f);
			imageBorderMesh.Alpha = 0f;
			tbm.Size = new Vector2(howToPlay.Size.X - 24f, howToPlay.Size.Y - 24f);
			tbm.Offset = Vector2.Zero;
		}
	}

	protected override void DoUpdate()
	{
		tbm.Alpha = GameMath.Interpolate(tbm.Alpha, 1f, 0.1f);
		if (howToPlay.CurrentItem.HasImage)
		{
			Sized2DRectangleMesh sized2DRectangleMesh = rMesh;
			float alpha = (imageBorderMesh.Alpha = tbm.Alpha);
			sized2DRectangleMesh.Alpha = alpha;
		}
		base.DoUpdate();
	}
}
