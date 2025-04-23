using UnityEngine;

public class ItemConditions : MonoBehaviour
{
    [SerializeField] bool rightClickAim;
    [SerializeField] bool leftClickAim;

    public bool CheckRightClickAim()
    {
        return rightClickAim;
    }

    public bool CheckLeftClickAim()
    {
        return leftClickAim;
    }
}
