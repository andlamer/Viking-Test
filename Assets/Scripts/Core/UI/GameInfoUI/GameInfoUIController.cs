using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VikingTest.Core.UI
{
    public class GameInfoUIController : MonoBehaviour, IGameInfoUIController
    {
        [SerializeField] private GameObject healthBarGameObject;
        [SerializeField] private GameObject scoreGameObject;

        public void EnableGameInfoUI()
        {
            gameObject.SetActive(true);
            healthBarGameObject.SetActive(true);
            scoreGameObject.SetActive(true);
        }

        public void DisableGameInfoUI()
        {
            healthBarGameObject.SetActive(false);
            scoreGameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}

