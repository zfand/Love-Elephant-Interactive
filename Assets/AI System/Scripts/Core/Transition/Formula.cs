using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Formula {
	public string name;
	public List<FormulaOperation> operations;

	public float GetValue(AIRuntimeController controller){
		float value = 0.0f;
		if (operations.Count > 0) {
			
			BaseAttribute firstAttribute = controller.GetAttribute (operations[0].key);
			value = firstAttribute != null ? firstAttribute.CurValue : controller.level;
			for (int cnt=0; cnt< operations.Count-2; cnt++) {

				BaseAttribute keyAttribute = controller.GetAttribute (operations[cnt+1].key);
				float secondValue = (keyAttribute != null ? keyAttribute.CurValue : controller.level);

				value=GetValue(operations[cnt].operation ,value,secondValue);
			}
		}
		return value;
	}

	public float GetValue(MathOperation operation,float firstValue, float secondValue){
		switch (operation) {
		case MathOperation.Add:

			return (firstValue+secondValue);
		case MathOperation.Substract:
			return (firstValue-secondValue);
		case MathOperation.Multiply:
			return (firstValue*secondValue);
		case MathOperation.Divide:
			return (firstValue/secondValue);
		}
		return 0;
	}
}

[System.Serializable]
public class FormulaOperation{
	public MathOperation operation;
	public string key;
}

public enum MathOperation{
	Add,
	Substract,
	Multiply,
	Divide
}