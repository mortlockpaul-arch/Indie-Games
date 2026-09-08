using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace System.Text.RegularExpressions;

[RequiresDynamicCode("Compiling a RegEx requires dynamic code.")]
internal sealed class RegexLWCGCompiler : RegexCompiler
{
	private static readonly bool s_includePatternInName = Environment.GetEnvironmentVariable("DOTNET_SYSTEM_TEXT_REGULAREXPRESSIONS_PATTERNINNAME") == "1";

	private static readonly Type[] s_paramTypes = new Type[2]
	{
		typeof(RegexRunner),
		typeof(ReadOnlySpan<char>)
	};

	private static int s_regexCount;

	public RegexRunnerFactory FactoryInstanceFromCode(string pattern, RegexTree regexTree, RegexOptions options, bool hasTimeout)
	{
		if (!regexTree.Root.SupportsCompilation(out var _))
		{
			return null;
		}
		_regexTree = regexTree;
		_options = options;
		_hasTimeout = hasTimeout;
		uint value = (uint)Interlocked.Increment(ref s_regexCount);
		string value2 = string.Empty;
		if (s_includePatternInName)
		{
			value2 = string.Concat("_".AsSpan(), (pattern.Length > 100) ? pattern.AsSpan(0, 100) : pattern.AsSpan());
		}
		DynamicMethod tryFindNextStartingPositionMethod = DefineDynamicMethod($"Regex{value}_TryFindNextPossibleStartingPosition{value2}", typeof(bool), typeof(CompiledRegexRunner), s_paramTypes);
		EmitTryFindNextPossibleStartingPosition();
		DynamicMethod tryMatchAtCurrentPositionMethod = DefineDynamicMethod($"Regex{value}_TryMatchAtCurrentPosition{value2}", typeof(bool), typeof(CompiledRegexRunner), s_paramTypes);
		EmitTryMatchAtCurrentPosition();
		DynamicMethod scanMethod = DefineDynamicMethod($"Regex{value}_Scan{value2}", null, typeof(CompiledRegexRunner), new Type[2]
		{
			typeof(RegexRunner),
			typeof(ReadOnlySpan<char>)
		});
		EmitScan(options, tryFindNextStartingPositionMethod, tryMatchAtCurrentPositionMethod);
		return new CompiledRegexRunnerFactory(scanMethod, _searchValues?.ToArray(), regexTree.Culture);
	}

	private DynamicMethod DefineDynamicMethod(string methname, Type returntype, Type hostType, Type[] paramTypes)
	{
		DynamicMethod dynamicMethod = new DynamicMethod(methname, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, returntype, paramTypes, hostType, skipVisibility: false);
		_ilg = dynamicMethod.GetILGenerator();
		return dynamicMethod;
	}
}
