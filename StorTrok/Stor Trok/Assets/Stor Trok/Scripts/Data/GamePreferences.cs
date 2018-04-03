using UnityEngine;

[System.Serializable]
public class GamePreferences {
	public static GamePreferences game_preferences;

	public string last_username;

	public static void save(){
		SaveData.SaveToFile<GamePreferences> (Application.dataPath + "/Resources/game_preferences.dat", game_preferences);
	}
	public static void load(){
		GamePreferences.game_preferences = SaveData.ReadFromFile<GamePreferences>(Application.dataPath + "/Resources/game_preferences.dat");
		if (game_preferences == null) {
			game_preferences = new GamePreferences ();
		}
	}
}
