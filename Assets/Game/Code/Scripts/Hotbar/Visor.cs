using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Visor : MonoBehaviour
{
    [SerializeField] List<VisorTypes> Action;

    private bool isVisorEnabled;
    
    private MeshRenderer meshRenderer;
    private Collider coll;

    private bool meshRendererEnabled;
    private bool colliderEnabled;

    private void Start()
    {
        if (Action.Count > 3) Action.RemoveRange(3 , Action.Count - 3);

        isVisorEnabled = GameObject.Find("Visor");

        meshRenderer = GetComponent<MeshRenderer>();
        coll = GetComponent<Collider>();

        meshRendererEnabled = meshRenderer != null ? GetComponent<MeshRenderer>().enabled : false;
        colliderEnabled = coll != null ? GetComponent<Collider>().enabled : false;

        DoAction();
    }
    void Update()
    {
        // Só realiza a ação a cada vez que o visor for ativado ou desativado. Melhor pra otimização.
        if (GameObject.Find("Visor") != isVisorEnabled)
        {
            DoAction();
            isVisorEnabled = GameObject.Find("Visor");
        }
    }

    private void DoAction()
    {
        if (GameObject.Find("Visor") != null)
        {
            foreach (VisorTypes elem in Action)
            {
                switch (elem.type)
                {
                    case VisorTypes.Type.Object:

                        if (meshRenderer) meshRenderer.enabled = elem.mode == VisorTypes.Mode.Active;
                        if (coll) coll.enabled = elem.mode == VisorTypes.Mode.Active;

                        break;

                    case VisorTypes.Type.Mesh:

                        if (meshRenderer) meshRenderer.enabled = elem.mode == VisorTypes.Mode.Active;

                        break;
                    case VisorTypes.Type.Collider:

                        if (coll) coll.enabled = elem.mode == VisorTypes.Mode.Active;

                        break;
                }
            }
        }
        else
        {
            if (meshRenderer) meshRenderer.enabled = meshRendererEnabled;
            if (coll) coll.enabled = colliderEnabled;
        }
    }
}


[System.Serializable]
public class VisorTypes
{
    public enum Type { Object, Mesh, Collider };
    [Tooltip("Which component will be enabled or disabled when the viewport is enabled. The Object option means (Mesh and Collider)")] public Type type;

    public enum Mode { Active, Inactive };
    [Tooltip("The component will enabled or disabled when the visor is enable")] public Mode mode;
}