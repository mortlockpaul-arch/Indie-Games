using Mono.Cecil;

namespace MonoMod.Utils;

[MonoMod__OldName__("MonoMod.Relinker")]
public delegate IMetadataTokenProvider Relinker(IMetadataTokenProvider mtp, IGenericParameterProvider context);
