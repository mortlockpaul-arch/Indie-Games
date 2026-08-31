using System;

namespace SynapseGaming.LightingSystem.Serialization;

/// <summary>
/// Used to include a member in serialization when the inclusion based model is active on
/// the containing class (see [SerializationInclusionModel]).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
public class SerializeMemberAttribute : Attribute
{
}
