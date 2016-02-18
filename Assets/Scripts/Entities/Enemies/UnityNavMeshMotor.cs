using RAIN.Motion;
using RAIN.Serialization;
using UnityEngine;

[RAINSerializableClass]
public class UnityNavMeshMotor : RAINMotor {
    [RAINSerializableField]
    private float _closeEnoughDistance = 0.1f;

    [RAINSerializableField]
    private float _speed = 3.5f;

    private NavMeshAgent _agent = null;

    private Vector3 _lastPosition = Vector3.zero;

    public override float DefaultCloseEnoughDistance {
        get { return _closeEnoughDistance; }
        set { _closeEnoughDistance = value; }
    }

    public override float DefaultSpeed {
        get { return _speed; }
        set { _speed = value; }
    }

    // It seems like the NavMeshAgent forces positions on
    // the NavMesh, so 3D Movement doesn't make sense
    public override bool Allow3DMovement {
        get { return false; }
        set { }
    }

    // Don't support this at the moment, not sure if the NavMeshAgent
    // can go off of the mesh
    public override bool AllowOffGraphMovement {
        get { return false; }
        set { }
    }

    // 3D Rotation is technically doable, but not like
    // we support it with the BasicMotor
    public override bool Allow3DRotation {
        get { return false; }
        set { }
    }

    public override void BodyInit() {
        base.BodyInit();

        if (AI.Body == null)
            _agent = null;
        else {
            _agent = AI.Body.GetComponent<NavMeshAgent>();
            if (_agent == null)
                _agent = AI.Body.AddComponent<NavMeshAgent>();
        }
    }

    public override void UpdateMotionTransforms() {
        // I don't believe the Unity Navigation Mesh can handle transforms, so this stays as identity
        AI.Kinematic.ParentTransform = Matrix4x4.identity;
        AI.Kinematic.Position = AI.Body.transform.position;
        AI.Kinematic.Orientation = AI.Body.transform.rotation.eulerAngles;

        // Velocities likely won't matter as we never actually use them in this motor
        AI.Kinematic.ResetVelocities();

        // Set our speed to zero, we'll set it when we are using it
        _agent.speed = 0;
    }

    public override void ApplyMotionTransforms() {
    }

    public override bool Move() {
        if (!MoveTarget.IsValid)
            return false;

        // Set our acceleration
        _agent.speed = _speed;

        // We'll just update these constantly as our value can change when the MoveTarget changes
        _agent.stoppingDistance = Mathf.Max(DefaultCloseEnoughDistance, MoveTarget.CloseEnoughDistance);

        // Have to make sure the target is still in the same place
        Vector3 tEndMoved = _lastPosition - MoveTarget.Position;
        tEndMoved.y = 0;

        // If we don't have a path or our target moved
        if (!_agent.hasPath || !Mathf.Approximately(tEndMoved.sqrMagnitude, 0)) {
            _agent.destination = MoveTarget.Position;
            _lastPosition = MoveTarget.Position;

            // We can return at least if we are at our destination at this point
            return IsAt(_agent.destination);
        }

        // Still making a path or our path is invalid
        if (_agent.pathPending || _agent.pathStatus == NavMeshPathStatus.PathInvalid)
            return false;

        return _agent.remainingDistance <= _agent.stoppingDistance;
    }

    public override bool IsAt(Vector3 aPosition) {
        Vector3 tPosition = AI.Body.transform.position - aPosition;
        tPosition.y = 0;

        return tPosition.magnitude <= _agent.stoppingDistance;
    }

    public override bool IsAt(MoveLookTarget aTarget) {
        return IsAt(aTarget.Position);
    }

    public override bool Face() {
        // Too tired to do this
        return true;
    }

    public override bool IsFacing(Vector3 aPosition) {
        // Too tired to do this
        return true;
    }

    public override bool IsFacing(MoveLookTarget aTarget) {
        // Too tired to do this
        return true;
    }

    public override void Stop() {
        _agent.Stop();
    }
}