using _0004;
using I;
using l;
using p;
using y;

namespace I
{
	internal abstract class X
	{
		internal y.v a5h;

		public bool Updated;

		public abstract bool ShouldCorrectContactNormal { get; }

		public abstract bool GenerateContactCandidate(out l.W<_0004.b> contactList);

		public abstract _0004._0006 GetRegion(ref _0004.b contact);

		public abstract void Initialize(y.h convex, y.v triangle);

		public abstract void CleanUp();
	}
}
namespace i
{
	internal class X : v
	{
		private new p.a<I.W> a5h = new p.a<I.W>();

		protected override void GiveBackTester(I.X tester)
		{
			a5h.GiveBack((I.W)tester);
		}

		protected override I.X GetTester()
		{
			return a5h.Take();
		}
	}
}
