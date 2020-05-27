using System;
using Assets.Scripts.Main;
using ModestTree;
using UnityEngine;
using Zenject;
using static Assets.Scripts.Util.GameEvents;

namespace Assets.Scripts.Misc
{
    public class GuiHandler : MonoBehaviour, IDisposable, IInitializable
    {
        private SignalBus signalBus;
        private GameController gameController;
        private GUIStyle guiStyle;

        [Inject]
        public void Construct(SignalBus signalBus, GameController gameController, GUIStyle guiStyle)
        {
            this.signalBus = signalBus;
            this.gameController = gameController;
            this.guiStyle = guiStyle;
        }

        private void OnGameOver()
        {
            throw new NotImplementedException();
        }

        private void OnGameStart()
        {
            throw new NotImplementedException();
        }

        private void OnGUI()
        {
            guiStyle.fontSize = 58;
            guiStyle.normal.textColor = Color.white;
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            {
                switch (gameController.State)
                {
                    case GameStates.WaitingToStart:
                        {
                            StartGui();
                            break;
                        }
                    case GameStates.Playing:
                        {
                            PlayingGui();
                            break;
                        }
                    case GameStates.GameOver:
                        {
                            StartGui();
                            break;
                        }
                    default:
                        {
                            Assert.That(false);
                            break;
                        }
                }
            }
            GUILayout.EndArea();
        }

        private void PlayingGui()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space(30);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(30);
                    GUILayout.Label("Score: " + gameController.Score.ToString(), guiStyle);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void StartGui()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(100);
                    GUILayout.BeginVertical();
                    {
                        GUILayout.FlexibleSpace();

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("High Score: " + gameController.HighScore.ToString(), guiStyle);
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("Last Score: " + gameController.LastScore.ToString(), guiStyle);
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.FlexibleSpace();
                        GUILayout.Space(60);

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();

                            GUILayout.Label("> Tap to start <", guiStyle);

                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                }

                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        public void Initialize()
        {
            signalBus.Subscribe<GameOverSignal>(OnGameOver);
            signalBus.Subscribe<GameStartSignal>(OnGameStart);
        }

        public void Dispose()
        {
            signalBus.Unsubscribe<GameOverSignal>(OnGameOver);
            signalBus.Unsubscribe<GameStartSignal>(OnGameStart);
        }
    }
}
