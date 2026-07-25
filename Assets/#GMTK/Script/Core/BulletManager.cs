using System.Collections.Generic;
using UnityEngine;

namespace GMTK
{
    public class BulletManager : MonoBehaviour
    {
        public static BulletManager instance {get; protected set;}        

        protected List<GameObject> listAllBullets = new List<GameObject>();

        void Awake()
        {
            instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        public void AddBullet(GameObject _objBullet)
        {
            listAllBullets.Add(_objBullet);
        }

        public void RemoveBullet(GameObject _objBullet)
        {
            listAllBullets.Remove(_objBullet);
        }

        public void RemoveAllBullet()
        {
            listAllBullets.ForEach(_obj => _obj.SetActive(false));
            listAllBullets.Clear();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}