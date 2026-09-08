using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Eyehook.Framework;

public class Shadow
{
	private static Game Game;

	private static BasicEffect basicEffect;

	private static VertexPositionColor[] shadow;

	private static RasterizerState cullNone;

	private static Vector3[] transform = new Vector3[4];

	public static void Initialize(Game game)
	{
		Game = game;
		basicEffect = new BasicEffect(game.GraphicsDevice);
		basicEffect.VertexColorEnabled = true;
		basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0f, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
		cullNone = new RasterizerState();
		cullNone.CullMode = CullMode.None;
		shadow = new VertexPositionColor[4];
		for (int i = 0; i < 4; i++)
		{
			shadow[i] = new VertexPositionColor(Vector3.Zero, Color.Black);
		}
	}

	private static float angle(Vector3 v1)
	{
		return (float)Math.Atan2(v1.Y, v1.X);
	}

	public static void DrawShadow(Vector3 lightPos, VertexPositionColor[] square, Vector3 squareCenter)
	{
		Matrix matrix = Matrix.CreateRotationZ(0f - angle(squareCenter - lightPos));
		float num = Vector3.Distance(lightPos, squareCenter);
		float num2 = float.MaxValue;
		float num3 = float.MinValue;
		int num4 = 0;
		int num5 = 0;
		for (int i = 0; i < 4; i++)
		{
			transform[i] = Vector3.Transform(square[i].Position - squareCenter, matrix);
			transform[i].X += num;
			float num6 = angle(transform[i]);
			if ((double)num6 < (double)num2)
			{
				num4 = i;
				num2 = num6;
			}
			if ((double)num6 > (double)num3)
			{
				num5 = i;
				num3 = num6;
			}
		}
		shadow[0].Position = square[num4].Position;
		shadow[1].Position = square[num5].Position;
		shadow[2].Position = (square[num4].Position - lightPos) * 1000f;
		shadow[3].Position = (square[num5].Position - lightPos) * 1000f;
		Game.GraphicsDevice.RasterizerState = cullNone;
		basicEffect.CurrentTechnique.Passes[0].Apply();
		Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, shadow, 0, 2);
		Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, square, 0, 2);
	}
}
