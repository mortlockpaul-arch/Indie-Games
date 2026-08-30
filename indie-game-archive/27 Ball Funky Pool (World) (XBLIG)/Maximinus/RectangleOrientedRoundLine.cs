using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class RectangleOrientedRoundLine
{
	public List<RoundLine> list;

	private Vector2 P0;

	private Vector2 P1;

	private Vector2 P2;

	private Vector2 P3;

	private Vector2 position;

	private double rotation;

	public RectangleOrientedRoundLine(Vector2 size, Vector2 pos, float rotation)
	{
		list = new List<RoundLine>();
		P0 = new Vector2((0f - size.X) / 2f, (0f - size.Y) / 2f) + pos;
		P1 = new Vector2((0f - size.X) / 2f, size.Y / 2f) + pos;
		P2 = new Vector2(size.X / 2f, size.Y / 2f) + pos;
		P3 = new Vector2(size.X / 2f, (0f - size.Y) / 2f) + pos;
		position = Vector2.Zero;
		this.rotation = 0.0;
		Update(Vector2.Zero, rotation);
	}

	public void Update(Vector2 pos, double rotation)
	{
		position = pos;
		this.rotation = rotation;
		Matrix matrix = Matrix.CreateRotationZ((float)rotation) * Matrix.CreateTranslation(new Vector3(position, 0f));
		Vector2 vector = Vector2.Transform(P0, matrix);
		Vector2 vector2 = Vector2.Transform(P1, matrix);
		Vector2 vector3 = Vector2.Transform(P2, matrix);
		Vector2 vector4 = Vector2.Transform(P3, matrix);
		list.Clear();
		list.Add(new RoundLine(vector, vector2));
		list.Add(new RoundLine(vector2, vector3));
		list.Add(new RoundLine(vector3, vector4));
		list.Add(new RoundLine(vector4, vector));
	}
}
