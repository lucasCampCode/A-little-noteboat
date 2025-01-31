﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBehaviour : MonoBehaviour
{
    [SerializeField]
    private float _health;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private bool _destroyOnDeath;

    private bool _tripedTrigger = false;

    public float Health
    {
        get { return _health; }
    }

    /// <summary>
    /// Subtracts the given damage value from health
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0)
            _health = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_destroyOnDeath && _health <= 0)
            Destroy(gameObject);
        else if (!_destroyOnDeath && _health <= 0)
        {
            if (!_tripedTrigger)
            {
                _animator?.SetTrigger("hasDied");
                _tripedTrigger = !_tripedTrigger;
            }
        }
    }
}
