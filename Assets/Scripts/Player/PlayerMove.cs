using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed;

    private float _hMov;
    private float _vMov;
    private Vector3 _velocity;
    
    void Update()
    {
        Inputs();
        Move();
    }

    private void Move()
    {
        _velocity.Set(_hMov, 0, _vMov);
        _velocity.Normalize();
        if (_hMov != 0 || _vMov != 0) transform.forward = _velocity;

        transform.position += _velocity * speed * Time.deltaTime;
    }

    private void Inputs()
    {
        _hMov = Input.GetAxisRaw("Horizontal");
        _vMov = Input.GetAxisRaw("Vertical");
    }
}
