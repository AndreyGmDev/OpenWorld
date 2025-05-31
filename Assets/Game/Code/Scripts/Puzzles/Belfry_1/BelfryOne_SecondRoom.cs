using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BelfryOne_SecondRoom : MonoBehaviour
{
    [SerializeField, Tooltip("Os objetos dem estar na layer InteractableObjects e precisam ter suas MeshRenderer desativadas para serem ativadas quando o player interagir com cada objeto.")] GameObject[] computers = new GameObject[4]; // Os 4 computadores na cena.
    [SerializeField] GameObject door;

    private InputActionsManager input;
    private PlayerInteraction playerInteraction;

    private int[] order = { 1, 2, 3, 4 };
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
                        item.GetComponent<MeshRenderer>().enabled = true;
                        TryOrder();
                    }
                }
            }
        }
    }

    private void TryOrder()
    {
        if (!computers[order[attempt]].GetComponent<MeshRenderer>().enabled)
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
            }
            else
            {
                if (!door)
                    door.SetActive(false); // Abrir a porta
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

        foreach (var item in computers)
        {
            item.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
