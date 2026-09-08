using Microsoft.Xna.Framework;

namespace Quasar.Meshes.Text;

public struct TextDrawProperties
{
	public float Scale;

	public Vector2 Offset;

	public float ZValue;

	public HorizontalAlignment Alignment;

	public TextDrawProperties(Vector2 offset, float scale)
	{
		ZValue = 0f;
		Scale = scale;
		Offset = offset;
		Alignment = HorizontalAlignment.Left;
	}

	public TextDrawProperties(Vector2 offset, float scale, HorizontalAlignment alignment)
	{
		ZValue = 0f;
		Scale = scale;
		Offset = offset;
		Alignment = alignment;
	}

	public TextDrawProperties(Vector2 offset)
	{
		ZValue = 0f;
		Scale = 1f;
		Offset = offset;
		Alignment = HorizontalAlignment.Left;
	}

	public TextDrawProperties(float scale, HorizontalAlignment alignment)
	{
		ZValue = 0f;
		Scale = scale;
		Offset = Vector2.Zero;
		Alignment = alignment;
	}

	public TextDrawProperties(HorizontalAlignment alignment)
	{
		ZValue = 0f;
		Scale = 1f;
		Offset = Vector2.Zero;
		Alignment = alignment;
	}

	public TextDrawProperties(Vector2 offset, HorizontalAlignment alignment)
	{
		ZValue = 0f;
		Scale = 1f;
		Offset = offset;
		Alignment = alignment;
	}
}
