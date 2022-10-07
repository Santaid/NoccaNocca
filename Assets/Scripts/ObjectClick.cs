using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectClick : MonoBehaviour, IPointerClickHandler{

	public void OnPointerClick(PointerEventData eventData){
		// GameMainScript.instance.clickCount++;
		// GameMainScript.instance.x = (int)this.transform.position.x;
		// GameMainScript.instance.y = (int)this.transform.position.z;
		// Debug.Log(x);
		// Debug.Log(y);
	}
}