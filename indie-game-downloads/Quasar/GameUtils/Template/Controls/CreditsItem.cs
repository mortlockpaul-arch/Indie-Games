using Quasar.Language;

namespace Quasar.GameUtils.Template.Controls;

public class CreditsItem
{
	private string image;

	private string name;

	private string gamertag;

	private bool hasLongDesc;

	private string longDesc;

	private CreditsFunctions function;

	public string Image => image;

	public string Name => name;

	public string Gamertag => gamertag;

	public bool HasGamertag
	{
		get
		{
			if (gamertag != null)
			{
				return gamertag.Length > 0;
			}
			return false;
		}
	}

	public CreditsFunctions Function => function;

	public string Description
	{
		get
		{
			if (hasLongDesc)
			{
				return longDesc;
			}
			string text = "";
			string text2 = "";
			if ((function & CreditsFunctions.Graphics) != 0)
			{
				text = text + text2 + "CREDITS_GRAPHICS".Translate();
				text2 = ", ";
			}
			if ((function & CreditsFunctions.Gameplay) != 0)
			{
				text = text + text2 + "CREDITS_GAMEPLAY".Translate();
				text2 = ", ";
			}
			if ((function & CreditsFunctions.Betatest) != 0)
			{
				text = text + text2 + "CREDITS_BETATEST".Translate();
				text2 = ", ";
			}
			if ((function & CreditsFunctions.Sound) != 0)
			{
				text = text + text2 + "CREDITS_SOUND".Translate();
				text2 = ", ";
			}
			if ((function & CreditsFunctions.Music) != 0)
			{
				text = text + text2 + "CREDITS_MUSIC".Translate();
				text2 = ", ";
			}
			if ((function & CreditsFunctions.Programming) != 0)
			{
				text = text + text2 + "CREDITS_PROGRAMMING".Translate();
				text2 = ", ";
			}
			if ((function & CreditsFunctions.Modelling) != 0)
			{
				text = text + text2 + "CREDITS_MODELLING".Translate();
				text2 = ", ";
			}
			if ((function & CreditsFunctions.Voice) != 0)
			{
				text = text + text2 + "CREDITS_VOICE".Translate();
				text2 = ", ";
			}
			return text;
		}
	}

	public CreditsItem(string name, string gamertag, CreditsFunctions function, string image)
	{
		this.image = image;
		this.name = name;
		this.gamertag = gamertag;
		this.function = function;
	}

	public CreditsItem(string name, string longDesc, string image)
	{
		this.image = image;
		this.name = name;
		hasLongDesc = true;
		this.longDesc = longDesc;
	}
}
