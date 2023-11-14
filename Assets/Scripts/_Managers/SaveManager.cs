public class SaveManager
{
    public void LocalSave<T>(LocalSaveKeys key, T value)
    {
        ES3.Save(key.ToString(), value);
    }

    public T LocalLoad<T>(LocalSaveKeys key, T defaultValue)
    {
        return ES3.Load<T>(key.ToString(), defaultValue);
    }
}
