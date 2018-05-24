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
    private float _lengthCoef = 1f;
    private float _fieldLength;

    public GameObject[] ToChangeHeight;
    public GameObject[] ToMoveUp;
    public Transform Top;

	void Start ()
    {
        var p = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 13));
        _fieldLength = -p.y;
#if RELEASE
        GameAnalytics.Initialize();
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
#endif
        _currentLevel = Math.Max(1,ZPlayerPrefs.GetInt("level", 1));
        _sortedGenerators.AddRange(Generators);
        _sortedGenerators.Sort(new GeneratorComparer());
        for (var i = 0; i < _sortedGenerators.Count && _sortedGenerators[i].MinLevel <= _currentLevel; i++)
            _generatorIndex = i;

        for (var i = 1; i < Leagues.Length && _currentLevel >= Leagues[i].MinMatches; i++)
            _leagueIndex = i;

        //ChangeHeight();

        Goal.faceColor = Leagues[_leagueIndex].Color;
        SetLevel();
        GeneratePlayers();
        ThrowBall();
        Physics.gravity *= 0.6f;
    }

    private void Update()
    {
        if (_lengthCoef <= 1f || _lastBall == null)
            return;

        var p = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 13));
        if (_lastBall.gameObject.transform.position.y > p.y - p.y)
        {
            var pos = Camera.main.gameObject.transform.position;
            Camera.main.gameObject.transform.position = new Vector3(pos.x, pos.y += Time.deltaTime, pos.z);
        }
        else if (Camera.main.gameObject.transform.position.y > 0f)
        {
            var pos = Camera.main.gameObject.transform.position;
            Camera.main.gameObject.transform.position = new Vector3(pos.x, pos.y -= Time.deltaTime, pos.z);
        }
    }

    void ChangeHeight()
    {
        
        foreach (var g in ToChangeHeight)
        {
            g.transform.localScale = new Vector3(g.transform.localScale.x,
                g.transform.localScale.y * _lengthCoef,
                g.transform.localScale.z);

            g.transform.position = new Vector3(g.transform.position.x,
                _lengthCoef * _fieldLength / 2f,
                g.transform.position.z);
        }
            
        foreach(var g in ToMoveUp)
        {
            g.transform.position = new Vector3(g.transform.position.x, 
                ((g.transform.position.y + _fieldLength) * _lengthCoef) - ((_lengthCoef / 2) * _fieldLength), 
                g.transform.position.z);
        }
    }

    void GeneratePlayers()
    {
        foreach (var p in _players)
            Destroy(p.gameObject);
        _players.Clear();

        _players.AddRange(_sortedGenerators[_generatorIndex].Generate(Player));
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
        _lastBall.Init(Top.transform.position.y);
        _lastBall.RigidBody.AddForce(new Vector2(-2f,0f) * _lastBall.RigidBody.mass, ForceMode.Impulse);
        _lastBall.Won += Won;
        _lastBall.Lost += Lost;
    }

    private void Lost()
    {
        _lastBall.Won -= Won;
        _lastBall.Lost -= Lost;
        _lastBall.Collider.enabled = false;
        ThrowBall();
    }

    private void Won()
    {
        _currentLevel++;
#if RELEASE
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", _currentLevel);
#endif
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
            yield return new WaitForSeconds(0.1f);
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

        Field.color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);
        GeneratePlayers();
        ThrowBall();
        SetLevel();
#if RELEASE
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
#endif
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
#if RELEASE
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", _currentLevel);
#endif
        }
        else
        {
            _quited = false;
#if RELEASE
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
#endif
        }
    }

    private void OnApplicationQuit()
    {
        if (_quited)
            return;
        _quited = true;
#if RELEASE
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", _currentLevel);
#endif
    }

    [Serializable]
    public class LeagueSettings
    {
        public int MinMatches;
        public string Name;
        public Color Color;
    }
}
