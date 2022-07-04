using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    public List<Node> _neighbors = new List<Node>();
    
    public int cost = 1;

    [SerializeField]
    private Material _detectedMaterial;
    private Material _startingMaterial;
    private Renderer _rend;
    private void Start()
    {
        _rend = GetComponent<Renderer>();
        _startingMaterial = _rend.material;
        
    }

    public void Detect(bool d)
    {
        _rend.material = d ? _detectedMaterial : _startingMaterial;
    }

    public List<Node> GetNeighbors()
    {
        

        return _neighbors;
    }

   
}
