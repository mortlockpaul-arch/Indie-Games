namespace System.Runtime.Serialization;

/// <summary>
/// Allows an object to control its own serialization and deserialization.
/// </summary>
public interface ISerializable
{
	/// <summary>
	/// Populates a System.Runtime.Serialization.SerializationInfo with the data
	/// needed to serialize the target object.
	/// </summary>
	/// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
	/// <param name="context">The destination (see System.Runtime.Serialization.StreamingContext) for this
	/// serialization.</param>
	void GetObjectData(SerializationInfo info, StreamingContext context);
}
