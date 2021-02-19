using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour
{

    private BoxCollider2D bc;
    public Text interactText;

    private Player player;

    public bool Selected 
    {
        get { return interactText.gameObject.activeInHierarchy; }
        set 
        {
            interactText.gameObject.SetActive(value);
        }
    }

    protected void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        Selected = false;
    }

    protected void Start()
    {
        string inputKey = InputManager.current != null ? InputManager.current.GetKeyCodesForInputMapping(InputMapping.Interact)[0].ToString() : "[Undefined]";

        //Set text
        interactText.text = "Press \'" + inputKey + "\' to interact";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.GetMask("Player")) 
        {
            player = collision.gameObject.GetComponent<Player>();
            player.AddInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.GetMask("Player"))
        {
            player.RemoveInteractable(this);
            player = null;
            Selected = false;
        }
    }

    public virtual void Interact() 
    {

    }

}
