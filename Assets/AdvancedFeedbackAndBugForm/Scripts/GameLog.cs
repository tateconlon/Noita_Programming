using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdvancedFeedbackRatingForm
{
    public class GameLog : MonoBehaviour
    {
        [SerializeField] private string fileName = "";
        private const string Address = "/AdvancedFeedbackAndBugForm/TempLogs/LogFile.text";

        private void Awake()
        {
            fileName = Application.dataPath + Address;
        }


        private void OnEnable()
        {
            Application.logMessageReceived += ApplicationOnlogMessageReceived;
#pragma warning disable EventHandlerLeakAnalyzer
            SceneManager.sceneLoaded += OnSceneLoaded;
#pragma warning restore EventHandlerLeakAnalyzer
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= ApplicationOnlogMessageReceived;
#pragma warning disable EventHandlerLeakAnalyzer
            SceneManager.sceneLoaded += OnSceneLoaded;
#pragma warning restore EventHandlerLeakAnalyzer
        }

        [ContextMenu("Create Error Debug")]
        private void CreateTestError()
        {
            Debug.LogError("Error");
        }

        [ContextMenu("Read File")]
        public string ReadText()
        {
            string path = Application.dataPath + Address;
            string readFile;

            if (!File.Exists(path))
            {
                return "";
            }
            
            using(StreamReader reader = new StreamReader(path))
            {
                readFile = reader.ReadToEnd();
            }
            
            return readFile;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetMessage("[Date : " + DateTime.Now + " ]" + " [Scene] Loaded : " + scene.name);
        }

        private void ApplicationOnlogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (type != LogType.Error) return;
            SetMessage("[Date : " + DateTime.Now + " ]" + "[Error] Condition : " + condition + " Stack Trace : " + stacktrace);
        }

        private void SetMessage(string condition)
        {
            if (fileName == string.Empty) return;
            TextWriter textWriter = new StreamWriter(fileName, true);
            textWriter.WriteLine(condition);
            textWriter.Close();
        }

        private void OnApplicationQuit()
        {
            File.Delete(Application.dataPath + Address);
        }
    }
}