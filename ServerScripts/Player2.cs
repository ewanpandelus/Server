using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public int id;
    public string username;

    [SerializeField] private Rotater rotater;

    private Vector3 position0;
    private Vector3 position1;
    private Vector3 position2;

    private float time0 = int.MinValue;
    private float time1 = int.MinValue;
    private float time2 = int.MinValue;

    float u = 0;
    float v = 0;

    private Vector3 predictedVelocityX = new Vector3(0, 0, 0);
    private Vector3 predictedAccelerationY = new Vector3(0, 0, 0);
 


    private int count = 0;

    public void InitialisePlayer(int _id, string _username)
    {
        id = _id;
        username = _username;
      
    }
    public void Update()
    {
        if(predictedVelocityX != new Vector3(0,0,0))
        {
            if (!float.IsNaN(predictedVelocityX.x)&&!float.IsInfinity(predictedVelocityX.x))
            {
                transform.position += predictedVelocityX * Time.deltaTime;
            }
        }
        if (predictedAccelerationY != new Vector3(0, 0, 0))
        {
            AccountForYAcceleration();
        }
        ServerSend.PlayerPosition(this);
    }

    
    public void SetPosition(Vector3 _position, float _elaspedTime)
    {
        if(count > 500)
        {
            transform.position = _position;
            count = 3;
        }
        transform.position = new Vector3(_position.x,transform.position.y,0);
        Vector3 clientPosition = _position;
        ServerSend.PlayerPosition(this);
        AssignPreviousPositionsAndTimes(clientPosition,_elaspedTime);
        predictedVelocityX = CalculateVelocityX();
        predictedAccelerationY = CalculateAccelerationY();
    }
    public void SetRotation(float rotationX)
    { 
        ServerSend.PlayerRotation(this,rotationX);
    }
    private void AssignPreviousPositionsAndTimes(Vector3 _position, float _elapsedTime)
    {
        if (count < 1)
        {
            position0 = _position;
            time0 = _elapsedTime;
        }
        else if(count == 1)
        {
            position1 = _position;
            time1 = _elapsedTime;
        }
        else if(count == 2)
        { 
            position2 = _position;
            time2 = _elapsedTime;
        }
        else
        {
            CascadeTimes(_position, _elapsedTime);
        }
        count++;
    }
    private void CascadeTimes(Vector3 _position, float _elapsedTime)
    {
        position0 = position1;
        time0 = time1;

        position1 = position2;
        time1 = time2;

        position2 = _position;
        time2 = _elapsedTime;
    }
    private Vector3 CalculateVelocityX()
    {
        if (time0 != int.MinValue && time1 != int.MinValue)
        {
            float displacementX = position1.x - position0.x;
            float timeElapsed = time1 - time0;
            float xVelocity = displacementX / timeElapsed;
            return new Vector3(xVelocity, 0, 0);
        }
        else return new Vector3(0,0,0);
    }

    private Vector3 CalculateAccelerationY()
    {
        if (time0 != int.MinValue && time1 != int.MinValue && time2!= int.MinValue)
        {
            u = CalculateYVelocity(position0, position1, time1 - time0);
            v = CalculateYVelocity(position1, position2, time2 - time1);
            return new Vector3(0, (v - u)/(time2 - time1),0);
            //a = v-u/t
        }
        else return new Vector3(0, 0, 0);
    }
    private float CalculateYVelocity(Vector3 initialPos, Vector3 resultantPos, float timeInterval)
    {
        float displacementY = resultantPos.y - initialPos.y;
        return displacementY / timeInterval;
        //v = v1-v0/t
    }
    private void AccountForYAcceleration()
    {
        float displacementY = (u * Time.deltaTime) + (0.5f * predictedAccelerationY.y * Time.deltaTime * Time.deltaTime);
        // s = ut +1/2at^2

    }
}

