using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Language;
using Quasar.Textures;

namespace Quasar.GameUtils.Awards;

public class Award
{
	public const string AWARD_TEXTURE_DIR = "Awards/";

	public const string AWARD_SECRET_TEXTURE = "Awards/secret_award";

	private string id;

	private string name;

	private string hint;

	private string description;

	private Texture2D texture;

	private uint progressNeeded;

	private uint progressIncrement;

	private bool secret;

	public static Texture2D SecretTexture => TextureManager.Textures["Awards/secret_award"];

	public string Id => id;

	public string Name => name;

	public string Hint => hint;

	public string Description => description;

	public Texture2D Texture => texture;

	public uint ProgressNeeded => progressNeeded;

	public uint ProgressIncrement => progressIncrement;

	public bool IsSecret => secret;

	public bool IsProgress => progressNeeded > 1;

	public virtual void FromXml(XElement xe)
	{
		progressIncrement = xe.ParseUIntAttribute("progressIncrement", 1u);
		progressNeeded = xe.ParseUIntAttribute("progressNeeded", 1u);
		id = xe.GetAttribute("id");
		secret = xe.ParseBoolAttribute("secret", defaultValue: false);
		string text = id.ToUpper();
		description = LanguageManager.Texts["AWARD_" + text + "_DESCRIPTION"];
		hint = LanguageManager.Texts["AWARD_" + text + "_HINT"];
		name = LanguageManager.Texts["AWARD_" + text + "_NAME"];
		texture = TextureManager.Textures["Awards/" + id];
	}
}
