using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Visor : MonoBehaviour
{
    [SerializeField] List<VisorTypes> Action;

    private void Start()
    {
        Action.RemoveRange(3 , Action.Count - 3);
        print(Action.Select(x => x.type));
    }
    void Update()
    {
        /*if (GameObject.Find("Visor").activeSelf)
        {
            switch (mode)
            {
                case Mode.Active:
                    break;
            }
        }*/
    }
}

[System.Serializable]
public class VisorTypes
{
    public enum Type { Object, Mesh, Collider, sla, eae };
    [Tooltip("What component will activate or desactivate when the visor is enable")] public Type type;

    public  enum Mode { Active, Inactive };
    [Tooltip("The component will activate or desactivate when the visor is enable")] public Mode mode;
}