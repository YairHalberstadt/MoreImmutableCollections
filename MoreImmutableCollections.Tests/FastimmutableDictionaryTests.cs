using System;
using System.Collections.Generic;
using Xunit;

namespace MoreImmutableCollections.Tests
{
	public class FastImmutableDictionaryTests
	{
		[Fact]
		public void TestTryGetValue()
		{
			var builder = FastImmutableDictionary.CreateBuilder<int, string>();
			builder.Add(1, "one");
			builder.Add(2, "two");
			builder.Add(3, "three");
			var dict = builder.MoveToImmutable();
			Assert.True(dict.TryGetValue(2, out var result));
			Assert.Equal("two", result);
			Assert.False(dict.TryGetValue(4, out result));
			Assert.Null(result);
		}

		[Fact]
		public void TestIndexer()
		{
			var values = new List<(int, string)>
			{
				(1, "one"),
				(2, "two")
			};
			var dict = values.ToFastImmutableDictionary(x => x.Item1, x => x.Item2);
			Assert.Equal("two", dict[2]);
			Assert.Throws<KeyNotFoundException>(() => dict[3]);
			Assert.Throws<NotSupportedException>(() => ((IDictionary<int, string>) dict)[2] = "three");
		}

		[Fact]
		public void TestOperators()
		{
			var dic = new[] {1, 2, 3}.ToFastImmutableDictionary(x => x);
			var dic2 = new[] { 1, 2, 3 }.ToFastImmutableDictionary(x => x);

			// ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
			Assert.True(dic == dic);
#pragma warning restore CS1718 // Comparison made to same variable
			Assert.True(((FastImmutableDictionary<int, int>)null) == null);
			Assert.False(dic == null);
			Assert.False(null == dic);
			Assert.False(dic == dic2);

			// ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
			Assert.False(dic != dic);
#pragma warning restore CS1718 // Comparison made to same variable
			Assert.False(((FastImmutableDictionary<int, int>)null) != null);
			Assert.True(dic != null);
			Assert.True(null != dic);
			Assert.True(dic != dic2);
		}
	}
}