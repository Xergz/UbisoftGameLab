using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour {
    public abstract void ReceiveHit();
    protected abstract void OnTriggerEnter(Collider other);
    protected abstract void OnTriggerStay(Collider other);
}
