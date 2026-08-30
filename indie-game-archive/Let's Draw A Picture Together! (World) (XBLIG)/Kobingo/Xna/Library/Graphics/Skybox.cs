using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Library.Graphics;

public class Skybox
{
	[CompilerGenerated]
	private Matrix _003CView_003Ek__BackingField;

	[CompilerGenerated]
	private Matrix _003CProjection_003Ek__BackingField;

	public Texture2D Front { get; set; }

	public Texture2D Back { get; set; }

	public Texture2D Left { get; set; }

	public Texture2D Right { get; set; }

	public Texture2D Top { get; set; }

	public Texture2D Bottom { get; set; }

	public GraphicsDevice GraphicsDevice { get; private set; }

	public Matrix View
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CView_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CView_003Ek__BackingField = value;
		}
	}

	public Matrix Projection
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CProjection_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CProjection_003Ek__BackingField = value;
		}
	}

	public BasicEffect Effect { get; private set; }

	public Skybox(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
	{
		if (graphicsDevice == null)
		{
			throw new ArgumentNullException("graphicsDevice");
		}
		if (basicEffect == null)
		{
			throw new ArgumentNullException("basicEffect");
		}
		GraphicsDevice = graphicsDevice;
		Effect = basicEffect;
	}

	public void Draw()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		VertexPositionTexture[] array = (VertexPositionTexture[])(object)new VertexPositionTexture[6]
		{
			new VertexPositionTexture(new Vector3(-10f, 10f, 0f), new Vector2(0f, 0f)),
			new VertexPositionTexture(new Vector3(10f, 10f, 0f), new Vector2(1f, 0f)),
			new VertexPositionTexture(new Vector3(-10f, -10f, 0f), new Vector2(0f, 1f)),
			new VertexPositionTexture(new Vector3(10f, 10f, 0f), new Vector2(1f, 0f)),
			new VertexPositionTexture(new Vector3(10f, -10f, 0f), new Vector2(1f, 1f)),
			new VertexPositionTexture(new Vector3(-10f, -10f, 0f), new Vector2(0f, 1f))
		};
		GraphicsDevice.SamplerStates[0].AddressU = (TextureAddressMode)3;
		GraphicsDevice.SamplerStates[0].AddressV = (TextureAddressMode)3;
		Effect.Projection = Projection;
		Effect.View = View;
		Effect.TextureEnabled = true;
		GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionTexture.VertexElements);
		GraphicsDevice.RenderState.DepthBufferEnable = true;
		((Effect)Effect).Begin();
		foreach (EffectPass pass in ((Effect)Effect).CurrentTechnique.Passes)
		{
			pass.Begin();
			Effect.World = Matrix.CreateRotationY(MathHelper.ToRadians(180f)) * Matrix.CreateTranslation(new Vector3(0f, 0f, 10f));
			Effect.Texture = Front;
			((Effect)Effect).CommitChanges();
			GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>((PrimitiveType)4, array, 0, 2);
			Effect.World = Matrix.CreateTranslation(new Vector3(0f, 0f, -10f));
			Effect.Texture = Back;
			((Effect)Effect).CommitChanges();
			GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>((PrimitiveType)4, array, 0, 2);
			Effect.World = Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(new Vector3(-10f, 0f, 0f));
			Effect.Texture = Left;
			((Effect)Effect).CommitChanges();
			GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>((PrimitiveType)4, array, 0, 2);
			Effect.World = Matrix.CreateRotationY(MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(new Vector3(10f, 0f, 0f));
			Effect.Texture = Right;
			((Effect)Effect).CommitChanges();
			GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>((PrimitiveType)4, array, 0, 2);
			Effect.World = Matrix.CreateRotationX(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(new Vector3(0f, 10f, 0f));
			Effect.Texture = Top;
			((Effect)Effect).CommitChanges();
			GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>((PrimitiveType)4, array, 0, 2);
			Effect.World = Matrix.CreateRotationX(MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(new Vector3(0f, -10f, 0f));
			Effect.Texture = Bottom;
			((Effect)Effect).CommitChanges();
			GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>((PrimitiveType)4, array, 0, 2);
			pass.End();
		}
		((Effect)Effect).End();
	}
}
