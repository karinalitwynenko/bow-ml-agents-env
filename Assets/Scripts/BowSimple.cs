using UnityEngine;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class BowSimple : Bow {
	public bool animateBowstring;
	GameObject arrow;
	public GameObject arrowPrefab;
	public GameObject target;
	public LineRenderer bowstring;
	bool isArrowPrepared;
	bool isBowstringPulled;
	Vector3 maxBowstringPullout = new Vector3(-2.6f, 0, 0.01f);
	Vector3 bowstringRestPullout = new Vector3(-0.54f, 0, 0.01f);

	public override void CollectObservations(VectorSensor sensor) {
		sensor.AddObservation(isArrowPrepared ? 1 : 0);
		sensor.AddObservation(transform.rotation.z);
		sensor.AddObservation(arrowsLeft);	
    }

	public override void OnActionReceived(ActionBuffers actionBuffers) {
		UseBow(actionBuffers.ContinuousActions);	
    }

	public override void OnEpisodeBegin() {
		arrowsLeft = arrowsOnStart;
		hits = 0;
		points = 0;
		CreateArrow();
		DisplayArrowsLeft();
		DisplayPoints();
	}

	public override void Heuristic(in ActionBuffers actionsOut) {
		var continuousActionsOut = actionsOut.ContinuousActions;
		continuousActionsOut.Clear();
		continuousActionsOut[0] = Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
		continuousActionsOut[1] = Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
        continuousActionsOut[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

	void Update() {
		if(isBowstringPulled && animateBowstring) {
			Vector3 pullout = bowstring.GetPosition(1) + new Vector3(-0.01f, 0, 0);
			if(pullout.x > maxBowstringPullout.x) {
				bowstring.SetPosition(1, pullout);
				arrow.transform.localPosition += new Vector3(-0.01f, 0, 0);
			}
			else {
				Shoot();
				bowstring.SetPosition(1, bowstringRestPullout);
				isBowstringPulled = false;
			}
		}
	}

	public void UseBow(ActionSegment<float> actions) {
		if(actions[0] > 0f) {
			transform.Rotate(Vector3.forward * 200 * Time.deltaTime);
		}
		else if(actions[1] > 0f) {
			transform.Rotate(-Vector3.forward * 200 * Time.deltaTime);
		}

		Vector3 forward = transform.right;
		Vector3 toOther = (transform.position - target.transform.position).normalized;
		if(Vector3.Dot(forward, toOther) < -0.98f) {
			AddReward(0.005f); // agent aims towards the target
		}

		if(actions[2] > 0f && isArrowPrepared) {
			if(animateBowstring) {
				isBowstringPulled = true;
				isArrowPrepared = false; 
			}
			else {
				Shoot();
			}
			
		}
	}

	void Start() {
		CreateArrow();
	}

	public void CreateArrow() {
		Destroy(arrow);
		arrow = Instantiate(arrowPrefab, Vector3.zero, this.transform.localRotation) as GameObject;
		arrow.transform.parent = this.transform;
		arrow.name = "Arrow";
		arrow.transform.localPosition = new Vector3 (0.98f, 0, -0.1f);
		arrow.GetComponent<Arrow>().SetBow(gameObject);
		isArrowPrepared = true;
	}

	public void Shoot() {
		if (arrow.GetComponent<Rigidbody>() == null) {
			bowstring.SetPosition(1, maxBowstringPullout);
			arrow.transform.parent = gameObject.transform.parent;
			arrow.AddComponent<Rigidbody>();
			arrow.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z))*new Vector3(30f,0,0), ForceMode.VelocityChange);	
			bowstring.SetPosition(1, bowstringRestPullout);
			isArrowPrepared = false;
		}
	}

}

