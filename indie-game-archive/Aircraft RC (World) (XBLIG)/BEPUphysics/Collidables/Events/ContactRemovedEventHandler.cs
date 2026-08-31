using BEPUphysics.CollisionTests;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Handles any special logic when a contact point between two bodies is removed.
/// </summary>
/// <param name="sender">Entry sending the event.</param>
/// <param name="other">Other entry within the pair opposing the monitored entry.</param>
/// <param name="pair">Pair presiding over the interaction of the two involved bodies and data about the removed contact.
/// This reference cannot be safely kept outside of the scope of the handler; pairs can quickly return to the resource pool.</param>
/// <param name="contact">Removed contact data.</param>
public delegate void ContactRemovedEventHandler<T>(T sender, Collidable other, CollidablePairHandler pair, ContactData contact);
