using Assets.Scripts;
using Assets.Scripts.Generators;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class GameController : MonoBehaviour {

    public Player Player;
    public BallController Ball;

    public float[] _positionsX = new[] { -1f, 0f, 1f };
    public float[] _positionsY = new[] { -1f, 0f, 1f };

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
    private Quaternion _playerRotation = Quaternion.Euler(10f, 0f, 0f);
    private List<Generator> _sortedGenerators = new List<Generator>();
    private int _currentLevel = 0;
    private int _leagueIndex = 0;
    private int _generatorIndex = 0;

    private string _winText = "GOAL!!!";

	void Start ()
    {
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
    }

    void GeneratePlayers()
    {
        foreach (var p in _players)
            Destroy(p.gameObject);
        _players.Clear();

        _players.AddRange(_sortedGenerators[_generatorIndex].Generate(Player, _positionsX, _positionsY));
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
        _lastBall = Instantiate(Ball, new Vector2(1.4f, -2f), Quaternion.identity);
        _lastBall.RigidBody.AddForce(new Vector2(-2f,4f), ForceMode.Impulse);
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
        _currentLevel++;
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
    }

    void SetLevel()
    {
        var level = (_currentLevel - Leagues[_leagueIndex].MinMatches) + 1;
        var ending = "th";
        if (level < 10 || level > 19)
        {
            var e = level % 10;
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

        Score.text = level + ending + " match";
        Score.faceColor = Leagues[_leagueIndex].Color;
        League.text = Leagues[_leagueIndex].Name;
        League.faceColor = Leagues[_leagueIndex].Color;
    }

    [Serializable]
    public class LeagueSettings
    {
        public int MinMatches;
        public string Name;
        public Color Color;
    }
}
