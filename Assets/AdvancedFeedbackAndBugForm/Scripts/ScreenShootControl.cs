using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedFeedbackRatingForm
{
    public class ScreenShootControl : MonoBehaviour
    {
        public bool CanTakeScreenShoot { get; set; }
        private LogManager _logManager;

        [Header("Edit Panel")]
        [SerializeField]
        private CanvasGroup editPanelGroup;

        [SerializeField] private Drawable drawable;
        private CanvasGroup _logPanelCanvasGroup;


        public void OpenEditPanel(SSHolder ssHolder)
        {
            Debug.Log("Screenshot aciyorum göstermek icin");
            drawable.SetDrawableProp(ssHolder);
            editPanelGroup.gameObject.SetActive(true);
            editPanelGroup.GetComponent<CanvasGroup>().alpha = 1;
            editPanelGroup.interactable = true;
            editPanelGroup.blocksRaycasts = true;
        }

        public void CloseEditPanel()
        {
            editPanelGroup.gameObject.SetActive(false);
            editPanelGroup.interactable = false;
            editPanelGroup.GetComponent<CanvasGroup>().alpha = 0;
            editPanelGroup.blocksRaycasts = false;
        }

        private void Start()
        {
            _logManager = GetComponent<LogManager>();
            _logPanelCanvasGroup = GetComponent<CanvasGroup>();
        }

        public void TakeScreenShoot()
        {
            if (_logManager.ssHolderList.Count > 2) return;
            StartCoroutine(IeTakeScreenShoot());
        }

        private IEnumerator IeTakeScreenShoot()
        {
            _logPanelCanvasGroup.alpha = 0;
            yield return new WaitForSecondsRealtime(0.25f);
            CanTakeScreenShoot = true;
        }

        private void LateUpdate()
        {
            if(CanTakeScreenShoot)
            {
                StartCoroutine(RecordFrame());
            }
        }

        IEnumerator RecordFrame()
        {
            CanTakeScreenShoot = false;
            yield return new WaitForEndOfFrame();



            Texture2D screenShotTexture = ScreenCapture.CaptureScreenshotAsTexture();
            _logManager.AddScreenShoot(screenShotTexture);
            yield return new WaitForSecondsRealtime(0.25f);
            _logPanelCanvasGroup.alpha = 1;
        }
    }
}