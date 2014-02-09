using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(BaseStateAction))]
public class BaseStateActionDrawer : BaseDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		base.OnGUI (position, property, label);
		position.y += 2;
		
		EditorGUI.BeginProperty(position, label, property);
		
		SerializedProperty typeProperty =  property.FindPropertyRelative("type");
		SerializedProperty stringProperty = property.FindPropertyRelative ("stringValue");
		SerializedProperty targetProperty = property.FindPropertyRelative ("targetType");
		SerializedProperty gameObjectProperty = property.FindPropertyRelative ("gameObjectValue");
		SerializedProperty vector3Property = property.FindPropertyRelative ("vector3Value");
		SerializedProperty floatProperty = property.FindPropertyRelative ("floatValue");
		SerializedProperty float2Property = property.FindPropertyRelative ("floatValue2");
		SerializedProperty intProperty = property.FindPropertyRelative ("intValue");
		SerializedProperty boolProperty = property.FindPropertyRelative ("boolValue");
		SerializedProperty audioProperty = property.FindPropertyRelative ("audioClipValue");
		SerializedProperty setFloatTypeProperty = property.FindPropertyRelative ("setFloatType");
		SerializedProperty customActionProperty = property.FindPropertyRelative ("customActionValue");

		position.height = 17;
		string value;
		EditorGUIUtility.labelWidth=50;
		switch(typeProperty.enumValueIndex){
		case 0://SendMessage
			position.width/=3;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			List<string> sendMessageDropDown= new List<string>(targetProperty.enumNames);
			value=sendMessageDropDown[targetProperty.enumValueIndex];
			sendMessageDropDown.Remove("None");

			value=UnityEditorTools.StringPopup(position,value,sendMessageDropDown.ToArray());
			targetProperty.enumValueIndex=(int)Enum.Parse(typeof(TargetType),value);

			//EditorGUI.PropertyField(position, targetProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, stringProperty,new GUIContent(""));
			break;
		case 1://Instantiate
			position.width/=4;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, targetProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, gameObjectProperty,new GUIContent(""));

			switch(targetProperty.enumValueIndex){
			case 2://Child
				position.x += position.width;
				stringProperty.stringValue=EditorGUI.TagField(position,stringProperty.stringValue);
				break;
			default:
				position.x += position.width;
				EditorGUI.PropertyField(position, vector3Property,new GUIContent(""));
				break;
			}

			break;
		case 2://LookAt
			position.width/=targetProperty.enumValueIndex==0?3:2;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			List<string> lookAtDropDown= new List<string>(targetProperty.enumNames);
			value=lookAtDropDown[targetProperty.enumValueIndex];
			lookAtDropDown.Remove("Self");
			
			value=UnityEditorTools.StringPopup(position,value,lookAtDropDown.ToArray());
			targetProperty.enumValueIndex=(int)Enum.Parse(typeof(TargetType),value);
			//EditorGUI.PropertyField(position, targetProperty,new GUIContent(""));

			if(targetProperty.enumValueIndex==0){
				position.x += position.width;
				EditorGUI.PropertyField(position, vector3Property,new GUIContent(""));
			}

			break;
		case 3://SetTarget
			position.width/=2;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			stringProperty.stringValue=EditorGUI.TagField(position,stringProperty.stringValue);
			break;
		case 4://WaitForSecodns
			position.width/=2;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
			break;
		case 5://Destroy
			position.width/=3;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			List<string> destroyDropDown= new List<string>(targetProperty.enumNames);
			value=destroyDropDown[targetProperty.enumValueIndex];
			destroyDropDown.Remove("None");
			
			value=UnityEditorTools.StringPopup(position,value,destroyDropDown.ToArray());
			targetProperty.enumValueIndex=(int)Enum.Parse(typeof(TargetType),value);
			//EditorGUI.PropertyField(position, targetProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
			break;
		case 6://AddAttribute
			position.width/=3;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			stringProperty.stringValue=UnityEditorTools.StringPopup(position,stringProperty.stringValue,GetAIController(property).AttributeNames.ToArray());
			position.x += position.width;
			EditorGUI.PropertyField(position, intProperty,new GUIContent(""));
			break;
		case 7://ConsumeAttribute
			position.width/=3;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			stringProperty.stringValue=UnityEditorTools.StringPopup(position,stringProperty.stringValue,GetAIController(property).AttributeNames.ToArray());
			position.x += position.width;
			EditorGUI.PropertyField(position, intProperty,new GUIContent(""));
			break;
		case 8://SetFloat

			position.width/=(setFloatTypeProperty.enumValueIndex==2?5:4);
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			if(animator != null){
				stringProperty.stringValue=UnityEditorTools.StringPopup(position, stringProperty.stringValue,GetParameterNames(AnimatorControllerParameterType.Float));
			}else{
				EditorGUI.PropertyField(position, stringProperty,new GUIContent(""));
			}
			position.x += position.width;
			EditorGUI.PropertyField(position, setFloatTypeProperty,new GUIContent(""));
			position.x += position.width;

			switch(setFloatTypeProperty.enumValueIndex){
			case 0://Constant
				EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
				break;
			case 1://ForwardVelocity
				EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
				break;
			case 2://Random
				EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
				position.x += position.width;
				EditorGUI.PropertyField(position, float2Property,new GUIContent(""));
				break;
			case 3://TargetAngle
				EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
				break;
			case 4:

				break;
			case 5://AngleVelocity
				EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
				break;
			}
			break;
		case 9://SetInt
			position.width/=3;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			if(animator != null){
				stringProperty.stringValue=	UnityEditorTools.StringPopup(position,stringProperty.stringValue,GetParameterNames(AnimatorControllerParameterType.Int));
			}else{
				EditorGUI.PropertyField(position, stringProperty,new GUIContent(""));
			}
			position.x += position.width;
			EditorGUI.PropertyField(position, intProperty,new GUIContent(""));
			break;
		case 10://SetBool
			position.width/=3;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			if(animator != null){
				stringProperty.stringValue=UnityEditorTools.StringPopup(position, stringProperty.stringValue,GetParameterNames(AnimatorControllerParameterType.Bool));
			}else{
				EditorGUI.PropertyField(position, stringProperty,new GUIContent(""));
			}
			position.x += position.width;
			EditorGUI.PropertyField(position, boolProperty,new GUIContent(""));
			break;
		case 11://SetTrigger
			position.width/=2;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			if(animator != null){
				stringProperty.stringValue=UnityEditorTools.StringPopup(position, stringProperty.stringValue,GetParameterNames(AnimatorControllerParameterType.Trigger));
			}else{
				EditorGUI.PropertyField(position, stringProperty,new GUIContent(""));
			}
			break;
		case 12://PlaySound
			position.width/=3;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, audioProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
			break;
		case 13://CrossFade
			position.width/=3;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			if(animator != null){
				stringProperty.stringValue=UnityEditorTools.StringPopup(position, stringProperty.stringValue,stateNames);
			}else{
				EditorGUI.PropertyField(position, stringProperty,new GUIContent(""));
			}
			position.x += position.width;
			EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));
			break;
		case 14://SetLayerWeight
			position.width/=3;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, intProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, floatProperty,new GUIContent(""));

			break;
		case 15://Custom
			position.width/=2;
			EditorGUI.PropertyField(position, typeProperty,new GUIContent(""));
			position.x += position.width;
			EditorGUI.PropertyField(position, customActionProperty,new GUIContent(""));
			break;
		}
		EditorGUI.EndProperty();
	}

}
