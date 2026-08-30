using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class Drawing3D
{
	public class Lighting
	{
		public Vector3 Direction = Vector3.Zero;

		public static Vector3 DirectionFinalGlobeClicker = Vector3.Normalize(new Vector3(-2f, -1f, -1f));

		public Vector3 DiffuseColor = Vector3.One * 1f;

		public Vector3 SpecularColor = Vector3.One * 0.25f;

		public void Update(Vector3 newDir)
		{
			Direction = newDir;
		}
	}

	public class ModelAlpha
	{
		public float Alpha;

		public Model model;

		public List<Color> SpecularColor;

		public int SpecularPower;

		public ModelAlpha()
		{
			Alpha = 1f;
			SpecularPower = -1;
			SpecularColor = new List<Color>();
			model = null;
		}

		public ModelAlpha(Model model)
			: this()
		{
			this.model = model;
		}
	}

	public class ModelAlphaBones : ModelAlpha
	{
		private Matrix finalTransform;

		private Matrix[] transforms;

		public Matrix CustomTransform
		{
			set
			{
				finalTransform = transforms[0] * value;
			}
		}

		public Matrix TransformsZero => finalTransform;

		public ModelAlphaBones(Model model)
			: base(model)
		{
			transforms = new Matrix[model.Bones.Count];
			model.CopyAbsoluteBoneTransformsTo(transforms);
			finalTransform = transforms[0];
		}
	}

	public class DrawParams
	{
		public bool skipStandardDraw;

		public Vector3 lightingDir;

		public bool hasLighting;

		public bool isCustomColor;

		public Color customColor;

		public bool hasCustomAmbientColor;

		public Color customAmbientColor;

		public Matrix transforms;

		public static readonly Vector3 LightDirNetPlank = new Vector3(-5f, -1f, 0f);

		public static readonly Vector3 LightDirNetPillar = Vector3.Normalize(new Vector3(-2f, -3f, -2f));

		public static readonly Vector3 LightDirFilet = new Vector3(-1f, 0f, -1f);

		public static readonly Vector3 LightDirScoreBoard = Vector3.Normalize(new Vector3(-0f, -0.33f, -1f));

		public static readonly Vector3 LightDirNeonSucces = Vector3.Normalize(new Vector3(-1f, 0f, -0.5f));

		public static readonly Vector3 LightDirBallChooseAvatar = new Vector3(1f, -1f, 0f);

		public static readonly Vector3 LightDirBall = Vector3.Normalize(new Vector3(0f, -1f, -1.5f));

		public static readonly Color ColorScoreBoard = new Color(Color.White.ToVector3() * 0.5f);

		public void Reset(Matrix transformsZero)
		{
			skipStandardDraw = false;
			lightingDir = Vector3.UnitY * -1f;
			hasLighting = true;
			isCustomColor = false;
			customColor = Color.White;
			hasCustomAmbientColor = false;
			customAmbientColor = Color.White;
			transforms = transformsZero;
		}
	}

	public static Lighting LightingDefault = new Lighting();

	public static void DrawModel(ModelAlpha model, Matrix transform, bool hasCustomLighting, bool isColorCustom, Color color, Matrix cameraViewMatrix, Matrix cameraProjectionMatrix)
	{
		DrawModel(model, transform, hasCustomLighting, isColorCustom, color, cameraViewMatrix, cameraProjectionMatrix, LightingDefault.Direction, LightingDefault, hasCustomAmbientColor: false, Color.White);
	}

	public static void DrawModel(ModelAlpha model, Matrix transform, bool hasCustomLighting, bool isColorCustom, Color color, Matrix cameraViewMatrix, Matrix cameraProjectionMatrix, Vector3 lightingDirection)
	{
		DrawModel(model, transform, hasCustomLighting, isColorCustom, color, cameraViewMatrix, cameraProjectionMatrix, lightingDirection, LightingDefault, hasCustomAmbientColor: false, Color.White);
	}

	public static void DrawModel(ModelAlpha model, Matrix transform, bool hasCustomLighting, bool isColorCustom, Color color, Matrix cameraViewMatrix, Matrix cameraProjectionMatrix, Lighting lighting)
	{
		DrawModel(model, transform, hasCustomLighting, isColorCustom, color, cameraViewMatrix, cameraProjectionMatrix, lighting.Direction, lighting, hasCustomAmbientColor: false, Color.White);
	}

	public static void DrawModel(ModelAlpha model, Matrix transform, bool hasCustomLighting, bool isColorCustom, Color color, Matrix cameraViewMatrix, Matrix cameraProjectionMatrix, Vector3 lightingDirection, bool hasCustomAmbientColor, Color customAmbientColor)
	{
		DrawModel(model, transform, hasCustomLighting, isColorCustom, color, cameraViewMatrix, cameraProjectionMatrix, lightingDirection, LightingDefault, hasCustomAmbientColor, customAmbientColor);
	}

	public static void DrawModel(ModelAlpha model, Matrix transform, bool hasCustomLighting, bool isColorCustom, Color color, Matrix cameraViewMatrix, Matrix cameraProjectionMatrix, Vector3 lightingDirection, Lighting lighting, bool hasCustomAmbientColor, Color customAmbientColor)
	{
		foreach (ModelMesh mesh in model.model.Meshes)
		{
			foreach (BasicEffect effect in mesh.Effects)
			{
				if (hasCustomAmbientColor)
				{
					effect.AmbientLightColor = customAmbientColor.ToVector3();
				}
				if (hasCustomLighting)
				{
					if (!isColorCustom)
					{
						effect.DirectionalLight1.Enabled = true;
						effect.DirectionalLight1.DiffuseColor = lighting.DiffuseColor;
						effect.DirectionalLight1.SpecularColor = lighting.SpecularColor;
						effect.DirectionalLight1.Direction = lightingDirection;
						effect.DirectionalLight0.Enabled = false;
						effect.DirectionalLight2.Enabled = false;
						effect.LightingEnabled = true;
					}
					else
					{
						effect.DirectionalLight0.Enabled = false;
						effect.DirectionalLight2.Enabled = false;
						effect.DirectionalLight1.Enabled = true;
						effect.DirectionalLight1.DiffuseColor = color.ToVector3();
						effect.DirectionalLight1.Direction = lightingDirection;
						effect.PreferPerPixelLighting = true;
						effect.LightingEnabled = true;
					}
				}
				else
				{
					effect.EnableDefaultLighting();
				}
				effect.World = transform;
				effect.Projection = cameraProjectionMatrix;
				effect.View = cameraViewMatrix;
				effect.Alpha = model.Alpha;
				_ = effect.Alpha;
				_ = 1f;
				if (hasCustomLighting && isColorCustom && color.A != byte.MaxValue)
				{
					effect.Alpha *= (float)(int)color.A / 255f;
				}
				if (MaximinusGame.Id == MaximinusGame.ID.Billard || MaximinusGame.Id == MaximinusGame.ID.Billard9Ball || MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
				{
					if (model.model.Bones[mesh.ParentBone.Index - 1].Name == "TABLE-SPECULAR")
					{
						effect.DirectionalLight1.SpecularColor = Color.White.ToVector3();
						effect.SpecularPower = 100f;
					}
					else if (model.model.Bones[mesh.ParentBone.Index].Name == "FunkyBande")
					{
						effect.DirectionalLight1.Direction = Vector3.UnitZ * 1f * effect.DirectionalLight1.Direction.Length();
						effect.AmbientLightColor = Vector3.Zero;
						effect.DiffuseColor = Vector3.One;
					}
				}
				if (model.SpecularColor.Count > 0)
				{
					effect.DirectionalLight1.SpecularColor = model.SpecularColor[0].ToVector3();
				}
				if (model.SpecularPower != -1)
				{
					effect.SpecularPower = model.SpecularPower;
				}
			}
			mesh.Draw();
		}
	}
}
