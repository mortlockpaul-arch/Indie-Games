using BEPUphysics.CollisionTests;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Handles any special logic when two bodies are touching and generate a contact point.
/// Unlike ContactCreatedEventHandler, this will trigger at the time of contact generation instead of at the end of the space's update.
/// This allows the contact's data to be adjusted prior to usage in the velocity solver, 
/// but other actions such as altering the owning space's pair or entry listings are unsafe.
/// </summary>
/// <param name="sender">Entry sending the event.</param>
/// <param name="other">Other entry within the pair opposing the monitored entry.</param>
/// <param name="pair">Pair presiding over the interaction of the two involved bodies.
/// This reference cannot be safely kept outside of the scope of the handler; pairs can quickly return to the resource pool.</param>
/// <param name="contact">Newly generated contact point between the pair's two bodies.
/// This reference cannot be safely kept outside of the scope of the handler; contacts can quickly return to the resource pool.</param>
public delegate void CreatingContactEventHandler<T>(T sender, Collidable other, CollidablePairHandler pair, Contact contact);
