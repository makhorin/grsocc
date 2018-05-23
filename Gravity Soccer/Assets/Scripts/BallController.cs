using UnityEngine;
using Lean.Touch;
using System;

public class BallController : MonoBehaviour
{
    public Rigidbody RigidBody;
    public Collider Collider;
    public TrailRenderer Trail;
    public ParticleSystem Explosion;
    public event Action Lost;
    public event Action Won;
    private bool _ready;
    private float _time;
    void Start()
    {
        LeanTouch.OnFingerSwipe += OnSwipe;
    }

    private void OnSwipe(LeanFinger finger)
    {
        if (!_ready)
            return;

        var velocity = new Vector2(finger.SwipeScaledDelta.x * 0.03f, Math.Max(0f, finger.SwipeScaledDelta.y * 0.04f));
        RigidBody.velocity = velocity * RigidBody.mass;
        RigidBody.AddTorque(velocity * 0.1f);
        Trail.enabled = true;
        _time = Trail.time * 3f;
    }

    private void Update()
    {
        if (!_ready)
            return;

        if (Math.Abs(transform.position.y) > 2.2f || Math.Abs(transform.position.x) > 1.1f)
        {
            enabled = false;
            LeanTouch.OnFingerSwipe -= OnSwipe;
            Destroy(gameObject);
            if (Lost != null)
                Lost();
        }
        else if (_time <= 0f)
            Trail.enabled = false;
        else
            _time -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Gate":
                LeanTouch.OnFingerSwipe -= OnSwipe;
                if (Won != null)
                    Won();
                break;
            case "Wall":
                _ready = true;
                break;
            case "Enemy":
                Destroy(Instantiate(Explosion, transform.position, Quaternion.identity), 1f);
                LeanTouch.OnFingerSwipe -= OnSwipe;
                Destroy(gameObject);
                if (Lost != null)
                    Lost();
                break;
        }
    }
}
