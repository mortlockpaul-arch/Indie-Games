using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// The Default Sprite 3D Billboard Particle System class.
/// This class just inherits from the Default Sprite Particle System class and overrides the DrawSprite()
/// function to draw the sprites as Billboards in 3D space.
/// </summary>
/// <typeparam name="Particle">The Particle class to use.</typeparam>
/// <typeparam name="Vertex">The Vertex Format to use.</typeparam>
public class DPSFDefaultSprite3DBillboardTextureCoordinates<Particle, Vertex> : DPSFDefaultSprite3DBillboardParticleSystem<Particle, Vertex> where Particle : DPSFParticle, new() where Vertex : struct, IDPSFParticleVertex
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DPSFDefaultSprite3DBillboardTextureCoordinates(Game cGame)
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
		DefaultSprite3DBillboardTextureCoordinatesParticle defaultSprite3DBillboardTextureCoordinatesParticle = (DefaultSprite3DBillboardTextureCoordinatesParticle)Particle;
		Vector3 vector = Vector3.Transform(defaultSprite3DBillboardTextureCoordinatesParticle.Position, base.View);
		Vector2 position = new Vector2(vector.X, vector.Y);
		Rectangle textureCoordinates = defaultSprite3DBillboardTextureCoordinatesParticle.TextureCoordinates;
		cSpriteBatch.Draw(scale: new Vector2(defaultSprite3DBillboardTextureCoordinatesParticle.Width / (float)textureCoordinates.Width, (0f - defaultSprite3DBillboardTextureCoordinatesParticle.Height) / (float)textureCoordinates.Height), origin: new Vector2(textureCoordinates.Width / 2, textureCoordinates.Height / 2), texture: base.Texture, position: position, sourceRectangle: textureCoordinates, color: defaultSprite3DBillboardTextureCoordinatesParticle.ColorAsPremultiplied, rotation: defaultSprite3DBillboardTextureCoordinatesParticle.Rotation, effects: defaultSprite3DBillboardTextureCoordinatesParticle.FlipMode, layerDepth: vector.Z);
	}
}
