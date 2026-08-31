namespace System.Runtime.Serialization;

/// <summary>
/// Holds the value, Type, and name of a serialized object.
/// </summary>
public class SerializationEntry
{
	private string HCB = "";

	private object HC_0002;

	/// <summary>
	/// Gets the name of the object.
	/// </summary>
	public string Name => HCB;

	/// <summary>
	/// Gets the value contained in the object.
	/// </summary>
	public object Value => HC_0002;

	/// <summary>
	/// Creates a new SerializationEntry instance.
	/// </summary>
	/// <param name="name">Name of the object.</param>
	/// <param name="value">Value contained in the object.</param>
	public SerializationEntry(string name, object value)
	{
		HCB = name;
		HC_0002 = value;
	}
}
