using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;

namespace System.ComponentModel.Design;

public class DesignerVerb : MenuCommand
{
	[GeneratedRegex("\\(\\&.\\)")]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.37416")]
	private static Regex ParameterReplacementRegex => _003CRegexGenerator_g_003EFAD5189FC365C646BC85F408728B63C8346E4C70E77F1DA0C508136C49906F750__ParameterReplacementRegex_0.Instance;

	public string Description
	{
		get
		{
			object obj = Properties["Description"];
			if (obj == null)
			{
				return string.Empty;
			}
			return (string)obj;
		}
		set
		{
			Properties["Description"] = value;
		}
	}

	public string Text
	{
		get
		{
			object obj = Properties["Text"];
			if (obj == null)
			{
				return string.Empty;
			}
			return (string)obj;
		}
	}

	public DesignerVerb(string text, EventHandler handler)
		: this(text, handler, StandardCommands.VerbFirst)
	{
	}

	public DesignerVerb(string text, EventHandler handler, CommandID startCommandID)
		: base(handler, startCommandID)
	{
		Properties["Text"] = ((text == null) ? null : ParameterReplacementRegex.Replace(text, ""));
	}

	public override string ToString()
	{
		return Text + " : " + base.ToString();
	}
}
