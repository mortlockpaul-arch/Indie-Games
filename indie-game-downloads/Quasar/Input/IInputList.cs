namespace Quasar.Input;

public interface IInputList
{
	IInputState Result { get; }

	bool HasGlyph { get; }

	char Glyph { get; }

	void Update();

	void SetGlyph(char glyph);
}
