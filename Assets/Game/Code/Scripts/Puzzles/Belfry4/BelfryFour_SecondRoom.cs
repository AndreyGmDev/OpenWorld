using UnityEngine;

public class BelfryFour_SecondRoom : MonoBehaviour
{
    [SerializeField, Tooltip("Os objetos dem estar na layer InteractableObjects.")] GameObject computer; // O computador na cena.
    [SerializeField, Tooltip("Precisam ter suas MeshRenderer ativadas para serem desativadas quando o player interagir com cada computador.")] GameObject screenComputer; // A tela do computador na cena.
    [SerializeField] GameObject[] visualOrder = new GameObject[5];
    [SerializeField] GameObject[] targets = new GameObject[5];
    [SerializeField] GameObject door;

    private InputActionsManager input;
    private PlayerInteraction playerInteraction;

    private int[] order = { 1, 2, 3, 4, 5 }; // Cada n° representa um elemento do array computers. Se a ordem for 3 , 2, ..., então o computador 3 será o primeiro e assim por diante.
    private int attempt = 0;
    private bool orderIsWrong;
    private void Start()
    {
        input = InputActionsManager.Instance;
        playerInteraction = FindFirstObjectByType<PlayerInteraction>();

        // Colocar a visual ordem na posição certa
        for (int i = 0; i < visualOrder.Length; i++)
        {
            // Coloca a ordem correta visível para o player.
            visualOrder[i].transform.position = screenComputer.transform.position;
        }
    }

    private void Update()
    {
        // Ativar o puzzle
        if (computer != null && screenComputer != null)
        {
            if (computer == playerInteraction.NextInteraction())
            {
                if (screenComputer.GetComponent<MeshRenderer>().enabled)
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

        // Pega cada alvo.
        foreach (var target in targets)
        {
            // Confere quando algum alvo for acertado.
            if (target.GetComponent<Target>().WasCollided())
            {
                TryOrder(target); // Passa o alvo acertado.
            }
        }
    }

    // Inicia o puzzle.
    private void StartPuzzle()
    {
        ChangeOrder(); // Decide a ordem.

        screenComputer.GetComponent<MeshRenderer>().enabled = false;
    }

    // Finaliza o puzzle.
    private void FinishPuzzle()
    {
        if (orderIsWrong)
        {
            ChangeOrder(); // Altera a ordem.
            orderIsWrong = false; // Reseta a variável.
            attempt = 0; // Reseta a variável.
        }
        else
        {
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
        // Deixar desativado
        for (int i = 0; i < order.Length; i++)
        {
            visualOrder[i].GetComponent<MeshRenderer>().enabled = false;
        }

        visualOrder[order[attempt]].GetComponent<MeshRenderer>().enabled = true;

        // Se não tiver colidido, a ordem está incorreta.
        if (!targets[order[attempt] - 1] == target)
        {
            orderIsWrong = true;
        }

        attempt++;

        if (attempt >= 5)
        {
            FinishPuzzle();
        }
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

        // Desabilita a tela de cada computador.
        screenComputer.GetComponent<MeshRenderer>().enabled = true;

        // Faz com que o player ainda não tenha interagido com nenhum objeto(Isso é mais para o visual, código no PlayerInteraction).
        playerInteraction.HasInteracted(false);
    }
}
