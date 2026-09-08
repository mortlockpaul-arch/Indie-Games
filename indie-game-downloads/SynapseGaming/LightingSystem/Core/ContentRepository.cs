using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using _0016;
using Microsoft.Xna.Framework.Content;
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

		internal void A(ContentReader P_0)
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
		internal List<string> _3A_0018 = new List<string>();

		internal override PipelineAssetType PipelineAssetType => PipelineAssetType.Model;

		internal ModelData()
		{
		}

		internal ModelData(List<string> P_0)
		{
			_3A_0018 = P_0;
		}
	}

	/// <summary>
	/// Relative path to the light map cache directory.
	/// </summary>
	public const string LightMapCachePath = "LightMapCache\\";

	private static ContentRepository _3A_0018;

	private static Dictionary<string, ContentRepository> _3AL = new Dictionary<string, ContentRepository>(4);

	private string _3A_0019 = "";

	private string _3A3 = "";

	private string _3A6 = "";

	private string _3AD = "";

	private ProcessorRenderingType _3A_0017;

	private ContentManager _3A_0003;

	private Dictionary<string, ModelData> _3Al = new Dictionary<string, ModelData>();

	private Dictionary<string, SoundEffectData> _3At = new Dictionary<string, SoundEffectData>();

	private List<string> _3AF = new List<string>();

	private List<string> _3Ac = new List<string>();

	private Dictionary<string, PrefabObjectGenerator> _3Ag = new Dictionary<string, PrefabObjectGenerator>();

	private Dictionary<string, string> _3AI = new Dictionary<string, string>();

	/// <summary>
	/// The object's current name.
	/// </summary>
	public string Name
	{
		get
		{
			return _3A_0019;
		}
		set
		{
		}
	}

	internal string FileName
	{
		get
		{
			return _3A3;
		}
		set
		{
			_3A3 = text;
		}
	}

	internal string XnbContentManagerFileName
	{
		get
		{
			return _3A6;
		}
		set
		{
			_3A6 = text;
		}
	}

	internal string ProjectFile
	{
		get
		{
			return _3AD;
		}
		set
		{
			_3AD = text;
		}
	}

	internal ProcessorRenderingType ProcessorRenderingType
	{
		get
		{
			return _3A_0017;
		}
		set
		{
			_3A_0017 = processorRenderingType;
		}
	}

	/// <summary>
	/// Gets the default content repository. This may be null if no content repositories are loaded.
	/// </summary>
	public static ContentRepository DefaultContentRepository => _3A_0018;

	/// <summary>
	/// List of all content repositories.
	/// </summary>
	public static Dictionary<string, ContentRepository> ContentRepositories => _3AL;

	internal Dictionary<string, ModelData> Models => _3Al;

	internal Dictionary<string, SoundEffectData> SoundEffects => _3At;

	internal List<string> LightMaps => _3AF;

	internal List<string> LightOcclusionBuffers => _3Ac;

	internal Dictionary<string, PrefabObjectGenerator> Prefabs => _3Ag;

	private static void V(ContentRepository P_0)
	{
		string name = P_0.Name;
		if (_3AL.ContainsKey(name))
		{
			throw new Exception($"Content repository named '{name}' already exists.");
		}
		_3AL.Add(name, P_0);
		if (_3A_0018 == null)
		{
			_3A_0018 = P_0;
		}
	}

	private static void d(ContentRepository P_0)
	{
		_3AL.Remove(P_0.Name);
		if (_3A_0018 != P_0)
		{
			return;
		}
		_3A_0018 = null;
		using Dictionary<string, ContentRepository>.Enumerator enumerator = _3AL.GetEnumerator();
		if (enumerator.MoveNext())
		{
			_3A_0018 = enumerator.Current.Value;
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
		if (_3AL.TryGetValue(contentrepositoryname, out var value))
		{
			return value;
		}
		return null;
	}

	internal ContentRepository(string P_0, ContentManager P_1)
	{
		_3A_0019 = P_0;
		_3A_0003 = P_1;
		V(this);
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
		d(this);
	}

	internal void _000E(string P_0, string P_1, ModelData P_2)
	{
		if (!_3AI.ContainsKey(P_0))
		{
			_3Al.Add(P_0, P_2);
			_3AI.Add(P_0, P_1);
		}
	}

	internal void _9(string P_0)
	{
		_3Al.Remove(P_0);
		_3AI.Remove(P_0);
	}

	internal void _0004(string P_0, string P_1, SoundEffectData P_2)
	{
		if (!_3AI.ContainsKey(P_0))
		{
			_3At.Add(P_0, P_2);
			_3AI.Add(P_0, P_1);
		}
	}

	internal void J(string P_0)
	{
		_3At.Remove(P_0);
		_3AI.Remove(P_0);
	}

	internal void _0013(string P_0, string P_1)
	{
		if (!_3AI.ContainsKey(P_0))
		{
			_3AF.Add(P_0);
			_3AI.Add(P_0, P_1);
		}
	}

	internal void _0(string P_0, string P_1)
	{
		if (!_3AI.ContainsKey(P_0))
		{
			_3Ac.Add(P_0);
			_3AI.Add(P_0, P_1);
		}
	}

	internal void W(string P_0, PrefabObjectGenerator P_1)
	{
		if (_3Ag.ContainsKey(P_0))
		{
			_3Ag[P_0] = P_1;
		}
		else
		{
			_3Ag.Add(P_0, P_1);
		}
	}

	internal string h(int P_0)
	{
		return string.Format("{0}{1}.lm", "LightMapCache\\", P_0);
	}

	internal string _0011(int P_0)
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
		return LoadBySourceAssetPath<LightMap>(h(mesh._3A_0019), allownull: true);
	}

	/// <summary>
	/// Loads the light occlusion buffer associated with a directional light.
	/// </summary>
	/// <param name="light"></param>
	/// <returns></returns>
	public LightOcclusionBuffer LoadLightOcclusionBuffer(ILight light)
	{
		return LoadBySourceAssetPath<LightOcclusionBuffer>(_0011(light.UniqueId), allownull: true);
	}

	/// <summary>
	/// Loads a prefab by name. Prefabs are created in the SunBurn editor.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public PrefabObjectGenerator LoadPrefab(string name)
	{
		if (_3Ag.TryGetValue(name, out var value))
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
		foreach (KeyValuePair<string, string> item in _3AI)
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
		return _3A_0003.Load<T>(xnbassetpath);
	}

	/// <summary>
	/// Removes all objects from the container. Commonly used while clearing the scene.
	/// </summary>
	public void Clear()
	{
		_3Al.Clear();
		_3At.Clear();
		_3AI.Clear();
		_3AF.Clear();
		_3Ac.Clear();
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		_3Al = _0016._6._6o(info);
		SerializationHelper.DeserializeField(ref _3At, info, "SoundEffects", usedefault: true);
		if (_3At == null)
		{
			_3At = new Dictionary<string, SoundEffectData>();
		}
		SerializationHelper.DeserializeField(ref _3AF, info, "LightMaps", usedefault: false);
		SerializationHelper.DeserializeField(ref _3Ac, info, "LightOcclusionBuffers", usedefault: false);
		SerializationHelper.DeserializeField(ref _3Ag, info, "Prefabs", usedefault: false);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializationHelper.SerializeFieldOrEnum(ref _3Al, info, "Models");
		SerializationHelper.SerializeFieldOrEnum(ref _3At, info, "SoundEffects");
		SerializationHelper.SerializeFieldOrEnum(ref _3AF, info, "LightMaps");
		SerializationHelper.SerializeFieldOrEnum(ref _3Ac, info, "LightOcclusionBuffers");
		SerializationHelper.SerializeFieldOrEnum(ref _3Ag, info, "Prefabs");
	}
}
