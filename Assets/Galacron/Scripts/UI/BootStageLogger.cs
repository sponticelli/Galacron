using System;
using System.Collections.Generic;
using Galacron.Data;
using Nexus.Core.Bootstrap;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Galacron.UI
{
    public class BootStageLogger : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private BootStageConfig _config;
        
        [Header("References")]
        [SerializeField] private TMPTypingEffect _stateText;
        [SerializeField] private TMPTypingEffect _catchPhraseText;
        
        
        public UnityEvent  onBootComplete;
        
        private BootstrapStage _currentStage = BootstrapStage.NotStarted;

        private class Command
        {
            public string stateName;
            public string catchPhrase;
            public bool changeState;
        }
        
        private Queue<Command> _commands = new Queue<Command>();
        
        private bool  _bootComplete;
        
        public void Awake()
        {
            var stage = _config.GetStage(_currentStage);
            
            _commands.Enqueue(new Command
            {
                stateName = stage.StateName,
                catchPhrase = stage.CatchPhrases[Random.Range(0, stage.CatchPhrases.Length)],
                changeState = true
            });
            
            ProcessCommand();
        }
        
        public void OnBootrapComplete()
        {
            if (_commands.Count == 0)
            {
                onBootComplete?.Invoke();
            } 
            else
            {
                _bootComplete = true;
            }
        }
        
        public void OnProgress(BootstrapProgress progress)
        {
            Debug.Log($"Progress: {progress.Stage} {progress.Progress} {progress.CurrentServiceName} {progress.CurrentService}/{progress.TotalServices}");
            
            
            var stage = _config.GetStage(progress.Stage);
            if (stage == null)
            {
                Debug.LogWarning($"No stage found for {progress.Stage}");
                return;
            }
            
            if (_currentStage != progress.Stage)
            {
                _currentStage = progress.Stage;
                _commands.Enqueue(new Command
                {
                    stateName = stage.StateName,
                    catchPhrase = stage.CatchPhrases[Random.Range(0, stage.CatchPhrases.Length)],
                    changeState = true
                });
            } 
            else
            {
                _commands.Enqueue(new Command
                {
                    stateName = stage.StateName,
                    catchPhrase = stage.CatchPhrases[Random.Range(0, stage.CatchPhrases.Length)],
                    changeState = false
                });
            }
        }
        
        public void OnStateTypingComplete()
        {
            ProcessCommand();
        }
        
        public void OnCatchPhraseTypingComplete()
        {
            ProcessCommand();
        }
        
        private void ProcessCommand()
        {
            if (_commands.Count == 0)
            {
                if (_bootComplete)
                {
                    onBootComplete?.Invoke();
                }
                return;
            }
            
            var command = _commands.Dequeue();
            
            _stateText.StartTyping(command.stateName);
            _catchPhraseText.StartTyping(command.catchPhrase);
        }
    }
}