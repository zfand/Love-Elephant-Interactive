using UnityEngine;
using UnityEditor;
using System.Collections;

public class TwoAreaEditorWindow : EditorWindow {
	protected Rect leftRect= new Rect(0,0,200,1000);
	protected Rect rightRect{
		get{
			return new Rect(leftRect.x+leftRect.width,0,position.width-leftRect.width,leftRect.height);
		}
	}
	private Rect seperatorArea;
	private bool seperatorDrag;
	private string leftAreaTitle;
	private string rightAreaTitle;
	private string leftAreaStyle;
	private string rightAreaStyle;
	protected Vector2 rightScroll;
	protected Vector2 leftScroll;
	protected Vector2 scrollView = new Vector2 (10000, 10000);

	public virtual void Initialize (string leftTitle,string rightTitle, string style)
	{
		Initialize (leftTitle, rightTitle, style, style);
	}

	public virtual void Initialize (string leftTitle,string rightTitle, string leftStyle, string rightStyle)
	{
		leftAreaTitle = leftTitle;
		rightAreaTitle = rightTitle;
		leftAreaStyle = leftStyle;
		rightAreaStyle = rightStyle;
		leftRect.height = position.height;
	}

	private void OnGUI(){
		leftRect.height = position.height - leftRect.y;
		seperatorArea = new Rect (leftRect.width - 2, 0, 4, position.height);
		GUILayout.BeginArea(leftRect,leftAreaTitle,leftAreaStyle);
		leftScroll = GUILayout.BeginScrollView (leftScroll);
		OnLeftGUI ();
		GUILayout.EndScrollView ();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea ();

		GUILayout.BeginArea(rightRect,rightAreaTitle,rightAreaStyle);
		rightScroll = GUI.BeginScrollView (new Rect (0, 20, rightRect.width, this.position.height - 20), rightScroll, new Rect (0, 20, scrollView.x, scrollView.y), true, true);
		OnRightGUI ();
		GUI.EndScrollView ();
		GUILayout.FlexibleSpace ();
		GUILayout.EndArea ();
		HandleEvents ();
	}

	private void HandleEvents (){
		if (seperatorArea.Contains (Event.current.mousePosition)) {
			GUI.Label (new Rect (leftRect.width - 12, Event.current.mousePosition.y - 4, 59, 10), UnityTools.Textures.SeperatorArrows);
			Repaint ();
		}

		Event ev = Event.current;
		switch (ev.type) {
		case EventType.mouseDown:
			seperatorDrag = false;
			if (seperatorArea.Contains (ev.mousePosition)) {
				seperatorDrag = true;
			} 
			break;
		case EventType.mouseUp:
			ev.Use ();
			break;
		case EventType.mouseDrag:
			if (seperatorDrag) {
				leftRect.width += ev.delta.x;
				leftRect.width = Mathf.Clamp (leftRect.width, 100, position.width - 100);
				ev.Use ();
			}
			break;
		}
		
		wantsMouseMove=true;
		if(ev.isMouse){
			ev.Use();
		}
	}

	protected virtual void OnLeftGUI(){

	}

	protected virtual void OnRightGUI(){

	}
}
