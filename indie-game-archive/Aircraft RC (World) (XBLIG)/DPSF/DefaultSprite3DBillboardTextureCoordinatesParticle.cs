using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// Particle used by the Default Sprite 3D Billboard Particle System
/// </summary>
public class DefaultSprite3DBillboardTextureCoordinatesParticle : DefaultSprite3DBillboardParticle
{
	/// <summary>
	/// The top-left Position and the Dimensions of this Picture in the Texture
	/// </summary>
	public Rectangle TextureCoordinates;

	/// <summary>
	/// Sets the Texture Coordinates to use for the Picture that represents this Particle.
	/// </summary>
	/// <param name="textureCoordinates">The top-left Position and the Dimensions of the Picture in the Texture.</param>
	public void SetTextureCoordinates(Rectangle textureCoordinates)
	{
		TextureCoordinates.X = textureCoordinates.X;
		TextureCoordinates.Y = textureCoordinates.Y;
		TextureCoordinates.Width = textureCoordinates.Width;
		TextureCoordinates.Height = textureCoordinates.Height;
	}

	/// <summary>
	/// Sets the Texture Coordinates to use for the Picture that represents this Particle
	/// </summary>
	/// <param name="iLeft">The X position of the top-left corner of the Picture in the Texture</param>
	/// <param name="iTop">The Y position of the top-left corner of the Picture in the Texture</param>
	/// <param name="iRight">The X position of the bottom-right corner of the Picture in the Texture</param>
	/// <param name="iBottom">The Y position of the bottom-right corner of the Picture in the Texture</param>
	public void SetTextureCoordinates(int iLeft, int iTop, int iRight, int iBottom)
	{
		TextureCoordinates.X = iLeft;
		TextureCoordinates.Y = iTop;
		TextureCoordinates.Width = iRight - iLeft;
		TextureCoordinates.Height = iBottom - iTop;
	}

	/// <summary>
	/// Resets the Particle variables to their default values
	/// </summary>
	public override void Reset()
	{
		base.Reset();
		TextureCoordinates = default(Rectangle);
	}

	/// <summary>
	/// Deep copy all of the Particle properties
	/// </summary>
	/// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
	public override void CopyFrom(DPSFParticle ParticleToCopy)
	{
		DefaultSprite3DBillboardTextureCoordinatesParticle defaultSprite3DBillboardTextureCoordinatesParticle = (DefaultSprite3DBillboardTextureCoordinatesParticle)ParticleToCopy;
		base.CopyFrom((DPSFParticle)defaultSprite3DBillboardTextureCoordinatesParticle);
		TextureCoordinates = defaultSprite3DBillboardTextureCoordinatesParticle.TextureCoordinates;
	}
}
