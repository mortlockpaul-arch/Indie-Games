using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public class CategoryAttribute : Attribute
{
	private volatile bool _localized;

	private readonly object _locker = new object();

	private string _categoryValue;

	[CompilerGenerated]
	private static CategoryAttribute _003CAction_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CAppearance_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CAsynchronous_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CBehavior_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CData_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CDefault_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CDesign_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CDragDrop_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CFocus_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CFormat_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CKey_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CLayout_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CMouse_003Ek__BackingField;

	[CompilerGenerated]
	private static CategoryAttribute _003CWindowStyle_003Ek__BackingField;

	public static CategoryAttribute Action => _003CAction_003Ek__BackingField ?? (_003CAction_003Ek__BackingField = new CategoryAttribute("Action"));

	public static CategoryAttribute Appearance => _003CAppearance_003Ek__BackingField ?? (_003CAppearance_003Ek__BackingField = new CategoryAttribute("Appearance"));

	public static CategoryAttribute Asynchronous => _003CAsynchronous_003Ek__BackingField ?? (_003CAsynchronous_003Ek__BackingField = new CategoryAttribute("Asynchronous"));

	public static CategoryAttribute Behavior => _003CBehavior_003Ek__BackingField ?? (_003CBehavior_003Ek__BackingField = new CategoryAttribute("Behavior"));

	public static CategoryAttribute Data => _003CData_003Ek__BackingField ?? (_003CData_003Ek__BackingField = new CategoryAttribute("Data"));

	public static CategoryAttribute Default => _003CDefault_003Ek__BackingField ?? (_003CDefault_003Ek__BackingField = new CategoryAttribute());

	public static CategoryAttribute Design => _003CDesign_003Ek__BackingField ?? (_003CDesign_003Ek__BackingField = new CategoryAttribute("Design"));

	public static CategoryAttribute DragDrop => _003CDragDrop_003Ek__BackingField ?? (_003CDragDrop_003Ek__BackingField = new CategoryAttribute("DragDrop"));

	public static CategoryAttribute Focus => _003CFocus_003Ek__BackingField ?? (_003CFocus_003Ek__BackingField = new CategoryAttribute("Focus"));

	public static CategoryAttribute Format => _003CFormat_003Ek__BackingField ?? (_003CFormat_003Ek__BackingField = new CategoryAttribute("Format"));

	public static CategoryAttribute Key => _003CKey_003Ek__BackingField ?? (_003CKey_003Ek__BackingField = new CategoryAttribute("Key"));

	public static CategoryAttribute Layout => _003CLayout_003Ek__BackingField ?? (_003CLayout_003Ek__BackingField = new CategoryAttribute("Layout"));

	public static CategoryAttribute Mouse => _003CMouse_003Ek__BackingField ?? (_003CMouse_003Ek__BackingField = new CategoryAttribute("Mouse"));

	public static CategoryAttribute WindowStyle => _003CWindowStyle_003Ek__BackingField ?? (_003CWindowStyle_003Ek__BackingField = new CategoryAttribute("WindowStyle"));

	public string Category
	{
		get
		{
			if (!_localized)
			{
				lock (_locker)
				{
					string localizedString = GetLocalizedString(_categoryValue);
					if (localizedString != null)
					{
						_categoryValue = localizedString;
					}
					_localized = true;
				}
			}
			return _categoryValue;
		}
	}

	public CategoryAttribute()
		: this("Default")
	{
	}

	public CategoryAttribute(string category)
	{
		_categoryValue = category;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is CategoryAttribute categoryAttribute)
		{
			return categoryAttribute.Category == Category;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Category?.GetHashCode() ?? 0;
	}

	protected virtual string? GetLocalizedString(string value)
	{
		return value switch
		{
			"Action" => System.SR.PropertyCategoryAction, 
			"Appearance" => System.SR.PropertyCategoryAppearance, 
			"Asynchronous" => System.SR.PropertyCategoryAsynchronous, 
			"Behavior" => System.SR.PropertyCategoryBehavior, 
			"Config" => System.SR.PropertyCategoryConfig, 
			"Data" => System.SR.PropertyCategoryData, 
			"DDE" => System.SR.PropertyCategoryDDE, 
			"Default" => System.SR.PropertyCategoryDefault, 
			"Design" => System.SR.PropertyCategoryDesign, 
			"DragDrop" => System.SR.PropertyCategoryDragDrop, 
			"Focus" => System.SR.PropertyCategoryFocus, 
			"Font" => System.SR.PropertyCategoryFont, 
			"Format" => System.SR.PropertyCategoryFormat, 
			"Key" => System.SR.PropertyCategoryKey, 
			"Layout" => System.SR.PropertyCategoryLayout, 
			"List" => System.SR.PropertyCategoryList, 
			"Mouse" => System.SR.PropertyCategoryMouse, 
			"Position" => System.SR.PropertyCategoryPosition, 
			"Scale" => System.SR.PropertyCategoryScale, 
			"Text" => System.SR.PropertyCategoryText, 
			"WindowStyle" => System.SR.PropertyCategoryWindowStyle, 
			_ => null, 
		};
	}

	public override bool IsDefaultAttribute()
	{
		return Category == Default.Category;
	}
}
