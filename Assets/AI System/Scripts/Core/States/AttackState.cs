using UnityEngine;
using System.Collections;

[System.Serializable]
public class AttackState : State{

	public AttackState():base(){
		properties.Add(new SerializedStringValuePair("Damage",0));
		properties.Add(new SerializedStringValuePair("Hit Chance",70.0f));
	}

	public int Damage{
		get{
			return (int)GetProperty("Damage").intValue;
		}
	}

	public float HitChance{
		get{
			return (float)GetProperty("Hit Chance").floatValue;
		}
	}
}
