using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using Unity.Cloud.UserReporting;
using Unity.Cloud.UserReporting.Client;
using Unity.Cloud.UserReporting.Plugin;
using UnityEngine;
using UnityEngine.UI;

namespace AdvancedFeedbackRatingForm
{
    public class LogManager : MonoBehaviour
    {
        public UserReportingPlatformType UserReportingPlatform;
        public bool CanTakeScreenShoot;
        public GameObject Button_CaptureScreenshot;
        public GameObject Container_Screenshots;
        public GameObject Button_FeedbackFormOpener;
        public static LogManager Instance;
        [Header("Log Panel")]
        [SerializeField] private GameObject logPanelObject;
        public GameObject Button_Submit;
        [Header("Screen Shoot")] [SerializeField] private Image prefabUI;
        public List<SSHolder> ssHolderList = new List<SSHolder>();
        private CursorLockMode lastCursorCondition;
        private bool lastCursorVisibilityStatus;
        private bool isDefaultButtonActive = true;
        [SerializeField] private Transform parent;
        [Header("Submit")]
        [SerializeField] private TMP_InputField titleText;
        [SerializeField] private TMP_Dropdown typeDropdown;
        [SerializeField] private TMP_InputField descriptionText;
        public UserReport CurrentUserReport { get; private set; }

        //component
        private ScreenShootControl _screenShootControl;
        private GameLog _gameLog;
        private UnityUserReportingUpdater unityUserReportingUpdater;

        private void Awake()
        {
            Instance = this;
        }

        private UserReportingClientConfiguration GetConfiguration()
        {
            return new UserReportingClientConfiguration();
        }

        private void Start()
        {
            _screenShootControl = GetComponent<ScreenShootControl>();
            _gameLog = GetComponent<GameLog>();
            isDefaultButtonActive = (Button_FeedbackFormOpener == null ? false : (Button_FeedbackFormOpener.activeSelf));
            if (CanTakeScreenShoot)
            {
                Button_CaptureScreenshot.SetActive(true);
                Container_Screenshots.SetActive(true);
            }
            else
            {
                Button_CaptureScreenshot.SetActive(false);
                Container_Screenshots.SetActive(false);
            }


            this.unityUserReportingUpdater = new UnityUserReportingUpdater();

            // Configure Client
            bool configured = false;
            if (this.UserReportingPlatform == UserReportingPlatformType.Async)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type asyncUnityUserReportingPlatformType = assembly.GetType("Unity.Cloud.UserReporting.Plugin.Version2018_3.AsyncUnityUserReportingPlatform");
                if (asyncUnityUserReportingPlatformType != null)
                {
                    object activatedObject = Activator.CreateInstance(asyncUnityUserReportingPlatformType);
                    IUserReportingPlatform asyncUnityUserReportingPlatform = activatedObject as IUserReportingPlatform;
                    if (asyncUnityUserReportingPlatform != null)
                    {
                        UnityUserReporting.Configure(asyncUnityUserReportingPlatform, this.GetConfiguration());
                        configured = true;
                    }
                }
            }

            if (!configured)
            {
                UnityUserReporting.Configure(this.GetConfiguration());
            }

            // Ping
            string url = string.Format("https://userreporting.cloud.unity3d.com/api/userreporting/projects/{0}/ping", UnityUserReporting.CurrentClient.ProjectIdentifier);
            UnityUserReporting.CurrentClient.Platform.Post(url, "application/json", Encoding.UTF8.GetBytes("\"Ping\""), (upload, download) => { }, (result, bytes) => { });


            UnityUserReporting.CurrentClient.IsSelfReporting = false;
        }
        private void Update()
        {
            this.unityUserReportingUpdater.Reset();
            this.StartCoroutine(this.unityUserReportingUpdater);

            if (logPanelObject.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }



        public void OpenLogPanel()
        {
            lastCursorCondition = Cursor.lockState;
            lastCursorVisibilityStatus = Cursor.visible;
            Button_Submit.GetComponentInChildren<Text>().text = "Submit";
            if(Button_FeedbackFormOpener != null)
            {
                Button_FeedbackFormOpener.SetActive(false);
            }
            logPanelObject.SetActive(true);
            UnityUserReporting.CurrentClient.CreateUserReport((br) =>
            {
                this.CurrentUserReport = br;

            });
            titleText.text = "";
            descriptionText.text = "";
        }

        public void CloseLogPanel()
        {
            logPanelObject.SetActive(false);
            if (Button_FeedbackFormOpener != null)
            {
                Button_FeedbackFormOpener.SetActive(isDefaultButtonActive);
            }
            foreach (var t in ssHolderList) Destroy(t.gameObject);
            ssHolderList.Clear();
            typeDropdown.value = 0;
            titleText.text = "";
            descriptionText.text = "";
            Cursor.lockState = lastCursorCondition;
            Cursor.visible = lastCursorVisibilityStatus;
        }

        public void Submit()
        {
            bool canSendValidation = false;
            string typeText = typeDropdown.options[typeDropdown.value].text;
            string title = titleText.text;
            string description = descriptionText.text;
            string gameLogText = _gameLog.ReadText();
            byte[] screenShoot1 = ssHolderList.Count > 0 ? ssHolderList[0].texture2D.EncodeToJPG() : null;
            byte[] screenShoot2 = ssHolderList.Count > 1 ? ssHolderList[1].texture2D.EncodeToJPG() : null;
            byte[] screenShoot3 = ssHolderList.Count > 2 ? ssHolderList[2].texture2D.EncodeToJPG() : null;

            List<UserReportScreenshot> screenShotList = new List<UserReportScreenshot>();
            if (screenShoot1 != null)
            {
                UserReportScreenshot us1 = new UserReportScreenshot();
                us1.DataBase64 = Convert.ToBase64String(screenShoot1);
                screenShotList.Add(us1);
                canSendValidation = true;
            }
            if (screenShoot2 != null)
            {
                UserReportScreenshot us2 = new UserReportScreenshot();
                us2.DataBase64 = Convert.ToBase64String(screenShoot2);
                screenShotList.Add(us2);
                canSendValidation = true;
            }
            if (screenShoot3 != null)
            {
                UserReportScreenshot us3 = new UserReportScreenshot();
                us3.DataBase64 = Convert.ToBase64String(screenShoot3);
                screenShotList.Add(us3);
                canSendValidation = true;
            }

            if (screenShotList.Count > 0)
            {
                this.CurrentUserReport.Screenshots = screenShotList;
            }

            this.CurrentUserReport.Summary = typeText;
            if (!String.IsNullOrEmpty(title))
            {
                UserReportNamedValue value1 = new UserReportNamedValue("Title", title);
                this.CurrentUserReport.Fields.Add(value1);
                canSendValidation = true;
            }
            if (!String.IsNullOrEmpty(typeText))
            {
                UserReportNamedValue value2 = new UserReportNamedValue("Type", typeText);
                this.CurrentUserReport.Fields.Add(value2);
            }
            if (!String.IsNullOrEmpty(description))
            {
                UserReportNamedValue value3 = new UserReportNamedValue("Description", description);
                this.CurrentUserReport.Fields.Add(value3);
                canSendValidation = true;
            }
            if (!String.IsNullOrEmpty(gameLogText))
            {
                UserReportNamedValue value4 = new UserReportNamedValue("LogText", gameLogText);
                this.CurrentUserReport.Fields.Add(value4);
            }

            if (String.IsNullOrEmpty(title + description))
            {
                canSendValidation = false;
            }

            if (canSendValidation)
            {
                Button_Submit.GetComponentInChildren<Text>().text = "Please Wait...";
                // Send Report
                UnityUserReporting.CurrentClient.SendUserReport(this.CurrentUserReport, (uploadProgress, downloadProgress) =>
                {

                }, (success, br2) =>
                {
                    if (!success)
                    {
                        Button_Submit.GetComponentInChildren<Text>().text = "Try Again!";
                        StartCoroutine(RevertSubmitButton());
                    }
                    else
                    {
                        Button_Submit.GetComponentInChildren<Text>().text = "Submitted!";
                        this.CurrentUserReport = null;
                        StartCoroutine(ClosePanelNow());
                    }
                });

            }
        }

        IEnumerator ClosePanelNow()
        {
            yield return new WaitForSecondsRealtime(1);
            CloseLogPanel();
        }

        IEnumerator RevertSubmitButton()
        {
            yield return new WaitForSecondsRealtime(1);
            Button_Submit.GetComponentInChildren<Text>().text = "Submit";
        }


        public void AddScreenShoot(Texture2D texture2D)
        {
            Rect rec = new Rect(0, 0, texture2D.width, texture2D.height);
            Sprite sprite = Sprite.Create(texture2D, rec, new Vector2(texture2D.width * .5f, texture2D.height * .5f));
            SSHolder ssHolder = Instantiate(prefabUI, parent).GetComponent<SSHolder>();
            ssHolder.image.sprite = sprite;
            ssHolder.texture2D = texture2D;
            ssHolderList.Add(ssHolder);
            ssHolder.deleteButton.onClick.AddListener(() =>
            {
                ssHolderList.Remove(ssHolder);
                Destroy(ssHolder.gameObject);
            });
            ssHolder.imageButton.onClick.AddListener(() => _screenShootControl.OpenEditPanel(ssHolder));
        }
    }
}