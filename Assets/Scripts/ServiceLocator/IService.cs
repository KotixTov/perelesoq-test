public interface IService
{
    public virtual ServiceContext Context => ServiceContext.Scene;
}

public enum ServiceContext
{
    Scene,
    Project
}