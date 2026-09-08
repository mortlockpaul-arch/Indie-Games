using System.Diagnostics.CodeAnalysis;

namespace System.Security.AccessControl;

public abstract class GenericAce
{
	private readonly AceType _type;

	private AceFlags _flags;

	internal ushort _indexInAcl;

	public AceType AceType => _type;

	public AceFlags AceFlags
	{
		get
		{
			return _flags;
		}
		set
		{
			_flags = value;
		}
	}

	public bool IsInherited => (AceFlags & AceFlags.Inherited) != 0;

	public InheritanceFlags InheritanceFlags
	{
		get
		{
			InheritanceFlags inheritanceFlags = InheritanceFlags.None;
			if ((AceFlags & AceFlags.ContainerInherit) != AceFlags.None)
			{
				inheritanceFlags |= InheritanceFlags.ContainerInherit;
			}
			if ((AceFlags & AceFlags.ObjectInherit) != AceFlags.None)
			{
				inheritanceFlags |= InheritanceFlags.ObjectInherit;
			}
			return inheritanceFlags;
		}
	}

	public PropagationFlags PropagationFlags
	{
		get
		{
			PropagationFlags propagationFlags = PropagationFlags.None;
			if ((AceFlags & AceFlags.InheritOnly) != AceFlags.None)
			{
				propagationFlags |= PropagationFlags.InheritOnly;
			}
			if ((AceFlags & AceFlags.NoPropagateInherit) != AceFlags.None)
			{
				propagationFlags |= PropagationFlags.NoPropagateInherit;
			}
			return propagationFlags;
		}
	}

	public AuditFlags AuditFlags
	{
		get
		{
			AuditFlags auditFlags = AuditFlags.None;
			if ((AceFlags & AceFlags.SuccessfulAccess) != AceFlags.None)
			{
				auditFlags |= AuditFlags.Success;
			}
			if ((AceFlags & AceFlags.FailedAccess) != AceFlags.None)
			{
				auditFlags |= AuditFlags.Failure;
			}
			return auditFlags;
		}
	}

	public abstract int BinaryLength { get; }

	internal void MarshalHeader(byte[] binaryForm, int offset)
	{
		ArgumentNullException.ThrowIfNull(binaryForm, "binaryForm");
		int binaryLength = BinaryLength;
		ArgumentOutOfRangeException.ThrowIfNegative(offset, "offset");
		if (binaryForm.Length - offset < BinaryLength)
		{
			throw new ArgumentOutOfRangeException("binaryForm", System.SR.ArgumentOutOfRange_ArrayTooSmall);
		}
		if (binaryLength > 65535)
		{
			throw new InvalidOperationException();
		}
		binaryForm[offset] = (byte)AceType;
		binaryForm[offset + 1] = (byte)AceFlags;
		binaryForm[offset + 2] = (byte)binaryLength;
		binaryForm[offset + 3] = (byte)(binaryLength >> 8);
	}

	internal GenericAce(AceType type, AceFlags flags)
	{
		_type = type;
		_flags = flags;
	}

	internal static AceFlags AceFlagsFromAuditFlags(AuditFlags auditFlags)
	{
		AceFlags aceFlags = AceFlags.None;
		if ((auditFlags & AuditFlags.Success) != AuditFlags.None)
		{
			aceFlags |= AceFlags.SuccessfulAccess;
		}
		if ((auditFlags & AuditFlags.Failure) != AuditFlags.None)
		{
			aceFlags |= AceFlags.FailedAccess;
		}
		if (aceFlags == AceFlags.None)
		{
			throw new ArgumentException(System.SR.Arg_EnumAtLeastOneFlag, "auditFlags");
		}
		return aceFlags;
	}

	internal static AceFlags AceFlagsFromInheritanceFlags(InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags)
	{
		AceFlags aceFlags = AceFlags.None;
		if ((inheritanceFlags & InheritanceFlags.ContainerInherit) != InheritanceFlags.None)
		{
			aceFlags |= AceFlags.ContainerInherit;
		}
		if ((inheritanceFlags & InheritanceFlags.ObjectInherit) != InheritanceFlags.None)
		{
			aceFlags |= AceFlags.ObjectInherit;
		}
		if (aceFlags != AceFlags.None)
		{
			if ((propagationFlags & PropagationFlags.NoPropagateInherit) != PropagationFlags.None)
			{
				aceFlags |= AceFlags.NoPropagateInherit;
			}
			if ((propagationFlags & PropagationFlags.InheritOnly) != PropagationFlags.None)
			{
				aceFlags |= AceFlags.InheritOnly;
			}
		}
		return aceFlags;
	}

	internal static void VerifyHeader(byte[] binaryForm, int offset)
	{
		ArgumentNullException.ThrowIfNull(binaryForm, "binaryForm");
		ArgumentOutOfRangeException.ThrowIfNegative(offset, "offset");
		if (binaryForm.Length - offset < 4)
		{
			throw new ArgumentOutOfRangeException("binaryForm", System.SR.ArgumentOutOfRange_ArrayTooSmall);
		}
		if ((binaryForm[offset + 3] << 8) + binaryForm[offset + 2] > binaryForm.Length - offset)
		{
			throw new ArgumentOutOfRangeException("binaryForm", System.SR.ArgumentOutOfRange_ArrayTooSmall);
		}
	}

	public static GenericAce CreateFromBinaryForm(byte[] binaryForm, int offset)
	{
		VerifyHeader(binaryForm, offset);
		AceType aceType = (AceType)binaryForm[offset];
		GenericAce genericAce;
		if (aceType == AceType.AccessAllowed || aceType == AceType.AccessDenied || aceType == AceType.SystemAudit || aceType == AceType.SystemAlarm || aceType == AceType.AccessAllowedCallback || aceType == AceType.AccessDeniedCallback || aceType == AceType.SystemAuditCallback || aceType == AceType.SystemAlarmCallback)
		{
			if (CommonAce.ParseBinaryForm(binaryForm, offset, out var qualifier, out var accessMask, out var sid, out var isCallback, out var opaque))
			{
				genericAce = new CommonAce((AceFlags)binaryForm[offset + 1], qualifier, accessMask, sid, isCallback, opaque);
				goto IL_0148;
			}
		}
		else if (aceType == AceType.AccessAllowedObject || aceType == AceType.AccessDeniedObject || aceType == AceType.SystemAuditObject || aceType == AceType.SystemAlarmObject || aceType == AceType.AccessAllowedCallbackObject || aceType == AceType.AccessDeniedCallbackObject || aceType == AceType.SystemAuditCallbackObject || aceType == AceType.SystemAlarmCallbackObject)
		{
			if (ObjectAce.ParseBinaryForm(binaryForm, offset, out var qualifier2, out var accessMask2, out var sid2, out var objectFlags, out var objectAceType, out var inheritedObjectAceType, out var isCallback2, out var opaque2))
			{
				genericAce = new ObjectAce((AceFlags)binaryForm[offset + 1], qualifier2, accessMask2, sid2, objectFlags, objectAceType, inheritedObjectAceType, isCallback2, opaque2);
				goto IL_0148;
			}
		}
		else if (aceType == AceType.AccessAllowedCompound)
		{
			if (CompoundAce.ParseBinaryForm(binaryForm, offset, out var accessMask3, out var compoundAceType, out var sid3))
			{
				genericAce = new CompoundAce((AceFlags)binaryForm[offset + 1], accessMask3, compoundAceType, sid3);
				goto IL_0148;
			}
		}
		else
		{
			AceFlags flags = (AceFlags)binaryForm[offset + 1];
			byte[] array = null;
			int num = binaryForm[offset + 2] + (binaryForm[offset + 3] << 8);
			if (num % 4 == 0)
			{
				int num2 = num - 4;
				if (num2 > 0)
				{
					array = new byte[num2];
					for (int i = 0; i < num2; i++)
					{
						array[i] = binaryForm[offset + num - num2 + i];
					}
				}
				genericAce = new CustomAce(aceType, flags, array);
				goto IL_0148;
			}
		}
		goto IL_019c;
		IL_019c:
		throw new ArgumentException(System.SR.ArgumentException_InvalidAceBinaryForm, "binaryForm");
		IL_0148:
		if ((genericAce is ObjectAce || binaryForm[offset + 2] + (binaryForm[offset + 3] << 8) == genericAce.BinaryLength) && (!(genericAce is ObjectAce) || binaryForm[offset + 2] + (binaryForm[offset + 3] << 8) == genericAce.BinaryLength || binaryForm[offset + 2] + (binaryForm[offset + 3] << 8) - 32 == genericAce.BinaryLength))
		{
			return genericAce;
		}
		goto IL_019c;
	}

	public abstract void GetBinaryForm(byte[] binaryForm, int offset);

	public GenericAce Copy()
	{
		byte[] binaryForm = new byte[BinaryLength];
		GetBinaryForm(binaryForm, 0);
		return CreateFromBinaryForm(binaryForm, 0);
	}

	public sealed override bool Equals([NotNullWhen(true)] object? o)
	{
		GenericAce genericAce = o as GenericAce;
		if (genericAce == null)
		{
			return false;
		}
		if (AceType != genericAce.AceType || AceFlags != genericAce.AceFlags)
		{
			return false;
		}
		int binaryLength = BinaryLength;
		if (binaryLength != genericAce.BinaryLength)
		{
			return false;
		}
		byte[] array = new byte[binaryLength];
		GetBinaryForm(array, 0);
		byte[] array2 = new byte[binaryLength];
		genericAce.GetBinaryForm(array2, 0);
		return ((ReadOnlySpan<byte>)array.AsSpan()).SequenceEqual((ReadOnlySpan<byte>)array2);
	}

	public sealed override int GetHashCode()
	{
		int binaryLength = BinaryLength;
		byte[] array = new byte[binaryLength];
		GetBinaryForm(array, 0);
		int num = 0;
		for (int i = 0; i < binaryLength; i += 4)
		{
			int num2 = array[i] + (array[i + 1] << 8) + (array[i + 2] << 16) + (array[i + 3] << 24);
			num ^= num2;
		}
		return num;
	}

	public static bool operator ==(GenericAce? left, GenericAce? right)
	{
		if ((object)left == null && (object)right == null)
		{
			return true;
		}
		if ((object)left == null || (object)right == null)
		{
			return false;
		}
		return left.Equals(right);
	}

	public static bool operator !=(GenericAce? left, GenericAce? right)
	{
		return !(left == right);
	}
}
