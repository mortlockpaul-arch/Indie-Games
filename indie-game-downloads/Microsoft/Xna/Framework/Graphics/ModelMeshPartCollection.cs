using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.Graphics;

public sealed class ModelMeshPartCollection : ReadOnlyCollection<ModelMeshPart>
{
	public struct Enumerator : IEnumerator<ModelMeshPart>, IEnumerator, IDisposable
	{
		private readonly ModelMeshPartCollection collection;

		private int position;

		public ModelMeshPart Current => collection[position];

		object IEnumerator.Current => collection[position];

		internal Enumerator(ModelMeshPartCollection collection)
		{
			this.collection = collection;
			position = -1;
		}

		public bool MoveNext()
		{
			position++;
			return position < collection.Count;
		}

		public void Dispose()
		{
		}

		void IEnumerator.Reset()
		{
			position = -1;
		}
	}

	internal ModelMeshPartCollection(IList<ModelMeshPart> list)
		: base(list)
	{
	}

	public new Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}
}
