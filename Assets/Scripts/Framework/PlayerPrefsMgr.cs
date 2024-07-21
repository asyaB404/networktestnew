using UnityEngine;

public static class PlayerPrefsMgr
{
    public static string PlayerName
    {
        get => PlayerPrefs.GetString("player_name", "noob");
        set => PlayerPrefs.SetString("player_name", value);
    }
}