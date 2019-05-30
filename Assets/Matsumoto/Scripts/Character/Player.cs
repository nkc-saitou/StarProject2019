using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Matsumoto.Audio;
using UnityEngine.Experimental.U2D.Animation;

namespace Matsumoto.Character {

	public enum PlayerState {
		Star,
		Morphing,
		Circle,
	}

	enum Angle : byte {
		UpperLeft = 1,
		Up = 2,
		UpperRight = 4,
		Right = 8,
		DownerRight = 16,
		Down = 32,
		DownerLeft = 64,
		Left = 128,
	}

	public class Player : MonoBehaviour, IDamageable {

		public PlayerStatus StarStatus;
		public PlayerStatus CircleStatus;
		public float MorphSpeed = 1;
		public float ChargeSpeed = 1;
		public float MaxChargePower = 500;
		public float AttackHitTime = 0.2f;
		public float AttackWaitTime = 0.3f; // AttackHitTimeより大きくないと判定がアレ
		public float JumpWaitTime = 0.2f;
		public bool DashIsAttack = true;

		public Collider2D AttackCollider;
		public DynamicBone PlayerModel;
		public ParticleSystem MoveEffect;
		public GameObject JumpEffectPrefab;
		public ParticleSystem AttackEffect;
		public GameObject HitEffectPrefab;
		public GameObject DeathEffectPrefab;

		private StageController _stageController;
		private Animator _animator;
		private Transform _eye;
		private SpriteRenderer _body;
		private SpriteRenderer _bodyWithBone;
		private Material _bodyMaterial;
		private Renderer[] _playerRenderers;

		private bool _canAttack = true;
		private bool _isGround = false;
		private PlayerStatus _currentStatus;

		private Angle _gravityDirection = Angle.Down;
		private byte _ground = 0;
		private float _morph = 0;
		private float _attackWait = 0;
		private float _lastAttackedTime = 0;
		private float _jumpWait = 0;

		public event Action<PlayerState, PlayerState> OnStateChanged;
		public event Action<bool> OnIsDashChanged;

		public float RollSpeed {
			get; private set;
		}

		public Vector2 MoveVector {
			get; private set;
		}

		public float MoveSpeed {
			get; private set;
		}

		public Rigidbody2D PlayerRig {
			get; private set;
		}

		public PlayerState State {
			get; private set;
		}

		private bool _isFreeze;
		public bool IsFreeze {
			get { return _isFreeze; }
			set {
				_isFreeze = value;
				PlayerRig.simulated = !value;

			}
		}

		private bool _isRenderer = true;
		public bool IsRenderer {
			get { return _isRenderer; }
			set {
				if(_isRenderer == value) return;
				_isRenderer = value;

				foreach(var item in _playerRenderers) {
					item.enabled = value;
				}
			}
		}

		public bool IsRotate {
			get; set;
		}

		private bool _isDash;
		public bool IsDash {
			get { return _isDash; }
			set {
				if (_isDash == value) return;
				OnIsDashChanged(_isDash = value);
			}
		}


		public bool IsAttacking {
			get; private set;
		}


		private void Awake() {

			AttackCollider.enabled = false;

			_stageController = FindObjectOfType<StageController>();
			_animator = GetComponent<Animator>();
			PlayerRig = GetComponent<Rigidbody2D>();
			_eye = transform.Find("Eye");

			_body = transform.Find("Body").GetComponent<SpriteRenderer>();
			_bodyWithBone = _body.transform.Find("StarWithBone").GetComponent<SpriteRenderer>();
			_bodyMaterial = Instantiate(_body.material);
			_body.material = _bodyMaterial;
			_bodyWithBone.material = _bodyMaterial;
			_bodyMaterial.EnableKeyword("_EMISSION");

			_playerRenderers = GetComponentsInChildren<Renderer>();

			_currentStatus = ScriptableObject.CreateInstance<PlayerStatus>();
			_currentStatus.Material = new PhysicsMaterial2D("CurrentMat");

			// イベント
			var controller = FindObjectOfType<StageController>();
			controller.OnGameStart += (c) => {
				IsFreeze = false;
			};

			controller.OnGameClear += (c) => {
				IsFreeze = true;
			};

			controller.OnGameOver += (c) => {
				IsFreeze = true;
			};

			IsFreeze = true;

			//ポーズ追加
			var pause = PauseSystem.Instance;
			pause.AddPauseList(this);
		}

		private void Start() {

			AttackEffect.Stop();

			OnStateChanged += (oldState, newState) => {

				switch(newState) {
					case PlayerState.Star:
						MoveEffect.Stop();
						break;
					case PlayerState.Morphing:
						MoveEffect.Stop();
						break;
					case PlayerState.Circle:
						MoveEffect.Play();
						break;
					default:
						break;
				}
			};

			OnIsDashChanged += e => {
				if (!DashIsAttack) return;
				Debug.Log("Toggled " + e);
				ToggleAttackState(e);
			};

			ChangeState(PlayerState.Star, true);
			Morph(0);

			// Dynamicbone
			PlayerModel.GetComponent<SpriteSkin>().enabled = true;
		}

		// Update is called once per frame
		private void Update() {

			if(IsFreeze) return;

			CheckBlockSide();
			if(_isGround) _canAttack = true;

			AttackUpdate();

			MorphUpdate(Input.GetButton("Morph"));

			if(Input.GetKeyDown(KeyCode.P)) {
				ApplyDamage(gameObject, DamageType.Enemy);
			}

			// エフェクト操作
			var main = AttackEffect.main;
			main.startRotation = new ParticleSystem.MinMaxCurve(_body.transform.eulerAngles.z);
		}

		private void MorphUpdate(bool isMorph) {

			var morph = _morph;
			morph += (isMorph ? 1 : -1) * Time.deltaTime * MorphSpeed;
			morph = Mathf.Clamp(morph, 0, 1);
			if(morph != _morph) {

				if(_morph == 0 && _jumpWait == 0) {
					// Jump
					_jumpWait = JumpWaitTime;
					PlayerRig.AddForce(ToVector(_gravityDirection) * -MaxChargePower);

					var g = Instantiate(JumpEffectPrefab, transform);
					g.transform.SetParent(null);
					Destroy(g.gameObject, 5);
				}

				_jumpWait = Mathf.Max(0, _jumpWait - Time.deltaTime);

				_morph = morph;
				Morph(_morph);

				if(morph == 0) ChangeState(PlayerState.Star);
				else if(morph == 1) ChangeState(PlayerState.Circle);
				else ChangeState(PlayerState.Morphing);
			}
		}

		private bool CheckCanAttack() {
			if(State != PlayerState.Star) return false;
			if(!_canAttack) return false;

			return _attackWait == 0;
		}

		private void FixedUpdate() {

			if (IsFreeze) return;

			var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			Move(input);
			AnimationUpdate();
		}

		private void CheckBlockSide() {

			var length = 0.55f;
			var maskStick = LayerMask.GetMask("CanStickGround");
			var maskGround = LayerMask.GetMask("CanStickGround", "Ground");

			var checkList = new[] {
			new KeyValuePair<Vector2, int>(new Vector2(-1,  1), maskStick),
			new KeyValuePair<Vector2, int>(new Vector2( 0,  1), maskStick),
			new KeyValuePair<Vector2, int>(new Vector2( 1,  1), maskStick),
			new KeyValuePair<Vector2, int>(new Vector2( 1,  0), maskStick),
			new KeyValuePair<Vector2, int>(new Vector2( 1, -1), maskGround),
			new KeyValuePair<Vector2, int>(new Vector2( 0, -1), maskGround),
			new KeyValuePair<Vector2, int>(new Vector2(-1, -1), maskGround),
			new KeyValuePair<Vector2, int>(new Vector2(-1,  0), maskStick),
		};

			_ground = 0;
			for(int i = 0;i < checkList.Length;i++) {

				var hit = Physics2D.Raycast(transform.position, checkList[i].Key, length, checkList[i].Value);
				Color c;
				if(hit.collider) {
					_ground |= (byte)Mathf.Pow(2, i);
					c = Color.green;
				}
				else {
					c = Color.red;
				}
				Debug.DrawRay(transform.position, checkList[i].Key.normalized * length, c);
			}

			if(State == PlayerState.Star) _isGround = _ground > 0;
			else _isGround = (_ground & (byte)(Angle.DownerLeft | Angle.Down | Angle.DownerRight)) > 0;

		}

		private void Move(Vector2 input) {

			var addSpeed = 0.0f;
			if(_gravityDirection == Angle.Down)
				addSpeed = input.x;
			if(_gravityDirection == Angle.Up)
				addSpeed = -input.x;
			if(_gravityDirection == Angle.Left)
				addSpeed = -input.y;
			if(_gravityDirection == Angle.Right)
				addSpeed = input.y;

			if(addSpeed != 0) RollSpeed += addSpeed * _currentStatus.MaxAddSpeed;
			else RollSpeed = Mathf.MoveTowards(RollSpeed, 0, _currentStatus.MaxSubSpeed);

			if(Mathf.Abs(RollSpeed) < _currentStatus.MaxSpeed) {
				IsDash = false;
			}

			var maxSpeed = IsDash ? _currentStatus.MaxDashSpeed : _currentStatus.MaxSpeed;

			// 最大速度を制限
			RollSpeed = Mathf.Clamp(RollSpeed, -maxSpeed, maxSpeed);

			// move and gravity
			MoveVector = new Vector2(1, 0);
			var old = _gravityDirection;
			var moveVec = MoveVector;
			_gravityDirection = State == PlayerState.Star ? CalcGravityDirectionAndMoveVec(input, _gravityDirection, out moveVec) : Angle.Down;
			MoveVector = moveVec;

			if(old != _gravityDirection) RollSpeed = SpeedConvert(RollSpeed, old, _gravityDirection);

			var vel = PlayerRig.velocity;

			// 重力と入力に分解して計算
			var g = (Vector2)Vector3.Project(vel, ToVector(_gravityDirection));
			var v = (Vector2)Vector3.Project(vel, MoveVector);

			g += ToVector(_gravityDirection) * _currentStatus.Gravity * Time.deltaTime; // gravity

			var diff = v - MoveVector * RollSpeed;
			diff = Vector2.MoveTowards(diff, new Vector2(), _currentStatus.MaxSubSpeed*2);
			v = MoveVector * RollSpeed + diff;

			vel = g + v;

			// 速度の更新
			MoveSpeed = vel.magnitude;

			// 空気抵抗
			vel = Vector2.MoveTowards(vel, new Vector2(), MoveSpeed * _currentStatus.AirResistance * Time.deltaTime);

			// 適用
			PlayerRig.velocity = vel;

			// Animation
			var scale = _eye.localScale;
			if(addSpeed > 0) scale.x = 1;
			if(addSpeed < 0) scale.x = -1;
			if(_gravityDirection == Angle.Left) scale.x = 1;
			if(_gravityDirection == Angle.Right) scale.x = -1;

			if(_gravityDirection == Angle.Up) {
				if(addSpeed != 0) scale.x *= -1;
				scale.y = -1;
			}
			else {
				scale.y = 1;
			}
			_eye.localScale = scale;
		}

		private void AnimationUpdate() {

			var speed = RollSpeed / 2;
			if(IsRotate) speed = 0;

			_animator.SetFloat("Speed", speed);
		}

		private Angle CalcGravityDirectionAndMoveVec(Vector2 input, Angle currentGravity, out Vector2 moveVec) {

			moveVec = new Vector2(1, 0);
			if(_ground == 0) return Angle.Down;

			Func<byte, int, byte> rotate = (n, count) => {
				if(count == 0) return n;

				var mul = count > 0 ? 2.0f : 0.5f;
				for(int i = 0;i < Mathf.Abs(count);i++) {
					var next = (n * mul);
					if(next > 128) next = 1;
					if(next < 1) next = 128;
					n = (byte)next;
				}

				return n;
			};

			var g = currentGravity;
			var current = (byte)g;

			// 順番に見ていく
			var isFinded = false;
			for(int i = 0;i < 3;i++) {
				current = rotate(current, 2);
				if((_ground & current) > 0) {
					if(isFinded) break;
					g = ToAngle(current);
					isFinded = true;
				}
			}

			// 複数壁があるときは入力で判断
			if(isFinded) {
				if(input.x != 0) {
					if((_ground & (byte)Angle.Down) > 0) g = Angle.Down;
					if((_ground & (byte)Angle.Up) > 0) g = Angle.Up;
				}
				if(input.y != 0) {
					if((_ground & (byte)Angle.Left) > 0) g = Angle.Left;
					if((_ground & (byte)Angle.Right) > 0) g = Angle.Right;
				}
			}

			moveVec = ToVector(ToAngle(rotate((byte)g, -2)));
			return g;
		}

		private void Morph(float ratio) {

			_currentStatus.Material.friction = Mathf.Lerp(StarStatus.Material.friction, CircleStatus.Material.friction, ratio);
			_currentStatus.Material.bounciness = Mathf.Lerp(StarStatus.Material.bounciness, CircleStatus.Material.bounciness, ratio);

			_currentStatus.BodyColor = Color.Lerp(StarStatus.BodyColor, CircleStatus.BodyColor, ratio);
			_currentStatus.MaxSpeed = Mathf.Lerp(StarStatus.MaxSpeed, CircleStatus.MaxSpeed, ratio);
			_currentStatus.MaxDashSpeed = Mathf.Lerp(StarStatus.MaxDashSpeed, CircleStatus.MaxDashSpeed, ratio);
			_currentStatus.DashPower = Mathf.Lerp(StarStatus.DashPower, CircleStatus.DashPower, ratio);
			_currentStatus.MaxAddSpeed = Mathf.Lerp(StarStatus.MaxAddSpeed, CircleStatus.MaxAddSpeed, ratio);
			_currentStatus.MaxSubSpeed = Mathf.Lerp(StarStatus.MaxSubSpeed, CircleStatus.MaxSubSpeed, ratio);
			_currentStatus.Gravity = Mathf.Lerp(StarStatus.Gravity, CircleStatus.Gravity, ratio);
			_currentStatus.AirResistance = Mathf.Lerp(StarStatus.AirResistance, CircleStatus.AirResistance, ratio);

			PlayerRig.sharedMaterial = _currentStatus.Material;

			// Material
			_bodyMaterial.SetColor("_EmissionColor", _currentStatus.BodyColor);

			// Animation
			_animator.SetFloat("Morph", ratio);

			// 描画対象選択
			if(!IsRenderer) return;
			var pstar = ratio != 0;
			_body.enabled = pstar;
			_bodyWithBone.enabled = !pstar;
		}

		private IEnumerator MorphAnimation(PlayerState toMorph, float morphSpeed) {

			if(toMorph == PlayerState.Morphing) yield break;
			if(State == toMorph) yield break;

			State = PlayerState.Morphing;

			Func<PlayerState, float, float> toVec = (state, current) => {
				switch(state) {
					case PlayerState.Star: return 1 - current;
					case PlayerState.Morphing: return 0;
					case PlayerState.Circle: return current;
					default: return 0;
				}
			};

			var t = 0.0f;

			while((t += Time.deltaTime * morphSpeed) <= 1.0f) {
				Morph(toVec(toMorph, t));
				yield return null;
			}

			Morph(toVec(toMorph, 1.0f));

			ChangeState(toMorph);
		}

		private void AttackUpdate() {

			// 情報の更新
			_attackWait -= Time.deltaTime;
			_attackWait = Mathf.Max(_attackWait, 0);

			// 攻撃をやめるか判定
			if (DashIsAttack) {

			}
			else {
				if(_lastAttackedTime + AttackHitTime > Time.time && IsAttacking)
					ToggleAttackState(false);
			}


			// 入力
			if(CheckCanAttack() && Input.GetButtonDown("Attack")) {
				DoAttack();
				_attackWait = AttackWaitTime;
				_canAttack = false;
			}

		}

		private void DoAttack() {

			var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

			if(input.x > 0) {
				RollSpeed = _currentStatus.MaxDashSpeed;
			}
			else if(input.x < 0) {
				RollSpeed = -_currentStatus.MaxDashSpeed;
			}

			if(_gravityDirection == Angle.Up) RollSpeed *= -1;

			PlayerRig.AddForce(input * _currentStatus.DashPower);
			IsDash = true;

			AudioManager.PlaySE("Dash", position:transform.position);

			// 攻撃判定
			_lastAttackedTime = Time.time;
			ToggleAttackState(true);
		}

		private void ChangeState(PlayerState state, bool isForce = false) {

			if(state == State && !isForce) return;
			OnStateChanged(State, state);
			State = state;

		}

		private void ToggleAttackState(bool enable) {
			AttackCollider.enabled = IsAttacking = enable;
			if (enable) {
				AttackEffect.Play();
			}
			else {
				AttackEffect.Stop();
			}
		}

		private float SpeedConvert(float speed, Angle from, Angle to) {
			if(from == Angle.Down && to == Angle.Up) return -speed;
			if(from == Angle.Up && to == Angle.Down) return -speed;
			return speed;
		}

		private Angle ToAngle(byte angle) {
			switch(angle) {
				case 1: return Angle.UpperLeft;
				case 2: return Angle.Up;
				case 4: return Angle.UpperRight;
				case 8: return Angle.Right;
				case 16: return Angle.DownerRight;
				case 32: return Angle.Down;
				case 64: return Angle.DownerLeft;
				case 128: return Angle.Left;
				default: return Angle.Down;
			}
		}

		private Vector2 ToVector(Angle angle) {
			switch(angle) {
				case Angle.UpperLeft: return new Vector2(-1, 1).normalized;
				case Angle.Up: return new Vector2(0, 1);
				case Angle.UpperRight: return new Vector2(1, 1).normalized;
				case Angle.Right: return new Vector2(1, 0);
				case Angle.DownerRight: return new Vector2(1, -1).normalized;
				case Angle.Down: return new Vector2(0, -1);
				case Angle.DownerLeft: return new Vector2(-1, -1).normalized;
				case Angle.Left: return new Vector2(-1, 0);
				default: return new Vector2();
			}
		}

		IEnumerator HitStop(float time) {

			// 保存
			var vel = PlayerRig.velocity;

			IsFreeze = true;
			PlayerRig.velocity = new Vector2();
			PlayerRig.isKinematic = true;
			AnimationUpdate();

			yield return new WaitForSeconds(time);

			// 復帰
			IsFreeze = false;
			PlayerRig.velocity = vel;
			PlayerRig.isKinematic = false;
			AnimationUpdate();
		}

		/// <summary>
		/// 操作をやめる
		/// </summary>
		public void Stop() {
			IsFreeze = true;
			PlayerRig.simulated = true;
			PlayerRig.velocity = new Vector2(0, -Mathf.Abs(PlayerRig.velocity.y));
			RollSpeed = 0;
			AnimationUpdate();
			StartCoroutine(MorphAnimation(PlayerState.Star, MorphSpeed));
		}

		public IEnumerator MoveTo(Vector2 relationalPositon, float speed = 1.0f, float eps = 0.5f) {

			var endPos = (Vector2)transform.position + relationalPositon;
			Debug.Log(endPos);
			var diff = endPos - (Vector2)transform.position;
			while (diff.sqrMagnitude > eps * eps) {
				var input = diff.normalized;
				Move(input);
				AnimationUpdate();
				yield return null;
				diff = endPos - (Vector2)transform.position;
			}

		}

		private void OnTriggerEnter2D(Collider2D collision) {

			if(!IsAttacking) return;

			var enemy = collision.gameObject.GetComponent<IEnemy>();
			if(enemy == null) return;

			var g = Instantiate(HitEffectPrefab, transform.position, transform.rotation);
			Destroy(g.gameObject, 5);

			enemy.ApplyDamage();

			// 敵にあたったら回復
			_canAttack = true;
			_attackWait = 0;

			AudioManager.PlaySE("AttackHit_3", position: transform.position);

			StartCoroutine(HitStop(0.1f));
		}

		private void OnDrawGizmos() {
			if(!AttackCollider.enabled) return;

			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(transform.position, 0.5f);
		}

		public void ApplyDamage(GameObject damager, DamageType type, float power = 1.0f) {

			if(type == DamageType.Enemy && IsAttacking) return;

			// 死亡演出
			IsRenderer = false;
			var p = Instantiate(DeathEffectPrefab, transform.position, transform.rotation);
			Destroy(p, 5.0f);

			AudioManager.PlaySE("Death", position: transform.position);

			_stageController.GameOver();

		}
	}
}