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

	void Start ()
    {
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

        Goal.faceColor = Leagues[_leagueIndex].Color;
        SetLevel();
        GeneratePlayers();
        ThrowBall();
        Physics.gravity *= 0.6f;
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
