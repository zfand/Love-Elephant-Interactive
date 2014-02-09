using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using System;

[CustomPropertyDrawer(typeof(BaseIKAction))]
public class BaseIKActionDrawer : BaseDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		base.OnGUI (position, property, label);
		position.y += 2;
		EditorGUI.BeginProperty (position, label, property);
		SerializedProperty typeProperty =  property.FindPropertyRelative("type");
		SerializedProperty ikGoalProperty =  property.FindPropertyRelative("ikGoal");
		SerializedProperty weightProperty =  property.FindPropertyRelative("weight");
		SerializedProperty bodyWeightProperty =  property.FindPropertyRelative("bodyWeight");
		SerializedProperty headWeightProperty =  property.FindPropertyRelative("headWeight");
		SerializedProperty eyesWeightProperty =  property.FindPropertyRelative("eyesWeight");
		SerializedProperty clampWeightProperty =  property.FindPropertyRelative("clampWeight");
		SerializedProperty stringProperty =  property.FindPropertyRelative("stringValue");
		SerializedProperty setFloatTypeProperty =  property.FindPropertyRelative("setFloatType");
		SerializedProperty vector3Property =  property.FindPropertyRelative("vector3Value");


		position.height = 17;
		EditorGUIUtility.labelWidth=50;
		switch(typeProperty.enumValueIndex){
			case 0://SetIKPosition
			position.width/=4;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, ikGoalProperty,new GUIContent(""));
			position.x += position.width;
			stringProperty.stringValue=EditorGUI.TagField(position,stringProperty.stringValue);
			position.x += position.width;
			EditorGUI.PropertyField(position, vector3Property,new GUIContent(""));
			break;
		case 1://SetIKPositionWeight
			//position.width/=setFloatTypeProperty.enumValueIndex==2?5: 4;

			switch(setFloatTypeProperty.enumValueIndex){
			case 0://Constant
				position.width/=4;
				break;
			case 1://ForwardVelocity
				position.width/=3;
				break;
			case 2://Random
				position.width/=5;
				break;
			case 3://TargetAngle
				position.width/=3;
				break;
			case 4://GetFloat
				position.width/=4;
				break;
			}
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, ikGoalProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, setFloatTypeProperty,new GUIContent(""));
			position.x += position.width;

			switch(setFloatTypeProperty.enumValueIndex){
			case 0://Constant
				EditorGUI.PropertyField(position, weightProperty,new GUIContent(""));
				break;
			case 1://ForwardVelocity
				break;
			case 2://Random
				EditorGUI.PropertyField(position, weightProperty,new GUIContent(""));
				position.x += position.width;
				EditorGUI.PropertyField(position, bodyWeightProperty,new GUIContent(""));
				break;
			case 3://TargetAngle

				break;
			case 4://GetFloat
				if(animator != null){
					stringProperty.stringValue=UnityEditorTools.StringPopup(position,stringProperty.stringValue,GetParameterNames(AnimatorControllerParameterType.Float));
				}else{
					EditorGUI.PropertyField(position, stringProperty,new GUIContent(""));
				}
				break;
			}
			break;
		case 2://SetIKRotation
			position.width/=2;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, ikGoalProperty,new GUIContent(""));
			break;
		case 3://SetIKRotationWeight
			position.width/=3;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, ikGoalProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, weightProperty,new GUIContent(""));
			break;
		case 4://SetLookAtPosition
			position.width/=2;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			stringProperty.stringValue=EditorGUI.TagField(position,stringProperty.stringValue);
			break;
		case 5://SetLookAtWeight
			position.width/=6;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, weightProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, bodyWeightProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, headWeightProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, eyesWeightProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, clampWeightProperty,new GUIContent(""));
			break;
		case 6:
			position.width/=3;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, ikGoalProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, vector3Property,new GUIContent(""));
			break;
		}
		EditorGUI.EndProperty();
	}
}
