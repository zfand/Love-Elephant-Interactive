using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Node{
	private const float kNodeWidth = 150f;
	private const float kNodeHeight = 30f;
	public string id;
	public Rect position;
	public string title;
	[SerializeField]
	private string typeString;
	public Type StateType{
		get{
			return Type.GetType(typeString);
		}
	} 

	public Node(){
		id = Guid.NewGuid ().ToString();
		position=new Rect(0,0,kNodeWidth,kNodeHeight);
		typeString = this.GetType ().ToString ();
		title=System.Text.RegularExpressions.Regex.Replace(this.GetType ().ToString(), "[A-Z]", " $0");
	}
}
 