using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaLibrary.Graphics;

public class ModelData : IDisposable
{
	private bool disposed;

	protected Model model;

	public Vector3 Position;

	protected Vector3 rotate;

	protected Vector3 yawPitchRoll;

	protected float scale;

	protected FillMode fillMode;

	protected CullMode cullMode;

	protected Texture2D texture;

	private Matrix[] boneTransforms;

	public bool IsDisposed => disposed;

	public Model Model => model;

	public Vector3 Rotate
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return rotate;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			rotate = value;
		}
	}

	public Vector3 YawPitchRoll
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return yawPitchRoll;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			yawPitchRoll = value;
		}
	}

	public float Yaw
	{
		get
		{
			return yawPitchRoll.X;
		}
		set
		{
			yawPitchRoll.X = value;
		}
	}

	public float Pitch
	{
		get
		{
			return yawPitchRoll.Y;
		}
		set
		{
			yawPitchRoll.Y = value;
		}
	}

	public float Roll
	{
		get
		{
			return yawPitchRoll.Z;
		}
		set
		{
			yawPitchRoll.Z = value;
		}
	}

	public Matrix[] BoneTransforms => boneTransforms;

	public Texture2D Texture
	{
		get
		{
			return texture;
		}
		set
		{
			texture = value;
		}
	}

	public float Scale
	{
		get
		{
			return scale;
		}
		set
		{
			scale = value;
		}
	}

	public FillMode FillMode
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return fillMode;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			fillMode = value;
		}
	}

	public CullMode CullMode
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return cullMode;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			cullMode = value;
		}
	}

	public ModelData(Model model)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Position = Vector3.Zero;
		rotate = Vector3.Zero;
		yawPitchRoll = Vector3.Zero;
		scale = 1f;
		fillMode = (FillMode)3;
		cullMode = (CullMode)1;
		base._002Ector();
		SetModelData(model);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	private void Dispose(bool disposing)
	{
		if (!disposed)
		{
		}
		disposed = true;
	}

	public unsafe virtual void Draw(Matrix view, Matrix projection, Matrix world)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = Model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						Effect current2 = ((Enumerator)(ref enumerator2)).Current;
						BasicEffect val = (BasicEffect)(object)((current2 is BasicEffect) ? current2 : null);
						if (val != null)
						{
							val.View = view;
							val.Projection = projection;
							val.World = GetWorldMatrix(current, world);
							val.EnableDefaultLighting();
							val.PreferPerPixelLighting = true;
						}
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public virtual void SetModelData(Model model)
	{
		this.model = model;
		boneTransforms = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)model.Bones).Count];
		model.CopyAbsoluteBoneTransformsTo(boneTransforms);
	}

	public Matrix GetWorldMatrix(ModelMesh mesh, Matrix world)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (model == null || mesh == null)
		{
			return Matrix.Identity;
		}
		return BoneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(Scale) * Matrix.CreateRotationX(Rotate.X) * Matrix.CreateRotationY(Rotate.Y) * Matrix.CreateRotationZ(Rotate.Z) * Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll) * Matrix.CreateTranslation(Position) * world;
	}

	public override string ToString()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		text += $"Position : {Position}\n";
		text += $"Rotate : {Rotate}\n";
		text += $"Scale : {Scale}\n";
		text += $"FillMode : {FillMode}\n";
		return text + $"CullMode : {CullMode}\n";
	}
}
