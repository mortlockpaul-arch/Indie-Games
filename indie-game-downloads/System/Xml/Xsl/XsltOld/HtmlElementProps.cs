using System.Collections;

namespace System.Xml.Xsl.XsltOld;

internal sealed class HtmlElementProps
{
	private bool _empty;

	private bool _abrParent;

	private bool _uriParent;

	private bool _noEntities;

	private bool _blockWS;

	private bool _head;

	private bool _nameParent;

	private static readonly Hashtable s_table = CreatePropsTable();

	public bool Empty => _empty;

	public bool AbrParent => _abrParent;

	public bool UriParent => _uriParent;

	public bool NoEntities => _noEntities;

	public bool Head => _head;

	public bool NameParent => _nameParent;

	public static HtmlElementProps Create(bool empty, bool abrParent, bool uriParent, bool noEntities, bool blockWS, bool head, bool nameParent)
	{
		return new HtmlElementProps
		{
			_empty = empty,
			_abrParent = abrParent,
			_uriParent = uriParent,
			_noEntities = noEntities,
			_blockWS = blockWS,
			_head = head,
			_nameParent = nameParent
		};
	}

	public static HtmlElementProps GetProps(string name)
	{
		return (HtmlElementProps)s_table[name];
	}

	private static Hashtable CreatePropsTable()
	{
		bool flag = false;
		bool flag2 = true;
		return new Hashtable(71, StringComparer.OrdinalIgnoreCase)
		{
			{
				"a",
				Create(flag, flag, flag2, flag, flag, flag, flag2)
			},
			{
				"address",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"applet",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"area",
				Create(flag2, flag2, flag2, flag, flag2, flag, flag)
			},
			{
				"base",
				Create(flag2, flag, flag2, flag, flag2, flag, flag)
			},
			{
				"basefont",
				Create(flag2, flag, flag, flag, flag2, flag, flag)
			},
			{
				"blockquote",
				Create(flag, flag, flag2, flag, flag2, flag, flag)
			},
			{
				"body",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"br",
				Create(flag2, flag, flag, flag, flag, flag, flag)
			},
			{
				"button",
				Create(flag, flag2, flag, flag, flag, flag, flag)
			},
			{
				"caption",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"center",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"col",
				Create(flag2, flag, flag, flag, flag2, flag, flag)
			},
			{
				"colgroup",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"dd",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"del",
				Create(flag, flag, flag2, flag, flag2, flag, flag)
			},
			{
				"dir",
				Create(flag, flag2, flag, flag, flag2, flag, flag)
			},
			{
				"div",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"dl",
				Create(flag, flag2, flag, flag, flag2, flag, flag)
			},
			{
				"dt",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"fieldset",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"font",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"form",
				Create(flag, flag, flag2, flag, flag2, flag, flag)
			},
			{
				"frame",
				Create(flag2, flag2, flag, flag, flag2, flag, flag)
			},
			{
				"frameset",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"h1",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"h2",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"h3",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"h4",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"h5",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"h6",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"head",
				Create(flag, flag, flag2, flag, flag2, flag2, flag)
			},
			{
				"hr",
				Create(flag2, flag2, flag, flag, flag2, flag, flag)
			},
			{
				"html",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"iframe",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"img",
				Create(flag2, flag2, flag2, flag, flag, flag, flag)
			},
			{
				"input",
				Create(flag2, flag2, flag2, flag, flag, flag, flag)
			},
			{
				"ins",
				Create(flag, flag, flag2, flag, flag2, flag, flag)
			},
			{
				"isindex",
				Create(flag2, flag, flag, flag, flag2, flag, flag)
			},
			{
				"legend",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"li",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"link",
				Create(flag2, flag, flag2, flag, flag2, flag, flag)
			},
			{
				"map",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"menu",
				Create(flag, flag2, flag, flag, flag2, flag, flag)
			},
			{
				"meta",
				Create(flag2, flag, flag, flag, flag2, flag, flag)
			},
			{
				"noframes",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"noscript",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"object",
				Create(flag, flag2, flag2, flag, flag, flag, flag)
			},
			{
				"ol",
				Create(flag, flag2, flag, flag, flag2, flag, flag)
			},
			{
				"optgroup",
				Create(flag, flag2, flag, flag, flag2, flag, flag)
			},
			{
				"option",
				Create(flag, flag2, flag, flag, flag2, flag, flag)
			},
			{
				"p",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"param",
				Create(flag2, flag, flag, flag, flag2, flag, flag)
			},
			{
				"pre",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"q",
				Create(flag, flag, flag2, flag, flag, flag, flag)
			},
			{
				"s",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"script",
				Create(flag, flag2, flag2, flag2, flag, flag, flag)
			},
			{
				"select",
				Create(flag, flag2, flag, flag, flag, flag, flag)
			},
			{
				"strike",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"style",
				Create(flag, flag, flag, flag2, flag2, flag, flag)
			},
			{
				"table",
				Create(flag, flag, flag2, flag, flag2, flag, flag)
			},
			{
				"tbody",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"td",
				Create(flag, flag2, flag, flag, flag2, flag, flag)
			},
			{
				"textarea",
				Create(flag, flag2, flag, flag, flag, flag, flag)
			},
			{
				"tfoot",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"th",
				Create(flag, flag2, flag, flag, flag2, flag, flag)
			},
			{
				"thead",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"title",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"tr",
				Create(flag, flag, flag, flag, flag2, flag, flag)
			},
			{
				"ul",
				Create(flag, flag2, flag, flag, flag2, flag, flag)
			},
			{
				"xmp",
				Create(flag, flag, flag, flag, flag, flag, flag)
			}
		};
	}
}
