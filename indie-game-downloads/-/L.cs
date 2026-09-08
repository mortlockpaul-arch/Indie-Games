using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using _6;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Serialization;

namespace _0018
{
	internal class L
	{
		internal static uint D()
		{
			return 1u;
		}
	}
}
namespace _0003
{
	internal class L<T> : global::_0003._0018<T> where T : IWorldBoundingBoxObject
	{
		internal Dictionary<T, global::_0003._0018<T>> _3A_0018 = new Dictionary<T, global::_0003._0018<T>>(128);

		internal L(BoundingBox P_0, int P_1)
		{
			Init(ref P_0, P_1, this);
		}

		public L()
		{
		}

		internal void _0018(ref BoundingBox P_0, int P_1)
		{
			Init(ref P_0, P_1, this);
		}

		internal override void U()
		{
			_3A_0018.Clear();
			base.U();
		}
	}
	internal class l
	{
		private class DA_0018
		{
			[CompilerGenerated]
			private bool _3A_0018;

			[CompilerGenerated]
			private IPlugin _3AL;

			public bool AutoLoaded
			{
				[CompilerGenerated]
				get
				{
					return _3A_0018;
				}
				[CompilerGenerated]
				private set
				{
					_3A_0018 = flag;
				}
			}

			public IPlugin Plugin
			{
				[CompilerGenerated]
				get
				{
					return _3AL;
				}
				[CompilerGenerated]
				private set
				{
					_3AL = plugin;
				}
			}

			public static DA_0018 Create(IPlugin plugin, bool autoloaded)
			{
				DA_0018 obj = new DA_0018();
				obj.Plugin = plugin;
				obj.AutoLoaded = autoloaded;
				return obj;
			}
		}

		private TypeDictionary<DA_0018> _3A_0018 = new TypeDictionary<DA_0018>();

		internal void LT()
		{
			SunBurnConfiguration current = SunBurnConfiguration.Current;
			foreach (SunBurnConfiguration.SunBurnPluginConfigurationElement plugin in current.Plugins)
			{
				foreach (string assembly2 in plugin.Assemblies)
				{
					Assembly assembly = Assembly.Load(assembly2);
					object[] customAttributes = assembly.GetCustomAttributes(typeof(SunBurnPluginTypeAttribute), inherit: true);
					object[] array = customAttributes;
					for (int i = 0; i < array.Length; i++)
					{
						SunBurnPluginTypeAttribute sunBurnPluginTypeAttribute = (SunBurnPluginTypeAttribute)array[i];
						Type type = assembly.GetType(sunBurnPluginTypeAttribute.FullName);
						if ((object)type == null)
						{
							type = Type.GetType(sunBurnPluginTypeAttribute.AssemblyQualifiedName);
						}
						if ((object)type == null)
						{
							throw new Exception($"Unable to load plugin class '{sunBurnPluginTypeAttribute.AssemblyQualifiedName}'.");
						}
						Ly(type, true);
					}
				}
			}
		}

		internal void Ly<T>(bool P_0) where T : IPlugin
		{
			Ly(typeof(T), P_0);
		}

		internal void Ly(Type P_0, bool P_1)
		{
			if (_3A_0018.Items.ContainsKey(P_0))
			{
				return;
			}
			if (!typeof(IPlugin).IsAssignableFrom(P_0))
			{
				throw new Exception($"Class '{P_0.Name}' is not a SunBurn plugin.");
			}
			try
			{
				IPlugin plugin = (IPlugin)Activator.CreateInstance(P_0);
				if (plugin == null)
				{
					throw new Exception("Plugin class instance is null.");
				}
				_3A_0018.Add(P_0, DA_0018.Create(plugin, P_1));
			}
			catch (Exception innerException)
			{
				throw new Exception($"Unable to create plugin '{P_0.Name}'.", innerException);
			}
		}

		internal void L_0015(IManagerServiceProvider P_0, bool P_1)
		{
			foreach (KeyValuePair<Type, DA_0018> item in _3A_0018.Items)
			{
				DA_0018 value = item.Value;
				if (!value.AutoLoaded || P_1)
				{
					value.Plugin.Initialize(P_0);
				}
			}
		}

		internal void X()
		{
			foreach (KeyValuePair<Type, DA_0018> item in _3A_0018.Items)
			{
				item.Value.Plugin.Unload();
			}
		}
	}
}
namespace _0001
{
	[Serializable]
	internal sealed class L : IComparer
	{
		public static readonly L Default = new L();

		internal static readonly L _3A_0018 = new L(CultureInfo.InvariantCulture);

		private CompareInfo _3AL;

		private L()
		{
		}

		internal L(CultureInfo P_0)
		{
			if (P_0 == null)
			{
				throw new ArgumentNullException("culture");
			}
			_3AL = P_0.CompareInfo;
		}

		public int Compare(object a, object b)
		{
			if (a == b)
			{
				return 0;
			}
			if (a == null)
			{
				return -1;
			}
			if (b == null)
			{
				return 1;
			}
			if (_3AL != null)
			{
				string text = a as string;
				string text2 = b as string;
				if (text != null && text2 != null)
				{
					return _3AL.Compare(text, text2);
				}
			}
			if (a is IComparable)
			{
				return (a as IComparable).CompareTo(b);
			}
			if (b is IComparable)
			{
				return -(b as IComparable).CompareTo(a);
			}
			throw new ArgumentException("Neither 'a' nor 'b' implements IComparable.");
		}
	}
}
namespace _0016
{
	internal class L
	{
		internal static MemberInfo[] _6u(Type P_0)
		{
			object[] customAttributes = P_0.GetCustomAttributes(typeof(SerializationInclusionModelAttribute), inherit: true);
			if (customAttributes == null || customAttributes.Length < 1)
			{
				return global::_6.L.GetSerializableMembers(P_0);
			}
			return _6U(P_0);
		}

		private static MemberInfo[] _6U(Type P_0)
		{
			MemberInfo[] members = P_0.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			List<MemberInfo> list = new List<MemberInfo>(members.Length);
			MemberInfo[] array = members;
			foreach (MemberInfo memberInfo in array)
			{
				PropertyInfo propertyInfo = memberInfo as PropertyInfo;
				FieldInfo fieldInfo = memberInfo as FieldInfo;
				if ((object)propertyInfo != null && propertyInfo.CanRead && propertyInfo.CanWrite)
				{
					list.Add(memberInfo);
				}
				else if ((object)fieldInfo != null)
				{
					list.Add(memberInfo);
				}
			}
			return list.ToArray();
		}
	}
}
