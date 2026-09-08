namespace Microsoft.Xna.Framework.Graphics;

public sealed class EffectTechnique
{
	public string Name { get; private set; }

	public EffectPassCollection Passes { get; private set; }

	public EffectAnnotationCollection Annotations { get; private set; }

	internal nint TechniquePointer { get; private set; }

	internal EffectTechnique(string name, nint pointer, EffectPassCollection passes, EffectAnnotationCollection annotations)
	{
		Name = name;
		Passes = passes;
		Annotations = annotations;
		TechniquePointer = pointer;
	}
}
