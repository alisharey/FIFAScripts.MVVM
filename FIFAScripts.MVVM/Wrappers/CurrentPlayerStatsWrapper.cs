using System.Collections;
using CommunityToolkit.Mvvm.Messaging;
using FIFAScripts.MVVM.Messages;

namespace FIFAScripts.MVVM.Wrappers;

public class CurrentPlayerStatsWrapper<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _playerStats;

    private IMessenger? Messenger { get; }

    public CurrentPlayerStatsWrapper() => _playerStats = new Dictionary<TKey, TValue>();

    public CurrentPlayerStatsWrapper(IMessenger messanger)
    {
        _playerStats = new Dictionary<TKey, TValue>();
        Messenger = messanger;
    }

    public CurrentPlayerStatsWrapper(IMessenger messanger, Dictionary<TKey, TValue> playerStats)
    {
        _playerStats = playerStats;
        Messenger = messanger;
    }

    public TValue this[TKey key]
    {
        get => GetStat(key);
        set
        {
            AddOrUpdateStat(key, value);
            Messenger?.Send(new ForceUpdateStatsMessage());
        }
    }

    // Method to add or update player statistics
    public void AddOrUpdateStat(TKey key, TValue value)
    {
        _playerStats[key] = value;
    }

    // Method to retrieve player statistics
    public TValue GetStat(TKey key)
    {
        if (_playerStats.TryGetValue(key, out TValue? value))
        {
            return value;
        }
        // Return default value if key not found
        return default!;
    }

    // Method to call a function based on player statistics
    public void CallFunctionBasedOnStat(Func<TValue, bool> condition, Action action)
    {
        foreach (var kvp in _playerStats)
        {
            if (condition(kvp.Value))
            {
                action();
                return; // Assuming we want to execute the action only once
            }
        }
    }

    public Dictionary<TKey, TValue> GetPlayerStatsDict()
    {
        return _playerStats;
    }

    // Implementation of IEnumerable<KeyValuePair<TKey, TValue>> for iteration
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _playerStats.GetEnumerator();
    }

    // Explicit implementation of IEnumerable.GetEnumerator required by the interface
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}