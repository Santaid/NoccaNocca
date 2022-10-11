using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_move : MonoBehaviour
{
	//public GameObject player;
	private GameObject destination;
	bool PlayerIsSelected = false;
	bool DestinationIsSelected = false;
	void Update(){
		if(Input.GetMouseButtonDown(0) && PlayerIsSelected == false){
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				
				if(hit.collider.gameObject == this.gameObject){
					//player = hit.collider.gameObject;
					PlayerIsSelected = true;
					//string objName = player.name;
					Debug.Log("player " + this.gameObject.name);
				}
			}
		}else if(Input.GetMouseButtonDown(0) && PlayerIsSelected == true && DestinationIsSelected == false){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				destination = hit.collider.gameObject;
				DestinationIsSelected = true;
				string objName = destination.name;
				print("destination " + objName);
				move();         
			}
		}
	}

	#region move
	void move(){
		Vector3 targetPosition= new Vector3(destination.transform.position.x, destination.transform.position.y  + 0.5f, destination.transform.position.z);
		transform.position = targetPosition;
		PlayerIsSelected = false;
		DestinationIsSelected = false;
	}
	#endregion

}
