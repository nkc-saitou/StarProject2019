using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/Create Player Status", fileName = "NewPlayerStatus")]
public class PlayerStatus : ScriptableObject {

	public PhysicsMaterial2D Material;
	public float MaxSpeed;
	public float MaxDashSpeed;
	public float DashPower;
	public float MaxAddSpeed;
	public float MaxSubSpeed;
	public float Gravity;
	public float AirResistance;
}

