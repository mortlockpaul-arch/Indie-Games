using System.Collections;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld;

internal sealed class Stylesheet
{
	private sealed class WhitespaceElement
	{
		private readonly int _key;

		private readonly double _priority;

		private bool _preserveSpace;

		internal double Priority => _priority;

		internal int Key => _key;

		internal bool PreserveSpace => _preserveSpace;

		internal WhitespaceElement(int Key, double priority, bool PreserveSpace)
		{
			_key = Key;
			_priority = priority;
			_preserveSpace = PreserveSpace;
		}

		internal void ReplaceValue(bool PreserveSpace)
		{
			_preserveSpace = PreserveSpace;
		}
	}

	private readonly ArrayList _imports = new ArrayList();

	private Hashtable _modeManagers;

	private readonly Hashtable _templateNameTable = new Hashtable();

	private Hashtable _attributeSetTable;

	private int _templateCount;

	private Hashtable _queryKeyTable;

	private ArrayList _whitespaceList;

	private bool _whitespace;

	private readonly Hashtable _scriptObjectTypes = new Hashtable();

	private TemplateManager _templates;

	internal bool Whitespace => _whitespace;

	internal ArrayList Imports => _imports;

	internal Hashtable AttributeSetTable => _attributeSetTable;

	internal Hashtable ScriptObjectTypes => _scriptObjectTypes;

	internal void AddSpace(Compiler compiler, string query, double Priority, bool PreserveSpace)
	{
		WhitespaceElement whitespaceElement;
		if (_queryKeyTable != null)
		{
			if (_queryKeyTable.Contains(query))
			{
				whitespaceElement = (WhitespaceElement)_queryKeyTable[query];
				whitespaceElement.ReplaceValue(PreserveSpace);
				return;
			}
		}
		else
		{
			_queryKeyTable = new Hashtable();
			_whitespaceList = new ArrayList();
		}
		whitespaceElement = new WhitespaceElement(compiler.AddQuery(query), Priority, PreserveSpace);
		_queryKeyTable[query] = whitespaceElement;
		_whitespaceList.Add(whitespaceElement);
	}

	internal void SortWhiteSpace()
	{
		if (_queryKeyTable != null)
		{
			for (int i = 0; i < _whitespaceList.Count; i++)
			{
				for (int num = _whitespaceList.Count - 1; num > i; num--)
				{
					WhitespaceElement whitespaceElement = (WhitespaceElement)_whitespaceList[num - 1];
					WhitespaceElement whitespaceElement2 = (WhitespaceElement)_whitespaceList[num];
					if (whitespaceElement2.Priority < whitespaceElement.Priority)
					{
						_whitespaceList[num - 1] = whitespaceElement2;
						_whitespaceList[num] = whitespaceElement;
					}
				}
			}
			_whitespace = true;
		}
		if (_imports == null)
		{
			return;
		}
		for (int num2 = _imports.Count - 1; num2 >= 0; num2--)
		{
			Stylesheet stylesheet = (Stylesheet)_imports[num2];
			if (stylesheet.Whitespace)
			{
				stylesheet.SortWhiteSpace();
				_whitespace = true;
			}
		}
	}

	internal bool PreserveWhiteSpace(Processor proc, XPathNavigator node)
	{
		if (_whitespaceList != null)
		{
			int num = _whitespaceList.Count - 1;
			while (0 <= num)
			{
				WhitespaceElement whitespaceElement = (WhitespaceElement)_whitespaceList[num];
				if (proc.Matches(node, whitespaceElement.Key))
				{
					return whitespaceElement.PreserveSpace;
				}
				num--;
			}
		}
		if (_imports != null)
		{
			for (int num2 = _imports.Count - 1; num2 >= 0; num2--)
			{
				if (!((Stylesheet)_imports[num2]).PreserveWhiteSpace(proc, node))
				{
					return false;
				}
			}
		}
		return true;
	}

	internal void AddAttributeSet(AttributeSetAction attributeSet)
	{
		if (_attributeSetTable == null)
		{
			_attributeSetTable = new Hashtable();
		}
		if (!_attributeSetTable.ContainsKey(attributeSet.Name))
		{
			_attributeSetTable[attributeSet.Name] = attributeSet;
		}
		else
		{
			((AttributeSetAction)_attributeSetTable[attributeSet.Name]).Merge(attributeSet);
		}
	}

	internal void AddTemplate(TemplateAction template)
	{
		XmlQualifiedName xmlQualifiedName = template.Mode;
		if (template.Name != null)
		{
			if (_templateNameTable.ContainsKey(template.Name))
			{
				throw XsltException.Create(System.SR.Xslt_DupTemplateName, template.Name.ToString());
			}
			_templateNameTable[template.Name] = template;
		}
		if (template.MatchKey == -1)
		{
			return;
		}
		if (_modeManagers == null)
		{
			_modeManagers = new Hashtable();
		}
		if ((object)xmlQualifiedName == null)
		{
			xmlQualifiedName = XmlQualifiedName.Empty;
		}
		TemplateManager templateManager = (TemplateManager)_modeManagers[xmlQualifiedName];
		if (templateManager == null)
		{
			templateManager = new TemplateManager(this, xmlQualifiedName);
			_modeManagers[xmlQualifiedName] = templateManager;
			if (xmlQualifiedName.IsEmpty)
			{
				_templates = templateManager;
			}
		}
		template.TemplateId = ++_templateCount;
		templateManager.AddTemplate(template);
	}

	internal void ProcessTemplates()
	{
		if (_modeManagers != null)
		{
			IDictionaryEnumerator enumerator = _modeManagers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				((TemplateManager)enumerator.Value).ProcessTemplates();
			}
		}
		if (_imports != null)
		{
			for (int num = _imports.Count - 1; num >= 0; num--)
			{
				((Stylesheet)_imports[num]).ProcessTemplates();
			}
		}
	}

	internal void ReplaceNamespaceAlias(Compiler compiler)
	{
		if (_modeManagers != null)
		{
			IDictionaryEnumerator enumerator = _modeManagers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TemplateManager templateManager = (TemplateManager)enumerator.Value;
				if (templateManager.templates != null)
				{
					for (int i = 0; i < templateManager.templates.Count; i++)
					{
						((TemplateAction)templateManager.templates[i]).ReplaceNamespaceAlias(compiler);
					}
				}
			}
		}
		if (_templateNameTable != null)
		{
			IDictionaryEnumerator enumerator2 = _templateNameTable.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				((TemplateAction)enumerator2.Value).ReplaceNamespaceAlias(compiler);
			}
		}
		if (_imports != null)
		{
			for (int num = _imports.Count - 1; num >= 0; num--)
			{
				((Stylesheet)_imports[num]).ReplaceNamespaceAlias(compiler);
			}
		}
	}

	internal TemplateAction FindTemplate(Processor processor, XPathNavigator navigator, XmlQualifiedName mode)
	{
		TemplateAction templateAction = null;
		if (_modeManagers != null)
		{
			TemplateManager templateManager = (TemplateManager)_modeManagers[mode];
			if (templateManager != null)
			{
				templateAction = templateManager.FindTemplate(processor, navigator);
			}
		}
		return templateAction ?? FindTemplateImports(processor, navigator, mode);
	}

	internal TemplateAction FindTemplateImports(Processor processor, XPathNavigator navigator, XmlQualifiedName mode)
	{
		TemplateAction templateAction = null;
		if (_imports != null)
		{
			for (int num = _imports.Count - 1; num >= 0; num--)
			{
				templateAction = ((Stylesheet)_imports[num]).FindTemplate(processor, navigator, mode);
				if (templateAction != null)
				{
					return templateAction;
				}
			}
		}
		return templateAction;
	}

	internal TemplateAction FindTemplate(Processor processor, XPathNavigator navigator)
	{
		TemplateAction templateAction = null;
		if (_templates != null)
		{
			templateAction = _templates.FindTemplate(processor, navigator);
		}
		if (templateAction == null)
		{
			templateAction = FindTemplateImports(processor, navigator);
		}
		return templateAction;
	}

	internal TemplateAction FindTemplate(XmlQualifiedName name)
	{
		TemplateAction templateAction = null;
		if (_templateNameTable != null)
		{
			templateAction = (TemplateAction)_templateNameTable[name];
		}
		if (templateAction == null && _imports != null)
		{
			for (int num = _imports.Count - 1; num >= 0; num--)
			{
				templateAction = ((Stylesheet)_imports[num]).FindTemplate(name);
				if (templateAction != null)
				{
					return templateAction;
				}
			}
		}
		return templateAction;
	}

	internal TemplateAction FindTemplateImports(Processor processor, XPathNavigator navigator)
	{
		TemplateAction templateAction = null;
		if (_imports != null)
		{
			for (int num = _imports.Count - 1; num >= 0; num--)
			{
				templateAction = ((Stylesheet)_imports[num]).FindTemplate(processor, navigator);
				if (templateAction != null)
				{
					return templateAction;
				}
			}
		}
		return templateAction;
	}
}
