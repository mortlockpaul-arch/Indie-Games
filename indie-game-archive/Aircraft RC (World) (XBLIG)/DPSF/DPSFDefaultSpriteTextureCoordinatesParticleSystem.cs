using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// The Default Sprite with Texture Coordinates Particle System class
/// </summary>
/// <typeparam name="Particle">The Particle class to use</typeparam>
/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
public class DPSFDefaultSpriteTextureCoordinatesParticleSystem<Particle, Vertex> : DPSFDefaultSpriteParticleSystem<Particle, Vertex> where Particle : DPSFParticle, new() where Vertex : struct, IDPSFParticleVertex
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DPSFDefaultSpriteTextureCoordinatesParticleSystem(Game cGame)
		: base(cGame)
	{
	}

	/// <summary>
	/// Function to draw a Sprite Particle. This function should be used to draw the given
	/// Particle with the provided SpriteBatch.
	/// </summary>
	/// <param name="Particle">The Particle Sprite to Draw</param>
	/// <param name="cSpriteBatch">The SpriteBatch to use to doing the Drawing</param>
	protected override void DrawSprite(DPSFParticle Particle, SpriteBatch cSpriteBatch)
	{
		DefaultSpriteTextureCoordinatesParticle defaultSpriteTextureCoordinatesParticle = (DefaultSpriteTextureCoordinatesParticle)Particle;
		Rectangle destinationRectangle = new Rectangle((int)defaultSpriteTextureCoordinatesParticle.Position.X, (int)defaultSpriteTextureCoordinatesParticle.Position.Y, (int)defaultSpriteTextureCoordinatesParticle.Width, (int)defaultSpriteTextureCoordinatesParticle.Height);
		float layerDepth = MathHelper.Clamp(defaultSpriteTextureCoordinatesParticle.Position.Z, 0f, 1f);
		Rectangle textureCoordinates = defaultSpriteTextureCoordinatesParticle.TextureCoordinates;
		cSpriteBatch.Draw(origin: new Vector2(textureCoordinates.Width / 2, textureCoordinates.Height / 2), texture: base.Texture, destinationRectangle: destinationRectangle, sourceRectangle: textureCoordinates, color: defaultSpriteTextureCoordinatesParticle.Color, rotation: defaultSpriteTextureCoordinatesParticle.Rotation, effects: defaultSpriteTextureCoordinatesParticle.FlipMode, layerDepth: layerDepth);
	}
}
