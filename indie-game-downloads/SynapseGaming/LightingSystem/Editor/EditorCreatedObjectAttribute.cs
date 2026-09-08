using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Attribute that indicates the class should appear in the SunBurn editor's list of creatable object types.
///
/// Note: for the type to appear it must implement the IEditorCreatedObject interface and have a default constructor.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EditorCreatedObjectAttribute : Attribute
{
}
