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
	private BoundingBox _3A_0018;

	[CompilerGenerated]
	private BoundingSphere _3AL;

	/// <summary>
	/// Bounding area that completely contains the associated object.
	/// </summary>
	public BoundingBox BoundingBox
	{
		[CompilerGenerated]
		get
		{
			return _3A_0018;
		}
		[CompilerGenerated]
		set
		{
			_3A_0018 = value;
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
			return _3AL;
		}
		[CompilerGenerated]
		set
		{
			_3AL = value;
		}
	}
}
