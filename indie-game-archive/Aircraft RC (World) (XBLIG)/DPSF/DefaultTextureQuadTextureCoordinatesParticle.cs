using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// Particle used by the Default Textured Quad with Texture Coordinates Particle System
/// </summary>
public class DefaultTextureQuadTextureCoordinatesParticle : DefaultTexturedQuadParticle
{
	/// <summary>
	/// The Normalized (0.0 - 1.0) Top-Left Texture Coordinate to use for the Particle's image
	/// </summary>
	public Vector2 NormalizedTextureCoordinateLeftTop;

	/// <summary>
	/// The Normalized (0.0 - 1.0) Bottom-Right Texture Coordinate to use for the Particle's image
	/// </summary>
	public Vector2 NormalizedTextureCoordinateRightBottom;

	/// <summary>
	/// Sets the Normalized Texture Coordinates using the absolute (i.e. non-normalized) top-left coordinate and the dimensions of the Picture in the Texture
	/// </summary>
	/// <param name="sTextureCoordinates">The top-left Position and the Dimensions of the Picture in the Texture</param>
	/// <param name="iTextureWidth">The Width of the Texture that the Picture is in</param>
	/// <param name="iTextureHeight">The Height of the Texture that the Picture is in</param>
	public void SetTextureCoordinates(Rectangle sTextureCoordinates, int iTextureWidth, int iTextureHeight)
	{
		SetTextureCoordinates(sTextureCoordinates.Left, sTextureCoordinates.Top, sTextureCoordinates.Right, sTextureCoordinates.Bottom, iTextureWidth, iTextureHeight);
	}

	/// <summary>
	/// Sets the Normalized Texture Coordinates using the absolute (i.e. non-normalized) coordinates of the Picture in the Texture
	/// </summary>
	/// <param name="iLeft">The X position of the top-left corner of the Picture in the Texture</param>
	/// <param name="iTop">The Y position of the top-left corner of the Picture in the Texture</param>
	/// <param name="iRight">The X position of the bottom-right corner of the Picture in the Texture</param>
	/// <param name="iBottom">The Y position of the bottom-right corner of the Picture in the Texture</param>
	/// <param name="iTextureWidth">The Width of the Texture that the Picture is in</param>
	/// <param name="iTextureHeight">The Height of the Texture that the Picture is in</param>
	public void SetTextureCoordinates(int iLeft, int iTop, int iRight, int iBottom, int iTextureWidth, int iTextureHeight)
	{
		NormalizedTextureCoordinateLeftTop.X = (float)iLeft / (float)iTextureWidth;
		NormalizedTextureCoordinateLeftTop.Y = (float)iTop / (float)iTextureHeight;
		NormalizedTextureCoordinateRightBottom.X = (float)iRight / (float)iTextureHeight;
		NormalizedTextureCoordinateRightBottom.Y = (float)iBottom / (float)iTextureHeight;
	}

	/// <summary>
	/// Resets the Particle variables to their default values
	/// </summary>
	public override void Reset()
	{
		base.Reset();
		NormalizedTextureCoordinateLeftTop = new Vector2(0f, 0f);
		NormalizedTextureCoordinateRightBottom = new Vector2(1f, 1f);
	}

	/// <summary>
	/// Deep copy all of the Particle properties
	/// </summary>
	/// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
	public override void CopyFrom(DPSFParticle ParticleToCopy)
	{
		DefaultTextureQuadTextureCoordinatesParticle defaultTextureQuadTextureCoordinatesParticle = (DefaultTextureQuadTextureCoordinatesParticle)ParticleToCopy;
		base.CopyFrom((DPSFParticle)defaultTextureQuadTextureCoordinatesParticle);
		NormalizedTextureCoordinateLeftTop = defaultTextureQuadTextureCoordinatesParticle.NormalizedTextureCoordinateLeftTop;
		NormalizedTextureCoordinateRightBottom = defaultTextureQuadTextureCoordinatesParticle.NormalizedTextureCoordinateRightBottom;
	}
}
