using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

namespace Matsumoto.Character {

	public enum FollowerState {
		Follow,
		Stay,
	}

	public class PlayerFollower : MonoBehaviour {

		public PlayerStatus StarStatus;
		public PlayerStatus CircleStatus;

		public Player Target;
		public Collider2D MainCollider;
		public float FollowStartRange = 2.0f;
		public float FlyRange = 4.0f;
		public float WalkSpeed = 5.0f;
		public float FlySpeed = 10.0f;
		public float RandomInterval = 3.0f;
		public float ChangeIntervalTime = 2.0f;		// 切り替える最低の時間
		public float MorphSpeed = 9;
		public float RandomScale = 0.4f;            // ランダムで変化する量

		public DynamicBone FollowerModel;

		private Animator _animator;
		private Rigidbody2D _rigidbody;
		private SpriteRenderer _body;
		private Transform _eye;
		private SpriteRenderer _bodyWithBone;

		private int _detectColliders;
		private float _speed = 0.0f;
		private float _changeInterval = 0.0f;
		private float _morph = 0;
		private bool _isGround = true;
		private bool _isFly = false;
		private Vector2 _nextRandomposition;

		public FollowerState State {
			get; private set;
		}

		// Use this for initialization
		void Awake() {

			_animator = GetComponent<Animator>();
			_rigidbody = GetComponent<Rigidbody2D>();
			_body = transform.Find("Body").GetComponent<SpriteRenderer>();
			_eye = transform.Find("Eye");
			_bodyWithBone = _body.transform.Find("StarWithBone").GetComponent<SpriteRenderer>();

			ChangeState(FollowerState.Follow);

			// ランダムなパラメーターを設定
			FollowStartRange += Random.Range(FollowStartRange * RandomScale, -FollowStartRange * RandomScale);
			FlyRange += Random.Range(FlyRange * RandomScale, -FlyRange * RandomScale);
			WalkSpeed += Random.Range(WalkSpeed * RandomScale, -WalkSpeed * RandomScale);
			FlySpeed += Random.Range(FlySpeed * RandomScale, -FlySpeed * RandomScale);
			RandomInterval += Random.Range(RandomInterval * RandomScale, -RandomInterval * RandomScale);
			ChangeIntervalTime += Random.Range(ChangeIntervalTime * RandomScale, -ChangeIntervalTime * RandomScale);

			PauseSystem.Instance.AddPauseList(this);
		}

		private void Start() {
			// Dynamicbone
			FollowerModel.GetComponent<SpriteSkin>().enabled = true;
		}

		// Update is called once per frame
		void Update() {

			//GroundUpdate();

			// state
			var isFollow = 
				FollowStartRange * FollowStartRange < (transform.position - Target.transform.position).sqrMagnitude;

			// めり込みそうなときは変えない
			if(!isFollow && _detectColliders > 0) {
				isFollow = true;
			}

			ChangeState(isFollow ? FollowerState.Follow : FollowerState.Stay);

			var morphVec = _isFly ? 1 : -1;
			_morph += morphVec * MorphSpeed * Time.deltaTime;
			_morph = Mathf.Clamp(_morph, 0, 1);

			UpdateAnimation();
		}

		private void FixedUpdate() {

			_changeInterval = Mathf.Max(0, _changeInterval - Time.deltaTime);

			// 移動
			switch(State) {
				case FollowerState.Follow:
					FollowMove();
					break;
				case FollowerState.Stay:
					Stay();
					break;
				default:
					break;
			}

		}

		private void GroundUpdate() {

			var length = 0.55f;
			var maskGround = LayerMask.GetMask("CanStickGround", "Ground");
			var hit = Physics2D.Raycast(transform.position, Vector2.down, length, maskGround);
			_isGround = hit.collider;
			Debug.DrawRay(transform.position, Vector2.down * length, _isGround ? Color.green : Color.red);

		}

		private void UpdateAnimation() {

			// 描画対象選択
			var pstar = _morph != 0;
			_body.enabled = pstar;
			_bodyWithBone.enabled = !pstar;

			_animator.SetFloat("Morph", _morph);
			_animator.SetFloat("Speed", _speed / 2);
		}

		private void FollowMove() {

			var targetPos = Target.transform.position;
			var diff = targetPos - transform.position;
			var vec = diff.normalized;
			var vel = _rigidbody.velocity;

			var isFly = FlyRange * FlyRange < diff.sqrMagnitude;

			// めり込んでいたら戻さない
			if(!isFly && _detectColliders > 0) {
				isFly = _isFly;
			}

			// インターバル内であれば変えない
			if (isFly != _isFly && _changeInterval > 0) {
				isFly = _isFly;

			}

			// 変更した瞬間
			if(_isFly != isFly) {

				_isFly = isFly;
				_changeInterval = ChangeIntervalTime;

				MainCollider.enabled = !isFly;

				// スピードの変換
				if (_isFly)
					_speed = Mathf.Abs(_speed);
				else
					_speed = (_speed * vec).x;
			}

			if(isFly) {

				// 空中でついてくる
				_speed += StarStatus.MaxAddSpeed;
				_speed = Mathf.Clamp(_speed, -FlySpeed, FlySpeed);

				vel = (vel.normalized + (Vector2)vec).normalized * _speed;

			}
			else {

				// 地上でついてくる
				_speed += Mathf.Sign(vec.x) * StarStatus.MaxAddSpeed;
				_speed = Mathf.Clamp(_speed, -WalkSpeed, WalkSpeed);

				vel.x = _speed;
				vel.y -= StarStatus.Gravity * Time.deltaTime;
			}

			_rigidbody.velocity = vel;

			var scale = _eye.localScale;
			if(vec.x > 0) scale.x = 1;
			if(vec.x < 0) scale.x = -1;
			_eye.localScale = scale;

		}

		private void Stay() {

			var targetPos = Target.transform.position;
			var diff = targetPos - transform.position;
			var vec = diff.normalized;
			var vel = _rigidbody.velocity;

			if(_isFly) {
				_isFly = false;
				MainCollider.enabled = true;

				// スピードの変換
				_speed = (_speed * vec).x;
			}

			_speed = Mathf.MoveTowards(_speed, 0, StarStatus.MaxSubSpeed);

			vel.x = _speed;
			vel.y -= StarStatus.Gravity * Time.deltaTime;

			_rigidbody.velocity = vel;

		}

		private void ChangeState(FollowerState state) {
			if(State == state) return;
			State = state;

			switch(state) {
				case FollowerState.Follow:

					break;
				case FollowerState.Stay:
					break;
				default:
					break;
			}
		}

		private void OnTriggerEnter2D(Collider2D collision) {
			_detectColliders++;
		}

		private void OnTriggerExit2D(Collider2D collision) {
			_detectColliders--;
		}
	}
}