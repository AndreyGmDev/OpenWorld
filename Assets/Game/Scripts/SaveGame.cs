using UnityEngine;

public struct SaveGameInfos
{
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;
}

public class SaveGame : MonoBehaviour
{
    // Inicia o Singleton do SaveSame.
    private static SaveGame saveGame;

    public static SaveGame instance
    {
        get
        {
            // Confere se a instância já foi criada
            if (saveGame == null)
            {
                // Procura o SaveGame na cena
                saveGame = FindFirstObjectByType<SaveGame>();

                // Se não encontrar, cria uma nova GameObject com esse script
                if (saveGame == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    saveGame = obj.AddComponent<SaveGame>();
                }
            }
            return saveGame;
        }
    }
    // Finalização do Singleton.

    // Váriaveis do player. Utilizado no Script do Player.
    public Vector3 playerPosition = Vector3.zero;
    public Quaternion playerRotation = Quaternion.identity;

    private void Awake()
    {
        // Permite somente uma instância de SaveGame na cêna.
        if (saveGame == null)
        {
            saveGame = this;
        }
        else if (saveGame != this)
        {
            Destroy(gameObject);
        }

        // Pega os saves criados anteriormente.
        GetPlayerTransform();
    }

    private void GetPlayerTransform()
    {
        if (PlayerPrefs.HasKey("playerPositionX"))
        {
            playerPosition = new Vector3(
                PlayerPrefs.GetFloat("playerPositionX"),
                PlayerPrefs.GetFloat("playerPositionY"),
                PlayerPrefs.GetFloat("playerPositionZ"));
        }

        if (PlayerPrefs.HasKey("playerRotationX"))
        {
            playerRotation = new Quaternion(
                PlayerPrefs.GetFloat("playerRotationX"),
                PlayerPrefs.GetFloat("playerRotationY"),
                PlayerPrefs.GetFloat("playerRotationZ"),
                PlayerPrefs.GetFloat("playerRotationW"));
        }
    }

    // Recebe o SavePlayerTransform do PlayerController.
    public void SavePlayerTransform(in SaveGameInfos infos)
    {
        PlayerPrefs.SetFloat("playerPositionX", infos.PlayerPosition.x);
        PlayerPrefs.SetFloat("playerPositionY", infos.PlayerPosition.y);
        PlayerPrefs.SetFloat("playerPositionZ", infos.PlayerPosition.z);

        PlayerPrefs.SetFloat("playerRotationX", infos.PlayerRotation.x);
        PlayerPrefs.SetFloat("playerRotationY", infos.PlayerRotation.y);
        PlayerPrefs.SetFloat("playerRotationZ", infos.PlayerRotation.z);
        PlayerPrefs.SetFloat("playerRotationW", infos.PlayerRotation.w);
    }
}
