using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ConfigController : MonoBehaviour {
	private int isTournamentMode;	
	public GameObject p1TeamSel;		//p1 team selection settings
	public GameObject p1FormationSel;	//p1 formation selection settings
	public GameObject p2TeamSel;		//p2 team selection settings
	public GameObject p2FormationSel;	//p2 formation selection settings
	public GameObject timeSel;			//time selection settings


	private float buttonAnimationSpeed = 11;	//speed on animation effect when tapped on button
	private bool  canTap = true;				//flag to prevent double tap
	public AudioClip tapSfx;					//tap sound for buttons click

	public Texture2D[] availableTeams;			//just the images.
	public string[] availableFormations;		//Just the string values. We setup actual values somewhere else.
	public string[] availableTimes;				//Just the string values. We setup actual values somewhere else.


	public GameObject p1Team;
	public GameObject p1PowerBar;
	public GameObject p1TimeBar;
	public GameObject p2Team;
	public GameObject p2PowerBar;
	public GameObject p2TimeBar;
	public GameObject p1FormationLabel;			//UI 3d text object
	public GameObject p2FormationLabel;			//UI 3d text object
	public GameObject gameTimeLabel;			//UI 3d text object

	private int p1FormationCounter = 0;			//Actual player-1 formation index
	private int p1TeamCounter = 0;				//Actual player-1 team index
	private int p2FormationCounter = 0;			//Actual player-2 formation index
	private int p2TeamCounter = 0;				//Actual player-2 team index
	private int timeCounter = 0;				//Actual game-time index

	void Awake (){

		//check if this config scene is getting used for tournament or normal play mode
		isTournamentMode = PlayerPrefs.GetInt("IsTournament");
		if(isTournamentMode == 1) {

			//first of all, check if we are going to continue an unfinished tournament
			if(PlayerPrefs.GetInt("TorunamentLevel") > 0) {
				//if so, there is no need for any configuration. load the next scene.
				SceneManager.LoadScene("Tournament-c#");
				return;
			}


			//disable unnecessary options
			p2TeamSel.SetActive(false);
			p2FormationSel.SetActive(false);
			timeSel.SetActive(false);

			p1TeamSel.transform.position = new Vector3(0, 4.5f, -1);
			p1FormationSel.transform.position = new Vector3(0, -4.3f, -1);
		}

		p1FormationLabel.GetComponent<TextMesh>().text = availableFormations[p1FormationCounter];	//loads default formation
		p1PowerBar.transform.localScale = new Vector3(TeamsManager.getTeamSettings(p1TeamCounter).x / 10,
		                                              p1PowerBar.transform.localScale.y,
		                                              p1PowerBar.transform.localScale.z);
		p1TimeBar.transform.localScale = new Vector3(TeamsManager.getTeamSettings(p1TeamCounter).y / 30,
		                                             p1TimeBar.transform.localScale.y,
		                                             p1TimeBar.transform.localScale.z);

		p2FormationLabel.GetComponent<TextMesh>().text = availableFormations[p2FormationCounter];	//loads default formation
		p2PowerBar.transform.localScale = new Vector3(TeamsManager.getTeamSettings(p2TeamCounter).x / 10,
		                                              p2PowerBar.transform.localScale.y,
		                                              p2PowerBar.transform.localScale.z);
		p2TimeBar.transform.localScale = new Vector3(TeamsManager.getTeamSettings(p2TeamCounter).y / 30,
		                                             p2TimeBar.transform.localScale.y,
		                                             p2TimeBar.transform.localScale.z);

		gameTimeLabel.GetComponent<TextMesh>().text = availableTimes[timeCounter];				//loads default game-time
	}

	
	void Update (){	
		if(canTap) {
			StartCoroutine(tapManager());
		}

		if(Input.GetKeyDown(KeyCode.Escape))
			SceneManager.LoadScene("Menu-c#");
	}

	private RaycastHit hitInfo;
	private Ray ray;
	IEnumerator tapManager (){
        
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			yield break;
			
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			
			switch(objectHit.name) {

				case "p1-TBR":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));	
					p1TeamCounter++;			
					fixCounterLengths();		
					p1Team.GetComponent<Renderer>().material.mainTexture = availableTeams[p1TeamCounter]; //set the flag on UI
					p1PowerBar.transform.localScale = new Vector3(TeamsManager.getTeamSettings(p1TeamCounter).x / 10,
					                                              p1PowerBar.transform.localScale.y,
					                                              p1PowerBar.transform.localScale.z);
				                                              	
					p1TimeBar.transform.localScale = new Vector3(TeamsManager.getTeamSettings(p1TeamCounter).y / 10,
					                                             p1TimeBar.transform.localScale.y,
					                                             p1TimeBar.transform.localScale.z);
					yield return new WaitForSeconds(0.07f);
					StartCoroutine(animateButton(p1Team));
					break;

				case "p1-TBL":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));	
					p1TeamCounter--;			
					fixCounterLengths();		
					p1Team.GetComponent<Renderer>().material.mainTexture = availableTeams[p1TeamCounter]; 
					p1PowerBar.transform.localScale = new Vector3(TeamsManager.getTeamSettings(p1TeamCounter).x / 10,
					                                              p1PowerBar.transform.localScale.y,
					                                              p1PowerBar.transform.localScale.z);
				                                              	
					p1TimeBar.transform.localScale = new Vector3(TeamsManager.getTeamSettings(p1TeamCounter).y / 10,
					                                             p1TimeBar.transform.localScale.y,
					                                             p1TimeBar.transform.localScale.z);
					yield return new WaitForSeconds(0.07f);
					StartCoroutine(animateButton(p1Team));
					break;

				case "p2-TBR":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));	
					p2TeamCounter++;			
					fixCounterLengths();		
					p2Team.GetComponent<Renderer>().material.mainTexture = availableTeams[p2TeamCounter]; //set the flag on UI
					p2PowerBar.transform.localScale = new Vector3(TeamsManager.getTeamSettings(p2TeamCounter).x / 10,
					                                              p2PowerBar.transform.localScale.y,
					                                              p2PowerBar.transform.localScale.z);
				                                              	
					p2TimeBar.transform.localScale = new Vector3(TeamsManager.getTeamSettings(p2TeamCounter).y / 10,
					                                             p2TimeBar.transform.localScale.y,
					                                             p2TimeBar.transform.localScale.z);
					yield return new WaitForSeconds(0.07f);
					StartCoroutine(animateButton(p2Team));
					break;

				case "p2-TBL":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));	
					p2TeamCounter--;			
					fixCounterLengths();		
					p2Team.GetComponent<Renderer>().material.mainTexture = availableTeams[p2TeamCounter]; //set the flag on UI
					p2PowerBar.transform.localScale = new Vector3(TeamsManager.getTeamSettings(p2TeamCounter).x / 10,
					                                              p2PowerBar.transform.localScale.y,
					                                              p2PowerBar.transform.localScale.z);
				                                              	
					p2TimeBar.transform.localScale = new Vector3(TeamsManager.getTeamSettings(p2TeamCounter).y / 10,
					                                             p2TimeBar.transform.localScale.y,
					                                             p2TimeBar.transform.localScale.z);
					yield return new WaitForSeconds(0.07f);
					StartCoroutine(animateButton(p2Team));
					break;
			
				case "p1-FBL":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));	
					p1FormationCounter--;		
					fixCounterLengths();		
					p1FormationLabel.GetComponent<TextMesh>().text = availableFormations[p1FormationCounter]; 
					break;
					
				case "p1-FBR":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					p1FormationCounter++;
					fixCounterLengths();
					p1FormationLabel.GetComponent<TextMesh>().text = availableFormations[p1FormationCounter];
					break;
					
				case "p2-FBL":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					p2FormationCounter--;
					fixCounterLengths();
					p2FormationLabel.GetComponent<TextMesh>().text = availableFormations[p2FormationCounter];
					break;
				
				case "p2-FBR":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					p2FormationCounter++;
					fixCounterLengths();
					p2FormationLabel.GetComponent<TextMesh>().text = availableFormations[p2FormationCounter];
					break;
					
				case "durationBtnLeft":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					timeCounter--;
					fixCounterLengths();
					gameTimeLabel.GetComponent<TextMesh>().text = availableTimes[timeCounter];
					break;
					
				case "durationBtnRight":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					timeCounter++;
					fixCounterLengths();
					gameTimeLabel.GetComponent<TextMesh>().text = availableTimes[timeCounter];
					break;
					
				case "Btn-Back":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(0.5f);
					//No need to save anything
					SceneManager.LoadScene("Menu-c#");
					break;
					
				case "Btn-Start":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					//Save configurations
					PlayerPrefs.SetInt("PlayerFormation", p1FormationCounter);		//save the player-1 formation index
					PlayerPrefs.SetInt("PlayerFlag", p1TeamCounter);				//save the player-1 team index

					PlayerPrefs.SetInt("Player2Formation", p2FormationCounter);		//save the player-2 formation index
					PlayerPrefs.SetInt("Player2Flag", p2TeamCounter);				//save the player-2 team index
					//Opponent uses the exact same settings as player-2, so:
					PlayerPrefs.SetInt("OpponentFormation", p2FormationCounter);	//save the Opponent formation index
					PlayerPrefs.SetInt("OpponentFlag", p2TeamCounter);				//save the Opponent team index

					PlayerPrefs.SetInt("GameTime", timeCounter);					//save the game-time value
					//** Please note that we just set the indexes here. We fetch the actual index values in the <<Game>> scene.
					
					yield return new WaitForSeconds(0.5f);

					
					if(isTournamentMode == 1)
						SceneManager.LoadScene("Tournament-c#");
					else
						SceneManager.LoadScene("Game-c#");

					break;			
			}	
		}
	}

	
	void fixCounterLengths (){
		//set array counters limitations
		
		//Player-1 formation
		if(p1FormationCounter > availableFormations.Length - 1)
			p1FormationCounter = 0;
		if(p1FormationCounter < 0)
			p1FormationCounter = availableFormations.Length - 1;

		//Player-1 team
		if(p1TeamCounter > availableTeams.Length - 1)
			p1TeamCounter = 0;
		if(p1TeamCounter < 0)
			p1TeamCounter = availableTeams.Length - 1;
			
		//Player-2 formation
		if(p2FormationCounter > availableFormations.Length - 1)
			p2FormationCounter = 0;
		if(p2FormationCounter < 0)
			p2FormationCounter = availableFormations.Length - 1;

		//Player-2 team
		if(p2TeamCounter > availableTeams.Length - 1)
			p2TeamCounter = 0;
		if(p2TeamCounter < 0)
			p2TeamCounter = availableTeams.Length - 1;
			
		//GameTime
		if(timeCounter > availableTimes.Length - 1)
			timeCounter = 0;
		if(timeCounter < 0)
			timeCounter = availableTimes.Length - 1;
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