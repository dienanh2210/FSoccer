using UnityEngine;
using System.Collections;

public class BallManager : MonoBehaviour {
		
	private GameObject gameController;	//Reference to main game controller
	public AudioClip ballHitPost;				//Sfx for hitting the poles

	void Awake (){
		gameController = GameObject.FindGameObjectWithTag("GameController");
	}

	void Update (){
		manageBallFriction();
	}

	void LateUpdate (){
		//we restrict rotation and position once again to make sure that ball won't has an unwanted effect.
		transform.position = new Vector3(transform.position.x,
			                             transform.position.y,
			                             -0.5f);

	
	}

	private float ballSpeed;
	void manageBallFriction (){
		ballSpeed = GetComponent<Rigidbody>().velocity.magnitude;
		//print("Ball Speed: " + rigidbody.velocity.magnitude);
		if(ballSpeed < 0.5f) {
			
			GetComponent<Rigidbody>().drag = 2;
		} else {
			//let it slide
			GetComponent<Rigidbody>().drag = 0.9f;
		}
	}

	void OnCollisionEnter ( Collision other  ){
		switch(other.gameObject.tag) {
			case "gatePost":
				playSfx(ballHitPost);
				break;
		}
	}

	void OnTriggerEnter ( Collider other  ){
		switch(other.gameObject.tag) {
			case "opponentGoalTrigger":
				StartCoroutine(gameController.GetComponent<GlobalGameManager>().managePostGoal("Player"));
				break;
				
			case "playerGoalTrigger":
				StartCoroutine(gameController.GetComponent<GlobalGameManager>().managePostGoal("Opponent"));
				break;
		}
	}


	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}

}