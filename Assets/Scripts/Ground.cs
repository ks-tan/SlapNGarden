using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

	[SerializeField] private GameObject _groundTile;
	[SerializeField] private GameObject _plant;
	[SerializeField] private GameObject _fence;
	[SerializeField] private Sprite[] _fenceSprites;
	private float tileLength;
	private Transform topCollider;
	private Transform bottomCollider;
	private Transform leftCollider;
	private Transform rightCollider;

	void Awake () {
		tileLength = _groundTile.GetComponent<SpriteRenderer> ().bounds.size.x;
	}

	void CenterCamera (int length, int height) {
		float centerX = length / 2f * tileLength - tileLength / 2;
		float centerY = height / 2f * tileLength - tileLength / 2;
		float zPosition = Camera.main.transform.position.z;
		Camera.main.transform.position = new Vector3 (centerX, centerY + 1, zPosition);
	}

	void ZoomCameraToFit (GameObject topCornerTile, GameObject bottomCornerTile) {
		Vector2 topCornerViewportPoint = Camera.main.WorldToViewportPoint (topCornerTile.transform.position);
		Vector2 bottomCornerViewPortPoint = Camera.main.WorldToViewportPoint (bottomCornerTile.transform.position);
		bool isTopCornerOnScreen =  topCornerViewportPoint.x > 0 && topCornerViewportPoint.x < 1 && topCornerViewportPoint.y > 0 && topCornerViewportPoint.y < 1;
		bool isBottomCornerOnScreen =  bottomCornerViewPortPoint.x > 0 && bottomCornerViewPortPoint.x < 1 && bottomCornerViewPortPoint.y > 0 && bottomCornerViewPortPoint.y < 1;
		if (isTopCornerOnScreen && isBottomCornerOnScreen)
			return;
		Camera.main.orthographicSize = Camera.main.orthographicSize + 2;
		ZoomCameraToFit (topCornerTile, bottomCornerTile);
	}

	public void InstantiateGround (int length, int height) {
		GameObject topCornerTile = null;
		GameObject bottomCornerTile = null;
		for (int col = 0; col < length; col++) {
			for (int row = 0; row < height; row++) {
				GameObject currentTile = GameObject.Instantiate (_groundTile, transform);
				GameObject plant = GameObject.Instantiate (_plant,currentTile.transform);
				currentTile.transform.position = new Vector2 (col * tileLength, row * tileLength);
				plant.transform.position = new Vector2 (col * tileLength, row * tileLength);
				if (col == 0 && row == 0) 
					bottomCornerTile = currentTile;
				if (col == length - 1 && row == height - 1) 
					topCornerTile = currentTile;
			}
		}
		CenterCamera (length, height);
		ZoomCameraToFit (topCornerTile, bottomCornerTile);
		GenerateScreenEdgeColliders ();
	}

	public void GenerateScreenEdgeColliders() {
		//Generate our empty objects
		topCollider = new GameObject().transform;
		bottomCollider = new GameObject().transform;
		rightCollider = new GameObject().transform;
		leftCollider = new GameObject().transform;

		//Name our objects 
		topCollider.name = "TopCollider";
		bottomCollider.name = "BottomCollider";
		rightCollider.name = "RightCollider";
		leftCollider.name = "LeftCollider";

		//Add the colliders
		topCollider.gameObject.AddComponent<BoxCollider2D>();
		bottomCollider.gameObject.AddComponent<BoxCollider2D>();
		rightCollider.gameObject.AddComponent<BoxCollider2D>();
		leftCollider.gameObject.AddComponent<BoxCollider2D>();

		//Make them the child of camera
		topCollider.parent = Camera.main.transform;
		bottomCollider.parent = Camera.main.transform;
		rightCollider.parent = Camera.main.transform;
		leftCollider.parent = Camera.main.transform;

		//Generate world space point information for position and scale calculations
		Vector2 cameraPos = Camera.main.transform.position;
		Vector2 screenSize;
		screenSize.x = Vector2.Distance (Camera.main.ScreenToWorldPoint(new Vector2(0,0)),Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f;
		screenSize.y = Vector2.Distance (Camera.main.ScreenToWorldPoint(new Vector2(0,0)),Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f;

		//Change our scale and positions to match the edges of the screen...
		int colDepth = 1;
		int zPosition = 0;
		rightCollider.localScale = new Vector3(1, screenSize.y * 2, 1);
		rightCollider.position = new Vector3(cameraPos.x + screenSize.x + (rightCollider.localScale.x * 0.5f), cameraPos.y, zPosition);
		leftCollider.localScale = new Vector3(colDepth, screenSize.y * 2, colDepth);
		leftCollider.position = new Vector3(cameraPos.x - screenSize.x - (leftCollider.localScale.x * 0.5f), cameraPos.y, zPosition);
		topCollider.localScale = new Vector3(screenSize.x * 2, colDepth, colDepth);
		topCollider.position = new Vector3(cameraPos.x, cameraPos.y + screenSize.y + (topCollider.localScale.y * 0.5f), zPosition);
		bottomCollider.localScale = new Vector3(screenSize.x * 2, colDepth, colDepth);
		bottomCollider.position = new Vector3(cameraPos.x, cameraPos.y - screenSize.y - (bottomCollider.localScale.y * 0.5f), zPosition);
	}
}
