using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BaseTransition))]
public class BaseTransitionDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		position.y += 2;
		
		EditorGUI.BeginProperty(position, label, property);
		
		SerializedProperty titleProperty = property.FindPropertyRelative("title");

		position.height = 17;

		EditorGUI.LabelField(position,titleProperty.stringValue);
		
		position.x += position.width;
		EditorGUI.EndProperty();
	}
	
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 21;
	}
}
