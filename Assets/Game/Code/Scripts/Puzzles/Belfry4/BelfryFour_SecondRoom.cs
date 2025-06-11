using System.Collections;
using UnityEngine;

public class BelfryFour_SecondRoom : MonoBehaviour
{
    [SerializeField, Tooltip("Os objetos dem estar na layer InteractableObjects.")] GameObject computer; // O computador na cena.
    [SerializeField, Tooltip("Precisam ter suas MeshRenderer ativadas para serem desativadas quando o player interagir com cada computador.")] GameObject screenComputer; // A tela do computador na cena.
    [SerializeField] GameObject[] visualOrder = new GameObject[5];
    [SerializeField] GameObject[] screenComputerWinOrLose = new GameObject[2];
    [SerializeField] GameObject[] targets = new GameObject[5];
    [SerializeField] GameObject door;

    private InputActionsManager input;
    private PlayerInteraction playerInteraction;

    private int[] order = { 1, 2, 3, 4, 5 }; // Cada n° representa um elemento do array computers. Se a ordem for 3 , 2, ..., então o computador 3 será o primeiro e assim por diante.
    private int attempt = 0;
    private bool orderIsWrong;
    private bool puzzleStarted;
    private void Start()
    {
        input = InputActionsManager.Instance;
        playerInteraction = FindFirstObjectByType<PlayerInteraction>();

        // Para cada tela.
        for (int i = 0; i < visualOrder.Length; i++)
        {
            visualOrder[i].transform.position = screenComputer.transform.position; // Corrige a posição.
            visualOrder[i].SetActive(false); // Desativa as telas.
        }

        // Para cada tela.
        for (int i = 0; i < screenComputerWinOrLose.Length; i++)
        {
            screenComputerWinOrLose[i].transform.position = screenComputer.transform.position; // Corrige a posição.
            screenComputerWinOrLose[i].SetActive(false); // Desativa as telas.
        }
    }

    private void Update()
    {
        // Ativar o puzzle
        if (computer != null && screenComputer != null)
        {
            if (computer == playerInteraction.NextInteraction())
            {
                if (screenComputer.activeSelf)
                {
                    // Enquanto a tela do computador está desativada (Mesh ativo), não houve interação.
                    playerInteraction.HasInteracted(false);

                    if (input.inputActions.Game.Interaction.WasPressedThisFrame())
                    {
                        StartPuzzle(); // Inicia o puzzle.
                    }
                }
                else
                {
                    // Se a tela do computador está ativada (Mesh desativado), houve interação.
                    playerInteraction.HasInteracted(true);
                }
            }
            else
            {
                if (computer.TryGetComponent<Outline>(out var outline))
                {
                    outline.enabled = false;
                }

            }
        }

        if (puzzleStarted)
        {
            // Pega cada alvo.
            foreach (var target in targets)
            {
                // Confere quando algum alvo for acertado.
                if (target.GetComponent<Target>().WasCollided())
                {
                    TryOrder(target);
                }
            }
        }
    }

    // Inicia o puzzle.
    private void StartPuzzle()
    {
        ChangeOrder(); // Decide a ordem.

        screenComputer.SetActive(false); // Ativa o computador.

        visualOrder[order[attempt] - 1].SetActive(true); // Ativa o na tela do computador o 1° número da ordem.

        puzzleStarted = true;
    }

    // Finaliza o puzzle.
    private IEnumerator FinishPuzzle()
    {
        // Para cada visual order.
        for (int i = 0; i < visualOrder.Length; i++)
        {
            // Desativa a ordem para o player.
            visualOrder[i].SetActive(false);
        }

        if (orderIsWrong)
        {
            ChangeOrder(); // Altera a ordem.
            orderIsWrong = false; // Reseta a variável.
            attempt = 0; // Reseta a variável.
            puzzleStarted = false;

            screenComputerWinOrLose[1].SetActive(true); // Ativa a tela de derrota.

            yield return new WaitForSeconds(1);

            screenComputerWinOrLose[1].SetActive(false);
            screenComputer.SetActive(true);
        }
        else
        {
            screenComputerWinOrLose[0].SetActive(true); // Ativa a tela de Vitória.

            if (door != null)
            {
                door.SetActive(false); // Abrir a porta.
            }

            // Finaliza o puzzle.
            enabled = false;
        }
    }

    private void TryOrder(GameObject target)
    {
        // Se não tiver colidido, a ordem está incorreta.
        if (!(targets[order[attempt] - 1] == target))
        {
            orderIsWrong = true;
        }

        attempt++;

        if (attempt >= 5)
        {
            StartCoroutine(nameof(FinishPuzzle));
            return;
        }

        // Deixar desativado
        for (int i = 0; i < order.Length; i++)
        {
            visualOrder[i].SetActive(false);
        }

        visualOrder[order[attempt] - 1].SetActive(true);
    }

    // Altera a ordem.
    private void ChangeOrder()
    {
        // Criar a ordem.
        for (int i = 0; i < order.Length; i++)
        {
            int j = Random.Range(0, order.Length);

            // Embaralha os arrays.
            (order[j], order[i]) = (order[i], order[j]);
        }

        // Faz com que o player ainda não tenha interagido com nenhum objeto(Isso é mais para o visual, código no PlayerInteraction).
        playerInteraction.HasInteracted(false);
    }
}
