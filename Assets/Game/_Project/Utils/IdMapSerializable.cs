using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Serializable map with integer keys (IDs) for use in inspector
/// </summary>
/// /// <typeparam name="T">Value type associated with ID.</typeparam>
[Serializable]
public sealed class IdMapSerializable<T> : IEnumerable<KeyValuePair<int, T>>
{
    [SerializeField] private List<IdValuePair<T>> _pairs = new();

    private Dictionary<int, T> _map;
    private bool _isInitialized;

    /// <summary>
    /// Lazily initialized dictionary for fast access by ID.
    /// </summary>
    public IReadOnlyDictionary<int, T> Map
    {
        get
        {
            if (!_isInitialized || _map == null)
                Initialize();
            return _map;
        }
    }

    public T this[int id]
    {
        get
        {
            if (!_isInitialized || _map == null)
                Initialize();
            return _map[id];
        }
    }

    public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
    {
        if (!_isInitialized || _map == null)
            Initialize();
        return _map.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Builds the internal dictionary from the serialized list.
    /// </summary>
    public void Initialize()
    {
        if (_pairs == null)
            _pairs = new List<IdValuePair<T>>();

        _map = _pairs.ToDictionary(pair => pair.Id, pair => pair.Value);
        _isInitialized = true;
    }

#if UNITY_EDITOR
    private void OnValidate() => _isInitialized = false;
#endif

    [System.Serializable]
    public sealed class IdValuePair<K>
    {
        public int Id;
        public K Value;
    }
}