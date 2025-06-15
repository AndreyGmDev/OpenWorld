using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Visor : MonoBehaviour
{
    [SerializeField] List<VisorTypes> Action;

    private bool isVisorEnabled;
    
    private MeshRenderer meshRenderer;
    private Collider coll;

    public GameObject VisorUI;
    private Hotbar hotbar;

    private bool meshRendererEnabled;
    private bool colliderEnabled;

    private void Start()
    {
        if (Action.Count > 3) Action.RemoveRange(3 , Action.Count - 3);

        isVisorEnabled = GameObject.Find("Visor");

        if (VisorUI != null) {
            VisorUI.SetActive(true);
        }

        // Get reference to Hotbar component
        hotbar = FindObjectOfType<Hotbar>();

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
        bool visorActive = GameObject.Find("Visor") != null;
        
        // Play sound effects based on visor state using audio clips from Hotbar
        if (visorActive && !isVisorEnabled)
        {
            // Visor is being turned on
            if (hotbar != null && hotbar.visorOnSFX != null)
                AudioManager.Instance.PlaySoundFXClip(hotbar.visorOnSFX, transform, 1f, false);
        }
        else if (!visorActive && isVisorEnabled)
        {
            // Visor is being turned off
            if (hotbar != null && hotbar.visorOffSFX != null)
                AudioManager.Instance.PlaySoundFXClip(hotbar.visorOffSFX, transform, 1f, false);
        }

        if (visorActive)
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
    [Tooltip("The component will enabled or disabled when the visor is enabled")] public Mode mode;
}