using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [SerializeField] private Text text;

    public void OnMouse()
    {
        text.enabled = true;
    }

    public void OffMouse()
    {
        text.enabled = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        text.enabled=false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
