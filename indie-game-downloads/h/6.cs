namespace h;

internal class _6
{
	internal class _00065h
	{
		private string a5h;

		private string a5b;

		private string a56;

		internal string DisplayName => a5h;

		internal string DRMProductName => a5b;

		internal string FileName => "SunBurn-Deploy.xnb";

		internal _00065h(string P_0, string P_1, string P_2)
		{
			a5h = P_0;
			a5b = P_1;
			a56 = P_2;
		}
	}

	internal enum _00065b
	{
		SunBurn_Indie,
		SunBurn_Pro,
		SunBurn_Studio,
		SunBurn_ProNonCom
	}

	private const string a5h = "SunBurn-Deploy.xnb";

	internal static _00065h[] a5b = new _00065h[4]
	{
		new _00065h("SunBurn Indie", "SunBurn Pro", "SunBurn2-Indie.auth"),
		new _00065h("SunBurn Pro", "SunBurn Community", "SunBurn2-Pro.auth"),
		new _00065h("SunBurn Studio", "SunBurn Studio", "SunBurn2-Studio.auth"),
		new _00065h("SunBurn Pro Non-Commercial", "SunBurn ProNonCom", "SunBurn2-Pro-NonCommercial.auth")
	};

	private static string a56 = null;

	internal static string ActivationPath
	{
		get
		{
			return a56;
		}
		set
		{
			a56 = text;
			if (string.IsNullOrEmpty(a56))
			{
				a56 = "";
				return;
			}
			char c = a56[a56.Length - 1];
			if (c != '\\' && c != '/')
			{
				a56 += "\\";
			}
		}
	}
}
