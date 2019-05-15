using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;

namespace StarProject2019.Saitou
{

    public class GimmickGear : MonoBehaviour
    {
        const float _rollSpeed = 2.0f;

        Animator _animator;
        Player _player;

        bool _isStopRoll = true;
        public bool IsStopRoll
        {
            get { return _isStopRoll; }
            set
            {
                _isStopRoll = value;
                _animator.SetFloat("Speed", value ? _rollSpeed : 0);
            }
        }

        // Use this for initialization
        void Start()
        {
            _animator = transform.GetChild(0).GetComponent<Animator>();
            _player = FindObjectOfType<Player>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionStay2D(Collision2D _collision)
        {
            if (_collision.gameObject != _player.gameObject) return;

            _animator.SetFloat("Speed", _player.RollSpeed != 0 ? _rollSpeed : 0);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            _animator.SetFloat("Speed", 0);
        }
    }
}
