using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SteerByPhysics : MonoBehaviour
{
    [NonSerialized] public bool steeringEnabled = true;
    Vector2 _steeringForce;
    Vector2 _appliedForce;
    Vector2 _appliedImpulse;
    [SerializeField] float maxV = 100;
    [SerializeField] float maxF = 2000;
    [SerializeField] float turnMultiplier = 2;
    Vector2 _wanderTarget;


    // Individual forces that should be added up into steering_force
    Vector2? _seekF;
    Vector2? _wanderF;
    Vector2? _separationF;

    Vector2? _applyForceF;

    Vector2? _applyImpulseF;

    void Start()
    {
        float wanderSeed = Random.Range(0f, 2.0f * Mathf.PI);
        _wanderTarget = new Vector2(40.0f * Mathf.Cos(wanderSeed), 40.0f * Mathf.Sin(wanderSeed));
    }

    public void DoFixedUpdate(Rigidbody2D _rb2D)
    {
        if (!steeringEnabled) return;

        _steeringForce = CalculateSteeringForce() / _rb2D.mass;
        _appliedForce = CalculateAppliedForce() / _rb2D.mass;
        _appliedImpulse = CalculateAppliedImpulse() / _rb2D.mass;

        _rb2D.AddForce(_steeringForce + _appliedForce, ForceMode2D.Force);

        Vector2 clampedVelocity = Vector2.ClampMagnitude(_rb2D.velocity, maxV);
        _rb2D.velocity = clampedVelocity + _appliedImpulse;

        _applyForceF = Vector2.zero;
    }

    public void RotateTowardsVelocity(Rigidbody2D _rb2D, float lerpVal)
    {
        _rb2D.RotateTowardsVelocity(lerpVal);
    }

    Vector2 CalculateSteeringForce()
    {
        _steeringForce = Vector2.zero;

        if (_seekF.HasValue) _steeringForce += _seekF.Value;
        if (_wanderF.HasValue) _steeringForce += _wanderF.Value;
        if (_separationF.HasValue) _steeringForce += _separationF.Value;

        _seekF = null;
        _wanderF = null;
        _separationF = null;

        return Vector2.ClampMagnitude(_steeringForce, maxF);
    }

    Vector2 CalculateAppliedForce()
    {
        _appliedForce = Vector2.zero;

        if (_applyForceF.HasValue) _appliedForce += _applyForceF.Value;

        return _appliedForce;
    }

    Vector2 CalculateAppliedImpulse()
    {
        _appliedImpulse = Vector2.zero;

        if (_applyImpulseF.HasValue) _appliedImpulse += _applyImpulseF.Value;

        return _appliedImpulse;
    }

    public void ApplySteeringForce(float force, float angleRadians, float duration = 0.01f)
    {
        _applyForceF = force * new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));

        DOTween.Sequence()
            .AppendInterval(duration / 2.0f)
            .Append(DOTween.To(() => _applyForceF.Value, x => _applyForceF = x,
                Vector2.zero, duration / 2.0f).SetEase(Ease.Linear))
            .AppendCallback(() => { _applyForceF = null; });
    }

    public void ApplySteeringImpulse(float force, float angleRadians, float duration = 0.01f)
    {
        _applyImpulseF = force * new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));

        DOTween.Sequence()
            .AppendInterval(duration / 2.0f)
            .Append(DOTween.To(() => _applyImpulseF.Value, x => _applyImpulseF = x,
                Vector2.zero, duration / 2.0f).SetEase(Ease.Linear))
            .AppendCallback(() => { _applyImpulseF = null; });
    }

    public void SeekPoint(Rigidbody2D _rb2D, Vector2 targetPosition, float deceleration = 1.0f, float weight = 1.0f)
    {
        Vector2 selfToTargetPosition = targetPosition - _rb2D.position;
        float distanceToTargetPosition = selfToTargetPosition.magnitude;

        if (distanceToTargetPosition > 0)
        {
            float velocityMagnitude = distanceToTargetPosition / (deceleration * 0.08f);
            velocityMagnitude = Mathf.Min(velocityMagnitude, maxV);

            Vector2 newVelocity = velocityMagnitude * selfToTargetPosition.normalized;

            _seekF = turnMultiplier * weight * (newVelocity - _rb2D.velocity);
        }
        else
        {
            _seekF = Vector2.zero;
        }
    }

    public void SteeringSeparate(Rigidbody2D _rb2D, float separationRadius, IEnumerable<GameObject> avoidGameObjects, float weight = 1.0f)
    {
        _separationF = Vector2.zero;

        foreach (GameObject avoidGameObject in avoidGameObjects)
        {
            Rigidbody2D avoidRigidbody2D = avoidGameObject.GetComponent<Rigidbody2D>();
            
            if (_rb2D.GetInstanceID() == avoidRigidbody2D.GetInstanceID()) continue;
            if (Vector2.Distance(_rb2D.position, avoidRigidbody2D.position) >= 2.0f * separationRadius) continue;

            _separationF += separationRadius * (_rb2D.position - avoidRigidbody2D.position).normalized;
        }

        _separationF *= weight;
    }

    public void Wander(float rs = 40.0f, float distance = 40.0f, float jitter = 20.0f, float weight = 1.0f)
    {
        _wanderTarget += jitter * new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        _wanderTarget.Normalize();
        _wanderTarget *= rs;

        Vector2 localVector = _wanderTarget + new Vector2(distance, 0);
        Vector2 worldVector = transform.TransformDirection(localVector);

        _wanderF = weight * worldVector;
    }
}