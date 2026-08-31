using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects;
using X;

namespace Z
{
	internal class _0002<T> : global::Z.B<T> where T : IWorldBoundingBoxObject
	{
		internal Dictionary<T, global::Z.B<T>> HCB = new Dictionary<T, global::Z.B<T>>(128);

		internal _0002(BoundingBox P_0, int P_1)
		{
			Init(ref P_0, P_1, this);
		}

		public _0002()
		{
		}

		internal void B(ref BoundingBox P_0, int P_1)
		{
			Init(ref P_0, P_1, this);
		}

		internal override void G()
		{
			HCB.Clear();
			base.G();
		}
	}
	internal class _0012 : IDisposable
	{
		private GraphicsDevice HCB;

		private BasicEffect HC_0002;

		private VertexBuffer HC_0012;

		private IndexBuffer HCH;

		private static Vector3[] HC7 = new Vector3[8]
		{
			new Vector3(1f, 1f, 1f),
			new Vector3(0f, 1f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(1f, 1f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		};

		private static int[] HC_0001 = new int[24]
		{
			1, 0, 3, 2, 4, 5, 6, 7, 0, 4,
			2, 6, 5, 1, 7, 3, 0, 1, 4, 5,
			3, 2, 7, 6
		};

		internal BasicEffect DefaultEffect => HC_0002;

		internal _0012(GraphicsDevice P_0)
		{
			HCB = P_0;
			HC_0002 = new SunBurnBasicEffect(HCB);
			HC_0002.TextureEnabled = false;
			HC_0002.VertexColorEnabled = false;
			HC_0002.PreferPerPixelLighting = false;
			HC_0002.LightingEnabled = false;
			HC_0002.FogEnabled = false;
			HC_0002.SpecularColor = Vector3.Zero;
			VertexPositionTexture[] array = new VertexPositionTexture[24];
			short[] array2 = new short[36];
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 6; i++)
			{
				Vector3 position = HC7[HC_0001[num]];
				Vector3 position2 = HC7[HC_0001[num + 1]];
				Vector3 position3 = HC7[HC_0001[num + 2]];
				Vector3 position4 = HC7[HC_0001[num + 3]];
				array[num].Position = position;
				array[num + 1].Position = position2;
				array[num + 2].Position = position3;
				array[num + 3].Position = position4;
				array2[num2++] = (byte)num;
				array2[num2++] = (byte)(num + 1);
				array2[num2++] = (byte)(num + 2);
				array2[num2++] = (byte)(num + 3);
				array2[num2++] = (byte)(num + 2);
				array2[num2++] = (byte)(num + 1);
				num += 4;
			}
			HC_0012 = new VertexBuffer(HCB, typeof(VertexPositionTexture), array.Length, BufferUsage.WriteOnly);
			HC_0012.SetData(array);
			HCH = new IndexBuffer(HCB, typeof(short), array2.Length, BufferUsage.WriteOnly);
			HCH.SetData(array2);
		}

		internal Matrix _1(BoundingBox P_0)
		{
			Matrix result = Matrix.CreateScale(P_0.Max - P_0.Min);
			result.Translation = P_0.Min;
			return result;
		}

		internal void b()
		{
			HCB.SetVertexBuffer(HC_0012);
			HCB.Indices = HCH;
			HCB.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12);
		}

		public void Dispose()
		{
			F.B._7_0004(ref HC_0002);
			F.B._7_0004(ref HC_0012);
			F.B._7_0004(ref HCH);
		}
	}
	internal class _0001
	{
		private bool HCB;

		private IGraphicsDeviceService HC_0002;

		private int HC_0012;

		private int HCH;

		private int HC7;

		internal bool Changed
		{
			get
			{
				PresentationParameters presentationParameters = HC_0002.GraphicsDevice.PresentationParameters;
				if (HCB || presentationParameters.BackBufferWidth != HC_0012 || presentationParameters.BackBufferHeight != HCH || presentationParameters.MultiSampleCount != HC7)
				{
					HC_0012 = presentationParameters.BackBufferWidth;
					HCH = presentationParameters.BackBufferHeight;
					HC7 = presentationParameters.MultiSampleCount;
					HCB = false;
					return true;
				}
				return false;
			}
		}

		internal _0001()
		{
			IGraphicsDeviceService graphicsDeviceManager = SunBurnCoreSystem.Instance.GraphicsDeviceManager;
			HC_0002 = graphicsDeviceManager;
			HC_0002.DeviceCreated += _0002y;
			HC_0002.DeviceReset += _0002y;
			HC_0002.DeviceDisposing += _0002y;
		}

		private void _0002y(object P_0, EventArgs P_1)
		{
			HCB = true;
		}
	}
	internal class _000F
	{
		private class _0001CB
		{
			[CompilerGenerated]
			private bool HCB;

			[CompilerGenerated]
			private IPlugin HC_0002;

			public bool AutoLoaded
			{
				[CompilerGenerated]
				get
				{
					return HCB;
				}
				[CompilerGenerated]
				private set
				{
					HCB = hCB;
				}
			}

			public IPlugin Plugin
			{
				[CompilerGenerated]
				get
				{
					return HC_0002;
				}
				[CompilerGenerated]
				private set
				{
					HC_0002 = plugin;
				}
			}

			public static _0001CB Create(IPlugin plugin, bool autoloaded)
			{
				_0001CB obj = new _0001CB();
				obj.Plugin = plugin;
				obj.AutoLoaded = autoloaded;
				return obj;
			}
		}

		private TypeDictionary<_0001CB> HCB = new TypeDictionary<_0001CB>();

		internal void _0002z()
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
						_0002A(type, true);
					}
				}
			}
		}

		internal void _0002A<T>(bool P_0) where T : IPlugin
		{
			_0002A(typeof(T), P_0);
		}

		internal void _0002A(Type P_0, bool P_1)
		{
			if (HCB.Items.ContainsKey(P_0))
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
				HCB.Add(P_0, _0001CB.Create(plugin, P_1));
			}
			catch (Exception innerException)
			{
				throw new Exception($"Unable to create plugin '{P_0.Name}'.", innerException);
			}
		}

		internal void _0002c(IManagerServiceProvider P_0, bool P_1)
		{
			foreach (KeyValuePair<Type, _0001CB> item in HCB.Items)
			{
				_0001CB value = item.Value;
				if (!value.AutoLoaded || P_1)
				{
					value.Plugin.Initialize(P_0);
				}
			}
		}

		internal void u()
		{
			foreach (KeyValuePair<Type, _0001CB> item in HCB.Items)
			{
				item.Value.Plugin.Unload();
			}
		}
	}
	internal class _0011
	{
		private struct _0001CB
		{
			internal string HCB;

			internal float HC_0002;

			internal _0001CB(string P_0, float P_1)
			{
				HCB = P_0;
				HC_0002 = P_1;
			}
		}

		private static List<_0001CB> HCB = new List<_0001CB>(32);

		internal static void _0002Q(string P_0)
		{
			HCB.Add(new _0001CB(P_0, -1f));
		}

		internal static void _0002_0016(string P_0, float P_1)
		{
			HCB.Add(new _0001CB(P_0, P_1));
		}

		internal static void _0002v(string P_0)
		{
		}
	}
}
namespace z
{
	internal abstract class _0002 : B
	{
		public new static _0002 Create()
		{
			return new D();
		}

		public _0002()
		{
		}

		public abstract byte[] EncryptValue(byte[] rgb);

		public abstract byte[] DecryptValue(byte[] rgb);

		public abstract K ExportParameters(bool include);

		public abstract void ImportParameters(K parameters);

		internal void H8(K P_0)
		{
			if (P_0.P != null)
			{
				Array.Clear(P_0.P, 0, P_0.P.Length);
			}
			if (P_0.Q != null)
			{
				Array.Clear(P_0.Q, 0, P_0.Q.Length);
			}
			if (P_0.DP != null)
			{
				Array.Clear(P_0.DP, 0, P_0.DP.Length);
			}
			if (P_0.DQ != null)
			{
				Array.Clear(P_0.DQ, 0, P_0.DQ.Length);
			}
			if (P_0.InverseQ != null)
			{
				Array.Clear(P_0.InverseQ, 0, P_0.InverseQ.Length);
			}
			if (P_0.D != null)
			{
				Array.Clear(P_0.D, 0, P_0.D.Length);
			}
		}
	}
	internal abstract class _0012
	{
		public abstract string Parameters { get; set; }

		public _0012()
		{
		}

		public abstract byte[] DecryptKeyExchange(byte[] rgb);

		public abstract void SetKey(B key);
	}
	internal class _0001 : _7
	{
		public _0001()
			: base("Unexpected error occured during a cryptographic operation.")
		{
			base.HResult = -2146233295;
		}

		public _0001(string message)
			: base(message)
		{
			base.HResult = -2146233295;
		}

		public _0001(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233295;
		}

		public _0001(string format, string insert)
			: base(string.Format(format, insert))
		{
			base.HResult = -2146233295;
		}
	}
	internal sealed class _000F
	{
		private int HCB;

		private int HC_0002;

		private int HC_0012;

		public int MaxSize => HCB;

		public int MinSize => HC_0002;

		public int SkipSize => HC_0012;

		public _000F(int minSize, int maxSize, int skipSize)
		{
			HCB = maxSize;
			HC_0002 = minSize;
			HC_0012 = skipSize;
		}

		internal bool H5(int P_0)
		{
			int num = P_0 - MinSize;
			bool flag = num >= 0 && P_0 <= MaxSize;
			if (SkipSize != 0)
			{
				if (flag)
				{
					return num % SkipSize == 0;
				}
				return false;
			}
			return flag;
		}

		internal static bool H3(_000F[] P_0, int P_1)
		{
			foreach (_000F obj in P_0)
			{
				if (obj.H5(P_1))
				{
					return true;
				}
			}
			return false;
		}
	}
	internal class _0011 : _0012
	{
		private _0002 HCB;

		public override string Parameters
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public _0011()
		{
			HCB = null;
		}

		public _0011(B key)
		{
			SetKey(key);
		}

		public override byte[] DecryptKeyExchange(byte[] rgbData)
		{
			k hash = k.Create();
			byte[] array = global::X._0002.Decrypt_OAEP(HCB, hash, rgbData);
			if (array != null)
			{
				return array;
			}
			throw new _7("OAEP decoding error.");
		}

		public override void SetKey(B key)
		{
			HCB = (_0002)key;
		}
	}
	internal class _0003 : _0012
	{
		private _0002 HCB;

		public override string Parameters
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public _0003()
		{
			HCB = null;
		}

		public _0003(B key)
		{
			SetKey(key);
		}

		public override byte[] DecryptKeyExchange(byte[] rgbData)
		{
			if (HCB == null)
			{
				throw new _0001("No key pair available.");
			}
			byte[] array = global::X._0002.Decrypt_v15(HCB, rgbData);
			if (array != null)
			{
				return array;
			}
			throw new _7("PKCS1 decoding error.");
		}

		public override void SetKey(B key)
		{
			HCB = (_0002)key;
		}
	}
	internal sealed class _0013 : k
	{
		private s HCB;

		public _0013()
		{
			HCB = new s();
		}

		~_0013()
		{
			Dispose(disposing: false);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override void HashCore(byte[] rgb, int start, int size)
		{
			State = 1;
			HCB.HashCore(rgb, start, size);
		}

		protected override byte[] HashFinal()
		{
			State = 0;
			return HCB.HashFinal();
		}

		public override void Initialize()
		{
			HCB.Initialize();
		}
	}
}
