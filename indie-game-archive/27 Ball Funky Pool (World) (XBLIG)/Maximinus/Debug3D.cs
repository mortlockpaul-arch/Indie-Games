using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class Debug3D
{
	public class LineList
	{
		private VertexPositionNormalColor[] vertices;

		private BasicEffect effect;

		public LineList(VertexPositionNormalColor[] vertices, Color col)
		{
			this.vertices = vertices;
			effect = Drawing3D_V2.NewEffectNoLighting;
			effect.DiffuseColor = col.ToVector3();
		}

		public void Draw(Matrix world)
		{
			effect.World = world;
			effect.View = MaximinusGame.Instance.Camera.View;
			effect.Projection = MaximinusGame.Instance.Camera.Proj;
			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				MaximinusGame.Draw2D.Device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, vertices.Length / 2);
			}
		}
	}

	public class DebugObject : ObjDrawUpdate
	{
		public Matrix Orientation;

		private Matrix transform;

		protected Matrix transformPre;

		private Model model;

		private Vector3 pos;

		private Vector3 scale;

		private bool dirty;

		public Color color;

		public Vector3 Pos
		{
			get
			{
				return pos;
			}
			set
			{
				pos = value;
				dirty = true;
				Update(MaximinusGame.gameTime);
			}
		}

		protected Vector3 Scale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
				dirty = true;
				Update(MaximinusGame.gameTime);
			}
		}

		public DebugObject(bool useAutoDraw, Model model, Vector3 pos, Vector3 scale, Color color)
			: base(useAutoDraw, useAutoDraw: true)
		{
			if (model == null)
			{
				throw new Exception("model not initialized");
			}
			this.model = model;
			this.pos = pos;
			this.scale = scale;
			this.color = color;
			transformPre = Matrix.Identity;
			Orientation = Matrix.Identity;
			dirty = true;
			Update(MaximinusGame.gameTime);
		}

		public override void Update(GameTime gameTime)
		{
			if (dirty)
			{
				transform = transformPre * Matrix.CreateScale(scale) * Orientation * Matrix.CreateTranslation(pos);
				dirty = false;
			}
		}

		public override void Draw(GameTime gameTime)
		{
			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (BasicEffect effect in mesh.Effects)
				{
					Drawing3D_V2.ApplyEffect(effect, mesh.ParentBone.Transform * transform, useDefaultLighting: true);
					if (color != Color.White)
					{
						effect.AmbientLightColor = color.ToVector3();
						effect.DirectionalLight0.DiffuseColor = effect.AmbientLightColor;
						effect.DirectionalLight1.DiffuseColor = effect.AmbientLightColor;
						effect.DirectionalLight2.DiffuseColor = effect.AmbientLightColor;
					}
				}
				mesh.Draw();
			}
		}
	}

	public class StaticPointShow : DebugObject
	{
		private static Model model;

		public static void LoadContent()
		{
			model = MaximinusGame.ContentManager.Load<Model>("Models/PointShow");
		}

		public StaticPointShow(Vector3 pos)
			: this(pos, 1f, Color.White)
		{
		}

		public StaticPointShow(Vector3 pos, Color color)
			: this(pos, 1f, color)
		{
		}

		public StaticPointShow(Vector3 pos, float scale)
			: this(pos, scale, Color.White)
		{
		}

		public StaticPointShow(Vector3 pos, float scale, Color color)
			: base(useAutoDraw: true, model, pos, new Vector3(scale, scale, scale), color)
		{
		}
	}

	public class Orientation
	{
		private Arrow[] arrows;

		private Vector3 scale;

		public Orientation(bool useAutoDraw, Vector3 scale)
		{
			this.scale = scale;
			arrows = new Arrow[3];
			arrows[0] = new Arrow(useAutoDraw);
			arrows[0].color = Color.Red;
			arrows[1] = new Arrow(useAutoDraw);
			arrows[1].color = Color.Blue;
			arrows[2] = new Arrow(useAutoDraw);
			arrows[2].color = Color.Yellow;
		}

		public void UpdateValue(Vector3 p, Matrix o)
		{
			Arrow[] array = arrows;
			foreach (Arrow arrow in array)
			{
				arrow.Orientation = o;
			}
			arrows[0].UpdateValue(p, p + Matrix.Identity.Forward * scale.X);
			arrows[1].UpdateValue(p, p + Matrix.Identity.Right * scale.Z);
			arrows[2].UpdateValue(p, p + Matrix.Identity.Up * scale.Y);
		}
	}

	public class Cross
	{
		private static Model model;

		private Arrow[] arrows;

		private Vector3[] offsetStart;

		private Vector3[] offsetEnd;

		public Matrix Orientation
		{
			set
			{
				Arrow[] array = arrows;
				foreach (Arrow arrow in array)
				{
					arrow.Orientation = value;
				}
			}
		}

		public Color color
		{
			set
			{
				Arrow[] array = arrows;
				foreach (Arrow arrow in array)
				{
					arrow.color = value;
				}
			}
		}

		public Vector3 Pos
		{
			set
			{
				for (int i = 0; i < arrows.Length; i++)
				{
					arrows[i].UpdateValue(value + offsetStart[i], value + offsetEnd[i]);
				}
			}
		}

		public static void LoadContent()
		{
			model = MaximinusGame.ContentManager.Load<Model>("Models/Arrow");
		}

		public Cross(Vector3 scale)
			: this(useAutoDraw: true, scale)
		{
		}

		public Cross(bool useAutoDraw, Vector3 scale)
		{
			arrows = new Arrow[6];
			offsetStart = new Vector3[6];
			offsetEnd = new Vector3[6];
			for (int i = 0; i < arrows.Length; i++)
			{
				arrows[i] = new Arrow(useAutoDraw);
			}
			ref Vector3 reference = ref offsetStart[0];
			reference = Vector3.UnitX * 0.05f * scale.X;
			ref Vector3 reference2 = ref offsetEnd[0];
			reference2 = Vector3.UnitX * 1f * scale.X;
			ref Vector3 reference3 = ref offsetStart[1];
			reference3 = Vector3.UnitX * -0.05f * scale.X;
			ref Vector3 reference4 = ref offsetEnd[1];
			reference4 = Vector3.UnitX * -1f * scale.X;
			ref Vector3 reference5 = ref offsetStart[2];
			reference5 = Vector3.UnitY * 0.05f * scale.Y;
			ref Vector3 reference6 = ref offsetEnd[2];
			reference6 = Vector3.UnitY * 1f * scale.Y;
			ref Vector3 reference7 = ref offsetStart[3];
			reference7 = Vector3.UnitY * -0.05f * scale.Y;
			ref Vector3 reference8 = ref offsetEnd[3];
			reference8 = Vector3.UnitY * -1f * scale.Y;
			ref Vector3 reference9 = ref offsetStart[4];
			reference9 = Vector3.UnitZ * 0.05f * scale.Z;
			ref Vector3 reference10 = ref offsetEnd[4];
			reference10 = Vector3.UnitZ * 1f * scale.Z;
			ref Vector3 reference11 = ref offsetStart[5];
			reference11 = Vector3.UnitZ * -0.05f * scale.Z;
			ref Vector3 reference12 = ref offsetEnd[5];
			reference12 = Vector3.UnitZ * -1f * scale.Z;
			Pos = Vector3.Zero;
		}

		public void Draw(GameTime gameTime)
		{
			for (int i = 0; i < arrows.Length; i++)
			{
				arrows[i].Draw(gameTime);
			}
		}
	}

	public class Arrow : DebugObject
	{
		private static Model model;

		public static void LoadContent()
		{
			model = MaximinusGame.ContentManager.Load<Model>("Models/Arrow");
		}

		public Arrow()
			: this(useAutoDraw: true)
		{
		}

		public Arrow(bool useAutoDraw)
			: this(useAutoDraw, Color.White)
		{
		}

		public Arrow(bool useAutoDraw, Color color)
			: base(useAutoDraw, model, Vector3.Zero, new Vector3(1f, 1f, 1f), color)
		{
		}

		public void UpdateValue(Vector3 startPos, Vector3 endPos)
		{
			Vector3 vector = endPos - startPos;
			float num = vector.Length();
			transformPre = Matrix.CreateScale(num, 1f, 1f);
			Vector2 vector2 = new Vector2(vector.X, vector.Z);
			if (vector2.Length() == 0f)
			{
				transformPre *= Matrix.CreateRotationZ((float)Math.PI / 2f * (float)Math.Sign(vector.Y));
			}
			else
			{
				vector2.Normalize();
				Vector2 vector3 = MyMath.Vector2Orthogonal(vector2);
				transformPre *= Matrix.CreateRotationY((float)MyMath.AngleRadFromVectorNorm(vector2) * -1f) * Matrix.CreateFromAxisAngle(new Vector3(vector3.X, 0f, vector3.Y), (float)Math.Asin(vector.Y / num));
			}
			base.Pos = startPos;
		}
	}
}
