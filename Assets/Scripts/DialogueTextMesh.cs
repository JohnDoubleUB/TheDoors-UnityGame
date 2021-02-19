using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTextMesh : MonoBehaviour
{
    public float textAppearSpeed = 1f;

    private TMP_Text textMesh;
    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;
    private Dictionary<int, string[]> characterEffects;
    private string speaker = "";
    
    private int visibleIndexes = 0;
    private float currentTime = 0f;


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

            //Visible stuff magic
            if (visibleIndexes != textMesh.textInfo.characterCount) 
            {
                if (currentTime >= 1f) 
                {
                    visibleIndexes++;
                    currentTime = 0f;
                }
                
                currentTime += (textAppearSpeed * 100f) * Time.deltaTime;

                TMP_CharacterInfo c;
                int vertexIndex; 

                for (int i = speaker.Length; i < textMesh.textInfo.characterCount; i++) 
                {
                    c = textMesh.textInfo.characterInfo[i];
                    vertexIndex = c.vertexIndex;
                    if (c.character == ' ') continue;

                    for (int j = 0; j < 4; j++)
                    {
                        Color currentColor = colors[vertexIndex + j];
                        colors[vertexIndex + j] = new Color(currentColor.r, currentColor.g, currentColor.b, i < visibleIndexes ? 1f : 0f);
                    }

                }
            }


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
            //is this ok?
            switch (effectToAdd)
            {
                case "w":
                    offset = Wavy(Time.time + actualIndex);
                    
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] += offset;
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

                case "c":
                    for (int j = 0; j < 4; j++)
                    {
                        offset = Shaky(2f);
                        vertices[vertexIndex + j] += offset;
                    }
                    break;

                case "r":
                    for (int j = 0; j < 4; j++)
                    {
                        colors[vertexIndex + j] = new Color(1, 0, 0, colors[vertexIndex + j].a);
                    }
                    break;

                case "g":
                    for (int j = 0; j < 4; j++)
                    {
                        colors[vertexIndex + j] = new Color(0, 1, 0, colors[vertexIndex + j].a);
                    }
                    break;

                case "b":
                    for (int j = 0; j < 4; j++)
                    {
                        colors[vertexIndex + j] = new Color(0, 0, 1, colors[vertexIndex + j].a);
                    }
                    break;
                
                case "y":
                    for (int j = 0; j < 4; j++)
                    {
                        colors[vertexIndex + j] = new Color(1f, 0.92f, 0.016f, colors[vertexIndex + j].a);
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
        visibleIndexes = speaker.Length;
        currentTime = 1f;
    }

    public void SetText(string text, string speaker, Dictionary<int, string[]> characterEffects) 
    {
        textMesh.text = speaker + text;
        this.speaker = speaker;
        this.characterEffects = characterEffects;
        visibleIndexes = speaker.Length;
        currentTime = 1f;
        
        if (!string.IsNullOrEmpty(textMesh.text))
        {
            textMesh.ForceMeshUpdate();
            mesh = textMesh.mesh;
            colors = mesh.colors;

            for (int i = speaker.Length; i < textMesh.textInfo.characterCount; i++)
            {
                TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
                if (c.character == ' ') continue;
                int vertexIndex = c.vertexIndex;
                for (int j = 0; j < 4; j++)
                {
                    Color currentColor = colors[vertexIndex + j];
                    colors[vertexIndex + j] = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
                }
            }


            mesh.colors = colors;
            textMesh.canvasRenderer.SetMesh(mesh);
        }
    }

    public void SetAllTextVisible() 
    {
        visibleIndexes = textMesh.textInfo.characterCount;
    }

}
