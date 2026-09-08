using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Shaders;
using Quasar.Textures;

internal class ScrollbarItem : RenderItem
{
	private Layout2D.LayoutData markLayout;

	private BorderedRectangle mark;

	private bool vertical = true;

	private Vector2 target;

	private int totalTicks = -1;

	private int markerSize = 1;

	private int targetTick;

	public int BaseItem
	{
		set
		{
			if (value >= 0)
			{
				int max = totalTicks - markerSize;
				int num = GameMath.Clamp(0, max, value);
				float num2 = (float)num / (float)totalTicks;
				targetTick = value;
				if (vertical)
				{
					target = markLayout.Top - new Vector2(0f, mark.Size.Y * 0.5f + num2 * markLayout.size.Y);
				}
				else
				{
					target = markLayout.Left + new Vector2(mark.Size.X * 0.5f + num2 * markLayout.size.X, 0f);
				}
			}
		}
	}

	public int TotalItems
	{
		set
		{
			if (value > 0 && totalTicks != value)
			{
				totalTicks = value;
				MarkerSize = markerSize;
			}
		}
	}

	public int MarkerSize
	{
		set
		{
			if (value > 0)
			{
				markerSize = value;
				float num = GameMath.Saturate((float)value / (float)totalTicks);
				if (vertical)
				{
					mark.Size = new Vector2(mark.Size.X, markLayout.Height * num);
				}
				else
				{
					mark.Size = new Vector2(markLayout.Width * num, mark.Size.Y);
				}
				BaseItem = 0;
				mark.Offset = target;
			}
		}
	}

	public ScrollbarItem(Layout2D.LayoutData layout, bool vertical)
	{
		this.vertical = vertical;
		float[] coordBorder = new float[4] { 16f, 16f, 16f, 16f };
		addMesh(new BorderedRectangle(TextureManager.Textures["GUI/RoundBorderTex"], layout, coordBorder)
		{
			Diffuse = Vector3.Zero,
			Alpha = 0.5f
		});
		Layout2D.InsideBorderLayout(layout, 5f, out var result);
		addMesh(new BorderedRectangle(TextureManager.Textures["GUI/ExtBorderMask"], result, coordBorder)
		{
			FirstMaterial = 
			{
				Textures = { (Texture)TextureManager.Textures["GUI/WoodTileTex"] }
			},
			Shader = ShaderManager.Shaders["GUIMask"]
		});
		float[] coordBorder2 = new float[4] { 4f, 4f, 4f, 4f };
		Layout2D.InsideBorderLayout(result, 12f, out markLayout);
		mark = new BorderedRectangle(TextureManager.Textures["GUI/RoundBorderSmallTex"], markLayout, coordBorder2);
		mark.FirstMaterial.Textures.Add(TextureManager.Textures["GUI/WoodLightTileTex"]);
		mark.Shader = ShaderManager.Shaders["GUIMask"];
		addMesh(mark);
		TotalItems = 6;
		BaseItem = 2;
	}

	protected override void DoUpdate()
	{
		Visible = totalTicks > markerSize;
		mark.Offset = Vector2.Lerp(mark.Offset, target, 0.2f);
		base.DoUpdate();
	}
}
