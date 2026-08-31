using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// Class to manage the Updating and Drawing of DPSF Particle Systems each frame
/// </summary>
public class ParticleSystemManager
{
	private List<IDPSFParticleSystem> mcParticleSystemListSortedByUpdateOrder = new List<IDPSFParticleSystem>();

	private List<IDPSFParticleSystem> mcParticleSystemListSortedByDrawOrder = new List<IDPSFParticleSystem>();

	private bool mbPerformUpdates = true;

	private bool mbPerfomDraws = true;

	private bool mbAParticleSystemsUpdateOrderWasChanged;

	private bool mbAParticleSystemsDrawOrderWasChanged;

	private float mfSimulationSpeed = 1f;

	private bool mbUseManagersSimulationSpeed = true;

	private int miUpdatesPerSecond;

	private bool mbUseManagersUpdatesPerSecond = true;

	/// <summary>
	/// Handle to the particle system whose Update() function is currently being called.
	/// We need this in case a PS removes itself from the Manager during its Update() function.
	/// </summary>
	private IDPSFParticleSystem _particleSystemBeingUpdated;

	private bool _isParticleSystemBeingUpdatedRemovedFromManager;

	/// <summary>
	/// Get if the Particle Systems are inheriting from DrawableGameComponent or not
	/// </summary>
	public bool ParticleSystemsInheritDrawableGameComponent => false;

	/// <summary>
	/// Get / Set if the Particle Systems should be Updated or not
	/// </summary>
	public bool Enabled
	{
		get
		{
			return mbPerformUpdates;
		}
		set
		{
			mbPerformUpdates = true;
		}
	}

	/// <summary>
	/// Get / Set if this Particle Systems should be drawn or not.
	/// <para>NOTE: Setting this to false causes the particle systems' Draw() function to not be called, including the 
	/// particle systems' BeforeDraw() and AfterDraw() functions.</para>
	/// </summary>
	public bool Visible
	{
		get
		{
			return mbPerfomDraws;
		}
		set
		{
			mbPerfomDraws = value;
		}
	}

	/// <summary>
	/// Get / Set if the Particle System Manager's SimulationSpeed property
	/// should be used for each of the particle systems it contains or not.
	/// <para>Default value is true.</para>
	/// </summary>
	public bool SimulationSpeedIsEnabled
	{
		get
		{
			return mbUseManagersSimulationSpeed;
		}
		set
		{
			mbUseManagersSimulationSpeed = value;
			if (mbUseManagersSimulationSpeed)
			{
				SimulationSpeed = SimulationSpeed;
			}
		}
	}

	/// <summary>
	/// Get / Set if the Particle System Manager's UpdatesPerSecond property
	/// should be used for each of the particle systems it contains or not.
	/// <para>Default value is true.</para>
	/// </summary>
	public bool UpdatesPerSecondIsEnabled
	{
		get
		{
			return mbUseManagersUpdatesPerSecond;
		}
		set
		{
			mbUseManagersUpdatesPerSecond = value;
			if (mbUseManagersUpdatesPerSecond)
			{
				UpdatesPerSecond = UpdatesPerSecond;
			}
		}
	}

	/// <summary>
	/// Get / Set how fast the Particle System Simulations should run. 
	/// <para>Example: 1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.</para>
	/// <para>NOTE: This sets the SimulationSpeed property of each individual Particle
	/// System in this Manager to the given value. It will also set a particle system's
	/// Simulation Speed when the particle system is re-initialized, 
	/// and when a new Particle System is added to the Manager in the future.</para>
	/// <para>NOTE: Setting this property only has an effect if the SimulationSpeedIsEnabled property is true.</para>
	/// <para>NOTE: This will be set to zero if a negative value is specified.</para>
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
			if (mbUseManagersSimulationSpeed)
			{
				SetSimulationSpeedForAllParticleSystems(mfSimulationSpeed);
			}
		}
	}

	/// <summary>
	/// Get / Set how often the Particle Systems should be Updated. 
	/// <para>NOTE: This sets the UpdatesPerSecond property of each individual Particle
	/// System in this Manager to the given value. It will also set a particle system's
	/// Updates Per Second when the particle system is re-initialized, 
	/// and when a new Particle System is added to the Manager in the future.</para>
	/// <para>NOTE: Setting this property only has an effect if the UpdatesPerSecondIsEnabled property is true.</para>
	/// <para>NOTE: A value of zero means update the particle systems every time Update() is called.</para>
	/// <para>NOTE: This will be set to zero if a negative value is specified.</para>
	/// </summary>
	public int UpdatesPerSecond
	{
		get
		{
			return miUpdatesPerSecond;
		}
		set
		{
			if (value < 0)
			{
				miUpdatesPerSecond = 0;
			}
			else
			{
				miUpdatesPerSecond = value;
			}
			if (mbUseManagersUpdatesPerSecond)
			{
				SetUpdatesPerSecondForAllParticleSystems(miUpdatesPerSecond);
			}
		}
	}

	/// <summary>
	/// Get the cumulative Number Of Active Particles of all Particle Systems in this Manager
	/// </summary>
	public int TotalNumberOfActiveParticles
	{
		get
		{
			int num = 0;
			int count = mcParticleSystemListSortedByUpdateOrder.Count;
			for (int i = 0; i < count; i++)
			{
				num += mcParticleSystemListSortedByUpdateOrder[i].NumberOfActiveParticles;
			}
			return num;
		}
	}

	/// <summary>
	/// Get the cumulative Number Of Particles Being Drawn by all Particle Systems in this Manager.
	/// This is the total number of Active AND Visible Particles.
	/// <para>NOTE: This ignores whether the Manager is Visible or not.</para>
	/// </summary>
	public int TotalNumberOfParticlesBeingDrawn
	{
		get
		{
			int num = 0;
			int count = mcParticleSystemListSortedByUpdateOrder.Count;
			for (int i = 0; i < count; i++)
			{
				num += mcParticleSystemListSortedByUpdateOrder[i].NumberOfParticlesBeingDrawn;
			}
			return num;
		}
	}

	/// <summary>
	/// Get the cumulative Max Number Of Particles allocated in memory by all Particle Systems in the Manager.
	/// </summary>
	public int TotalNumberOfParticlesAllocatedInMemory
	{
		get
		{
			int num = 0;
			int count = mcParticleSystemListSortedByUpdateOrder.Count;
			for (int i = 0; i < count; i++)
			{
				num += mcParticleSystemListSortedByUpdateOrder[i].NumberOfParticlesAllocatedInMemory;
			}
			return num;
		}
	}

	/// <summary>
	/// Gets the cumulative time (in milliseconds) it took to perform the Update() function on each particle system in this manager.
	/// <para>Note: Only particle systems that have their PerformanceProfilingIsEnabled property set to true will be included in this total.</para>
	/// </summary>
	public double TotalPerformanceTimeToDoUpdatesInMilliseconds { get; private set; }

	/// <summary>
	/// Gets the cumulative time (in milliseconds) it took to perform the Draw() function on each particle system in this manager.
	/// <para>Note: Only particle systems that have their PerformanceProfilingIsEnabled property set to true will be included in this total.</para>
	/// </summary>
	public double TotalPerformanceTimeToDoDrawsInMilliseconds { get; private set; }

	/// <summary>
	/// Returns a Linked List of handles to the Particle Systems in this Manager
	/// </summary>
	public List<IDPSFParticleSystem> ParticleSystems => mcParticleSystemListSortedByUpdateOrder;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:DPSF.ParticleSystemManager" /> class.
	/// </summary>
	public ParticleSystemManager()
	{
		UpdatesPerSecond = DPSFDefaultSettings.UpdatesPerSecond;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:DPSF.ParticleSystemManager" /> class, copying the settings of the given Particle System Manager.
	/// </summary>
	/// <param name="managerToCopy">The Particle System Manager to copy from.</param>
	public ParticleSystemManager(ParticleSystemManager managerToCopy)
		: this()
	{
		CopyFrom(managerToCopy);
	}

	/// <summary>
	/// Copies the given DPSF Particle System Manager's information into this Manager.
	/// </summary>
	/// <param name="cManagerToCopy">The Particle System Manager to copy from.</param>
	public void CopyFrom(ParticleSystemManager cManagerToCopy)
	{
		mbPerformUpdates = cManagerToCopy.mbPerformUpdates;
		mbPerfomDraws = cManagerToCopy.mbPerfomDraws;
		mbAParticleSystemsUpdateOrderWasChanged = cManagerToCopy.mbAParticleSystemsUpdateOrderWasChanged;
		mbAParticleSystemsDrawOrderWasChanged = cManagerToCopy.mbAParticleSystemsDrawOrderWasChanged;
		mfSimulationSpeed = cManagerToCopy.mfSimulationSpeed;
		mbUseManagersSimulationSpeed = cManagerToCopy.mbUseManagersSimulationSpeed;
		miUpdatesPerSecond = cManagerToCopy.miUpdatesPerSecond;
		mbUseManagersUpdatesPerSecond = cManagerToCopy.mbUseManagersUpdatesPerSecond;
		int count = cManagerToCopy.ParticleSystems.Count;
		for (int i = 0; i < count; i++)
		{
			AddParticleSystem(cManagerToCopy.ParticleSystems[i]);
		}
	}

	/// <summary>
	/// Sets each individual Particle Systems' Simulation Speed to the specified Simulation Speed.
	/// </summary>
	/// <param name="fSimulationSpeed">The new Simulation Speed that all Particle Systems 
	/// currently in this Manager should have</param>
	public void SetSimulationSpeedForAllParticleSystems(float fSimulationSpeed)
	{
		IDPSFParticleSystem iDPSFParticleSystem = null;
		int count = mcParticleSystemListSortedByUpdateOrder.Count;
		for (int i = 0; i < count; i++)
		{
			iDPSFParticleSystem = mcParticleSystemListSortedByUpdateOrder[i];
			if (iDPSFParticleSystem.IsInitialized)
			{
				iDPSFParticleSystem.SimulationSpeed = fSimulationSpeed;
			}
		}
	}

	/// <summary>
	/// Sets each individual Particle Systems' Updates Per Second to the specified Updates Per Second.
	/// </summary>
	/// <param name="iUpdatesPerSecond">The new Updates Per Second that all particle systems
	/// currently in this Manager should have</param>
	public void SetUpdatesPerSecondForAllParticleSystems(int iUpdatesPerSecond)
	{
		IDPSFParticleSystem iDPSFParticleSystem = null;
		int count = mcParticleSystemListSortedByUpdateOrder.Count;
		for (int i = 0; i < count; i++)
		{
			iDPSFParticleSystem = mcParticleSystemListSortedByUpdateOrder[i];
			if (iDPSFParticleSystem.IsInitialized)
			{
				iDPSFParticleSystem.UpdatesPerSecond = iUpdatesPerSecond;
			}
		}
	}

	/// <summary>
	/// Sets the PerformanceProfilingIsEnabled property of all particle systems in this manager to the given value.
	/// </summary>
	/// <param name="performanceProfilingIsEnabled">Set if Performance Profiling should be enabled or not.</param>
	public void SetPerformanceProfilingIsEnabledForAllParticleSystems(bool performanceProfilingIsEnabled)
	{
		IDPSFParticleSystem iDPSFParticleSystem = null;
		int count = mcParticleSystemListSortedByUpdateOrder.Count;
		for (int i = 0; i < count; i++)
		{
			iDPSFParticleSystem = mcParticleSystemListSortedByUpdateOrder[i];
			if (iDPSFParticleSystem.IsInitialized)
			{
				iDPSFParticleSystem.PerformanceProfilingIsEnabled = performanceProfilingIsEnabled;
			}
		}
	}

	/// <summary>
	/// Sets the World, View, and Projection Matrices for all of the Particle Systems in this Manager.
	/// <para>NOTE: Sprite particle systems are not affected by the World, View, and Projection matrices.</para>
	/// </summary>
	/// <param name="cWorld">The World Matrix</param>
	/// <param name="cView">The View Matrix</param>
	/// <param name="cProjection">The Projection Matrix</param>
	public void SetWorldViewProjectionMatricesForAllParticleSystems(Matrix cWorld, Matrix cView, Matrix cProjection)
	{
		IDPSFParticleSystem iDPSFParticleSystem = null;
		int count = mcParticleSystemListSortedByUpdateOrder.Count;
		for (int i = 0; i < count; i++)
		{
			iDPSFParticleSystem = mcParticleSystemListSortedByUpdateOrder[i];
			if (iDPSFParticleSystem.IsInitialized)
			{
				iDPSFParticleSystem.SetWorldViewProjectionMatrices(cWorld, cView, cProjection);
			}
		}
	}

	/// <summary>
	/// Sets the SpriteBatchSettings.TransformationMatrix for all Sprite Particle Systems in this Manager.
	/// </summary>
	/// <param name="sTransformationMatrix">The Transformation Matrix to apply to the Sprite Particle Systems</param>
	public void SetTransformationMatrixForAllSpriteParticleSystems(Matrix sTransformationMatrix)
	{
		IDPSFParticleSystem iDPSFParticleSystem = null;
		int count = mcParticleSystemListSortedByUpdateOrder.Count;
		for (int i = 0; i < count; i++)
		{
			iDPSFParticleSystem = mcParticleSystemListSortedByUpdateOrder[i];
			if (iDPSFParticleSystem.ParticleType == ParticleTypes.Sprite && iDPSFParticleSystem.IsInitialized)
			{
				iDPSFParticleSystem.SpriteBatchSettings.TransformationMatrix = sTransformationMatrix;
			}
		}
	}

	/// <summary>
	/// Sets the CameraPosition property of all particle systems in this manager to the given Camera Position.
	/// This is done by calling the particle system's virtual SetCameraPosition() function.
	/// </summary>
	/// <param name="cameraPosition">The current position of the Camera.</param>
	public void SetCameraPositionForAllParticleSystems(Vector3 cameraPosition)
	{
		IDPSFParticleSystem iDPSFParticleSystem = null;
		int count = mcParticleSystemListSortedByUpdateOrder.Count;
		for (int i = 0; i < count; i++)
		{
			iDPSFParticleSystem = mcParticleSystemListSortedByUpdateOrder[i];
			if (iDPSFParticleSystem.IsInitialized)
			{
				iDPSFParticleSystem.SetCameraPosition(cameraPosition);
			}
		}
	}

	/// <summary>
	/// Returns true if the given Particle System is in the Manager, false if not.
	/// </summary>
	/// <param name="cParticleSystemToFind">The Particle System to look for</param>
	/// <returns>Returns true if the given Particle System is in the Manager, false if not.</returns>
	public bool ContainsParticleSystem(IDPSFParticleSystem cParticleSystemToFind)
	{
		return mcParticleSystemListSortedByUpdateOrder.Contains(cParticleSystemToFind);
	}

	/// <summary>
	/// Returns true if the Particle System with the given ID is in the Manager, false if not.
	/// </summary>
	/// <param name="iIDOfParticleSystemToFind">The ID of the Particle System to find</param>
	/// <returns>Returns true if the Particle System with the given ID is in the Manager, false if not.</returns>
	public bool ContainsParticleSystem(int iIDOfParticleSystemToFind)
	{
		int count = mcParticleSystemListSortedByUpdateOrder.Count;
		for (int i = 0; i < count; i++)
		{
			if (mcParticleSystemListSortedByUpdateOrder[i].ID == iIDOfParticleSystemToFind)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Add an initialized Particle System to the Particle System Manager.
	/// <para>NOTE: This sets the Particle System's ParticleSystemManagerToCopyPropertiesFrom
	/// property to this Particle System Manager.</para>
	/// </summary>
	/// <param name="cParticleSystemToAdd">The initialized Particle System to add</param>
	public void AddParticleSystem(IDPSFParticleSystem cParticleSystemToAdd)
	{
		if (cParticleSystemToAdd == null)
		{
			throw new ArgumentNullException("cParticleSystemToAdd", "A particle system with a value of null cannot be added to the particle system manager.");
		}
		mcParticleSystemListSortedByUpdateOrder.Add(cParticleSystemToAdd);
		mcParticleSystemListSortedByDrawOrder.Add(cParticleSystemToAdd);
		cParticleSystemToAdd.UpdateOrderChanged += ParticleSystem_UpdateOrderChanged;
		cParticleSystemToAdd.DrawOrderChanged += ParticleSystem_DrawOrderChanged;
		cParticleSystemToAdd.ParticleSystemManagerToCopyPropertiesFrom = this;
		SortParticleSystemLists();
	}

	/// <summary>
	/// Removes the specified Particle System from the Particle System Manager.
	/// Returns true if the Particle System was found and removed, false if it was not found.
	/// </summary>
	/// <param name="cParticleSystemToRemove">A handle to the Particle System to Remove</param>
	/// <returns>Returns true if the Particle System was found and removed, false if it was not found.</returns>
	public bool RemoveParticleSystem(IDPSFParticleSystem cParticleSystemToRemove)
	{
		if (cParticleSystemToRemove == null)
		{
			throw new ArgumentNullException("cParticleSystemToRemove", "A particle system with a value of null cannot be removed from the particle system manager.");
		}
		if (cParticleSystemToRemove == _particleSystemBeingUpdated)
		{
			_isParticleSystemBeingUpdatedRemovedFromManager = true;
		}
		cParticleSystemToRemove.UpdateOrderChanged -= ParticleSystem_UpdateOrderChanged;
		cParticleSystemToRemove.DrawOrderChanged -= ParticleSystem_DrawOrderChanged;
		mcParticleSystemListSortedByUpdateOrder.Remove(cParticleSystemToRemove);
		return mcParticleSystemListSortedByDrawOrder.Remove(cParticleSystemToRemove);
	}

	/// <summary>
	/// Removes the specified Particle System from the Particle System Manager.
	/// Returns true if the Particle System was found and removed, false if it was not found.
	/// </summary>
	/// <param name="iIDOfParticleSystemToRemove">The ID of the Particle System to Remove</param>
	/// <returns>Returns true if the Particle System was found and removed, false if it was not found.</returns>
	public bool RemoveParticleSystem(int iIDOfParticleSystemToRemove)
	{
		IDPSFParticleSystem iDPSFParticleSystem = null;
		int count = mcParticleSystemListSortedByUpdateOrder.Count;
		for (int i = 0; i < count; i++)
		{
			iDPSFParticleSystem = mcParticleSystemListSortedByUpdateOrder[i];
			if (iDPSFParticleSystem.ID == iIDOfParticleSystemToRemove)
			{
				return RemoveParticleSystem(iDPSFParticleSystem);
			}
		}
		return false;
	}

	/// <summary>
	/// Removes all Particle Systems from the Particle System Manager
	/// </summary>
	public void RemoveAllParticleSystems()
	{
		int count = mcParticleSystemListSortedByUpdateOrder.Count;
		for (int num = count - 1; num >= 0; num--)
		{
			RemoveParticleSystem(mcParticleSystemListSortedByUpdateOrder[num]);
		}
		mcParticleSystemListSortedByUpdateOrder.Clear();
		mcParticleSystemListSortedByDrawOrder.Clear();
	}

	/// <summary>
	/// Calls the AutoInitialize() function for every Particle System in this Manager
	/// </summary>
	/// <param name="cGraphicsDevice">The Graphics Device that the Particle Systems should be drawn to</param>
	/// <param name="cContentManager">The Content Manager used to load Effect files and Textures</param>
	/// <param name="cSpriteBatch">The Sprite Batch that the Sprite Particle System should use to draw its particles.
	/// If this is not initializing a Sprite particle system, or you want the particle system to use its own Sprite Batch,
	/// pass in null.</param>
	public void AutoInitializeAllParticleSystems(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
	{
		int count = mcParticleSystemListSortedByUpdateOrder.Count;
		for (int i = 0; i < count; i++)
		{
			mcParticleSystemListSortedByUpdateOrder[i].AutoInitialize(cGraphicsDevice, cContentManager, cSpriteBatch);
		}
	}

	/// <summary>
	/// Calls the Destroy() function for every Particle System in this Manager
	/// </summary>
	public void DestroyAllParticleSystems()
	{
		int count = mcParticleSystemListSortedByUpdateOrder.Count;
		for (int i = 0; i < count; i++)
		{
			mcParticleSystemListSortedByUpdateOrder[i].Destroy();
		}
	}

	/// <summary>
	/// Destroys each Particle System in the Manager, then removes them from the Manager
	/// </summary>
	public void DestroyAndRemoveAllParticleSystems()
	{
		DestroyAllParticleSystems();
		RemoveAllParticleSystems();
	}

	/// <summary>
	/// Updates all of the Particle Systems.
	/// <para>NOTE: This will only Update the Particle Systems if they do not inherit from DrawableGameComponent, 
	/// since if they do they will be updated automatically by the Game object.</para>
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">The amount of Time in seconds that has passed since
	/// the last Update</param>
	public void UpdateAllParticleSystems(float fElapsedTimeInSeconds)
	{
		if (!ParticleSystemsInheritDrawableGameComponent)
		{
			UpdateAllParticleSystemsForced(fElapsedTimeInSeconds);
		}
	}

	/// <summary>
	/// Updates all of the Particle Systems.
	/// <para>NOTE: If the Particle Systems inherit from DrawableGameComponent and this is called, the Particle
	/// Systems will be updated twice each frame; once here and once when called automatically by the game object.
	/// If not inheriting from DrawableGameComponent, this function acts the same as calling UpdateAllParticleSystems().</para>
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">The amount of Time in seconds that has passed since
	/// the last Update</param>
	public void UpdateAllParticleSystemsForced(float fElapsedTimeInSeconds)
	{
		if (!Enabled)
		{
			return;
		}
		if (mbAParticleSystemsUpdateOrderWasChanged)
		{
			SortParticleSystemsByUpdateOrderList();
			mbAParticleSystemsUpdateOrderWasChanged = false;
		}
		TotalPerformanceTimeToDoUpdatesInMilliseconds = 0.0;
		ResetParticleSystemBeingUpdatedVariables();
		int count = mcParticleSystemListSortedByUpdateOrder.Count;
		for (int i = 0; i < count; i++)
		{
			_particleSystemBeingUpdated = mcParticleSystemListSortedByUpdateOrder[i];
			if (_particleSystemBeingUpdated.IsInitialized)
			{
				_particleSystemBeingUpdated.Update(fElapsedTimeInSeconds);
				TotalPerformanceTimeToDoUpdatesInMilliseconds += _particleSystemBeingUpdated.PerformanceTimeToDoUpdateInMilliseconds;
				count = mcParticleSystemListSortedByUpdateOrder.Count;
				if (_isParticleSystemBeingUpdatedRemovedFromManager)
				{
					i--;
					_isParticleSystemBeingUpdatedRemovedFromManager = false;
				}
			}
		}
		ResetParticleSystemBeingUpdatedVariables();
	}

	/// <summary>
	/// Resets the variables used for determining if a PS removed itself from the PS Manager during its Update() function call.
	/// </summary>
	private void ResetParticleSystemBeingUpdatedVariables()
	{
		_particleSystemBeingUpdated = null;
		_isParticleSystemBeingUpdatedRemovedFromManager = false;
	}

	/// <summary>
	/// Draws all of the Particle Systems.
	/// <para>NOTE: This will only Draw the Particle Systems if they do not inherit from DrawableGameComponent, 
	/// since if they do they will be drawn automatically by the Game object.</para>
	/// </summary>
	public void DrawAllParticleSystems()
	{
		if (!ParticleSystemsInheritDrawableGameComponent)
		{
			DrawAllParticleSystemsForced();
		}
	}

	/// <summary>
	/// Draws all of the Particle Systems, even if they inherit from DrawableGameComponent.
	/// <para>NOTE: If the Particle Systems inherit from DrawableGameComponent and this is called, the Particle
	/// Systems will be drawn twice each frame; once here and once when called automatically by the game object.
	/// If not inheriting from DrawableGameComponent, this function acts the same as calling DrawAllParticleSystems().</para>
	/// </summary>
	public void DrawAllParticleSystemsForced()
	{
		if (!Visible)
		{
			return;
		}
		if (mbAParticleSystemsDrawOrderWasChanged)
		{
			SortParticleSystemsByDrawOrderList();
			mbAParticleSystemsDrawOrderWasChanged = false;
		}
		TotalPerformanceTimeToDoDrawsInMilliseconds = 0.0;
		IDPSFParticleSystem iDPSFParticleSystem = null;
		int count = mcParticleSystemListSortedByDrawOrder.Count;
		for (int i = 0; i < count; i++)
		{
			iDPSFParticleSystem = mcParticleSystemListSortedByDrawOrder[i];
			if (iDPSFParticleSystem.IsInitialized)
			{
				iDPSFParticleSystem.DrawForced();
				TotalPerformanceTimeToDoDrawsInMilliseconds += iDPSFParticleSystem.PerformanceTimeToDoDrawInMilliseconds;
			}
		}
	}

	/// <summary>
	/// Draws all of the Particle Systems to a Texture and returns the Texture, which has a Transparent Black background
	/// </summary>
	/// <param name="cGraphicsDevice">A Graphics Device to use for drawing; The Graphics Device contents will not be overwritten.
	/// <para>NOTE: The size of the Texture before scaling will be the size of the Graphics Device's Viewport.</para></param>
	/// <param name="iTextureWidth">The desired Width of the Texture</param>
	/// <param name="iTextureHeight">The desired Height of the Texture</param>
	/// <returns>Returns a Texture with the Particle Systems in their current state drawn on it</returns>
	public Texture2D DrawAllParticleSystemsToTexture(GraphicsDevice cGraphicsDevice, int iTextureWidth, int iTextureHeight)
	{
		Texture2D texture2D = null;
		int width = cGraphicsDevice.Viewport.Width;
		int height = cGraphicsDevice.Viewport.Height;
		RenderTarget2D graphicsDevicesCurrentRenderTarget = GetGraphicsDevicesCurrentRenderTarget(cGraphicsDevice);
		RenderTarget2D renderTarget2D = new RenderTarget2D(cGraphicsDevice, width, height);
		cGraphicsDevice.SetRenderTarget(renderTarget2D);
		cGraphicsDevice.Clear(Color.Transparent);
		IDPSFParticleSystem iDPSFParticleSystem = null;
		int count = mcParticleSystemListSortedByDrawOrder.Count;
		for (int i = 0; i < count; i++)
		{
			iDPSFParticleSystem = mcParticleSystemListSortedByDrawOrder[i];
			if (iDPSFParticleSystem.IsInitialized)
			{
				RenderTarget2D graphicsDevicesCurrentRenderTarget2 = GetGraphicsDevicesCurrentRenderTarget(iDPSFParticleSystem.GraphicsDevice);
				iDPSFParticleSystem.GraphicsDevice.SetRenderTarget(renderTarget2D);
				iDPSFParticleSystem.DrawForced();
				iDPSFParticleSystem.GraphicsDevice.SetRenderTarget(graphicsDevicesCurrentRenderTarget2);
			}
		}
		cGraphicsDevice.SetRenderTarget(graphicsDevicesCurrentRenderTarget);
		Texture2D texture2D2 = renderTarget2D;
		if (width == iTextureWidth && height == iTextureHeight)
		{
			return texture2D2;
		}
		DepthStencilState depthStencilState = cGraphicsDevice.DepthStencilState;
		RenderTarget2D renderTarget2D2 = new RenderTarget2D(cGraphicsDevice, iTextureWidth, iTextureHeight);
		cGraphicsDevice.SetRenderTarget(renderTarget2D2);
		cGraphicsDevice.DepthStencilState = DepthStencilState.Default;
		cGraphicsDevice.Clear(Color.Transparent);
		SpriteBatch spriteBatch = new SpriteBatch(cGraphicsDevice);
		spriteBatch.Begin();
		spriteBatch.Draw(texture2D2, new Rectangle(0, 0, iTextureWidth, iTextureHeight), Color.White);
		spriteBatch.End();
		cGraphicsDevice.SetRenderTarget(graphicsDevicesCurrentRenderTarget);
		cGraphicsDevice.DepthStencilState = depthStencilState;
		return renderTarget2D2;
	}

	/// <summary>
	/// Draws the given Texture to the given Tile Set Render Target at the specified Position.
	/// </summary>
	/// <param name="cGraphicsDevice">The Graphics Device used to do the drawing</param>
	/// <param name="cTileSetRenderTarget">The Tile Set Render Target to draw to</param>
	/// <param name="cTexture">The Texture to draw</param>
	/// <param name="sPositionAndDimensionsInTileSetToAddImage">The Position where the Texture should be drawn
	/// on the Tile Set Render Target, and its Dimensions</param>
	private void AddImageToTileSet(GraphicsDevice cGraphicsDevice, ref RenderTarget2D cTileSetRenderTarget, Texture2D cTexture, Rectangle sPositionAndDimensionsInTileSetToAddImage)
	{
		RenderTarget2D graphicsDevicesCurrentRenderTarget = GetGraphicsDevicesCurrentRenderTarget(cGraphicsDevice);
		DepthStencilState depthStencilState = cGraphicsDevice.DepthStencilState;
		cGraphicsDevice.SetRenderTarget(cTileSetRenderTarget);
		cGraphicsDevice.DepthStencilState = DepthStencilState.Default;
		SpriteBatch spriteBatch = new SpriteBatch(cGraphicsDevice);
		spriteBatch.Begin();
		spriteBatch.Draw(cTexture, sPositionAndDimensionsInTileSetToAddImage, Color.White);
		spriteBatch.End();
		cGraphicsDevice.SetRenderTarget(graphicsDevicesCurrentRenderTarget);
		cGraphicsDevice.DepthStencilState = depthStencilState;
	}

	/// <summary>
	/// Gets the graphics device's current render target, or null if it is not set.
	/// </summary>
	/// <param name="graphicsDevice">The graphics device.</param>
	/// <returns></returns>
	private RenderTarget2D GetGraphicsDevicesCurrentRenderTarget(GraphicsDevice graphicsDevice)
	{
		RenderTargetBinding[] renderTargets = graphicsDevice.GetRenderTargets();
		if (renderTargets.Length <= 0)
		{
			return null;
		}
		return (RenderTarget2D)renderTargets[0].RenderTarget;
	}

	/// <summary>
	/// Sort the two Particle System Lists
	/// </summary>
	private void SortParticleSystemLists()
	{
		SortParticleSystemsByUpdateOrderList();
		SortParticleSystemsByDrawOrderList();
	}

	/// <summary>
	/// Sorts the Particle System List Sorted By Update Order
	/// </summary>
	private void SortParticleSystemsByUpdateOrderList()
	{
		mcParticleSystemListSortedByUpdateOrder.Sort((IDPSFParticleSystem cPS1, IDPSFParticleSystem cPS2) => cPS1.UpdateOrder.CompareTo(cPS2.UpdateOrder));
	}

	/// <summary>
	/// Sorts the Particle System List Sorted By Draw Order
	/// </summary>
	private void SortParticleSystemsByDrawOrderList()
	{
		mcParticleSystemListSortedByDrawOrder.Sort((IDPSFParticleSystem cPS1, IDPSFParticleSystem cPS2) => cPS1.DrawOrder.CompareTo(cPS2.DrawOrder));
	}

	/// <summary>
	/// Records that the Particle Systems need to be resorted before doing the next Updates
	/// </summary>
	/// <param name="sender">The Object that sent the event</param>
	/// <param name="e">Extra information</param>
	private void ParticleSystem_UpdateOrderChanged(object sender, EventArgs e)
	{
		mbAParticleSystemsUpdateOrderWasChanged = true;
	}

	/// <summary>
	/// Records that the Particle Systems need to be resorted before doing the next Draws
	/// </summary>
	/// <param name="sender">The Object that sent the event</param>
	/// <param name="e">Extra information</param>
	private void ParticleSystem_DrawOrderChanged(object sender, EventArgs e)
	{
		mbAParticleSystemsDrawOrderWasChanged = true;
	}
}
