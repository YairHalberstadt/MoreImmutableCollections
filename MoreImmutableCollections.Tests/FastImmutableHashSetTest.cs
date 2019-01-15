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
#pragma warning disable xUnit2017 // Do not use Contains() to check if a value exists in a collection
			Assert.True(hashSet.Contains(3));
			Assert.False(hashSet.Contains(5));
#pragma warning restore xUnit2017 // Do not use Contains() to check if a value exists in a collection
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

		[Fact]
		public void TestOperators()
		{
			var dic = new[] { 1, 2, 3 }.ToFastImmutableHashSet();
			var dic2 = new[] { 1, 2, 3 }.ToFastImmutableHashSet();

			// ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
			Assert.True(dic == dic);
#pragma warning restore CS1718 // Comparison made to same variable
			Assert.True(((FastImmutableHashSet<int>)null) == null);
			Assert.False(dic == null);
			Assert.False(null == dic);
			Assert.False(dic == dic2);

			// ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
			Assert.False(dic != dic);
#pragma warning restore CS1718 // Comparison made to same variable
			Assert.False(((FastImmutableHashSet<int>)null) != null);
			Assert.True(dic != null);
			Assert.True(null != dic);
			Assert.True(dic != dic2);
		}
	}
}

	
