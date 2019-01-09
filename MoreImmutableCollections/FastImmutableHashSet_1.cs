using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Collections.Immutable
{
	/// <summary>
	/// An immutable unordered hash set implementation, optimized for fast lookups at the expense of reuasability.
	/// </summary>
	/// <typeparam name="T">The type of elements in the set.</typeparam>
	[DebuggerDisplay("Count = {" + nameof(Count) + "}")]
	public struct FastImmutableHashSet<T> : ISet<T>, IEquatable<FastImmutableHashSet<T>>
	{
		/// <summary>
		///  An empty (initialized) instance of <see cref="FastImmutableHashSet{T}"/>.
		/// </summary>
		public static readonly FastImmutableHashSet<T> Empty = new FastImmutableHashSet<T>(new HashSet<T>());

		/// <summary>
		/// The backing field for this instance. References to this value should never be shared with outside code.
		/// </summary>
		/// <remarks>
		/// This would be private, but we make it internal so that our own extension methods can access it.
		/// </remarks>
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		internal HashSet<T> hashSet;

		/// <summary>
		/// Initializes a new instance of the <see cref="FastImmutableHashSet{T}"/> struct
		/// *without making a defensive copy*.
		/// </summary>
		/// <param name="items">The hashset to use. May be null for "default" hashSets.</param>
		internal FastImmutableHashSet(HashSet<T> items)
		{
			this.hashSet = items;
		}

		#region Operators

		/// <summary>
		/// Checks equality between two instances.
		/// </summary>
		/// <param name="left">The instance to the left of the operator.</param>
		/// <param name="right">The instance to the right of the operator.</param>
		/// <returns><c>true</c> if the values' underlying hashSets are reference equal; <c>false</c> otherwise.</returns>
		public static bool operator ==(FastImmutableHashSet<T> left, FastImmutableHashSet<T> right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Checks inequality between two instances.
		/// </summary>
		/// <param name="left">The instance to the left of the operator.</param>
		/// <param name="right">The instance to the right of the operator.</param>
		/// <returns><c>true</c> if the values' underlying hashSets are reference not equal; <c>false</c> otherwise.</returns>
		public static bool operator !=(FastImmutableHashSet<T> left, FastImmutableHashSet<T> right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		/// Checks equality between two instances.
		/// </summary>
		/// <param name="left">The instance to the left of the operator.</param>
		/// <param name="right">The instance to the right of the operator.</param>
		/// <returns><c>true</c> if the values' underlying hashSets are reference equal; <c>false</c> otherwise.</returns>
		public static bool operator ==(FastImmutableHashSet<T>? left, FastImmutableHashSet<T>? right)
		{
			return left.GetValueOrDefault().Equals(right.GetValueOrDefault());
		}

		/// <summary>
		/// Checks inequality between two instances.
		/// </summary>
		/// <param name="left">The instance to the left of the operator.</param>
		/// <param name="right">The instance to the right of the operator.</param>
		/// <returns><c>true</c> if the values' underlying hashSets are reference not equal; <c>false</c> otherwise.</returns>
		public static bool operator !=(FastImmutableHashSet<T>? left, FastImmutableHashSet<T>? right)
		{
			return !left.GetValueOrDefault().Equals(right.GetValueOrDefault());
		}

		#endregion

		/// <summary>
		/// See the <see cref="IImmutableSet{T}"/> interface.
		/// </summary>
		public int Count
		{
			get { return hashSet.Count; }
		}

		/// <summary>
		/// See the <see cref="IImmutableSet{T}"/> interface.
		/// </summary>
		public bool IsEmpty
		{
			get { return this.Count == 0; }
		}

		/// <summary>
		/// Gets a value indicating whether this struct was initialized without an actual hashset instance.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsDefault
		{
			get { return this.hashSet == null; }
		}

		/// <summary>
		/// Gets a value indicating whether this struct is empty or uninitialized.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsDefaultOrEmpty
		{
			get
			{
				var self = this;
				return self.hashSet == null || self.Count == 0;
			}
		}

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

			var builder = new Builder(hashSet.Comparer);
			builder.UnionWith(hashSet);
			return builder;
		}

		/// <summary>
		/// Returns an enumerator for the contents of the hashset.
		/// </summary>
		/// <returns>An enumerator.</returns>
		[Pure]
		public HashSet<T>.Enumerator GetEnumerator()
		{
			var self = this;
			self.ThrowNullRefIfNotInitialized();
			return self.hashSet.GetEnumerator();
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
			var self = this;
			return self.hashSet == null ? 0 : self.hashSet.GetHashCode();
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
			if (obj is FastImmutableHashSet<T> other)
			{
				return this.hashSet == other.hashSet;
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
		public bool Equals(FastImmutableHashSet<T> other)
		{
			return this.hashSet == other.hashSet;
		}

		/// <summary>
		/// Returns an enumerator for the contents of the hashSet.
		/// </summary>
		/// <returns>An enumerator.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the <see cref="IsDefault"/> property returns true.</exception>
		[Pure]
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Returns an enumerator for the contents of the hashSet.
		/// </summary>
		/// <returns>An enumerator.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the <see cref="IsDefault"/> property returns true.</exception>
		[Pure]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Throws a null reference exception if the hashSet field is null.
		/// </summary>
		internal void ThrowNullRefIfNotInitialized()
		{
			// Force NullReferenceException if hashSet is null by touching its Length.
			// This way of checking has a nice property of requiring very little code
			// and not having any conditions/branches.
			// In a faulting scenario we are relying on hardware to generate the fault.
			// And in the non-faulting scenario (most common) the check is virtually free since
			// if we are going to do anything with the hashSet, we will need Length anyways
			// so touching it, and potentially causing a cache miss, is not going to be an
			// extra expense.
			var unused = this.hashSet.Count;
		}

		/// <summary>
		/// Throws an <see cref="InvalidOperationException"/> if the <see cref="hashSet"/> field is null, i.e. the
		/// <see cref="IsDefault"/> property returns true.  The
		/// <see cref="InvalidOperationException"/> message specifies that the operation cannot be performed
		/// on a default instance of <see cref="ImmutablehashSet{T}"/>.
		///
		/// This is intended for explicitly implemented interface method and property implementations.
		/// </summary>
		private void ThrowInvalidOperationIfNotInitialized()
		{
			if (this.IsDefault)
			{
				throw new InvalidOperationException(@"This operation cannot be performed on a default instance of FastImmutableHashSet<T>.  Consider initializing the array, or checking the FastImmutableHashSet<T>.IsDefault property.");
			}
		}

		public bool Contains(T value)
		{
			return hashSet.Contains(value);
		}

		#region Builder

		/// <summary>
		/// A writable hashSet accessor that can be converted into an <see cref="HashSet{T}"/>
		/// instance without allocating memory.
		/// </summary>
		[DebuggerDisplay("Count = {Count}")]
		public sealed class Builder : ISet<T>, IReadOnlyCollection<T>
		{
			/// <summary>
			/// The backing hashSet for the builder.
			/// </summary>
			private HashSet<T> _hashSet;

			/// <summary>
			/// Initializes a new instance of the <see cref="Builder"/> class.
			/// </summary>
			internal Builder()
			{
				_hashSet = new HashSet<T>();
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Builder"/> class that uses the specified equality comparer for the set type.
			/// </summary>
			/// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the set,
			/// or null to use the default <see cref="IEqualityComparer{T}"/> implementation for the set type.</param>
			internal Builder(IEqualityComparer<T> comparer)
			{
				_hashSet = new HashSet<T>(comparer);
			}

			void ICollection<T>.Add(T item)
			{
				_hashSet.Add(item);
			}

			public void ExceptWith(IEnumerable<T> other)
			{
				_hashSet.ExceptWith(other);
			}

			public void IntersectWith(IEnumerable<T> other)
			{
				_hashSet.IntersectWith(other);
			}

			public bool IsProperSubsetOf(IEnumerable<T> other)
			{
				return _hashSet.IsProperSubsetOf(other);
			}

			public bool IsProperSupersetOf(IEnumerable<T> other)
			{
				return _hashSet.IsProperSupersetOf(other);
			}

			public bool IsSubsetOf(IEnumerable<T> other)
			{
				return _hashSet.IsSubsetOf(other);
			}

			public bool IsSupersetOf(IEnumerable<T> other)
			{
				return _hashSet.IsSupersetOf(other);
			}

			public bool Overlaps(IEnumerable<T> other)
			{
				return _hashSet.Overlaps(other);
			}

			public bool SetEquals(IEnumerable<T> other)
			{
				return _hashSet.SetEquals(other);
			}

			public void SymmetricExceptWith(IEnumerable<T> other)
			{
				_hashSet.SymmetricExceptWith(other);
			}

			public void UnionWith(IEnumerable<T> other)
			{
				_hashSet.UnionWith(other);
			}

			public bool Add(T item)
			{
				return _hashSet.Add(item);
			}

			public void Clear()
			{
				_hashSet.Clear();
			}

			public bool Contains(T item)
			{
				return _hashSet.Contains(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				_hashSet.CopyTo(array, arrayIndex);
			}

			public bool Remove(T item)
			{
				return _hashSet.Remove(item);
			}

			public int Count => _hashSet.Count;
			bool ICollection<T>.IsReadOnly => false;

			/// <summary>
			/// Returns an immutable copy of the current contents of this collection.
			/// </summary>
			/// <returns>An immutable hashSet.</returns>
			public FastImmutableHashSet<T> ToImmutable()
			{
				return new FastImmutableHashSet<T>(new HashSet<T>(this));
			}

			/// <summary>
			/// Extracts the internal hashSet as an <see cref="FastImmutableHashSet{T}"/> and replaces it 
			/// with a zero length hashSet.
			/// </summary>
			public FastImmutableHashSet<T> MoveToImmutable()
			{
				var hashSet = _hashSet;
				_hashSet = new HashSet<T>();
				return new FastImmutableHashSet<T>(hashSet);
			}

			public IEnumerator<T> GetEnumerator()
			{
				return _hashSet.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable) _hashSet).GetEnumerator();
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Checks whether a given sequence of items entirely describe the contents of this set.
		/// </summary>
		/// <param name="other">The sequence of items to check against this set.</param>
		/// <returns>A value indicating whether the sets are equal.</returns>
		[Pure]
		public bool SetEquals(IEnumerable<T> other)
		{
			if (other == null) throw new ArgumentNullException(nameof(other));

			if (object.ReferenceEquals(this, other))
			{
				return true;
			}

			return hashSet.SetEquals(other);
		}

		/// <summary>
		/// Determines whether the current set is a property (strict) subset of a specified collection.
		/// </summary>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <returns>true if the current set is a correct subset of <paramref name="other"/>; otherwise, false.</returns>
		[Pure]
		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			if (other == null) throw new ArgumentNullException(nameof(other));

			return hashSet.IsProperSubsetOf(other);
		}

		/// <summary>
		/// Determines whether the current set is a correct superset of a specified collection.
		/// </summary>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <returns>true if the current set is a correct superset of <paramref name="other"/>; otherwise, false.</returns>
		[Pure]
		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			if (other == null) throw new ArgumentNullException(nameof(other));

			return hashSet.IsProperSupersetOf(other);
		}

		/// <summary>
		/// Determines whether a set is a subset of a specified collection.
		/// </summary>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <returns>true if the current set is a subset of <paramref name="other"/>; otherwise, false.</returns>
		[Pure]
		public bool IsSubsetOf(IEnumerable<T> other)
		{
			if (other == null) throw new ArgumentNullException(nameof(other));

			return hashSet.IsSubsetOf(other);
		}

		/// <summary>
		/// Determines whether the current set is a superset of a specified collection.
		/// </summary>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <returns>true if the current set is a superset of <paramref name="other"/>; otherwise, false.</returns>
		[Pure]
		public bool IsSupersetOf(IEnumerable<T> other)
		{
			if (other == null) throw new ArgumentNullException(nameof(other));

			return hashSet.IsSupersetOf(other);
		}

		/// <summary>
		/// Determines whether the current set overlaps with the specified collection.
		/// </summary>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <returns>true if the current set and <paramref name="other"/> share at least one common element; otherwise, false.</returns>
		[Pure]
		public bool Overlaps(IEnumerable<T> other)
		{
			if (other == null) throw new ArgumentNullException(nameof(other));

			return hashSet.Overlaps(other);
		}

		#endregion

		#region IImmutableSet<T> Methods

		/// <summary>
		/// See the <see cref="IImmutableSet{T}"/> interface.
		/// </summary>
		[Pure]
		public FastImmutableHashSet<T> WithComparer(IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}

			if (equalityComparer == hashSet.Comparer)
			{
				return this;
			}

			var builder = new Builder(equalityComparer);
			builder.UnionWith(this);
			return builder.ToImmutable();
		}

		#endregion

		#region ICollection<T> members

		/// <summary>
		/// See the <see cref="ICollection{T}"/> interface.
		/// </summary>
		bool ICollection<T>.IsReadOnly => true;

		/// <summary>
		/// See the <see cref="ICollection{T}"/> interface.
		/// </summary>
		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			hashSet.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// See the <see cref="IList{T}"/> interface.
		/// </summary>
		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// See the <see cref="ICollection{T}"/> interface.
		/// </summary>
		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// See the <see cref="IList{T}"/> interface.
		/// </summary>
		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		private delegate bool TryGetValueDelegate(HashSet<T> hashSet, T equalValue, out T actualValue);

		private static readonly TryGetValueDelegate _tryGetValueDelegate = (TryGetValueDelegate) typeof(HashSet<T>).GetMethod("TryGetValue")?.CreateDelegate(typeof(TryGetValueDelegate));

		public bool TryGetValue(T equalValue, out T actualValue)
		{
			if(_tryGetValueDelegate != null)
				return _tryGetValueDelegate(hashSet, equalValue, out actualValue);

			actualValue = default;
			throw new NotSupportedException("HashSet.TryGetValue doesn't exist on this platform. Try upgrading to .Net Core 2.0, .Net Framework 4.7.2, or .Net Standard 2.1");
		}

		bool ISet<T>.Add(T item)
		{
			throw new NotSupportedException();
		}

		void ISet<T>.ExceptWith(IEnumerable<T> other)
		{
			throw new NotSupportedException();
		}

		void ISet<T>.IntersectWith(IEnumerable<T> other)
		{
			throw new NotSupportedException();
		}

		void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
		{
			throw new NotSupportedException();
		}

		void ISet<T>.UnionWith(IEnumerable<T> other)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
