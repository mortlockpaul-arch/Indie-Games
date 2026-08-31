using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// Magnet that attracts particles to/from an infinite line in 3D space
/// </summary>
public class MagnetLine : DefaultParticleSystemMagnet
{
	/// <summary>
	/// The Direction that the Line points in
	/// </summary>
	private Vector3 msDirection = Vector3.Forward;

	/// <summary>
	/// A 3D point that the Line passes through
	/// </summary>
	public Vector3 PositionOnLine { get; set; }

	/// <summary>
	/// The direction that the Line points in. This direction, along with the opposite (i.e. negative) of 
	/// this direction form the line, since a line has infinite length. This value is 
	/// automatically normalized when it is set.
	/// </summary>
	public Vector3 Direction
	{
		get
		{
			return msDirection;
		}
		set
		{
			msDirection = value;
			msDirection.Normalize();
		}
	}

	/// <summary>
	/// Explicit Constructor
	/// </summary>
	/// <param name="sPositionOnLine">A 3D Position that the Line Magnet passes through</param>
	/// <param name="sDirection">The Direction that the Line points in</param>
	/// <param name="eMode">The Mode that the Magnet should be in</param>
	/// <param name="eDistanceFunction">The Function to use to determine how much a Particle should be affected by 
	/// the Magnet based on how far away from the Magnet it is</param>
	/// <param name="fMinDistance">The Min Distance that the Magnet should be able to affect Particles at. If the
	/// Particle is closer to the Magnet than this distance, the Magnet will not affect the Particle.</param>
	/// <param name="fMaxDistance">The Max Distance that the Magnet should be able to affect Particles at. If the
	/// Particle is further away from the Magnet tan this distance, the Manget will not affect the Particle.</param>
	/// <param name="fMaxForce">The Max Force that the Magnet is able to exert on a Particle</param>
	/// <param name="iType">The Type of Magnet this is. This may be used in conjunction with the "Other" Magnet
	/// Mode to distinguish which type of custom user effect the Magnet should have on the Particles.</param>
	public MagnetLine(Vector3 sPositionOnLine, Vector3 sDirection, MagnetModes eMode, DistanceFunctions eDistanceFunction, float fMinDistance, float fMaxDistance, float fMaxForce, int iType)
		: base(eMode, eDistanceFunction, fMinDistance, fMaxDistance, fMaxForce, iType)
	{
		meMagnetType = MagnetTypes.LineMagnet;
		PositionOnLine = sPositionOnLine;
		Direction = sDirection;
	}

	/// <summary>
	/// Copy Constructor
	/// </summary>
	/// <param name="cMagnetToCopy">The Line Magnet to copy from</param>
	public MagnetLine(MagnetLine cMagnetToCopy)
		: base(MagnetModes.Attract, DistanceFunctions.Constant, 0f, 0f, 0f, 0)
	{
		CopyFrom(cMagnetToCopy);
	}

	/// <summary>
	/// Copies the given Line Magnet's data into this Line Magnet's data
	/// </summary>
	/// <param name="cMagnetToCopy">The Line Magnet to copy from</param>
	public void CopyFrom(MagnetLine cMagnetToCopy)
	{
		CopyFrom((DefaultParticleSystemMagnet)cMagnetToCopy);
		PositionOnLine = cMagnetToCopy.PositionOnLine;
		Direction = cMagnetToCopy.Direction;
	}

	/// <summary>
	/// Sets the Direction of the Line by specifying 2 points in 3D space that are on the Line.
	/// <para>NOTE: The 2 points cannot be the same.</para>
	/// </summary>
	/// <param name="sFirstPointOnTheLine">The first point that falls on the Line</param>
	/// <param name="sSecondPointOnTheLine">The second point that falls on the Line</param>
	public void SetDirection(Vector3 sFirstPointOnTheLine, Vector3 sSecondPointOnTheLine)
	{
		if (sFirstPointOnTheLine != sSecondPointOnTheLine)
		{
			Direction = sSecondPointOnTheLine - sFirstPointOnTheLine;
		}
	}
}
