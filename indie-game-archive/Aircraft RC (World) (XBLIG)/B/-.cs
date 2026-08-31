namespace B;

internal class _0002
{
	internal static uint _0001()
	{
		return 1u;
	}
}
internal class _0012
{
	internal class _0001CB
	{
		private string HCB;

		private string HC_0002;

		private string HC_0012;

		internal string DisplayName => HCB;

		internal string DRMProductName => HC_0002;

		internal string FileName => "SunBurn-Deploy.xnb";

		internal _0001CB(string P_0, string P_1, string P_2)
		{
			HCB = P_0;
			HC_0002 = P_1;
			HC_0012 = P_2;
		}
	}

	internal enum _0001C_0002
	{
		SunBurn_Indie,
		SunBurn_Pro,
		SunBurn_Studio,
		SunBurn_ProNonCom
	}

	private const string HCB = "SunBurn-Deploy.xnb";

	internal static _0001CB[] HC_0002 = new _0001CB[4]
	{
		new _0001CB("SunBurn Indie", "SunBurn Pro", "SunBurn2-Indie.auth"),
		new _0001CB("SunBurn Pro", "SunBurn Community", "SunBurn2-Pro.auth"),
		new _0001CB("SunBurn Studio", "SunBurn Studio", "SunBurn2-Studio.auth"),
		new _0001CB("SunBurn Pro Non-Commercial", "SunBurn ProNonCom", "SunBurn2-Pro-NonCommercial.auth")
	};

	private static string HC_0012 = null;

	internal static string ActivationPath
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = text;
			if (string.IsNullOrEmpty(HC_0012))
			{
				HC_0012 = "";
				return;
			}
			char c = HC_0012[HC_0012.Length - 1];
			if (c != '\\' && c != '/')
			{
				HC_0012 += "\\";
			}
		}
	}
}
