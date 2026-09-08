using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using p;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Helper class that renders a full viewport quad using the user
/// effect provided to the Render method.
/// </summary>
public class FullFrameQuad : IDisposable
{
	private int _3A_0018;

	private int _3AL;

	private GraphicsDevice _3A_0019;

	private VertexBuffer _3A3;

	private VertexPositionTexture[] _3A6 = new VertexPositionTexture[4];

	private static VertexPositionTexture[] _3AD = new VertexPositionTexture[4];

	/// <summary>
	/// The quad's VertexBuffer (used in custom rendering).
	/// </summary>
	public VertexBuffer VertexBuffer => _3A3;

	/// <summary>
	/// Renders a quad without creating a unique FullFrameQuad instance.
	/// </summary>
	/// <param name="device"></param>
	/// <param name="width">Target wiewport width.</param>
	/// <param name="height">Target wiewport height.</param>
	public static void Render(GraphicsDevice device, int width, int height)
	{
		LL(_3AD, width, height, -Vector2.One, Vector2.One);
		device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _3AD, 0, 2);
	}

	private static void LL(VertexPositionTexture[] P_0, int P_1, int P_2, Vector2 P_3, Vector2 P_4)
	{
		Vector3 vector = new Vector3(-0.5f / (float)P_1, 0.5f / (float)P_2, 0f);
		P_0[0].Position = new Vector3(P_3.X, P_4.Y, 0f) + vector;
		P_0[0].TextureCoordinate = new Vector2(0f, 0f);
		P_0[1].Position = new Vector3(P_4.X, P_4.Y, 0f) + vector;
		P_0[1].TextureCoordinate = new Vector2(1f, 0f);
		P_0[2].Position = new Vector3(P_3.X, P_3.Y, 0f) + vector;
		P_0[2].TextureCoordinate = new Vector2(0f, 1f);
		P_0[3].Position = new Vector3(P_4.X, P_3.Y, 0f) + vector;
		P_0[3].TextureCoordinate = new Vector2(1f, 1f);
	}

	/// <summary>
	/// Creates a new FullFrameQuad instance.
	/// </summary>
	/// <param name="device"></param>
	/// <param name="width">Target wiewport width.</param>
	/// <param name="height">Target wiewport height.</param>
	public FullFrameQuad(GraphicsDevice device, int width, int height)
		: this(device, width, height, -Vector2.One, Vector2.One)
	{
	}

	/// <summary>
	/// Creates a new FullFrameQuad instance with min/max screen space
	/// rendering bounds for partial screen coverage.
	/// </summary>
	/// <param name="device"></param>
	/// <param name="width">Target wiewport width.</param>
	/// <param name="height">Target wiewport height.</param>
	/// <param name="screenmin">Screen space min render area.</param>
	/// <param name="screenmax">Screen space max render area.</param>
	public FullFrameQuad(GraphicsDevice device, int width, int height, Vector2 screenmin, Vector2 screenmax)
	{
		_3A_0019 = device;
		_3A_0018 = width;
		_3AL = height;
		LL(_3A6, width, height, screenmin, screenmax);
		_3A3 = new VertexBuffer(device, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);
		_3A3.SetData(_3A6);
	}

	/// <summary>
	/// Renders the quad using the supplied effect.
	/// </summary>
	/// <param name="effect"></param>
	public void Render(Effect effect)
	{
		_3A_0019.SetVertexBuffer(_3A3);
		for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
		{
			effect.CurrentTechnique.Passes[i].Apply();
			_3A_0019.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
		}
	}

	/// <summary>
	/// Renders the quad.
	/// </summary>
	public void Render()
	{
		_3A_0019.SetVertexBuffer(_3A3);
		_3A_0019.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
	}

	/// <summary>
	/// Disposes all related graphics objects.
	/// </summary>
	public void Dispose()
	{
		_3A_0019 = null;
		p._0018._6_0006(ref _3A3);
	}
}
