using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class CoroutineTest : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.P))
            {
                StartCoroutine(Move());
            }
        }

        IEnumerator Move()
        {
            MoveUP();
            for (int i = 0; i < 30; i++)
            {
                gameObject.transform.position += Vector3.forward * 0.01f;
                yield return null;
            }

            yield return new WaitForSeconds(3);
            Debug.Log("Finished");
        }
        
        IEnumerator MoveUP()
        {
            for (int i = 0; i < 30; i++)
            {
                gameObject.transform.position += Vector3.up * 0.01f;
                yield return null;
            }

            yield return new WaitForSeconds(3);
            Debug.Log("Finished");
        }
    }
}