using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenegadeEngine.Graphics;

public abstract class PrimitiveShape : IDisposable
{
	public VertexBuffer vertexBuffer;

	public IndexBuffer meshIndexBuffer;

	protected IndexBuffer wireIndexBuffer;

	public BasicEffect effect;

	public Matrix world = Matrix.Identity;

	protected Vector3 position = Vector3.Zero;

	protected Vector3 rotation = Vector3.Zero;

	protected Vector3 scale = Vector3.One;

	protected Vector3 wireColor = Color.Red.ToVector3();

	protected Vector3 meshColor = Color.Green.ToVector3();

	protected EffectParameterCollection parameters;

	public int vertexCount;

	public int triangleCount;

	public int lineCount;

	public bool Wireframe;

	public bool Shaded = true;

	public ShapeType ShapeType { get; set; }

	public Vector3 MeshColor
	{
		get
		{
			return meshColor;
		}
		set
		{
			meshColor = value;
		}
	}

	public Vector3 DiffuseColor
	{
		get
		{
			return effect.DiffuseColor;
		}
		set
		{
			effect.DiffuseColor = value;
		}
	}

	public Vector3 EmissiveColor
	{
		get
		{
			return parameters["EmissiveColor"].GetValueVector3();
		}
		set
		{
			parameters["EmissiveColor"].SetValue(value);
		}
	}

	public float Alpha
	{
		get
		{
			return effect.Alpha;
		}
		set
		{
			effect.Alpha = value;
		}
	}

	public virtual Vector3 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public virtual Vector3 Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation.X = MathHelper.WrapAngle(value.X);
			rotation.Y = MathHelper.WrapAngle(value.Y);
			rotation.Z = MathHelper.WrapAngle(value.Z);
		}
	}

	public virtual Vector3 Scale
	{
		get
		{
			return scale;
		}
		set
		{
			float num = float.Epsilon;
			if (value.X > num)
			{
				scale.X = value.X;
			}
			if (value.Y > num)
			{
				scale.Y = value.Y;
			}
			if (value.Z > num)
			{
				scale.Z = value.Z;
			}
		}
	}

	public void SetColors(Color shading, Color wireColor)
	{
		meshColor = shading.ToVector3();
		this.wireColor = wireColor.ToVector3();
	}

	protected virtual void InitializeEffect(GraphicsDevice device)
	{
		effect = new BasicEffect(device);
		parameters = effect.Parameters;
		effect.EnableDefaultLighting();
		effect.LightingEnabled = true;
		effect.PreferPerPixelLighting = true;
	}

	~PrimitiveShape()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (vertexBuffer != null)
		{
			vertexBuffer.Dispose();
		}
		if (meshIndexBuffer != null)
		{
			meshIndexBuffer.Dispose();
		}
		if (wireIndexBuffer != null)
		{
			wireIndexBuffer.Dispose();
		}
		if (effect != null)
		{
			effect.Dispose();
		}
		GC.SuppressFinalize(this);
	}

	public virtual void Update()
	{
		world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.CreateTranslation(position);
	}

	public virtual void UpdateWorld(Vector3 position, Vector3 rotation, Vector3 scale)
	{
		world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.CreateTranslation(position);
	}

	public virtual void Draw(ref Matrix view, ref Matrix projection)
	{
		effect.World = world;
		effect.View = view;
		effect.Projection = projection;
		GraphicsDevice graphicsDevice = effect.GraphicsDevice;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.SetVertexBuffer(vertexBuffer);
		if (Shaded && triangleCount > 0)
		{
			graphicsDevice.BlendState = BlendState.AlphaBlend;
			graphicsDevice.Indices = meshIndexBuffer;
			foreach (EffectTechnique technique in effect.Techniques)
			{
				foreach (EffectPass pass in technique.Passes)
				{
					pass.Apply();
					graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, triangleCount);
				}
			}
		}
		if (!Wireframe || lineCount <= 0)
		{
			return;
		}
		effect.DiffuseColor = wireColor;
		graphicsDevice.Indices = wireIndexBuffer;
		foreach (EffectTechnique technique2 in effect.Techniques)
		{
			foreach (EffectPass pass2 in technique2.Passes)
			{
				pass2.Apply();
				graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, vertexCount, 0, lineCount);
			}
		}
	}

	public virtual void Draw(ref Matrix world, ref Matrix view, ref Matrix projection)
	{
		effect.World = world;
		effect.View = view;
		effect.Projection = projection;
		GraphicsDevice graphicsDevice = effect.GraphicsDevice;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.SetVertexBuffer(vertexBuffer);
		if (triangleCount > 0)
		{
			graphicsDevice.BlendState = BlendState.AlphaBlend;
			graphicsDevice.Indices = meshIndexBuffer;
			foreach (EffectTechnique technique in effect.Techniques)
			{
				foreach (EffectPass pass in technique.Passes)
				{
					pass.Apply();
					graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, triangleCount);
				}
			}
		}
		if (!Wireframe || lineCount <= 0)
		{
			return;
		}
		effect.DiffuseColor = wireColor;
		graphicsDevice.Indices = wireIndexBuffer;
		foreach (EffectTechnique technique2 in effect.Techniques)
		{
			foreach (EffectPass pass2 in technique2.Passes)
			{
				pass2.Apply();
				graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, vertexCount, 0, lineCount);
			}
		}
	}
}
