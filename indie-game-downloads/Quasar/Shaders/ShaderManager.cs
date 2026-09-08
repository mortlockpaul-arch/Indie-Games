using Quasar.Global;

namespace Quasar.Shaders;

public class ShaderManager : Manager<Shader>
{
	public const string BASE_SHADER = "Base";

	public const string ERROR_SHADER = "Error";

	public const string GUICOLOR_SHADER = "GUIColor";

	public const string SIMPLE_SHADER = "Simple";

	public const string GUI_SHADER = "GUI";

	public const string POINTGUI_SHADER = "PointGUI";

	private static ShaderManager instance;

	public static ShaderManager Shaders
	{
		get
		{
			if (instance == null)
			{
				instance = new ShaderManager();
			}
			return instance;
		}
	}

	private ShaderManager()
	{
	}

	public static Shader LoadShader(string path)
	{
		return LoadShader(path, addToManager: false);
	}

	protected override Shader createDefaultItem()
	{
		return FixedShader.GetShader("Error");
	}

	protected override Shader LoadItem(string name)
	{
		if (!(name == "GUI") && !(name == "PointGUI"))
		{
			return new HLSLShader("Shaders/" + name);
		}
		return FixedShader.GetShader("GUI");
	}

	public static Shader LoadShader(string path, bool addToManager)
	{
		if (addToManager && Shaders.TryGetValue(path, out var item))
		{
			return item;
		}
		try
		{
			item = new HLSLShader(path);
		}
		catch
		{
			item = Shaders.DefaultItem;
		}
		if (addToManager)
		{
			Shaders.Add(path, item);
		}
		return item;
	}
}
