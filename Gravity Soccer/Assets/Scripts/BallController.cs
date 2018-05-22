using UnityEngine;
using Lean.Touch;
using Random = System.Random;
using System;

public class BallController : MonoBehaviour
{
    public Rigidbody RigidBody;
    public Collider Collider;
    public TrailRenderer Trail;
    public ParticleSystem Explosion;
    public event Action Lost;
    public event Action Won;

    private Random _rnd = new Random();
    private float _time;
    void Start()
    {
        LeanTouch.OnFingerSwipe += OnSwipe;
    }

    private void OnSwipe(LeanFinger finger)
    {
        var velocity = new Vector2(finger.SwipeScaledDelta.x * 0.015f, Math.Max(0f, finger.SwipeScaledDelta.y * 0.03f));
        RigidBody.velocity = velocity;
        RigidBody.AddTorque(velocity * 0.1f);
        Trail.enabled = true;
        _time = Trail.time * 3f;
    }

    private void Update()
    {
        _time -= Time.deltaTime;
        if (_time <= 0f)
            Trail.enabled = false;

        if (Math.Abs(transform.position.y) > 2.2f || Math.Abs(transform.position.x) > 1.4f)
        {
            enabled = false;
            LeanTouch.OnFingerSwipe -= OnSwipe;
            Destroy(gameObject);
            if (Lost != null)
                Lost();
        }
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
                LeanTouch.OnFingerSwipe -= OnSwipe;
                Destroy(gameObject);
                if (Lost != null)
                    Lost();
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
