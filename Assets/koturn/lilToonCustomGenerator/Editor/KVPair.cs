using System;
using UnityEngine;


namespace Koturn.LilToonCustomGenerator.Editor
{
    /// <summary>
    /// Serializable key-value pair.
    /// </summary>
    [System.Runtime.InteropServices.Guid("0aac7173-24f7-42a4-c994-c989601408ce")]
    [Serializable]
    public sealed class KVPair<TKey, TValue>
    {
        /// <summary>
        /// Serialize name of backing field of <see cref="Key"/>.
        /// </summary>
        public const string NameOfKey = nameof(_key);
        /// <summary>
        /// Serialize name of backing field of <see cref="Value"/>.
        /// </summary>
        public const string NameOfValue = nameof(_value);

        /// <summary>
        /// Key.
        /// </summary>
        public TKey Key => _key;
        /// <summary>
        /// Value
        /// </summary>
        public TValue Value => _value;

        /// <summary>
        /// Backing field of <see cref="Key"/>.
        /// </summary>
        [SerializeField]
        private TKey _key;
        /// <summary>
        /// Backing field of <see cref="Value"/>.
        /// </summary>
        [SerializeField]
        private TValue _value;

        /// <summary>
        /// Create instance with key and value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">value.</param>
        public KVPair(TKey key, TValue val)
        {
            _key = key;
            _value = val;
        }
    }

    /// <summary>
    /// Helper class for <see cref="KVPair{TKey, TValue}"/>.
    /// </summary>
    public static class KVPair
    {
        /// <summary>
        /// Serialize name of backing field of <see cref="Key"/>.
        /// </summary>
        public const string NameOfKey = KVPair<string, string>.NameOfKey;
        /// <summary>
        /// Serialize name of backing field of <see cref="Value"/>.
        /// </summary>
        public const string NameOfValue = KVPair<string, string>.NameOfValue;

        /// <summary>
        /// Create an instance of <see cref="KVPair{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">Type of key.</typeparam>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="key">Key.</param>
        /// <param name="val">Value.</param>
        /// <returns>An instance of <see cref="KVPair{TKey, TValue}"/></returns>
        public static KVPair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue val)
        {
            return new KVPair<TKey, TValue>(key, val);
        }
    }
}
