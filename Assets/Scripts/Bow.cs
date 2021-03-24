using UnityEngine;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Bow : Agent {
	[Tooltip("Maximum number of arrows available in an episode.")]
	public int arrowsOnStart;
	public TextMesh pointsTextMesh;
	public TextMesh arrowsTextMesh;
	public TextMesh accuracyTextMesh;
	protected int arrowsLeft;
	protected int hits;
	protected float points;
	
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