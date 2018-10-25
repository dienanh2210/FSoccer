using UnityEngine;
using System.Collections;

public class GoalKeeperController : MonoBehaviour {

	public bool isGoalkeeper = false;
	[Range(0.7f, 2.0f)]
	public float moveSpeed = 1.2f;		//increasing this parameter will result in a better reflex of goalkeeper

	private bool canMove = false;
	private float startDelay = 3.0f;

	IEnumerator Start () {
		
		if(!GlobalGameManager.isPenaltyKick)
			this.enabled = false;

		yield return new WaitForSeconds(startDelay);
		canMove = true;
	}
	
	void Update () {

		checkIsGoalKeeper();

		if(isGoalkeeper && canMove && !GlobalGameManager.goalHappened && !GlobalGameManager.gameIsFinished)
			StartCoroutine(moveGoalkeeper());

	}

	
	void checkIsGoalKeeper () {

	
		if(PenaltyController.penaltyRound % 2 == 1) {
			if(this.gameObject.tag == "Opponent" || this.gameObject.tag == "Player_2")
				isGoalkeeper = true;
			else 
				isGoalkeeper = false;
		}


		if(PenaltyController.penaltyRound % 2 == 0) {
			if(this.gameObject.tag == "Player")
				isGoalkeeper = true;
			else 
				isGoalkeeper = false;
		}
	}

	public IEnumerator moveGoalkeeper() {

		if(canMove)
			canMove = false;

		Vector3 cPos = transform.position;
		Vector3 dest = getNewDestination(transform.position);
		//print ("Destination: " + dest);

		float t = 0;
		while(t < 1) {
			t += Time.deltaTime * moveSpeed;
			transform.position = new Vector3(dest.x,
			                                 Mathf.SmoothStep(cPos.y, dest.y, t),
			                                 dest.z);
			yield return 0;
		}

		if(t >= 1) {
			canMove = true;
			yield break;
		}
	}
	Vector3 getNewDestination(Vector3 p) {

		int dir = 1;

		if(p.y >= 0)
			dir = -1;
		else 
			dir = 1;

		return new Vector3(13, Mathf.Abs(UnityEngine.Random.Range(-4.0f, 4.0f)) * dir, p.z);
	}

}
 