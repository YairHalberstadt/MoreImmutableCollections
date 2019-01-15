using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;

namespace MoreImmutableCollections
{
	/// <summary>
	/// An immutable unordered Dictionary, optimized for fast lookups at the expense of reuasability.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class FastImmutableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IDictionary<TKey, TValue>,  IDictionary
	{
		/// <summary>
		///  An empty (initialized) instance of <see cref="FastImmutableDictionary{TKey, TValue}"/>.
		/// </summary>
		public static readonly FastImmutableDictionary<TKey, TValue> Empty = new FastImmutableDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());

		/// <summary>
		/// The backing field for this instance. References to this value should never be shared with outside code.
		/// </summary>
		/// <remarks>
		/// This would be private, but we make it internal so that our own extension methods can access it.
		/// </remarks>
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		internal readonly Dictionary<TKey, TValue> dictionary;

		/// <summary>
		/// Initializes a new instance of the <see cref="FastImmutableDictionary{TKey, TValue}"/> struct
		/// *without making a defensive copy*.
		/// </summary>
		/// <param name="items">The hashset to use. Not null.</param>
		internal FastImmutableDictionary(Dictionary<TKey, TValue> items)
		{
			this.dictionary = items ?? throw new ArgumentNullException(nameof(dictionary));
		}

		#region Operators

		/// <summary>
		/// Checks equality between two instances.
		/// </summary>
		/// <param name="left">The instance to the left of the operator.</param>
		/// <param name="right">The instance to the right of the operator.</param>
		/// <returns><c>true</c> if the values' underlying dictionaries are reference equal; <c>false</c> otherwise.</returns>
		public static bool operator ==(FastImmutableDictionary<TKey, TValue> left, FastImmutableDictionary<TKey, TValue> right)
		{
			return left?.Equals(right) ?? right is null;
		}

		/// <summary>
		/// Checks inequality between two instances.
		/// </summary>
		/// <param name="left">The instance to the left of the operator.</param>
		/// <param name="right">The instance to the right of the operator.</param>
		/// <returns><c>true</c> if the values' underlying dictionaries are reference not equal; <c>false</c> otherwise.</returns>
		public static bool operator !=(FastImmutableDictionary<TKey, TValue> left, FastImmutableDictionary<TKey, TValue> right)
		{
			return !(left == right);
		}

		#endregion


		/// <summary>
		/// See the <see cref="IImmutableSet{T}"/> interface.
		/// </summary>
		public int Count
		{
			get { return dictionary.Count; }
		}

		/// <summary>
		/// See the <see cref="IImmutableSet{T}"/> interface.
		/// </summary>
		public bool IsEmpty
		{
			get { return this.Count == 0; }
		}

		public IEnumerable<TKey> Keys => dictionary.Keys;

		public IEnumerable<TValue> Values => dictionary.Values;

		ICollection<TKey> IDictionary<TKey, TValue>.Keys => dictionary.Keys;

		ICollection<TValue> IDictionary<TKey, TValue>.Values => dictionary.Values;

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;

		bool IDictionary.IsFixedSize => true;

		bool IDictionary.IsReadOnly => true;

		ICollection IDictionary.Keys => dictionary.Keys;

		ICollection IDictionary.Values => dictionary.Values;

		bool ICollection.IsSynchronized => true;

		object ICollection.SyncRoot => throw new NotSupportedException();

		object IDictionary.this[object key] { get => dictionary[(TKey)key]; set => throw new NotSupportedException(); }
		TValue IDictionary<TKey, TValue>.this[TKey key] { get => dictionary[key]; set => throw new NotSupportedException(); }

		public TValue this[TKey key] => dictionary[key];

		/// <summary>
		/// Returns a builder that is populated with the same contents as this hashset.
		/// </summary>
		/// <returns>The new builder.</returns>
		[Pure]
		public Builder ToBuilder()
		{
			var self = this;
			if (self.Count == 0)
			{
				return new Builder(); // allow the builder to create itself with a reasonable default capacity
			}

			var builder = new Builder(dictionary, dictionary.Comparer);
			return builder;
		}

		/// <summary>
		/// Returns an enumerator for the contents of the hashset.
		/// </summary>
		/// <returns>An enumerator.</returns>
		[Pure]
		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			var self = this;
			return self.dictionary.GetEnumerator();
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		[Pure]
		public override int GetHashCode()
		{
			return dictionary.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified <see cref="Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		[Pure]
		public override bool Equals(object obj)
		{
			if (obj is FastImmutableDictionary<TKey, TValue> other)
			{
				return this.dictionary == other.dictionary;
			}
			return false;
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		[Pure]
		public bool Equals(FastImmutableDictionary<TKey, TValue> other)
		{
			return this.dictionary == other?.dictionary;
		}

		[Pure]
		IDictionaryEnumerator IDictionary.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Returns an enumerator for the contents of the dictionary.
		/// </summary>
		/// <returns>An enumerator.</returns>
		[Pure]
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Returns an enumerator for the contents of the dictionary.
		/// </summary>
		/// <returns>An enumerator.</returns>
		[Pure]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public bool ContainsKey(TKey key)
		{
			return dictionary.ContainsKey(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return dictionary.TryGetValue(key, out value);
		}

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			throw new NotSupportedException();
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			throw new NotSupportedException();
		}

		bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
		{
			throw new NotSupportedException();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>) dictionary).Contains(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		void IDictionary.Add(object key, object value)
		{
			throw new NotSupportedException();
		}

		void IDictionary.Clear()
		{
			throw new NotSupportedException();
		}

		bool IDictionary.Contains(object key)
		{
			if (key is TKey typedKey)
				return dictionary.ContainsKey(typedKey);
			return false;
		}

		void IDictionary.Remove(object key)
		{
			throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection) dictionary).CopyTo(array, index);
		}

		public class Builder : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionary
		{
			/// <summary>
			/// The backing dictionary for the builder.
			/// </summary>
			private Dictionary<TKey, TValue> _dictionary;

			/// <summary>
			/// Initializes a new instance of the <see cref="Builder"/> class.
			/// </summary>
			internal Builder()
			{
				_dictionary = new Dictionary<TKey, TValue>();
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Builder"/> class that uses the specified equality comparer for the set type.
			/// </summary>
			/// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the set,
			/// or null to use the default <see cref="IEqualityComparer{T}"/> implementation for the set type.</param>
			internal Builder(IEqualityComparer<TKey> comparer)
			{
				_dictionary = new Dictionary<TKey, TValue>(comparer);
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Builder"/> class,
			/// that contains elements copied from the specified <see cref="IDictionary{TKey, TValue}"/>
			/// and uses the default equality comparer for the key type
			/// </summary>
			internal Builder(IDictionary<TKey, TValue> dictionary)
			{
				_dictionary = new Dictionary<TKey, TValue>(dictionary);
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Builder"/> class ,
			/// that contains elements copied from the specified <see cref="IDictionary{TKey, TValue}"/>
			/// and uses the specified equality comparer for the set type.
			/// </summary>
			/// <param name="dictionary"></param>
			/// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the set,
			/// or null to use the default <see cref="IEqualityComparer{T}"/> implementation for the set type.</param>
			internal Builder(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			{
				_dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
			}


			public bool Contains(object key)
			{
				return ((IDictionary) _dictionary).Contains(key);
			}

			IDictionaryEnumerator IDictionary.GetEnumerator()
			{
				return ((IDictionary) _dictionary).GetEnumerator();
			}

			public void Remove(object key)
			{
				((IDictionary) _dictionary).Remove(key);
			}

			public bool IsFixedSize => ((IDictionary) _dictionary).IsFixedSize;

			bool IDictionary.IsReadOnly => ((IDictionary) _dictionary).IsReadOnly;

			public object this[object key]
			{
				get => ((IDictionary) _dictionary)[key];
				set => ((IDictionary) _dictionary)[key] = value;
			}

			IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
			{
				return _dictionary.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable) _dictionary).GetEnumerator();
			}

			void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
			{
				((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
			}

			public void Add(object key, object value)
			{
				((IDictionary) _dictionary).Add(key, value);
			}

			void IDictionary.Clear()
			{
				_dictionary.Clear();
			}

			void ICollection<KeyValuePair<TKey, TValue>>.Clear()
			{
				_dictionary.Clear();
			}

			bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
			{
				return ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).Contains(item);
			}

			void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
				((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
			}

			bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
			{
				return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item);
			}

			void ICollection.CopyTo(Array array, int index)
			{
				((ICollection) _dictionary).CopyTo(array, index);
			}

			int ICollection.Count => _dictionary.Count;

			public bool IsSynchronized => ((ICollection) _dictionary).IsSynchronized;

			public object SyncRoot => ((ICollection) _dictionary).SyncRoot;

			int ICollection<KeyValuePair<TKey, TValue>>.Count => _dictionary.Count;

			bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

			public void Add(TKey key, TValue value)
			{
				_dictionary.Add(key, value);
			}

			bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
			{
				return _dictionary.ContainsKey(key);
			}

			bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
			{
				return _dictionary.TryGetValue(key, out value);
			}

			public bool Remove(TKey key)
			{
				return _dictionary.Remove(key);
			}

			bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key)
			{
				return _dictionary.ContainsKey(key);
			}

			bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
			{
				return _dictionary.TryGetValue(key, out value);
			}

			public TValue this[TKey key]
			{
				get => _dictionary[key];
				set => _dictionary[key] = value;
			}

			IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;

			ICollection IDictionary.Values => ((IDictionary) _dictionary).Values;

			ICollection IDictionary.Keys => ((IDictionary) _dictionary).Keys;

			IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

			ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys;

			ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values;

			int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count => _dictionary.Count;

			/// <summary>
			/// Returns an immutable copy of the current contents of this collection.
			/// </summary>
			/// <returns>An immutable hashSet.</returns>
			public FastImmutableDictionary<TKey, TValue> ToImmutable()
			{
				return new FastImmutableDictionary<TKey, TValue>(new Dictionary<TKey, TValue>(this));
			}

			/// <summary>
			/// Extracts the internal hashSet as an <see cref="FastImmutableHashSet{T}"/> and replaces it 
			/// with a zero length hashSet.
			/// </summary>
			public FastImmutableDictionary<TKey, TValue> MoveToImmutable()
			{
				var dictionary = _dictionary;
				_dictionary = new Dictionary<TKey, TValue>();
				return new FastImmutableDictionary<TKey, TValue>(dictionary);
			}
		}

		[Pure]
		public FastImmutableDictionary<TKey, TValue> WithComparer(IEqualityComparer<TKey> comparer)
		{
			return new FastImmutableDictionary<TKey, TValue>(new Dictionary<TKey, TValue>(dictionary, comparer));
		}
	}
}
