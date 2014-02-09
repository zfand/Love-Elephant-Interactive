using UnityEngine;
using System.Collections;

[System.Serializable]
public class BaseCondition  {
	public ConditionType type;
	public float floatVal;
	public float floatVal2;
	public string stringValue;
	public bool boolValue;
	public ComparerType comparerType;
	public LayerMask layerValue;
	public TargetInformationType targetInformationType;
	public CustomCondition customCoditionValue;

	public BaseCondition(){
		type = ConditionType.ExitTime;
	}

	private float exitTime;
	private bool firstExecution=true;
	[System.NonSerialized]
	public bool executed=false;
	public virtual bool Validate(AIRuntimeController controller){

		switch (type) {
		case ConditionType.ExitTime:
			if(firstExecution){
				exitTime=Time.time+floatVal;
				firstExecution=false;
			}
			return Time.time>exitTime;
		case ConditionType.ExitTimeRandom:
			if(firstExecution){
				exitTime=Time.time+Random.Range(floatVal,floatVal2);
				firstExecution=false;
			}
			return Time.time>exitTime;
		case ConditionType.Distance:
			if(controller.target != null){
				float distance=(controller.target.position - controller.transform.position).magnitude;
				switch(comparerType){
				case ComparerType.Less:
					return distance<floatVal;
				case ComparerType.Greater:
					return distance>floatVal;
				}
			}
			return false;
		case ConditionType.ExecuteOnce:
			return executed;
		case ConditionType.Attribute:
			switch(comparerType){
			case ComparerType.Less:
				return controller.GetAttribute(stringValue).CurValue<controller.GetAttribute(stringValue).MaxHealth*floatVal*0.01f;
			case ComparerType.Greater:
				return controller.GetAttribute(stringValue).CurValue>controller.GetAttribute(stringValue).MaxHealth*floatVal*0.01f;
			default:
				return false;
			}
		case ConditionType.Target:
			switch(targetInformationType){
			case TargetInformationType.Attribute:
				if(controller.target != null){
					AIRuntimeController targetController=controller.target.AIRuntimeController();
					if(targetController != null && targetController.GetAttribute(stringValue)!= null){
						switch(comparerType){
						case ComparerType.Less:
							return targetController.GetAttribute(stringValue).CurValue<targetController.GetAttribute(stringValue).MaxHealth*floatVal*0.01f;
						case ComparerType.Greater:
							return targetController.GetAttribute(stringValue).CurValue>targetController.GetAttribute(stringValue).MaxHealth*floatVal*0.01f;
						}
					}
				}
				return false;
			case TargetInformationType.InTransition:
				if(controller.target != null){
					Animator targetAnimator= controller.target.GetComponent<Animator>();
					if(targetAnimator != null){
						return targetAnimator.IsInTransition(0) == boolValue;
					}
				}
				return false;
			case TargetInformationType.IsName:
				if(controller.target != null){
					Animator targetAnimator= controller.target.GetComponent<Animator>();
					if(targetAnimator != null){
						AnimatorStateInfo targetStateInfo=targetAnimator.GetCurrentAnimatorStateInfo(0);
						return targetStateInfo.IsName(stringValue) == boolValue;
					}
				}
				return false;
			case TargetInformationType.IsNull:

				return (controller.target == null)== boolValue;
			}
			return false;
		case ConditionType.View:
			if(controller.target == null){
				return boolValue==false;
			}
			float targetAngle = Vector3.Angle (controller.target.position - controller.transform.position, controller.transform.forward);
			if (Mathf.Abs (targetAngle) < (floatVal / 2)) {
				RaycastHit hit;
				if (Physics.Linecast (controller.transform.position + Vector3.up, controller.target.position + Vector3.up, out hit, layerValue)) {
					if (hit.transform == controller.target.transform) {  
						return boolValue==true;
					}
				}
			}
			return boolValue==false;
		case ConditionType.AttributeChanged:
			BaseAttribute attribute=controller.GetAttribute(stringValue);

			if( attribute.AttributeChanged != AttributeChangedCallback){
				attribute.AttributeChanged=AttributeChangedCallback;
			}
			if(attributeChanged && Time.time>exitTime){
				attributeChanged=false;
			}

			return attributeChanged;		
		case ConditionType.Formula:
			Debug.Log(controller.GetFormula(stringValue).GetValue(controller));
			switch(comparerType){
			case ComparerType.Less:
				return controller.GetFormula(stringValue).GetValue(controller)<floatVal;
			case ComparerType.Greater:
				return controller.GetFormula(stringValue).GetValue(controller)>floatVal;
			default:
				return false;
			}
		case ConditionType.IsName:
			AnimatorStateInfo stateInfo=controller.animator.GetCurrentAnimatorStateInfo(0);
			return stateInfo.IsName(stringValue) == boolValue;
		case ConditionType.InTransition:
			return controller.animator.IsInTransition(0)== boolValue;
		case ConditionType.GetFloat:
			switch(comparerType){
			case ComparerType.Less:
				return controller.animator.GetFloat(stringValue)<floatVal;
			case ComparerType.Greater:
				return controller.animator.GetFloat(stringValue)>floatVal;
			default:
				return false;
			}
		case ConditionType.GetBool:
			return controller.animator.GetBool(stringValue)==boolValue;
		case ConditionType.Custom:
			return customCoditionValue.Validate(controller);
		default:
			return false;
		}
	}

	[System.NonSerialized]
	public bool attributeChanged;
	private void AttributeChangedCallback(int val){
		attributeChanged = true;
		if (type == ConditionType.AttributeChanged) {
			exitTime = Time.time + 0.5f;
		}
	}

	public virtual void Reset(){
		firstExecution = true;
		if (type != ConditionType.AttributeChanged) {
			exitTime = 0.0f;
		}
		executed = false;
		attributeChanged = false;
	}
}

public enum ConditionType{
	ExitTime,
	Distance,
	ExecuteOnce,
	Attribute,
	Target,
	View,
	AttributeChanged,
	Formula,
	IsName,
	InTransition,
	ExitTimeRandom,
	GetFloat,
	GetBool,
	Custom
}

public enum ComparerType{
	Greater,
	Less
}

public enum TargetInformationType{
	Attribute,
	IsName,
	InTransition,
	IsNull
}
