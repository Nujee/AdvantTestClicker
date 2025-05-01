using Leopotam.EcsLite;

public static class ECSExtensions
{
    public static ref T AddOrGet<T>(this EcsPool<T> pool, int entity) where T : struct
    {
        return ref pool.Has(entity) 
            ? ref pool.Get(entity)
            : ref pool.Add(entity);
    }
}