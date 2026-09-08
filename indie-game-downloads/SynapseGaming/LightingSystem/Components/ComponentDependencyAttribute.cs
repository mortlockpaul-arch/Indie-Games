using System;
using System.Runtime.CompilerServices;

namespace SynapseGaming.LightingSystem.Components;

/// <summary>
/// Attribute used to define a component's dependency on another component. This allows components
/// to automatically add their dependencies to an object.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class ComponentDependencyAttribute : Attribute
{
	[CompilerGenerated]
	private Type _3A_0018;

	/// <summary>
	/// Defines the component type the class is dependent on.
	/// </summary>
	public Type Dependency
	{
		[CompilerGenerated]
		get
		{
			return _3A_0018;
		}
		[CompilerGenerated]
		set
		{
			_3A_0018 = value;
		}
	}

	/// <summary>
	/// Creates a new ComponentDependencyAttribute instance.
	/// </summary>
	/// <param name="dependency">Component type the class is dependent on.</param>
	public ComponentDependencyAttribute(Type dependency)
	{
		Dependency = dependency;
	}
}
