using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarProject2019.Saitou
{
    /// <summary>
    /// シーソーにのったオブジェクトを取得する
    /// </summary>
    public class SeesawRide : MonoBehaviour
    {
        //-------------------------------------------
        // public
        //-------------------------------------------

        //-------------------------------------------
        // Property
        //-------------------------------------------

        /// <summary>
        /// シーソーに載っているオブジェクトのリスト
        /// </summary>
        public List<GameObject> RideObject { get; private set; } = new List<GameObject>();

        //-------------------------------------------
        // function
        //-------------------------------------------

        private void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D _collision)
        {
            // シーソーにのったオブジェクトを格納していく
            RideObject.Add(_collision.gameObject);
        }

        private void OnTriggerExit2D(Collider2D _collision)
        {
            // シーソーから離れたオブジェクトを排除
            foreach (GameObject g in RideObject)
            {
                if (_collision.gameObject == g)
                {
                    RideObject.Remove(g);
                    break;
                }
            }
        }
    }
}