using System;

namespace SynapseGaming.LightingSystem.Serialization;

/// <summary>
/// Provides an inclusion based model for member serialization. Please note: if the class
/// implements IFullSerializable in its hierarchy chain this attribute is ignored and the
/// full serializable methods are used instead.
///
/// By default .Net uses an exclusion based serialization model, which serializes ALL members
/// and requires specifically excluding those that should not be serialized.
///
/// In contrast this attribute enables an inclusion based model to serialize members of the
/// class. This means by default NO members are serialized and members can be specifically
/// included by adding the [SerializeMember] attribute to them.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class SerializationInclusionModelAttribute : Attribute
{
}
