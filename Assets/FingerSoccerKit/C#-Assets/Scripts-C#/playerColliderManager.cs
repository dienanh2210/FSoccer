using UnityEngine;
using System.Collections;

public class playerColliderManager : MonoBehaviour {			

	public AudioClip unitsBallHit;		//units hits the ball sfx
	public AudioClip unitsGeneralHit;	//units general hit sfx (Not used)

	void OnCollisionEnter ( Collision other  ){
		switch(other.gameObject.tag) {
			case "Opponent":
				//PlaySfx(unitsGeneralHit);
				break;
			case "ball":
				PlaySfx(unitsBallHit);
				break;
		}
	}
	
	void PlaySfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}

}