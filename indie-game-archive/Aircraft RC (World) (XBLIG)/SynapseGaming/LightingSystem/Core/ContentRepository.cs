using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Xna.Framework.Content;
using R;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides storage of automatically loaded game content. This includes
/// models, light maps, and light occlusion buffers loaded with scenes
/// and during rendering.
///
/// Content repositories must be loaded via a content manager before
/// scenes and other objects referencing their contents are loaded.
/// </summary>
[Serializable]
public class ContentRepository : IFullSerializable, ISerializable, INamedObject, IDisposable
{
	/// <summary>
	/// Used internally.
	/// </summary>
	[Serializable]
	public class BaseAssetData : IFullSerializable, ISerializable
	{
		/// <summary />
		public string PipelineImporterClassName = string.Empty;

		/// <summary />
		public string PipelineProcessorClassName = string.Empty;

		/// <summary>
		/// Name is the class PropertyName, Value is "val.ToString()".
		/// </summary>
		public Dictionary<string, string> PipelineProcessorOptions = new Dictionary<string, string>();

		internal virtual PipelineAssetType PipelineAssetType => PipelineAssetType.None;

		/// <summary>
		/// Deserializes object data from the provided SerializationInfo.
		/// </summary>
		/// <param name="info">Contains the serialized object data.</param>
		/// <param name="context"></param>
		public void SetObjectData(SerializationInfo info, StreamingContext context)
		{
			PipelineProcessorOptions.Clear();
			SerializationHelper.DeserializeField(ref PipelineImporterClassName, info, "PipelineImporterClassName", usedefault: true);
			SerializationHelper.DeserializeField(ref PipelineProcessorClassName, info, "PipelineProcessorClassName", usedefault: true);
			SerializationHelper.DeserializeField(ref PipelineProcessorOptions, info, "PipelineProcessorOptions", usedefault: false);
		}

		/// <summary>
		/// Serializes object data to the provided SerializationInfo.
		/// </summary>
		/// <param name="info">SerializationInfo to store the serialized data.</param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			SerializationHelper.SerializeFieldOrEnum(ref PipelineImporterClassName, info, "PipelineImporterClassName");
			SerializationHelper.SerializeFieldOrEnum(ref PipelineProcessorClassName, info, "PipelineProcessorClassName");
			SerializationHelper.SerializeFieldOrEnum(ref PipelineProcessorOptions, info, "PipelineProcessorOptions");
		}

		internal void m(ContentReader P_0)
		{
			PipelineImporterClassName = P_0.ReadString();
			PipelineProcessorClassName = P_0.ReadString();
			PipelineProcessorOptions = P_0.ReadObject<Dictionary<string, string>>();
		}
	}

	/// <summary>
	/// Used internally.
	/// </summary>
	[Serializable]
	public class SoundEffectData : BaseAssetData
	{
		internal override PipelineAssetType PipelineAssetType => PipelineAssetType.Sound;
	}

	/// <summary>
	/// Used internally.
	/// </summary>
	[Serializable]
	public class ModelData : BaseAssetData
	{
		internal List<string> HCB = new List<string>();

		internal override PipelineAssetType PipelineAssetType => PipelineAssetType.Model;

		internal ModelData()
		{
		}

		internal ModelData(List<string> P_0)
		{
			HCB = P_0;
		}
	}

	/// <summary>
	/// Relative path to the light map cache directory.
	/// </summary>
	public const string LightMapCachePath = "LightMapCache\\";

	private static ContentRepository HCB;

	private static Dictionary<string, ContentRepository> HC_0002 = new Dictionary<string, ContentRepository>(4);

	private string HC_0012 = "";

	private string HCH = "";

	private string HC7 = "";

	private string HC_0001 = "";

	private ProcessorRenderingType HCw;

	private ContentManager HCZ;

	private Dictionary<string, ModelData> HC_000F = new Dictionary<string, ModelData>();

	private Dictionary<string, SoundEffectData> HCy = new Dictionary<string, SoundEffectData>();

	private List<string> HC6 = new List<string>();

	private List<string> HCD = new List<string>();

	private Dictionary<string, PrefabObjectGenerator> HC_0011 = new Dictionary<string, PrefabObjectGenerator>();

	private Dictionary<string, string> HCK = new Dictionary<string, string>();

	/// <summary>
	/// The object's current name.
	/// </summary>
	public string Name
	{
		get
		{
			return HC_0012;
		}
		set
		{
		}
	}

	internal string FileName
	{
		get
		{
			return HCH;
		}
		set
		{
			HCH = hCH;
		}
	}

	internal string XnbContentManagerFileName
	{
		get
		{
			return HC7;
		}
		set
		{
			HC7 = hC;
		}
	}

	internal string ProjectFile
	{
		get
		{
			return HC_0001;
		}
		set
		{
			HC_0001 = text;
		}
	}

	internal ProcessorRenderingType ProcessorRenderingType
	{
		get
		{
			return HCw;
		}
		set
		{
			HCw = hCw;
		}
	}

	/// <summary>
	/// Gets the default content repository. This may be null if no content repositories are loaded.
	/// </summary>
	public static ContentRepository DefaultContentRepository => HCB;

	/// <summary>
	/// List of all content repositories.
	/// </summary>
	public static Dictionary<string, ContentRepository> ContentRepositories => HC_0002;

	internal Dictionary<string, ModelData> Models => HC_000F;

	internal Dictionary<string, SoundEffectData> SoundEffects => HCy;

	internal List<string> LightMaps => HC6;

	internal List<string> LightOcclusionBuffers => HCD;

	internal Dictionary<string, PrefabObjectGenerator> Prefabs => HC_0011;

	private static void _5(ContentRepository P_0)
	{
		string name = P_0.Name;
		if (HC_0002.ContainsKey(name))
		{
			throw new Exception($"Content repository named '{name}' already exists.");
		}
		HC_0002.Add(name, P_0);
		if (HCB == null)
		{
			HCB = P_0;
		}
	}

	private static void _3(ContentRepository P_0)
	{
		HC_0002.Remove(P_0.Name);
		if (HCB != P_0)
		{
			return;
		}
		HCB = null;
		using Dictionary<string, ContentRepository>.Enumerator enumerator = HC_0002.GetEnumerator();
		if (enumerator.MoveNext())
		{
			HCB = enumerator.Current.Value;
		}
	}

	/// <summary>
	/// Finds a content repository by name.
	/// </summary>
	/// <param name="contentrepositoryname">Name of the content manager to find.</param>
	/// <returns></returns>
	public static ContentRepository Find(string contentrepositoryname)
	{
		if (string.IsNullOrEmpty(contentrepositoryname))
		{
			return null;
		}
		if (HC_0002.TryGetValue(contentrepositoryname, out var value))
		{
			return value;
		}
		return null;
	}

	internal ContentRepository(string P_0, ContentManager P_1)
	{
		HC_0012 = P_0;
		HCZ = P_1;
		_5(this);
	}

	/// <summary>
	/// Only for serialization. Using this constructor in game code may cause an exception.
	/// </summary>
	public ContentRepository()
	{
	}

	/// <summary>
	/// Disposes the content repository. This removes it from the list of available repositories.
	/// </summary>
	public void Dispose()
	{
		_3(this);
	}

	internal void t(string P_0, string P_1, ModelData P_2)
	{
		if (!HCK.ContainsKey(P_0))
		{
			HC_000F.Add(P_0, P_2);
			HCK.Add(P_0, P_1);
		}
	}

	internal void I(string P_0)
	{
		HC_000F.Remove(P_0);
		HCK.Remove(P_0);
	}

	internal void Q(string P_0, string P_1, SoundEffectData P_2)
	{
		if (!HCK.ContainsKey(P_0))
		{
			HCy.Add(P_0, P_2);
			HCK.Add(P_0, P_1);
		}
	}

	internal void _0016(string P_0)
	{
		HCy.Remove(P_0);
		HCK.Remove(P_0);
	}

	internal void v(string P_0, string P_1)
	{
		if (!HCK.ContainsKey(P_0))
		{
			HC6.Add(P_0);
			HCK.Add(P_0, P_1);
		}
	}

	internal void _2(string P_0, string P_1)
	{
		if (!HCK.ContainsKey(P_0))
		{
			HCD.Add(P_0);
			HCK.Add(P_0, P_1);
		}
	}

	internal void _0005(string P_0, PrefabObjectGenerator P_1)
	{
		if (HC_0011.ContainsKey(P_0))
		{
			HC_0011[P_0] = P_1;
		}
		else
		{
			HC_0011.Add(P_0, P_1);
		}
	}

	internal string _4(int P_0)
	{
		return string.Format("{0}{1}.lm", "LightMapCache\\", P_0);
	}

	internal string x(int P_0)
	{
		return string.Format("{0}{1}.om", "LightMapCache\\", P_0);
	}

	/// <summary>
	/// Loads the light map associated with a RenderableMesh.
	/// </summary>
	/// <param name="mesh"></param>
	/// <returns></returns>
	public LightMap LoadLightMap(RenderableMesh mesh)
	{
		return LoadBySourceAssetPath<LightMap>(_4(mesh.HC_0012), allownull: true);
	}

	/// <summary>
	/// Loads the light occlusion buffer associated with a directional light.
	/// </summary>
	/// <param name="light"></param>
	/// <returns></returns>
	public LightOcclusionBuffer LoadLightOcclusionBuffer(ILight light)
	{
		return LoadBySourceAssetPath<LightOcclusionBuffer>(x(light.UniqueId), allownull: true);
	}

	/// <summary>
	/// Loads a prefab by name. Prefabs are created in the SunBurn editor.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public PrefabObjectGenerator LoadPrefab(string name)
	{
		if (HC_0011.TryGetValue(name, out var value))
		{
			return value;
		}
		return null;
	}

	/// <summary>
	/// Loads an asset using the relative source path. The path includes
	/// the original file extension.
	///
	/// For instance: "models\\chair.fbx"
	/// </summary>
	/// <typeparam name="T">Type of returned class.</typeparam>
	/// <param name="sourceassetpath">Asset relative source path.</param>
	/// <param name="allownull">Determines if an exception should
	/// be thrown when the asset does not exist.</param>
	/// <returns></returns>
	public T LoadBySourceAssetPath<T>(string sourceassetpath, bool allownull)
	{
		string text = string.Empty;
		foreach (KeyValuePair<string, string> item in HCK)
		{
			if (item.Key.Equals(sourceassetpath, StringComparison.InvariantCultureIgnoreCase))
			{
				text = item.Value;
				break;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			if (allownull)
			{
				return default(T);
			}
			throw new Exception($"Asset with source path '{sourceassetpath}' does not exist in the repository.");
		}
		return Load<T>(text);
	}

	/// <summary>
	/// Loads an asset using the xna style path. The path does not include
	/// the file extension.
	///
	/// For instance: "models\\chair"
	/// </summary>
	/// <typeparam name="T">Type of returned class.</typeparam>
	/// <param name="xnbassetpath">Asset path.</param>
	/// <returns></returns>
	public T Load<T>(string xnbassetpath)
	{
		if (string.IsNullOrEmpty(xnbassetpath))
		{
			return default(T);
		}
		return HCZ.Load<T>(xnbassetpath);
	}

	/// <summary>
	/// Removes all objects from the container. Commonly used while clearing the scene.
	/// </summary>
	public void Clear()
	{
		HC_000F.Clear();
		HCy.Clear();
		HCK.Clear();
		HC6.Clear();
		HCD.Clear();
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		HC_000F = R._7._7_0006(info);
		SerializationHelper.DeserializeField(ref HCy, info, "SoundEffects", usedefault: true);
		if (HCy == null)
		{
			HCy = new Dictionary<string, SoundEffectData>();
		}
		SerializationHelper.DeserializeField(ref HC6, info, "LightMaps", usedefault: false);
		SerializationHelper.DeserializeField(ref HCD, info, "LightOcclusionBuffers", usedefault: false);
		SerializationHelper.DeserializeField(ref HC_0011, info, "Prefabs", usedefault: false);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializationHelper.SerializeFieldOrEnum(ref HC_000F, info, "Models");
		SerializationHelper.SerializeFieldOrEnum(ref HCy, info, "SoundEffects");
		SerializationHelper.SerializeFieldOrEnum(ref HC6, info, "LightMaps");
		SerializationHelper.SerializeFieldOrEnum(ref HCD, info, "LightOcclusionBuffers");
		SerializationHelper.SerializeFieldOrEnum(ref HC_0011, info, "Prefabs");
	}
}
