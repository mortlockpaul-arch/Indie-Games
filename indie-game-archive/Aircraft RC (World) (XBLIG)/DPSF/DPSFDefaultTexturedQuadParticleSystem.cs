using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// The Default Textured Quad Particle System class
/// </summary>
/// <typeparam name="Particle">The Particle class to use</typeparam>
/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
public class DPSFDefaultTexturedQuadParticleSystem<Particle, Vertex> : DPSFDefaultQuadParticleSystem<Particle, Vertex> where Particle : DPSFParticle, new() where Vertex : struct, IDPSFParticleVertex
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DPSFDefaultTexturedQuadParticleSystem(Game cGame)
		: base(cGame)
	{
	}

	/// <summary>
	/// Function to update the Vertex properties according to the Particle properties
	/// </summary>
	/// <param name="sVertexBuffer">The array containing the Vertices to be drawn</param>
	/// <param name="iIndex">The Index in the array where the Particle's Vertex info should be placed</param>
	/// <param name="Particle">The Particle to copy the information from</param>
	protected virtual void UpdateVertexProperties(ref DefaultTexturedQuadParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
	{
		DefaultTexturedQuadParticle defaultTexturedQuadParticle = (DefaultTexturedQuadParticle)Particle;
		float num = defaultTexturedQuadParticle.Width / 2f;
		float num2 = defaultTexturedQuadParticle.Height / 2f;
		Vector3 value = new Vector3(0f - num, 0f - num2, 0f);
		Vector3 value2 = new Vector3(num, 0f - num2, 0f);
		Vector3 value3 = new Vector3(0f - num, num2, 0f);
		Vector3 value4 = new Vector3(num, num2, 0f);
		value = Vector3.Transform(value, defaultTexturedQuadParticle.Orientation) + defaultTexturedQuadParticle.Position;
		value2 = Vector3.Transform(value2, defaultTexturedQuadParticle.Orientation) + defaultTexturedQuadParticle.Position;
		value3 = Vector3.Transform(value3, defaultTexturedQuadParticle.Orientation) + defaultTexturedQuadParticle.Position;
		value4 = Vector3.Transform(value4, defaultTexturedQuadParticle.Orientation) + defaultTexturedQuadParticle.Position;
		Color colorAsPremultiplied = defaultTexturedQuadParticle.ColorAsPremultiplied;
		sVertexBuffer[iIndex].Position = value3;
		sVertexBuffer[iIndex].TextureCoordinate = new Vector2(0f, 1f);
		sVertexBuffer[iIndex].Color = colorAsPremultiplied;
		sVertexBuffer[iIndex + 1].Position = value;
		sVertexBuffer[iIndex + 1].TextureCoordinate = new Vector2(0f, 0f);
		sVertexBuffer[iIndex + 1].Color = colorAsPremultiplied;
		sVertexBuffer[iIndex + 2].Position = value4;
		sVertexBuffer[iIndex + 2].TextureCoordinate = new Vector2(1f, 1f);
		sVertexBuffer[iIndex + 2].Color = colorAsPremultiplied;
		sVertexBuffer[iIndex + 3].Position = value2;
		sVertexBuffer[iIndex + 3].TextureCoordinate = new Vector2(1f, 0f);
		sVertexBuffer[iIndex + 3].Color = colorAsPremultiplied;
		if (base.GraphicsDevice.GraphicsProfile == GraphicsProfile.HiDef)
		{
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 1;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 2;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 1;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 3;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 2;
		}
		else
		{
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 1);
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 2);
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)iIndex;
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 1);
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 3);
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 2);
		}
	}

	/// <summary>
	/// Virtual function to Set the Effect's Parameters before drawing the Particles
	/// </summary>
	protected override void SetEffectParameters()
	{
		if (base.Effect is AlphaTestEffect alphaTestEffect)
		{
			alphaTestEffect.World = base.World;
			alphaTestEffect.View = base.View;
			alphaTestEffect.Projection = base.Projection;
			alphaTestEffect.Texture = base.Texture;
			alphaTestEffect.VertexColorEnabled = true;
		}
	}
}
