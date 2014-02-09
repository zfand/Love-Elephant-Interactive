using UnityEngine;
using System.Collections;

[System.Serializable]
public class BaseAttribute {
	public string name;
	public AnimationCurve maxValue;
	public float multiplier=1.0f;
	private int level=1;
	private int curValue;
	public delegate void AttributeChangedEvent(int value);
	private event AttributeChangedEvent attributeChanged;


	public int CurValue{
		get{
			return curValue;
		}
	}

	public int MaxHealth{
		get{
			return (int)(maxValue.Evaluate(level*0.01f)*multiplier);
		}
	}

	public AttributeChangedEvent AttributeChanged{
		get{
			return attributeChanged;
		}
		set{
			attributeChanged+=value;
		}
	}

	/// <summary>
	/// Substract a value from CurValue and retruns true if less or equal zero
	/// </summary>
	public bool Consume(int val){
		curValue -= val;
		curValue = Mathf.Clamp (curValue,0, MaxHealth);
		if (attributeChanged != null) {
			attributeChanged (curValue);
		}
		return (curValue < 1);
	}

	/// <summary>
	/// Add a value and retruns true if CurValue is MaxValue
	/// </summary>
	public bool Add(int val){
		curValue += val;
		curValue = Mathf.Clamp (curValue, 0, MaxHealth);
		if (attributeChanged != null) {
			attributeChanged (curValue);
		}
		return (curValue == MaxHealth);
	}

	public BaseAttribute(int level){
		this.level = level;
	}

	public BaseAttribute(BaseAttribute other, int level){
		this.name = other.name;
		this.maxValue = other.maxValue;
		this.multiplier = other.multiplier;
		this.level = level;
		this.curValue = MaxHealth;
	}
}
