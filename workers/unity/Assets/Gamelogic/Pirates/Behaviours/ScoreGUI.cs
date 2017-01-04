using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    // Enable this MonoBehaviour on client workers only
    [EngineType(EnginePlatform.Client)]
    public class ScoreGUI : MonoBehaviour
    {
        /* 
         * Client will only have write-access for their own designated PlayerShip entity's ShipControls component,
         * so this MonoBehaviour will be enabled on the client's designated PlayerShip GameObject only and not on
         * the GameObject of other players' ships.
         */
        [Require] private ShipControls.Writer ShipControlsWriter;

        [Require]
        private Score.Reader ScoreReader;

        private Text totalPointsGUI;

        private void Awake()
        {
            totalPointsGUI = GameObject.Find("Canvas").GetComponentInChildren<Text>();
            GameObject.Find("Background").GetComponent<Image>().color = Color.clear;
            updateGUI(0);
        }

        private void OnEnable()
        {
            ScoreReader.ComponentUpdated += OnComponentUpdated;
        }

        private void OnDisable()
        {
            ScoreReader.ComponentUpdated -= OnComponentUpdated;
        }

        // Callback for whenever one or more property of the Score component is updated
        private void OnComponentUpdated(Score.Update update)
        {
            // Update object will have values only for fields which have been updated
            if (update.numberOfPoints.HasValue)
            {
                updateGUI(update.numberOfPoints.Value);
            }
        }

        void updateGUI(int score)
        {
            if (score > 0)
            {
                GameObject.Find("Background").GetComponent<Image>().color = Color.white;
                var text = "Score: " + score.ToString() + " ";
                totalPointsGUI.text = text;
            }
            else
            {
                GameObject.Find("Background").GetComponent<Image>().color = Color.clear;
            }
        }
    }
}