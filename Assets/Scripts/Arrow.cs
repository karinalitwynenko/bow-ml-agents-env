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
				bow.GetComponent<Bow>().CreateArrow();
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
		var bowScript = bow.GetComponent<Bow>();

		if(collision.transform.name == WALL) {
			bowScript.AddReward(-0.002f);
			bowScript.SetArrowsLeft(bowScript.GetArrowsLeft() - 1);
			bowScript.DisplayArrowsLeft();
		}
		else if(collision.transform.name == TARGET) {
			targetHit = true;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().isKinematic = true;
			collisionY = collision.GetContact(0).point.y;
			collisionY -= collision.transform.position.y;

			GameObject pointsText = Instantiate(
				pointsPrefab,
				bowScript.target.transform.position + new Vector3(-1f, 4f, -1f),
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
			bowScript.SetPoints(bowScript.GetPoints() + points);
			bowScript.DisplayPoints();
			bowScript.AddReward(points);
			bowScript.SetHits(bowScript.GetHits() + 1);
		}

		bowScript.DisplayAccuracy();

		if(bowScript.GetArrowsLeft() < 0) {
			bowScript.SetReward(bowScript.GetPoints() + (bowScript.arrowsOnStart - bowScript.GetHits()) * -0.1f);
			// bowScript.AddReward((bowScript.arrowsOnStart - bowScript.GetHits()) * -0.1f);
			bowScript.EndEpisode();
		}
	}

	public void SetBow(GameObject bow) {
		this.bow = bow;
	}

}

