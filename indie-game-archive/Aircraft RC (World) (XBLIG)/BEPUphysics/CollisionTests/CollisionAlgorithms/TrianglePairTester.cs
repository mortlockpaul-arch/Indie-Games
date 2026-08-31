using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms;

/// <summary>
///  Persistent tester that compares triangles against convex objects.
/// </summary>
public abstract class TrianglePairTester
{
	internal TriangleShape triangle;

	/// <summary>
	///  Whether or not the pair tester was updated during the last attempt.
	/// </summary>
	public bool Updated;

	/// <summary>
	/// Whether or not the last found contact should have its normal corrected.
	/// </summary>
	public abstract bool ShouldCorrectContactNormal { get; }

	/// <summary>
	///  Generates a contact between the triangle and convex.
	/// </summary>
	/// <param name="contactList">Contact between the shapes, if any.</param>
	/// <returns>Whether or not the shapes are colliding.</returns>
	public abstract bool GenerateContactCandidate(out TinyStructList<ContactData> contactList);

	/// <summary>
	/// Gets the triangle region in which the contact resides.
	/// </summary>
	/// <param name="contact">Contact to check.</param>
	/// <returns>Region in which the contact resides.</returns>
	public abstract VoronoiRegion GetRegion(ref ContactData contact);

	/// <summary>
	///  Initializes the pair tester.
	/// </summary>
	/// <param name="convex">Convex shape to use.</param>
	/// <param name="triangle">Triangle shape to use.</param>
	public abstract void Initialize(ConvexShape convex, TriangleShape triangle);

	/// <summary>
	/// Cleans up the pair tester.
	/// </summary>
	public abstract void CleanUp();
}
