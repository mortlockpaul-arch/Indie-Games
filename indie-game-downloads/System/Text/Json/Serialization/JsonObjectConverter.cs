namespace System.Text.Json.Serialization;

internal abstract class JsonObjectConverter<T> : JsonResumableConverter<T>
{
	internal override bool CanPopulate => true;

	private protected sealed override ConverterStrategy GetDefaultConverterStrategy()
	{
		return ConverterStrategy.Object;
	}
}
