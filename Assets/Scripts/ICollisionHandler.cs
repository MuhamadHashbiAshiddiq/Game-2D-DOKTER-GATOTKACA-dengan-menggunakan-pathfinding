using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionHandler
{
    void CollisionEnter(string colliderName, GameObject other);
    void CollisionEnter2(string colliderName, GameObject other);
    void CollisionEnter3(string colliderName, GameObject other);
}