using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class AIController : ScriptableObject {
	public List<State> states;
	public List<BaseAttribute> attributes;
	public List<Formula> formula;
	public RuntimeAnimatorController runtimeAnimatorController;

	public State GetState(string id){
		return states.Find(state=>state.id==id);
	}

	public void Initialize(){
		List<State> copy = new List<State> (states);
		states.Clear ();
		foreach (State state in copy) {
			State instance=(State)System.Activator.CreateInstance(state.StateType);
			FieldInfo[] fields= state.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo info in fields){
		
				info.SetValue(instance, info.GetValue(state));
			}
			states.Add(instance);
		}
	}

	public List<string> AttributeNames{
		get{
			List<string> list= new List<string>();
			foreach(BaseAttribute attribute in attributes){
				list.Add(attribute.name);
			}
			return list;
		}
	}

	public List<string> FormulaNames{
		get{
			List<string> list= new List<string>();
			foreach(Formula mFormula in formula){
				list.Add(mFormula.name);
			}
			return list;
		}
	}
}
