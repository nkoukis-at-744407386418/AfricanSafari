using System.Collections;

using Unity.RemoteConfig;

using UnityEngine;

using UnityEngine.UI;



namespace VrGamesDev
{
    /// <summary>
    /// This class retrieve the cloud server Remote config data
    /// and display a full screen pop up announcement to the user
    /// Just drop the prefab in the scene you want to display the announcement
    /// </summary>
    public class VRG_Announcement : VRG_Base
    {
        /// <summary>
        /// From VRG_Remote: The date, this is when the anncouncement was made
        /// </summary>
        [Header("From: Settings")]
        [Tooltip("From VRG_Remote: The date, this is when the anncouncement was made")]
        [SerializeField] private string m_Settings_Date = "VRG_Announcement.date";

        /// <summary>
        /// From VRG_Remote: The title of the current anncouncement
        /// </summary>
        [Tooltip("From VRG_Remote: The title of the current anncouncement")]
        [SerializeField] private string m_Settings_Title = "VRG_Announcement.title";

        /// <summary>
        /// From VRG_Remote: The body of the message
        /// </summary>
        [Tooltip("From VRG_Remote: The body of the message")]
        [SerializeField] private string m_Settings_Body = "VRG_Announcement.body";

        /// <summary>
        /// From VRG_Remote: How many times will show to the user, 0 for infinite
        /// </summary>
        [Tooltip("From VRG_Remote: How many times will show to the user, 0 for infinite")]
        [SerializeField] private string m_Settings_Repeat = "VRG_Announcement.repeat";

        /// <summary>
        /// From UI: The date text container
        /// </summary>
        [Header("From: UI")]
        [Tooltip("From UI: The date text container")]
        [SerializeField] private Text m_UI_Date = null;

        /// <summary>
        /// From UI: The Title text container
        /// </summary>
        [Tooltip("From UI: The Title text container")]
        [SerializeField] private Text m_UI_Title = null;

        /// <summary>
        /// From UI: The Body text container
        /// </summary>
        [Tooltip("From UI: The Body text container")]
        [SerializeField] private Text m_UI_Body = null;

        private void Awake()
        {
            this.UI_Activate(false);
        }

        private void OnEnable()
        {
            // play the waits and delays
            StartCoroutine(OnEnable_IEnumerator());
        }

        // when activated Do your thing, load the scene
        private IEnumerator OnEnable_IEnumerator()
        {
            // Add a listener to apply settings when successfully retrieved: 
            ConfigManager.FetchCompleted += ApplyRemoteSettings;

            // Fetch configuration setting from the remote service: 
            ConfigManager.FetchConfigs<VRG_Remote_UserAttributes, VRG_Remote_AppAttributes>(new VRG_Remote_UserAttributes(), new VRG_Remote_AppAttributes());

            WAS_echo("Inside: OnEnable_IEnumerator()"); // logs to editor 
            this.UI_Activate(false);

            // ... wait until next frame
            yield return null;
        }

        private void OnDisable()
        {
            this.UI_Activate(false);
        }

        // the function called when there is a remote setting
        protected override void RemoteSettings_Remote()
        {
            // Remove a listener to apply settings when successfully retrieved: 
            ConfigManager.FetchCompleted -= ApplyRemoteSettings;

            int iRepeat = VRG_Remote.GetInt(this.m_Settings_Repeat);
            WAS_echo("Inside: " + this.m_Settings_Repeat + "  = " + Animator.StringToHash(iRepeat.ToString()) + " - " + iRepeat);

            string sDate = VRG_Remote.GetString(this.m_Settings_Date).Trim();
            WAS_echo("Inside: " + this.m_Settings_Date + "  = " + Animator.StringToHash(sDate) + " - " + sDate);

            string sTitle = VRG_Remote.GetString(this.m_Settings_Title).Trim();
            WAS_echo("Inside: " + this.m_Settings_Title + " = " + Animator.StringToHash(sTitle) + " - " + sTitle);

            string sBody = VRG_Remote.GetString(this.m_Settings_Body).Trim();
            WAS_echo("Inside: " + this.m_Settings_Body + " = " + Animator.StringToHash(sBody) + " - " + sBody);

            int iLocalRepeat = VRG_Session.Int("VRG_Announcement", "repeat");
            WAS_echo(iRepeat + " > " + iLocalRepeat);

            bool bShow = false;

            if (
                (iRepeat == 0 || iRepeat > iLocalRepeat)
                && sDate != ""
                && sTitle != ""
                && sBody != ""
                )
            {
                bShow = true;

                this.m_UI_Date.text = sDate;
                this.m_UI_Title.text = sTitle;
                this.m_UI_Body.text = sBody.Replace("<br>", "\n");

                int iHash = Animator.StringToHash(sDate + sTitle + sBody);
                WAS_echo("Inside: Hash = " + iHash);

                int sLocalHash = VRG_Session.Int("VRG_Announcement", "hash");


                if (sLocalHash != iHash)
                {
                    WAS_echo("sLocalHash = " + sLocalHash);

                    VRG_Session.Int("VRG_Announcement", "hash", iHash);
                    VRG_Session.Int("VRG_Announcement", "repeat", 1);
                }
                else
                {
                    WAS_echo("iLocalRepeat = " + iLocalRepeat);

                    VRG_Session.Add("VRG_Announcement", "repeat");
                }
            }

            this.UI_Activate(bShow);

            if (!bShow)
            {
                this.gameObject.SetActive(false);
            }
        }

        // Activate the slider and the messages of the UI
        private void UI_Activate(bool valueLocal)
        {
            WAS_echo("UI_Activate with valueLocal = " + valueLocal);

            if (this != null)
            {
                WAS_echo("this.transform = " + this.transform + "this.isActiveAndEnabled = " + this.isActiveAndEnabled);

                if (this.transform != null && this.isActiveAndEnabled)
                {
                    WAS_echo("Ok si lo voy a hacer");
                    foreach (Transform child in this.transform)
                    {
                        child.gameObject.SetActive(valueLocal);
                    }
                }
            }
        }

    }
}