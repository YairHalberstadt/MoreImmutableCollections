using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Collections.Immutable
{
	/// <summary>
	/// A set of initialization methods for instances of <see cref="FastImmutableHashSet{T}"/>.
	/// </summary>
	public static class FastImmutableHashSet
	{
		/// <summary>
		/// Returns an empty collection.
		/// </summary>
		/// <typeparam name="T">The type of items stored by the collection.</typeparam>
		/// <returns>The immutable collection.</returns>
		[Pure]
		public static FastImmutableHashSet<T> Create<T>()
		{
			return FastImmutableHashSet<T>.Empty;
		}

		/// <summary>
		/// Returns an empty collection.
		/// </summary>
		/// <typeparam name="T">The type of items stored by the collection.</typeparam>
		/// <param name="equalityComparer">The equality comparer.</param>
		/// <returns>
		/// The immutable collection.
		/// </returns>
		[Pure]
		public static FastImmutableHashSet<T> Create<T>(IEqualityComparer<T> equalityComparer)
		{
			return new FastImmutableHashSet<T>(new HashSet<T>(equalityComparer));
		}

		/// <summary>
		/// Creates a new immutable collection prefilled with the specified item.
		/// </summary>
		/// <typeparam name="T">The type of items stored by the collection.</typeparam>
		/// <param name="item">The item to prepopulate.</param>
		/// <returns>The new immutable collection.</returns>
		[Pure]
		public static FastImmutableHashSet<T> Create<T>(T item)
		{
			return new FastImmutableHashSet<T>(new HashSet<T> {item});
		}

		/// <summary>
		/// Creates a new immutable collection prefilled with the specified item.
		/// </summary>
		/// <typeparam name="T">The type of items stored by the collection.</typeparam>
		/// <param name="equalityComparer">The equality comparer.</param>
		/// <param name="item">The item to prepopulate.</param>
		/// <returns>The new immutable collection.</returns>
		[Pure]
		public static FastImmutableHashSet<T> Create<T>(IEqualityComparer<T> equalityComparer, T item)
		{
			return new FastImmutableHashSet<T>(new HashSet<T>(equalityComparer) {item});
		}

		/// <summary>
		/// Creates a new immutable collection prefilled with the specified items.
		/// </summary>
		/// <typeparam name="T">The type of items stored by the collection.</typeparam>
		/// <param name="items">The items to prepopulate.</param>
		/// <returns>The new immutable collection.</returns>
		[Pure]
		public static FastImmutableHashSet<T> CreateRange<T>(IEnumerable<T> items)
		{
			return new FastImmutableHashSet<T>(new HashSet<T>(items));
		}

		/// <summary>
		/// Creates a new immutable collection prefilled with the specified items.
		/// </summary>
		/// <typeparam name="T">The type of items stored by the collection.</typeparam>
		/// <param name="equalityComparer">The equality comparer.</param>
		/// <param name="items">The items to prepopulate.</param>
		/// <returns>The new immutable collection.</returns>
		[Pure]
		public static FastImmutableHashSet<T> CreateRange<T>(IEqualityComparer<T> equalityComparer, IEnumerable<T> items)
		{
			return new FastImmutableHashSet<T>(new HashSet<T>(items, equalityComparer));
		}

		/// <summary>
		/// Creates a new immutable collection prefilled with the specified items.
		/// </summary>
		/// <typeparam name="T">The type of items stored by the collection.</typeparam>
		/// <param name="items">The items to prepopulate.</param>
		/// <returns>The new immutable collection.</returns>
		[Pure]
		public static FastImmutableHashSet<T> Create<T>(params T[] items)
		{
			return new FastImmutableHashSet<T>(new HashSet<T>(items));
		}

		/// <summary>
		/// Creates a new immutable collection prefilled with the specified items.
		/// </summary>
		/// <typeparam name="T">The type of items stored by the collection.</typeparam>
		/// <param name="equalityComparer">The equality comparer.</param>
		/// <param name="items">The items to prepopulate.</param>
		/// <returns>The new immutable collection.</returns>
		[Pure]
		public static FastImmutableHashSet<T> Create<T>(IEqualityComparer<T> equalityComparer, params T[] items)
		{
			return new FastImmutableHashSet<T>(new HashSet<T>(items, equalityComparer));
		}

		/// <summary>
		/// Creates a new immutable hash set builder.
		/// </summary>
		/// <typeparam name="T">The type of items stored by the collection.</typeparam>
		/// <returns>The immutable collection.</returns>
		[Pure]
		public static FastImmutableHashSet<T>.Builder CreateBuilder<T>()
		{
			return Create<T>().ToBuilder();
		}

		/// <summary>
		/// Creates a new immutable hash set builder.
		/// </summary>
		/// <typeparam name="T">The type of items stored by the collection.</typeparam>
		/// <param name="equalityComparer">The equality comparer.</param>
		/// <returns>
		/// The immutable collection.
		/// </returns>
		[Pure]
		public static FastImmutableHashSet<T>.Builder CreateBuilder<T>(IEqualityComparer<T> equalityComparer)
		{
			return Create<T>(equalityComparer).ToBuilder();
		}

		/// <summary>
		/// Enumerates a sequence exactly once and produces an immutable set of its contents.
		/// </summary>
		/// <typeparam name="TSource">The type of element in the sequence.</typeparam>
		/// <param name="source">The sequence to enumerate.</param>
		/// <param name="equalityComparer">The equality comparer to use for initializing and adding members to the hash set.</param>
		/// <returns>An immutable set.</returns>
		[Pure]
		public static FastImmutableHashSet<TSource> ToFastImmutableHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> equalityComparer)
		{
			return new FastImmutableHashSet<TSource>(new HashSet<TSource>(source, equalityComparer));
		}

		/// <summary>
		/// Returns an immutable copy of the current contents of the builder's collection.
		/// </summary>
		/// <param name="builder">The builder to create the immutable set from.</param>
		/// <returns>An immutable set.</returns>
		[Pure]
		public static FastImmutableHashSet<TSource> ToFastImmutableHashSet<TSource>(this FastImmutableHashSet<TSource>.Builder builder)
		{
			return builder.ToImmutable();
		}


		/// <summary>
		/// Enumerates a sequence exactly once and produces an immutable set of its contents.
		/// </summary>
		/// <typeparam name="TSource">The type of element in the sequence.</typeparam>
		/// <param name="source">The sequence to enumerate.</param>
		/// <returns>An immutable set.</returns>
		[Pure]
		public static FastImmutableHashSet<TSource> ToFastImmutableHashSet<TSource>(this IEnumerable<TSource> source)
		{
			return ToFastImmutableHashSet(source, null);
		}
	}
}
