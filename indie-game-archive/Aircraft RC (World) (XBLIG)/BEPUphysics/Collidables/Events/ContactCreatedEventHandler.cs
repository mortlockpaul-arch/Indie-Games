using BEPUphysics.CollisionTests;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Handles any special logic when two bodies are touching and generate a contact point.
/// </summary>
/// <param name="sender">Entry sending the event.</param>
/// <param name="other">Other entry within the pair opposing the monitored entry.</param>
/// <param name="pair">Pair presiding over the interaction of the two involved bodies.
/// This reference cannot be safely kept outside of the scope of the handler; pairs can quickly return to the resource pool.</param>
/// <param name="contact">Created contact data.</param>
public delegate void ContactCreatedEventHandler<T>(T sender, Collidable other, CollidablePairHandler pair, ContactData contact);
