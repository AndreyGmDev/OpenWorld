using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string characterName; // Nome do personagem que fala
        public Sprite characterImage; // Imagem associada ao personagem
        [TextArea(3, 10)] public string dialogueText; // Texto do diálogo
    }

    [System.Serializable]
    public class DialogueGroup
    {
        public int flag; // Número da flag que identifica o grupo de diálogos
        public List<DialogueLine> dialogueLines; // Lista de linhas de diálogo no grupo
    }

    public List<DialogueGroup> dialogueGroups; // Lista de grupos de diálogos
}
