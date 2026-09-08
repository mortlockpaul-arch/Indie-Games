namespace Quasar.Meshes.Text;

internal struct TextBoxDrawResults(int baseIndex, int charCount, int pxWidth)
{
	public int baseIndex = baseIndex;

	public int charCount = charCount;

	public int pxWidth = pxWidth;
}
