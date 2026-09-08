using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;
using Quasar.Shaders;

namespace Quasar.Meshes;

public class Rectangle2DMeshWithTangents : UserVertexMesh<VertexNormalMap>
{
	private bool dirty;

	private Vector2 size;

	private Vector2 offset;

	private float zValue;

	private float rotation;

	private bool flipX;

	private bool flipY;

	private bool useXZCoords;

	private Vector2 uvScale = Vector2.One;

	public Vector2 Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
			dirty = true;
		}
	}

	public Vector2 Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
			dirty = true;
		}
	}

	public Vector2 UVScale
	{
		get
		{
			return uvScale;
		}
		set
		{
			uvScale = value;
			dirty = true;
		}
	}

	public bool UseXZCoords
	{
		get
		{
			return useXZCoords;
		}
		set
		{
			useXZCoords = value;
			dirty = true;
		}
	}

	public float ZValue
	{
		get
		{
			return zValue;
		}
		set
		{
			zValue = value;
			dirty = true;
		}
	}

	public float Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
			dirty = true;
		}
	}

	public bool FlipX
	{
		get
		{
			return flipX;
		}
		set
		{
			flipX = value;
			dirty = true;
		}
	}

	public bool FlipY
	{
		get
		{
			return flipY;
		}
		set
		{
			flipY = value;
			dirty = true;
		}
	}

	private void initializeBuffer()
	{
		initMesh(4);
		base.PrimitiveCount = 2;
		base.PrimitiveType = PrimitiveType.TriangleStrip;
		fillBuffer();
		for (int i = 0; i < 4; i++)
		{
			verticesBuffer[i].Normal = new Vector3(0f, 0f, 1f);
			verticesBuffer[i].Binormal = new Vector3(0f, 1f, 0f);
			verticesBuffer[i].Tangent = new Vector3(1f, 0f, 0f);
		}
	}

	private void fillBuffer()
	{
		generateVertices();
		updateBoundingSphere(4);
		dirty = false;
	}

	private void generateVertices()
	{
		Vector2 vector = size / 2f;
		if (rotation != 0f)
		{
			float num = (float)Math.Sin(rotation);
			float num2 = (float)Math.Cos(rotation);
			verticesBuffer[0].Position = new Vector3((0f - vector.X) * num2 - vector.Y * num + offset.X, vector.Y * num2 - vector.X * num + offset.Y, zValue);
			verticesBuffer[1].Position = new Vector3(vector.X * num2 - vector.Y * num + offset.X, vector.Y * num2 + vector.X * num + offset.Y, zValue);
			verticesBuffer[2].Position = new Vector3((0f - vector.X) * num2 + vector.Y * num + offset.X, (0f - vector.Y) * num2 - vector.X * num + offset.Y, zValue);
			verticesBuffer[3].Position = new Vector3(vector.X * num2 + vector.Y * num + offset.X, (0f - vector.Y) * num2 + vector.X * num + offset.Y, zValue);
		}
		else
		{
			verticesBuffer[0].Position = new Vector3(0f - vector.X + offset.X, vector.Y + offset.Y, zValue);
			verticesBuffer[1].Position = new Vector3(vector.X + offset.X, vector.Y + offset.Y, zValue);
			verticesBuffer[2].Position = new Vector3(0f - vector.X + offset.X, 0f - vector.Y + offset.Y, zValue);
			verticesBuffer[3].Position = new Vector3(vector.X + offset.X, 0f - vector.Y + offset.Y, zValue);
		}
		for (int i = 0; i < 4; i++)
		{
			if (!useXZCoords)
			{
				verticesBuffer[i].Normal = new Vector3(0f, 0f, 1f);
				verticesBuffer[i].Binormal = new Vector3(0f, 1f, 0f);
				verticesBuffer[i].Tangent = new Vector3(1f, 0f, 0f);
			}
			else
			{
				verticesBuffer[i].Normal = new Vector3(0f, 1f, 0f);
				verticesBuffer[i].Binormal = new Vector3(0f, 0f, 1f);
				verticesBuffer[i].Tangent = new Vector3(1f, 0f, 0f);
			}
		}
		verticesBuffer[0].UV = new Vector2(flipX ? uvScale.X : 0f, flipY ? uvScale.Y : 0f);
		verticesBuffer[1].UV = new Vector2(flipX ? 0f : uvScale.X, flipY ? uvScale.Y : 0f);
		verticesBuffer[2].UV = new Vector2(flipX ? uvScale.X : 0f, flipY ? 0f : uvScale.Y);
		verticesBuffer[3].UV = new Vector2(flipX ? 0f : uvScale.X, flipY ? 0f : uvScale.Y);
		if (useXZCoords)
		{
			verticesBuffer[0].Position = new Vector3(verticesBuffer[0].Position.X, verticesBuffer[0].Position.Z, verticesBuffer[0].Position.Y);
			verticesBuffer[1].Position = new Vector3(verticesBuffer[1].Position.X, verticesBuffer[1].Position.Z, verticesBuffer[1].Position.Y);
			verticesBuffer[2].Position = new Vector3(verticesBuffer[2].Position.X, verticesBuffer[2].Position.Z, verticesBuffer[2].Position.Y);
			verticesBuffer[3].Position = new Vector3(verticesBuffer[3].Position.X, verticesBuffer[3].Position.Z, verticesBuffer[3].Position.Y);
			VertexNormalMap vertexNormalMap = verticesBuffer[1];
			ref VertexNormalMap reference = ref verticesBuffer[1];
			reference = verticesBuffer[2];
			verticesBuffer[2] = vertexNormalMap;
		}
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public Rectangle2DMeshWithTangents(Vector2 size, Vector3 backgroundColor)
		: this(size, Vector2.Zero, new Vector4(backgroundColor, 1f))
	{
	}

	public Rectangle2DMeshWithTangents(Layout2D.LayoutData layout, Vector3 backgroundColor)
		: this(layout.size, layout.position, new Vector4(backgroundColor, 1f))
	{
	}

	public Rectangle2DMeshWithTangents(Layout2D.LayoutData layout, Vector4 backgroundColor)
		: this(layout.size, layout.position, backgroundColor)
	{
	}

	public Rectangle2DMeshWithTangents(Vector2 size, Vector4 backgroundColor)
		: this(size, Vector2.Zero, backgroundColor)
	{
	}

	public Rectangle2DMeshWithTangents(Vector2 size, Vector2 offset, Vector4 backgroundColor)
	{
		this.size = size;
		this.offset = offset;
		initializeBuffer();
		Material item = new Material
		{
			DiffuseWithAlpha = backgroundColor,
			Ambient = GameMath.ToVector3(backgroundColor)
		};
		shader = ShaderManager.Shaders["GUIColor"];
		materials.Add(item);
	}

	public Rectangle2DMeshWithTangents(Layout2D.LayoutData layout, Texture2D backgroundTexture)
		: this(layout.size, layout.position, backgroundTexture)
	{
	}

	public Rectangle2DMeshWithTangents(Vector2 size, Texture2D backgroundTexture)
		: this(size, Vector2.Zero, backgroundTexture)
	{
	}

	public Rectangle2DMeshWithTangents(Vector2 size, Vector2 offset, Texture2D backgroundTexture)
	{
		this.size = size;
		this.offset = offset;
		initializeBuffer();
		Material material = new Material();
		shader = ShaderManager.Shaders["GUI"];
		material.Textures.Add(backgroundTexture);
		materials.Add(material);
	}

	public override void Render(Transform motion)
	{
		if (dirty)
		{
			fillBuffer();
		}
		base.Render(motion);
	}
}
