using System;
using BEPUphysics.DataStructures;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests;

/// <summary>
///  Helper class that reduces contact manifolds to reasonable numbers of contacts.
/// </summary>
public static class ContactReducer
{
	/// <summary>
	///  Reduces the contact manifold to a good subset.
	/// </summary>
	/// <param name="contacts">Contacts to reduce.</param>
	/// <param name="contactCandidates">Contact candidates to include in the reduction process.</param>
	/// <param name="contactsToRemove">Contacts that need to removed to reach the reduced state.</param>
	/// <param name="toAdd">Contact candidates that should be added to reach the reduced state.</param>
	/// <exception cref="T:System.InvalidOperationException">Thrown when the set being reduced is empty.</exception>
	public static void ReduceContacts(RawList<Contact> contacts, RawValueList<ContactData> contactCandidates, RawList<int> contactsToRemove, RawValueList<ContactData> toAdd)
	{
		float num = float.MinValue;
		int num2 = -1;
		Vector3 value = Toolbox.ZeroVector;
		for (int i = 0; i < contacts.count; i++)
		{
			Vector3.Add(ref value, ref contacts.Elements[i].Normal, out value);
			if (contacts.Elements[i].PenetrationDepth > num)
			{
				num2 = i;
				num = contacts.Elements[i].PenetrationDepth;
			}
		}
		for (int j = 0; j < contactCandidates.count; j++)
		{
			Vector3.Add(ref value, ref contactCandidates.Elements[j].Normal, out value);
			if (contactCandidates.Elements[j].PenetrationDepth > num)
			{
				num2 = contacts.count + j;
				num = contactCandidates.Elements[j].PenetrationDepth;
			}
		}
		if (value.LengthSquared() < 1E-07f)
		{
			if (contacts.count > 0)
			{
				value = contacts.Elements[0].Normal;
			}
			else
			{
				if (contactCandidates.count <= 0)
				{
					throw new ArgumentException("Cannot reduce an empty contact set.");
				}
				value = contactCandidates.Elements[0].Normal;
			}
		}
		Vector3 value2 = ((num2 >= contacts.count) ? contactCandidates.Elements[num2 - contacts.count].Position : contacts.Elements[num2].Position);
		float num3 = 0f;
		int num4 = -1;
		float result;
		for (int k = 0; k < contacts.count; k++)
		{
			Vector3.DistanceSquared(ref contacts.Elements[k].Position, ref value2, out result);
			if (result > num3)
			{
				num3 = result;
				num4 = k;
			}
		}
		for (int l = 0; l < contactCandidates.count; l++)
		{
			Vector3.DistanceSquared(ref contactCandidates.Elements[l].Position, ref value2, out result);
			if (result > num3)
			{
				num3 = result;
				num4 = contacts.count + l;
			}
		}
		if (num4 == -1)
		{
			if (contacts.count > 0)
			{
				for (int m = 1; m < contacts.count; m++)
				{
					contactsToRemove.Add(m);
				}
				return;
			}
			if (contactCandidates.count > 0)
			{
				toAdd.Add(ref contactCandidates.Elements[0]);
				return;
			}
			throw new ArgumentException("Cannot reduce an empty contact set.");
		}
		Vector3 value3 = ((num4 >= contacts.count) ? contactCandidates.Elements[num4 - contacts.count].Position : contacts.Elements[num4].Position);
		Vector3.Subtract(ref value2, ref value3, out var result2);
		Vector3.Cross(ref result2, ref value, out var result3);
		float num5 = float.MaxValue;
		float num6 = float.MinValue;
		int num7 = -1;
		int num8 = -1;
		for (int n = 0; n < contacts.count; n++)
		{
			Vector3.Dot(ref contacts.Elements[n].Position, ref result3, out var result4);
			if (result4 < num5)
			{
				num7 = n;
				num5 = result4;
			}
			if (result4 > num6)
			{
				num8 = n;
				num6 = result4;
			}
		}
		for (int num9 = 0; num9 < contactCandidates.count; num9++)
		{
			Vector3.Dot(ref contactCandidates.Elements[num9].Position, ref result3, out var result5);
			if (result5 < num5)
			{
				num7 = num9 + contacts.count;
				num5 = result5;
			}
			if (result5 > num6)
			{
				num8 = num9 + contacts.count;
				num6 = result5;
			}
		}
		for (int num10 = 0; num10 < contactCandidates.count; num10++)
		{
			int num11 = num10 + contacts.count;
			if (num11 == num2 || num11 == num4 || num11 == num7 || num11 == num8)
			{
				toAdd.Add(ref contactCandidates.Elements[num10]);
			}
		}
		for (int num12 = 0; num12 < contacts.count; num12++)
		{
			if (num12 != num2 && num12 != num4 && num12 != num7 && num12 != num8)
			{
				contactsToRemove.Add(num12);
			}
		}
	}

	/// <summary>
	///  Reduces a 4-contact manifold and contact candidate to 4 total contacts.
	/// </summary>
	/// <param name="contacts">Contacts to reduce.</param>
	/// <param name="contactCandidate">Contact candidate to include in the reduction process.</param>
	/// <param name="toRemove">Contacts that need to be removed to reduce the manifold.</param>
	/// <param name="addCandidate">Whether or not to add the contact candidate to reach the reduced manifold.</param>
	/// <exception cref="T:System.ArgumentException">Thrown when the contact manifold being reduced doesn't have 4 contacts.</exception>
	public static void ReduceContacts(RawList<Contact> contacts, ref ContactData contactCandidate, RawList<int> toRemove, out bool addCandidate)
	{
		if (contacts.count != 4)
		{
			throw new ArgumentException("Can only use this method to reduce contact lists with four contacts and a contact candidate.");
		}
		float num = float.MinValue;
		int num2 = -1;
		for (int i = 0; i < 4; i++)
		{
			if (contacts.Elements[i].PenetrationDepth > num)
			{
				num2 = i;
				num = contacts.Elements[i].PenetrationDepth;
			}
		}
		if (contactCandidate.PenetrationDepth > num)
		{
			num2 = 4;
		}
		Vector3 value = ((num2 >= 4) ? contactCandidate.Position : contacts.Elements[num2].Position);
		float num3 = 0f;
		int num4 = -1;
		float result;
		for (int j = 0; j < 4; j++)
		{
			Vector3.DistanceSquared(ref contacts.Elements[j].Position, ref value, out result);
			if (result > num3)
			{
				num3 = result;
				num4 = j;
			}
		}
		Vector3.DistanceSquared(ref contactCandidate.Position, ref value, out result);
		if (result > num3)
		{
			num4 = 4;
		}
		Vector3 value2 = ((num4 >= contacts.count) ? contactCandidate.Position : contacts.Elements[num4].Position);
		Vector3.Subtract(ref value, ref value2, out var result2);
		Vector3.Cross(ref result2, ref contacts.Elements[0].Normal, out var result3);
		float num5 = float.MaxValue;
		float num6 = float.MinValue;
		int num7 = -1;
		int num8 = -1;
		float result4;
		for (int k = 0; k < 4; k++)
		{
			Vector3.Dot(ref contacts.Elements[k].Position, ref result3, out result4);
			if (result4 < num5)
			{
				num7 = k;
				num5 = result4;
			}
			if (result4 > num6)
			{
				num8 = k;
				num6 = result4;
			}
		}
		Vector3.Dot(ref contactCandidate.Position, ref result3, out result4);
		if (result4 < num5)
		{
			num7 = 4;
		}
		if (result4 > num6)
		{
			num8 = 4;
		}
		if (4 == num2 || 4 == num4 || 4 == num7 || 4 == num8)
		{
			addCandidate = true;
			for (int l = 0; l < 4; l++)
			{
				if (l != num2 && l != num4 && l != num7 && l != num8)
				{
					toRemove.Add(l);
					break;
				}
			}
		}
		else
		{
			addCandidate = false;
		}
	}
}
