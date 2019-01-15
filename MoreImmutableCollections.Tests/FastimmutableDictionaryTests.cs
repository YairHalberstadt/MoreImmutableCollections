using System;
using System.Collections.Generic;
using Xunit;

namespace MoreImmutableCollections.Tests
{
	public class FastimmutableDictionaryTests
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
	}
}