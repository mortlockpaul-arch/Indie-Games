using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Meshes.Generic;
using Quasar.Shaders;

namespace Quasar.Animation;

public class SpriteAnimationMesh : UserVertexMesh<SpriteAnimationVertex>
{
	public const string ANIMATION_SHADER = "SpriteAnims/SpriteAnim";

	private bool dirty;

	private SpriteAnimationSet animationSet;

	private Vector2 size;

	private Vector2 offset;

	private float zValue;

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

	public SpriteAnimationMesh(SpriteAnimationSet animationSet, Texture texture, Vector2 size)
	{
		this.size = size;
		this.animationSet = animationSet;
		Material material = new Material();
		material.Texture = texture;
		material.AddIntParameter(animationSet.GridSizeW);
		material.AddIntParameter(animationSet.GridSizeH);
		material.AddIntParameter(0);
		material.AddIntParameter(0);
		shader = ShaderManager.Shaders["SpriteAnims/SpriteAnim"];
		materials.Add(material);
		initializeBuffer();
	}

	private void fillBuffer()
	{
		Vector2 vector = size / 2f;
		verticesBuffer[0].Position = new Vector3(0f - vector.X + offset.X, vector.Y + offset.Y, zValue);
		verticesBuffer[1].Position = new Vector3(vector.X + offset.X, vector.Y + offset.Y, zValue);
		verticesBuffer[2].Position = new Vector3(0f - vector.X + offset.X, 0f - vector.Y + offset.Y, zValue);
		verticesBuffer[3].Position = new Vector3(vector.X + offset.X, 0f - vector.Y + offset.Y, zValue);
		verticesBuffer[0].UV = new Vector2(0f, 0f);
		verticesBuffer[1].UV = new Vector2(1f, 0f);
		verticesBuffer[2].UV = new Vector2(0f, 1f);
		verticesBuffer[3].UV = new Vector2(1f, 1f);
		updateBoundingSphere(4);
		dirty = false;
	}

	private void initializeBuffer()
	{
		initMesh(4);
		base.PrimitiveCount = 2;
		base.PrimitiveType = PrimitiveType.TriangleStrip;
		fillBuffer();
		updateBoundingSphere(4);
	}

	public void UpdateAnimation(ref SpriteAnimationState currentState)
	{
		for (int i = 0; i < 4; i++)
		{
			verticesBuffer[i].Tile = currentState.CurrentFrame;
		}
		base.FirstMaterial.SetIntParameter(2, currentState.FlipX ? 1 : 0);
		base.FirstMaterial.SetIntParameter(3, currentState.FlipY ? 1 : 0);
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
