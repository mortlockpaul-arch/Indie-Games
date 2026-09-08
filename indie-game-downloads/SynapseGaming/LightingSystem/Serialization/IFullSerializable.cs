using System.Runtime.Serialization;

namespace SynapseGaming.LightingSystem.Serialization;

/// <summary>
/// Interface that provides objects with serialization support
/// compatible with SunBurn's built-in xml format.
/// </summary>
public interface IFullSerializable : ISerializable
{
	/// <summary>
	/// Populates an object with data from the System.Runtime.Serialization.SerializationInfo.
	/// </summary>
	/// <param name="info">The System.Runtime.Serialization.SerializationInfo to retrieve data from.</param>
	/// <param name="context">The source (see System.Runtime.Serialization.StreamingContext) for this serialization.</param>
	void SetObjectData(SerializationInfo info, StreamingContext context);
}
