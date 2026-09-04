using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast;

public class PrimitiveBatch : IDisposable
{
	private const int DefaultBufferSize = 500;

	private VertexPositionColor[] vertices;

	private int positionInBuffer;

	private VertexDeclaration vertexDeclaration;

	private BasicEffect basicEffect;

	private GraphicsDevice device;

	private PrimitiveType primitiveType;

	private int numVertsPerPrimitive;

	private bool hasBegun;

	private bool isDisposed;

	public PrimitiveBatch(GraphicsDevice graphicsDevice)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		vertices = (VertexPositionColor[])(object)new VertexPositionColor[500];
		base._002Ector();
		if (graphicsDevice == null)
		{
			throw new ArgumentNullException("graphicsDevice");
		}
		device = graphicsDevice;
		vertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionColor.VertexElements);
		basicEffect = new BasicEffect(graphicsDevice, (EffectPool)null);
		basicEffect.VertexColorEnabled = true;
		BasicEffect obj = basicEffect;
		Viewport viewport = graphicsDevice.Viewport;
		float num = ((Viewport)(ref viewport)).Width;
		Viewport viewport2 = graphicsDevice.Viewport;
		obj.Projection = Matrix.CreateOrthographicOffCenter(0f, num, (float)((Viewport)(ref viewport2)).Height, 0f, 0f, 1f);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing && !isDisposed)
		{
			if (vertexDeclaration != null)
			{
				vertexDeclaration.Dispose();
			}
			if (basicEffect != null)
			{
				((Effect)basicEffect).Dispose();
			}
			isDisposed = true;
		}
	}

	public void Begin(PrimitiveType primitiveType)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (hasBegun)
		{
			throw new InvalidOperationException("End must be called before Begin can be called again.");
		}
		if ((int)primitiveType == 3 || (int)primitiveType == 6 || (int)primitiveType == 5)
		{
			throw new NotSupportedException("The specified primitiveType is not supported by PrimitiveBatch.");
		}
		this.primitiveType = primitiveType;
		numVertsPerPrimitive = NumVertsPerPrimitive(primitiveType);
		device.VertexDeclaration = vertexDeclaration;
		basicEffect.Projection = MainGame.ProjectionMatrix;
		basicEffect.View = MainGame.ViewMatrix;
		((Effect)basicEffect).Begin();
		((Effect)basicEffect).CurrentTechnique.Passes[0].Begin();
		hasBegun = true;
	}

	public void AddVertex(Vector2 vertex, Color color)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!hasBegun)
		{
			throw new InvalidOperationException("Begin must be called before AddVertex can be called.");
		}
		if (positionInBuffer % numVertsPerPrimitive == 0 && positionInBuffer + numVertsPerPrimitive >= vertices.Length)
		{
			Flush();
		}
		vertices[positionInBuffer].Position = new Vector3(vertex, 0f);
		vertices[positionInBuffer].Color = color;
		positionInBuffer++;
	}

	public void End()
	{
		if (!hasBegun)
		{
			throw new InvalidOperationException("Begin must be called before End can be called.");
		}
		Flush();
		((Effect)basicEffect).CurrentTechnique.Passes[0].End();
		((Effect)basicEffect).End();
		hasBegun = false;
	}

	private void Flush()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!hasBegun)
		{
			throw new InvalidOperationException("Begin must be called before Flush can be called.");
		}
		if (positionInBuffer != 0)
		{
			int num = positionInBuffer / numVertsPerPrimitive;
			device.DrawUserPrimitives<VertexPositionColor>(primitiveType, vertices, 0, num);
			positionInBuffer = 0;
		}
	}

	private static int NumVertsPerPrimitive(PrimitiveType primitive)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected I4, but got Unknown
		return (primitive - 1) switch
		{
			0 => 1, 
			1 => 2, 
			3 => 3, 
			_ => throw new InvalidOperationException("primitive is not valid"), 
		};
	}
}
