using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(BaseCondition))]
public class BaseConditionDrawer : BaseDrawer
{

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		base.OnGUI (position, property, label);
		position.y += 2;
		
		EditorGUI.BeginProperty(position, label, property);
		
		SerializedProperty typeProperty =  property.FindPropertyRelative("type");
		SerializedProperty floatProperty = property.FindPropertyRelative("floatVal");
		SerializedProperty float2Property = property.FindPropertyRelative("floatVal2");
		SerializedProperty stringProperty = property.FindPropertyRelative("stringValue");
		SerializedProperty comparerProperty = property.FindPropertyRelative ("comparerType");
		SerializedProperty boolProperty = property.FindPropertyRelative("boolValue");
		SerializedProperty layerProperty = property.FindPropertyRelative("layerValue");
		SerializedProperty targetInformationTypeProperty = property.FindPropertyRelative("targetInformationType");
		SerializedProperty customCoditionProperty = property.FindPropertyRelative("customCoditionValue");

		position.height = 17;
		float width= position.width;
		EditorGUIUtility.labelWidth=50;

		switch(typeProperty.enumValueIndex){
		case 0://ExitTime
			position.width /=2;
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
			break;
		case 1://Distance
			position.width /=3;
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, comparerProperty, new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
			break;
		case 2://ExecuteOnce
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			break;
		case 3://Attribute
			position.width /=4;
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			position.x += position.width;
			stringProperty.stringValue=UnityEditorTools.StringPopup(position,stringProperty.stringValue,GetAIController(property).AttributeNames.ToArray());
			position.x += position.width;
			EditorGUI.PropertyField(position, comparerProperty, new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
			break;
		case 4://Target

			switch(targetInformationTypeProperty.enumValueIndex){
			case 0:
				position.width /=5;
				break;
			case 1:
				position.width /=4;
				break;
			case 2:
				position.width /=3;
				break;
			case 3:
				position.width /=3;
				break;
			}
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, targetInformationTypeProperty,new GUIContent(""));
			position.x += position.width;

			switch(targetInformationTypeProperty.enumValueIndex){
			case 0://Attribute
				stringProperty.stringValue=UnityEditorTools.StringPopup(position,stringProperty.stringValue,GetAIController(property).AttributeNames.ToArray());
				position.x += position.width;
				EditorGUI.PropertyField(position, comparerProperty, new GUIContent(""));
				position.x += position.width;
				EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
				break;
			case 1://IsName
				if(animator != null){
					stringProperty.stringValue=UnityEditorTools.StringPopup(position,stringProperty.stringValue,stateNames);
				}else{
					EditorGUI.PropertyField(position, stringProperty,new GUIContent(""));
				}
				position.x += position.width;
				EditorGUI.PropertyField(position, boolProperty,new GUIContent(""));
				break;
			case 2://InTransition
				EditorGUI.PropertyField(position, boolProperty,new GUIContent(""));
				break;
			case 3://IsNull
				EditorGUI.PropertyField(position, boolProperty,new GUIContent(""));
				break;
			}
			break;
		case 5://View
			position.width /=4;
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, layerProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, boolProperty,new GUIContent(""));
			break;
		case 6://AttributeChanged
			position.width /=2;
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			position.x += position.width;
			stringProperty.stringValue= UnityEditorTools.StringPopup(position, stringProperty.stringValue,GetAIController(property).AttributeNames.ToArray());
			break;
		case 7://Formula
			position.width /=4;
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			position.x += position.width;
			stringProperty.stringValue=UnityEditorTools.StringPopup(position,stringProperty.stringValue,GetAIController(property).FormulaNames.ToArray());
			position.x += position.width;
			EditorGUI.PropertyField(position, comparerProperty, new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
			break;
		case 8://IsName
			position.width /=3;
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			position.x += position.width;
			if(animator != null){
				stringProperty.stringValue=UnityEditorTools.StringPopup(position,stringProperty.stringValue,stateNames);
			}else{
				EditorGUI.PropertyField(position, stringProperty,new GUIContent(""));
			}
			position.x += position.width;
			EditorGUI.PropertyField(position, boolProperty,new GUIContent(""));
			break;
		case 9://InTransition
			position.width /=2;
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, boolProperty,new GUIContent(""));
			break;
		case 10://ExitTimeRandom
			position.width /=3;
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, float2Property,new GUIContent(""));
			break;
		case 11://GetFloat
			position.width /=4;
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			position.x += position.width;
			if(animator != null){
				stringProperty.stringValue=UnityEditorTools.StringPopup(position,stringProperty.stringValue,GetParameterNames(AnimatorControllerParameterType.Float));
			}else{
				EditorGUI.PropertyField(position, stringProperty,new GUIContent(""));
			}
			position.x += position.width;
			EditorGUI.PropertyField(position, comparerProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
			break;
		case 12://GetBool
			position.width /=3;
			EditorGUI.PropertyField(position, typeProperty, new GUIContent(""));
			position.x += position.width;
			if(animator != null){
				stringProperty.stringValue=UnityEditorTools.StringPopup(position,stringProperty.stringValue,GetParameterNames(AnimatorControllerParameterType.Bool));
			}else{
				EditorGUI.PropertyField(position, stringProperty,new GUIContent(""));
			}
			position.x += position.width;
			EditorGUI.PropertyField(position, boolProperty,new GUIContent(""));
			break;
		case 13://Custom Condition
			position.width/=2;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, customCoditionProperty,new GUIContent(""));
			break;
		}

		position.width = width;
		EditorGUI.EndProperty();
	}
}
