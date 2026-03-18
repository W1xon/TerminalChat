using System.Collections.Immutable;

namespace SignalRLearn;

public class GroupRegistry
{
    private ImmutableHashSet<Group> _storage = ImmutableHashSet<Group>.Empty;

    public bool IsTrust(Group group)
    {
        if(_storage.Any(g => g.Id == group.Id))
            return _storage.Any(g => g.Id == group.Id && g.KeyWord == group.KeyWord);

        ImmutableInterlocked.Update(ref _storage, s => s.Add(group));
        return true;
    }
}