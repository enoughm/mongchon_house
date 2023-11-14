using System;

[Serializable]
public class BuildSettingData
{
    public string buildType;
    public string defaultCompany;
    public string productName;
    public string applicationIdentifier;
    public bool useAndroidCustomKeystore;

    public string keyaliasPass;
    public string keystorePass;
    public string androidKeyaliasPass;
    public string androidKeystorePass;

    public string androidKeystoreName;
    public string androidKeyaliasName;

    public int resolutionWidth;
    public int resolutionHeight;
}
