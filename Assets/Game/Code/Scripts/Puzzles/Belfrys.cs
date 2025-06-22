using UnityEngine;

public class Belfrys : MonoBehaviour
{
    [SerializeField, Tooltip("O teleport de cada belfry, na ordem das belfrys a serem desafiadas")] GameObject[] belfrys;

    private SaveGame saveGame;

    private void Start()
    {
        saveGame = SaveGame.Instance;

        // O número de chapéus coletados significa qual belfry o player deve desafiar no momento.
        // Belfry1 = 0 chapéus.
        // Belfry2 = 1 chapéus.
        // Belfry3 = 2 chapéus.
        // Belfry4 = 3 chapéus.
        int hatCollected = saveGame.LoadData().HatsCollected; 

        // Primeiramente desativa todos os teleportes para cada belfry.
        for (int i = 0; i < belfrys.Length; i++)
        {
            belfrys[i].SetActive(false);
        }

        // Depois ativa o teleporte certo para o player desafiar.
        for (int i = 0; i < belfrys.Length; i++)
        {
            // Confere se tem mais chapéus coletados que o número de belfrys.
            if (hatCollected <= belfrys.Length - 1)
            {
                // Ativa a belfry certa para o player desafiar.
                if (i == hatCollected)
                {
                    belfrys[i].SetActive(true);
                }
            }
        }
    }
}
