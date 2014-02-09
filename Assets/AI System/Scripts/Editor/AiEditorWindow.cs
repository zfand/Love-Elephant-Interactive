using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

public class AiEditorWindow : TwoAreaGraphEditorWindow
{
	public AIController controller;
	public static AiEditorWindow editor;
	private int connectionIndex = -1;
	private int transitionIndex;
	private int stateIndex;
	private List<bool> formulaFoldout;
	private bool pathFoldOut;
	private bool dragNode;
	private bool curvyStyle;
	
	public static void Init (AIController controller)
	{
		editor = (AiEditorWindow)EditorWindow.GetWindow (typeof(AiEditorWindow));
		editor.Initialize ("Properties", "Graph", "Window", "Window");
		editor.controller = controller;
		State state = editor.controller.states.Find (x => x.StateType == typeof(AnyState));
		if (state == null) {
			AnyState anyState= new AnyState();
			anyState.position.x=editor.rightRect.width*0.5f;
			anyState.position.y=editor.position.height*0.5f;
			editor.controller.states.Add(anyState);
		}
		
		editor.formulaFoldout = new List<bool> (System.Linq.Enumerable.Repeat (false, controller.formula.Count));
		editor.curvyStyle = EditorPrefs.GetBool ("CurvyStyle", false);
	}
	
	private void OnEnable(){
		editor = (AiEditorWindow)EditorWindow.GetWindow (typeof(AiEditorWindow));
	}
	
	#region LeftGUI
	protected override void OnLeftGUI ()
	{
		base.OnLeftGUI ();
		if (controller.states.Count > 0) {
			DrawNodeProperties ();
		}
	}
	
	protected override void HandleLeftAreaEvents ()
	{
		base.HandleLeftAreaEvents ();
		Event ev = Event.current;
		switch (ev.type) {
		case EventType.mouseUp:
			dragNode = false;
			break;
		}
	}

	private ReorderableList actionList;
	private void DoActionList(SerializedObject serializedObject){
		serializedObject.Update ();
		if (actionList== null) {
			actionList = new ReorderableList (serializedObject, serializedObject.FindProperty ("states").GetArrayElementAtIndex(stateIndex).FindPropertyRelative("stateActions"), true, true, true, true);
			actionList.name="IEnumerator";
		} else {
			actionList.DoList ();
		}
		serializedObject.ApplyModifiedProperties ();
	}

	private ReorderableList actionListUpdate;
	private void DoActionListUpdate(SerializedObject serializedObject){
		serializedObject.Update ();
		if (actionListUpdate== null) {
			actionListUpdate = new ReorderableList (serializedObject, serializedObject.FindProperty ("states").GetArrayElementAtIndex(stateIndex).FindPropertyRelative("update"), true, true, true, true);
			actionListUpdate.name="Update";
		} else {
			actionListUpdate.DoList ();
		}
		serializedObject.ApplyModifiedProperties ();
	}

	private ReorderableList transitionList;
	private void DoTransitionList(SerializedObject serializedObject){
		serializedObject.Update ();
		if (transitionList== null) {
			transitionList = new ReorderableList (serializedObject, serializedObject.FindProperty ("states").GetArrayElementAtIndex(stateIndex).FindPropertyRelative("transitions"), false, true, false, true);
		} else {
			transitionList.DoList ();
		}
		serializedObject.ApplyModifiedProperties ();
	}
	
	private ReorderableList conditionList;
	private void DoConditionList(SerializedObject serializedObject){
		serializedObject.Update ();
		if (transitionList != null &&controller.states [stateIndex].transitions!= null && controller.states [stateIndex].transitions.Length > 0) {
			if (conditionList == null || (transitionList.index != transitionIndex && Event.current.type == EventType.Layout)) {
				conditionList = new ReorderableList (serializedObject, serializedObject.FindProperty ("states").GetArrayElementAtIndex (stateIndex).FindPropertyRelative ("transitions").GetArrayElementAtIndex (transitionList.index).FindPropertyRelative ("conditions"), true, true, true, true);
				transitionIndex = transitionList.index;
			} else {
				conditionList.DoList ();
			}
		}
		serializedObject.ApplyModifiedProperties ();
	}
	
	private ReorderableList ikActionList;
	private void DoIKActionList(SerializedObject serializedObject){
		serializedObject.Update ();
		if (ikActionList== null) {
			ikActionList = new ReorderableList (serializedObject, serializedObject.FindProperty ("states").GetArrayElementAtIndex(stateIndex).FindPropertyRelative("ikActions"), true, true, true, true);
			ikActionList.name="IK";
		} else {
			ikActionList.DoList ();
		}
		serializedObject.ApplyModifiedProperties ();
	}

	private void ResetReorderableLists(){
		actionList = null;
		actionListUpdate = null;
		transitionList = null;
		conditionList = null;
		ikActionList = null;
		GUI.FocusControl ("");
	}
	
	private void DoProperties(){
		EditorGUIUtility.labelWidth = 95.0f;
		if (controller.states [stateIndex].properties.Count > 0) {
			GUILayout.BeginVertical ("HelpBox");
			resetStateProperties = EditorGUILayout.Toggle ("Reset", resetStateProperties);
			if (resetStateProperties) {
				State instance=(State)System.Activator.CreateInstance(controller.states[stateIndex].StateType);
				FieldInfo[] fields= controller.states[stateIndex].GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				foreach (FieldInfo info in fields){
					if(!info.Name.Equals("properties")){
						info.SetValue(instance, info.GetValue(controller.states[stateIndex]));
					}
				}
				controller.states[stateIndex]=instance;
				resetStateProperties=false;
			}
			foreach (SerializedStringValuePair svp in controller.states[stateIndex].properties) {
				if (svp.valueType == typeof(float)) {
					svp.floatValue = EditorGUILayout.FloatField (svp.key, svp.floatValue);
				}else if(svp.valueType == typeof(string)){
					svp.stringValue=EditorGUILayout.TextField(svp.key,svp.stringValue);
				}else if(svp.valueType ==typeof(int)){
					svp.intValue=EditorGUILayout.IntField(svp.key,svp.intValue);
				}else if(svp.valueType == typeof(bool)){
					svp.boolValue=EditorGUILayout.Toggle(svp.key,svp.boolValue);
				}else if(svp.valueType == typeof(Vector3[])){
					pathFoldOut=EditorGUILayout.Foldout(pathFoldOut,"Path");
					if(pathFoldOut){
						GUILayout.BeginHorizontal();
						if(GUILayout.Button("","OL Plus",GUILayout.Width(20))){
							Camera sceneCamera = SceneView.lastActiveSceneView.camera;
							GameObject go=new GameObject("");
							go.transform.position=sceneCamera.transform.position+sceneCamera.transform.forward*5;
							
							System.Array.Resize (ref svp.vector3Array, svp.vector3Array.Length + 1);
							svp.vector3Array [ svp.vector3Array.Length - 1] = go.transform.position;
							Selection.activeTransform=go.transform;
							DestroyImmediate(go);
						}
						if(svp.vector3Array.Length>0){
							if(GUILayout.Button("","OL Minus")){
								System.Array.Resize (ref svp.vector3Array, svp.vector3Array.Length - 1);
							}
						}
						GUILayout.EndHorizontal();
						
						for(int i=0; i< svp.vector3Array.Length;i++){
							svp.vector3Array[i]=EditorGUILayout.Vector3Field("",svp.vector3Array[i]);
						} 
					}
				}
			}
			GUILayout.EndVertical ();
		}
	}
	
	
	private bool resetStateProperties;
	private void DrawNodeProperties ()
	{
		DrawGeneralInformation ();
		GUILayout.BeginVertical ("HelpBox");
		GUILayout.Label ("Description");
		GUI.skin.textField.wordWrap = true;
		controller.states [stateIndex].description = EditorGUILayout.TextArea (controller.states [stateIndex].description,GUILayout.MinHeight(50));
		GUILayout.EndVertical ();
		GUILayout.BeginVertical ("HelpBox");
		EditorGUIUtility.labelWidth = 50;
		controller.states [stateIndex].title = EditorGUILayout.TextField ("Title", controller.states [stateIndex].title);
		
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Apply Root Motion");
		GUILayout.FlexibleSpace ();
		controller.states [stateIndex].applyRootMotion = EditorGUILayout.Toggle ( controller.states [stateIndex].applyRootMotion);
		GUILayout.EndHorizontal ();
		
		SerializedObject obj = new SerializedObject (controller);
		DoActionList (obj);
		DoActionListUpdate (obj);
		if (UnityEditorInternal.InternalEditorUtility.HasPro()) {
			DoIKActionList (obj);
		}
		DoTransitionList (obj);
		DoConditionList (obj);
		GUILayout.EndVertical ();
		DoProperties ();
		
	}
	
	private void DrawGeneralInformation(){
		GUILayout.BeginVertical ("HelpBox");
		
		bool state = EditorPrefs.GetBool ("Information", false);
		if (state = EditorGUILayout.Foldout (state, "General")) {
			EditorGUILayout.HelpBox("Add an Animator Controller for popups(optional).",MessageType.Info);
			controller.runtimeAnimatorController=(RuntimeAnimatorController)EditorGUILayout.ObjectField(controller.runtimeAnimatorController,typeof(RuntimeAnimatorController),false);
			
			DrawAttributesInformation();
			DrawFormulaInformation();
		}
		EditorPrefs.SetBool ("Information", state);
		GUILayout.EndVertical ();
	}
	
	private void DrawAttributesInformation(){
		GUILayout.BeginHorizontal();
		GUILayout.Space(3);
		GUILayout.BeginVertical("box");
		bool state = EditorPrefs.GetBool ("Attributes",false);
		if(state=EditorGUILayout.Foldout(state,"Attributes")){
			GUILayout.BeginHorizontal();
			GUILayout.Space(8);
			GUILayout.BeginVertical();
			int cnt=0;
			EditorGUIUtility.labelWidth=50;
			int deleteIndex=-1;
			foreach(BaseAttribute attribute in controller.attributes){
				GUILayout.BeginHorizontal();

				bool attributeFoldOut=EditorPrefs.GetBool("Attribute"+attribute.name,false);
				attributeFoldOut=EditorGUILayout.Foldout(attributeFoldOut,attribute.name);

				GUILayout.FlexibleSpace();
				if(GUILayout.Button("","OL Minus")){
					deleteIndex=cnt;
				}
				GUILayout.EndHorizontal();
				if(attributeFoldOut){
					GUILayout.BeginHorizontal();
					GUILayout.Space(10);
					GUILayout.BeginVertical();
					attribute.name=EditorGUILayout.TextField("Name",attribute.name);
					attribute.maxValue=EditorGUILayout.CurveField("Value",attribute.maxValue);
					attribute.multiplier=EditorGUILayout.FloatField("Mult",attribute.multiplier);
					
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
				cnt++;
				EditorPrefs.SetBool("Attribute"+attribute.name,attributeFoldOut);
			}
			if(deleteIndex != -1){
				controller.attributes.RemoveAt(deleteIndex);
			}
			
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("","OL Plus")){
				if(controller.attributes==null){
					controller.attributes= new List<BaseAttribute>();
				}
				controller.attributes.Add(new BaseAttribute(1));
			}
			GUILayout.EndHorizontal();
		}
		EditorPrefs.SetBool ("Attributes", state);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	private void DrawFormulaInformation(){
		GUILayout.BeginHorizontal();
		GUILayout.Space(3);
		GUILayout.BeginVertical("box");
		bool state = EditorPrefs.GetBool ("Formula", false);
		
		if(state=EditorGUILayout.Foldout(state,"Formula")){
			if(GUILayout.Button("","OL Plus")){
				formulaFoldout.Add(true);
				if(controller.formula==null){
					controller.formula= new List<Formula>();
				}
				controller.formula.Add(new Formula());
			}
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(8);
			GUILayout.BeginVertical();
			int cnt=0;
			EditorGUIUtility.labelWidth=50;
			int deleteIndex=-1;
			foreach(Formula formula in controller.formula){
				GUILayout.BeginHorizontal();
				formulaFoldout[cnt]=EditorGUILayout.Foldout(formulaFoldout[cnt],formula.name);
				GUILayout.FlexibleSpace();
				if(GUILayout.Button("","OL Minus")){
					deleteIndex=cnt;
				}
				GUILayout.EndHorizontal();
				if(formulaFoldout[cnt]){
					GUILayout.BeginHorizontal();
					GUILayout.Space(10);
					GUILayout.BeginVertical();
					formula.name=EditorGUILayout.TextField("Name",formula.name);
					List<string> valueNames=new List<string>(controller.AttributeNames);
					valueNames.Add("Level");
					
					
					GUILayout.BeginHorizontal();
					if(formula.operations!= null && formula.operations.Count>0 ){
						for(int i=0;i< formula.operations.Count-1;i++){
							
							formula.operations[i].key=UnityEditorTools.StringPopup(formula.operations[i].key ,valueNames.ToArray());
							if(i != (formula.operations.Count-2))
								formula.operations[i].operation=(MathOperation)EditorGUILayout.EnumPopup(formula.operations[i].operation);
						}
					}
					GUILayout.FlexibleSpace();
					if(GUILayout.Button("","OL Plus")){
						if(formula.operations.Count==0){
							for(int i=0; i< 3;i++){
								formula.operations.Add(new FormulaOperation());
							}
						}else{
							formula.operations.Add(new FormulaOperation());
						}
					}
					if(GUILayout.Button("","OL Minus") && formula.operations.Count>0){
						
						formula.operations.RemoveAt(formula.operations.Count-1);
					}
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
				cnt++;
			}
			if(deleteIndex != -1){
				controller.formula.RemoveAt(deleteIndex);
			}
			
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		EditorPrefs.SetBool ("Formula", state);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	#endregion
	
	#region RightGUI
	protected override void OnRightGUI ()
	{
		base.OnRightGUI ();
		if (controller.states.Count > 0) {
			DrawConnections ();
			DrawNodes ();
		}
		HandleCreateEvents ();
	}
	
	private void DrawConnections ()
	{
		if (connectionIndex != -1) {
			DrawConnection (controller.states [connectionIndex].position.center, Event.current.mousePosition,new Rect(rightScroll.x+5,rightScroll.y+25,rightRect.width-20,rightRect.width), Color.green);
		}
		if (Event.current.type == EventType.Repaint) {
			foreach (State node in controller.states) {
				if (node.transitions != null) {
					string to=string.Empty;
					foreach (BaseTransition target in node.transitions) {
						if(target.toState != to){
							to=target.toState;
							if(curvyStyle){
								DrawCurvyConnection (controller.GetState (target.fromState).position.center, controller.GetState (target.toState).position.center,new Rect(rightScroll.x+5,rightScroll.y+25,rightRect.width-20,position.height-45), controller.states.FindIndex (x => x == node) == stateIndex && node.GetTransitionAtIndex(transitionIndex).toState == target.toState ? Color.cyan : Color.white);
							}else{
								DrawConnection (controller.GetState (target.fromState).position.center, controller.GetState (target.toState).position.center,new Rect(rightScroll.x+5,rightScroll.y+25,rightRect.width-20,position.height-45), controller.states.FindIndex (x => x == node) == stateIndex && node.GetTransitionAtIndex(transitionIndex).toState == target.toState ? Color.cyan : Color.white);
							}
						}
					}
				}
			}
		}
	}
	
	private void DrawNodes ()
	{
		foreach (State node in controller.states) {
			if (!node.Equals (controller.states [stateIndex])) {
				DrawNode (node);
			}
		}
		
		DrawNode (controller.states [stateIndex]);
		if (dragNode) {
			AutoPan (0.6f);
		}
		
	}
	
	private float debugProgress;
	private AIRuntimeController lastDebugController;
	private State lastDebugState;
	private void DrawNode (State node)
	{
		GUI.Box (node.position, node.title, UnityEditor.Graphs.Styles.GetNodeStyle ("node", node.isDefaultState ? UnityEditor.Graphs.Styles.Color.Orange : node.StateType == typeof(AnyState)? UnityEditor.Graphs.Styles.Color.Aqua: UnityEditor.Graphs.Styles.Color.Gray, node == controller.states [stateIndex]));
		DebugState (node);
		HandleNodeEvents (node);
	}
	
	private void DebugState(State node){
		lastDebugController = (Selection.activeGameObject != null && Selection.activeGameObject.AIRuntimeController () != null) ? Selection.activeGameObject.AIRuntimeController () : lastDebugController;
		if (EditorApplication.isPlaying && lastDebugController != null && node.id==lastDebugController.CurrentState.id) {
			if(lastDebugState== null || lastDebugState.id != node.id){
				debugProgress=0;
				lastDebugState=node;
				Debug.Log(lastDebugState.title);
			}
			GUI.Box(new Rect(node.position.x+5,node.position.y+20,debugProgress,5),"", "MeLivePlayBar");
		}
	}
	
	public void AutoPan (float speed)
	{
		if (Event.current.type != EventType.repaint) {
			return;
		}
		if (Event.current.mousePosition.x > rightRect.width + rightScroll.x - 50) {
			scrollView.x += (speed + 1);
			rightScroll.x += speed;
			controller.states [stateIndex].position.x += speed;
		}
		
		if ((Event.current.mousePosition.x < rightScroll.x + 50) && rightScroll.x > 0) {
			rightScroll.x -= speed;
			controller.states [stateIndex].position.x -= speed;
		}
		
		if (Event.current.mousePosition.y > position.height + rightScroll.y - 50) {
			scrollView.y += (speed + 1);
			rightScroll.y += speed;
			controller.states [stateIndex].position.y += speed;
		}
		
		if ((Event.current.mousePosition.y < rightScroll.y + 50) && rightScroll.y > 0) {
			rightScroll.y -= speed;
			controller.states [stateIndex].position.y -= speed;
		}
		Repaint ();
	}
	#endregion
	
	#region Events
	private void HandleCreateEvents ()
	{
		Event e = Event.current;
		switch (e.type) {
		case EventType.mouseDown:
			if (e.button == 1) {
				GenericMenu genericMenu = new GenericMenu ();
				IEnumerable<Type> types= AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()) .Where(type => type.IsSubclassOf(typeof(State)) && type != typeof(AnyState));
				foreach(Type type in types){
					genericMenu.AddItem (new GUIContent ("Create State/"+type.ToString()), false, new GenericMenu.MenuFunction2 (this.OnCreateStateCallback), new object[] {
						type,
						Event.current.mousePosition
					});
				}
				genericMenu.AddItem(new GUIContent("Style/Linnear"),false,new GenericMenu.MenuFunction2 (this.ChangeStyle),false);
				genericMenu.AddItem(new GUIContent("Style/Angular"),false,new GenericMenu.MenuFunction2 (this.ChangeStyle),true);
				genericMenu.ShowAsContext ();
				e.Use ();
			}
			break;
		case EventType.mouseUp:
			if (connectionIndex != -1) {
				foreach (State node in controller.states) {
					if (node.position.Contains (e.mousePosition) && node.StateType != typeof(AnyState)) {
						BaseTransition transition = new BaseTransition (controller.states [connectionIndex].id, node.id, controller);
						if (controller.states [connectionIndex].transitions == null) {
							controller.states [connectionIndex].transitions = new BaseTransition[0];
						}
						System.Array.Resize (ref controller.states [connectionIndex].transitions, controller.states [connectionIndex].transitions.Length + 1);
						controller.states [connectionIndex].transitions [controller.states [connectionIndex].transitions.Length - 1] = transition;
					}
				}
				connectionIndex = -1;
			}
			break;
		}
	}
	
	private void HandleNodeEvents (State node)
	{
		Event ev = Event.current;
		switch (ev.type) {
		case EventType.mouseDown:
			if (node.position.Contains (ev.mousePosition) && Event.current.button == 0) {
				dragNode = true;
			}
			if (node.position.Contains (ev.mousePosition) && Event.current.button == 1) {
				GenericMenu genericMenu = new GenericMenu ();
				genericMenu.AddItem (new GUIContent ("Make Transition"), false, new GenericMenu.MenuFunction2 (this.MakeTransitionCallback), node);
				if (!((State)node).isDefaultState) {
					genericMenu.AddItem (new GUIContent ("Set As Default"), false, new GenericMenu.MenuFunction2 (this.SetDefaultCallback), node);
				} else {
					genericMenu.AddDisabledItem (new GUIContent ("Set As Default"));
				}
				
				if(node.StateType == typeof(AnyState)){
					genericMenu.AddDisabledItem (new GUIContent ("Delete"));
				}else{
					genericMenu.AddItem (new GUIContent ("Delete"), false, new GenericMenu.MenuFunction2 (this.DeleteStateCallback), node);
				}
				if(node.StateType == typeof(AnyState)){
					genericMenu.AddDisabledItem (new GUIContent ("Copy"));
				}else{
					genericMenu.AddItem (new GUIContent ("Copy"), false, new GenericMenu.MenuFunction2 (this.CopyState), node);
				}
				if(copyOfState!= null && node.StateType != typeof(AnyState)){
					genericMenu.AddItem (new GUIContent ("Paste"), false, new GenericMenu.MenuFunction2 (this.PasteState), node);
				}else{
					genericMenu.AddDisabledItem (new GUIContent ("Paste"));
				}
				genericMenu.ShowAsContext ();
				ev.Use ();
			}
			break;
		case EventType.mouseUp:
			dragNode = false;
			break;
		case EventType.mouseDrag:
			if (dragNode) {
				controller.states[stateIndex].position.x += Event.current.delta.x;
				controller.states[stateIndex].position.y += Event.current.delta.y;
				
				if (controller.states[stateIndex].position.y - 30 < editor.rightRect.y) {
					controller.states[stateIndex].position.y = editor.rightRect.y + 30;
				}
				if (controller.states[stateIndex].position.x < editor.leftRect.x + 10) {
					controller.states[stateIndex].position.x = editor.leftRect.x + 10;
				}
				ev.Use ();
			}
			break;
		}
		
		if (node.position.Contains (ev.mousePosition) && (ev.type != EventType.MouseDown || ev.button != 0 ? false : ev.clickCount == 1)) {
			if (controller.states [stateIndex] != node) {
				OnStateSelectionChanged (node);
			}
		}
	}
	#endregion
	
	#region Callbacks
	private State copyOfState;
	private void CopyState(object data){
		State state = (State)data;
		
		State instance=(State)System.Activator.CreateInstance(state.StateType);
		FieldInfo[] fields= state.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		foreach (FieldInfo info in fields){
			info.SetValue(instance, info.GetValue(state));
		}
		copyOfState = instance;
	}
	
	private void PasteState(object data){
		State state = (State)data;
		
		State instance=(State)System.Activator.CreateInstance(copyOfState.StateType);
		FieldInfo[] fields= copyOfState.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		foreach (FieldInfo info in fields){
			info.SetValue(instance, info.GetValue(copyOfState));
		}
		copyOfState.position = state.position;
		copyOfState.id = state.id;
		
		copyOfState.transitions = state.transitions;
		copyOfState.isDefaultState = false;
		controller.states[stateIndex]=copyOfState;
		ResetReorderableLists ();
		
	}
	
	private void ChangeStyle(object data){
		curvyStyle = (bool)data;
	}
	
	private void OnCreateStateCallback (object data)
	{
		object[] userData = (object[])data;
		State state = (State)Activator.CreateInstance((Type)userData [0]);
		Vector2 position = (Vector2)userData [1];
		state.position.x = position.x;
		state.position.y = position.y;
		controller.states.Add (state);
		
		if (controller.states.Count == 2) {
			state.isDefaultState = true;
		}
	}
	
	private void MakeTransitionCallback (object userData)
	{
		State state = (State)userData;
		connectionIndex = controller.states.FindIndex (x => x == state);
		
	}
	
	private void SetDefaultCallback (object userData)
	{
		State state = (State)userData;
		State before = controller.states.Find (x => x.isDefaultState == true);
		if (before != null) {
			before.isDefaultState = false;
		}
		state.isDefaultState = true;
	}
	
	private void DeleteStateCallback (object userData)
	{
		State state = (State)userData;
		foreach (State node in controller.states) {
			if (node.transitions != null) {
				List<BaseTransition> removeTransitions = new List<BaseTransition> ();
				foreach (BaseTransition transition in node.transitions) {
					if (transition.toState == state.id) {
						removeTransitions.Add (transition);
					}
				}
				
				List<BaseTransition> mTransitions = new List<BaseTransition> (node.transitions);
				foreach (BaseTransition transiton in removeTransitions) {
					mTransitions.Remove (transiton);
				}
				node.transitions = mTransitions.ToArray ();
			}
		}
		controller.states.Remove (state);
		stateIndex = 0;
		transitionIndex = 0;
		ResetReorderableLists ();
	}
	
	private void OnStateSelectionChanged (State state)
	{
		stateIndex = controller.states.FindIndex (x => x == state);
		transitionIndex = 0;
		ResetReorderableLists ();
		Event.current.Use ();
	}
	
	private void OnDestroy (){
		SceneView.onSceneGUIDelegate -= OnSceneView;
		EditorPrefs.SetBool ("CurvyStyle", curvyStyle);
		EditorUtility.SetDirty (controller);
		Resources.UnloadUnusedAssets ();

	}
	#endregion
	
	#region SceneView
	private void Update(){
		if(SceneView.onSceneGUIDelegate != this.OnSceneView)
		{
			SceneView.onSceneGUIDelegate += this.OnSceneView;
		}
		
		debugProgress +=  Time.deltaTime*30; 
		if (debugProgress > 142) {
			debugProgress=0;
		}
		Repaint ();
	}
	
	private void OnSceneView(SceneView sceneview)
	{
		if (controller.states.Count>0 &&  pathFoldOut && controller.states[stateIndex].GetProperty("Path") != null) {
			List<Vector3> path=new List<Vector3>(controller.states[stateIndex].GetProperty("Path").vector3Array);
			if(path.Count<2){
				return;
			}
			
			Handles.color=Color.cyan;
			for (int index=0; index< path.Count;index++){
				
				Handles.SphereCap(0,path[index],Quaternion.identity,0.5f);
				path[index] = Handles.PositionHandle (path[index], Quaternion.identity);
				RaycastHit hit;
				if(Physics.Raycast(path[index]+ Vector3.up,Vector3.down,out hit)){
					path[index]=hit.point;
				}else{
					if(Physics.Raycast(path[index]-Vector3.up,Vector3.up,out hit)){
						path[index]=hit.point;
					}
				}
			}
			
			controller.states[stateIndex].GetProperty("Path").vector3Array=path.ToArray();
			List<Vector3> curve;
			Handles.color=Color.red;
			if (PatrolState.CatmullRom(path, out curve, 10, true)){
				for (int n = 0; n < curve.Count - 1; n++){
					Handles.DrawLine(curve[n], curve[n + 1]);
				}
			}
			editor.Repaint ();
			HandleUtility.Repaint();
		}
	}
	#endregion
}
