using System.Runtime.CompilerServices;

namespace System.Net.Quic;

public sealed class QuicReceiveWindowSizes
{
	public int Connection { get; set; } = 16777216;

	public int LocallyInitiatedBidirectionalStream { get; set; } = 65536;

	public int RemotelyInitiatedBidirectionalStream { get; set; } = 65536;

	public int UnidirectionalStream { get; set; } = 65536;

	internal void Validate(string argumentName)
	{
		ValidatePowerOf(argumentName, Connection, "Connection");
		ValidatePowerOf(argumentName, LocallyInitiatedBidirectionalStream, "LocallyInitiatedBidirectionalStream");
		ValidatePowerOf(argumentName, RemotelyInitiatedBidirectionalStream, "RemotelyInitiatedBidirectionalStream");
		ValidatePowerOf(argumentName, UnidirectionalStream, "UnidirectionalStream");
		static void ValidatePowerOf(string paramName, int value, [CallerArgumentExpression("value")] string propertyName = null)
		{
			if (value <= 0 || ((value - 1) & value) != 0)
			{
				throw new ArgumentOutOfRangeException(paramName, value, System.SR.Format(System.SR.net_quic_power_of_2, "InitialReceiveWindowSizes." + propertyName));
			}
		}
	}
}
