using UnityEngine;
using System.Collections;

public abstract class Behavior : Component {

    public abstract void Attack();

    public abstract void Defend();

    public abstract void Flee();

    public abstract void Wander();

    public abstract void Chase();

    public abstract void Encounter();
}
