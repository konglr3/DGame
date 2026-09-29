using GOS.Common;

namespace GameLogic
{
    
    
    public class GOSEngineModule : IEngineModule
    {
        public IEngineSaver Saver { get; private set; } = new GOSUnitySaver();
    }

    public class GOSUnitySaver : IEngineSaver
    {
        public string GetString(string key, string defaultValue = null)
        {
            return UnityEngine.PlayerPrefs.GetString(key, defaultValue);
        }

        public bool SetString(string key, string value)
        {
            UnityEngine.PlayerPrefs.SetString(key, value);
            return true;
        }
    }
}