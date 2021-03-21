using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {
	const string WALL = "Wall";
	const string TARGET = "Target";
	GameObject points;
	public GameObject pointsPrefab;
	public GameObject bow;
	bool collided = false;
	float alpha = 1f;
	float fadeOutRate = 0.6f;
	bool targetHit = false;

	void Update() {
		if (collided) {
			if(targetHit && alpha > 0) {
				alpha -= Time.deltaTime * fadeOutRate;
				GetComponent<Renderer>().material.color = new Color(
					Color.white.r, 
					Color.white.g, 
					Color.white.b, 
					alpha
				);
			}
			else {
				bow.GetComponent<BowSimple>().CreateArrow();
			}
		}
		else if (transform.GetComponent<Rigidbody>() != null) {
			Vector3 velocity = GetComponent<Rigidbody>().velocity;
			if (velocity != Vector3.zero) {
				// calculate rotation update
				float angleZ = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
				float angleY = Mathf.Atan2(velocity.z, velocity.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3(0, -angleY, angleZ);
			}
		}	
	}

	void OnCollisionEnter(Collision collision) {
		collided = true;
		float collisionY;
		float points;
		var bowSimpleScript = bow.GetComponent<BowSimple>();

		if(collision.transform.name == WALL) {
			bowSimpleScript.AddReward(-0.002f);
			bowSimpleScript.SetArrowsLeft(bowSimpleScript.GetArrowsLeft() - 1);
			bowSimpleScript.DisplayArrowsLeft();
		}
		else if(collision.transform.name == TARGET) {
			targetHit = true;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().isKinematic = true;
			collisionY = collision.GetContact(0).point.y;
			collisionY -= collision.transform.position.y;

			GameObject pointsText = Instantiate(
				pointsPrefab,
				bow.GetComponent<BowSimple>().target.transform.position + new Vector3(-1f, 4f, -1f),
				Quaternion.identity,
				this.transform
			) as GameObject;

			TextMesh pointsTextMesh = pointsText.GetComponent<TextMesh>();
			if (collisionY < 0.62 && collisionY > -0.62)
				points = 1f;
			else if (collisionY < 1.03 && collisionY > -1.03)
				points = 0.9f;
			else if (collisionY < 1.67 && collisionY > -1.67)
				points = 0.8f;
			else
				points = 0.7f;

			pointsTextMesh.text = "+" + points.ToString().Replace(",", ".");
			bowSimpleScript.SetPoints(bowSimpleScript.GetComponent<BowSimple>().GetPoints() + points);
			bowSimpleScript.DisplayPoints();
			bowSimpleScript.AddReward(points);
			bowSimpleScript.SetHits(bowSimpleScript.GetHits() + 1);
		}

		bowSimpleScript.DisplayAccuracy();

		if(bowSimpleScript.GetArrowsLeft() < 0) {
			bowSimpleScript.AddReward((bowSimpleScript.arrowsOnStart - bowSimpleScript.GetHits()) * -0.1f);
			bowSimpleScript.EndEpisode();
		}
	}

	public void SetBow(GameObject bow) {
		this.bow = bow;
	}

}

