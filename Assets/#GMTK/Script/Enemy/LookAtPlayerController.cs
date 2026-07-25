using UnityEngine;

namespace GMTK.Enemy
{
    using System.Collections;
    using Player;
    public class LookAtPlayerController : MonoBehaviour
    {
        protected GameObject objPlayer;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        IEnumerator Start()
        {
            PlayerLiveController _player;

            do
            {
                _player = FindAnyObjectByType<PlayerLiveController>();

                yield return null;
            }
            while (_player == null);

            objPlayer = _player.gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            LookPlayer();
        }

        protected void LookPlayer()
        {
            if (objPlayer == null) return;

            Vector2 direction = objPlayer.transform.position - transform.position;
            if (direction.sqrMagnitude < 0.0001f) return;

            float _angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, _angle);
        }
    }
}