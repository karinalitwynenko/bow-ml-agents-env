using UnityEngine;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Bow : Agent {
	[Tooltip("Maximum number of arrows available in an episode.")]
	public int arrowsOnStart;
	protected GameObject arrow;
	public GameObject arrowPrefab;
	public GameObject target;
	public LineRenderer bowstring;
	protected bool isArrowPrepared;
	protected bool isBowstringPulled;
	public TextMesh pointsTextMesh;
	public TextMesh arrowsTextMesh;
	public TextMesh accuracyTextMesh;
	protected int arrowsLeft;
	protected int hits;
	protected float points;
	
	public void CreateArrow() {
		Destroy(arrow);
		arrow = Instantiate(arrowPrefab, Vector3.zero, this.transform.localRotation) as GameObject;
		arrow.transform.parent = this.transform;
		arrow.name = "Arrow";
		arrow.transform.localPosition = new Vector3 (0.98f, 0, -0.1f);
		arrow.GetComponent<Arrow>().SetBow(gameObject);
		isArrowPrepared = true;
	}

	public void SetHits(int hits) {
		this.hits = hits;
	}

	public int GetHits() {
		return hits;
	}

	public float GetPoints() {
		return points;
	}

	public void SetPoints(float points) {
		this.points = points;
	}

	public int GetArrowsLeft() {
		return arrowsLeft;
	}

	public void SetArrowsLeft(int arrows) {
		arrowsLeft = arrows;
	}

	public void DisplayPoints() {
		if(pointsTextMesh != null) {
			pointsTextMesh.text = "Points: " + points;
		}
	}

	public void DisplayArrowsLeft() {
		if(arrowsTextMesh != null) {
			arrowsTextMesh.text = "Arrows: " + arrowsLeft;
		}
	}

	public void DisplayAccuracy() {
		if(accuracyTextMesh == null) {
			return;
		}

		if(arrowsLeft != arrowsOnStart || hits > 0) {
			accuracyTextMesh.text = "Accuracy: " + ((float)hits / (arrowsOnStart - arrowsLeft + hits));
		}
		else {
			accuracyTextMesh.text = "Accuracy: 0";
		}
	}
	
}