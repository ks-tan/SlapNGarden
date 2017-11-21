using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {

	[SerializeField] private Sprite[] _plantSprites;
	[SerializeField] private int _maxSpeed;
	[SerializeField] private GameObject _triggerForMoving;
	[SerializeField] private AudioClip[] _plantAudios;
	private int _plantIndex;
	private Rigidbody2D _plantRigidbody;
	private SpriteRenderer _plantSpriteRenderer;
	private Animator _plantAnimator;
	private AudioSource _plantAudioSource;
	private bool _isPlanted;
	private bool _isGrabbed;

	void Awake () {
		_plantRigidbody = GetComponent<Rigidbody2D> ();
		_plantSpriteRenderer = GetComponent<SpriteRenderer> ();
		_plantAnimator = GetComponent<Animator> ();
		_plantAudioSource = GetComponent<AudioSource> ();
		_isPlanted = true;
		_isGrabbed = false;
		SetPlantIndex ();
	}

	void SetPlantIndex () {
		_plantIndex = Random.Range (0, _plantSprites.Length);
	}

	void Update () {
		transform.rotation = Quaternion.identity;
		if (_isPlanted || _isGrabbed) {	
			_plantRigidbody.velocity = new Vector2 (0, 0);
		} else {
			bool isNotMoving = _plantRigidbody.velocity == new Vector2 (0, 0);
			bool isChangingDirection = Random.Range (0, 50) == 25;
			if (isNotMoving || isChangingDirection)
				_plantRigidbody.velocity = new Vector2 (Random.Range (-_maxSpeed, _maxSpeed), Random.Range (-_maxSpeed, _maxSpeed));
			if (_plantRigidbody.velocity.x >= 0)
				transform.localScale = new Vector3 (1, 1, 1);
			else
				transform.localScale = new Vector3 (-1, 1, 1);
		}
	}

	public void SetIsGrabbed (bool isGrabbed) {
		_isGrabbed = isGrabbed;
		if (_isGrabbed) {
			_plantAudioSource.clip = _plantAudios [1];
			_plantAudioSource.Play ();
			_plantAudioSource.loop = false;
			if (_plantIndex == 0)
				_plantAnimator.Play ("PlantWalk0", -1, 0f);
			if (_plantIndex == 1)
				_plantAnimator.Play ("PlantWalk1", -1, 0f);
		}
	}

	public void SetIsPlanted (bool isPlanted) {
		_isPlanted = isPlanted;
		if (!isPlanted) {
			_plantAudioSource.clip = _plantAudios [2];
			_plantAudioSource.Play ();
			_plantAudioSource.loop = true;
			_plantSpriteRenderer.sortingOrder = 2;
			_triggerForMoving.SetActive (true);
			if (_plantIndex == 0)
				_plantAnimator.Play ("PlantWalk0", -1, 0f);
			if (_plantIndex == 1)
				_plantAnimator.Play ("PlantWalk1", -1, 0f);
		} else {
			_plantAudioSource.clip = _plantAudios [0];
			_plantAudioSource.Play ();
			_plantAudioSource.loop = false;
			_plantAnimator.Play ("Idle", -1, 0f);
			_plantSpriteRenderer.sortingOrder = 1;
			_triggerForMoving.SetActive (false);
		}
	}

	public bool GetIsPlanted() {
		return _isPlanted;
	}

}
