using System;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class BattleMapHoverController : MonoBehaviour
    {
        private BattleManager battleManager;
        public Map map;
        public bool state = true;
        public GameObject courser; //is this normal?

        private void Awake()
        {
        }

        private void Update()
        {
            // Can I rewrite this more efficient way?
            if (state)
            {
                int mask = (1 << 9) | (1 << 13);
                //mask = ~mask;

                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,Single.PositiveInfinity, mask);
                

                if (hit)
                {
                    courser.transform.position = map.GetCourserPosition(hitInfo.point);
                }
                
            }

            if (Input.GetMouseButtonUp(0))
            {
                int mask = 1 << 9;
                mask = ~mask;

                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,Single.PositiveInfinity, mask);
                Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction);

                Vector3 hitPoint = hitInfo.point;
                Vector3 pos;
                pos = map.GetCourserPosition(hitPoint);
                courser.transform.position = map.GetCourserPosition(hitInfo.point);
            }

            // throw new NotImplementedException();
        }
    }
}