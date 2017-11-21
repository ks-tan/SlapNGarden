using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour {

	[SerializeField] private Sprite[] _groundTileSet;
	private SpriteRenderer _groundTileSpriteRenderer;

	void Awake () {
		_groundTileSpriteRenderer = GetComponent<SpriteRenderer> ();
		SetRandomTile ();
	}

	void SetRandomTile () {
		int tileIndex = Random.Range (0, _groundTileSet.Length);
		_groundTileSpriteRenderer.sprite = _groundTileSet [tileIndex];
	}

}
