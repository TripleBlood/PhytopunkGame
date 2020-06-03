using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UIEffectController : MonoBehaviour
    {
        private GameObject counterGameObject;
        private GameObject effectIconGameObject;
        
        private Text counter;
        private Image effectIcon;

        private void Awake()
        {
            this.effectIcon = gameObject.transform.GetComponent<Image>();
            this.counter = gameObject.transform.Find("TextPanel").Find("Counter").GetComponent<Text>();

            // Debug.Log(effectIcon.sprite.name);
            
            UpdateIcon("AbilityIcons/OverloadAbIcon");
        }

        public void UpdatCounter(string counter)
        {
            this.counter.text = counter;
        }

        public void UpdateIcon(string resourseURL)
        {
            effectIcon.sprite =
                Resources.Load(resourseURL, typeof(Sprite)) as Sprite;
        }
    }
}