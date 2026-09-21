namespace GOS.Common
{
    public interface IEngineModule
    {
        IEngineSaver Saver { get; }
    }

    public interface IEngineSaver
    {
        string GetString(string key, string defaultValue = null);
        
        bool SetString(string key, string value);
    }
}