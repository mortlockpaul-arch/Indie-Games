using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Meshes.Text;

public struct TextBoxDrawProperties
{
	public HorizontalAlignment HorizontalAlignment;

	public VerticalAlignment VerticalAlignment;

	public float LineSpacing;

	public float ZValue;

	public Vector2 Size;

	public Vector2 Offset;

	public float Scale;

	public Layout2D.LayoutData Layout
	{
		get
		{
			return new Layout2D.LayoutData(Offset, Size);
		}
		set
		{
			Offset = value.position;
			Size = value.size;
		}
	}

	public TextBoxDrawProperties(Vector2 offset, Vector2 Size, float scale, HorizontalAlignment alignment, VerticalAlignment vAlign)
	{
		ZValue = 0f;
		HorizontalAlignment = alignment;
		this.Size = Size;
		Scale = scale;
		VerticalAlignment = vAlign;
		Offset = offset;
		LineSpacing = 1f;
	}

	public TextBoxDrawProperties(Vector2 Size, float scale, HorizontalAlignment alignment, VerticalAlignment vAlign)
	{
		ZValue = 0f;
		HorizontalAlignment = alignment;
		this.Size = Size;
		Scale = scale;
		VerticalAlignment = vAlign;
		Offset = Vector2.Zero;
		LineSpacing = 1f;
	}

	public TextBoxDrawProperties(Vector2 offset, Vector2 Size, HorizontalAlignment alignment, VerticalAlignment vAlign)
	{
		ZValue = 0f;
		HorizontalAlignment = alignment;
		this.Size = Size;
		Scale = 1f;
		VerticalAlignment = vAlign;
		Offset = offset;
		LineSpacing = 1f;
	}

	public TextBoxDrawProperties(Vector2 Size, HorizontalAlignment alignment, VerticalAlignment vAlign)
	{
		ZValue = 0f;
		HorizontalAlignment = alignment;
		this.Size = Size;
		Scale = 1f;
		VerticalAlignment = vAlign;
		Offset = Vector2.Zero;
		LineSpacing = 1f;
	}

	public TextBoxDrawProperties(Layout2D.LayoutData layout, HorizontalAlignment alignment, VerticalAlignment vAlign)
	{
		ZValue = 0f;
		HorizontalAlignment = alignment;
		Size = layout.size;
		Scale = 1f;
		VerticalAlignment = vAlign;
		Offset = layout.position;
		LineSpacing = 1f;
	}

	public TextBoxDrawProperties(Layout2D.LayoutData layout, float scale, HorizontalAlignment alignment, VerticalAlignment vAlign)
	{
		ZValue = 0f;
		HorizontalAlignment = alignment;
		Size = layout.size;
		Scale = scale;
		VerticalAlignment = vAlign;
		Offset = layout.position;
		LineSpacing = 1f;
	}
}
