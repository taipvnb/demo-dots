using System;
using Demo.Scripts.Authorings;
using Demo.Scripts.Components;
using Demo.Scripts.Systems;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Demo.Scripts
{
    public class UIController : MonoBehaviour
    {
        private void Start()
        {
            Application.targetFrameRate = 60;
        }

        public void OnSpawnButtonClicked()
        {
        }
    }
}