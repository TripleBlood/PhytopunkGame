using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Utils
{
    public class UIinSpace : MonoBehaviour
    {
        public Vector3 pivotPoint;
        public Vector2 offset = new Vector2(0,0);
        
        public bool initiated;

        private Image _renderer;
        private Text text;

        private bool floatUp;

        private int mask = ~((1 << 5) | (1 << 14) | (1 << 9)) ;


        private void Awake()
        {
            _renderer = GetComponent<Image>();
            try
            {
                text = gameObject.GetComponentInChildren<Text>();
                //Debug.Log(text.text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Update()
        {
            if (initiated)
            {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(pivotPoint, Camera.main.gameObject.transform.position - pivotPoint, out hitInfo, 
                    (Camera.main.gameObject.transform.position - pivotPoint).magnitude, mask);
                if (!hit)
                {
                    _renderer.enabled = true;
                    gameObject.transform.position =  (Vector2)Camera.main.WorldToScreenPoint(pivotPoint) + offset;
                    try
                    {
                        text.enabled = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    _renderer.enabled = false;
                    //Debug.Log(hitInfo.collider.gameObject.name);
                    try
                    {
                        text.enabled = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                if (floatUp)
                {
                    pivotPoint += new Vector3(0, 0.006f, 0);
                    Color c = text.color;
                    c.a -= 0.005f;
                    text.color = c;

                    if (c.a <= 0)
                    {
                        Destroy(this);
                    }
                }
            }

            if (Input.GetKey(KeyCode.P))
            {
                Debug.DrawLine(pivotPoint, Camera.main.gameObject.transform.position, Color.blue, 3.5f);
            }
        }

        public void ChangeText(string text)
        {
            try
            {
                this.text.text = text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Initiate(Vector3 pivotPoint, Vector2 offset)
        {
            this.pivotPoint = pivotPoint;
            this.offset = offset;
            
            initiated = true;
        }
        
        public void Initiate(Vector3 pivotPoint)
        {
            this.pivotPoint = pivotPoint;
            
            initiated = true;
        }
        
        public void Initiate(Vector3 pivotPoint, Vector2 offset, bool floatUp, Color color)
        {
            this.pivotPoint = pivotPoint;
            this.offset = offset;
            this.floatUp = floatUp;

            text.color = color;
            
            initiated = true;
        }
        
        public void Initiate(Vector3 pivotPoint, bool floatUp, Color color)
        {
            this.pivotPoint = pivotPoint;
            this.offset = offset;
            this.floatUp = floatUp;

            text.color = color;
            
            initiated = true;
        }
    }
}