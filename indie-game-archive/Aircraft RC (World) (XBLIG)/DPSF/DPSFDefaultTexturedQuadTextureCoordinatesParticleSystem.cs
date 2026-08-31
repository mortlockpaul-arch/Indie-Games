using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// The Default Textured Quad with Texture Coordinates Particle System class
/// </summary>
/// <typeparam name="Particle">The Particle class to use</typeparam>
/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
public class DPSFDefaultTexturedQuadTextureCoordinatesParticleSystem<Particle, Vertex> : DPSFDefaultTexturedQuadParticleSystem<Particle, Vertex> where Particle : DPSFParticle, new() where Vertex : struct, IDPSFParticleVertex
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DPSFDefaultTexturedQuadTextureCoordinatesParticleSystem(Game cGame)
		: base(cGame)
	{
	}

	/// <summary>
	/// Function to update the Vertex properties according to the Particle properties
	/// </summary>
	/// <param name="sVertexBuffer">The array containing the Vertices to be drawn</param>
	/// <param name="iIndex">The Index in the array where the Particle's Vertex info should be placed</param>
	/// <param name="Particle">The Particle to copy the information from</param>
	protected override void UpdateVertexProperties(ref DefaultTexturedQuadParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
	{
		DefaultTextureQuadTextureCoordinatesParticle defaultTextureQuadTextureCoordinatesParticle = (DefaultTextureQuadTextureCoordinatesParticle)Particle;
		float num = defaultTextureQuadTextureCoordinatesParticle.Width / 2f;
		float num2 = defaultTextureQuadTextureCoordinatesParticle.Height / 2f;
		Vector3 value = new Vector3(0f - num, num2, 0f);
		Vector3 value2 = new Vector3(num, num2, 0f);
		Vector3 value3 = new Vector3(0f - num, 0f - num2, 0f);
		Vector3 value4 = new Vector3(num, 0f - num2, 0f);
		value = Vector3.Transform(value, defaultTextureQuadTextureCoordinatesParticle.Orientation) + defaultTextureQuadTextureCoordinatesParticle.Position;
		value2 = Vector3.Transform(value2, defaultTextureQuadTextureCoordinatesParticle.Orientation) + defaultTextureQuadTextureCoordinatesParticle.Position;
		value3 = Vector3.Transform(value3, defaultTextureQuadTextureCoordinatesParticle.Orientation) + defaultTextureQuadTextureCoordinatesParticle.Position;
		value4 = Vector3.Transform(value4, defaultTextureQuadTextureCoordinatesParticle.Orientation) + defaultTextureQuadTextureCoordinatesParticle.Position;
		Color colorAsPremultiplied = defaultTextureQuadTextureCoordinatesParticle.ColorAsPremultiplied;
		sVertexBuffer[iIndex].Position = value3;
		sVertexBuffer[iIndex].TextureCoordinate = new Vector2(defaultTextureQuadTextureCoordinatesParticle.NormalizedTextureCoordinateLeftTop.X, defaultTextureQuadTextureCoordinatesParticle.NormalizedTextureCoordinateRightBottom.Y);
		sVertexBuffer[iIndex].Color = colorAsPremultiplied;
		sVertexBuffer[iIndex + 1].Position = value;
		sVertexBuffer[iIndex + 1].TextureCoordinate = defaultTextureQuadTextureCoordinatesParticle.NormalizedTextureCoordinateLeftTop;
		sVertexBuffer[iIndex + 1].Color = colorAsPremultiplied;
		sVertexBuffer[iIndex + 2].Position = value4;
		sVertexBuffer[iIndex + 2].TextureCoordinate = defaultTextureQuadTextureCoordinatesParticle.NormalizedTextureCoordinateRightBottom;
		sVertexBuffer[iIndex + 2].Color = colorAsPremultiplied;
		sVertexBuffer[iIndex + 3].Position = value2;
		sVertexBuffer[iIndex + 3].TextureCoordinate = new Vector2(defaultTextureQuadTextureCoordinatesParticle.NormalizedTextureCoordinateRightBottom.X, defaultTextureQuadTextureCoordinatesParticle.NormalizedTextureCoordinateLeftTop.Y);
		sVertexBuffer[iIndex + 3].Color = colorAsPremultiplied;
		if (base.GraphicsDevice.GraphicsProfile == GraphicsProfile.HiDef)
		{
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 2;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 1;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 2;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 3;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 1;
		}
		else
		{
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)iIndex;
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 2);
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 1);
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 2);
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 3);
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 1);
		}
	}
}
