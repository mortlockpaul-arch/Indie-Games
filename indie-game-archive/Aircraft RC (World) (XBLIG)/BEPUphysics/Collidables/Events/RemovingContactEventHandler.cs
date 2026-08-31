using BEPUphysics.CollisionTests;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Handles any special logic when a contact point between two bodies is removed.
/// Unlike ContactRemovedEventHandler, this will trigger at the time of contact removal instead of at the end of the space's update.
/// Performing operations outside of the scope of the controller is unsafe.
/// </summary>
/// <param name="sender">Entry sending the event.</param>
/// <param name="other">Other entry within the pair opposing the monitored entry.</param>
/// <param name="pair">Pair presiding over the interaction of the two involved bodies and data about the removed contact.
/// This reference cannot be safely kept outside of the scope of the handler; pairs can quickly return to the resource pool.</param>
/// <param name="contact">Contact between the two entries.  This reference cannot be safely kept outside of the scope of the handler;
/// it will be immediately returned to the resource pool after the event handler completes.</param>
public delegate void RemovingContactEventHandler<T>(T sender, Collidable other, CollidablePairHandler pair, Contact contact);
