using UnityEngine;
using TMPro;

public class WaveTextEffect : MonoBehaviour
{
    [Header("Configurações da Onda")]
    public float amplitude = 5f; // Altura da onda
    public float frequency = 1f; // Velocidade da onda

    private TMP_Text textMeshPro;
    private string originalText;
    private Vector3[] vertices;
    private TMP_MeshInfo[] cachedMeshInfo;

    void Start()
    {
        textMeshPro = GetComponent<TMP_Text>();

        if (textMeshPro == null)
        {
            Debug.LogError("Nenhum componente TextMeshPro foi encontrado!");
            enabled = false;
            return;
        }

        // Cache do texto original
        originalText = textMeshPro.text;
        textMeshPro.ForceMeshUpdate();
        cachedMeshInfo = textMeshPro.textInfo.CopyMeshInfoVertexData();
    }

    void Update()
    {
        if (textMeshPro == null) return;

        textMeshPro.ForceMeshUpdate();
        var textInfo = textMeshPro.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;

            // Pega os vértices do caractere
            vertices = cachedMeshInfo[materialIndex].vertices;

            Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                // Aplica o efeito de onda
                Vector3 offset = Mathf.Sin(Time.time * frequency + vertices[vertexIndex + j].x * 0.01f) * amplitude * Vector3.up;
                destinationVertices[vertexIndex + j] = vertices[vertexIndex + j] + offset;
            }
        }

        // Atualiza o mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
