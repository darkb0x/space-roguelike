using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Settings : MonoBehaviour
    {
        void Start()
        {
            SetTargetFPS(60);
        }

        private void SetTargetFPS(int fps)
        {
            Application.targetFrameRate = fps;
        }
    }
}
