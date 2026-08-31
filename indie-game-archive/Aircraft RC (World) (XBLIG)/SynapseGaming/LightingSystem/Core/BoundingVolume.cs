using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Class used to provide a bounding box
/// and sphere for an object.
/// </summary>
public class BoundingVolume : IBoundingVolume
{
	[CompilerGenerated]
	private BoundingBox HCB;

	[CompilerGenerated]
	private BoundingSphere HC_0002;

	/// <summary>
	/// Bounding area that completely contains the associated object.
	/// </summary>
	public BoundingBox BoundingBox
	{
		[CompilerGenerated]
		get
		{
			return HCB;
		}
		[CompilerGenerated]
		set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// Bounding area that completely contains the associated object.
	/// </summary>
	public BoundingSphere BoundingSphere
	{
		[CompilerGenerated]
		get
		{
			return HC_0002;
		}
		[CompilerGenerated]
		set
		{
			HC_0002 = value;
		}
	}
}
