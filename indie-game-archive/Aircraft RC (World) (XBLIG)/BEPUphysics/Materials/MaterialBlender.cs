namespace BEPUphysics.Materials;

/// <summary>
/// A function which takes two materials and computes the interaction properties between them.
/// </summary>
/// <param name="a">First material to blend.</param>
/// <param name="b">Second material to blend.</param>
/// <param name="properties">Interaction properties between the two materials.</param>
public delegate void MaterialBlender(Material a, Material b, out InteractionProperties properties);
