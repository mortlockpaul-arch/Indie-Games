using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xclna.Xna.Animation.Visualization;

public sealed class CubeComponent : DrawableGameComponent, IAttachable
{
	private int[] indices;

	private VertexDeclaration vertexDeclaration;

	private BasicEffect effect;

	private IGraphicsDeviceService graphics;

	private Color color;

	private float sideLength;

	private VertexPositionColor[] verts;

	private Vector3[] buffer;

	private BonePose pose;

	private Matrix localTransform;

	public Matrix World
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return effect.World;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			effect.World = value;
		}
	}

	public Matrix View
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return effect.View;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			effect.View = value;
		}
	}

	public Matrix Projection
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return effect.Projection;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			effect.Projection = value;
		}
	}

	public Color Color
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return color;
		}
		set
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < verts.Length; i++)
			{
				verts[i].Color = value;
			}
			color = value;
		}
	}

	public BoundingBox BoundingBox
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			Matrix world = effect.World;
			for (int i = 0; i < buffer.Length; i++)
			{
				Vector3.Transform(ref verts[i].Position, ref world, ref buffer[i]);
			}
			return BoundingBox.CreateFromPoints((IEnumerable<Vector3>)buffer);
		}
	}

	public Matrix LocalTransform
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return localTransform;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			localTransform = value;
		}
	}

	Matrix IAttachable.CombinedTransform
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return World;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			World = value;
		}
	}

	public BonePose AttachedBone
	{
		get
		{
			return pose;
		}
		set
		{
			pose = value;
		}
	}

	public CubeComponent(Game game, Color color, float sideLength)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Expected O, but got Unknown
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		pose = null;
		localTransform = Matrix.Identity;
		((DrawableGameComponent)this)._002Ector(game);
		this.sideLength = sideLength;
		graphics = (IGraphicsDeviceService)game.Services.GetService(typeof(IGraphicsDeviceService));
		effect = new BasicEffect(graphics.GraphicsDevice, (EffectPool)null);
		indices = new int[36]
		{
			0, 1, 2, 2, 3, 0, 3, 2, 6, 6,
			7, 3, 7, 6, 5, 5, 4, 7, 4, 5,
			1, 1, 0, 4, 5, 6, 2, 2, 1, 5,
			7, 4, 0, 0, 3, 7
		};
		Vector3[] array = (Vector3[])(object)new Vector3[8]
		{
			new Vector3(-1f, -1f, 1f),
			new Vector3(-1f, -1f, -1f),
			new Vector3(-1f, 1f, -1f),
			new Vector3(-1f, 1f, 1f),
			new Vector3(1f, -1f, 1f),
			new Vector3(1f, -1f, -1f),
			new Vector3(1f, 1f, -1f),
			new Vector3(1f, 1f, 1f)
		};
		for (int i = 0; i < array.Length; i++)
		{
			ref Vector3 reference = ref array[i];
			reference.X *= sideLength / 2f;
			ref Vector3 reference2 = ref array[i];
			reference2.Y *= sideLength / 2f;
			ref Vector3 reference3 = ref array[i];
			reference3.Z *= sideLength / 2f;
		}
		vertexDeclaration = new VertexDeclaration(graphics.GraphicsDevice, VertexPositionColor.VertexElements);
		verts = (VertexPositionColor[])(object)new VertexPositionColor[8];
		buffer = (Vector3[])(object)new Vector3[8];
		for (int i = 0; i < verts.Length; i++)
		{
			verts[i].Position = array[i];
			verts[i].Color = color;
		}
		effect.VertexColorEnabled = true;
		((Collection<IGameComponent>)(object)game.Components).Add((IGameComponent)(object)this);
	}

	protected override void Dispose(bool disposing)
	{
		vertexDeclaration.Dispose();
		((Effect)effect).Dispose();
		((DrawableGameComponent)this).Dispose(disposing);
	}

	public override void Draw(GameTime gameTime)
	{
		((Effect)effect).Begin();
		foreach (EffectPass pass in ((Effect)effect).CurrentTechnique.Passes)
		{
			pass.Begin();
			graphics.GraphicsDevice.VertexDeclaration = vertexDeclaration;
			graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>((PrimitiveType)4, verts, 0, verts.Length, indices, 0, 12);
			pass.End();
		}
		((Effect)effect).End();
	}
}
