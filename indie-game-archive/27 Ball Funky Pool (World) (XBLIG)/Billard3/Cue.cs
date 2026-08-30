using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class Cue
{
	public static Obj obj;

	public static Obj objNoAlpha;

	private static Matrix transform;

	private static float inclinaisonDeg = -19f;

	private static float height = 4.5f;

	private static float distance = 12.5f;

	private static float scale = 1f;

	private static Vector3 offset;

	private static Vector3 pos;

	public static Matrix Transform => transform;

	public static void LoadContent(ContentManager Content)
	{
		if (CameraBillard.BoxShot)
		{
			inclinaisonDeg = -5f;
			height = 4f;
			distance = 37.5f;
			scale = 3f;
		}
		Model model = Content.Load<Model>("Models/cue");
		obj = new Obj(Obj.IDenum.Cue, model);
		obj.SpecularColor.Add(Color.White);
		obj.SpecularPower = 100;
		objNoAlpha = new Obj((Obj.IDenum)(-1), model);
		objNoAlpha.Alpha = 1f;
		obj.Alpha = 0f;
		objNoAlpha.SpecularPower = obj.SpecularPower;
	}

	public static void Update_Aiming()
	{
		offset = Vector3.Zero;
		pos = Statics.balls[0].Pos.Value;
		transform = ComputeTransform(pos, Aiming.AngleRad, offset);
	}

	public static Matrix ComputeTransform(Vector3 wballPos, double aimAngle, Vector3 shootOffset)
	{
		Vector3 position = wballPos - Aiming.AimVectorStatic(aimAngle) * distance + Vector3.UnitY * height;
		float radians = 0f - (float)aimAngle;
		float radians2 = MathHelper.ToRadians(inclinaisonDeg);
		return Matrix.CreateScale(scale) * Matrix.CreateRotationZ(radians2) * Matrix.CreateRotationY(radians) * Matrix.CreateTranslation(position) * Matrix.CreateTranslation(shootOffset);
	}

	public static void Update(GameTime gameTime)
	{
		switch (GameState.Current)
		{
		case GameState.Type.AIMING:
		case GameState.Type.CHOOSING_POWER:
			if (obj.Alpha != 1f)
			{
				obj.Alpha = Utils.incrementRatio(obj.Alpha, 45);
			}
			return;
		}
		if (GameState.IsTransitioningTo(GameState.Type.WATCHING_MOVE, out var ratioTrans))
		{
			float num = ((ratioTrans < 0.9f) ? Utils.PowerCurveInverse(MathHelper.Lerp(0f, 1f, ratioTrans / 0.9f), 1) : MathHelper.Lerp(1f, -0.3f, (ratioTrans - 0.9f) / 0.100000024f));
			offset = Aiming.AimVector * -1f * MathHelper.Lerp(1f, 5f, (GameModeRules.CurrentPlayer == GameModeRules.IndexCPU) ? Bot.PowerRatio : ChoosePower.Ratio) * num;
		}
		else if (obj.Alpha != 0f)
		{
			obj.Alpha = Utils.decrementRatio(obj.Alpha, 45);
			offset += Aiming.AimVector * -1f * 0.15f;
		}
		transform = ComputeTransform(pos, Aiming.AngleRad, offset);
	}
}
