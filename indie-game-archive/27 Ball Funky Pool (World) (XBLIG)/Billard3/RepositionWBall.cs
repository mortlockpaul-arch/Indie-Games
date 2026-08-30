using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class RepositionWBall
{
	public static Obj obj;

	public static Vector2 DefaultPos;

	public static Vector3 DefaultPosV3;

	public static readonly Vector3 LookAtDirection = Vector3.UnitX * -1f;

	public static void LoadContent(ContentManager Content)
	{
		obj = new Obj(Obj.IDenum.RepositionWBall, Content.Load<Model>("Models/reposition-wball-arrows"));
	}
}
