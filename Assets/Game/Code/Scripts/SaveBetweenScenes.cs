using UnityEngine;

[CreateAssetMenu(fileName = "SaveBetweenScenes", menuName = "Scriptable Objects/SaveBetweenScenes")]
public class SaveBetweenScenes : ScriptableObject
{
    // Conferir se é para carregar o SaveGame no jogo(SaveGame.LoadData()). Se estiver desativado então o save não será carregado no inicio da fase mesmo existindo um save.
    public bool loadSaveGame;

    public SaveGameInfos saveGameInfos;
    public SaveGameInfos newSaveGameInfos;


    public void CanLoadSaveGame(bool canLoad)
    {
        loadSaveGame = canLoad;
    }

    public void BetweenScenesPlayerInfos(in SaveGameInfos infos)
    {
        // Pega todas as informações do saveGameInfos.
        newSaveGameInfos = saveGameInfos;

        // Altera a do player.
        newSaveGameInfos.PlayerPosition = infos.PlayerPosition;
        newSaveGameInfos.PlayerRotation = infos.PlayerRotation;
        newSaveGameInfos.CameraControllerRotation = infos.CameraControllerRotation;

        // Altera o DayCycle
        newSaveGameInfos.Seconds = infos.Seconds;
    }
}
