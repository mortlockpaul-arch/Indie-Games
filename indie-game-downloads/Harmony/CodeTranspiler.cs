using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony.ILCopying;

namespace Harmony;

internal class CodeTranspiler
{
	private IEnumerable<CodeInstruction> codeInstructions;

	private static readonly Dictionary<OpCode, OpCode> allJumpCodes = new Dictionary<OpCode, OpCode>
	{
		{
			OpCodes.Beq_S,
			OpCodes.Beq
		},
		{
			OpCodes.Bge_S,
			OpCodes.Bge
		},
		{
			OpCodes.Bge_Un_S,
			OpCodes.Bge_Un
		},
		{
			OpCodes.Bgt_S,
			OpCodes.Bgt
		},
		{
			OpCodes.Bgt_Un_S,
			OpCodes.Bgt_Un
		},
		{
			OpCodes.Ble_S,
			OpCodes.Ble
		},
		{
			OpCodes.Ble_Un_S,
			OpCodes.Ble_Un
		},
		{
			OpCodes.Blt_S,
			OpCodes.Blt
		},
		{
			OpCodes.Blt_Un_S,
			OpCodes.Blt_Un
		},
		{
			OpCodes.Bne_Un_S,
			OpCodes.Bne_Un
		},
		{
			OpCodes.Brfalse_S,
			OpCodes.Brfalse
		},
		{
			OpCodes.Brtrue_S,
			OpCodes.Brtrue
		},
		{
			OpCodes.Br_S,
			OpCodes.Br
		},
		{
			OpCodes.Leave_S,
			OpCodes.Leave
		}
	};

	public CodeTranspiler(List<ILInstruction> ilInstructions)
	{
		codeInstructions = ilInstructions.Select((ILInstruction ilInstruction) => ilInstruction.GetCodeInstruction()).ToList().AsEnumerable();
	}

	public static object ConvertInstruction(Type type, object op, out List<KeyValuePair<string, object>> unassigned)
	{
		object obj = Activator.CreateInstance(type, OpCodes.Nop, null);
		List<KeyValuePair<string, object>> nonExisting = new List<KeyValuePair<string, object>>();
		Traverse.IterateFields(op, obj, delegate(string name, Traverse trvFrom, Traverse trvDest)
		{
			object obj2 = trvFrom.GetValue();
			if (!trvDest.FieldExists())
			{
				nonExisting.Add(new KeyValuePair<string, object>(name, obj2));
			}
			else
			{
				if (name == "opcode")
				{
					obj2 = ReplaceShortJumps((OpCode)obj2);
				}
				trvDest.SetValue(obj2);
			}
		});
		unassigned = nonExisting;
		return obj;
	}

	public static IEnumerable ConvertInstructions(Type type, IEnumerable enumerable, out Dictionary<object, List<KeyValuePair<string, object>>> unassignedValues)
	{
		Assembly assembly = type.GetGenericTypeDefinition().Assembly;
		Type type2 = assembly.GetType(typeof(List<>).FullName);
		Type type3 = type.GetGenericArguments()[0];
		object obj = Activator.CreateInstance(assembly.GetType(type2.MakeGenericType(type3).FullName));
		MethodInfo method = obj.GetType().GetMethod("Add");
		unassignedValues = new Dictionary<object, List<KeyValuePair<string, object>>>();
		foreach (object item in enumerable)
		{
			object obj2 = ConvertInstruction(type3, item, out var unassigned);
			unassignedValues.Add(obj2, unassigned);
			method.Invoke(obj, new object[1] { obj2 });
		}
		return obj as IEnumerable;
	}

	public static IEnumerable<CodeInstruction> ConvertInstructions(IEnumerable instructions, Dictionary<object, List<KeyValuePair<string, object>>> unassignedValues)
	{
		List<CodeInstruction> list = new List<CodeInstruction>();
		foreach (object instruction in instructions)
		{
			CodeInstruction codeInstruction = new CodeInstruction(OpCodes.Nop);
			Traverse.IterateFields(instruction, codeInstruction, delegate(Traverse trvFrom, Traverse trvDest)
			{
				trvDest.SetValue(trvFrom.GetValue());
			});
			if (unassignedValues.TryGetValue(instruction, out var value))
			{
				Traverse traverse = Traverse.Create(codeInstruction);
				foreach (KeyValuePair<string, object> item in value)
				{
					traverse.Field(item.Key).SetValue(item.Value);
				}
			}
			list.Add(codeInstruction);
		}
		return list;
	}

	public static IEnumerable ConvertInstructions(MethodInfo transpiler, IEnumerable enumerable, out Dictionary<object, List<KeyValuePair<string, object>>> unassignedValues)
	{
		return ConvertInstructions((from p in transpiler.GetParameters()
			select p.ParameterType).FirstOrDefault((Type t) => t.IsGenericType && t.GetGenericTypeDefinition().Name.StartsWith("IEnumerable")), enumerable, out unassignedValues);
	}

	public static List<object> GetTranspilerCallParameters(ILGenerator generator, MethodInfo transpiler, MethodBase method, IEnumerable instructions)
	{
		List<object> list = new List<object>();
		foreach (Type item in from param in transpiler.GetParameters()
			select param.ParameterType)
		{
			if (item.IsAssignableFrom(typeof(ILGenerator)))
			{
				list.Add(generator);
			}
			else if (item.IsAssignableFrom(typeof(MethodBase)))
			{
				list.Add(method);
			}
			else
			{
				list.Add(instructions);
			}
		}
		return list;
	}

	public IEnumerable<CodeInstruction> GetResult(ILGenerator generator, MethodBase method)
	{
		return codeInstructions;
	}

	private static OpCode ReplaceShortJumps(OpCode opcode)
	{
		foreach (KeyValuePair<OpCode, OpCode> allJumpCode in allJumpCodes)
		{
			if (opcode == allJumpCode.Key)
			{
				return allJumpCode.Value;
			}
		}
		return opcode;
	}
}
