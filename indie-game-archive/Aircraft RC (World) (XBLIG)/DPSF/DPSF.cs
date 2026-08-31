using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// The Base Particle System Framework Class.
/// This class contains the methods and properties needed to keep track of, update, and draw Particles
/// </summary>
/// <typeparam name="Particle">The Particle class used to hold a particle's information. The Particle class
/// specified must be or inherit from the DPSFParticle class</typeparam>
/// <typeparam name="Vertex">The Particle Vertex struct used to hold a vertex's information used for drawing</typeparam>
public class DPSF<Particle, Vertex> : IDPSFParticleSystem where Particle : DPSFParticle, new() where Vertex : struct, IDPSFParticleVertex
{
	/// <summary>
	/// The function prototype that Particle System Events must follow
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">How much time in seconds has elapsed since the last update</param>
	public delegate void UpdateParticleSystemDelegate(float fElapsedTimeInSeconds);

	/// <summary>
	/// The function prototype that the Particle Events must follow
	/// </summary>
	/// <param name="cParticle">The Particle to be updated</param>
	/// <param name="fElapsedTimeInSeconds">How much time in seconds has elapsed since the last update</param>
	public delegate void UpdateParticleDelegate(Particle cParticle, float fElapsedTimeInSeconds);

	/// <summary>
	/// The function prototype that the Particle Initialization Functions must follow
	/// </summary>
	/// <param name="cParticle">The Particle to be initialized</param>
	public delegate void InitializeParticleDelegate(Particle cParticle);

	/// <summary>
	/// The function prototype that the Vertex Update Functions must follow
	/// </summary>
	/// <param name="sParticleVertexBuffer">The vertex buffer array</param>
	/// <param name="iIndexInVertexBuffer">The index in the vertex buffer that the Particle properties should be written to</param>
	/// <param name="cParticle">The Particle whose properties should be copied to the vertex buffer</param>
	public delegate void UpdateVertexDelegate(ref Vertex[] sParticleVertexBuffer, int iIndexInVertexBuffer, Particle cParticle);

	/// <summary>
	/// Class to hold all of the Particle Events
	/// </summary>
	public class CParticleEvents
	{
		/// <summary>
		/// The Particle Event Types
		/// </summary>
		private enum EParticleEventTypes
		{
			EveryTime,
			OneTime,
			Timed,
			NormalizedTimed
		}

		/// <summary>
		/// Class to hold a Particle Event's information
		/// </summary>
		private class CParticleEvent
		{
			public UpdateParticleDelegate cFunctionToCall;

			public EParticleEventTypes eType;

			public int iExecutionOrder;

			public int iGroup;

			/// <summary>
			/// Explicit Constructor
			/// </summary>
			/// <param name="_cFunctionToCall">The Function the Event should call when it Fires</param>
			/// <param name="_eType">The Type of Event this is</param>
			/// <param name="_iExecutionOrder">The Order, relative to other Events, of when this Event should be Fired</param>
			/// <param name="_iGroup">The Group this Event should belong to</param>
			public CParticleEvent(UpdateParticleDelegate _cFunctionToCall, EParticleEventTypes _eType, int _iExecutionOrder, int _iGroup)
			{
				cFunctionToCall = _cFunctionToCall;
				eType = _eType;
				iExecutionOrder = _iExecutionOrder;
				iGroup = _iGroup;
			}

			/// <summary>
			/// Overload the == operator to test for value equality
			/// </summary>
			/// <returns>Returns true if the structures have the same values, false if not</returns>
			public static bool operator ==(CParticleEvent c1, CParticleEvent c2)
			{
				if (c1.cFunctionToCall == c2.cFunctionToCall && c1.iExecutionOrder == c2.iExecutionOrder && c1.eType == c2.eType && c1.iGroup == c2.iGroup)
				{
					return true;
				}
				return false;
			}

			/// <summary>
			/// Overload the != operator to test for value equality
			/// </summary>
			/// <returns>Returns true if the structures do not have the same values, false if they do</returns>
			public static bool operator !=(CParticleEvent c1, CParticleEvent c2)
			{
				return !(c1 == c2);
			}

			/// <summary>
			/// Override the Equals method
			/// </summary>
			public override bool Equals(object obj)
			{
				if (!(obj is CParticleEvent))
				{
					return false;
				}
				return this == (CParticleEvent)obj;
			}

			/// <summary>
			/// Override the GetHashCode method
			/// </summary>
			public override int GetHashCode()
			{
				return iExecutionOrder;
			}
		}

		/// <summary>
		/// Class to hold a Timed Particle Event's information
		/// </summary>
		private class CTimedParticleEvent : CParticleEvent
		{
			public float fTimeToFire;

			/// <summary>
			/// Explicit Constructor
			/// </summary>
			/// <param name="_cFunctionToCall">The Function the Event should call when it Fires</param>
			/// <param name="_eTimedType">The Type of Timed Event this is (Timed or NormalizedTimed)</param>
			/// <param name="_iExecutionOrder">The Order, relative to other Events, of when this Event should Fire</param>
			/// <param name="_iGroup">The Group this Event should belong to</param>
			/// <param name="_fTimeToFire">The Time at which this Event should Fire</param>
			public CTimedParticleEvent(UpdateParticleDelegate _cFunctionToCall, EParticleEventTypes _eTimedType, int _iExecutionOrder, int _iGroup, float _fTimeToFire)
				: base(_cFunctionToCall, _eTimedType, _iExecutionOrder, _iGroup)
			{
				fTimeToFire = _fTimeToFire;
			}

			/// <summary>
			/// Overload the == operator to test for value equality
			/// </summary>
			/// <returns>Returns true if the structures have the same values, false if not</returns>
			public static bool operator ==(CTimedParticleEvent c1, CTimedParticleEvent c2)
			{
				if (c1.cFunctionToCall == c2.cFunctionToCall && c1.iExecutionOrder == c2.iExecutionOrder && c1.eType == c2.eType && c1.iGroup == c2.iGroup && c1.fTimeToFire == c2.fTimeToFire)
				{
					return true;
				}
				return false;
			}

			/// <summary>
			/// Overload the != operator to test for value equality
			/// </summary>
			/// <returns>Returns true if the structures do not have the same values, false if they do</returns>
			public static bool operator !=(CTimedParticleEvent c1, CTimedParticleEvent c2)
			{
				return !(c1 == c2);
			}

			/// <summary>
			/// Override the Equals method
			/// </summary>
			public override bool Equals(object obj)
			{
				if (!(obj is CTimedParticleEvent))
				{
					return false;
				}
				return this == (CTimedParticleEvent)obj;
			}

			/// <summary>
			/// Override the GetHashCode method
			/// </summary>
			public override int GetHashCode()
			{
				return iExecutionOrder;
			}
		}

		private List<CParticleEvent> mcParticleEventList = new List<CParticleEvent>();

		private int EventSorter(CParticleEvent c1, CParticleEvent c2)
		{
			return c1.iExecutionOrder.CompareTo(c2.iExecutionOrder);
		}

		/// <summary>
		/// Adds a new EveryTime Event with a default Execution Order and Group of zero. 
		/// EveryTime Events fire every frame (i.e. every time the Update() function is called).
		/// </summary>
		/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
		public void AddEveryTimeEvent(UpdateParticleDelegate cFunctionToCall)
		{
			AddEveryTimeEvent(cFunctionToCall, 0, 0);
		}

		/// <summary>
		/// Adds a new EveryTime Event with a default Group of zero. 
		/// EveryTime Events fire every frame (i.e. every time the Update() function is called).
		/// </summary>
		/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute. 
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
		/// order they are added.</para></param>
		public void AddEveryTimeEvent(UpdateParticleDelegate cFunctionToCall, int iExecutionOrder)
		{
			AddEveryTimeEvent(cFunctionToCall, iExecutionOrder, 0);
		}

		/// <summary>
		/// Adds a new EveryTime Event. 
		/// EveryTime Events fire every frame (i.e. every time the Update() function is called).
		/// </summary>
		/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute.
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
		/// order they are added.</para></param>
		/// <param name="iGroup">The Group that this Event should belong to</param>
		public void AddEveryTimeEvent(UpdateParticleDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
		{
			mcParticleEventList.Add(new CParticleEvent(cFunctionToCall, EParticleEventTypes.EveryTime, iExecutionOrder, iGroup));
			mcParticleEventList.Sort(EventSorter);
		}

		/// <summary>
		/// Removes all EveryTime Events with the specified Function.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="cFunction">The Function of the EveryTime Event to remove</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveEveryTimeEvents(UpdateParticleDelegate cFunction)
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.EveryTime && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Removes all EveryTime Events in the specified Group.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="iGroup">The Group to remove the EveryTime Events from</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveEveryTimeEvents(int iGroup)
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.EveryTime && cEvent.iGroup == iGroup);
		}

		/// <summary>
		/// Removes an EveryTime Event with the specified Function, Execution Order, and Group.
		/// Returns true if the Event was found and removed, false if not.
		/// </summary>
		/// <param name="cFunction">The Function of the EveryTime Event to remove</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
		/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
		/// <returns>Returns true if the Event was found and removed, false if not.</returns>
		public bool RemoveEveryTimeEvent(UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleEventList.Remove(new CParticleEvent(cFunction, EParticleEventTypes.EveryTime, iExecutionOrder, iGroup));
		}

		/// <summary>
		/// Removes all EveryTime Events.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllEveryTimeEvents()
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.EveryTime);
		}

		/// <summary>
		/// Returns if there is an EveryTime Event with the specified Function or not.
		/// </summary>
		/// <param name="cFunction">The Function of the EveryTime Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
		public bool ContainsEveryTimeEvent(UpdateParticleDelegate cFunction)
		{
			return Extensions.Exists(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.EveryTime && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Returns if there is an EveryTime Event with the specifed Function, Execution Order, and Group or not.
		/// </summary>
		/// <param name="cFunction">The Function of the EveryTime Event to look for</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
		/// <param name="iGroup">The Group of the Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
		public bool ContainsEveryTimeEvent(UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleEventList.Contains(new CParticleEvent(cFunction, EParticleEventTypes.EveryTime, iExecutionOrder, iGroup));
		}

		/// <summary>
		/// Adds a new OneTime Event with a default Execution Order and Group of zero. 
		/// OneTime Events fire once then are automatically removed.
		/// </summary>
		/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
		public void AddOneTimeEvent(UpdateParticleDelegate cFunctionToCall)
		{
			AddOneTimeEvent(cFunctionToCall, 0, 0);
		}

		/// <summary>
		/// Adds a new OneTime Event with a default Group of zero. 
		/// OneTime Events fire once then are automatically removed.
		/// </summary>
		/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute.
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
		/// order they are added.</para></param>
		public void AddOneTimeEvent(UpdateParticleDelegate cFunctionToCall, int iExecutionOrder)
		{
			AddOneTimeEvent(cFunctionToCall, iExecutionOrder, 0);
		}

		/// <summary>
		/// Adds a new OneTime Event. 
		/// OneTime Events fire once then are automatically removed.
		/// </summary>
		/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute.
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
		/// order they are added.</para></param>
		/// <param name="iGroup">The Group that this Event should belong to</param>
		public void AddOneTimeEvent(UpdateParticleDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
		{
			mcParticleEventList.Add(new CParticleEvent(cFunctionToCall, EParticleEventTypes.OneTime, iExecutionOrder, iGroup));
			mcParticleEventList.Sort(EventSorter);
		}

		/// <summary>
		/// Removes all OneTime Events with the specified Function.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="cFunction">The Function of the OneTime Event to remove</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveOneTimeEvents(UpdateParticleDelegate cFunction)
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.OneTime && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Removes all OneTime Events in the specified Group.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="iGroup">The Group to remove the OneTime Events from</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveOneTimeEvents(int iGroup)
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.OneTime && cEvent.iGroup == iGroup);
		}

		/// <summary>
		/// Removes a OneTime Event with the specified Function To Call, Execution Order, and Group.
		/// Returns true if the Event was found and removed, false if not.
		/// </summary>
		/// <param name="cFunction">The Function of the OneTime Event to remove</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
		/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
		/// <returns>Returns true if the Event was found and removed, false if not.</returns>
		public bool RemoveOneTimeEvent(UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleEventList.Remove(new CParticleEvent(cFunction, EParticleEventTypes.OneTime, iExecutionOrder, iGroup));
		}

		/// <summary>
		/// Removes all OneTime Events.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllOneTimeEvents()
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.OneTime);
		}

		/// <summary>
		/// Returns if there is an OneTime Event with the specified Function or not.
		/// </summary>
		/// <param name="cFunction">The Function of the OneTime Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
		public bool ContainsOneTimeEvent(UpdateParticleDelegate cFunction)
		{
			return Extensions.Exists(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.OneTime && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Returns if there is an OneTime Event with the specifed Function, Execution Order, and Group or not.
		/// </summary>
		/// <param name="cFunction">The Function of the OneTime Event to look for</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
		/// <param name="iGroup">The Group of the Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
		public bool ContainsOneTimeEvent(UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleEventList.Contains(new CParticleEvent(cFunction, EParticleEventTypes.OneTime, iExecutionOrder, iGroup));
		}

		/// <summary>
		/// Adds a new Timed Event with a default Execution Order and Group of zero. 
		/// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
		/// </summary>
		/// <param name="fTimeToFire">The Time when the Event should fire
		/// (i.e. when the Function should be called)</param>
		/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
		public void AddTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunctionToCall)
		{
			AddTimedEvent(fTimeToFire, cFunctionToCall, 0, 0);
		}

		/// <summary>
		/// Adds a new Timed Event with a default Group of zero. 
		/// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
		/// </summary>
		/// <param name="fTimeToFire">The Time when the Event should fire
		/// (i.e. when the Function should be called)</param>
		/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute. 
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
		/// order they are added.</para></param>
		public void AddTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunctionToCall, int iExecutionOrder)
		{
			AddTimedEvent(fTimeToFire, cFunctionToCall, iExecutionOrder, 0);
		}

		/// <summary>
		/// Adds a new Timed Event. 
		/// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
		/// </summary>
		/// <param name="fTimeToFire">The Time when the Event should fire
		/// (i.e. when the Function should be called)</param>
		/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute.
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
		/// order they are added.</para></param>
		/// <param name="iGroup">The Group that this Event should belong to</param>
		public void AddTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
		{
			mcParticleEventList.Add(new CTimedParticleEvent(cFunctionToCall, EParticleEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
			mcParticleEventList.Sort(EventSorter);
		}

		/// <summary>
		/// Removes all Timed Events with the specified Function.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="cFunction">The Function that is called when the Event fires</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveTimedEvents(UpdateParticleDelegate cFunction)
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.Timed && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Removes all Timed Events in the specified Group.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="iGroup">The Group to remove the Timed Events from</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveTimedEvents(int iGroup)
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.Timed && cEvent.iGroup == iGroup);
		}

		/// <summary>
		/// Removes a Timed Event with the specified Function, Time To Fire, Execution Order, and Group.
		/// Returns true if the Event was found and removed, false if not.
		/// </summary>
		/// <param name="fTimeToFire">The Time the Event is scheduled to fire at</param>
		/// <param name="cFunction">The Function that is called when the Event fires</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
		/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
		/// <returns>Returns true if the Event was found and removed, false if not.</returns>
		public bool RemoveTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleEventList.Remove(new CTimedParticleEvent(cFunction, EParticleEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
		}

		/// <summary>
		/// Removes all Timed Events.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllTimedEvents()
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.Timed);
		}

		/// <summary>
		/// Returns if there is a Timed Event with the specified Function or not.
		/// </summary>
		/// <param name="cFunction">The Function of the Timed Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
		public bool ContainsTimedEvent(UpdateParticleDelegate cFunction)
		{
			return Extensions.Exists(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.Timed && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Returns if there is a Timed Event with the specifed Timed To Fire, Function, Execution Order, and Group or not.
		/// </summary>
		/// <param name="fTimeToFire">The Time the Event is scheduled to fire at</param>
		/// <param name="cFunction">The Function of the Timed Event to look for</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
		/// <param name="iGroup">The Group of the Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
		public bool ContainsTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleEventList.Contains(new CTimedParticleEvent(cFunction, EParticleEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
		}

		/// <summary>
		/// Adds a new Normalized Timed Event with a default Execution Order and Group of zero. 
		/// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
		/// </summary>
		/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire. 
		/// <para>NOTE: This is clamped to the range of 0.0 - 1.0.</para></param>
		/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
		public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunctionToCall)
		{
			AddNormalizedTimedEvent(fNormalizedTimeToFire, cFunctionToCall, 0, 0);
		}

		/// <summary>
		/// Adds a new Normalized Timed Event with a default Group of zero. 
		/// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
		/// </summary>
		/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire. 
		/// <para>NOTE: This is clamped to the range of 0.0 - 1.0.</para></param>
		/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute. NOTE: Events with lower Execution Order are executed first. NOTE: Events
		/// with the same Execution Order are not guaranteed to be executed in the order they are added.</param>
		public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunctionToCall, int iExecutionOrder)
		{
			AddNormalizedTimedEvent(fNormalizedTimeToFire, cFunctionToCall, iExecutionOrder, 0);
		}

		/// <summary>
		/// Adds a new Normalized Timed Event. 
		/// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
		/// </summary>
		/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire
		/// (compared against the Particle's Normalized Elapsed Time). NOTE: This is clamped to the range of 0.0 - 1.0</param>
		/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute.
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
		/// order they are added.</para></param>
		/// <param name="iGroup">The Group that this Event should belong to</param>
		public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
		{
			fNormalizedTimeToFire = MathHelper.Clamp(fNormalizedTimeToFire, 0f, 1f);
			mcParticleEventList.Add(new CTimedParticleEvent(cFunctionToCall, EParticleEventTypes.NormalizedTimed, iExecutionOrder, iGroup, fNormalizedTimeToFire));
			mcParticleEventList.Sort(EventSorter);
		}

		/// <summary>
		/// Removes all Normalized Timed Events with the specified Function.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="cFunction">The Function that is called when the Event fires</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveNormalizedTimedEvents(UpdateParticleDelegate cFunction)
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.NormalizedTimed && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Removes all Normalized Timed Events in the specified Group.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="iGroup">The Group to remove the Normalized Timed Events from</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveNormalizedTimedEvents(int iGroup)
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.NormalizedTimed && cEvent.iGroup == iGroup);
		}

		/// <summary>
		/// Removes a Normalized Timed Event with the specified Function, Time To Fire, Execution Order, and Group.
		/// Returns true if the Event was found and removed, false if not.
		/// </summary>
		/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) the Event is scheduled to fire at</param>
		/// <param name="cFunction">The Function that is called when the Event fires</param>
		/// <param name="iExectionOrder">The Execution Order of the Event to remove. Default is zero.</param>
		/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
		/// <returns>Returns true if the Event was found and removed, false if not.</returns>
		public bool RemoveNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunction, int iExectionOrder, int iGroup)
		{
			return mcParticleEventList.Remove(new CTimedParticleEvent(cFunction, EParticleEventTypes.NormalizedTimed, iExectionOrder, iGroup, fNormalizedTimeToFire));
		}

		/// <summary>
		/// Removes all Normalized Timed Events.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllNormalizedTimedEvents()
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.NormalizedTimed);
		}

		/// <summary>
		/// Returns if there is a NormalizedTimed Event with the specified Function or not.
		/// </summary>
		/// <param name="cFunction">The Function of the NormalizedTimed Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
		public bool ContainsNormalizedTimedEvent(UpdateParticleDelegate cFunction)
		{
			return Extensions.Exists(mcParticleEventList, (CParticleEvent cEvent) => cEvent.eType == EParticleEventTypes.NormalizedTimed && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Returns if there is a NormalizedTimed Event with the specifed Normalized Time To Fire, Function, Execution Order, and Group or not.
		/// </summary>
		/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) the Event is scheduled to fire at</param>
		/// <param name="cFunction">The Function of the NormalizedTimed Event to look for</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
		/// <param name="iGroup">The Group of the Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
		public bool ContainsNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleEventList.Contains(new CTimedParticleEvent(cFunction, EParticleEventTypes.NormalizedTimed, iExecutionOrder, iGroup, fNormalizedTimeToFire));
		}

		/// <summary>
		/// Removes all Timed Events and Normalized Timed Events.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllTimedAndNormalizedTimedEvents()
		{
			return RemoveAllTimedEvents() + RemoveAllNormalizedTimedEvents();
		}

		/// <summary>
		/// Removes all Events.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllEvents()
		{
			int count = mcParticleEventList.Count;
			mcParticleEventList.Clear();
			return count;
		}

		/// <summary>
		/// Removes all Events in the specified Group.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="iGroup">The Group to remove all Events from</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllEventsInGroup(int iGroup)
		{
			return Extensions.RemoveAll(mcParticleEventList, (CParticleEvent cEvent) => cEvent.iGroup == iGroup);
		}

		/// <summary>
		/// Updates the given Particle according to the Particle Events. This is called automatically
		/// every frame by the Particle System.
		/// </summary>
		/// <param name="cParticle">The Particle to update</param>
		/// <param name="fElapsedTimeInSeconds">The amount of Time Elapsed since the last Update</param>
		public void Update(Particle cParticle, float fElapsedTimeInSeconds)
		{
			if (mcParticleEventList.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < mcParticleEventList.Count; i++)
			{
				if (mcParticleEventList[i].eType == EParticleEventTypes.EveryTime || mcParticleEventList[i].eType == EParticleEventTypes.OneTime)
				{
					mcParticleEventList[i].cFunctionToCall(cParticle, fElapsedTimeInSeconds);
					continue;
				}
				CTimedParticleEvent cTimedParticleEvent = (CTimedParticleEvent)mcParticleEventList[i];
				if (cTimedParticleEvent.eType == EParticleEventTypes.Timed)
				{
					float num = ((cParticle.LastElapsedTime == 0f) ? (-0.1f) : cParticle.LastElapsedTime);
					if (num < cTimedParticleEvent.fTimeToFire && cParticle.ElapsedTime >= cTimedParticleEvent.fTimeToFire)
					{
						cTimedParticleEvent.cFunctionToCall(cParticle, fElapsedTimeInSeconds);
					}
				}
				else
				{
					float num2 = ((cParticle.LastNormalizedElapsedTime == 0f) ? (-0.1f) : cParticle.LastNormalizedElapsedTime);
					if (num2 < cTimedParticleEvent.fTimeToFire && cParticle.NormalizedElapsedTime >= cTimedParticleEvent.fTimeToFire)
					{
						cTimedParticleEvent.cFunctionToCall(cParticle, fElapsedTimeInSeconds);
					}
				}
			}
		}
	}

	/// <summary>
	/// Class to hold all of the Particle System Events and related info
	/// </summary>
	public class CParticleSystemEvents
	{
		/// <summary>
		/// The Particle System Event Types
		/// </summary>
		private enum EParticleSystemEventTypes
		{
			EveryTime,
			OneTime,
			Timed,
			NormalizedTimed
		}

		/// <summary>
		/// Class to hold a Particle System Event's information
		/// </summary>
		private class CParticleSystemEvent
		{
			public UpdateParticleSystemDelegate cFunctionToCall;

			public EParticleSystemEventTypes eType;

			public int iExecutionOrder;

			public int iGroup;

			/// <summary>
			/// Explicit Constructor
			/// </summary>
			/// <param name="_cFunctionToCall">The Function the Event should call when it Fires</param>
			/// <param name="_eType">The Type of Event this is</param>
			/// <param name="_iExecutionOrder">The Order, relative to other Events, of when this Event should be Fired</param>
			/// <param name="_iGroup">The Group this Event should belong to</param>
			public CParticleSystemEvent(UpdateParticleSystemDelegate _cFunctionToCall, EParticleSystemEventTypes _eType, int _iExecutionOrder, int _iGroup)
			{
				cFunctionToCall = _cFunctionToCall;
				eType = _eType;
				iExecutionOrder = _iExecutionOrder;
				iGroup = _iGroup;
			}

			/// <summary>
			/// Overload the == operator to test for value equality
			/// </summary>
			/// <returns>Returns true if the structures have the same values, false if not</returns>
			public static bool operator ==(CParticleSystemEvent c1, CParticleSystemEvent c2)
			{
				if (c1.cFunctionToCall == c2.cFunctionToCall && c1.iExecutionOrder == c2.iExecutionOrder && c1.eType == c2.eType && c1.iGroup == c2.iGroup)
				{
					return true;
				}
				return false;
			}

			/// <summary>
			/// Overload the != operator to test for value equality
			/// </summary>
			/// <returns>Returns true if the structures do not have the same values, false if they do</returns>
			public static bool operator !=(CParticleSystemEvent c1, CParticleSystemEvent c2)
			{
				return !(c1 == c2);
			}

			/// <summary>
			/// Override the Equals method
			/// </summary>
			public override bool Equals(object obj)
			{
				if (!(obj is CParticleSystemEvent))
				{
					return false;
				}
				return this == (CParticleSystemEvent)obj;
			}

			/// <summary>
			/// Override the GetHashCode method
			/// </summary>
			public override int GetHashCode()
			{
				return iExecutionOrder;
			}
		}

		/// <summary>
		/// Class to hold a Timed Particle System Event's information
		/// </summary>
		private class CTimedParticleSystemEvent : CParticleSystemEvent
		{
			public float fTimeToFire;

			/// <summary>
			/// Explicit Constructor
			/// </summary>
			/// <param name="_cFunctionToCall">The Function the Event should call when it Fires</param>
			/// <param name="_eTimedType">The Type of Timed Event this is (Timed or NormalizedTimed)</param>
			/// <param name="_iExecutionOrder">The Order, relative to other Events, of when this Event should Fire</param>
			/// <param name="_iGroup">The Group this Event should belong to</param>
			/// <param name="_fTimeToFire">The Time at which this Event should Fire</param>
			public CTimedParticleSystemEvent(UpdateParticleSystemDelegate _cFunctionToCall, EParticleSystemEventTypes _eTimedType, int _iExecutionOrder, int _iGroup, float _fTimeToFire)
				: base(_cFunctionToCall, _eTimedType, _iExecutionOrder, _iGroup)
			{
				fTimeToFire = _fTimeToFire;
			}

			/// <summary>
			/// Overload the == operator to test for value equality
			/// </summary>
			/// <returns>Returns true if the structures have the same values, false if not</returns>
			public static bool operator ==(CTimedParticleSystemEvent c1, CTimedParticleSystemEvent c2)
			{
				if (c1.cFunctionToCall == c2.cFunctionToCall && c1.iExecutionOrder == c2.iExecutionOrder && c1.eType == c2.eType && c1.iGroup == c2.iGroup && c1.fTimeToFire == c2.fTimeToFire)
				{
					return true;
				}
				return false;
			}

			/// <summary>
			/// Overload the != operator to test for value equality
			/// </summary>
			/// <returns>Returns true if the structures do not have the same values, false if they do</returns>
			public static bool operator !=(CTimedParticleSystemEvent c1, CTimedParticleSystemEvent c2)
			{
				return !(c1 == c2);
			}

			/// <summary>
			/// Override the Equals method
			/// </summary>
			public override bool Equals(object obj)
			{
				if (!(obj is CTimedParticleSystemEvent))
				{
					return false;
				}
				return this == (CTimedParticleSystemEvent)obj;
			}

			/// <summary>
			/// Override the GetHashCode method
			/// </summary>
			public override int GetHashCode()
			{
				return iExecutionOrder;
			}
		}

		/// <summary>
		/// The Options of what should happen when the Particle System reaches the end of its Lifetime
		/// </summary>
		public enum EParticleSystemEndOfLifeOptions
		{
			/// <summary>
			/// When the Particle System reaches the end of its Lifetime nothing special happens; It just
			/// continues to operate as normal.
			/// </summary>
			Nothing,
			/// <summary>
			/// When the Particle System reaches the end of its Lifetime its Elapsed Time is reset to zero,
			/// so that all of the Timed Events will be repeated again.
			/// </summary>
			Repeat,
			/// <summary>
			/// When the Particle System reaches the end of its Lifetime it calls its Destroy() function, so
			/// the Particle System releases all its resources and is no longer updated or drawn.
			/// </summary>
			Destroy
		}

		/// <summary>
		/// Class to hold the Lifetime information of the Particle System
		/// </summary>
		public class CParticleSystemLifetimeData : DPSFParticle
		{
			private EParticleSystemEndOfLifeOptions meEndOfLifeOption;

			/// <summary>
			/// Get / Set what should happen when the Particle System reaches the end of its Lifetime
			/// </summary>
			public EParticleSystemEndOfLifeOptions EndOfLifeOption
			{
				get
				{
					return meEndOfLifeOption;
				}
				set
				{
					meEndOfLifeOption = value;
				}
			}

			/// <summary>
			/// Resets the class variables to their default values
			/// </summary>
			public override void Reset()
			{
				base.Reset();
				EndOfLifeOption = EParticleSystemEndOfLifeOptions.Nothing;
			}

			/// <summary>
			/// Deep copy the ParticleToCopy's values into this Particle
			/// </summary>
			/// <param name="ParticleToCopy">The Particle whose values should be Copied</param>
			public override void CopyFrom(DPSFParticle ParticleToCopy)
			{
				CParticleSystemLifetimeData cParticleSystemLifetimeData = (CParticleSystemLifetimeData)ParticleToCopy;
				base.CopyFrom(cParticleSystemLifetimeData);
				EndOfLifeOption = cParticleSystemLifetimeData.EndOfLifeOption;
			}
		}

		private List<CParticleSystemEvent> mcParticleSystemEventList = new List<CParticleSystemEvent>();

		private CParticleSystemLifetimeData mcParticleSystemLifetimeData = new CParticleSystemLifetimeData();

		/// <summary>
		/// Get / Set the Lifetime information of the Particle System
		/// </summary>
		public CParticleSystemLifetimeData LifetimeData
		{
			get
			{
				return mcParticleSystemLifetimeData;
			}
			set
			{
				mcParticleSystemLifetimeData = value;
			}
		}

		private int EventSorter(CParticleSystemEvent c1, CParticleSystemEvent c2)
		{
			return c1.iExecutionOrder.CompareTo(c2.iExecutionOrder);
		}

		/// <summary>
		/// Adds a new EveryTime Event with a default Execution Order and Group of zero. 
		/// EveryTime Events fire every frame (i.e. every time the Update() function is called).
		/// </summary>
		/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
		public void AddEveryTimeEvent(UpdateParticleSystemDelegate cFunctionToCall)
		{
			AddEveryTimeEvent(cFunctionToCall, 0, 0);
		}

		/// <summary>
		/// Adds a new EveryTime Event with a default Group of zero. 
		/// EveryTime Events fire every frame (i.e. every time the Update() function is called).
		/// </summary>
		/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute.
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
		/// the order they are added.</para></param>
		public void AddEveryTimeEvent(UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder)
		{
			AddEveryTimeEvent(cFunctionToCall, iExecutionOrder, 0);
		}

		/// <summary>
		/// Adds a new EveryTime Event. 
		/// EveryTime Events fire every frame (i.e. every time the Update() function is called).
		/// </summary>
		/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute.
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
		/// the order they are added.</para></param>
		/// <param name="iGroup">The Group that this Event should belong to</param>
		public void AddEveryTimeEvent(UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
		{
			mcParticleSystemEventList.Add(new CParticleSystemEvent(cFunctionToCall, EParticleSystemEventTypes.EveryTime, iExecutionOrder, iGroup));
			mcParticleSystemEventList.Sort(EventSorter);
		}

		/// <summary>
		/// Removes all EveryTime Events with the specified Function.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="cFunction">The Function of the EveryTime Event to remove</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveEveryTimeEvents(UpdateParticleSystemDelegate cFunction)
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.EveryTime && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Removes all EveryTime Events in the specified Group.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="iGroup">The Group to remove the EveryTime Events from</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveEveryTimeEvents(int iGroup)
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.EveryTime && cEvent.iGroup == iGroup);
		}

		/// <summary>
		/// Removes an EveryTime Event with the specified Function, Execution Order, and Group.
		/// Returns true if the Event was found and removed, false if not.
		/// </summary>
		/// <param name="cFunction">The Function of the EveryTime Event to remove</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
		/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
		/// <returns>Returns true if the Event was found and removed, false if not.</returns>
		public bool RemoveEveryTimeEvent(UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleSystemEventList.Remove(new CParticleSystemEvent(cFunction, EParticleSystemEventTypes.EveryTime, iExecutionOrder, iGroup));
		}

		/// <summary>
		/// Removes all EveryTime Events.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllEveryTimeEvents()
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.EveryTime);
		}

		/// <summary>
		/// Returns if there is an EveryTime Event with the specified Function or not.
		/// </summary>
		/// <param name="cFunction">The Function of the EveryTime Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
		public bool ContainsEveryTimeEvent(UpdateParticleSystemDelegate cFunction)
		{
			return Extensions.Exists(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.EveryTime && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Returns if there is an EveryTime Event with the specifed Function, Execution Order, and Group or not.
		/// </summary>
		/// <param name="cFunction">The Function of the EveryTime Event to look for</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
		/// <param name="iGroup">The Group of the Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
		public bool ContainsEveryTimeEvent(UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleSystemEventList.Contains(new CParticleSystemEvent(cFunction, EParticleSystemEventTypes.EveryTime, iExecutionOrder, iGroup));
		}

		/// <summary>
		/// Adds a new OneTime Event with a default Execution Order and Group of zero. 
		/// OneTime Events fire once then are automatically removed.
		/// </summary>
		/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
		public void AddOneTimeEvent(UpdateParticleSystemDelegate cFunctionToCall)
		{
			AddOneTimeEvent(cFunctionToCall, 0, 0);
		}

		/// <summary>
		/// Adds a new OneTime Event with a default Group of zero. 
		/// OneTime Events fire once then are automatically removed.
		/// </summary>
		/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute.
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
		/// the order they are added.</para></param>
		public void AddOneTimeEvent(UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder)
		{
			AddOneTimeEvent(cFunctionToCall, iExecutionOrder, 0);
		}

		/// <summary>
		/// Adds a new OneTime Event. 
		/// OneTime Events fire once then are automatically removed.
		/// </summary>
		/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute.
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
		/// the order they are added.</para></param>
		/// <param name="iGroup">The Group that this Event should belong to</param>
		public void AddOneTimeEvent(UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
		{
			mcParticleSystemEventList.Add(new CParticleSystemEvent(cFunctionToCall, EParticleSystemEventTypes.OneTime, iExecutionOrder, iGroup));
			mcParticleSystemEventList.Sort(EventSorter);
		}

		/// <summary>
		/// Removes all OneTime Events with the specified Function.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="cFunction">The Function of the OneTime Event to remove</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveOneTimeEvents(UpdateParticleSystemDelegate cFunction)
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.OneTime && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Removes all OneTime Events in the specified Group.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="iGroup">The Group to remove the OneTime Events from</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveOneTimeEvents(int iGroup)
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.OneTime && cEvent.iGroup == iGroup);
		}

		/// <summary>
		/// Removes a OneTime Event with the specified Function To Call, Execution Order, and Group.
		/// Returns true if the Event was found and removed, false if not.
		/// </summary>
		/// <param name="cFunction">The Function of the OneTime Event to remove</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
		/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
		/// <returns>Returns true if the Event was found and removed, false if not.</returns>
		public bool RemoveOneTimeEvent(UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleSystemEventList.Remove(new CParticleSystemEvent(cFunction, EParticleSystemEventTypes.OneTime, iExecutionOrder, iGroup));
		}

		/// <summary>
		/// Removes all OneTime Events.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllOneTimeEvents()
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.OneTime);
		}

		/// <summary>
		/// Returns if there is an OneTime Event with the specified Function or not.
		/// </summary>
		/// <param name="cFunction">The Function of the OneTime Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
		public bool ContainsOneTimeEvent(UpdateParticleSystemDelegate cFunction)
		{
			return Extensions.Exists(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.OneTime && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Returns if there is an OneTime Event with the specifed Function, Execution Order, and Group or not.
		/// </summary>
		/// <param name="cFunction">The Function of the OneTime Event to look for</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
		/// <param name="iGroup">The Group of the Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
		public bool ContainsOneTimeEvent(UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleSystemEventList.Contains(new CParticleSystemEvent(cFunction, EParticleSystemEventTypes.OneTime, iExecutionOrder, iGroup));
		}

		/// <summary>
		/// Adds a new Timed Event with a default Execution Order and Group of zero. 
		/// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
		/// </summary>
		/// <param name="fTimeToFire">The Time when the Event should fire
		/// (i.e. when the Function should be called)</param>
		/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
		public void AddTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunctionToCall)
		{
			AddTimedEvent(fTimeToFire, cFunctionToCall, 0, 0);
		}

		/// <summary>
		/// Adds a new Timed Event with a default Group of zero. 
		/// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
		/// </summary>
		/// <param name="fTimeToFire">The Time when the Event should fire
		/// (i.e. when the Function should be called)</param>
		/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute.
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
		/// the order they are added.</para></param>
		public void AddTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder)
		{
			AddTimedEvent(fTimeToFire, cFunctionToCall, iExecutionOrder, 0);
		}

		/// <summary>
		/// Adds a new Timed Event. 
		/// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
		/// </summary>
		/// <param name="fTimeToFire">The Time when the Event should fire
		/// (i.e. when the Function should be called)</param>
		/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute.
		/// <para>NOTE: Events with lower Execution Order are executed first.</para>
		/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
		/// the order they are added.</para></param>
		/// <param name="iGroup">The Group that this Event should belong to</param>
		public void AddTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
		{
			mcParticleSystemEventList.Add(new CTimedParticleSystemEvent(cFunctionToCall, EParticleSystemEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
			mcParticleSystemEventList.Sort(EventSorter);
		}

		/// <summary>
		/// Removes all Timed Events with the specified Function.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="cFunction">The Function that is called when the Event fires</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveTimedEvents(UpdateParticleSystemDelegate cFunction)
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.Timed && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Removes all Timed Events in the specified Group.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="iGroup">The Group to remove the Timed Events from</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveTimedEvents(int iGroup)
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.Timed && cEvent.iGroup == iGroup);
		}

		/// <summary>
		/// Removes a Timed Event with the specified Function, Time To Fire, Execution Order, and Group.
		/// Returns true if the Event was found and removed, false if not.
		/// </summary>
		/// <param name="fTimeToFire">The Time the Event is scheduled to fire at</param>
		/// <param name="cFunction">The Function that is called when the Event fires</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
		/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
		/// <returns>Returns true if the Event was found and removed, false if not.</returns>
		public bool RemoveTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleSystemEventList.Remove(new CTimedParticleSystemEvent(cFunction, EParticleSystemEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
		}

		/// <summary>
		/// Removes all Timed Events.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllTimedEvents()
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.Timed);
		}

		/// <summary>
		/// Returns if there is a Timed Event with the specified Function or not.
		/// </summary>
		/// <param name="cFunction">The Function of the Timed Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
		public bool ContainsTimedEvent(UpdateParticleSystemDelegate cFunction)
		{
			return Extensions.Exists(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.Timed && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Returns if there is a Timed Event with the specifed Timed To Fire, Function, Execution Order, and Group or not.
		/// </summary>
		/// <param name="fTimeToFire">The Time the Event is scheduled to fire at</param>
		/// <param name="cFunction">The Function of the Timed Event to look for</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
		/// <param name="iGroup">The Group of the Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
		public bool ContainsTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleSystemEventList.Contains(new CTimedParticleSystemEvent(cFunction, EParticleSystemEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
		}

		/// <summary>
		/// Adds a new Normalized Timed Event with a default Execution Order and Group of zero. 
		/// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
		/// </summary>
		/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire. 
		/// <para>NOTE: This is clamped to the range of 0.0 - 1.0.</para></param>
		/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
		public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunctionToCall)
		{
			AddNormalizedTimedEvent(fNormalizedTimeToFire, cFunctionToCall, 0, 0);
		}

		/// <summary>
		/// Adds a new Normalized Timed Event with a default Group of zero. 
		/// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
		/// </summary>
		/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire. 
		/// <para>NOTE: This is clamped to the range of 0.0 - 1.0.</para></param>
		/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
		/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
		/// should Execute. NOTE: Events with lower Execution Order are executed first. NOTE: Events
		/// with the same Execution Order are not guaranteed to be executed in the order they are added.</param>
		public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder)
		{
			AddNormalizedTimedEvent(fNormalizedTimeToFire, cFunctionToCall, iExecutionOrder, 0);
		}

		public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
		{
			fNormalizedTimeToFire = MathHelper.Clamp(fNormalizedTimeToFire, 0f, 1f);
			mcParticleSystemEventList.Add(new CTimedParticleSystemEvent(cFunctionToCall, EParticleSystemEventTypes.NormalizedTimed, iExecutionOrder, iGroup, fNormalizedTimeToFire));
			mcParticleSystemEventList.Sort(EventSorter);
		}

		/// <summary>
		/// Removes all Normalized Timed Events with the specified Function.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="cFunction">The Function that is called when the Event fires</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveNormalizedTimedEvents(UpdateParticleSystemDelegate cFunction)
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.NormalizedTimed && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Removes all Normalized Timed Events in the specified Group.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="iGroup">The Group to remove the Normalized Timed Events from</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveNormalizedTimedEvents(int iGroup)
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.NormalizedTimed && cEvent.iGroup == iGroup);
		}

		/// <summary>
		/// Removes a Normalized Timed Event with the specified Function, Time To Fire, Execution Order, and Group.
		/// Returns true if the Event was found and removed, false if not.
		/// </summary>
		/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) the Event is scheduled to fire at</param>
		/// <param name="cFunction">The Function that is called when the Event fires</param>
		/// <param name="iExectionOrder">The Execution Order of the Event to remove. Default is zero.</param>
		/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
		/// <returns>Returns true if the Event was found and removed, false if not.</returns>
		public bool RemoveNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunction, int iExectionOrder, int iGroup)
		{
			return mcParticleSystemEventList.Remove(new CTimedParticleSystemEvent(cFunction, EParticleSystemEventTypes.NormalizedTimed, iExectionOrder, iGroup, fNormalizedTimeToFire));
		}

		/// <summary>
		/// Removes all Normalized Timed Events.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllNormalizedTimedEvents()
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.NormalizedTimed);
		}

		/// <summary>
		/// Returns if there is a NormalizedTimed Event with the specified Function or not.
		/// </summary>
		/// <param name="cFunction">The Function of the NormalizedTimed Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
		public bool ContainsNormalizedTimedEvent(UpdateParticleSystemDelegate cFunction)
		{
			return Extensions.Exists(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.eType == EParticleSystemEventTypes.NormalizedTimed && cEvent.cFunctionToCall == cFunction);
		}

		/// <summary>
		/// Returns if there is a NormalizedTimed Event with the specifed Normalized Time To Fire, Function, Execution Order, and Group or not.
		/// </summary>
		/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) the Event is scheduled to fire at</param>
		/// <param name="cFunction">The Function of the NormalizedTimed Event to look for</param>
		/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
		/// <param name="iGroup">The Group of the Event to look for</param>
		/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
		public bool ContainsNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
		{
			return mcParticleSystemEventList.Contains(new CTimedParticleSystemEvent(cFunction, EParticleSystemEventTypes.NormalizedTimed, iExecutionOrder, iGroup, fNormalizedTimeToFire));
		}

		/// <summary>
		/// Removes all Timed Events and Normalized Timed Events.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllTimedAndNormalizedTimedEvents()
		{
			return RemoveAllTimedEvents() + RemoveAllNormalizedTimedEvents();
		}

		/// <summary>
		/// Removes all Events.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllEvents()
		{
			int count = mcParticleSystemEventList.Count;
			mcParticleSystemEventList.Clear();
			return count;
		}

		/// <summary>
		/// Removes all Events in the specified Group.
		/// Returns the number of Events that were removed.
		/// </summary>
		/// <param name="iGroup">The Group to remove all Events from</param>
		/// <returns>Returns the number of Events that were removed.</returns>
		public int RemoveAllEventsInGroup(int iGroup)
		{
			return Extensions.RemoveAll(mcParticleSystemEventList, (CParticleSystemEvent cEvent) => cEvent.iGroup == iGroup);
		}

		/// <summary>
		/// Updates the Particle System according to the Particle System Events. This is done automatically
		/// by the Particle System every frame (i.e. Everytime the Update() function is called).
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How much Time has passed, in seconds, 
		/// since the last Update</param>
		public void Update(float fElapsedTimeInSeconds)
		{
			Update(fElapsedTimeInSeconds, fElapsedTimeInSeconds);
			RemoveAllOneTimeEvents();
		}

		/// <summary>
		/// Updates the Particle System according to the Particle System Events
		/// </summary>
		/// <param name="fElapsedTimeThisPass">The amount of Elapsed Time to pass
		/// into the Event Functions being called on this Pass</param>
		/// <param name="fTotalElapsedTimeThisFrame">How much Time has passed, in seconds, 
		/// since the last Frame</param>
		private void Update(float fElapsedTimeThisPass, float fTotalElapsedTimeThisFrame)
		{
			bool flag = false;
			float fElapsedTimeThisPass2 = 0f;
			LifetimeData.UpdateElapsedTimeVariables(fElapsedTimeThisPass);
			if (mcParticleSystemEventList.Count <= 0)
			{
				return;
			}
			if (LifetimeData.ElapsedTime >= LifetimeData.Lifetime && LifetimeData.EndOfLifeOption == EParticleSystemEndOfLifeOptions.Repeat)
			{
				flag = true;
				fElapsedTimeThisPass2 = LifetimeData.ElapsedTime - LifetimeData.Lifetime;
				fElapsedTimeThisPass -= fElapsedTimeThisPass2;
				fElapsedTimeThisPass = Math.Max(fElapsedTimeThisPass, 0f);
				fElapsedTimeThisPass2 %= LifetimeData.Lifetime;
				LifetimeData.ElapsedTime = LifetimeData.LastElapsedTime;
				LifetimeData.UpdateElapsedTimeVariables(LifetimeData.Lifetime - LifetimeData.ElapsedTime);
			}
			if (fElapsedTimeThisPass > 0f)
			{
				for (int i = 0; i < mcParticleSystemEventList.Count; i++)
				{
					if (mcParticleSystemEventList[i].eType == EParticleSystemEventTypes.EveryTime)
					{
						mcParticleSystemEventList[i].cFunctionToCall(fElapsedTimeThisPass);
						continue;
					}
					if (mcParticleSystemEventList[i].eType == EParticleSystemEventTypes.OneTime)
					{
						if (!flag)
						{
							mcParticleSystemEventList[i].cFunctionToCall(fTotalElapsedTimeThisFrame);
						}
						continue;
					}
					CTimedParticleSystemEvent cTimedParticleSystemEvent = (CTimedParticleSystemEvent)mcParticleSystemEventList[i];
					if (cTimedParticleSystemEvent.eType == EParticleSystemEventTypes.Timed)
					{
						float num = ((LifetimeData.LastElapsedTime == 0f) ? (-0.1f) : LifetimeData.LastElapsedTime);
						if (num < cTimedParticleSystemEvent.fTimeToFire && LifetimeData.ElapsedTime >= cTimedParticleSystemEvent.fTimeToFire)
						{
							cTimedParticleSystemEvent.cFunctionToCall(fElapsedTimeThisPass);
						}
					}
					else
					{
						float num2 = ((LifetimeData.LastNormalizedElapsedTime == 0f) ? (-0.1f) : LifetimeData.LastNormalizedElapsedTime);
						if (num2 < cTimedParticleSystemEvent.fTimeToFire && LifetimeData.NormalizedElapsedTime >= cTimedParticleSystemEvent.fTimeToFire)
						{
							cTimedParticleSystemEvent.cFunctionToCall(fElapsedTimeThisPass);
						}
					}
				}
			}
			if (flag)
			{
				LifetimeData.ElapsedTime = 0f;
				Update(fElapsedTimeThisPass2, fTotalElapsedTimeThisFrame);
			}
		}
	}

	private const int MAX_MEMORY_IN_BYTES_THAT_XBOX_CAN_DRAW = 524287;

	private InitializeParticleDelegate mcParticleInitializationFunction;

	private UpdateVertexDelegate mcVertexUpdateFunction;

	private ParticleTypes meParticleType;

	private Particle[] mcParticles;

	private int miNumberOfParticlesToDraw;

	private Vertex[] mcParticleVerticesToDraw;

	private int[] miaIndexBufferArray;

	private short[] msaIndexBufferReachArray;

	private int miIndexBufferIndex;

	private SpriteBatch mcSpriteBatch;

	private Particle[] mcParticleSpritesToDraw;

	private SpriteBatchSettings mcSpriteBatchSettings;

	private LinkedList<Particle> mcActiveParticlesList;

	private LinkedList<Particle> mcInactiveParticlesList;

	private VertexDeclaration mcVertexDeclaration;

	private int miVertexSizeInBytes;

	private RenderProperties mcRenderProperties;

	private Effect mcEffect;

	private Texture2D mcTexture;

	private int miMaxNumberOfParticlesAllowed;

	private bool mbPerformDraws = true;

	private bool mbPerformUpdates = true;

	private int miDrawOrder;

	private int miUpdateOrder;

	private Game mcGame;

	private GraphicsDevice mcGraphicsDevice;

	private ContentManager mcContentManager;

	private float mfSimulationSpeed = 1f;

	private float mfInternalSimulationSpeed = 1f;

	private float mfTimeToWaitBetweenUpdates;

	private float mfTimeElapsedSinceLastUpdate;

	private CParticleEvents mcParticleEvents;

	private CParticleSystemEvents mcParticleSystemEvents;

	private AutoMemoryManagerSettings mcAutoMemoryManagerSettings;

	private float mfAutoMemoryManagersElapsedTime;

	private int miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds;

	private ParticleEmitter mcEmitter;

	private bool _lerpEmittersPositionAndOrientation = true;

	private Vector3 _emittersPreviousPosition = Vector3.Zero;

	private Quaternion _emittersPreviousOrientation = Quaternion.Identity;

	private Stopwatch _performanceProfilingStopwatch;

	private bool _performanceProfilingIsEnabled;

	private int miID;

	private int miType;

	private RandomNumbers mcRandom;

	private Matrix mcWorld = Matrix.Identity;

	private Matrix mcView = Matrix.Identity;

	private Matrix mcProjection = Matrix.Identity;

	private ParticleSystemManager mcParticleSystemManager;

	private static DPSFDefaultEffect SmcDPSFEffect;

	/// <summary>
	/// A static int used to keep track of the total number of Particle Systems created
	/// </summary>
	private static int _totalNumberOfParticleSystemsCreated;

	/// <summary>
	/// A static int used to keep track of how many DPSF particle systems are initialized at any given moment.
	/// </summary>
	private static int _numberOfParticleSystemsCurrentlyInitialized;

	private int miMaxParticlesThatXboxCanDrawAtOnce;

	/// <summary>
	/// The path used to load the Texture when the InitializeNonSerializableProperties() function is called.
	/// <para>NOTE: This is automatically set when the SetTexture() function is called.</para>
	/// </summary>
	public string DeserializationTexturePath { get; set; }

	/// <summary>
	/// The path used to load the Effect when the InitializeNonSerializableProperties() function is called.
	/// <para>NOTE: This is automatically set when the SetEffectAndTechnique(string, string) function is called. </para>
	/// </summary>
	public string DeserializationEffectPath { get; set; }

	/// <summary>
	/// The Name of the Technique to use when the InitializeNonSerializableProperties() function is called.
	/// <para>NOTE: This is automatically set when the SetEffectAndTechnique() and SetTechnique() functions are called.</para>
	/// </summary>
	public string DeserializationTechniqueName { get; set; }

	/// <summary>
	/// Returns true if the Particle System is Initialized, false if not.
	/// </summary>
	/// <returns>Returns true if the Particle System is Initialized, false if not.</returns>
	public bool IsInitialized
	{
		get
		{
			if (mcParticles != null && meParticleType != ParticleTypes.None)
			{
				if (GraphicsDevice == null)
				{
					return meParticleType == ParticleTypes.NoDisplay;
				}
				return true;
			}
			return false;
		}
	}

	/// <summary>
	/// A custom effect provided by DPSF. In DPSF v2.1.0 and prior this effect was used as the default effect for all particle system types.
	/// Each particle system type now uses one of the built-in XNA 4 effects as its default effect in order to make all of the particle system types
	/// fully compatible with the Reach profile, and usable on the Windows Phone 7.
	/// <para>This effect may still be used for a particle system by calling the SetEffectAndTechnique() function from the particle system's overridden
	/// InitializeRenderProperties() function.</para>
	/// <para>This Effect has several techniques that may be used (<see cref="T:DPSF.DPSFDefaultEffectTechniques" />).</para>
	/// </summary>
	public DPSFDefaultEffect DPSFDefaultEffect => SmcDPSFEffect;

	/// <summary>
	/// Get / Set if this Particle System should Draw its Particles or not.
	/// <para>NOTE: Setting this to false causes the Draw() function to not draw anything, including the 
	/// BeforeDraw() and AfterDraw() functions not to be called.</para>
	/// </summary>
	public bool Visible
	{
		get
		{
			return mbPerformDraws;
		}
		set
		{
			bool flag = mbPerformDraws;
			mbPerformDraws = value;
			if (flag != mbPerformDraws && VisibleChanged != null)
			{
				VisibleChanged(this, null);
			}
		}
	}

	/// <summary>
	/// Get / Set if this Particle System should Update itself and its Particles or not.
	/// <para>NOTE: Setting this to false causes the Update() function to not update anything.</para>
	/// </summary>
	[DPSFViewerParameter]
	public bool Enabled
	{
		get
		{
			return mbPerformUpdates;
		}
		set
		{
			mbPerformUpdates = value;
			if (EnabledChanged != null)
			{
				EnabledChanged(this, null);
			}
		}
	}

	/// <summary>
	/// The Order in which the Particle System should be Updated relative to other 
	/// DPSF Particle Systems in the same Particle System Manager. Particle Systems 
	/// are Updated in ascending order according to their Update Order (i.e. lowest first).
	/// <para>NOTE: The Update Order is one of the few properties that is not reset when
	/// the particle system is initialized or destroyed.</para>
	/// </summary>
	public int UpdateOrder
	{
		get
		{
			return miUpdateOrder;
		}
		set
		{
			miUpdateOrder = value;
			if (UpdateOrderChanged != null)
			{
				UpdateOrderChanged(this, null);
			}
		}
	}

	/// <summary>
	/// The Order in which the Particle System should be Drawn relative to other
	/// DPSF Particle Systems in the same Particle System Manager. Particle Systems
	/// are Drawn in ascending order according to their Draw Order (i.e. lowest first).
	/// <para>NOTE: The Draw Order is one of the few properties that is not reset when
	/// the particle system is initialized or destroyed.</para>
	/// </summary>
	public int DrawOrder
	{
		get
		{
			return miDrawOrder;
		}
		set
		{
			miDrawOrder = value;
			if (DrawOrderChanged != null)
			{
				DrawOrderChanged(this, null);
			}
		}
	}

	/// <summary>
	/// Get the Game object set in the constructor, if one was given.
	/// </summary>
	public Game Game => mcGame;

	/// <summary>
	/// Get / Set the Graphics Device to draw to
	/// </summary>
	public GraphicsDevice GraphicsDevice
	{
		get
		{
			return mcGraphicsDevice;
		}
		set
		{
			if (value != null || ParticleType == ParticleTypes.NoDisplay)
			{
				mcGraphicsDevice = value;
				return;
			}
			throw new ArgumentNullException("GraphicsDevice", "The specified Graphics Device is null. A valid Graphics Device is required.");
		}
	}

	/// <summary>
	/// Get if the Particle System is inheriting from DrawableGameComponent or not.
	/// <para>If inheriting from DrawableGameComponent, the Particle Systems
	/// are automatically added to the given Game object's Components and the
	/// Update() and Draw() functions are automatically called by the
	/// Game object when it updates and draws the rest of its Components.
	/// If the Update() and Draw() functions are called by the user anyways,
	/// they will exit without performing any operations, so it is suggested
	/// to include them anyways to make switching between inheriting and
	/// not inheriting from DrawableGameComponent seemless; just be aware
	/// that the updates and draws are actually being performed when the
	/// Game object is told to update and draw (i.e. when base.Update() and base.Draw()
	/// are called), not when these functions are being called.</para>
	/// </summary>
	public bool InheritsDrawableGameComponent => false;

	/// <summary>
	/// Get the unique ID of this Particle System.
	/// <para>NOTE: Each Particle System is automatically assigned a unique ID when it is instanciated.</para>
	/// </summary>
	public int ID => miID;

	/// <summary>
	/// Get / Set the Type of Particle System this is. This is a user provided value that you can use for whatever
	/// purpose you want; it is not used by the built-in DPSF functionality in any way.
	/// </summary>
	public int Type
	{
		get
		{
			return miType;
		}
		set
		{
			miType = value;
		}
	}

	/// <summary>
	/// Get the Name of the Class that this Particle System is using. This can be used to 
	/// check what type of Particle System this is at run-time.
	/// </summary>
	public string ClassName => GetType().Name;

	/// <summary>
	/// Get / Set the Content Manager to use to load Textures and Effects
	/// </summary>
	public ContentManager ContentManager
	{
		get
		{
			if (mcContentManager != null)
			{
				return mcContentManager;
			}
			throw new NullReferenceException("The Content Manager is trying to be accessed, but is null. Be sure you have Initialized the particle system and provided a valid Content Manager.");
		}
		set
		{
			if (value != null || ParticleType == ParticleTypes.NoDisplay)
			{
				mcContentManager = value;
				return;
			}
			throw new ArgumentNullException("ContentManager", "The specified Content Manager is null. A valid Content Manager is required.");
		}
	}

	/// <summary>
	/// Get / Set the Index Buffer values. The Index Buffer is used when drawing Quads in the HiDef profile.
	/// </summary>
	protected int[] IndexBuffer
	{
		get
		{
			return miaIndexBufferArray;
		}
		set
		{
			miaIndexBufferArray = value;
		}
	}

	/// <summary>
	/// Get / Set the Index Buffer values. The Index Buffer Reach is used when drawing Quads in the Reach profile.
	/// </summary>
	protected short[] IndexBufferReach
	{
		get
		{
			return msaIndexBufferReachArray;
		}
		set
		{
			msaIndexBufferReachArray = value;
		}
	}

	/// <summary>
	/// Get / Set the current position in the Index Buffer
	/// </summary>
	protected int IndexBufferIndex
	{
		get
		{
			return miIndexBufferIndex;
		}
		set
		{
			miIndexBufferIndex = value;
		}
	}

	/// <summary>
	/// Particle Events may be used to update Particles
	/// </summary>
	public CParticleEvents ParticleEvents
	{
		get
		{
			if (mcParticleEvents != null)
			{
				return mcParticleEvents;
			}
			throw new NullReferenceException("The ParticleEvents property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
		}
	}

	/// <summary>
	/// Particle System Events may be used to update the Particle System
	/// </summary>
	public CParticleSystemEvents ParticleSystemEvents
	{
		get
		{
			if (mcParticleSystemEvents != null)
			{
				return mcParticleSystemEvents;
			}
			throw new NullReferenceException("The ParticleSystemEvents property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
		}
	}

	/// <summary>
	/// Get the render properties used to draw the particles.
	/// </summary>
	public RenderProperties RenderProperties => mcRenderProperties;

	/// <summary>
	/// Returns if this particle system is dependent on an external Sprite Batch to draw its particles or not.
	/// <para>If false, the particle system will use its own SpriteBatch to draw its particles.</para>
	/// <para>If true, then you must call SpriteBatch.Begin() before calling ParticleSystem.Draw() to
	/// draw the particle system, and then call SpriteBatch.End() when done drawing the particle system, where
	/// the SpriteBatch referred to here is the one you passed into the InitializeSpriteParticleSystem() function.</para>
	/// <para>NOTE: This property only applies to Sprite particle systems.</para>
	/// </summary>
	public bool UsingExternalSpriteBatchToDrawParticles { get; private set; }

	/// <summary>
	/// The Sprite Batch drawing Settings used in the Sprite Batch's Begin() function call.
	/// <para>NOTE: These settings are only available for Sprite particle systems, and only for
	/// the Sprite particle systems using their own SpriteBatch (i.e. UsingExternalSpriteBatchToDrawParticles = false).</para>
	/// </summary>
	public SpriteBatchSettings SpriteBatchSettings
	{
		get
		{
			if (mcSpriteBatchSettings != null)
			{
				return mcSpriteBatchSettings;
			}
			throw new NullReferenceException("The SpriteBatchSettings property is trying to be accessed, but is null. Be sure you have Initialized the particle system. Also, this property is only available when using a Sprite particle system, and not Using and External Sprite Batch To Draw the Particles.");
		}
	}

	/// <summary>
	/// The Settings used to control the Automatic Memory Manager
	/// </summary>
	public AutoMemoryManagerSettings AutoMemoryManagerSettings
	{
		get
		{
			if (mcAutoMemoryManagerSettings != null)
			{
				return mcAutoMemoryManagerSettings;
			}
			throw new NullReferenceException("The AutoMemoryManagerSettings property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
		}
	}

	/// <summary>
	/// The Emitter is used to automatically generate new Particles
	/// </summary>
	public ParticleEmitter Emitter
	{
		get
		{
			if (mcEmitter != null)
			{
				return mcEmitter;
			}
			throw new NullReferenceException("The Emitter property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
		}
		set
		{
			if (value != null)
			{
				mcEmitter = value;
				return;
			}
			throw new ArgumentNullException("Emitter", "An invalid Emitter was specified. The Emitter cannot be null.");
		}
	}

	/// <summary>
	/// This property tells if the we should Lerp (Linearly Interpolate) the Position and Orientation of the Emitter from one update to 
	/// the next. If the Emitter is moving very fast, this allows the particle system to spawn new particles in between the Emitter's old
	/// and new position, so that new particles are evenly spaced out between the Emitter's previous and current position, instead of all 
	/// of the particles being spawned at the Emitter's new position.
	/// <para>If this property is true, the Emitter's Position and Orientation will be Lerped while emitting particles.</para>
	/// <para>If this property is false, all of the particles will be emitted as the Emitter's current Position and Orientation.</para>
	/// <para>If you generally want Lerping enabled, but want to temporarily disable it to "teleport" the emitter from one position
	/// to another without particles being Lerped between the two positions, you can set this properly to false and then back to true 
	/// after the particle system's Update() function has been called, or you can simply set the LerpEmittersPositionAndOrientationOnNextUpdate
	/// to false, which will disable Lerping the position and orientation only for the next particle system Update().</para>
	/// <para>Default is true.</para>
	/// </summary>
	public bool LerpEmittersPositionAndOrientation
	{
		get
		{
			return _lerpEmittersPositionAndOrientation;
		}
		set
		{
			_lerpEmittersPositionAndOrientation = value;
		}
	}

	/// <summary>
	/// If this is true the Emitter's Position and Orientation will not be Lerped during the particle system's next Update() function call.
	/// The Update() function will always set this value back to false after all of the particle's have been emitted for that Update() call.
	/// <para>Setting this to true allows you to "teleport" the Emitter from one position to another without particles being released at any
	/// positions in between the Emitter's old and new Position and Orientation.</para>
	/// </summary>
	public bool LerpEmittersPositionAndOrientationOnNextUpdate { get; set; }

	/// <summary>
	/// Get a RandomNumbers object used to generate Random Numbers
	/// </summary>
	public RandomNumbers RandomNumber
	{
		get
		{
			if (mcRandom != null)
			{
				return mcRandom;
			}
			throw new NullReferenceException("The RandomNumber property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
		}
	}

	/// <summary>
	/// Get / Set the World Matrix to use for drawing 3D Particles
	/// </summary>
	public Matrix World
	{
		get
		{
			return mcWorld;
		}
		set
		{
			mcWorld = value;
		}
	}

	/// <summary>
	/// Get / Set the View Matrix to use for drawing 3D Particles
	/// </summary>
	public Matrix View
	{
		get
		{
			return mcView;
		}
		set
		{
			mcView = value;
		}
	}

	/// <summary>
	/// Get / Set the Projection Matrix to use for drawing 3D Particles
	/// </summary>
	public Matrix Projection
	{
		get
		{
			return mcProjection;
		}
		set
		{
			mcProjection = value;
		}
	}

	/// <summary>
	/// Gets the result of multiplying the World, View, and Projection matrices.
	/// </summary>
	public Matrix WorldViewProjection => Matrix.Multiply(Matrix.Multiply(World, View), Projection);

	/// <summary>
	/// Set the VertexElement (i.e. Vertex Format) to use for each vertex of a Particle.
	/// <para>NOTE: VertexElement will not be changed if null value is given.</para>
	/// </summary>
	private VertexElement[] VertexElement
	{
		set
		{
			if (value != null && GraphicsDevice != null)
			{
				mcVertexDeclaration = new VertexDeclaration(value);
				return;
			}
			if (ParticleType == ParticleTypes.None || ParticleType == ParticleTypes.NoDisplay || ParticleType == ParticleTypes.Sprite)
			{
				mcVertexDeclaration = null;
				return;
			}
			if (GraphicsDevice == null)
			{
				throw new ArgumentNullException("GraphicsDevice", "The current Graphics Device is null. A valid Graphics Device is required to create a new Vertex Declaration in order to draw the current type of Particles.");
			}
			throw new ArgumentNullException("VertexElement", "The specified Vertex Element is null. A valid Vertex Element is required to draw the current Type of Particles.");
		}
	}

	/// <summary>
	/// Set the function to use to copy a Particle's renderable properties into the Vertex Buffer.
	/// <para>NOTE: VertexUpdateFunction will not be changed if null value is given.</para>
	/// </summary>
	public UpdateVertexDelegate VertexUpdateFunction
	{
		set
		{
			if (value == null)
			{
				if (ParticleType != ParticleTypes.None && ParticleType != ParticleTypes.NoDisplay && ParticleType != ParticleTypes.Sprite)
				{
					throw new ArgumentNullException("VertexUpdateFunction", "The specified Vertex Update Function is null. A valid Vertex Update Function is required to draw the current Type of Particles.");
				}
				mcVertexUpdateFunction = null;
			}
			else
			{
				mcVertexUpdateFunction = value.Invoke;
			}
		}
	}

	/// <summary>
	/// Sets the function to use to Initialize a Particle's properties.
	/// </summary>
	public InitializeParticleDelegate ParticleInitializationFunction
	{
		set
		{
			if (value != null)
			{
				mcParticleInitializationFunction = value.Invoke;
			}
			else
			{
				mcParticleInitializationFunction = null;
			}
		}
	}

	/// <summary>
	/// Get / Set the Effect to use to draw the Particles
	/// </summary>
	public Effect Effect
	{
		get
		{
			return mcEffect;
		}
		set
		{
			if (value == null)
			{
				if (ParticleType != ParticleTypes.None && ParticleType != ParticleTypes.NoDisplay && ParticleType != ParticleTypes.Sprite)
				{
					throw new ArgumentNullException("Effect", "Specified Effect to use is null. A valid Effect must be used to draw the current Type of Particles.");
				}
				mcEffect = null;
			}
			else
			{
				mcEffect = value.Clone();
			}
		}
	}

	/// <summary>
	/// Get / Set which Technique of the current Effect to use to draw the Particles
	/// </summary>
	public EffectTechnique Technique
	{
		get
		{
			if (mcEffect != null)
			{
				return mcEffect.CurrentTechnique;
			}
			return null;
		}
		set
		{
			if (mcEffect == null)
			{
				throw new InvalidOperationException("Effect is null when trying to specify the Technique to use. The Effect must be set before specifying the Technique.");
			}
			if (value == null)
			{
				throw new ArgumentNullException("Technique", "An invalid Technique to use was specified. The Technique to use cannot be null.");
			}
			mcEffect.CurrentTechnique = value;
		}
	}

	/// <summary>
	/// Get / Set the Texture to use to draw the Particles
	/// </summary>
	public Texture2D Texture
	{
		get
		{
			return mcTexture;
		}
		set
		{
			if (value == null)
			{
				if (ParticleType == ParticleTypes.Sprite || ParticleType == ParticleTypes.TexturedQuad)
				{
					throw new ArgumentNullException("sTexture", "Specified Texture to use is null. A valid Texture must be set to draw the current Type of Particles.");
				}
				mcTexture = null;
			}
			else
			{
				mcTexture = value;
			}
		}
	}

	/// <summary>
	/// Get / Set how fast the Particle System Simulation should run.
	/// <para>Example: 1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.</para>
	/// <para>NOTE: If a negative value is specified, the Speed Scale is set 
	/// to zero (pauses the simulation; has same effect as Enabled = false).</para>
	/// </summary>
	public float SimulationSpeed
	{
		get
		{
			return mfSimulationSpeed;
		}
		set
		{
			if (value < 0f)
			{
				mfSimulationSpeed = 0f;
			}
			else
			{
				mfSimulationSpeed = value;
			}
		}
	}

	/// <summary>
	/// Get / Set how fast the Particle System Simulation should run to look "normal".
	/// <para>1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.</para>
	/// <para>This is provided as a way of speeding up / slowing down the simulation to have 
	/// it look as desired, without having to rescale all of the particle velocities, etc. This allows
	/// you to use the exact same particle system class to create two particle systems, and then have one run
	/// slower or faster than the other, creating two different effects. If you then wanted to speed up or slow down
	/// both effects (i.e. particle systems), you could adjust the SimulationSpeed property on both particle systems 
	/// without having to worry about adjusting this property at all to get the effects back to normal speed; just reset 
	/// the SimulationSpeed property you changed back to 1.0.</para>
	/// <para>NOTE: If a negative value is specified, the Internal Simulation Speed is set to zero 
	/// (pauses the simulation; has the same effect as Enabled = false).</para>
	/// </summary>
	public float InternalSimulationSpeed
	{
		get
		{
			return mfInternalSimulationSpeed;
		}
		set
		{
			if (value < 0f)
			{
				mfInternalSimulationSpeed = 0f;
			}
			else
			{
				mfInternalSimulationSpeed = value;
			}
		}
	}

	/// <summary>
	/// Specify how often the Particle System should be Updated.
	/// <para>NOTE: Specifying a value of zero (default) will cause the Particle 
	/// System to be Updated every time the Update() function is called 
	/// (i.e. as often as possible).</para>
	/// <para>NOTE: If the Update() function is not called often enough to
	/// keep up with this specified Update rate, the Update function
	/// updates the Particle Systems as often as possible.</para>
	/// </summary>
	public int UpdatesPerSecond
	{
		get
		{
			if (mfTimeToWaitBetweenUpdates == 0f)
			{
				return 0;
			}
			return (int)(1f / mfTimeToWaitBetweenUpdates);
		}
		set
		{
			if (value <= 0)
			{
				mfTimeToWaitBetweenUpdates = 0f;
			}
			else
			{
				mfTimeToWaitBetweenUpdates = 1f / (float)value;
			}
		}
	}

	/// <summary>
	/// Get / Set if performance timings should be measured or not, such as how long it takes to perform updates and draws.
	/// <para>This should be disabled before building a release version of your application.</para>
	/// <para>Note: Performance profiling is not available on the Reach profile, so this will always return False on the Reach profile.</para>
	/// </summary>
	public bool PerformanceProfilingIsEnabled
	{
		get
		{
			return _performanceProfilingIsEnabled;
		}
		set
		{
			if (GraphicsDevice != null && GraphicsDevice.GraphicsProfile == GraphicsProfile.Reach)
			{
				_performanceProfilingIsEnabled = false;
			}
			else
			{
				_performanceProfilingIsEnabled = value;
			}
			if (_performanceProfilingIsEnabled)
			{
				if (_performanceProfilingStopwatch == null)
				{
					_performanceProfilingStopwatch = new Stopwatch();
				}
			}
			else
			{
				_performanceProfilingStopwatch = null;
			}
		}
	}

	/// <summary>
	/// Get how long (in milliseconds) it took to perform the last Update() function call.
	/// <para>Returns 0 if Performance Profiling is not Enabled.</para>
	/// </summary>
	public double PerformanceTimeToDoUpdateInMilliseconds { get; private set; }

	/// <summary>
	/// Get how long (in milliseconds) it took to perform the last Draw() function call.
	/// <para>Returns 0 if Performance Profiling is not Enabled.</para>
	/// </summary>
	public double PerformanceTimeToDoDrawInMilliseconds { get; private set; }

	/// <summary>
	/// The Particle System Manager whose properties (SimulationSpeed and 
	/// UpdatesPerSecond) this particle system should follow.  If null is not specified,
	/// the Manager's properties will be copied into this particle system immediately.
	/// <para>NOTE: This Particle System's properties will only clone the Manager's properties
	/// if the Manager's properties are Enabled. For example, the Manager's SimulationSpeed
	/// will only be copied to this Particle System if the Manager's SimulationSpeedIsEnabled
	/// property is true.</para>
	/// <para>NOTE: This value is automatically set to the last Particle System Manager the 
	/// Particle System is added to.</para>
	/// </summary>
	public ParticleSystemManager ParticleSystemManagerToCopyPropertiesFrom
	{
		get
		{
			return mcParticleSystemManager;
		}
		set
		{
			mcParticleSystemManager = value;
			if (mcParticleSystemManager != null)
			{
				if (mcParticleSystemManager.SimulationSpeedIsEnabled)
				{
					SimulationSpeed = mcParticleSystemManager.SimulationSpeed;
				}
				if (mcParticleSystemManager.UpdatesPerSecondIsEnabled)
				{
					UpdatesPerSecond = mcParticleSystemManager.UpdatesPerSecond;
				}
			}
		}
	}

	/// <summary>
	/// Get the type of Particles that this Particle System should draw.
	/// </summary>
	public ParticleTypes ParticleType
	{
		get
		{
			return meParticleType;
		}
		private set
		{
			meParticleType = value;
		}
	}

	/// <summary>
	/// Get / Set the absolute Number of Particles to Allocate Memory for.
	/// <para>NOTE: This value must be greater than or equal to zero.</para>
	/// <para>NOTE: Even if this many particles aren't used, the space for this many Particles 
	/// is still allocated in memory.</para>
	/// </summary>
	public int NumberOfParticlesAllocatedInMemory
	{
		get
		{
			if (mcParticles == null)
			{
				return 0;
			}
			return mcParticles.Length;
		}
		set
		{
			if (value >= 0)
			{
				bool flag = mcActiveParticlesList != null && mcActiveParticlesList.Count > 0;
				LinkedList<Particle> linkedList = (flag ? mcActiveParticlesList : null);
				Vertex[] array = (flag ? mcParticleVerticesToDraw : null);
				int[] array2 = (flag ? miaIndexBufferArray : null);
				short[] array3 = (flag ? msaIndexBufferReachArray : null);
				int num = miNumberOfParticlesToDraw;
				Particle[] array4 = (flag ? mcParticleSpritesToDraw : null);
				mcParticles = new Particle[value];
				mcActiveParticlesList = new LinkedList<Particle>();
				mcInactiveParticlesList = new LinkedList<Particle>();
				InitializeParticleArrays();
				if (!flag)
				{
					return;
				}
				if (num > value)
				{
					num = value;
				}
				LinkedListNode<Particle> linkedListNode = linkedList.Last;
				while (linkedListNode != null && AddParticle(linkedListNode.Value))
				{
					linkedListNode = linkedListNode.Previous;
				}
				if (num <= 0)
				{
					return;
				}
				switch (meParticleType)
				{
				case ParticleTypes.Quad:
				case ParticleTypes.TexturedQuad:
				{
					int num2 = num * 4;
					for (int j = 0; j < num2; j++)
					{
						mcParticleVerticesToDraw[j] = array[j];
					}
					if (GraphicsDevice.GraphicsProfile == GraphicsProfile.HiDef)
					{
						int val = ((miaIndexBufferArray != null) ? miaIndexBufferArray.Length : 0);
						int num3 = Math.Min(num * 6, val);
						for (int k = 0; k < num3; k++)
						{
							miaIndexBufferArray[k] = array2[k];
						}
					}
					else
					{
						int val2 = ((msaIndexBufferReachArray != null) ? msaIndexBufferReachArray.Length : 0);
						int num4 = Math.Min(num * 6, val2);
						for (int l = 0; l < num4; l++)
						{
							msaIndexBufferReachArray[l] = array3[l];
						}
					}
					break;
				}
				case ParticleTypes.Sprite:
				{
					for (int i = 0; i < num; i++)
					{
						mcParticleSpritesToDraw[i] = array4[i];
					}
					break;
				}
				}
				miNumberOfParticlesToDraw = num;
				return;
			}
			throw new ArgumentException("MaxNumberOfParticles", "The specified Max Number Of Particles is less than or equal to zero. The Max Number Of Particles must be greater than zero.");
		}
	}

	/// <summary>
	/// Get / Set the Max Number of Particles this Particle System is Allowed to contain at any given time.
	/// <para>NOTE: The Automatic Memory Manager will never allocate space for more Particles than this.</para>
	/// <para>NOTE: This value must be greater than or equal to zero.</para>
	/// </summary>
	public int MaxNumberOfParticlesAllowed
	{
		get
		{
			return miMaxNumberOfParticlesAllowed;
		}
		set
		{
			int num = value;
			if (num < 0)
			{
				num = 0;
			}
			miMaxNumberOfParticlesAllowed = num;
		}
	}

	/// <summary>
	/// Get the number of Particles that are currently Active
	/// </summary>
	public int NumberOfActiveParticles
	{
		get
		{
			if (IsInitialized)
			{
				return mcActiveParticlesList.Count;
			}
			return 0;
		}
	}

	/// <summary>
	/// Get the number of Particles being Drawn. That is, how many Particles 
	/// are both Active AND Visible.
	/// </summary>
	public int NumberOfParticlesBeingDrawn => miNumberOfParticlesToDraw;

	/// <summary>
	/// Get the number of Particles that may still be added before reaching the
	/// Max Number Of Particles Allowed. If the Max Number Of Particles Allowed is 
	/// greater than the Number Of Particles Allocated In Memory AND the Auto Memory Manager is
	/// set to not increase the amount of Allocated Memory, than this returns the number 
	/// of Particles that may still be added before running out of Memory.
	/// </summary>
	public int NumberOfParticlesStillPossibleToAdd
	{
		get
		{
			if (NumberOfParticlesAllocatedInMemory < MaxNumberOfParticlesAllowed && AutoMemoryManagerSettings.MemoryManagementMode != AutoMemoryManagerModes.IncreaseAndDecrease && AutoMemoryManagerSettings.MemoryManagementMode != AutoMemoryManagerModes.IncreaseOnly)
			{
				return NumberOfParticlesAllocatedInMemory - NumberOfActiveParticles;
			}
			return MaxNumberOfParticlesAllowed - NumberOfActiveParticles;
		}
	}

	/// <summary>
	/// Get / Protected Set a Linked List whose Nodes point to the Active Particles.
	/// <para>NOTE: The Protected Set option is only provided to allow the order of the 
	/// LinkedListNodes to be changed, changing the update and drawing 
	/// order of the Particles. Be sure that all of the original LinkedListNodes 
	/// (and only the original LinkedListNodes, no more) obtained from the 
	/// Get are included; they may only be rearranged. If they are not, 
	/// there may (and probably will) be unexpected results.</para>
	/// </summary>
	public LinkedList<Particle> ActiveParticles
	{
		get
		{
			if (mcActiveParticlesList != null)
			{
				return mcActiveParticlesList;
			}
			throw new NullReferenceException("The ActiveParticles property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
		}
		protected set
		{
			mcActiveParticlesList = value;
		}
	}

	/// <summary>
	/// Returns a Linked List whose Nodes point to the Inactive Particles
	/// </summary>
	public LinkedList<Particle> InactiveParticles
	{
		get
		{
			if (mcInactiveParticlesList != null)
			{
				return mcInactiveParticlesList;
			}
			throw new NullReferenceException("The InactiveParticles property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
		}
	}

	/// <summary>
	/// Returns the array of all Particle objects
	/// </summary>
	public Particle[] Particles
	{
		get
		{
			if (mcParticles != null)
			{
				return mcParticles;
			}
			throw new NullReferenceException("The Particles property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
		}
	}

	/// <summary>
	/// Raised when the UpdateOrder property changes
	/// </summary>
	public event EventHandler<EventArgs> UpdateOrderChanged;

	/// <summary>
	/// Raised when the DrawOrder property changes
	/// </summary>
	public event EventHandler<EventArgs> DrawOrderChanged;

	/// <summary>
	/// Raised when the Enabled property changes
	/// </summary>
	public event EventHandler<EventArgs> EnabledChanged;

	/// <summary>
	/// Raised when the Visible property changes
	/// </summary>
	public event EventHandler<EventArgs> VisibleChanged;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DPSF(Game cGame)
	{
		mcGame = cGame;
		miID = _totalNumberOfParticleSystemsCreated++;
	}

	/// <summary>
	/// Initializes a new No Display Particle System. This type of Particle System does not allow any of the Particles
	/// to be drawn to a Graphics Device (e.g. the screen).
	/// </summary>
	/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
	/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
	/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
	/// may also be adjusted manually at run-time.</param>
	/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
	/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
	/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
	/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
	/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
	/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
	/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
	/// may also be adjusted manually at run-time.</param>
	public void InitializeNoDisplayParticleSystem(int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow)
	{
		try
		{
			InitializeCommonVariables(null, null, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.NoDisplay);
		}
		catch (Exception ex)
		{
			ParticleType = ParticleTypes.None;
			throw new Exception("A problem occurred while Initializing the Particle System. Inner Exception: " + ex.ToString(), ex);
		}
		AfterInitialize();
	}

	/// <summary>
	/// Initializes a new Sprite Particle System
	/// </summary>
	/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
	/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
	/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
	/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
	/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
	/// may also be adjusted manually at run-time.</param>
	/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
	/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
	/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
	/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
	/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
	/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
	/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
	/// may also be adjusted manually at run-time.</param>
	/// <param name="sTexture">The asset name of the Texture to use to visualize the Particles</param>
	public void InitializeSpriteParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, string sTexture)
	{
		InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, sTexture, null);
	}

	/// <summary>
	/// Initializes a new Sprite Particle System
	/// </summary>
	/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
	/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
	/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
	/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
	/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
	/// may also be adjusted manually at run-time.</param>
	/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
	/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
	/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
	/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
	/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
	/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
	/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
	/// may also be adjusted manually at run-time.</param>
	/// <param name="sTexture">The asset name of the Texture to use to visualize the Particles</param>
	/// <param name="cSpriteBatchToDrawWith">The Sprite Batch that this particle system should use to draw its
	/// particles with.
	/// <para>If null, the particle system will use its own SpriteBatch to draw its particles.</para>
	/// <para>If not null, then you must call SpriteBatch.Begin() before calling ParticleSystem.Draw() to
	/// draw the particle system, and then call SpriteBatch.End() when done drawing the particle system.</para></param>
	public void InitializeSpriteParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, string sTexture, SpriteBatch cSpriteBatchToDrawWith)
	{
		try
		{
			InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.Sprite);
			SetTexture(sTexture);
			if (cSpriteBatchToDrawWith != null)
			{
				mcSpriteBatch = cSpriteBatchToDrawWith;
				mcSpriteBatchSettings = null;
				UsingExternalSpriteBatchToDrawParticles = true;
			}
		}
		catch (Exception ex)
		{
			ParticleType = ParticleTypes.None;
			throw new Exception("A problem occurred while Initializing the Particle System. Inner Exception: " + ex.ToString(), ex);
		}
		AfterInitialize();
	}

	/// <summary>
	/// Initializes a new Sprite Particle System
	/// </summary>
	/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
	/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
	/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
	/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
	/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
	/// may also be adjusted manually at run-time.</param>
	/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
	/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
	/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
	/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
	/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
	/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
	/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
	/// may also be adjusted manually at run-time.</param>
	/// <param name="cTexture">The Texture to use to visualize the Particles</param>
	public void InitializeSpriteParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, Texture2D cTexture)
	{
		InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, cTexture, null);
	}

	/// <summary>
	/// Initializes a new Sprite Particle System
	/// </summary>
	/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
	/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
	/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
	/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
	/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
	/// may also be adjusted manually at run-time.</param>
	/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
	/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
	/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
	/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
	/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
	/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
	/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
	/// may also be adjusted manually at run-time.</param>
	/// <param name="cTexture">The Texture to use to visualize the Particles</param>
	/// <param name="cSpriteBatchToDrawWith">The Sprite Batch that this particle system should use to draw its
	/// particles with.
	/// <para>If null, the particle system will use its own SpriteBatch to draw its particles.</para>
	/// <para>If not null, then you must call SpriteBatch.Begin() before calling ParticleSystem.Draw() to
	/// draw the particle system, and then call SpriteBatch.End() when done drawing the particle system.</para></param>
	public void InitializeSpriteParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, Texture2D cTexture, SpriteBatch cSpriteBatchToDrawWith)
	{
		try
		{
			InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.Sprite);
			Texture = cTexture;
			if (cSpriteBatchToDrawWith != null)
			{
				mcSpriteBatch = cSpriteBatchToDrawWith;
				mcSpriteBatchSettings = null;
				UsingExternalSpriteBatchToDrawParticles = true;
			}
		}
		catch (Exception ex)
		{
			ParticleType = ParticleTypes.None;
			throw new Exception("A problem occurred while Initializing the Particle System. Inner Exception: " + ex.ToString(), ex);
		}
		AfterInitialize();
	}

	/// <summary>
	/// Initializes a new Quad Particle System
	/// </summary>
	/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
	/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
	/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
	/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
	/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
	/// may also be adjusted manually at run-time.</param>
	/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
	/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
	/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
	/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
	/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
	/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
	/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
	/// may also be adjusted manually at run-time.</param>
	/// <param name="cVertexUpdateFunction">Function used to copy a Particle's drawable properties into the vertex buffer</param>
	public void InitializeQuadParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, UpdateVertexDelegate cVertexUpdateFunction)
	{
		try
		{
			InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.Quad);
			VertexUpdateFunction = cVertexUpdateFunction;
			Texture = null;
		}
		catch (Exception ex)
		{
			ParticleType = ParticleTypes.None;
			throw new Exception("A problem occurred while Initializing the Particle System. Inner Exception: " + ex.ToString(), ex);
		}
		AfterInitialize();
	}

	/// <summary>
	/// Initializes a new Textured Quad Particle System
	/// </summary>
	/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
	/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
	/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
	/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
	/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
	/// may also be adjusted manually at run-time.</param>
	/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
	/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
	/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
	/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
	/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
	/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
	/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
	/// may also be adjusted manually at run-time.</param>
	/// <param name="cVertexUpdateFunction">Function used to copy a Particle's drawable properties into the vertex buffer</param>
	/// <param name="sTexture">The asset name of the Texture to use to visualize the Particles</param>
	public void InitializeTexturedQuadParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, UpdateVertexDelegate cVertexUpdateFunction, string sTexture)
	{
		try
		{
			InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.TexturedQuad);
			VertexUpdateFunction = cVertexUpdateFunction;
			SetTexture(sTexture);
		}
		catch (Exception ex)
		{
			ParticleType = ParticleTypes.None;
			throw new Exception("A problem occurred while Initializing the Particle System. Inner Exception: " + ex.ToString(), ex);
		}
		AfterInitialize();
	}

	/// <summary>
	/// Initializes a new Textured Quad Particle System
	/// </summary>
	/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
	/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
	/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
	/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
	/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
	/// may also be adjusted manually at run-time.</param>
	/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
	/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
	/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
	/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
	/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
	/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
	/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
	/// may also be adjusted manually at run-time.</param>
	/// <param name="cVertexUpdateFunction">Function used to copy a Particle's drawable properties into the vertex buffer</param>
	/// <param name="cTexture">The Texture to use to visualize the Particles</param>
	public void InitializeTexturedQuadParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, UpdateVertexDelegate cVertexUpdateFunction, Texture2D cTexture)
	{
		try
		{
			InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.TexturedQuad);
			VertexUpdateFunction = cVertexUpdateFunction;
			Texture = cTexture;
		}
		catch (Exception ex)
		{
			ParticleType = ParticleTypes.None;
			throw new Exception("A problem occurred while Initializing the Particle System. Inner Exception: " + ex.ToString(), ex);
		}
		AfterInitialize();
	}

	/// <summary>
	/// Initialize the variables common to all Particle Systems
	/// </summary>
	/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
	/// <param name="cContentManager">Content Manager to use to load Effect files and Textures</param>
	/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
	/// be Allocated for. The Maximum Number Of Particles the Particle System should Allow is also
	/// set to this value initially.</param>
	/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
	/// Allowed at a single point in time. This should be less than or equal to the Number Of Particles 
	/// To Allocate Memory For if the Auto Memory Manager will not be used, as the Particle System 
	/// can only handle as many Particles as it has Memory Allocated For.</param>
	/// <param name="eParticleType">The Type of Particles this Particle System should draw</param>
	private void InitializeCommonVariables(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, ParticleTypes eParticleType)
	{
		Destroy();
		_numberOfParticleSystemsCurrentlyInitialized++;
		ParticleType = eParticleType;
		GraphicsDevice = cGraphicsDevice;
		ContentManager = cContentManager;
		if (SmcDPSFEffect == null && GraphicsDevice != null)
		{
			SmcDPSFEffect = new DPSFDefaultEffect(GraphicsDevice, DPSFDefaultEffect.DPSFDefaultEffectConfigurations.Xbox360HiDef);
		}
		SetDefaultEffect();
		Vertex val = default(Vertex);
		SetVertexDeclaration(val.SizeInBytes, val.VertexElements);
		miVertexSizeInBytes = val.SizeInBytes;
		if (ParticleType == ParticleTypes.Sprite)
		{
			mcSpriteBatch = new SpriteBatch(GraphicsDevice);
			mcSpriteBatchSettings = new SpriteBatchSettings();
		}
		mcRenderProperties = new RenderProperties();
		InitializeRenderProperties();
		NumberOfParticlesAllocatedInMemory = iNumberOfParticlesToAllocateMemoryFor;
		MaxNumberOfParticlesAllowed = iMaxNumberOfParticlesToAllow;
		Visible = true;
		Enabled = true;
		SimulationSpeed = 1f;
		InternalSimulationSpeed = 1f;
		UpdatesPerSecond = DPSFDefaultSettings.UpdatesPerSecond;
		mcRandom = new RandomNumbers();
		mcParticleEvents = new CParticleEvents();
		mcParticleSystemEvents = new CParticleSystemEvents();
		mcAutoMemoryManagerSettings = new AutoMemoryManagerSettings(DPSFDefaultSettings.AutoMemoryManagementSettings);
		PerformanceProfilingIsEnabled = DPSFDefaultSettings.PerformanceProfilingIsEnabled;
		mcEmitter = new ParticleEmitter();
		_lerpEmittersPositionAndOrientation = true;
		_emittersPreviousPosition = Vector3.Zero;
		_emittersPreviousOrientation = Quaternion.Identity;
		LerpEmittersPositionAndOrientationOnNextUpdate = false;
		ParticleSystemManagerToCopyPropertiesFrom = ParticleSystemManagerToCopyPropertiesFrom;
	}

	/// <summary>
	/// Release all resources used by the Particle System and reset all properties to their default values
	/// </summary>
	public void Destroy()
	{
		BeforeDestroy();
		if (meParticleType != ParticleTypes.None)
		{
			_numberOfParticleSystemsCurrentlyInitialized--;
		}
		meParticleType = ParticleTypes.None;
		mcGraphicsDevice = null;
		mcContentManager = null;
		mcParticleInitializationFunction = null;
		mcVertexUpdateFunction = null;
		mcRenderProperties = null;
		mcParticles = null;
		miNumberOfParticlesToDraw = 0;
		mcParticleVerticesToDraw = null;
		miaIndexBufferArray = null;
		msaIndexBufferReachArray = null;
		miIndexBufferIndex = 0;
		mcSpriteBatch = null;
		mcParticleSpritesToDraw = null;
		mcSpriteBatchSettings = null;
		if (mcActiveParticlesList != null)
		{
			mcActiveParticlesList.Clear();
		}
		mcActiveParticlesList = null;
		if (mcInactiveParticlesList != null)
		{
			mcInactiveParticlesList.Clear();
		}
		mcInactiveParticlesList = null;
		miMaxNumberOfParticlesAllowed = 0;
		mcVertexDeclaration = null;
		miVertexSizeInBytes = 0;
		mcEffect = null;
		mcTexture = null;
		DeserializationTexturePath = null;
		DeserializationEffectPath = null;
		DeserializationTechniqueName = null;
		Visible = false;
		Enabled = false;
		SimulationSpeed = 1f;
		InternalSimulationSpeed = 1f;
		mfTimeToWaitBetweenUpdates = 0f;
		mfTimeElapsedSinceLastUpdate = 0f;
		World = Matrix.Identity;
		View = Matrix.Identity;
		Projection = Matrix.Identity;
		mcRandom = null;
		mcParticleEvents = null;
		mcParticleSystemEvents = null;
		mcAutoMemoryManagerSettings = null;
		mcEmitter = null;
		_lerpEmittersPositionAndOrientation = true;
		_emittersPreviousPosition = Vector3.Zero;
		_emittersPreviousOrientation = Quaternion.Identity;
		LerpEmittersPositionAndOrientationOnNextUpdate = false;
		AfterDestroy();
	}

	public void InitializeNonSerializableProperties(Game cGame, GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
	{
		miID = _totalNumberOfParticleSystemsCreated++;
		mcGame = cGame;
		GraphicsDevice = cGraphicsDevice;
		ContentManager = cContentManager;
		VertexElement = default(Vertex).VertexElements;
		mcSpriteBatch = null;
		if (meParticleType == ParticleTypes.Sprite)
		{
			mcSpriteBatch = new SpriteBatch(GraphicsDevice);
		}
		if (meParticleType != ParticleTypes.NoDisplay && meParticleType != ParticleTypes.None)
		{
			if (!string.IsNullOrEmpty(DeserializationEffectPath) && !string.IsNullOrEmpty(DeserializationTechniqueName))
			{
				SetEffectAndTechnique(DeserializationEffectPath, DeserializationTechniqueName);
			}
			else
			{
				SetDefaultEffect();
			}
		}
		if (meParticleType == ParticleTypes.Sprite || meParticleType == ParticleTypes.TexturedQuad)
		{
			if (string.IsNullOrEmpty(DeserializationTexturePath))
			{
				throw new ArgumentNullException("DeserializationTexturePath", "The specified Texture to use is null. A valid Texture must be set to draw the current Type of Particles.");
			}
			SetTexture(DeserializationTexturePath);
		}
	}

	/// <summary>
	/// Sets the Graphics Device to use to the given graphics device.
	/// <para>NOTE: This only has an effect if the particle system does not inherit from DrawableGameComponent
	/// (i.e. InheritsDrawableGameComponent == false. i.e. using the DPSF.dll, not DPSFAsDrawableGameComponent.dll), since 
	/// the Graphics Device is read-only when inheriting from DrawableGameComponent. The Game object's Graphics Device
	/// is always used when inheriting from DrawableGameComponent.</para>
	/// </summary>
	/// <param name="graphicsDevice">The graphics device to use.</param>
	public void SetGraphicsDevice(GraphicsDevice graphicsDevice)
	{
		GraphicsDevice = graphicsDevice;
	}

	/// <summary>
	/// Set the World, View, and Projection matrices for this Particle System.
	/// <para>NOTE: Sprite particle systems are not affected by the World, View, and Projection matrices.</para>
	/// </summary>
	/// <param name="cWorld">The World matrix</param>
	/// <param name="cView">The View matrix</param>
	/// <param name="cProjection">The Projection matrix</param>
	public void SetWorldViewProjectionMatrices(Matrix cWorld, Matrix cView, Matrix cProjection)
	{
		World = cWorld;
		View = cView;
		Projection = cProjection;
	}

	/// <summary>
	/// Sets the vertex elements to use for each vertex of a particle.
	/// </summary>
	/// <param name="numberOfBytesPerVertex">The number of bytes per vertex.</param>
	/// <param name="elements">The vertex elements that make up the vertex.</param>
	private void SetVertexDeclaration(int numberOfBytesPerVertex, VertexElement[] elements)
	{
		if (numberOfBytesPerVertex > 0 && elements != null)
		{
			mcVertexDeclaration = new VertexDeclaration(numberOfBytesPerVertex, elements);
			return;
		}
		if (ParticleType == ParticleTypes.None || ParticleType == ParticleTypes.NoDisplay || ParticleType == ParticleTypes.Sprite)
		{
			mcVertexDeclaration = null;
			return;
		}
		throw new ArgumentNullException("VertexElement", "The specified Vertex Element is null. A valid Vertex Element is required to draw the current Type of Particles.");
	}

	/// <summary>
	/// Sets the Effect to be the default type for this type of particle system.
	/// This is done automatically when the particle system is initialized.
	/// <para>Default effect for each particle type is:</para>
	/// <list type="number">
	///     <item><description>NoDisplay and Sprite - null.</description></item>
	///     <item><description>Quad - BasicEffect.</description></item>
	///     <item><description>TexturedQuad - AlphaTestEffect.</description></item>
	/// </list>
	/// </summary>
	public void SetDefaultEffect()
	{
		switch (meParticleType)
		{
		case ParticleTypes.Quad:
			Effect = new BasicEffect(GraphicsDevice);
			break;
		case ParticleTypes.TexturedQuad:
			Effect = new AlphaTestEffect(GraphicsDevice);
			break;
		}
	}

	/// <summary>
	/// Sets the Effect and Technique to use to draw the Particles.
	/// <para>NOTE: This will automatically set the DeserializationEffectPath property to the given sEffect.</para>
	/// <para>NOTE: This will automatically set the DeserializationTechniqueName property to the given sTechnique.</para>
	/// </summary>
	/// <param name="sEffect">The Asset Name of the Effect to use</param>
	/// <param name="sTechnique">The name of the Effect's Technique to use</param>
	public void SetEffectAndTechnique(string sEffect, string sTechnique)
	{
		if (string.IsNullOrEmpty(sEffect))
		{
			throw new ArgumentNullException("sEffect", "The Effect string supplied is null or an empty string. The Effect to use cannot be null.");
		}
		Effect cEffect = ContentManager.Load<Effect>(sEffect);
		DeserializationEffectPath = sEffect;
		SetEffectAndTechnique(cEffect, sTechnique);
	}

	/// <summary>
	/// Sets the Effect and Technique to use to draw the Particles.
	/// <para>NOTE: This will automatically set the DeserializationTechniqueName property to the given sTechnique.</para>
	/// </summary>
	/// <param name="cEffect">The Effect to use</param>
	/// <param name="sTechnique">The name of the Effect's Technique to use</param>
	public void SetEffectAndTechnique(Effect cEffect, string sTechnique)
	{
		Effect = cEffect;
		SetTechnique(sTechnique);
	}

	/// <summary>
	/// Set which Technique of the current Effect to use to draw the Particles.
	/// <para>NOTE: This will automatically set the DeserializationTechniqueName property to the given sTechnique.</para>
	/// </summary>
	/// <param name="sTechnique">The name of the Effect's Technique to use</param>
	public void SetTechnique(string sTechnique)
	{
		if (mcEffect == null)
		{
			throw new InvalidOperationException("Effect is null when trying to specify the Technique to use. The Effect must be set before specifying the Technique.");
		}
		if (string.IsNullOrEmpty(sTechnique))
		{
			throw new ArgumentNullException("sTechnique", "The specified Technique to use is invalid. This parameter cannot be null or an empty string.");
		}
		mcEffect.CurrentTechnique = mcEffect.Techniques[sTechnique];
		DeserializationTechniqueName = sTechnique;
	}

	/// <summary>
	/// Set the Texture to use to draw the Particles
	/// </summary>
	/// <param name="sTexture">The Asset Name of the texture file to use (found in
	/// the XNA Properties of the file)</param>
	public void SetTexture(string sTexture)
	{
		if (string.IsNullOrEmpty(sTexture))
		{
			if (ParticleType == ParticleTypes.Sprite || ParticleType == ParticleTypes.TexturedQuad)
			{
				throw new ArgumentNullException("sTexture", "Specified Texture to use is null. A valid Texture must be set to draw the current Type of Particles.");
			}
			Texture = null;
			DeserializationTexturePath = null;
			return;
		}
		try
		{
			mcTexture = ContentManager.Load<Texture2D>(sTexture);
			mcTexture.Name = sTexture;
			DeserializationTexturePath = sTexture;
		}
		catch (KeyNotFoundException innerException)
		{
			throw new InvalidOperationException("There was a problem loading the texture \"" + sTexture + "\". Did you Dispose() this resource earlier somewhere else by accident?", innerException);
		}
	}

	/// <summary>
	/// This allocates the proper amount of space for the Particles and initializes the variables used to draw the Type of Particles specified. 
	/// For example, if using Textured Quads extra space will need to be allocated to hold the Particles, as each Quad Particle requires four 
	/// vertices, not one like Point Sprites. Also, the Index Buffer would be initialized, as it is required to draw Quads.
	/// </summary>
	private bool InitializeParticleArrays()
	{
		if (mcParticles == null)
		{
			return false;
		}
		int num = mcParticles.Length;
		switch (meParticleType)
		{
		case ParticleTypes.Quad:
		case ParticleTypes.TexturedQuad:
			mcParticleVerticesToDraw = new Vertex[num * 4];
			if (GraphicsDevice.GraphicsProfile == GraphicsProfile.HiDef)
			{
				miaIndexBufferArray = new int[num * 6];
			}
			else
			{
				msaIndexBufferReachArray = new short[num * 6];
			}
			miMaxParticlesThatXboxCanDrawAtOnce = 524287 / (miVertexSizeInBytes * 4);
			break;
		case ParticleTypes.Sprite:
			mcParticleSpritesToDraw = new Particle[num];
			break;
		}
		int num2 = mcParticles.Length;
		for (int i = 0; i < num2; i++)
		{
			mcParticles[i] = new Particle();
			mcInactiveParticlesList.AddFirst(mcParticles[i]);
		}
		if (mcParticleVerticesToDraw != null)
		{
			int num3 = mcParticleVerticesToDraw.Length;
			for (int j = 0; j < num3; j++)
			{
				mcParticleVerticesToDraw[j] = default(Vertex);
			}
		}
		miNumberOfParticlesToDraw = 0;
		return true;
	}

	/// <summary>
	/// Initialize the given Particle using the current Initialization Function
	/// </summary>
	/// <param name="cParticle">The Particle to Initialize</param>
	public void InitializeParticle(Particle cParticle)
	{
		mcParticleInitializationFunction(cParticle);
	}

	/// <summary>
	/// Adds a new Particle to the particle system, at the start of the Active Particle List. 
	/// This new Particle is initialized using the particle system's Particle Initialization Function
	/// </summary>
	/// <returns>True if a particle was added, False if there is not enough memory for another Particle</returns>
	public bool AddParticle()
	{
		return AddParticle(null);
	}

	/// <summary>
	/// Adds a new Particle to the particle system, at the start of the Active Particle List. Returns true if
	/// the Particle was added, false if there is not enough memory for another Particle.
	/// </summary>
	/// <param name="cParticleToCopy">The Particle to add to the Particle System. If this is null then a
	/// new Particle is initialized using the particle system's Particle Initialization Function</param>
	/// <returns>True if a particle was added, False if there is not enough memory for another Particle</returns>
	public bool AddParticle(Particle cParticleToCopy)
	{
		if (mcActiveParticlesList.Count >= miMaxNumberOfParticlesAllowed)
		{
			return false;
		}
		if (mcInactiveParticlesList.Count <= 0)
		{
			if (AutoMemoryManagerSettings.MemoryManagementMode != AutoMemoryManagerModes.IncreaseAndDecrease && AutoMemoryManagerSettings.MemoryManagementMode != AutoMemoryManagerModes.IncreaseOnly)
			{
				return false;
			}
			int num = (int)Math.Ceiling((float)NumberOfParticlesAllocatedInMemory * AutoMemoryManagerSettings.IncreaseAmount);
			num = (int)MathHelper.Min(num, MaxNumberOfParticlesAllowed);
			NumberOfParticlesAllocatedInMemory = num;
		}
		BeforeAddParticle();
		LinkedListNode<Particle> last = mcInactiveParticlesList.Last;
		mcInactiveParticlesList.RemoveLast();
		Particle value = last.Value;
		value.Reset();
		if (cParticleToCopy == null)
		{
			if (mcParticleInitializationFunction != null)
			{
				mcParticleInitializationFunction(value);
			}
		}
		else
		{
			value.CopyFrom(cParticleToCopy);
		}
		mcActiveParticlesList.AddFirst(last);
		AfterAddParticle();
		return true;
	}

	/// <summary>
	/// Adds the specified number of new Particles to the particle system. 
	/// These new Particles are initialized using the particle systems Particle Initialization Function
	/// </summary>
	/// <param name="iNumberOfParticlesToAdd">How many Particles to Add to the particle system</param>
	/// <returns>Returns how many Particles were able to be added to the particle system</returns>
	public int AddParticles(int iNumberOfParticlesToAdd)
	{
		return AddParticles(iNumberOfParticlesToAdd, null);
	}

	/// <summary>
	/// Adds the specified number of new Particles to the particle system, copying the 
	/// properties of the given Particle To Copy
	/// </summary>
	/// <param name="iNumberOfParticlesToAdd">How many copyies of the Particle To Copy to Add 
	/// to the particle system</param>
	/// <param name="cParticleToCopy">The Particle to copy from when Adding the Particles to the 
	/// Particle System. If this is null then the new Particles will be initialized using the 
	/// particle system's Particle Initialization Function</param>
	/// <returns>Returns how many Particles were able to be added to the particle system</returns>
	public int AddParticles(int iNumberOfParticlesToAdd, Particle cParticleToCopy)
	{
		int i;
		for (i = 0; i < iNumberOfParticlesToAdd; i++)
		{
			if (!AddParticle(cParticleToCopy))
			{
				break;
			}
		}
		return i;
	}

	/// <summary>
	/// Removes all Active Particles from the Active Particle List and adds them 
	/// to the Inactive Particle List
	/// </summary>
	public void RemoveAllParticles()
	{
		while (mcActiveParticlesList.Count > 0)
		{
			LinkedListNode<Particle> first = mcActiveParticlesList.First;
			mcActiveParticlesList.Remove(first);
			mcInactiveParticlesList.AddFirst(first);
		}
	}

	/// <summary>
	/// Updates the Particle System. This involves executing the Particle System
	/// Events, updating all Active Particles according to the Particle Events, and 
	/// adding new Particles according to the Emitter settings.
	/// <para>NOTE: This will only Update the Particle System if it does not inherit from DrawableGameComponent, 
	/// since if it does it will be updated automatically by the Game object.</para>
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">How much time in seconds has 
	/// elapsed since the last time this function was called</param>
	public void Update(float fElapsedTimeInSeconds)
	{
		Update(fElapsedTimeInSeconds, bCalledByDrawableGameComponent: false);
	}

	/// <summary>
	/// Updates the Particle System, even if the the Particle Systems inherits from DrawableGameComponent.
	/// <para>Updating the Particle System involves executing the Particle System Events, updating all Active 
	/// Particles according to the Particle Events, and adding new Particles according to the Emitter settings.</para>
	/// <para>NOTE: If inheriting from DrawableGameComponent and this is called, the Particle System will be updated
	/// twice per frame; once when it is called here, and again when automatically called by the Game object.
	/// If not inheriting from DrawableGameComponent, this acts the same as calling Update().</para>
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">How much time in seconds has 
	/// elapsed since the last time this function was called</param>
	public void UpdateForced(float fElapsedTimeInSeconds)
	{
		Update(fElapsedTimeInSeconds, bCalledByDrawableGameComponent: true);
	}

	/// <summary>
	/// Updates the Particle System. This involves executing the Particle System
	/// Events, updating all Active Particles according to the Particle Events, and 
	/// adding new Particles according to the Emitter's settings.
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">How much time in seconds has 
	/// elapsed since the last time this function was called</param>
	/// <param name="bCalledByDrawableGameComponent">Indicates if this function was
	/// called manually by the user or called automatically by the Drawable Game Component.
	/// If this function Inherits Drawable Game Component, but was not called by
	/// Drawable Game Component, nothing will be updated since the Particle System will
	/// automatically be updated when the Game Component's Update() function is called.</param>
	private void Update(float fElapsedTimeInSeconds, bool bCalledByDrawableGameComponent)
	{
		if (!IsInitialized || !Enabled || (InheritsDrawableGameComponent && !bCalledByDrawableGameComponent))
		{
			return;
		}
		mfTimeElapsedSinceLastUpdate += fElapsedTimeInSeconds;
		if (mfTimeElapsedSinceLastUpdate < mfTimeToWaitBetweenUpdates)
		{
			return;
		}
		if (_performanceProfilingIsEnabled)
		{
			_performanceProfilingStopwatch.Reset();
			_performanceProfilingStopwatch.Start();
		}
		mfTimeElapsedSinceLastUpdate -= mfTimeToWaitBetweenUpdates;
		float num = 0f;
		if (mfTimeToWaitBetweenUpdates <= 0f)
		{
			num = fElapsedTimeInSeconds;
		}
		else if (mfTimeElapsedSinceLastUpdate >= mfTimeToWaitBetweenUpdates)
		{
			num = mfTimeElapsedSinceLastUpdate + mfTimeToWaitBetweenUpdates;
			mfTimeElapsedSinceLastUpdate = 0f;
		}
		else
		{
			num = mfTimeToWaitBetweenUpdates;
		}
		float num2 = num * SimulationSpeed * InternalSimulationSpeed;
		BeforeUpdate(num2);
		miNumberOfParticlesToDraw = 0;
		miIndexBufferIndex = 0;
		int num3 = 0;
		ParticleSystemEvents.Update(num2);
		if (!IsInitialized)
		{
			return;
		}
		int num4 = mcEmitter.UpdateAndGetNumberOfParticlesToEmit(num2);
		if (!IsInitialized)
		{
			return;
		}
		if (!LerpEmittersPositionAndOrientationOnNextUpdate)
		{
			_emittersPreviousPosition = mcEmitter.PositionData.Position;
			_emittersPreviousOrientation = mcEmitter.OrientationData.Orientation;
			LerpEmittersPositionAndOrientationOnNextUpdate = true;
		}
		if (num4 > 0)
		{
			Vector3 position = mcEmitter.PositionData.Position;
			Quaternion orientation = mcEmitter.OrientationData.Orientation;
			if (!LerpEmittersPositionAndOrientation)
			{
				_emittersPreviousPosition = position;
				_emittersPreviousOrientation = orientation;
			}
			float num5 = 1f / (float)num4;
			int num6 = 0;
			bool flag = true;
			while (num6 < num4 && flag)
			{
				num6++;
				mcEmitter.PositionData.Position = Vector3.Lerp(_emittersPreviousPosition, position, num5 * (float)num6);
				mcEmitter.OrientationData.Orientation = Quaternion.Slerp(_emittersPreviousOrientation, orientation, num5 * (float)num6);
				flag = AddParticle();
				if (flag)
				{
					Particle value = ActiveParticles.First.Value;
					float fElapsedTimeInSeconds2 = MathHelper.Lerp(num2, 0f, num5 * (float)num6);
					value.UpdateElapsedTimeVariables(fElapsedTimeInSeconds2);
					ParticleEvents.Update(value, fElapsedTimeInSeconds2);
					if (!value.IsActive())
					{
						ActiveParticles.RemoveFirst();
						continue;
					}
					AddParticleToVertexBuffer(value);
					num3++;
				}
			}
			mcEmitter.PositionData.Position = position;
			mcEmitter.OrientationData.Orientation = orientation;
		}
		_emittersPreviousPosition = mcEmitter.PositionData.Position;
		_emittersPreviousOrientation = mcEmitter.OrientationData.Orientation;
		LinkedListNode<Particle> linkedListNode = mcActiveParticlesList.First;
		for (int i = 0; i < num3; i++)
		{
			linkedListNode = linkedListNode.Next;
		}
		while (linkedListNode != null)
		{
			Particle value2 = linkedListNode.Value;
			value2.UpdateElapsedTimeVariables(num2);
			ParticleEvents.Update(value2, num2);
			if (!value2.IsActive())
			{
				LinkedListNode<Particle> node = linkedListNode;
				linkedListNode = linkedListNode.Next;
				mcActiveParticlesList.Remove(node);
				mcInactiveParticlesList.AddFirst(node);
			}
			else
			{
				AddParticleToVertexBuffer(value2);
				linkedListNode = linkedListNode.Next;
			}
		}
		ParticleEvents.RemoveAllOneTimeEvents();
		if (AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.IncreaseAndDecrease || AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.DecreaseOnly)
		{
			mfAutoMemoryManagersElapsedTime += fElapsedTimeInSeconds;
			if (NumberOfActiveParticles > miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds)
			{
				miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds = NumberOfActiveParticles;
				mfAutoMemoryManagersElapsedTime = 0f;
			}
			if (mfAutoMemoryManagersElapsedTime >= AutoMemoryManagerSettings.SecondsMaxNumberOfParticlesMustExistForBeforeReducingSize)
			{
				int num7 = (int)Math.Ceiling((float)miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds * AutoMemoryManagerSettings.ReduceAmount);
				num7 = (int)MathHelper.Max(num7, AutoMemoryManagerSettings.AbsoluteMinNumberOfParticles);
				if (num7 < NumberOfParticlesAllocatedInMemory)
				{
					NumberOfParticlesAllocatedInMemory = num7;
				}
				miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds = 0;
			}
		}
		if ((double)ParticleSystemEvents.LifetimeData.NormalizedElapsedTime >= 1.0 && ParticleSystemEvents.LifetimeData.EndOfLifeOption == CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Destroy)
		{
			Destroy();
		}
		AfterUpdate(num2);
		if (_performanceProfilingIsEnabled)
		{
			_performanceProfilingStopwatch.Stop();
			PerformanceTimeToDoUpdateInMilliseconds = _performanceProfilingStopwatch.Elapsed.TotalMilliseconds;
		}
	}

	/// <summary>
	/// Adds the given Particle to the list of Particles to be Drawn (i.e. the Vertex Buffer), if it is Visible
	/// </summary>
	/// <param name="cParticle">The Particle to add to the Vertex Buffer</param>
	private void AddParticleToVertexBuffer(Particle cParticle)
	{
		if (cParticle.Visible)
		{
			switch (meParticleType)
			{
			case ParticleTypes.Quad:
			case ParticleTypes.TexturedQuad:
				mcVertexUpdateFunction(ref mcParticleVerticesToDraw, miNumberOfParticlesToDraw * 4, cParticle);
				break;
			case ParticleTypes.Sprite:
				mcParticleSpritesToDraw[miNumberOfParticlesToDraw] = cParticle;
				break;
			}
			miNumberOfParticlesToDraw++;
		}
	}

	/// <summary>
	/// Draws all of the Active Particles to the Graphics Device.
	/// <para>NOTE: This will only Draw the Particle System if it does not inherit from DrawableGameComponent, 
	/// since if it does it will be drawn automatically by the Game object.</para>
	/// </summary>
	public void Draw()
	{
		Draw(bCalledByDrawableGameComponent: false);
	}

	/// <summary>
	/// Draws all of the Active Particles to the Graphics Device, even if the the Particle Systems inherits
	/// from DrawableGameComponent.
	/// <para>NOTE: If inheriting from DrawableGameComponent and this is called, the Particle System will be drawn
	/// twice per frame; once when it is called here, and again when automatically called by the Game object.
	/// If not inheriting from DrawableGameComponent, this acts the same as calling Draw().</para>
	/// </summary>
	public void DrawForced()
	{
		Draw(bCalledByDrawableGameComponent: true);
	}

	/// <summary>
	/// Draws all of the Active Particles to the Graphics Device
	/// </summary>
	/// <param name="bCalledByDrawableGameComponent">Indicates if this function was
	/// called manually by the user or called automatically by the Drawable Game Component.
	/// If this function Inherits Drawable Game Component, but was not called by
	/// Drawable Game Component, nothing will be drawn since the Particle System will
	/// automatically be drawn when the Game Component's Draw() function is called.</param>
	private void Draw(bool bCalledByDrawableGameComponent)
	{
		if (!IsInitialized || !Visible || (InheritsDrawableGameComponent && !bCalledByDrawableGameComponent))
		{
			return;
		}
		if (_performanceProfilingIsEnabled)
		{
			_performanceProfilingStopwatch.Reset();
			_performanceProfilingStopwatch.Start();
		}
		ClearRenderStates();
		BeforeDraw();
		if (miNumberOfParticlesToDraw <= 0 || ParticleType == ParticleTypes.NoDisplay)
		{
			AfterDraw();
			if (_performanceProfilingIsEnabled)
			{
				_performanceProfilingStopwatch.Stop();
				PerformanceTimeToDoDrawInMilliseconds = _performanceProfilingStopwatch.Elapsed.TotalMilliseconds;
			}
			return;
		}
		if (meParticleType == ParticleTypes.Sprite && (Effect == null || Technique == null))
		{
			if (!UsingExternalSpriteBatchToDrawParticles)
			{
				mcSpriteBatch.Begin(SpriteBatchSettings.SortMode, RenderProperties.BlendState, RenderProperties.SamplerState, RenderProperties.DepthStencilState, RenderProperties.RasterizerState, Effect, SpriteBatchSettings.TransformationMatrix);
			}
			for (int i = 0; i < miNumberOfParticlesToDraw; i++)
			{
				DrawSprite(mcParticleSpritesToDraw[i], mcSpriteBatch);
			}
			if (!UsingExternalSpriteBatchToDrawParticles)
			{
				mcSpriteBatch.End();
			}
		}
		else
		{
			ApplyRenderState();
			SetEffectParameters();
			int count = mcEffect.CurrentTechnique.Passes.Count;
			for (int j = 0; j < count; j++)
			{
				EffectPass effectPass = mcEffect.CurrentTechnique.Passes[j];
				if (meParticleType == ParticleTypes.Sprite && !UsingExternalSpriteBatchToDrawParticles)
				{
					mcSpriteBatch.Begin(SpriteBatchSettings.SortMode, RenderProperties.BlendState, RenderProperties.SamplerState, RenderProperties.DepthStencilState, RenderProperties.RasterizerState, Effect, SpriteBatchSettings.TransformationMatrix);
				}
				effectPass.Apply();
				try
				{
					switch (meParticleType)
					{
					case ParticleTypes.Quad:
					case ParticleTypes.TexturedQuad:
					{
						int num = miNumberOfParticlesToDraw;
						int num2 = 0;
						int num3 = 0;
						while (num > 0)
						{
							int num4 = Math.Min(num, miMaxParticlesThatXboxCanDrawAtOnce);
							int primitiveCount = num4 * 2;
							int num5 = num4 * 4;
							GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, mcParticleVerticesToDraw, num2, num5, miaIndexBufferArray, num3, primitiveCount, mcVertexDeclaration);
							num2 += num5;
							num3 += num4 * 6;
							num -= num4;
						}
						break;
					}
					case ParticleTypes.Sprite:
					{
						for (int k = 0; k < miNumberOfParticlesToDraw; k++)
						{
							DrawSprite(mcParticleSpritesToDraw[k], mcSpriteBatch);
						}
						if (!UsingExternalSpriteBatchToDrawParticles)
						{
							mcSpriteBatch.End();
						}
						break;
					}
					case (ParticleTypes)4:
						break;
					}
				}
				catch (InvalidOperationException ex)
				{
					int num6 = miVertexSizeInBytes * miNumberOfParticlesToDraw;
					if (meParticleType == ParticleTypes.Quad || meParticleType == ParticleTypes.TexturedQuad)
					{
						num6 *= 4;
					}
					throw new Exception("Not enough video memory to draw the particle system. The XBox 360 can only allocate about " + 524287.ToString("###,###,###") + " bytes in video memory when using DrawUserPrimitives(). You are trying to allocate " + num6.ToString("###,###,###") + " bytes.\n\nInner exception: " + ex.ToString(), ex);
				}
			}
		}
		AfterDraw();
		if (_performanceProfilingIsEnabled)
		{
			_performanceProfilingStopwatch.Stop();
			PerformanceTimeToDoDrawInMilliseconds = _performanceProfilingStopwatch.Elapsed.TotalMilliseconds;
		}
	}

	/// <summary>
	/// Resets all of the Sampler States and Vertex Sampler States on the Graphics Device.
	/// This must be done before any rendering to prevent an XNA 4 bug that causes the graphics device
	/// to incorrectly track state information, which manifests itself as run-time errors.
	/// https://connect.microsoft.com/site226/feedback/details/586216/cloned-effect-?wa=wsignin1.0
	/// </summary>
	private void ClearRenderStates()
	{
		if (GraphicsDevice == null)
		{
			return;
		}
		for (int i = 0; i < 16; i++)
		{
			GraphicsDevice.SamplerStates[i] = SamplerState.PointClamp;
		}
		if (GraphicsDevice.GraphicsProfile == GraphicsProfile.HiDef)
		{
			for (int j = 0; j < 2; j++)
			{
				GraphicsDevice.VertexSamplerStates[j] = SamplerState.PointWrap;
			}
		}
	}

	/// <summary>
	/// Applies the Particle System's Render State properties to the Graphics Device.
	/// </summary>
	private void ApplyRenderState()
	{
		GraphicsDevice.BlendState = RenderProperties.BlendState;
		GraphicsDevice.DepthStencilState = RenderProperties.DepthStencilState;
		GraphicsDevice.RasterizerState = RenderProperties.RasterizerState;
		GraphicsDevice.SamplerStates[0] = RenderProperties.SamplerState;
	}

	/// <summary>
	/// Virtual function to Initialize the Particle System with default values.
	/// Particle system properties should not be set until after this is called, as 
	/// they are likely to be reset to their default values.
	/// </summary>
	/// <param name="cGraphicsDevice">The Graphics Device the Particle System should use</param>
	/// <param name="cContentManager">The Content Manager the Particle System should use to load resources</param>
	/// <param name="cSpriteBatch">The Sprite Batch that the Sprite Particle System should use to draw its particles.
	/// If this is not initializing a Sprite particle system, or you want the particle system to use its own Sprite Batch,
	/// pass in null.</param>
	public virtual void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
	{
	}

	/// <summary>
	/// Sets the Camera Position of the particle system, so that the particles know how to make themselves face the camera if needed.
	/// This virtual function does not do anything unless overridden, and all it should do is set an internal Vector3 variable
	/// (e.g. public Vector3 CameraPosition { get; set; }) to match the given Vector3.
	/// </summary>
	/// <param name="cameraPosition">The position that the camera is currently at.</param>
	public virtual void SetCameraPosition(Vector3 cameraPosition)
	{
	}

	/// <summary>
	/// Virtual function to draw a Sprite Particle. This function should be used to draw the given
	/// Particle with the provided SpriteBatch.
	/// </summary>
	/// <param name="Particle">The Particle Sprite to Draw</param>
	/// <param name="cSpriteBatch">The SpriteBatch to use to doing the Drawing</param>
	protected virtual void DrawSprite(DPSFParticle Particle, SpriteBatch cSpriteBatch)
	{
	}

	/// <summary>
	/// Virtual function that is called at the end of the Initialize() function.
	/// This may be used to perform operations after the Particle System has been Initialized, such as 
	/// initializing other Particle Systems nested within this Particle System.
	/// </summary>
	protected virtual void AfterInitialize()
	{
	}

	/// <summary>
	/// Virtual function that is called at the beginning of the Destroy() function.
	/// This may be used to perform operations before the Destroy() code is executed.
	/// </summary>
	protected virtual void BeforeDestroy()
	{
	}

	/// <summary>
	/// Virtual function that is called at the end of the Destroy() function.
	/// This may be used to perform operations after the Particle System has been Destroyed, such as 
	/// to destroy other Particle Systems nested within this Particle System.
	/// </summary>
	protected virtual void AfterDestroy()
	{
	}

	/// <summary>
	/// Virtual function that is called at the beginning of the Update() function.
	/// This may be used to perform operations before the Update() code is executed.
	/// </summary>
	protected virtual void BeforeUpdate(float fElapsedTimeInSeconds)
	{
	}

	/// <summary>
	/// Virtual function that is called at the end of the Update() function.
	/// This may be used to perform operations after the Particle System has been updated, such as 
	/// to Update Particle Systems nested within this Particle System.
	/// </summary>
	protected virtual void AfterUpdate(float fElapsedTimeInSeconds)
	{
	}

	/// <summary>
	/// Virtual function that is called at the beginning of the Draw() function.
	/// This may be used to perform operations before the Draw() code is executed.
	/// </summary>
	protected virtual void BeforeDraw()
	{
	}

	/// <summary>
	/// Virtual function that is called at the end of the Draw() function.
	/// This may be used to perform operations after the Particle System has been drawn, such as 
	/// to Draw Particle Systems nested within this Particle System.
	/// </summary>
	protected virtual void AfterDraw()
	{
	}

	/// <summary>
	/// Virtual function that is called at the beginning of the AddParticle() function.
	/// This may be used to execute some code before a new Particle is initialized and added.
	/// </summary>
	protected virtual void BeforeAddParticle()
	{
	}

	/// <summary>
	/// Virtual function that is called at the end of the AddParticle() function.
	/// This may be used to execute some code after a new Particle is initialized and added.
	/// </summary>
	protected virtual void AfterAddParticle()
	{
	}

	/// <summary>
	/// Virtual function to setup the Render Properties (i.e. BlendState, DepthStencilState, RasterizerState, and SamplerState)
	/// which will be applied to the Graphics Device before drawing the Particle System's Particles.
	/// <para>This function is only called once when initializing the particle system.</para>
	/// </summary>
	protected virtual void InitializeRenderProperties()
	{
	}

	/// <summary>
	/// Virtual function to Set the Effect's Parameters before drawing the Particles.
	/// <para>This is called every time before the particle system is drawn.</para>
	/// </summary>
	protected virtual void SetEffectParameters()
	{
	}
}
