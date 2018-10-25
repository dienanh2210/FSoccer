#pragma warning disable 0219

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerController : MonoBehaviour {

	internal int unitIndex;				//This unit's ID (given automatically by PlayerAI class)
	public GameObject selectionCircle;	//Reference to gameObject
	//Referenced GameObjects
	private GameObject helperBegin; 	//Start helper
	private GameObject helperEnd; 		//End Helper
	private GameObject arrowPlane; 		//arrow plane which is used to show shotPower
	private GameObject shootCircle;		//shoot Circle plane which is used to show shotPower

	private GameObject gameController;	//Reference to main game controller
	private float currentDistance;		//real distance of our touch/mouse position from initial drag position
	private float safeDistance; 			//A safe distance value which is always between min and max to avoid supershoots

	private float pwr;					//shoot power

	//this vector holds shooting direction
	private Vector3 shootDirectionVector;

	//prevent player to shoot twice in a round
	public static bool canShoot;
	internal float shootTime;
	private int timeAllowedToShoot = 10000; //In Seconds (in this kit we give players unlimited time to perform their turn of shooting)
	
	void Awake (){
		//Find and cache important gameObjects
		helperBegin = GameObject.FindGameObjectWithTag("mouseHelperBegin");
		helperEnd = GameObject.FindGameObjectWithTag("mouseHelperEnd");
		arrowPlane = GameObject.FindGameObjectWithTag("helperArrow");		
		gameController = GameObject.FindGameObjectWithTag("GameController");
		shootCircle = GameObject.FindGameObjectWithTag("shootCircle");
		
		//Init Variables
		pwr = 0.1f;
		currentDistance = 0;
		shootDirectionVector = new Vector3(0,0,0);
		canShoot = true;
		shootTime = timeAllowedToShoot;
		arrowPlane.GetComponent<Renderer>().enabled = false; //hide arrowPlane
		shootCircle.GetComponent<Renderer>().enabled = false; //hide shoot Circle
	}

	void Start (){
		
	}

	void Update (){
		
		//Active the selection circles around Player units when they have the turn.
		if(GlobalGameManager.playersTurn && gameObject.tag == "Player" && !GlobalGameManager.goalHappened)
			selectionCircle.GetComponent<Renderer>().enabled = true;
		else if(GlobalGameManager.opponentsTurn && gameObject.tag == "Player_2" && !GlobalGameManager.goalHappened)
			selectionCircle.GetComponent<Renderer>().enabled = true;			
		else	
			selectionCircle.GetComponent<Renderer>().enabled = false;	
	}


	void OnMouseDrag (){
		if( canShoot && ((GlobalGameManager.playersTurn && gameObject.tag == "Player") || (GlobalGameManager.opponentsTurn && gameObject.tag == "Player_2")) )
			{
				//print("Draged");
				currentDistance = Vector3.Distance(helperBegin.transform.position, transform.position);
				
				//limiters
				if(currentDistance <= GlobalGameManager.maxDistance)
					safeDistance = currentDistance;
				else
					safeDistance = GlobalGameManager.maxDistance;
					
				pwr = Mathf.Abs(safeDistance) * 12; //this is very important. change with extreme caution.				
				manageArrowTransform();										
				Vector3 dxy = helperBegin.transform.position - transform.position;
				float diff = dxy.magnitude;
				helperEnd.transform.position = transform.position + ((dxy / diff) * currentDistance * -1);
				
				helperEnd.transform.position = new Vector3( helperEnd.transform.position.x,
					                                        helperEnd.transform.position.y,
					                                        -0.5f);				
				
				//debug line from initial position to our current touch position
				Debug.DrawLine(transform.position, helperBegin.transform.position, Color.red);
				//debug line from initial position to maximum power position (mirrored)
				Debug.DrawLine(transform.position, arrowPlane.transform.position, Color.blue);
				//debug line from initial position to the exact opposite position (mirrored) of our current touch position
				Debug.DrawLine(transform.position, (2 * transform.position) - helperBegin.transform.position, Color.yellow);
				//cast ray forward and collect informations
				castRay();		
				shootDirectionVector = Vector3.Normalize(helperBegin.transform.position - transform.position);
				
			}
	}

	void sweepTest (){
		RaycastHit hit;
		if ( GetComponent<Rigidbody>().SweepTest( (helperEnd.transform.position - transform.position).normalized, out hit, 15 )) {
			print("if hit ??? : " + hit.distance + " - " + hit.transform.gameObject.name);
		}
	}
	private RaycastHit hitInfo;
	private Ray ray;
	void castRay (){
		
		//cast the ray from units position with a normalized direction out of it which is mirrored to our current drag vector.
		ray = new Ray(transform.position, (helperEnd.transform.position - transform.position).normalized );
			
		if(Physics.Raycast(ray, out hitInfo, currentDistance)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			
			//debug line whenever the ray hits something.
			Debug.DrawLine(ray.origin, hitInfo.point, Color.cyan);
					
			//draw reflected vector like a billiard game. this is the out vector which reflects from targets geometry.
			Vector3 reflectedVector = Vector3.Reflect( (hitInfo.point - ray.origin),  hitInfo.normal );
			Debug.DrawRay(hitInfo.point, reflectedVector , Color.gray, 0.2f);
			
			//draw inverted reflected vector (useful for fine-tuning the final shoot)
			Debug.DrawRay(hitInfo.transform.position, reflectedVector * -1, Color.white, 0.2f);
			
			//draw the inverted normal which is more likely to be similar to real world response.
			Debug.DrawRay(hitInfo.transform.position, hitInfo.normal * -3, Color.red, 0.2f);
			
			//Debug
			//print("Ray hits: " + objectHit.name + " At " + Time.time + " And Reflection is: " + reflectedVector);
		}
	}

	void manageArrowTransform (){
		//power arrow codes
		//hide arrowPlane
		arrowPlane.GetComponent<Renderer>().enabled = true;
		shootCircle.GetComponent<Renderer>().enabled = true;
		
		//calculate position
		if(currentDistance <= GlobalGameManager.maxDistance) {
			arrowPlane.transform.position = new Vector3(	(2 * transform.position.x) - helperBegin.transform.position.x,
			                                            	(2 * transform.position.y) - helperBegin.transform.position.y,
			                                          		-1.5f );
		} else {
			Vector3 dxy = helperBegin.transform.position - transform.position;
			float diff = dxy.magnitude;
			arrowPlane.transform.position = transform.position + ((dxy / diff) * GlobalGameManager.maxDistance * -1);
			arrowPlane.transform.position = new Vector3(arrowPlane.transform.position.x,
			                                            arrowPlane.transform.position.y,
			                                            -1.5f);
		}

		shootCircle.transform.position = transform.position + new Vector3(0, 0, 0.05f);

		//calculate rotation
		Vector3 dir = helperBegin.transform.position - transform.position;
		float outRotation; // between 0 - 360
		
		if(Vector3.Angle(dir, transform.forward) > 90) 
			outRotation = Vector3.Angle(dir, transform.right);
		else
			outRotation = Vector3.Angle(dir, transform.right) * -1;
			
		arrowPlane.transform.eulerAngles = new Vector3(0, 0, outRotation);
		//print(Vector3.Angle(dir, transform.forward));
		
		//calculate scale
		float scaleCoefX = Mathf.Log(1 + safeDistance/2, 2) * 2.2f;
		float scaleCoefY = Mathf.Log(1 + safeDistance/2, 2) * 2.2f;
		arrowPlane.transform.localScale = new Vector3(1 + scaleCoefX, 1 + scaleCoefY, 0.001f); //default scale
		shootCircle.transform.localScale = new Vector3(1 + scaleCoefX * 3, 1 + scaleCoefY * 3, 0.001f); //default scale
	}

	
	void OnMouseUp (){

		//Special checks for 2-player game
		if( (GlobalGameManager.playersTurn && gameObject.tag == "Player_2") || (GlobalGameManager.opponentsTurn && gameObject.tag == "Player") ) {
			arrowPlane.GetComponent<Renderer>().enabled = false;
			shootCircle.GetComponent<Renderer>().enabled = false;
			return;
		}
		
		//give the player a second chance to choose another ball if drag on the unit is too low
		print("currentDistance: " + currentDistance);
		if(currentDistance < 0.75f) {
			arrowPlane.GetComponent<Renderer>().enabled = false;
			shootCircle.GetComponent<Renderer>().enabled = false;
			return;
		}
		
		
		if(!canShoot)
			return;
		
		//no more shooting is possible	
		canShoot = false;
		
		
		shootTime = Time.time;
		
		//hide helper arrow object
		arrowPlane.GetComponent<Renderer>().enabled = false;
		shootCircle.GetComponent<Renderer>().enabled = false;
		
		//do the physics calculations and shoot the ball 
		Vector3 outPower = shootDirectionVector * pwr * -1;

		//add team power bonus
		//print ("OLD outPower: " + outPower.magnitude);
		outPower *= ( 1 + (TeamsManager.getTeamSettings(PlayerPrefs.GetInt("PlayerFlag")).x / 50) );
		//print ("NEW outPower: " + outPower.magnitude);

		
		if(outPower.magnitude >= 40)
			outPower *= 0.85f;
		
		//always make the player to move only in x-y plane and not on the z direction
		print("shoot power: " + outPower.magnitude);		
		GetComponent<Rigidbody>().AddForce(outPower, ForceMode.Impulse);
		
		//change the turn
		if(GlobalGameManager.gameMode == 0)
			StartCoroutine(gameController.GetComponent<GlobalGameManager>().managePostShoot("Player"));
		else if(GlobalGameManager.gameMode == 1)
			StartCoroutine(gameController.GetComponent<GlobalGameManager>().managePostShoot(gameObject.tag));
	}

}