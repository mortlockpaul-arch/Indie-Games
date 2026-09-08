using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Xml.Serialization;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using MonoMod;
using MonoMod.Utils;
using XnaToFna.ProxyForms;
using XnaToFna.XEX;

namespace XnaToFna;

public class XnaToFnaUtil : IDisposable
{
	public static ConstructorInfo m_UnverifiableCodeAttribute_ctor = typeof(UnverifiableCodeAttribute).GetConstructor(Type.EmptyTypes);

	public static ConstructorInfo m_XmlIgnore_ctor = typeof(XmlIgnoreAttribute).GetConstructor(Type.EmptyTypes);

	public static MethodInfo m_XnaToFnaHelper_PreUpdate = typeof(XnaToFnaHelper).GetMethod("PreUpdate");

	public static MethodInfo m_XnaToFnaHelper_MainHook = typeof(XnaToFnaHelper).GetMethod("MainHook");

	public static MethodInfo m_FileSystemHelper_FixPath = typeof(FileSystemHelper).GetMethod("FixPath");

	public static readonly byte[] DotNetFrameworkKeyToken = new byte[8] { 183, 122, 92, 86, 25, 52, 224, 137 };

	public static readonly Version DotNetFramework4Version = new Version(4, 0, 0, 0);

	public static readonly Version DotNetFramework2Version = new Version(2, 0, 0, 0);

	public static readonly Version DotNetX360Version = new Version(2, 0, 5, 0);

	public static readonly Assembly ThisAssembly = Assembly.GetExecutingAssembly();

	public static readonly string ThisAssemblyName = ThisAssembly.GetName().Name;

	public static readonly Version Version = ThisAssembly.GetName().Version;

	public readonly ModuleDefinition ThisModule;

	public List<XnaToFnaMapping> Mappings;

	public XnaToFnaModder Modder;

	public DefaultAssemblyResolver AssemblyResolver;

	public List<string> Directories;

	public List<string> ContentDirectoryNames;

	public List<string> ContentDirectories;

	public List<ModuleDefinition> Modules;

	public Dictionary<ModuleDefinition, string> ModulePaths;

	public HashSet<string> RemoveDeps;

	public List<ModuleDefinition> ModulesToStub;

	public List<string> ExtractedXEX;

	public bool HookEntryPoint;

	public bool PatchXNB;

	public bool PatchXACT;

	public bool PatchWindowsMedia;

	public bool DestroyLocks;

	public bool FixOldMonoXML;

	public bool DestroyMixedDeps;

	public bool StubMixedDeps;

	public bool HookIsTrialMode;

	public bool HookBinaryFormatter;

	public bool HookReflection;

	public List<string> DestroyPublicKeyTokens;

	public List<string> FixPathsFor;

	public ILPlatform PreferredPlatform;

	public void Stub(ModuleDefinition mod)
	{
		Log($"[Stub] Stubbing {mod.Assembly.Name.Name}");
		((MonoModder)Modder).Module = mod;
		ApplyCommonChanges(mod, "Stub");
		Log("[Stub] Mapping dependencies for MonoMod");
		((MonoModder)Modder).MapDependencies(mod);
		Log("[Stub] Stubbing");
		foreach (TypeDefinition type in mod.Types)
		{
			StubType(type);
		}
		Log("[Stub] Pre-processing");
		foreach (TypeDefinition type2 in mod.Types)
		{
			PreProcessType(type2);
		}
		Log("[Stub] Relinking (MonoMod PatchRefs pass)");
		((MonoModder)Modder).PatchRefs();
		Log("[Stub] Post-processing");
		foreach (TypeDefinition type3 in mod.Types)
		{
			PostProcessType(type3);
		}
		Log("[Stub] Rewriting and disposing module\n");
		((MonoModder)Modder).Module.Write(((MonoModder)Modder).WriterParameters);
		((MonoModder)Modder).Module.Dispose();
		((MonoModder)Modder).Module = null;
		((MonoModder)Modder).ClearCaches(false, false, true);
	}

	public void StubType(TypeDefinition type)
	{
		foreach (FieldDefinition field in type.Fields)
		{
			field.Attributes &= ~Mono.Cecil.FieldAttributes.HasFieldRVA;
		}
		foreach (MethodDefinition method in type.Methods)
		{
			if (method.HasPInvokeInfo)
			{
				method.PInvokeInfo = null;
			}
			method.IsManaged = true;
			method.IsIL = true;
			method.IsNative = false;
			method.PInvokeInfo = null;
			method.IsPreserveSig = false;
			method.IsInternalCall = false;
			method.IsPInvokeImpl = false;
			Mono.Cecil.Cil.MethodBody methodBody = (method.Body = new Mono.Cecil.Cil.MethodBody(method));
			methodBody.InitLocals = true;
			ILProcessor iLProcessor = methodBody.GetILProcessor();
			for (int i = 0; i < method.Parameters.Count; i++)
			{
				ParameterDefinition parameterDefinition = method.Parameters[i];
				if (parameterDefinition.IsOut || parameterDefinition.IsReturnValue)
				{
					iLProcessor.Emit(OpCodes.Ldarg, i);
					iLProcessor.EmitDefault(parameterDefinition.ParameterType, stind: true);
				}
			}
			iLProcessor.EmitDefault(method.ReturnType ?? method.Module.TypeSystem.Void);
			iLProcessor.Emit(OpCodes.Ret);
		}
		foreach (TypeDefinition nestedType in type.NestedTypes)
		{
			StubType(nestedType);
		}
	}

	public void SetupHelperRelinker()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Expected O, but got Unknown
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Expected O, but got Unknown
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Expected O, but got Unknown
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Expected O, but got Unknown
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Expected O, but got Unknown
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Expected O, but got Unknown
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Expected O, but got Unknown
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Expected O, but got Unknown
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Expected O, but got Unknown
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Expected O, but got Unknown
		((MonoModder)Modder).RelinkMap["System.Void Microsoft.Xna.Framework.Game::.ctor()"] = (object)new RelinkMapEntry("XnaToFna.XnaToFnaGame", "System.Void .ctor()");
		MethodInfo[] methods = typeof(XnaToFnaGame).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (MethodInfo method in methods)
		{
			((MonoModder)Modder).RelinkMap[method.GetFindableID(null, "Microsoft.Xna.Framework.Game")] = (object)new RelinkMapEntry("XnaToFna.XnaToFnaGame", method.GetFindableID(null, null, withType: false));
		}
		((MonoModder)Modder).RelinkMap["System.IntPtr Microsoft.Xna.Framework.GameWindow::get_Handle()"] = (object)new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.IntPtr GetProxyFormHandle(Microsoft.Xna.Framework.GameWindow)");
		((MonoModder)Modder).RelinkMap["System.Void Microsoft.Xna.Framework.GraphicsDeviceManager::ApplyChanges()"] = (object)new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void ApplyChanges(Microsoft.Xna.Framework.GraphicsDeviceManager)");
		Type[] types = typeof(Form).Assembly.GetTypes();
		foreach (Type type in types)
		{
			string fullName = type.FullName;
			if (fullName.StartsWith("XnaToFna.ProxyForms."))
			{
				((MonoModder)Modder).RelinkMap["System.Windows.Forms." + fullName.Substring(20)] = fullName;
			}
			else if (fullName.StartsWith("XnaToFna.ProxyDrawing."))
			{
				((MonoModder)Modder).RelinkMap["System.Drawing." + fullName.Substring(22)] = fullName;
			}
			else if (fullName.StartsWith("XnaToFna.ProxyDInput."))
			{
				((MonoModder)Modder).RelinkMap[fullName.Substring(21)] = fullName;
			}
			else
			{
				if (!fullName.StartsWith("XnaToFna.StubXDK."))
				{
					continue;
				}
				string key = "Microsoft.Xna.Framework." + fullName.Substring(17);
				((MonoModder)Modder).RelinkMap[key] = fullName;
				methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo in methods)
				{
					MonoModHook customAttribute = ((MemberInfo)methodInfo).GetCustomAttribute<MonoModHook>();
					if (customAttribute != null)
					{
						((MonoModder)Modder).RelinkMap[customAttribute.FindableID] = (object)new RelinkMapEntry(fullName, methodInfo.GetFindableID(null, null, withType: false));
					}
				}
				ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (ConstructorInfo constructorInfo in constructors)
				{
					MonoModHook customAttribute2 = ((MemberInfo)constructorInfo).GetCustomAttribute<MonoModHook>();
					if (customAttribute2 != null)
					{
						((MonoModder)Modder).RelinkMap[customAttribute2.FindableID] = (object)new RelinkMapEntry(fullName, constructorInfo.GetFindableID(null, null, withType: false));
					}
				}
			}
		}
		if (HookIsTrialMode)
		{
			((MonoModder)Modder).RelinkMap["System.Boolean Microsoft.Xna.Framework.GamerServices.Guide::get_IsTrialMode()"] = (object)new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.IntPtr get_IsTrialMode()");
		}
		if (HookBinaryFormatter)
		{
			((MonoModder)Modder).RelinkMap["System.Void System.Runtime.Serialization.Formatters.Binary.BinaryFormatter::.ctor()"] = (object)new RelinkMapEntry("XnaToFna.BinaryFormatterHelper", "System.Runtime.Serialization.Formatters.Binary.BinaryFormatter Create()");
			((MonoModder)Modder).RelinkMap["System.Void System.Runtime.Serialization.Formatters.Binary.BinaryFormatter::.ctor(System.Runtime.Serialization.ISurrogateSelector,System.Runtime.Serialization.StreamingContext)"] = (object)new RelinkMapEntry("XnaToFna.BinaryFormatterHelper", "System.Runtime.Serialization.Formatters.Binary.BinaryFormatter Create(System.Runtime.Serialization.ISurrogateSelector,System.Runtime.Serialization.StreamingContext)");
			((MonoModder)Modder).RelinkMap["System.Runtime.Serialization.SerializationBinder System.Runtime.Serialization.Formatters.Binary.BinaryFormatter::get_Binder()"] = (object)new RelinkMapEntry("XnaToFna.BinaryFormatterHelper", "System.Runtime.Serialization.SerializationBinder get_Binder(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter)");
			((MonoModder)Modder).RelinkMap["System.Void System.Runtime.Serialization.Formatters.Binary.BinaryFormatter::set_Binder(System.Runtime.Serialization.SerializationBinder)"] = (object)new RelinkMapEntry("XnaToFna.BinaryFormatterHelper", "System.Void set_Binder(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter,System.Runtime.Serialization.SerializationBinder)");
		}
		if (HookReflection)
		{
			((MonoModder)Modder).RelinkMap["System.Reflection.FieldInfo System.Type::GetField(System.String,System.Reflection.BindingFlags)"] = (object)new RelinkMapEntry("XnaToFna.ProxyReflection.FieldInfoHelper", "System.Reflection.FieldInfo GetField(System.Type,System.String,System.Reflection.BindingFlags)");
			((MonoModder)Modder).RelinkMap["System.Reflection.FieldInfo System.Type::GetField(System.String)"] = (object)new RelinkMapEntry("XnaToFna.ProxyReflection.FieldInfoHelper", "System.Reflection.FieldInfo GetField(System.Type,System.String)");
		}
		((MonoModder)Modder).RelinkMap["System.Void System.Threading.Thread::SetProcessorAffinity(System.Int32[])"] = (object)new RelinkMapEntry("XnaToFna.X360Helper", "System.Void SetProcessorAffinity(System.Threading.Thread,System.Int32[])");
		foreach (XnaToFnaMapping mapping in Mappings)
		{
			if (mapping.IsActive && mapping.Setup != null)
			{
				mapping.Setup(this, mapping);
			}
		}
	}

	public static void SetupGSRelinkMap(XnaToFnaUtil xtf, XnaToFnaMapping mapping)
	{
		SetupDirectRelinkMap(xtf, mapping, delegate(XnaToFnaUtil _, TypeDefinition type)
		{
			((MonoModder)xtf.Modder).RelinkMap[type.FullName] = type;
			if (type.FullName.Contains(".Net."))
			{
				((MonoModder)xtf.Modder).RelinkMap[type.FullName.Replace(".Net.", ".GamerServices.")] = type;
			}
		});
	}

	public static void SetupDirectRelinkMap(XnaToFnaUtil xtf, XnaToFnaMapping mapping)
	{
		SetupDirectRelinkMap(xtf, mapping, null);
	}

	public static void SetupDirectRelinkMap(XnaToFnaUtil xtf, XnaToFnaMapping mapping, Action<XnaToFnaUtil, TypeDefinition> action)
	{
		foreach (TypeDefinition type in mapping.Module.Types)
		{
			SetupDirectRelinkMapType(xtf, type, action);
		}
	}

	public static void SetupDirectRelinkMapType(XnaToFnaUtil xtf, TypeDefinition type, Action<XnaToFnaUtil, TypeDefinition> action)
	{
		if (action != null)
		{
			action(xtf, type);
		}
		else
		{
			((MonoModder)xtf.Modder).RelinkMap[type.FullName] = type;
		}
		foreach (TypeDefinition nestedType in type.NestedTypes)
		{
			SetupDirectRelinkMapType(xtf, nestedType, action);
		}
	}

	public void PreProcessType(TypeDefinition type)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		foreach (MethodDefinition method in type.Methods)
		{
			if (method.HasPInvokeInfo && (method.PInvokeInfo.Module.Name.EndsWith("32.dll") || method.PInvokeInfo.Module.Name.EndsWith("32")))
			{
				string text = method.PInvokeInfo.EntryPoint ?? method.Name;
				if (typeof(PInvokeHooks).GetMethod(text) != null)
				{
					Log($"[PreProcess] [PInvokeHooks] Remapping call to {text} ({method.GetFindableID()})");
					((MonoModder)Modder).RelinkMap[method.GetFindableID(null, null, withType: true, simple: true)] = (object)new RelinkMapEntry("XnaToFna.PInvokeHooks", text);
				}
				else
				{
					Log($"[PreProcess] [PInvokeHooks] Found unhooked call to {text} ({method.GetFindableID()})");
				}
			}
		}
		Stack<TypeDefinition> stack = new Stack<TypeDefinition>();
		try
		{
			for (TypeDefinition typeDefinition = type.BaseType?.Resolve(); typeDefinition != null; typeDefinition = typeDefinition.BaseType?.Resolve())
			{
				stack.Push(typeDefinition);
			}
		}
		catch
		{
		}
		foreach (FieldDefinition field in type.Fields)
		{
			string name = field.Name;
			if (FixOldMonoXML && stack.Any((TypeDefinition baseType) => baseType.FindField(name) != null || baseType.FindProperty(name) != null))
			{
				Log($"[PreProcess] Renaming field name collison {name} in {type.FullName}");
				field.Name = $"{name}_{type.Name}";
				((MonoModder)Modder).RelinkMap[$"{type.FullName}::{name}"] = field.FullName;
			}
		}
		foreach (TypeDefinition nestedType in type.NestedTypes)
		{
			PreProcessType(nestedType);
		}
	}

	public void PostProcessType(TypeDefinition type)
	{
		bool flag = false;
		if (type.BaseType?.FullName == "Microsoft.Xna.Framework.Game")
		{
			Log($"[PostProcess] Found type overriding Game: {type.FullName})");
			type.BaseType = type.Module.ImportReference(typeof(XnaToFnaGame));
			flag = true;
		}
		foreach (MethodDefinition method in type.Methods)
		{
			if (!method.HasBody)
			{
				continue;
			}
			string findableID = method.GetFindableID(null, null, withType: false);
			if (flag && findableID == "System.Void Update(Microsoft.Xna.Framework.GameTime)")
			{
				Log("[PostProcess] Injecting call to XnaToFnaHelper.PreUpdate into game Update");
				ILProcessor iLProcessor = method.Body.GetILProcessor();
				iLProcessor.InsertBefore(method.Body.Instructions[0], iLProcessor.Create(OpCodes.Ldarg_1));
				iLProcessor.InsertAfter(method.Body.Instructions[0], iLProcessor.Create(OpCodes.Callvirt, method.Module.ImportReference(m_XnaToFnaHelper_PreUpdate)));
				method.Body.UpdateOffsets(1, 2);
			}
			for (int i = 0; i < method.Body.Instructions.Count; i++)
			{
				Instruction instruction = method.Body.Instructions[i];
				if (instruction.OpCode == OpCodes.Callvirt && ((MethodReference)instruction.Operand).DeclaringType.FullName == "XnaToFna.XnaToFnaHelper")
				{
					instruction.OpCode = OpCodes.Call;
				}
				if (DestroyLocks)
				{
					CheckAndDestroyLock(method, i);
				}
				if (FixPathsFor.Count != 0)
				{
					CheckAndInjectFixPath(method, ref i);
				}
			}
			int num = 0;
			for (int j = 0; j < method.Body.Instructions.Count; j++)
			{
				Instruction instruction2 = method.Body.Instructions[j];
				instruction2.Offset = num;
				num += instruction2.GetSize();
			}
			for (int k = 0; k < method.Body.Instructions.Count; k++)
			{
				Instruction instruction3 = method.Body.Instructions[k];
				if (instruction3.Operand is Instruction)
				{
					int num2 = ((Instruction)instruction3.Operand).Offset - instruction3.Offset;
					if (num2 <= -127 || num2 >= 127)
					{
						instruction3.OpCode = ShortToLongOp(instruction3.OpCode);
					}
					else
					{
						instruction3.OpCode = LongToShortOp(instruction3.OpCode);
					}
				}
			}
		}
		foreach (TypeDefinition nestedType in type.NestedTypes)
		{
			PostProcessType(nestedType);
		}
	}

	public OpCode ShortToLongOp(OpCode op)
	{
		string name = Enum.GetName(typeof(Code), op.Code);
		if (!name.EndsWith("_S"))
		{
			return op;
		}
		return ((OpCode?)typeof(OpCodes).GetField(name.Substring(0, name.Length - 2))?.GetValue(null)) ?? op;
	}

	public OpCode LongToShortOp(OpCode op)
	{
		string name = Enum.GetName(typeof(Code), op.Code);
		if (name.EndsWith("_S"))
		{
			return op;
		}
		return ((OpCode?)typeof(OpCodes).GetField(name + "_S")?.GetValue(null)) ?? op;
	}

	public void CheckAndDestroyLock(MethodDefinition method, int instri)
	{
		Instruction instruction = method.Body.Instructions[instri];
		if (instri >= 1 && (instruction.OpCode == OpCodes.Brfalse || instruction.OpCode == OpCodes.Brfalse_S || instruction.OpCode == OpCodes.Brtrue || instruction.OpCode == OpCodes.Brtrue_S) && ((Instruction)instruction.Operand).Offset < instruction.Offset && instruction.Previous.Operand != null)
		{
			string text = (instruction.Previous.Operand as FieldReference)?.Name ?? (instruction.Previous.Operand as MethodReference)?.Name ?? instruction.Previous.Operand.ToString();
			text = text.ToLowerInvariant();
			if (instri - method.Body.Instructions.IndexOf((Instruction)instruction.Operand) <= 3 && text != null && (text.Contains("load") || text.Contains("content")))
			{
				Log($"[PostProcess] [HACK!!!] NOPing possible content loading waiting loop in {method.GetFindableID()}");
				if (instruction.Previous?.Previous.OpCode == OpCodes.Volatile)
				{
					instruction.Previous.Previous.OpCode = OpCodes.Nop;
				}
				instruction.Previous.OpCode = OpCodes.Nop;
				instruction.Previous.Operand = null;
				instruction.OpCode = OpCodes.Nop;
				instruction.Operand = null;
			}
		}
		if (instri < 3 || !(instruction.OpCode == OpCodes.Call) || !(((MethodReference)instruction.Operand).GetFindableID() == "System.Void System.Threading.Monitor::Enter(System.Object,System.Boolean&)"))
		{
			return;
		}
		if (method.DeclaringType.FullName.ToLowerInvariant().Contains("content") || method.Name.ToLowerInvariant().Contains("load") || method.Name.ToLowerInvariant().Contains("content"))
		{
			Log($"[PostProcess] [HACK!!!] Destroying possible content loading lock in {method.GetFindableID()}");
			DestroyMonitorLock(method, instri);
			return;
		}
		int num = instri;
		while (0 < num && instri - 4 <= num)
		{
			string text2 = ((method.Body.Instructions[num].Operand as FieldReference)?.Name ?? (method.Body.Instructions[num].Operand as MethodReference)?.Name ?? method.Body.Instructions[num].Operand?.ToString())?.ToLowerInvariant();
			if (text2 != null && (text2.Contains("load") || text2.Contains("content")))
			{
				Log($"[PostProcess] [HACK!!!] Destroying possible content loading lock in {method.GetFindableID()}");
				DestroyMonitorLock(method, instri);
				break;
			}
			num--;
		}
	}

	public void DestroyMonitorLock(MethodDefinition method, int instri)
	{
		Instruction instruction = method.Body.Instructions[instri];
		instruction.Operand = ((MonoModder)Modder).Module.ImportReference(((MonoModder)Modder).FindTypeDeep("XnaToFna.FakeMonitor").Resolve().FindMethod("System.Void Enter(System.Object,System.Boolean&)"));
		int num = 1;
		while (instri < method.Body.Instructions.Count && num > 0)
		{
			instruction = method.Body.Instructions[instri];
			if (instruction.OpCode == OpCodes.Call)
			{
				string findableID = ((MethodReference)instruction.Operand).GetFindableID();
				if (findableID == "System.Void System.Threading.Monitor::Enter(System.Object,System.Boolean&)")
				{
					num++;
				}
				else if (findableID == "System.Void System.Threading.Monitor::Exit(System.Object)")
				{
					num--;
				}
			}
			instri++;
		}
		if (num == 0)
		{
			instruction.Operand = ((MonoModder)Modder).Module.ImportReference(((MonoModder)Modder).FindTypeDeep("XnaToFna.FakeMonitor").Resolve().FindMethod("System.Void Exit(System.Object)"));
		}
	}

	public void CheckAndInjectFixPath(MethodDefinition method, ref int instri)
	{
		Collection<Instruction> instructions = method.Body.Instructions;
		Instruction instruction = instructions[instri];
		if (instruction.OpCode != OpCodes.Call && instruction.OpCode != OpCodes.Callvirt && instruction.OpCode != OpCodes.Newobj)
		{
			return;
		}
		MethodReference methodReference = (MethodReference)instruction.Operand;
		if (!FixPathsFor.Contains(methodReference.GetFindableID()) && !FixPathsFor.Contains(methodReference.GetFindableID(null, null, withType: false)) && !FixPathsFor.Contains(methodReference.GetFindableID(null, null, withType: true, simple: true)) && !FixPathsFor.Contains(methodReference.GetFindableID(null, null, withType: false, simple: true)))
		{
			return;
		}
		MethodReference method2 = method.Module.ImportReference(StackOpHelper.m_Push);
		MethodReference method3 = method.Module.ImportReference(StackOpHelper.m_Pop);
		MethodReference method4 = method.Module.ImportReference(m_FileSystemHelper_FixPath);
		ILProcessor iLProcessor = method.Body.GetILProcessor();
		for (int num = methodReference.Parameters.Count - 1; num > -1; num--)
		{
			if (methodReference.Parameters[num].ParameterType.MetadataType == MetadataType.String)
			{
				iLProcessor.InsertBefore(instructions[instri], iLProcessor.Create(OpCodes.Call, method4));
				instri++;
			}
			GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(method2);
			genericInstanceMethod.GenericArguments.Add(methodReference.Parameters[num].ParameterType);
			iLProcessor.InsertBefore(instructions[instri], iLProcessor.Create(OpCodes.Call, genericInstanceMethod));
			instri++;
		}
		for (int i = 0; i < methodReference.Parameters.Count; i++)
		{
			GenericInstanceMethod genericInstanceMethod2 = new GenericInstanceMethod(method3);
			genericInstanceMethod2.GenericArguments.Add(methodReference.Parameters[i].ParameterType);
			iLProcessor.InsertBefore(instructions[instri], iLProcessor.Create(OpCodes.Call, genericInstanceMethod2));
			instri++;
		}
	}

	public XnaToFnaUtil()
	{
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Expected O, but got Unknown
		Mappings = new List<XnaToFnaMapping>
		{
			new XnaToFnaMapping("System", new string[1] { "System.Net" }),
			new XnaToFnaMapping("FNA", new string[9] { "Microsoft.Xna.Framework", "Microsoft.Xna.Framework.Avatar", "Microsoft.Xna.Framework.Content.Pipeline", "Microsoft.Xna.Framework.Game", "Microsoft.Xna.Framework.Graphics", "Microsoft.Xna.Framework.Input.Touch", "Microsoft.Xna.Framework.Storage", "Microsoft.Xna.Framework.Video", "Microsoft.Xna.Framework.Xact" }),
			new XnaToFnaMapping("MonoGame.Framework.Net", new string[3] { "Microsoft.Xna.Framework.GamerServices", "Microsoft.Xna.Framework.Net", "Microsoft.Xna.Framework.Xdk" }, SetupGSRelinkMap),
			new XnaToFnaMapping("FNA.Steamworks", new string[4] { "FNA.Steamworks", "Microsoft.Xna.Framework.GamerServices", "Microsoft.Xna.Framework.Net", "Microsoft.Xna.Framework.Xdk" }, SetupGSRelinkMap)
		};
		AssemblyResolver = new DefaultAssemblyResolver();
		Directories = new List<string>();
		ContentDirectoryNames = new List<string> { "Content" };
		ContentDirectories = new List<string>();
		Modules = new List<ModuleDefinition>();
		ModulePaths = new Dictionary<ModuleDefinition, string>();
		RemoveDeps = new HashSet<string> { null, "", "Microsoft.DirectX.DirectInput", "Microsoft.VisualC" };
		ModulesToStub = new List<ModuleDefinition>();
		ExtractedXEX = new List<string>();
		HookEntryPoint = true;
		PatchXNB = true;
		PatchXACT = true;
		PatchWindowsMedia = true;
		DestroyLocks = true;
		StubMixedDeps = true;
		HookBinaryFormatter = true;
		HookReflection = true;
		DestroyPublicKeyTokens = new List<string>();
		FixPathsFor = new List<string>();
		PreferredPlatform = ILPlatform.AnyCPU;
		base._002Ector();
		Modder = new XnaToFnaModder(this);
		((MonoModder)Modder).ReadingMode = ReadingMode.Immediate;
		((MonoModder)Modder).Strict = false;
		((MonoModder)Modder).AssemblyResolver = AssemblyResolver;
		((MonoModder)Modder).DependencyDirs = Directories;
		((MonoModder)Modder).MissingDependencyResolver = new MissingDependencyResolver(MissingDependencyResolver);
		using (FileStream input = new FileStream(Assembly.GetExecutingAssembly().Location, FileMode.Open, FileAccess.Read))
		{
			ThisModule = MonoModExt.ReadModule(input, new ReaderParameters(ReadingMode.Immediate));
		}
		((MonoModder)Modder).DependencyCache[ThisModule.Assembly.Name.Name] = ThisModule;
		((MonoModder)Modder).DependencyCache[ThisModule.Assembly.Name.FullName] = ThisModule;
	}

	public XnaToFnaUtil(params string[] paths)
		: this()
	{
		ScanPaths(paths);
	}

	public void Log(string txt)
	{
		Console.Write("[XnaToFna] ");
		Console.WriteLine(txt);
	}

	public ModuleDefinition MissingDependencyResolver(MonoModder modder, ModuleDefinition main, string name, string fullName)
	{
		((MonoModder)Modder).Log($"Cannot map dependency {main.Name} -> (({fullName}), ({name})) - not found");
		return null;
	}

	public void ScanPaths(params string[] paths)
	{
		foreach (string path in paths)
		{
			ScanPath(path);
		}
	}

	public void ScanPath(string path)
	{
		if (Directory.Exists(path))
		{
			if (Directories.Contains(path))
			{
				return;
			}
			RestoreBackup(path);
			Log($"[ScanPath] Scanning directory {path}");
			Directories.Add(path);
			AssemblyResolver.AddSearchDirectory(path);
			foreach (string contentDirectoryName in ContentDirectoryNames)
			{
				string text;
				if (!Directory.Exists(text = Path.Combine(path, contentDirectoryName)))
				{
					continue;
				}
				if (ContentDirectories.Count == 0)
				{
					string text2 = Path.Combine(path, Path.GetFileName(ThisAssembly.Location));
					if (Path.GetDirectoryName(ThisAssembly.Location) != path)
					{
						Log("[ScanPath] Found separate game directory - copying XnaToFna.exe and FNA.dll");
						File.Copy(ThisAssembly.Location, text2, overwrite: true);
						string text3 = null;
						if (File.Exists(Path.ChangeExtension(ThisAssembly.Location, "pdb")))
						{
							text3 = "pdb";
						}
						if (File.Exists(Path.ChangeExtension(ThisAssembly.Location, "mdb")))
						{
							text3 = "mdb";
						}
						if (text3 != null)
						{
							File.Copy(Path.ChangeExtension(ThisAssembly.Location, text3), Path.ChangeExtension(text2, text3), overwrite: true);
						}
						if (File.Exists(Path.Combine(Path.GetDirectoryName(ThisAssembly.Location), "FNA.dll")))
						{
							File.Copy(Path.Combine(Path.GetDirectoryName(ThisAssembly.Location), "FNA.dll"), Path.Combine(path, "FNA.dll"), overwrite: true);
						}
						else if (File.Exists(Path.Combine(Path.GetDirectoryName(ThisAssembly.Location), "FNA.dll.tmp")))
						{
							File.Copy(Path.Combine(Path.GetDirectoryName(ThisAssembly.Location), "FNA.dll.tmp"), Path.Combine(path, "FNA.dll"), overwrite: true);
						}
					}
				}
				Log($"[ScanPath] Found Content directory: {text}");
				ContentDirectories.Add(text);
			}
			ScanPaths(Directory.GetFiles(path));
			return;
		}
		if (File.Exists(path + ".xex"))
		{
			if (!ExtractedXEX.Contains(path))
			{
				File.Delete(path);
			}
			return;
		}
		if (path.EndsWith(".xex"))
		{
			string text4 = path.Substring(0, path.Length - 4);
			if (string.IsNullOrEmpty(Path.GetExtension(text4)))
			{
				return;
			}
			using (Stream input = File.OpenRead(path))
			{
				using BinaryReader reader = new BinaryReader(input);
				using Stream stream = File.OpenWrite(text4);
				XEXImageData xEXImageData = new XEXImageData(reader);
				int num = 0;
				int count = xEXImageData.m_memorySize;
				if (xEXImageData.m_memorySize > 65536)
				{
					using MemoryStream memoryStream = new MemoryStream(xEXImageData.m_memoryData);
					using BinaryReader binaryReader = new BinaryReader(memoryStream);
					if (binaryReader.ReadUInt32() == 9460301)
					{
						memoryStream.Seek(640L, SeekOrigin.Begin);
						if (binaryReader.ReadUInt64() == 107152478071086L)
						{
							memoryStream.Seek(648L, SeekOrigin.Begin);
							binaryReader.ReadInt32();
							num = binaryReader.ReadInt32();
							count = xEXImageData.m_memorySize - num;
						}
					}
				}
				stream.Write(xEXImageData.m_memoryData, num, count);
			}
			path = text4;
			ExtractedXEX.Add(text4);
		}
		else if (!path.EndsWith(".dll") && !path.EndsWith(".exe"))
		{
			return;
		}
		AssemblyName name;
		try
		{
			name = AssemblyName.GetAssemblyName(path);
		}
		catch
		{
			return;
		}
		ReaderParameters readerParameters = ((MonoModder)Modder).GenReaderParameters(false, (string)null);
		readerParameters.ReadWrite = path != ThisAssembly.Location && !Mappings.Exists((XnaToFnaMapping mappings) => name.Name == mappings.Target);
		if (!File.Exists(path + ".mdb") && !File.Exists(Path.ChangeExtension(path, "pdb")))
		{
			readerParameters.ReadSymbols = false;
		}
		Log(string.Format("[ScanPath] Checking assembly {0} ({1})", name.Name, readerParameters.ReadWrite ? "rw" : "r-"));
		ModuleDefinition moduleDefinition;
		try
		{
			moduleDefinition = MonoModExt.ReadModule(path, readerParameters);
		}
		catch (Exception arg)
		{
			Log($"[ScanPath] WARNING: Cannot load assembly: {arg}");
			return;
		}
		bool flag = !readerParameters.ReadWrite || name.Name == ThisAssemblyName;
		if ((moduleDefinition.Attributes & ModuleAttributes.ILOnly) != ModuleAttributes.ILOnly)
		{
			Log($"[ScanPath] WARNING: Cannot handle mixed mode assembly {name.Name}");
			if (!StubMixedDeps)
			{
				if (DestroyMixedDeps)
				{
					RemoveDeps.Add(name.Name);
				}
				moduleDefinition.Dispose();
				return;
			}
			ModulesToStub.Add(moduleDefinition);
			flag = true;
		}
		if (flag && !readerParameters.ReadWrite)
		{
			foreach (XnaToFnaMapping mapping2 in Mappings)
			{
				if (name.Name == mapping2.Target)
				{
					mapping2.IsActive = true;
					mapping2.Module = moduleDefinition;
					string[] sources = mapping2.Sources;
					foreach (string text5 in sources)
					{
						Log($"[ScanPath] Mapping {text5} -> {name.Name}");
						((MonoModder)Modder).RelinkModuleMap[text5] = moduleDefinition;
					}
				}
			}
		}
		else if (!flag)
		{
			foreach (XnaToFnaMapping mapping in Mappings)
			{
				if (moduleDefinition.AssemblyReferences.Any((AssemblyNameReference dep) => mapping.Sources.Contains(dep.Name)))
				{
					flag = true;
					Log($"[ScanPath] XnaToFna-ing {name.Name}");
					break;
				}
			}
		}
		if (flag)
		{
			Modules.Add(moduleDefinition);
			ModulePaths[moduleDefinition] = path;
		}
		else
		{
			moduleDefinition.Dispose();
		}
	}

	public void RestoreBackup(string root)
	{
		string text = Path.Combine(root, "orig");
		if (Directory.Exists(text))
		{
			RestoreBackup(root, text);
		}
	}

	public void RestoreBackup(string root, string origRoot)
	{
		Log($"[RestoreBackup] Restoring from {origRoot} to {root}");
		foreach (string item in Directory.EnumerateFiles(origRoot, "*", SearchOption.AllDirectories))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(root + item.Substring(origRoot.Length)));
			File.Copy(item, root + item.Substring(origRoot.Length), overwrite: true);
		}
	}

	public void OrderModules()
	{
		List<ModuleDefinition> list = new List<ModuleDefinition>(Modules);
		Log("[OrderModules] Unordered: ");
		for (int i = 0; i < Modules.Count; i++)
		{
			Log($"[OrderModules] #{i + 1}: {Modules[i].Assembly.Name.Name}");
		}
		ModuleDefinition dep = null;
		foreach (ModuleDefinition module in Modules)
		{
			foreach (AssemblyNameReference depName in module.AssemblyReferences)
			{
				if (Modules.Exists((ModuleDefinition other) => (dep = other).Assembly.Name.Name == depName.Name) && list.IndexOf(dep) > list.IndexOf(module))
				{
					Log($"[OrderModules] Reordering {module.Assembly.Name.Name} dependency {dep.Name}");
					list.Remove(module);
					list.Insert(list.IndexOf(dep) + 1, module);
				}
			}
		}
		Modules = list;
		Log("[OrderModules] Reordered: ");
		for (int num = 0; num < Modules.Count; num++)
		{
			Log($"[OrderModules] #{num + 1}: {Modules[num].Assembly.Name.Name}");
		}
	}

	public void RelinkAll()
	{
		SetupHelperRelinker();
		foreach (ModuleDefinition module in Modules)
		{
			((MonoModder)Modder).DependencyCache[module.Assembly.Name.Name] = module;
		}
		foreach (ModuleDefinition item in ModulesToStub)
		{
			Stub(item);
		}
		foreach (ModuleDefinition module2 in Modules)
		{
			Relink(module2);
		}
	}

	public void Relink(ModuleDefinition mod)
	{
		if (Mappings.Exists((XnaToFnaMapping mappings) => mod.Assembly.Name.Name == mappings.Target) || ModulesToStub.Contains(mod) || mod.Assembly.Name.Name == ThisAssemblyName)
		{
			return;
		}
		Log($"[Relink] Relinking {mod.Assembly.Name.Name}");
		((MonoModder)Modder).Module = mod;
		ApplyCommonChanges(mod);
		Log("[Relink] Pre-processing");
		foreach (TypeDefinition type in mod.Types)
		{
			PreProcessType(type);
		}
		Log("[Relink] Relinking (MonoMod PatchRefs pass)");
		((MonoModder)Modder).PatchRefs();
		Log("[Relink] Post-processing");
		foreach (TypeDefinition type2 in mod.Types)
		{
			PostProcessType(type2);
		}
		if (HookEntryPoint && mod.EntryPoint != null)
		{
			Log("[Relink] Injecting XnaToFna entry point hook");
			ILProcessor iLProcessor = mod.EntryPoint.Body.GetILProcessor();
			Instruction instruction = iLProcessor.Create(OpCodes.Call, mod.ImportReference(m_XnaToFnaHelper_MainHook));
			iLProcessor.InsertBefore(mod.EntryPoint.Body.Instructions[0], instruction);
			iLProcessor.InsertBefore(instruction, iLProcessor.Create(OpCodes.Ldarg_0));
		}
		Log("[Relink] Rewriting and disposing module\n");
		((MonoModder)Modder).Module.Write(((MonoModder)Modder).WriterParameters);
		((MonoModder)Modder).Module.Dispose();
		((MonoModder)Modder).Module = null;
		((MonoModder)Modder).ClearCaches(false, false, true);
	}

	public void ApplyCommonChanges(ModuleDefinition mod, string tag = "Relink")
	{
		if (DestroyPublicKeyTokens.Contains(mod.Assembly.Name.Name))
		{
			Log($"[{tag}] Destroying public key token for module {mod.Assembly.Name.Name}");
			mod.Assembly.Name.PublicKeyToken = new byte[0];
		}
		Log($"[{tag}] Updating dependencies");
		for (int i = 0; i < mod.AssemblyReferences.Count; i++)
		{
			AssemblyNameReference dep = mod.AssemblyReferences[i];
			foreach (XnaToFnaMapping mapping in Mappings)
			{
				if (!mapping.Sources.Contains(dep.Name) || !((MonoModder)Modder).DependencyCache.ContainsKey(mapping.Target))
				{
					continue;
				}
				if (mod.AssemblyReferences.Any((AssemblyNameReference existingDep) => existingDep.Name == mapping.Target))
				{
					mod.AssemblyReferences.RemoveAt(i);
					i--;
				}
				else
				{
					Log($"[{tag}] Replacing dependency {dep.Name} -> {mapping.Target}");
					mod.AssemblyReferences[i] = ((MonoModder)Modder).DependencyCache[mapping.Target].Assembly.Name;
				}
				goto IL_02a8;
			}
			if (RemoveDeps.Contains(dep.Name))
			{
				Log($"[{tag}] Removing unwanted dependency {dep.Name}");
				mod.AssemblyReferences.RemoveAt(i);
				i--;
				continue;
			}
			if (DestroyPublicKeyTokens.Contains(dep.Name))
			{
				Log($"[{tag}] Destroying public key token for dependency {dep.Name}");
				dep.PublicKeyToken = new byte[0];
			}
			if (ModulesToStub.Any((ModuleDefinition stub) => stub.Assembly.Name.Name == dep.Name))
			{
				Log($"[{tag}] Fixing stubbed dependency {dep.Name}");
				dep.IsWindowsRuntime = false;
				dep.HasPublicKey = false;
			}
			if (dep.Version == DotNetX360Version)
			{
				dep.PublicKeyToken = DotNetFrameworkKeyToken;
				dep.Version = DotNetFramework4Version;
			}
			IL_02a8:;
		}
		if (!mod.AssemblyReferences.Any((AssemblyNameReference assemblyNameReference) => assemblyNameReference.Name == ThisAssemblyName))
		{
			Log($"[{tag}] Adding dependency XnaToFna");
			mod.AssemblyReferences.Add(((MonoModder)Modder).DependencyCache[ThisAssemblyName].Assembly.Name);
		}
		if (mod.Runtime < TargetRuntime.Net_4_0)
		{
			mod.Runtime = TargetRuntime.Net_4_0;
		}
		Log($"[{tag}] Updating module attributes");
		mod.Attributes &= ~ModuleAttributes.StrongNameSigned;
		if (PreferredPlatform != ILPlatform.Keep)
		{
			mod.Architecture = TargetArchitecture.I386;
			mod.Attributes &= ~(ModuleAttributes.Required32Bit | ModuleAttributes.Preferred32Bit);
			switch (PreferredPlatform)
			{
			case ILPlatform.x86:
				mod.Architecture = TargetArchitecture.I386;
				mod.Attributes |= ModuleAttributes.Required32Bit;
				break;
			case ILPlatform.x64:
				mod.Architecture = TargetArchitecture.AMD64;
				break;
			case ILPlatform.x86Pref:
				mod.Architecture = TargetArchitecture.I386;
				mod.Attributes |= ModuleAttributes.Preferred32Bit;
				break;
			}
		}
		bool flag = (mod.Attributes & ModuleAttributes.ILOnly) != ModuleAttributes.ILOnly;
		if ((ModulesToStub.Count != 0) | flag)
		{
			Log($"[{tag}] Making assembly unsafe");
			mod.Attributes |= ModuleAttributes.ILOnly;
			for (int num = 0; num < mod.Assembly.CustomAttributes.Count; num++)
			{
				if (mod.Assembly.CustomAttributes[num].AttributeType.FullName == "System.CLSCompliantAttribute")
				{
					mod.Assembly.CustomAttributes.RemoveAt(num);
					num--;
				}
			}
			if (!mod.CustomAttributes.Any((CustomAttribute ca) => ca.AttributeType.FullName == "System.Security.UnverifiableCodeAttribute"))
			{
				mod.AddAttribute(mod.ImportReference(m_UnverifiableCodeAttribute_ctor));
			}
		}
		Log($"[{tag}] Mapping dependencies for MonoMod");
		((MonoModder)Modder).MapDependencies(mod);
	}

	public void LoadModules()
	{
		foreach (ModuleDefinition mod in Modules)
		{
			if (!Mappings.Exists((XnaToFnaMapping mappings) => mod.Assembly.Name.Name == mappings.Target) && !(mod.Assembly.Name.Name == ThisAssemblyName) && !ModulesToStub.Contains(mod))
			{
				Assembly asm = Assembly.LoadFile(ModulePaths[mod]);
				AppDomain.CurrentDomain.TypeResolve += (object sender, ResolveEventArgs args) => (!(asm.GetType(args.Name) != null)) ? null : asm;
				AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) => (!(args.Name == asm.FullName) && !(args.Name == asm.GetName().Name)) ? null : asm;
			}
		}
	}

	public void UpdateContent()
	{
		if (ContentDirectories.Count == 0)
		{
			Log("[UpdateContent] No content directory found!");
			return;
		}
		using (ContentHelper.Game = new ContentHelperGame())
		{
			ContentHelper.Game.ActionQueue.Enqueue(delegate
			{
				foreach (string contentDirectory in ContentDirectories)
				{
					foreach (string item in Directory.EnumerateFiles(contentDirectory, "*", SearchOption.AllDirectories))
					{
						ContentHelper.UpdateContent(item, PatchXNB, PatchXACT, PatchWindowsMedia);
					}
				}
			});
			ContentHelper.Game.Run();
		}
		ContentHelper.Game = null;
	}

	public void Dispose()
	{
		XnaToFnaModder modder = Modder;
		if (modder != null)
		{
			((MonoModder)modder).Dispose();
		}
		foreach (ModuleDefinition module in Modules)
		{
			module.Dispose();
		}
		Modules.Clear();
		ModulesToStub.Clear();
		Directories.Clear();
	}
}
