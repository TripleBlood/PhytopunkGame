using System;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class TextScriptTest : MonoBehaviour
    {
        private Text text;
        private void Awake()
        {
            text = GetComponent<Text>();
        }

        void Update()
        {
            string str = "fuck <color=green>{0}</color>, this works!";
            text.text = String.Format(str, "me");
        }
    }
}