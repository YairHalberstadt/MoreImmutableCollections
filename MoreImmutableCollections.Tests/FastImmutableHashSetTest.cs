using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xunit;

namespace MoreImmutableCollections.Tests
{
	public class FastImmutableHashSetTests
	{
		[Fact]
		public void TestContains()
		{
			var builder = FastImmutableHashSet.CreateBuilder<int>();
			builder.UnionWith(new[] {1, 2, 3, 4});
			var hashSet = builder.MoveToImmutable();
			Assert.True(hashSet.Contains(3));
			Assert.False(hashSet.Contains(5));
		}

		[Fact]
		public void TestSetEquals()
		{
			var builder = FastImmutableHashSet.CreateBuilder<int>();
			builder.UnionWith(new[] { 1, 2, 3, 4 });
			Assert.True(builder.MoveToImmutable().SetEquals(new []{4,3,2,1}.ToFastImmutableHashSet()));
		}

		[Fact]
		public void TestTryGetValue()
		{
			(new[] {4, 3, 2, 1}.ToFastImmutableHashSet()).TryGetValue(4, out var four);
		}
	}
}

	
