using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class StandardEnemyBase : MonoBehaviour, IEnemy
{
    public virtual bool IsDown { get { return _isDown; } set { throw new AccessViolationException("You are only allowed to read this value."); } }
    public virtual bool PlayerInSight { get { return _playerInSight; } set { throw new AccessViolationException("You are only allowed to read this value."); } }

    public Rigidbody EnemyRigidBody { get { return _rigidBody; } set { throw new AccessViolationException("You are only allowed to read this value."); } }

    [SerializeField] private bool _isDown;
    [SerializeField, ReadOnly] private bool _playerInSight;

    [SerializeField] private double _milliSecondsTillGameOver;

    [SerializeField] private float _apertureAngle;
    [SerializeField] private float _maxSightDistance;
    [SerializeField] private int _rayCastItterations;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField, ReadOnly] private GameObject _lineOfSight;
    [SerializeField, ReadOnly] private System.Timers.Timer _gameOverCountDown;


    public virtual void Start()
    {
        _lineOfSight = new GameObject("LineOfSight");
        _gameOverCountDown = new System.Timers.Timer(_milliSecondsTillGameOver);

        _rigidBody = gameObject.GetComponent<Rigidbody>();

        _gameOverCountDown.Elapsed += (object sender, System.Timers.ElapsedEventArgs args) => 
        {
            // TODO: 
            // Make this cause a game over
            //
            // for testing purposes, this field is used to determine if the player has been seen too long.
            // this variable setting should be replaced with something responsible of causing the game to be over.
            _isDown = true;
        };
    }

    /// <summary>
    /// Creating a view cone, which will determine if the player enters the line of sight.
    /// </summary>
    /// <returns>true if the player is withing the viewing cone, false if not</returns>
    public bool PlayerInSightCheck()
    {
        Vector3 startingPoint = transform.position;
        float angleStep = _apertureAngle / _rayCastItterations;
        RaycastHit _hit;

        for (int i = 0; i <= _rayCastItterations; i++)
        {
            float angle = (-_apertureAngle * 0.5f) + (i * angleStep);
            Vector3 sightVector = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            if (Physics.Raycast(startingPoint, sightVector, out _hit, _maxSightDistance))
            {
                if (_hit.collider.tag == "Player")
                {
                    return true;
                }
            }
        }

        return false;
    }

    public virtual void Update()
    {
        
        Debug.Log(_playerInSight ? "I see a player" : "No one in sight");

        if (_playerInSight = PlayerInSightCheck() || ResetTimer())
        {
            if (_gameOverCountDown.Enabled == false)
                _gameOverCountDown.Enabled = true;
        }
    }

    private bool ResetTimer()
    {
        if (_gameOverCountDown.Enabled)
        {
            _gameOverCountDown.Interval=_milliSecondsTillGameOver;
            _gameOverCountDown.Enabled = false;
        }
        return false;
    }
}


