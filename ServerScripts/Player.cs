using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public Vector3 position;
    public Quaternion rotation;
    private bool[] inputs;
    private float moveSpeed = 5f;

 
    public void InitialisePlayer(int _id, string _username)
    {
        id = _id;
        username = _username;

        inputs = new bool[4];
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void Update()
    {
        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x += 1;
        }
        if (inputs[3])
        {
            _inputDirection.x -= 1;
        }

        Move(_inputDirection);
    }

    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void Move(Vector2 _inputDirection)
    {
        Vector3 _forward = (new Vector3(0, 0, 1));
        Vector3 _right = Vector3.Normalize(Vector3.Cross(_forward, new Vector3(0, 1, 0)));

        Vector3 _moveDirection = _right * _inputDirection.x+ _forward * _inputDirection.y;
        position += _moveDirection * moveSpeed;

       // ServerSend.PlayerPosition(this);
       // ServerSend.PlayerRotation(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        rotation = _rotation;
    }
}
