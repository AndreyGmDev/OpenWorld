using UnityEngine;

public class BelfryOne_SecondRoom : MonoBehaviour
{
    [SerializeField, Tooltip("Os objetos dem estar na layer InteractableObjects.")] GameObject[] computers = new GameObject[4]; // Os 4 computadores na cena.
    [SerializeField, Tooltip("Precisam ter suas MeshRenderer desativadas para serem ativadas quando o player interagir com cada computador.")] GameObject[] screenComputers = new GameObject[4]; // As 4 telas de computadores na cena.
    [SerializeField] GameObject[] visualOrder = new GameObject[4];
    [SerializeField] GameObject door;

    private InputActionsManager input;
    private PlayerInteraction playerInteraction;

    private int[] order = { 1, 2, 3, 4 }; // Cada n° representa um elemento do array computers. Se a ordem for 3 , 2, ..., então o computador 3 será o primeiro e assim por diante.
    private int attempt = 0;
    private bool orderIsWrong;
    private void Start()
    {
        input = InputActionsManager.Instance;
        playerInteraction = FindFirstObjectByType<PlayerInteraction>();

        ChangeOrder();
    }

    private void Update()
    {
        for (int i = 0; i < computers.Length; i++)
        {
            if (computers[i] != null && screenComputers[i])
            {
                if (computers[i] == playerInteraction.NextInteraction())
                {
                    if (input.inputActions.Game.Interaction.WasPressedThisFrame())
                    {
                        if (!screenComputers[i].GetComponent<MeshRenderer>().enabled)
                        {
                            screenComputers[i].GetComponent<MeshRenderer>().enabled = true;
                            TryOrder();
                        }
                    }
                }
                else
                {
                    if (computers[i].TryGetComponent<Outline>(out var outline))
                    {
                        outline.enabled = false;
                    }

                }
            }
        }
    }

    private void TryOrder()
    {
        if (screenComputers[order[attempt] - 1].GetComponent<MeshRenderer>().enabled == false)
        {
            orderIsWrong = true;
        }

        attempt++;

        if (attempt >= 4)
        {
            if (orderIsWrong)
            {
                ChangeOrder();
                orderIsWrong = false;
                attempt = 0;
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
    }

    private void ChangeOrder()
    {
        for (int i = 0; i < order.Length; i++)
        {
            int j = Random.Range(0, order.Length);

            // Embaralha os arrays.
            (order[j], order[i]) = (order[i], order[j]);
        }

        for (int i = 0; i < visualOrder.Length; i++)
        {
            // Coloca a ordem correta visível para o player.
            visualOrder[i].transform.position = screenComputers[order[i] - 1].transform.position;
        }

        foreach (var screen in screenComputers)
        {
            // Desabilita a tela de cada computador.
            screen.GetComponent<MeshRenderer>().enabled = false;
        }

        // Faz com que o player ainda não tenha interagido com nenhum objeto(Isso é mais para o visual, código no PlayerInteraction).
        playerInteraction.HasInteracted(false);
    }
}
