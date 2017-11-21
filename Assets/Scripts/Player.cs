using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	[SerializeField] Sprite _grabSprite;
	[SerializeField] Sprite _idleSprite;
	[SerializeField] AudioClip[] _playerAudios;
	private KeyCode _right;
	private KeyCode _left;
	private KeyCode _up;
	private KeyCode _down;
	private KeyCode _action;
	private int _speed;
	private bool _canPlant;
	private bool _canHarvest;
	private Player _anotherPlayer;
	private GameObject _currentGoundTile;
	private GameObject _currentMovingPlant;
	private bool _hasLostControl;
	private Rigidbody2D _playerRigidbody;
	private Animator _playerAnimator;
	private SpriteRenderer _playerSpriteRenderer;
	private AudioSource _playerAudioSource;

	void Awake() {
		_playerRigidbody = GetComponent<Rigidbody2D> ();
		_playerAnimator = GetComponent<Animator> ();
		_playerSpriteRenderer = GetComponent<SpriteRenderer> ();
		_playerAudioSource = GetComponent<AudioSource> ();
	}

	void Start() {
		_hasLostControl = false;
	}

	void Update () {
		if (!_hasLostControl) {
			if (Input.GetKey (_right))
				transform.position += new Vector3 (1, 0) * _speed * Time.deltaTime;
			if (Input.GetKey (_left))
				transform.position += new Vector3 (-1, 0) * _speed * Time.deltaTime;
			if (Input.GetKey (_up))
				transform.position += new Vector3 (0, 1) * _speed * Time.deltaTime;
			if (Input.GetKey (_down))
				transform.position += new Vector3 (0, -1) * _speed * Time.deltaTime;
			if (Input.GetKeyDown (_action)) {
				bool isTouchingAnotherPlayer = _anotherPlayer != null;
				if (isTouchingAnotherPlayer) {
					if (!_anotherPlayer.hasLostControl ()) {
						_playerAudioSource.clip = _playerAudios [0];
						_playerAudioSource.Play ();
						Camera.main.GetComponent<CameraShake> ().ShakeCamera (0.5f, 0.011f);
						StartCoroutine (_anotherPlayer.Slapped ());
						_playerAnimator.Play ("PlayerSlap", -1, 0f);
					}
				}
			}
			if (Input.GetKeyDown (_action)) {
				_playerSpriteRenderer.sprite = _grabSprite;
				if (_canHarvest) {
					bool isTouchingGroundTile = _currentGoundTile != null && _currentGoundTile.GetComponent<SpriteRenderer> ().bounds.Intersects (_playerSpriteRenderer.bounds);
					if (isTouchingGroundTile) {
						bool isGrabbingPlant = transform.childCount > 0;
						bool isTilePlanted = _currentGoundTile.transform.childCount > 0;
						bool canHarvest = isTilePlanted && !isGrabbingPlant;
						if (canHarvest) {
							GameObject currentPlant = _currentGoundTile.transform.GetChild (0).gameObject;
							GrabPlant (currentPlant);
							GameManager.instance.IncreaseP1Score ();
						}
					}
				}
				if (_canPlant) {
					bool isTouchingMovingPlant = _currentMovingPlant != null && _currentMovingPlant.GetComponent<SpriteRenderer> ().bounds.Intersects (_playerSpriteRenderer.bounds) && !_currentMovingPlant.GetComponent<Plant> ().GetIsPlanted ();
					bool isGrabbingPlant = transform.childCount > 0;
					if (isTouchingMovingPlant && !isGrabbingPlant) {
						GrabPlant (_currentMovingPlant);
					}
				}
			}
			if (Input.GetKeyUp (_action)) {
				_playerSpriteRenderer.sprite = _idleSprite;
				bool isGrabbingPlant = transform.childCount > 0;
				if (_canHarvest) {
					if (isGrabbingPlant) {
						ReleasePlant ();
					}
				}
				if (_canPlant) {
					if (isGrabbingPlant) {
						GameObject currentPlant = transform.GetChild (0).gameObject;
						if (_currentGoundTile == null || _currentGoundTile.transform.childCount > 0) {
							ReleasePlant ();
						} else {
							PlantPlant (currentPlant);
							GameManager.instance.IncreaseP2Score ();
						}
					}
				}
			}
		} else {
			_playerRigidbody.velocity = _playerRigidbody.velocity * 0.98f;
		}
	}

	void ReleasePlant() {
		GameObject currentPlant = transform.GetChild (0).gameObject;
		currentPlant.transform.SetParent (null);
		currentPlant.GetComponent<BoxCollider2D> ().isTrigger = false;
		currentPlant.GetComponent<Plant> ().SetIsPlanted (false);
		currentPlant.GetComponent<Plant> ().SetIsGrabbed (false);
	}

	void GrabPlant(GameObject plant) {
		plant.transform.SetParent (transform);
		plant.GetComponent<Plant> ().SetIsGrabbed (true);
		plant.GetComponent<BoxCollider2D> ().isTrigger = true;
		plant.transform.position = new Vector2 (transform.position.x - 0.5f, transform.position.y + 0.5f);
	}

	void PlantPlant(GameObject plant) {
		plant.GetComponent<BoxCollider2D> ().isTrigger = true;
		plant.transform.SetParent(_currentGoundTile.transform);
		plant.transform.position = _currentGoundTile.transform.position;
		plant.GetComponent<Plant> ().SetIsPlanted (true);
		plant.GetComponent<Plant> ().SetIsGrabbed (false);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			_anotherPlayer = other.GetComponent<Player> ();
		}
		if (other.gameObject.tag == "GroundTile") {
			_currentGoundTile = other.gameObject;
			_currentGoundTile.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0.5f);
		}
		if (other.gameObject.tag == "MovingPlant") {
			_currentMovingPlant = other.transform.parent.gameObject;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			_anotherPlayer = null;
		}
		if (other.gameObject.tag == "GroundTile") {
			_currentGoundTile = null;
			GameObject currentGoundTile = other.gameObject;
			currentGoundTile.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1);
		}
	}

	public void UpdateControls (KeyCode up, KeyCode down, KeyCode left, KeyCode right, KeyCode action) {
		this._up = up;
		this._down = down;
		this._left = left;
		this._right = right;
		this._action = action;
	}

	public void UpdateAllowedActions (bool canPlant, bool canHarvest) {
		this._canPlant = canPlant;
		this._canHarvest = canHarvest;
	}

	public void SetSpeed (int speed) {
		this._speed = speed;
	}

	public IEnumerator Slapped() {
		_hasLostControl = true;
		bool isGrabbingPlant = transform.childCount > 0;
		if (isGrabbingPlant) ReleasePlant ();
		GetComponent<BoxCollider2D> ().isTrigger = false;
		_playerSpriteRenderer.sprite = _idleSprite;
		_playerRigidbody.velocity = new Vector2 (Random.Range (-20, 20), Random.Range (-20, 20));
		for (int i = 0; i < 4; i++) {
			_playerSpriteRenderer.color = new Color (1, 1, 1, 0);
			yield return new WaitForSeconds(0.125f);
			_playerSpriteRenderer.color = new Color (1, 1, 1, 1);
			yield return new WaitForSeconds(0.125f);
		}
		yield return new WaitForSeconds(1f);
		_playerRigidbody.velocity = new Vector2 (0, 0);
		_anotherPlayer = null;
		_hasLostControl = false;
		GetComponent<BoxCollider2D> ().isTrigger = true;
	}

	public bool hasLostControl() {
		return _hasLostControl;
	}
}
