using UnityEngine;

public enum WieldType
{
	MainHand, OffHand, TwoHand
}

public class WeaponItem : Item
{
	public WieldType wieldType;
}
