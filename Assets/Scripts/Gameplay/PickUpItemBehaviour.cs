﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItemBehaviour : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _powerUps;
    // Update is called once per frame
    void Update()
    {
        int amount = 0;
        if (_powerUps.Count == 0)
            return;
        for(int i = 0; i < _powerUps.Count; i++)
            if (_powerUps[i] == null)
                _powerUps.Remove(_powerUps[i]);
        foreach (GameObject item in _powerUps)
            if (item.GetComponent<TripleShot>())
                amount++;
        foreach(GameObject item in _powerUps)
        {
            if(item.GetComponent<TripleShot>())
                item.GetComponent<TripleShot>().Amount = amount;
            item.GetComponent<PowerUp>().Upgrade();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Item"))//if the collision tag is item
        {
            _powerUps.Add(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }
}
