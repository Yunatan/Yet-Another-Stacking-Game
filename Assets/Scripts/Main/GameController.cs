using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using static Assets.Scripts.Util.GameEvents;

namespace Assets.Scripts.Main
{
    public enum GameStates
    {
        WaitingToStart,
        Playing,
        GameOver
    }

    public class GameController : IInitializable, IDisposable
    {
        private readonly Controls controls;
        private readonly SignalBus signalBus;

        public GameController(Controls controls, SignalBus signalBus)
        {
            this.controls = controls;
            this.signalBus = signalBus;
        }

        public int Score { get; set; }

        public int HighScore { get; set; }

        public int LastScore { get; set; }

        public GameStates State { get; private set; } = GameStates.WaitingToStart;

        public void Dispose()
        {
            PlayerPrefs.SetInt("highscore", HighScore);
            PlayerPrefs.SetInt("lastscore", LastScore);

            signalBus.Unsubscribe<GameOverSignal>(GameOver);
            signalBus.Unsubscribe<StackSignal>(IncreaseScore);
            signalBus.Unsubscribe<PerfectStackSignal>(IncreaseScore);
            controls.Main.Stack.performed -= StartGame;
            controls.Main.Stack.Disable();
        }

        public void Initialize()
        {
            HighScore = PlayerPrefs.GetInt("highscore", 0);
            LastScore = PlayerPrefs.GetInt("lastscore", 0);

            signalBus.Subscribe<GameOverSignal>(GameOver);
            signalBus.Subscribe<StackSignal>(IncreaseScore);
            signalBus.Subscribe<PerfectStackSignal>(IncreaseScore);
            controls.Main.Stack.performed += StartGame;
            controls.Main.Stack.Enable();
        }

        private void IncreaseScore()
        {
            Score += 1;
        }

        private void GameOver()
        {
            LastScore = Score;
            Score = 0;
            if (LastScore > HighScore)
            {
                HighScore = LastScore;
            }
            
            State = GameStates.GameOver;
        }

        private void StartGame(InputAction.CallbackContext context)
        {
            if (State == GameStates.WaitingToStart || State == GameStates.GameOver)
            {
                signalBus.Fire<GameStartSignal>();
                State = GameStates.Playing;
            }
        }
    }
}