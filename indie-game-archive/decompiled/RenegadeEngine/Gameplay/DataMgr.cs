using Microsoft.Xna.Framework;
using RenegadeEngine.Cyclone;
using RenegadeEngine.Graphics;

namespace RenegadeEngine.Gameplay;

public static class DataMgr
{
	public static int numTinies = 2000;

	public static int numGiants = 3;

	public static RigidBody[] smallBodies = new RigidBody[numTinies];

	public static RigidBody[] giantBodies = new RigidBody[numGiants];

	public static Colors[] smallColors = new Colors[numTinies];

	public static Colors[] giantColors = new Colors[numGiants];

	public static float movementRate = 0.007f;

	public static float giantMass = 1000f;

	public static float tinyMass = 20f;

	public static float giantSize = 2.5f;

	public static float smallSize = 0.3f;

	public static BoundingFrustum frust;

	public static Sphere giantStar;

	public static Sphere smallStar;

	public static VertexColorInstanceWorld[] starTransforms = new VertexColorInstanceWorld[numTinies];

	public static VertexColorInstanceWorld[] giantTransforms = new VertexColorInstanceWorld[numGiants];
}
