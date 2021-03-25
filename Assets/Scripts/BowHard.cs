using UnityEngine;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class BowHard : Bow {
	public bool animateBowstring;
	float stringPullout;
	float maxBowstringPullout = -2.2f;
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

	public void UseBow(ActionSegment<float> actions) {
		if(actions[0] > 0f) {
			transform.Rotate(Vector3.forward * 200 * Time.deltaTime);
		}
		else if(actions[1] > 0f) {
			transform.Rotate(-Vector3.forward * 200 * Time.deltaTime);
		}

		if(actions[2] > 0f) {
			PullBowstring();
		}
		else if(isBowstringPulled) {
			Vector3 forward = transform.right;
			Vector3 toOther = (transform.position - target.transform.position).normalized;

			if(stringPullout > -0.5f) { // pullout is too weak
				AddReward(-0.05f); 
			}
			else if(Vector3.Dot(forward, toOther) < -0.95f && stringPullout < -1f) {
				AddReward(0.02f); // agents shoots towards the target
			}				

			Shoot();
		}
	}

	void PullBowstring() {
		if(!isArrowPrepared || stringPullout < maxBowstringPullout)
			return;

		stringPullout -= 0.06f;
		bowstring.SetPosition(1, new Vector3(stringPullout, bowstringRestPullout.y, bowstringRestPullout.z));
		arrow.transform.localPosition = arrow.transform.localPosition - new Vector3(0.06f, 0f, 0f);
		isBowstringPulled = true;
	}

	public void Shoot() {
		if (arrow.GetComponent<Rigidbody>() == null) {
			arrow.transform.parent = gameObject.transform.parent;
			arrow.AddComponent<Rigidbody>();
			arrow.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z))*new Vector3(-stringPullout * 14,0,0), ForceMode.VelocityChange);	
			stringPullout = bowstringRestPullout.x;
			bowstring.SetPosition(1, bowstringRestPullout);
			isArrowPrepared = false;
			isBowstringPulled = false;
		}
	}

}

