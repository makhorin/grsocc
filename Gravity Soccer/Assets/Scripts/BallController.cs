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
    public event Action Kick;
    private bool _ready;
    private float _time;
    private float _topBorder;
    private GameObject _tutor;
    public void Init(float top)
    {
        _topBorder = top;
    }

    void Start()
    {
        LeanTouch.OnFingerSwipe += OnSwipe;
    }

    private void OnSwipe(LeanFinger finger)
    {
        if (!_ready)
            return;
        if (Kick != null)
            Kick();
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

        if ((transform.position.y < -2f || transform.position.y > _topBorder) ||
            transform.position.x > Math.Abs(1.2f))
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
                var exp = Instantiate(Explosion, transform.position, Quaternion.identity);
                Destroy(exp.gameObject, 1f);
                LeanTouch.OnFingerSwipe -= OnSwipe;
                Destroy(gameObject);
                if (Lost != null)
                    Lost();
                break;
        }
    }
}
