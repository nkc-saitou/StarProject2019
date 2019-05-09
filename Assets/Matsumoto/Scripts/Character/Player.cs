using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

	public class Player : MonoBehaviour {

		public PlayerStatus StarStatus;
		public PlayerStatus CircleStatus;
		public float MorphSpeed = 1;
		public float ChargeSpeed = 1;
		public float MaxChargePower = 500;
		public float AttackHitTime = 0.2f;
		public float AttackWaitTime = 0.3f; // AttackHitTimeより大きくないと判定がアレ
		public float JumpWaitTime = 0.2f;

		public Collider2D AttackCollider;
		public ParticleSystem MoveEffect;
		public ParticleSystem JumpEffect;
		public ParticleSystem AttackEffect;
		public ParticleSystem DashEffect;

		private Animator _animator;
		private Transform _eye;
		private SpriteRenderer _body;
		private SpriteRenderer _bodyWithBone;

		private bool _isDash = false;
		private bool _canDash = true;
		private bool _isGround = false;
		private PlayerStatus _currentStatus;
		private PlayerState _state;

		private float _speed = 0;
		private Angle _gravityDirection = Angle.Down;
		private byte _ground = 0;
		private float _morph = 0;
		private float _attackWait = 0;
		private float _jumpWait = 0;

		private Vector2 _moveVec;
		public Vector2 MoveVector {
			get { return _moveVec; }
		}

		private Rigidbody2D _rig;
		public Rigidbody2D Rig {
			get { return _rig; }
		}

		private bool _isFreeze;
		public bool IsFreeze {
			get { return _isFreeze; }
			set {
				_isFreeze = value;
				_rig.simulated = !value;
				_animator.SetFloat("Speed", value? _speed / 2 : 0);
			}
		}

		public event Action<PlayerState, PlayerState> OnChangeState;

		private void Awake() {

			AttackCollider.enabled = false;

			_animator = GetComponent<Animator>();
			_rig = GetComponent<Rigidbody2D>();
			_eye = transform.Find("Eye");
			_body = transform.Find("Body").GetComponent<SpriteRenderer>();
			_bodyWithBone = _body.transform.Find("StarWithBone").GetComponent<SpriteRenderer>();

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
		}

		private void Start() {

			OnChangeState += (oldState, newState) => {

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

			ChangeState(PlayerState.Star, true);
			Morph(0);

		}

		// Update is called once per frame
		private void Update() {

			if(IsFreeze) return;

			CheckBlockSide();
			if(_isGround) _canDash = true;

			if(CheckCanAttack() && Input.GetButtonDown("Attack")) {
				Attack();
				_attackWait = AttackWaitTime;
				// 地上は無限に使える
				_canDash = _isGround;
			}

			var morph = _morph;
			morph += (Input.GetButton("Morph") ? 1 : -1) * Time.deltaTime * MorphSpeed;
			morph = Mathf.Clamp(morph, 0, 1);
			if(morph != _morph) {

				if(_morph == 0 && _jumpWait == 0) {
					// Jump
					_jumpWait = JumpWaitTime;
					_rig.AddForce(ToVector(_gravityDirection) * -MaxChargePower);

					Debug.Log(_gravityDirection);
					var g = Instantiate(JumpEffect, transform);
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

			// エフェクト操作
			var main = DashEffect.main;
			main.startRotation = new ParticleSystem.MinMaxCurve(_body.transform.eulerAngles.z);
		}

		private bool CheckCanAttack() {
			if(_state != PlayerState.Star) return false;
			if(!_canDash) return false;

			_attackWait -= Time.deltaTime;
			_attackWait = Mathf.Max(_attackWait, 0);

			return _attackWait == 0;
		}

		private void FixedUpdate() {

			Move();
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

			if(_state == PlayerState.Star) _isGround = _ground > 0;
			else _isGround = (_ground & (byte)(Angle.DownerLeft | Angle.Down | Angle.DownerRight)) > 0;

		}

		private void Move() {

			if(IsFreeze) return;

			// calc speed
			var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			var addSpeed = 0.0f;
			if(_gravityDirection == Angle.Down)
				addSpeed = input.x;
			if(_gravityDirection == Angle.Up)
				addSpeed = -input.x;
			if(_gravityDirection == Angle.Left)
				addSpeed = -input.y;
			if(_gravityDirection == Angle.Right)
				addSpeed = input.y;

			if(addSpeed != 0) _speed += addSpeed * _currentStatus.MaxAddSpeed;
			else _speed = Mathf.MoveTowards(_speed, 0, _currentStatus.MaxSubSpeed);

			if(Mathf.Abs(_speed) < _currentStatus.MaxSpeed) {
				_isDash = false;
			}

			var maxSpeed = _isDash ? _currentStatus.MaxDashSpeed : _currentStatus.MaxSpeed;
			_speed = Mathf.Clamp(_speed, -maxSpeed, maxSpeed);

			// move and gravity
			_moveVec = new Vector2(1, 0);
			var old = _gravityDirection;
			_gravityDirection = _state == PlayerState.Star ? CalcGravityDirectionAndMoveVec(input, _gravityDirection, out _moveVec) : Angle.Down;
			if(old != _gravityDirection) _speed = SpeedConvert(_speed, old, _gravityDirection);

			var vel = _rig.velocity;

			// 重力と入力に分解して計算
			var g = (Vector2)Vector3.Project(vel, ToVector(_gravityDirection));
			var v = (Vector2)Vector3.Project(vel, _moveVec);

			g += ToVector(_gravityDirection) * _currentStatus.Gravity * Time.deltaTime; // gravity

			if(v.sqrMagnitude < maxSpeed * maxSpeed) {
				// move
				v = _moveVec * _speed;
			}
			else {
				var vn = v.normalized;
				v -= vn * _currentStatus.MaxSubSpeed;
			}
			vel = g + v;

			// 空気抵抗
			vel = Vector2.MoveTowards(vel, new Vector2(), vel.magnitude * _currentStatus.AirResistance * Time.deltaTime);

			// 適用
			_rig.velocity = vel;

			// Animation
			_animator.SetFloat("Speed", _speed / 2);

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

			_currentStatus.MaxSpeed = Mathf.Lerp(StarStatus.MaxSpeed, CircleStatus.MaxSpeed, ratio);
			_currentStatus.MaxDashSpeed = Mathf.Lerp(StarStatus.MaxDashSpeed, CircleStatus.MaxDashSpeed, ratio);
			_currentStatus.DashPower = Mathf.Lerp(StarStatus.DashPower, CircleStatus.DashPower, ratio);
			_currentStatus.MaxAddSpeed = Mathf.Lerp(StarStatus.MaxAddSpeed, CircleStatus.MaxAddSpeed, ratio);
			_currentStatus.MaxSubSpeed = Mathf.Lerp(StarStatus.MaxSubSpeed, CircleStatus.MaxSubSpeed, ratio);
			_currentStatus.Gravity = Mathf.Lerp(StarStatus.Gravity, CircleStatus.Gravity, ratio);
			_currentStatus.AirResistance = Mathf.Lerp(StarStatus.AirResistance, CircleStatus.AirResistance, ratio);

			_rig.sharedMaterial = _currentStatus.Material;

			// Animation
			_animator.SetFloat("Morph", ratio);

			// 描画対象選択
			var pstar = ratio != 0;
			_body.enabled = pstar;
			_bodyWithBone.enabled = !pstar;
		}

		private IEnumerator MorphAnimation(PlayerState toMorph, float morphSpeed) {

			if(toMorph == PlayerState.Morphing) yield break;
			if(_state == toMorph) yield break;

			_state = PlayerState.Morphing;

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

		private void Attack() {

			var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

			if(input.x > 0) {
				_speed = _currentStatus.MaxDashSpeed;
			}
			else if(input.x < 0) {
				_speed = -_currentStatus.MaxDashSpeed;
			}

			if(_gravityDirection == Angle.Up) _speed *= -1;

			_rig.AddForce(input * _currentStatus.DashPower);
			_isDash = true;

			// 攻撃判定
			StartCoroutine(AttackCollision());
		}

		private void ChangeState(PlayerState state, bool isForce = false) {

			if(state == _state && !isForce) return;
			OnChangeState(_state, state);
			_state = state;

		}

		private IEnumerator AttackCollision() {
			AttackCollider.enabled = true;
			DashEffect.Play();
			yield return new WaitForSeconds(AttackHitTime);
			AttackCollider.enabled = false;
			DashEffect.Stop();
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
			var vel = _rig.velocity;

			IsFreeze = true;
			_rig.velocity = new Vector2();
			_rig.isKinematic = true;
			_animator.SetFloat("Speed", 0);

			yield return new WaitForSeconds(time);

			// 復帰
			IsFreeze = false;
			_rig.velocity = vel;
			_rig.isKinematic = false;
		}

		private void OnTriggerEnter2D(Collider2D collision) {

			var enemy = collision.gameObject.GetComponent<Enemy>();
			if(!enemy) return;

			var g = Instantiate(AttackEffect, transform.position, transform.rotation);
			Destroy(g.gameObject, 5);

			enemy.Attack();

			// 敵にあたったら回復
			_canDash = true;
			_attackWait = 0;

			StartCoroutine(HitStop(0.1f));
		}

		private void OnDrawGizmos() {
			if(!AttackCollider.enabled) return;

			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(transform.position, 0.5f);
		}
	}
}