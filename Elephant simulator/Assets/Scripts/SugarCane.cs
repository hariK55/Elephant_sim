using System;
using UnityEngine;

public class SugarCane : MonoBehaviour
{
   
    public static SugarCane Instance { get; private set; }

    public event EventHandler OnCanePicked;
  
    private void Awake()
    {
        Instance = this;
    }
    public void SugarCaneInteract()
    {
        Debug.Log("take sugarcane!");
        OnCanePicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Interactable.Instance.Enable();
    }
}
