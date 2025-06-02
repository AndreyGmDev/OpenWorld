using UnityEngine;

public class BelfryOne_SecondRoom : MonoBehaviour
{
    [SerializeField, Tooltip("Os objetos dem estar na layer InteractableObjects e precisam ter suas MeshRenderer desativadas para serem ativadas quando o player interagir com cada objeto.")] GameObject[] computers = new GameObject[4]; // Os 4 computadores na cena.
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
        foreach (var item in computers)
        {
            if (item != null)
            {
                if (item == playerInteraction.NextInteraction())
                { 
                    if (input.inputActions.Game.Interaction.WasPressedThisFrame())
                    {
                        if (!item.GetComponent<MeshRenderer>().enabled)
                        {
                            item.GetComponent<MeshRenderer>().enabled = true;
                            TryOrder();
                        }
                    }
                }
            }
        }
    }

    private void TryOrder()
    {
        if (computers[order[attempt] - 1].GetComponent<MeshRenderer>().enabled == false)
        {
            orderIsWrong = true;
        }

        print(orderIsWrong);

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
            int temp = order[i];
            order[i] = order[j];
            order[j] = temp;
        }

        for (int i = 0; i < visualOrder.Length; i++)
        {
            visualOrder[i].transform.position = computers[order[i] - 1].transform.position;
        }

        foreach (var item in computers)
        {
            item.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
