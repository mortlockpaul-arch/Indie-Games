namespace DPSF;

/// <summary>
/// The Type of Particles that the Particle Systems can draw. Different Particle Types are drawn in 
/// different ways. For example, four vertices are required to draw a Quad, and only one is required 
/// to draw a Point Sprite.
/// </summary>
public enum ParticleTypes
{
	/// <summary>
	/// This is the default settings when we don't know what Type of Particles are going to be used yet.
	/// A particle system is not considered Initialized until the Particle Type does not equal this.
	/// </summary>
	None = 0,
	/// <summary>
	/// Use this when you do not want to draw your particles to the screen, as no vertex buffer will be
	/// created, saving memory. Also, the Draw() function will do nothing when this Particle Type is used.
	/// This Particle Type is useful when you just want to collect and analyze particle information without
	/// visualizing the particles.
	/// </summary>
	NoDisplay = 1,
	/// <summary>
	/// Texture in 2D screen coordinates. Drawn using a SpriteBatch object. Only allows for 2D roll
	/// rotations, always faces the camera, and must use a Texture.
	/// </summary>
	Sprite = 3,
	/// <summary>
	/// Four vertices in 3D world coordinates. Allows for rotations in all 3 dimensions, does
	/// not have to always face the camera, may be skewed into any quadrilateral, such as
	/// a square, rectangle, or trapezoid, and do not use a Texture.
	/// </summary>
	Quad = 5,
	/// <summary>
	/// Four vertices in 3D world coordinates. Allows for rotations in all 3 dimensions, does
	/// not have to always face the camera, may be skewed into any quadrilateral, such as
	/// a square, rectangle, or trapezoid, and must use a Texture.
	/// </summary>
	TexturedQuad = 6
}
