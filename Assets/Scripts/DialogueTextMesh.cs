using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTextMesh : MonoBehaviour
{
    private TMP_Text textMesh;
    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;
    private Dictionary<int, string[]> characterEffects;
    private string speaker = "";
    private int visibleIndexes = 0;


    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!string.IsNullOrEmpty(textMesh.text) && characterEffects != null) 
        {
            textMesh.ForceMeshUpdate();
            mesh = textMesh.mesh;
            vertices = mesh.vertices;
            colors = mesh.colors;

            foreach (KeyValuePair<int, string[]> characterWithEffect in characterEffects) AddCharacterEffects(characterWithEffect);

            //Assign these mesh verticies back to the mesh and force the text mesh to update!
            mesh.colors = colors;
            mesh.vertices = vertices;
            textMesh.canvasRenderer.SetMesh(mesh);
        }
    }

    private Vector2 Floaty(float time) 
    {
        return new Vector2(Mathf.Sin(time * 3.3f), Mathf.Cos(time * 2.5f));
    }

    private Vector2 Wavy(float time, float speed = 10) 
    {
        return Vector2.up * Mathf.Sin(time * 0.1f + speed * time); //* 0.04f;
    }

    private Vector2 Shaky(float distance = 0.8f) 
    {
        return Random.insideUnitCircle * distance;
    }

    private void AddCharacterEffects(KeyValuePair<int, string[]> characterWithEffect) 
    {
        int actualIndex = characterWithEffect.Key + speaker.Length;

        TMP_CharacterInfo c = textMesh.textInfo.characterInfo[actualIndex];
        int vertexIndex = c.vertexIndex;

        Vector3 offset;

        foreach (string effectToAdd in characterWithEffect.Value)
        {
            //Do thing with each effect
            switch (effectToAdd)
            {
                case "w":
                    offset = Wavy(Time.time + actualIndex);
                    
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] += offset;
                        //colors[index + j] = Color.yellow;
                    }

                    break;

                case "f":
                    offset = Floaty(Time.time + actualIndex);
                    
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] += offset;
                    }

                    break;

                case "s":
                    offset = Shaky(2f);
                    
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] += offset;
                    }

                    break;
            }
        }
    }

    public void SetText(string text, string speaker = "") 
    {
        textMesh.text = speaker + text;
        this.speaker = speaker;
        characterEffects = null;
    }

    public void SetText(string text, string speaker, Dictionary<int, string[]> characterEffects) 
    {
        textMesh.text = speaker + text;
        this.speaker = speaker;
        this.characterEffects = characterEffects;
    }

}
