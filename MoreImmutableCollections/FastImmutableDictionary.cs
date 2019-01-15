using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace MoreImmutableCollections
{
	public static class FastImmutableDictionary
	{
		/// <summary>
		/// Returns an empty collection.
		/// </summary>
		/// <typeparam name="TKey">The type of keys stored by the dictionary.</typeparam>
		/// <typeparam name="TValue">The type of values stored by the dictionary.</typeparam>
		/// <returns>The immutable collection.</returns>
		[Pure]
		public static FastImmutableDictionary<TKey, TValue> Create<TKey, TValue>()
		{
			return FastImmutableDictionary<TKey, TValue>.Empty;
		}

		/// <summary>
		/// Returns an empty collection with the specified key comparer.
		/// </summary>
		/// <typeparam name="TKey">The type of keys stored by the dictionary.</typeparam>
		/// <typeparam name="TValue">The type of values stored by the dictionary.</typeparam>
		/// <param name="keyComparer">The key comparer.</param>
		/// <returns>
		/// The immutable collection.
		/// </returns>
		[Pure]
		public static FastImmutableDictionary<TKey, TValue> Create<TKey, TValue>(IEqualityComparer<TKey> comparer)
		{
			return FastImmutableDictionary<TKey, TValue>.Empty.WithComparer(comparer);
		}

		/// <summary>
		/// Creates a new immutable collection prefilled with the specified items.
		/// </summary>
		/// <typeparam name="TKey">The type of keys stored by the dictionary.</typeparam>
		/// <typeparam name="TValue">The type of values stored by the dictionary.</typeparam>
		/// <param name="items">The items to prepopulate.</param>
		/// <returns>The new immutable collection.</returns>
		[Pure]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static FastImmutableDictionary<TKey, TValue> CreateRange<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items)
		{
			var dictionary = new Dictionary<TKey, TValue>();
			foreach (var item in items)
			{
				dictionary.Add(item.Key, item.Value);
			}

			return new FastImmutableDictionary<TKey, TValue>(dictionary);
		}

		/// <summary>
		/// Creates a new immutable collection prefilled with the specified items.
		/// </summary>
		/// <typeparam name="TKey">The type of keys stored by the dictionary.</typeparam>
		/// <typeparam name="TValue">The type of values stored by the dictionary.</typeparam>
		/// <param name="comparer">The key comparer.</param>
		/// <param name="items">The items to prepopulate.</param>
		/// <returns>The new immutable collection.</returns>
		[Pure]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static FastImmutableDictionary<TKey, TValue> CreateRange<TKey, TValue>(IEqualityComparer<TKey> comparer, IEnumerable<KeyValuePair<TKey, TValue>> items)
		{
			var dictionary = new Dictionary<TKey, TValue>(comparer);
			foreach (var item in items)
			{
				dictionary.Add(item.Key, item.Value);
			}

			return new FastImmutableDictionary<TKey, TValue>(dictionary);
		}

		/// <summary>
		/// Creates a new immutable dictionary builder.
		/// </summary>
		/// <typeparam name="TKey">The type of keys stored by the dictionary.</typeparam>
		/// <typeparam name="TValue">The type of values stored by the dictionary.</typeparam>
		/// <returns>The new builder.</returns>
		[Pure]
		public static FastImmutableDictionary<TKey, TValue>.Builder CreateBuilder<TKey, TValue>()
		{
			return new FastImmutableDictionary<TKey, TValue>.Builder();
		}

		/// <summary>
		/// Creates a new immutable dictionary builder.
		/// </summary>
		/// <typeparam name="TKey">The type of keys stored by the dictionary.</typeparam>
		/// <typeparam name="TValue">The type of values stored by the dictionary.</typeparam>
		/// <param name="comparer">The key comparer.</param>
		/// <returns>The new builder.</returns>
		[Pure]
		public static FastImmutableDictionary<TKey, TValue>.Builder CreateBuilder<TKey, TValue>(IEqualityComparer<TKey> comparer)
		{
			return new FastImmutableDictionary<TKey, TValue>.Builder(comparer);
		}

		/// <summary>
		/// Constructs an immutable dictionary based on some transformation of a sequence.
		/// </summary>
		/// <typeparam name="TSource">The type of element in the sequence.</typeparam>
		/// <typeparam name="TKey">The type of key in the resulting map.</typeparam>
		/// <typeparam name="TValue">The type of value in the resulting map.</typeparam>
		/// <param name="source">The sequence to enumerate to generate the map.</param>
		/// <param name="keySelector">The function that will produce the key for the map from each sequence element.</param>
		/// <param name="elementSelector">The function that will produce the value for the map from each sequence element.</param>
		/// <param name="comparer">The key comparer to use for the map.</param>
		/// <returns>The immutable map.</returns>
		[Pure]
		public static FastImmutableDictionary<TKey, TValue> ToFastImmutableDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
			if (elementSelector == null) throw new ArgumentNullException(nameof(elementSelector));

			return new FastImmutableDictionary<TKey, TValue>(
				source.ToDictionary(keySelector, elementSelector, comparer));
		}

		/// <summary>
		/// Returns an immutable copy of the current contents of the builder's collection.
		/// </summary>
		/// <param name="builder">The builder to create the immutable dictionary from.</param>
		/// <returns>An immutable dictionary.</returns>
		[Pure]
		public static FastImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(this FastImmutableDictionary<TKey, TValue>.Builder builder)
		{
			if (builder == null) throw new ArgumentNullException(nameof(builder));
			return builder.ToImmutable();
		}

		/// <summary>
		/// Constructs an immutable dictionary based on some transformation of a sequence.
		/// </summary>
		/// <typeparam name="TSource">The type of element in the sequence.</typeparam>
		/// <typeparam name="TKey">The type of key in the resulting map.</typeparam>
		/// <param name="source">The sequence to enumerate to generate the map.</param>
		/// <param name="keySelector">The function that will produce the key for the map from each sequence element.</param>
		/// <returns>The immutable map.</returns>
		[Pure]
		public static FastImmutableDictionary<TKey, TSource> ToFastImmutableDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return ToFastImmutableDictionary(source, keySelector, v => v, null);
		}

		/// <summary>
		/// Constructs an immutable dictionary based on some transformation of a sequence.
		/// </summary>
		/// <typeparam name="TSource">The type of element in the sequence.</typeparam>
		/// <typeparam name="TKey">The type of key in the resulting map.</typeparam>
		/// <param name="source">The sequence to enumerate to generate the map.</param>
		/// <param name="keySelector">The function that will produce the key for the map from each sequence element.</param>
		/// <param name="keyComparer">The key comparer to use for the map.</param>
		/// <returns>The immutable map.</returns>
		[Pure]
		public static FastImmutableDictionary<TKey, TSource> ToFastImmutableDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return ToFastImmutableDictionary(source, keySelector, v => v, comparer);
		}

		/// <summary>
		/// Constructs an immutable dictionary based on some transformation of a sequence.
		/// </summary>
		/// <typeparam name="TSource">The type of element in the sequence.</typeparam>
		/// <typeparam name="TKey">The type of key in the resulting map.</typeparam>
		/// <typeparam name="TValue">The type of value in the resulting map.</typeparam>
		/// <param name="source">The sequence to enumerate to generate the map.</param>
		/// <param name="keySelector">The function that will produce the key for the map from each sequence element.</param>
		/// <param name="elementSelector">The function that will produce the value for the map from each sequence element.</param>
		/// <returns>The immutable map.</returns>
		[Pure]
		public static FastImmutableDictionary<TKey, TValue> ToFastImmutableDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
		{
			return ToFastImmutableDictionary(source, keySelector, elementSelector, null);
		}

		/// <summary>
		/// Creates an immutable dictionary given a sequence of key=value pairs.
		/// </summary>
		/// <typeparam name="TKey">The type of key in the map.</typeparam>
		/// <typeparam name="TValue">The type of value in the map.</typeparam>
		/// <param name="source">The sequence of key=value pairs.</param>
		/// <param name="comparer">The key comparer to use when building the immutable map.</param>
		/// <returns>An immutable map.</returns>
		[Pure]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static FastImmutableDictionary<TKey, TValue> ToFastImmutableDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey> comparer)
		{
			if (source == null) throw new ArgumentException(nameof(source));
			if (source is FastImmutableDictionary<TKey, TValue> existingDictionary)
				return existingDictionary.WithComparer(comparer);

			return CreateRange(comparer, source);
		}



		/// <summary>
		/// Creates an immutable dictionary given a sequence of key=value pairs.
		/// </summary>
		/// <typeparam name="TKey">The type of key in the map.</typeparam>
		/// <typeparam name="TValue">The type of value in the map.</typeparam>
		/// <param name="source">The sequence of key=value pairs.</param>
		/// <returns>An immutable map.</returns>
		[Pure]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static FastImmutableDictionary<TKey, TValue> ToFastImmutableDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
		{
			return ToFastImmutableDictionary(source, null);
		}

		/// <summary>
		/// Gets the value for a given key if a matching key exists in the dictionary.
		/// </summary>
		/// <param name="dictionary">The dictionary to retrieve the value from.</param>
		/// <param name="key">The key to search for.</param>
		/// <returns>The value for the key, or the default value of type <typeparamref name="TValue"/> if no matching key was found.</returns>
		[Pure]
		public static TValue GetValueOrDefault<TKey, TValue>(this FastImmutableDictionary<TKey, TValue> dictionary, TKey key)
		{
			return GetValueOrDefault(dictionary, key, default(TValue));
		}

		/// <summary>
		/// Gets the value for a given key if a matching key exists in the dictionary.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary to retrieve the value from.</param>
		/// <param name="key">The key to search for.</param>
		/// <param name="defaultValue">The default value to return if no matching key is found in the dictionary.</param>
		/// <returns>
		/// The value for the key, or <paramref name="defaultValue"/> if no matching key was found.
		/// </returns>
		[Pure]
		public static TValue GetValueOrDefault<TKey, TValue>(this FastImmutableDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			if (dictionary.TryGetValue(key, out var value))
			{
				return value;
			}

			return defaultValue;
		}
	}
}
