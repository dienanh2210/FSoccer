using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
	
	private float buttonAnimationSpeed = 9;		//speed on animation effect when tapped on button
	private bool  canTap = false;				//flag to prevent double tap
	public AudioClip tapSfx;					//tap sound for buttons click	
	public GameObject playerWins;				//UI 3d text object
	public GameObject playerMoney;				//UI 3d text object

	void Awake (){

		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
		
		playerWins.GetComponent<TextMesh>().text = "" + PlayerPrefs.GetInt("PlayerWins");
		playerMoney.GetComponent<TextMesh>().text = "" + PlayerPrefs.GetInt("PlayerMoney");
	}
	IEnumerator Start() {
		yield return new WaitForSeconds(1.0f);
		canTap = true;
	}

	
	void Update (){	
		if(canTap) {
			StartCoroutine(tapManager());
		}
	}
	
	private RaycastHit hitInfo;
	private Ray ray;
	IEnumerator tapManager (){

		//Mouse of touch?
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			yield break;
			
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			switch(objectHit.name) {
			
			
				case "gameMode_1":								//player vs AI mode
					playSfx(tapSfx);							//play touch sound
					PlayerPrefs.SetInt("GameMode", 0);			//set game mode to fetch later in "Game" scene
					PlayerPrefs.SetInt("IsTournament", 0);		//are we playing in a tournament?
					PlayerPrefs.SetInt("IsPenalty", 0);			//are we playing penalty kicks?
					StartCoroutine(animateButton(objectHit));	//touch animation effect
					yield return new WaitForSeconds(1.0f);		//Wait for the animation to end
					SceneManager.LoadScene("Config-c#");		//Load the next scene
					break;

				case "gameMode_2":								//two player (human) mode
					playSfx(tapSfx);
					PlayerPrefs.SetInt("GameMode", 1);
					PlayerPrefs.SetInt("IsTournament", 0);
					PlayerPrefs.SetInt("IsPenalty", 0);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);
					SceneManager.LoadScene("Config-c#");
					break;	

				case "gameMode_3":
					playSfx(tapSfx);
					PlayerPrefs.SetInt("GameMode", 0);
					PlayerPrefs.SetInt("IsTournament", 1);
					PlayerPrefs.SetInt("IsPenalty", 0);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);

					SceneManager.LoadScene("Config-c#");

					break;	

				case "gameMode_4":								
					playSfx(tapSfx);
					PlayerPrefs.SetInt("GameMode", 0);
					PlayerPrefs.SetInt("IsTournament", 0);
					PlayerPrefs.SetInt("IsPenalty", 1);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);
					SceneManager.LoadScene("Penalty-c#");
					break;
						
				//Option buttons	
				case "Btn-01":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);
					SceneManager.LoadScene("Shop-c#");
					break;

				case "Btn-02":
				case "Status_2":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);
					SceneManager.LoadScene("BuyCoinPack-c#");
					break;

				case "Btn-03":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);
					Application.Quit();
					break;	
			}	
		}
	}

	
	IEnumerator animateButton ( GameObject _btn  ){
		canTap = false;
		Vector3 startingScale = _btn.transform.localScale;	//initial scale	
		Vector3 destinationScale = startingScale * 1.1f;		//target scale
		
		//Scale up
		float t = 0.0f; 
		while (t <= 1.0f) {
			t += Time.deltaTime * buttonAnimationSpeed;
			_btn.transform.localScale = new Vector3( Mathf.SmoothStep(startingScale.x, destinationScale.x, t),
			                                        Mathf.SmoothStep(startingScale.y, destinationScale.y, t),
			                                        _btn.transform.localScale.z);
			yield return 0;
		}
		
		//Scale down
		float r = 0.0f; 
		if(_btn.transform.localScale.x >= destinationScale.x) {
			while (r <= 1.0f) {
				r += Time.deltaTime * buttonAnimationSpeed;
				_btn.transform.localScale = new Vector3( Mathf.SmoothStep(destinationScale.x, startingScale.x, r),
				                                        Mathf.SmoothStep(destinationScale.y, startingScale.y, r),
				                                        _btn.transform.localScale.z);
				yield return 0;
			}
		}
		
		if(r >= 1)
			canTap = true;
	}
	
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}

}