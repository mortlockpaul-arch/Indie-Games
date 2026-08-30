using System;
using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class Draws
{
	private const float floorScale = 20f;

	private static Drawing3D.DrawParams drawParams = new Drawing3D.DrawParams();

	public static readonly Matrix defaultMat = Matrix.CreateRotationX((float)Math.PI / 2f);

	private static FrameRate frameRate;

	private static Drawing2D draw2D;

	private static readonly Matrix FloorTransform = Matrix.CreateScale(20f, 1f, 20f) * Matrix.CreateTranslation(Vector3.Down * 16f);

	public static Effect FloorEffect;

	private static VertexPositionNormalTexture[] floorVertices;

	private static Drawing3D.ModelAlpha Highlighter = null;

	private static Drawing3D.ModelAlpha Highlighter2 = null;

	private static Drawing3D.ModelAlpha FunkyBande;

	public static void Initialize(ContentManager Content)
	{
		if (MaximinusGame.Id == MaximinusGame.ID.Billard9Ball)
		{
			Highlighter = new Drawing3D.ModelAlpha(Content.Load<Model>("Models/highlight-ball"));
			Highlighter2 = new Drawing3D.ModelAlpha(Content.Load<Model>("Models/highlight2-ball"));
		}
		else if (MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
		{
			FunkyBande = new Drawing3D.ModelAlpha(Content.Load<Model>("Models/funkyBande"));
		}
		if (BillardGame.FPS)
		{
			frameRate = new FrameRate(Statics.draw2D, extraInfo: true);
		}
		draw2D = Statics.draw2D;
		floorVertices = new VertexPositionNormalTexture[8];
		ref VertexPositionNormalTexture reference = ref floorVertices[0];
		reference = new VertexPositionNormalTexture(new Vector3(-3f, 0f, 3f), new Vector3(0f, 1f, 0f), new Vector2(0f, 1f));
		ref VertexPositionNormalTexture reference2 = ref floorVertices[1];
		reference2 = new VertexPositionNormalTexture(new Vector3(-3f, 0f, -3f), new Vector3(0f, 1f, 0f), new Vector2(0f, 0f));
		ref VertexPositionNormalTexture reference3 = ref floorVertices[2];
		reference3 = new VertexPositionNormalTexture(new Vector3(3f, 0f, 3f), new Vector3(0f, 1f, 0f), new Vector2(1f, 1f));
		ref VertexPositionNormalTexture reference4 = ref floorVertices[3];
		reference4 = new VertexPositionNormalTexture(new Vector3(3f, 0f, -3f), new Vector3(0f, 1f, 0f), new Vector2(1f, 0f));
	}

	public static void Draw(GameTime gameTime)
	{
		if (Statics.ContentLoadedTime == -2.0)
		{
			Statics.draw2D.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
			Statics.draw2D.Device.Clear(Color.Black);
			Statics.draw2D.SpriteBatch.Draw(BillardGame.splash, Statics.draw2D.ScreenSize / 2f - new Vector2(BillardGame.splash.Width, BillardGame.splash.Height) / 2f, Color.White);
			Statics.draw2D.SpriteBatch.End();
			return;
		}
		InfoDisplay.DrawToTexture();
		Statics.cheatPrompt.DrawToTexture(gameTime);
		Statics.draw2D.Device.Clear(Color.Black);
		draw2D.PrepareFor3D();
		if (MaximinusGame.Id == MaximinusGame.ID.Billard9Ball && (GameState.Current == GameState.Type.AIMING || GameState.Current == GameState.Type.REPOSITION_WBALL || GameState.IsTransitioningTo(GameState.Type.AIMING)) && GameModeRules.LowestNumericalObjectBall != -1)
		{
			Matrix matrix = Matrix.CreateScale(0.9166663f) * Matrix.CreateTranslation(Statics.balls[GameModeRules.LowestNumericalObjectBall].Pos.Value);
			float num = (float)gameTime.TotalGameTime.TotalSeconds * 1f;
			Color customAmbientColor = ((GameModeRules.LowestNumericalObjectBall == 5) ? Color.Orange : LigneVisee.ColorOfBall(GameModeRules.LowestNumericalObjectBall));
			Drawing3D.DrawModel(Highlighter, Matrix.CreateScale(0.8f, 1.15f, 0.8f) * Matrix.CreateRotationY(num) * matrix, hasCustomLighting: true, isColorCustom: false, Color.White, Statics.cam.ViewMatrix, Statics.cam.ProjMatrix, Vector3.Up, hasCustomAmbientColor: true, customAmbientColor);
			Drawing3D.DrawModel(Highlighter2, Matrix.CreateRotationY(0f - num) * matrix, hasCustomLighting: true, isColorCustom: false, Color.White, Statics.cam.ViewMatrix, Statics.cam.ProjMatrix, Vector3.Up, hasCustomAmbientColor: true, customAmbientColor);
		}
		foreach (Obj @object in Statics.objects)
		{
			drawParams.Reset(Matrix.Identity);
			drawParams.transforms = defaultMat;
			if (Obj.IsBall(@object.id))
			{
				drawParams.skipStandardDraw = true;
				if (GameState.Current == GameState.Type.REPOSITION_WBALL && @object.id == Obj.IDenum.Ball0 && !GameState.RepoWballAvailable)
				{
					@object.Alpha = 0.5f;
				}
				else
				{
					@object.Alpha = 1f;
				}
				Ball ball = Statics.GetBall(@object.id);
				if (ball.state != Ball.State.DEAD)
				{
					ball.Draw(@object.Alpha, -1);
				}
			}
			else
			{
				switch (@object.id)
				{
				case Obj.IDenum.TablePlan:
					SetDrawParamsTable(drawParams);
					break;
				case Obj.IDenum.RepositionWBall:
					drawParams.skipStandardDraw = GameState.Current != GameState.Type.REPOSITION_WBALL;
					if (!drawParams.skipStandardDraw)
					{
						drawParams.transforms *= Matrix.CreateRotationX(-(float)Math.PI / 2f) * Matrix.CreateTranslation(Statics.balls[0].Pos.Value);
						drawParams.hasCustomAmbientColor = true;
						drawParams.customAmbientColor = Color.White;
						drawParams.hasLighting = true;
					}
					break;
				case Obj.IDenum.Cue:
					drawParams.transforms *= Cue.Transform;
					drawParams.hasLighting = true;
					drawParams.lightingDir = Vector3.Down;
					drawParams.hasCustomAmbientColor = false;
					drawParams.isCustomColor = true;
					drawParams.customColor = Color.White;
					if (Cue.obj.Alpha == 0f)
					{
						drawParams.skipStandardDraw = true;
					}
					break;
				}
			}
			if (!drawParams.skipStandardDraw)
			{
				if (MaximinusGame.Id == MaximinusGame.ID.Billard9Ball && @object.id == Obj.IDenum.TablePlan)
				{
					Drawing3D.DrawModel(@object, drawParams.transforms, drawParams.hasLighting, drawParams.isCustomColor, drawParams.customColor, Statics.cam.ViewMatrix, Statics.cam.ProjMatrix, drawParams.lightingDir, drawParams.hasCustomAmbientColor, drawParams.customAmbientColor);
				}
				else
				{
					Drawing3D.DrawModel(@object, drawParams.transforms, drawParams.hasLighting, drawParams.isCustomColor, drawParams.customColor, Statics.cam.ViewMatrix, Statics.cam.ProjMatrix, drawParams.lightingDir, drawParams.hasCustomAmbientColor, drawParams.customAmbientColor);
				}
			}
		}
		foreach (Rectangle listFunkyBande in FunkyBandes.listFunkyBandes)
		{
			Drawing3D.DrawModel(FunkyBande, Matrix.CreateRotationX(-(float)Math.PI / 2f) * Matrix.CreateScale(listFunkyBande.Width, 1.666666f, listFunkyBande.Height) * Matrix.CreateTranslation(new Vector3((float)listFunkyBande.X + (float)listFunkyBande.Width * 0.5f, 0f, (float)listFunkyBande.Y + (float)listFunkyBande.Height * 0.5f)), hasCustomLighting: false, isColorCustom: false, Color.White, Statics.cam.ViewMatrix, Statics.cam.ProjMatrix);
		}
		LigneVisee.Draw();
		Bot.Draw();
		Statics.diamonds.Draw(gameTime);
		DrawFloor();
		draw2D.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
		ChoosePower.Draw(draw2D.SpriteBatch);
		InfoDisplay.Draw(draw2D.SpriteBatch);
		Statics.menus.render(gameTime);
		Statics.cheatPrompt.Draw2D();
		if (BillardGame.FPS)
		{
			frameRate.render(gameTime);
		}
		DebugDraw2D(gameTime);
		float num2 = Timer.Ratio(gameTime, Statics.ContentLoadedTime + 0.5, 2.0);
		if (num2 < 1f)
		{
			Rectangle destinationRectangle = new Rectangle(0, 0, draw2D.ScreenSizePoint.X, draw2D.ScreenSizePoint.Y);
			destinationRectangle.X = (int)((float)destinationRectangle.Width * Utils.PowerCurve(num2, 4f));
			draw2D.SpriteBatch.Draw(GameMenus.Textures.black, destinationRectangle, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
			Rectangle destinationRectangle2 = new Rectangle(destinationRectangle.Center.X - BillardGame.splash.Width / 2, destinationRectangle.Center.Y - BillardGame.splash.Height / 2, BillardGame.splash.Width, BillardGame.splash.Height);
			draw2D.SpriteBatch.Draw(BillardGame.splash, destinationRectangle2, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		}
		draw2D.SpriteBatch.End();
	}

	public static void SetDrawParamsTable(Drawing3D.DrawParams param)
	{
		param.transforms *= Table.CustomTransform;
		param.hasLighting = true;
		param.lightingDir = Vector3.Down * 1.5f;
		param.isCustomColor = true;
		param.customColor = Color.White;
		param.hasCustomAmbientColor = false;
		param.customAmbientColor = new Color(Vector3.One * 1f);
	}

	private static void DebugDraw2D(GameTime gameTime)
	{
		DebugDrawCollision();
	}

	private static void DebugDrawCollision()
	{
	}

	private static void DrawFloor()
	{
		Vector2 vector = Vector2.One;
		if (MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
		{
			vector = new Vector2(1.5f, 3f);
		}
		FloorEffect.CurrentTechnique = FloorEffect.Techniques["SpotLight"];
		FloorEffect.Parameters["xWorld"].SetValue(Matrix.CreateScale(100f) * Matrix.CreateTranslation(Vector3.Up * -0.2f) * FloorTransform);
		FloorEffect.Parameters["xView"].SetValue(Statics.cam.ViewMatrix);
		FloorEffect.Parameters["xProjection"].SetValue(Statics.cam.ProjMatrix);
		FloorEffect.Parameters["xAmbient"].SetValue(0f);
		FloorEffect.Parameters["xLightPosition"].SetValue(Vector3.Up * 100f * vector.X);
		FloorEffect.Parameters["xConeDirection"].SetValue(Vector3.Down);
		FloorEffect.Parameters["xConeAngle"].SetValue(0.5f * vector.X);
		FloorEffect.Parameters["xConeDecay"].SetValue(50f * vector.X);
		FloorEffect.Parameters["xLightStrength"].SetValue(1.125f * vector.X);
		foreach (EffectPass pass in FloorEffect.CurrentTechnique.Passes)
		{
			pass.Apply();
			draw2D.Device.DrawUserPrimitives(PrimitiveType.TriangleStrip, floorVertices, 0, 2);
		}
		foreach (ModelMesh mesh in Statics.floor.model.Meshes)
		{
			foreach (BasicEffect effect in mesh.Effects)
			{
				effect.World = defaultMat * FloorTransform;
				if (MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
				{
					effect.World *= Matrix.CreateScale(vector.X * 0.7f, 1f, vector.Y * 0.65f);
				}
				effect.View = Statics.cam.ViewMatrix;
				effect.Projection = Statics.cam.ProjMatrix;
				effect.LightingEnabled = true;
				effect.DirectionalLight0.Enabled = false;
				effect.AmbientLightColor = Vector3.One * 0.2f;
				foreach (EffectPass pass2 in effect.CurrentTechnique.Passes)
				{
					pass2.Apply();
					mesh.Draw();
				}
			}
		}
	}
}
