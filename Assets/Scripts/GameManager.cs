﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	[Header("Game UI Elements")]
	[SerializeField] private Text _secondsLeft;
	[SerializeField] private Text _P1ScoreUI;
	[SerializeField] private Text _P2ScoreUI;
	[SerializeField] private GameObject _gameOverPanel;

	[Header("Ground Settings")]
	[SerializeField] private Ground _ground;
	[SerializeField] private int _groundLength;
	[SerializeField] private int _groundHeight;

	[Header("Player 1 Settings")]
	[SerializeField] private Player _player1;
	[SerializeField] private KeyCode _player1Up;
	[SerializeField] private KeyCode _player1Down;
	[SerializeField] private KeyCode _player1Left;
	[SerializeField] private KeyCode _player1Right;
	[SerializeField] private KeyCode _player1Action;
	[SerializeField] private int _player1Speed;
	[SerializeField] private bool _player1CanPlant;
	[SerializeField] private bool _player1CanHarvest;

	[Header("Player 2 Settings")]
	[SerializeField] private Player _player2;
	[SerializeField] private KeyCode _player2Up;
	[SerializeField] private KeyCode _player2Down;
	[SerializeField] private KeyCode _player2Left;
	[SerializeField] private KeyCode _player2Right;
	[SerializeField] private KeyCode _player2Action;
	[SerializeField] private int _player2Speed;
	[SerializeField] private bool _player2CanPlant;
	[SerializeField] private bool _player2CanHarvest;

	private int _P1Score;
	private int _P2Score;
	private bool _isGameOver;

	void Awake () {
		if (instance == null)
			instance = this;
	}

	void Start () {
		InitialiseGame ();
	}

	void InitialiseGame() {
		StopAllCoroutines ();

		_isGameOver = false;

		_groundHeight = Random.Range (3, 5);
		_groundLength = Random.Range (5, 10);

		_P1Score = 0;
		_P2Score = _groundLength * _groundHeight;
		_P1ScoreUI.text = _P1Score.ToString ();
		_P2ScoreUI.text = _P2Score.ToString ();

		_ground.InstantiateGround (_groundLength, _groundHeight);

		_player1.transform.position = Camera.main.ViewportToWorldPoint (new Vector3 (0.15f, 0.5f, 10f));
		_player1.UpdateControls (_player1Up, _player1Down, _player1Left, _player1Right, _player1Action);
		_player1.UpdateAllowedActions (_player1CanPlant, _player1CanHarvest);
		_player1.SetSpeed (_player1Speed);

		_player2.transform.position = Camera.main.ViewportToWorldPoint (new Vector3 (0.85f, 0.5f, 10f));
		_player2.UpdateControls (_player2Up, _player2Down, _player2Left, _player2Right, _player2Action);
		_player2.UpdateAllowedActions (_player2CanPlant, _player2CanHarvest);
		_player2.SetSpeed (_player2Speed);

		StartCoroutine (Countdown ());
	}

	public void IncreaseP1Score () {
		_P1Score++; _P2Score--;
		_P1ScoreUI.text = _P1Score.ToString ();
		_P2ScoreUI.text = _P2Score.ToString ();
	}

	public void IncreaseP2Score () {
		_P2Score++; _P1Score--;
		_P1ScoreUI.text = _P1Score.ToString ();
		_P2ScoreUI.text = _P2Score.ToString ();
	}

	public bool getIsGammeOver() {
		return _isGameOver;
	}

	void SetSecondsLeft (int seconds){
		_secondsLeft.text = seconds.ToString ();
	}

	IEnumerator Countdown() {
		for (int seconds = 10; seconds >= 0; seconds--) {
			SetSecondsLeft (seconds);
			yield return new WaitForSeconds (1);
		}
		_isGameOver = true;
		_gameOverPanel.SetActive (true);
		yield return new WaitForSeconds (10);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}


}
