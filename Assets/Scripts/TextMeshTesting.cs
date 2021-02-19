using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextMeshTesting : MonoBehaviour
{
    private TMP_Text textMesh;
    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;


    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (string.IsNullOrEmpty(textMesh.text)) return;
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;
        colors = mesh.colors;

        //itterate through every vertex of the array!

        // wobble per vert
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    Vector3 offset = Wobble(Time.time + i);
        //    vertices[i] = vertices[i] + offset;
        //}

        //Wobble per character
        Debug.Log("cvharacter count: " + textMesh.textInfo.characterCount);
        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];

            int index = c.vertexIndex;
            Vector3 offset;

            //For wobble
            offset = Wobble(Time.time + i);
            for (int j = 0; j < 4; j++)
            {
                vertices[index + j] += offset;
            }

            //For shaky
            offset = Shaky(2f);
            for (int j = 0; j < 4; j++)
            {
                vertices[index + j] += offset;
            }

            //For wavy
            offset = Wavy(Time.time + i);
            for (int j = 0; j < 4; j++)
            {
                vertices[index + j] += offset;
            }

            //For color
            for (int j = 0; j < 4; j++)
            {
                //switch (j) 
                //{
                //    case 0:
                //        colors[index + j] = Color.red;
                //        break;
                //    case 1:
                //        colors[index + j] = Color.green;
                //        break;
                //    case 2:
                //        colors[index + j] = Color.blue;
                //        break;
                //    case 3:
                //        colors[index + j] = Color.yellow;
                //        break;
                //}
                
                colors[index + j] = Color.red;
            }

        }

        //Assign these mesh verticies back to the mesh and force the text mesh to update!
        mesh.colors = colors;
        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    private Vector2 Wobble(float time) 
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

}
