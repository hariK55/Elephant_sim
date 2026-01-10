using System;
using UnityEngine;

public class SugarCane : MonoBehaviour
{
    private int eatVal = 5;
    private bool eatable = true;
    public static SugarCane Instance { get; private set; }

    public event EventHandler OnCanePicked;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
         GetComponent<Interactable>().setEatable(eatable);
        GetComponent<Interactable>().SetEatValue(eatVal);
        GetComponent<Rigidbody>().isKinematic = true;


    }
    public void SugarCaneInteract()
    {
        Debug.Log("take sugarcane!");
        OnCanePicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Interactable>().Enable(true);
        GetComponent<Rigidbody>().isKinematic =false;
    }
}
