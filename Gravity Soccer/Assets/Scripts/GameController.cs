using Assets.Scripts;
using Assets.Scripts.Generators;
using GameAnalyticsSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class GameController : MonoBehaviour {

    public Player Player;
    public BallController Ball;

    public Generator[] Generators;

    public TextMeshProUGUI Goal;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI League;
    public TextMeshProUGUI NewLeague;

    public SpriteRenderer Field;

    public ParticleSystem[] Particles;

    public LeagueSettings[] Leagues;

    private BallController _lastBall;
    private List<Player> _players = new List<Player>();

    private List<Generator> _sortedGenerators = new List<Generator>();
    private int _currentLevel;
    private int _leagueIndex = 0;
    private int _generatorIndex = 0;
    private bool _quited;
    private string _winText = "GOAL!!!";
    private float _lengthCoef = 2f;
    private float _fieldLength;

    public GameObject[] ToChangeHeight;
    public GameObject[] ToMoveUp;
    public GameObject Stripes;
    public GameObject StripesParent;
    public GameObject Tutor;
    private bool _tutorShown = true;
    private int _stripesCount;
    private float _gateDiff;
    public Transform Top;

	void Start ()
    {
        var p = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 13));
        _fieldLength = -p.y;
        GameAnalytics.Initialize();
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
        _currentLevel = Math.Max(1,ZPlayerPrefs.GetInt("level", 1));
        _sortedGenerators.AddRange(Generators);
        _sortedGenerators.Sort(new GeneratorComparer());
        for (var i = 0; i < _sortedGenerators.Count && _sortedGenerators[i].MinLevel <= _currentLevel; i++)
            _generatorIndex = i;

        for (var i = 1; i < Leagues.Length && _currentLevel >= Leagues[i].MinMatches; i++)
            _leagueIndex = i;
        _gateDiff = Top.position.y;
        Goal.faceColor = Leagues[_leagueIndex].Color;
        SetLevel();

        foreach (var g in ToChangeHeight)
            _baseScale.Add(g.transform.localScale.y);

        foreach (var g in ToMoveUp)
            _basePosition.Add(g.transform.position.y);

        ChangeHeight();

        GeneratePlayers();
        ThrowBall();

        Physics.gravity *= 0.6f;
    }

    private void Update()
    {
        if (_lengthCoef <= 1f || _lastBall == null)
            return;

        var p = Camera.main.ScreenToWorldPoint(new Vector3(0, 0f, 13));
        var pos = Camera.main.gameObject.transform.position;
        if (_lastBall.gameObject.transform.position.y > p.y + _fieldLength)
        {
            if (pos.y + _gateDiff >= Top.position.y)
                return;
            Camera.main.gameObject.transform.position = new Vector3(pos.x, pos.y += Time.deltaTime, pos.z);
        }
        else if (Camera.main.gameObject.transform.position.y > 0f)
            Camera.main.gameObject.transform.position = new Vector3(pos.x, pos.y -= Time.deltaTime, pos.z);
    }

    private List<float> _baseScale = new List<float>();
    private List<float> _basePosition = new List<float>();
    void ChangeHeight()
    {
        _lengthCoef = Interpolate(_currentLevel, _levels, _lengthCoefs);
        for (var i = 0; i < ToChangeHeight.Length; i++)
        {
                var g = ToChangeHeight[i];
                g.transform.localScale = new Vector3(g.transform.localScale.x,
                _baseScale[i] * _lengthCoef,
                g.transform.localScale.z);
        }

        for (var i = 0; i < ToMoveUp.Length; i++)
        {
            var g = ToMoveUp[i];
            g.transform.position = new Vector3(g.transform.position.x, 
                ((_basePosition[i] + _fieldLength) * _lengthCoef) - _fieldLength, 
                g.transform.position.z);
        }

        var stripes = (int)Math.Ceiling((_fieldLength * 2f  * _lengthCoef) * 0.5f) + 1;
        var start = -3f;
        for(var i = 0; i < stripes; i++)
        {
            if (_stripesCount < i + 1)
            {
                var s = Instantiate(Stripes, StripesParent.transform, true);
                s.transform.localPosition = new Vector3(0f, start, 0f);
                s.transform.localScale = Stripes.transform.localScale;
                _stripesCount++;
            }
            start += 6f;
        }
    }

    private int[] _levels = new[] { 1, 2, 5, 10, 50 };
    private float[] _lengthCoefs = new[] { 1f, 1f, 1.4f, 2f, 5f };

    private float Interpolate(int x, int[] xd, float[] yd)
    {
        int i;
        for (i = 1; i < _levels.Length; i++)
        {
            if (_levels[i] >= x)
                return (float)Interpolate(xd[i - 1], yd[i - 1], xd[i], yd[i], x);
        }

        return yd[i - 1];
    }


    static double Interpolate(double x0, double y0, double x1, double y1, double x)
    {
        return y0 * (x - x1) / (x0 - x1) + y1 * (x - x0) / (x1 - x0);
    }

    void GeneratePlayers()
    {
        foreach (var p in _players)
            Destroy(p.gameObject);
        _players.Clear();

        _players.AddRange(_sortedGenerators[_generatorIndex].Generate(Player, _currentLevel, Top.transform.position.y));
        Color playersColor;
        do
        {
            playersColor = UnityEngine.Random.ColorHSV(0, 1, 1f, 1f, 1, 1);
        } while (playersColor == Field.color);

        foreach(var player in _players)
            player.Body.material.color = playersColor;
    }

    void ThrowBall()
    {
        StartCoroutine(Throw());
    }

    private IEnumerator Throw()
    {
        yield return new WaitForSeconds(0.3f);
        _lastBall = Instantiate(Ball, new Vector2(1.25f, -1.7f), Quaternion.identity);
        if(_tutorShown)
            _lastBall.Kick += OnKick;
        _lastBall.Init(Top.transform.position.y);
        _lastBall.RigidBody.AddForce(new Vector2(-2f,0f) * _lastBall.RigidBody.mass, ForceMode.Impulse);
        _lastBall.Won += Won;
        _lastBall.Lost += Lost;
            
        foreach (var o in FindObjectsOfType<PlayerMoveBase>())
        {
            o.Reset();
            o.Init(_lastBall);
        }
    }

    private void OnKick()
    {
        _lastBall.Kick -= OnKick;
        _tutorShown = false;
        Tutor.SetActive(false);
        League.gameObject.SetActive(true);
        Score.gameObject.SetActive(true);
    }

    private void Lost()
    {
        _lastBall.Won -= Won;
        _lastBall.Lost -= Lost;
        _lastBall.Collider.enabled = false;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 0f, Camera.main.transform.position.z);
        ThrowBall();
    }

    private void Won()
    {
        _currentLevel++;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", _currentLevel);
        ZPlayerPrefs.SetInt("level", _currentLevel);
        _lastBall.Won -= Won;
        _lastBall.Lost -= Lost;
        Destroy(_lastBall.gameObject);
        StartCoroutine(WonCorutine());
    }

    private IEnumerator WonCorutine()
    {
        foreach (var c in _winText)
        {
            Goal.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        foreach (var p in Particles)
            p.Play();
        yield return new WaitForSeconds(0.5f);
        Goal.text = string.Empty;

        if (_generatorIndex + 1 < _sortedGenerators.Count && _sortedGenerators[_generatorIndex + 1].MinLevel <= _currentLevel)
            _generatorIndex++;

        if (_leagueIndex + 1 < Leagues.Length && _currentLevel >= Leagues[_leagueIndex +1].MinMatches)
        {
            _leagueIndex++;
            NewLeague.text = "NEW LEAGUE";
            NewLeague.faceColor = Leagues[_leagueIndex].Color;
            yield return new WaitForSeconds(2f);
            NewLeague.text = string.Empty;
            Goal.faceColor = Leagues[_leagueIndex].Color;
        }

        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 0f, Camera.main.transform.position.z);
        Field.color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);
        ChangeHeight();
        GeneratePlayers();
        ThrowBall();
        SetLevel();
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
    }

    void SetLevel()
    {
        var ending = "th";
        if (_currentLevel < 10 || _currentLevel > 19)
        {
            var e = _currentLevel % 10;
            switch(e)
            {
                case 1:
                    ending = "st";
                break;
                case 2:
                    ending = "nd";
                break;
                case 3:
                    ending = "rd";
                break;
            }
        }

        Score.text = _currentLevel + ending + " match";
        Score.faceColor = Leagues[_leagueIndex].Color;
        League.text = Leagues[_leagueIndex].Name;
        League.faceColor = Leagues[_leagueIndex].Color;
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            if (_quited)
                return;
            _quited = true;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", _currentLevel);
        }
        else
        {
            _quited = false;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
        }
    }

    private void OnApplicationQuit()
    {
        if (_quited)
            return;
        _quited = true;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", _currentLevel);
    }

    [Serializable]
    public class LeagueSettings
    {
        public int MinMatches;
        public string Name;
        public Color Color;
    }
}
