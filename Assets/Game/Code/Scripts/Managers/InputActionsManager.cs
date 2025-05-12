using UnityEngine;

public class InputActionsManager : MonoBehaviour
{
    public InputSystem_Actions inputActions;

    // Inicia o Singleton do InputActionManager.
    private static InputActionsManager input;

    public static InputActionsManager Instance
    {
        get
        {
            // Confere se a instância já foi criada
            if (input == null)
            {
                // Procura o InputActionManager na cena
                input = FindFirstObjectByType<InputActionsManager>();

                // Se não encontrar, cria uma nova GameObject com esse script
                if (input == null)
                {
                    // Confere se existe esse GameObject em cena, se houver, adiciona o script nele.
                    if (GameObject.Find("GameManager"))
                    {
                        GameObject obj = GameObject.Find("GameManager");
                        obj.AddComponent<MixerManager>();
                        print("Adicione o Script InputActionManager no GameManager");
                    }
                    else
                    {
                        GameObject obj = new GameObject("GameManager");
                        input = obj.AddComponent<InputActionsManager>();
                        print("Crie um GameManager e adicione o Script InputActionManager no GameManager");
                    }
                }
            }
            return input;
        }
    }
    // Finalização do Singleton.

    private void Awake()
    {
        // Permite somente uma instância de InputActionManager na cêna.
        if (input == null)
        {
            input = this;
        }
        else if (input != this)
        {
            print("Procure esses objetos e retire o script InputActionManager até sobrar apenas um: " + gameObject.name + ", " + input.name);
            Destroy(gameObject);
        }

        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }
}
