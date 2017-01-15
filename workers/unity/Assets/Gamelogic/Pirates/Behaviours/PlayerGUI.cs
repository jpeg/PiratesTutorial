using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    // Enable this MonoBehaviour on client workers only
    [EngineType(EnginePlatform.Client)]
    public class PlayerGUI : MonoBehaviour
    {
        /* 
         * Client will only have write-access for their own designated PlayerShip entity's ShipControls component,
         * so this MonoBehaviour will be enabled on the client's designated PlayerShip GameObject only and not on
         * the GameObject of other players' ships.
         */
        [Require] private ShipControls.Writer ShipControlsWriter;

        [Require]
        private Score.Reader ScoreReader;

        [Require]
        private Health.Reader HealthReader;

        private Text totalPointsGUI;
        private Text healthGUI;

        private void Awake()
        {
            totalPointsGUI = GameObject.Find("Score").GetComponent<Text>();
            if (ScoreReader != null)
            {
                GameObject.Find("ScoreBackground").GetComponent<Image>().color = Color.white;
                updateScore(ScoreReader.Data.numberOfPoints);
            } else
            {
                GameObject.Find("ScoreBackground").GetComponent<Image>().color = Color.clear;
                updateScore(0);
            }

            healthGUI = GameObject.Find("Health").GetComponent<Text>();
            GameObject.Find("HealthBackground").GetComponent<Image>().color = Color.white;
            if (HealthReader != null)
                updateHealth(HealthReader.Data.currentHealth, HealthReader.Data.maxHealth);
            else
                updateHealth(1000, 1000); //default player health
        }

        private void OnEnable()
        {
            ScoreReader.ComponentUpdated += OnScoreUpdated;
            HealthReader.ComponentUpdated += OnHealthUpdated;
            updateHealth(HealthReader.Data.currentHealth, HealthReader.Data.maxHealth);
        }

        private void OnDisable()
        {
            ScoreReader.ComponentUpdated -= OnScoreUpdated;
            HealthReader.ComponentUpdated -= OnHealthUpdated;
        }

        // Callback for whenever one or more property of the Score component is updated
        private void OnScoreUpdated(Score.Update update)
        {
            // Update object will have values only for fields which have been updated
            if (update.numberOfPoints.HasValue)
            {
                updateScore(update.numberOfPoints.Value);
            }
        }

        private void OnHealthUpdated(Health.Update update)
        {
            // Update object will have values only for fields which have been updated
            if (update.currentHealth.HasValue)
            {
                updateHealth(update.currentHealth.Value, HealthReader.Data.maxHealth);
            }
        }

        void updateScore(int score)
        {
            if (score > 0)
            {
                GameObject.Find("ScoreBackground").GetComponent<Image>().color = Color.white;
                var text = "Score: " + score.ToString() + " ";
                totalPointsGUI.text = text;
            }
            else
            {
                GameObject.Find("ScoreBackground").GetComponent<Image>().color = Color.clear;
            }
        }

        void updateHealth(int currentHealth, int maxHealth)
        {
            healthGUI.text = " Health: " + currentHealth.ToString() + "/" + maxHealth;
        }
    }
}