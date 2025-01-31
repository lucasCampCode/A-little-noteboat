﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovementBehavior : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private HealthBehaviour _health;

    [Tooltip("The target of the EnemyShootBehavior")]
    private GameObject _player;
    [SerializeField,Tooltip("changes the speed pramameter")]
    private Animator _animator;

    private float _moveSpeed = 10;

    Vector3 velocity;

    private bool _firstLoopComplete = false;
    private bool _waitComplete = false;
    private bool _secondLoopComplete = false;
    private float _timeOnFirstLoop = 0;
    private float _timeWaiting = 0;
    private float _timeOnSecondLoop = 0;

    private bool _isLooping = false;

    [Tooltip("The spot that the enemy will orbit and wait at")]
    [SerializeField] private Transform _waitSpot;
    [Tooltip("The spot that the enemy will exit at")]
    [SerializeField] private Transform _exitSpot;

    public Transform WaitSpot
    {
        get { return _waitSpot; }
        set { _waitSpot = value; }
    }

    public Transform ExitSpot
    {
        get { return _exitSpot; }
        set { _exitSpot = value; }
    }

    public bool IsWaiting { get; private set; } = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _health = GetComponent<HealthBehaviour>();
    }

    void FixedUpdate()
    {
        Move(CalculateMovement());

        _animator?.SetFloat("speed", _rigidbody.velocity.normalized.magnitude);
    }

    private Vector3 CalculateMovement()
    {
        //Direction from the enemy to the WaitSpot
        Vector3 toWaitSpot = (_waitSpot.position - transform.position).normalized;
        Vector3 moveDirection = new Vector3();
        Vector3 desiredVelocity = new Vector3();
        Vector3 steeringForce = new Vector3();
        float maxForce = 2;

        //If the first loop is not complete
        if (!_firstLoopComplete)
        {
            //Calculate an offset position and a vector towards it to prevent snapping when close enough to orbit
            Vector3 offsetPoint = new Vector3(_waitSpot.position.x - 1, _waitSpot.position.y, _waitSpot.position.z - 0.5f);
            Vector3 toOffsetPoint = (offsetPoint - transform.position).normalized;

            //If the distance to the waitSpot's position is greater than 1 and isn't looping
            if ((transform.position - offsetPoint).magnitude > 1f && !_isLooping)
            {
                //Calculate the steering force and velocity
                desiredVelocity = toOffsetPoint * _moveSpeed;
                steeringForce = desiredVelocity - velocity;

                if (steeringForce.magnitude > maxForce)
                    steeringForce = steeringForce.normalized * maxForce;

                velocity += steeringForce;

                //Clamp the magnitude of the force
                if (steeringForce.magnitude > maxForce)
                    steeringForce = steeringForce.normalized * maxForce;
            }
            //If the enemy is looping 
            else
            {
                //Set isLooping to true to prevent snapping the forwards to be towards the waitSpot while looping
                _isLooping = true;

                //Calculate the direction perpendicular to the vector towards the waitSpot
                moveDirection = new Vector3(toWaitSpot.z, 0, -1 * toWaitSpot.x).normalized;
                desiredVelocity = moveDirection * _moveSpeed;

                steeringForce = desiredVelocity - velocity;

                //Clamp the magnitude of the force
                if (steeringForce.magnitude > maxForce)
                    steeringForce = steeringForce.normalized * maxForce;

                velocity += steeringForce;

                //Increment time on first loop
                _timeOnFirstLoop += Time.deltaTime;
                //If the time on the first loop is greater than 2.5 seconds
                if (_timeOnFirstLoop > 2.5)
                {
                    //Set the first loop to be complete
                    _firstLoopComplete = true;
                    _isLooping = false;
                }
            }
            //Look where the enemy is going
            transform.LookAt(new Vector3((transform.position + velocity).x, transform.position.y, (transform.position + velocity).z));

            //Return the calculated vector
            return (velocity * Time.deltaTime);
        }
        //If the enemy has not waited
        else if (!_waitComplete)
        {
            //If not on the wait spot
            if ((transform.position - _waitSpot.position).magnitude > 0.25f)
            {
                //Calculate the steering force and velocity
                desiredVelocity = toWaitSpot * _moveSpeed;
                steeringForce = desiredVelocity - velocity;

                //Clamp the magnitude of the force
                if (steeringForce.magnitude > maxForce)
                    steeringForce = steeringForce.normalized * maxForce;

                velocity += steeringForce;

                //Look where the enemy is going
                transform.LookAt(new Vector3((transform.position + velocity).x, transform.position.y, (transform.position + velocity).z));

                //Return the vector to the waitSpot
                return (velocity * Time.deltaTime);
            }
            //If on the wait spot
            else
            {
                IsWaiting = true;

                //Look down the z axis
                transform.LookAt(new Vector3(transform.position.x, transform.position.y, transform.position.z - 1));

                //Increment time waiting
                _timeWaiting += Time.deltaTime;
                if (_timeWaiting > 5)
                {
                    _waitComplete = true;
                    IsWaiting = false;
                }
            }
        }
        //If the enemy has not done the second loop
        else if (!_secondLoopComplete)
        {
            //Calculate the direction perpendicular to the vector towards the waitSpot
            moveDirection = new Vector3(toWaitSpot.z, 0, -1 * toWaitSpot.x).normalized;
            velocity = moveDirection * _moveSpeed * Time.deltaTime;

            //Increment time on first loop
            _timeOnSecondLoop += Time.deltaTime;
            //If the time on the first loop is greater than three seconds
            if (_timeOnSecondLoop > 1)
                //Set the first loop to be complete
                _secondLoopComplete = true;

            //Look where the enemy is going
            transform.LookAt(new Vector3((transform.position + velocity).x, transform.position.y, (transform.position + velocity).z));

            //Return the perpendicular vector
            return (velocity);
        }
        //If the enemy has not exited
        else
        {
            //If not on the exit spot
            if ((transform.position - _exitSpot.position).magnitude > 0.25f)
            {
                Vector3 toExit = (_exitSpot.position - transform.position).normalized;

                //Calculate the steering force and velocity
                desiredVelocity = toExit * _moveSpeed;
                steeringForce = desiredVelocity - velocity;

                //Clamp the magnitude of the force
                if (steeringForce.magnitude > maxForce)
                    steeringForce = steeringForce.normalized * maxForce;

                velocity += steeringForce;

                //Look where the enemy is going
                transform.LookAt(new Vector3((transform.position + velocity).x, transform.position.y, (transform.position + velocity).z));

                //Return the vector to the exit
                return (velocity * Time.deltaTime);
            }
            //If on the exit spot
            else
            {
                //Destroy the enemy this script is attached to
                Destroy(gameObject);
            }
            //Look where the enemy is going
            transform.LookAt(new Vector3((transform.position + velocity).x, transform.position.y, (transform.position + velocity).z));
        }
        return new Vector3();
    }

    /// <summary>
    /// Applies the calculated force to the enemy's current position
    /// If the enemy is dead, it will fall
    /// </summary>
    /// <param name="change">The force to be applied to the enemy's position</param>
    private void Move(Vector3 change)
    {
        //If the plane is dead
        if (_health.Health <= 0)
            //Fall downwards
            _rigidbody.MovePosition(new Vector3(transform.position.x, transform.position.y - 0.15f, transform.position.z) + change);
        else
            //Move with the change
            _rigidbody.MovePosition(transform.position + change);
    }
}
