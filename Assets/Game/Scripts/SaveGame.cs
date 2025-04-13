using UnityEngine;

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
    public Vector3 playerRotation = Vector3.zero;

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

    // Update is called once per frame
    void Update()
    {
        // Realiza saves em tempo de execução.
        SavePlayerTransform();
    }

    private void GetPlayerTransform()
    {
        if (PlayerPrefs.HasKey("playerPositionX") && PlayerPrefs.HasKey("playerPositionY") && PlayerPrefs.HasKey("playerPositionZ"))
        {
            playerPosition = new Vector3(PlayerPrefs.GetFloat("playerPositionX"), PlayerPrefs.GetFloat("playerPositionY"), PlayerPrefs.GetFloat("playerPositionZ"));
        }

        if (PlayerPrefs.HasKey("playerRotationX") && PlayerPrefs.HasKey("playerRotationY") && PlayerPrefs.HasKey("playerRotationZ"))
        {
            playerRotation = new Vector3(PlayerPrefs.GetFloat("playerRotationX"), PlayerPrefs.GetFloat("playerRotationY"), PlayerPrefs.GetFloat("playerRotationZ"));
        }
    }

    private void SavePlayerTransform()
    {
        PlayerPrefs.SetFloat("playerPositionX", playerPosition.x);
        PlayerPrefs.SetFloat("playerPositionY", playerPosition.y);
        PlayerPrefs.SetFloat("playerPositionZ", playerPosition.z);

        PlayerPrefs.SetFloat("playerRotationX", playerRotation.x);
        PlayerPrefs.SetFloat("playerRotationY", playerRotation.y);
        PlayerPrefs.SetFloat("playerRotationZ", playerRotation.z);
    }
}
