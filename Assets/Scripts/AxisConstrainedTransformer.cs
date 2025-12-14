using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class AxisConstrainedTransformer : MonoBehaviour, ITransformer
{
    public enum Axis { X, Y, Z }

    [Header("Which axis to slide along (in Axis Reference space)")]
    public Axis slideAxis = Axis.Y;

    [Header("Use a reference so axis stays stable even if tile is rotated (recommended: your BOARD transform)")]
    public Transform axisReference;

    [Header("Collision blocking")]
    public LayerMask blockingLayers;          // e.g. Tiles + Walls
    public float skin = 0.002f;               // small gap so we never interpenetrate
    public float castShrink = 0.95f;          // shrink extents slightly to reduce “sticky” casts

    private IGrabbable _grabbable;
    private Rigidbody _rb;
    private Collider _col;

    private Quaternion _lockedRot;
    private Vector3 _startPos;
    private Vector3 _axisWorld;

    private float _startAxisCoord;
    private float _grabAxisCoordAtStart;

    private float _minCoord = float.NegativeInfinity;
    private float _maxCoord = float.PositiveInfinity;

    public void Initialize(IGrabbable grabbable)
    {
        _grabbable = grabbable;
        _rb = grabbable.Transform.GetComponent<Rigidbody>();
        _col = grabbable.Transform.GetComponent<Collider>();
    }

    public void BeginTransform()
    {
        if (_grabbable == null) return;

        _startPos = _grabbable.Transform.position;
        _lockedRot = _grabbable.Transform.rotation;

        _axisWorld = GetAxisWorld();
        if (_axisWorld.sqrMagnitude < 1e-8f) _axisWorld = Vector3.up;
        _axisWorld.Normalize();

        _startAxisCoord = Vector3.Dot(_startPos, _axisWorld);
        _grabAxisCoordAtStart = Vector3.Dot(GetGrabWorldPosition(), _axisWorld);
    }

    public void UpdateTransform()
    {
        if (_grabbable == null) return;

        // Desired axis coordinate based on controller/hand movement projected onto axis
        float grabAxisCoordNow = Vector3.Dot(GetGrabWorldPosition(), _axisWorld);
        float delta = grabAxisCoordNow - _grabAxisCoordAtStart;

        float targetCoord = Mathf.Clamp(_startAxisCoord + delta, _minCoord, _maxCoord);
        Vector3 targetPos = _startPos + (targetCoord - _startAxisCoord) * _axisWorld;

        Vector3 currentPos = _grabbable.Transform.position;
        Vector3 move = targetPos - currentPos;

        Vector3 finalPos = targetPos;

        if (move.sqrMagnitude > 1e-10f && _col != null)
        {
            Vector3 dir = move.normalized;
            float dist = move.magnitude;

            Vector3 halfExtents = _col.bounds.extents * castShrink;

            // Cast and find the nearest hit that is NOT self
            RaycastHit[] hits = Physics.BoxCastAll(
                currentPos,
                halfExtents,
                dir,
                _lockedRot,
                dist + skin,
                blockingLayers,
                QueryTriggerInteraction.Ignore
            );

            float nearest = dist;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider == null) continue;
                if (hits[i].collider == _col) continue; // ignore self
                if (hits[i].distance < nearest) nearest = hits[i].distance;
            }

            if (nearest < dist + skin)
            {
                float allowed = Mathf.Max(0f, nearest - skin);
                Vector3 blockedPos = currentPos + dir * allowed;

                // Re-project strictly onto axis (removes tiny drift)
                float c = Mathf.Clamp(Vector3.Dot(blockedPos, _axisWorld), _minCoord, _maxCoord);
                finalPos = _startPos + (c - _startAxisCoord) * _axisWorld;
            }
        }

        // Apply translation + lock rotation
        if (_rb != null)
        {
            _rb.MovePosition(finalPos);
            _rb.MoveRotation(_lockedRot);
        }
        else
        {
            _grabbable.Transform.SetPositionAndRotation(finalPos, _lockedRot);
        }
    }

    public void EndTransform()
    {
        // keep locked rotation at end too
        if (_rb != null)
            _rb.MoveRotation(_lockedRot);
        else if (_grabbable != null)
            _grabbable.Transform.rotation = _lockedRot;
    }

    private Vector3 GetAxisWorld()
    {
        Transform r = axisReference != null ? axisReference : transform;
        return slideAxis switch
        {
            Axis.X => r.right,
            Axis.Y => r.up,
            Axis.Z => r.forward,
            _ => r.up
        };
    }

    private Vector3 GetGrabWorldPosition()
    {
        // ISDK provides grab points as Pose list
        List<Pose> points = _grabbable.GrabPoints;
        if (points != null && points.Count > 0)
            return points[0].position;

        return _grabbable.Transform.position;
    }
}
