using UnityEngine.UI;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Image playerImage;
    public Image cameraImage;
    public float size;
    public Material material;
    public Transform player;
    public Transform cameraPos;
    private void Start()
    {
        //player = Object.FindFirstObjectByType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().transform;
    }
    private void Update()
    {
        //align the player icon to the player rotation; i do - and +90 because my game north is not on 0 degrees
        playerImage.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -player.eulerAngles.y + -90f));
        cameraImage.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -cameraPos.eulerAngles.y + -90f));
        //size is the value of the orthographic size of the camera you toke the screenshot with. *2 because the orthograpic size represents half the height of the screen.

        material.SetVector("_PlayerPos", player.position / (size * 2f));
    }
}