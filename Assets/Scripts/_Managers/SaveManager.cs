public class SaveManager
{
    public void LocalSave<T>(string key, T value)
    {
        ES3.Save(key, value);
    }

    public T LocalLoad<T>(string key, T defaultValue)
    {
        return ES3.Load<T>(key, defaultValue);
    }
}
