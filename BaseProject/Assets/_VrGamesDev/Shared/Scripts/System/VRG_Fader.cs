using System;
using System.Collections;

using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// This class modify the alpha value of an element <a href="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/class-CanvasGroup.html">Canvas group</a>
    /// It can fade in and fade out, it has events on every step to inform the other objects 
    /// </summary>
    public class VRG_Fader : VRG_Base
    {
        /// <summary>
        /// Canvas group that has the alpha property
        /// </summary>
        [Tooltip("Canvas group that has the alpha property")]
        [SerializeField] private CanvasGroup m_CanvasGroup = null;

        /// <summary>
        /// Array that can activate some gameobjects, When it starts the fading
        /// </summary>
        [Tooltip("The items to turn on when the fade occurs")]
        [SerializeField] private GameObject[] m_GameObjects = null;

        /// <summary>
        /// How long will wait before it apply the fade in
        /// </summary>
        [Header("From: Fade In")]
        [Tooltip("How long will wait before it apply the fade in")]
        [SerializeField] private float m_FadeInDelay = 0.0f;

        /// <summary>
        /// The duration in seconds for how long will it takes to apply the fade to alpha 100%
        /// </summary>
        [Tooltip("The duration of the Fade In from 0% to 100%")]
        [SerializeField] private float m_FadeInDuration = 0.50f;

        /// <summary>
        /// How long will wait before it apply the fade out
        /// </summary>
        [Header("From: Fade Out")]
        [Tooltip("How long will wait before it apply the fade out")]
        [SerializeField] private float m_FadeOutDelay = 0.10f;

        /// <summary>
        /// The duration in seconds for how long will it takes to apply the fade to alpha 0%
        /// </summary>
        [Tooltip("How long will take to apply the fade")]
        [SerializeField] private float m_FadeOutDuration = 0.50f;


        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        [Tooltip("The flag that inform the class if it is fade ining or outing.")]
        [SerializeField] private bool m_InOut = true;
        public bool inOut { get { return this.m_InOut; } }




        /// <summary>
        /// Public Event: Trigers ON = When the scene is unloaded and it is over, this event is triggered 
        /// </summary>
        public event Action OnFadeIn;

        /// <summary>
        /// Public Event: Trigers ON = When the scene is unloaded and it is over, this event is triggered
        /// </summary>
        public event Action OnFadeOut;


        private void Awake()
        {
            // if null == search for the group canvas in my own ppl
            if (this.m_CanvasGroup == null)
            {
                this.m_CanvasGroup = this.GetComponent<CanvasGroup>();
            }
        }

        // Ensure that all events are unsubscribed when this is destroyed.
        private void OnDestroy()
        {
            // two events, two destroys
            OnFadeIn = null;
            OnFadeOut = null;
        }

        /// <summary>
        /// Public Method to stop the auto transition from fade in and fade out
        /// </summary>
        /// <param name="valueLocal">bool, true for auto transition</param>
        public void SetAutoFade(bool valueLocal)
        {
            this.m_InOut = valueLocal;
        }


        /// <summary>
        /// Public method, to start the fading process
        /// </summary>
        /// <param name="valueLocal">
        /// <strong>TRUE</strong> = Fade in<br></br>
        /// <strong>FALSE</strong> = Fade out
        /// </param>
        public void Fade(bool valueLocal)
        {
            if (this.m_CanvasGroup != null)
            {
                if (valueLocal)
                {
                    StartCoroutine(this.FadeIn());
                }
                else
                {
                    StartCoroutine(this.FadeOut());
                }
            }
            else
            {
                this.Logs
                (
                    "CanvasGroup is NULL",
                    "VRG_Fader->Fade()",
                    ENUM_Verbose.WARNING
                );
            }
        }

        private IEnumerator FadeIn()
        {
            float fCurrent = 0.0f;
            this.m_CanvasGroup.alpha = 0.0f;

            if (this.m_FadeInDelay > 0)
            {
                yield return new WaitForSeconds(m_FadeInDelay);
            }

            foreach (GameObject child in this.m_GameObjects)
            {
                child.SetActive(true);
            }

            while (this.m_CanvasGroup.alpha < 1.0f)
            {
                fCurrent += Time.deltaTime;

                this.m_CanvasGroup.alpha = fCurrent / this.m_FadeInDuration;

                yield return null;
            }

            this.m_CanvasGroup.alpha = 1.0f;

            // invoke the events delegateds
            OnFadeIn?.Invoke();

            if (this.m_InOut)
            {
                this.Fade(false);
            }

            yield return null;
        }


        private IEnumerator FadeOut()
        {
            float fCurrent = this.m_FadeOutDuration;
            this.m_CanvasGroup.alpha = 1.0f;

            if (this.m_FadeOutDelay > 0)
            {
                yield return new WaitForSeconds(m_FadeOutDelay);
            }

            while (this.m_CanvasGroup.alpha > 0.0f)
            {
                fCurrent -= Time.deltaTime;

                this.m_CanvasGroup.alpha = fCurrent / this.m_FadeOutDuration;

                yield return null;
            }

            this.m_CanvasGroup.alpha = 0.0f;

            foreach (GameObject child in this.m_GameObjects)
            {
                child.SetActive(false);
            }

            // invoke the events delegateds
            OnFadeOut?.Invoke();

            yield return null;
        }
        

    }
}