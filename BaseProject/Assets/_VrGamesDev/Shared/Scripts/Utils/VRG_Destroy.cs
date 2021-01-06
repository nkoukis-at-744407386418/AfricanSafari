using System.Collections;using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// Destroy a GameObject after some time, it could destroy itself or its parent
    /// </summary>
    public class VRG_Destroy : VRG_Base
    {
        /// <summary>
        /// The time it will wait to perform the actions
        /// </summary>
        [Tooltip("The time it will wait to perform the actions")]
        [SerializeField] private float m_Delay = 0.0f;

        /// <summary>
        /// If it will wait until the next frame, useful if used on coroutines
        /// </summary>
        [Tooltip("If it will wait until the next frame, useful if used on coroutines")]
        [SerializeField] private bool m_NextFrame = false;

        /// <summary>
        /// If false, i will destroy myself, if true, i will destroy my parent (myself included)
        /// </summary>
        [Tooltip("If false, i will destroy myself, if true, i will destroy my parent (myself included)")]
        [SerializeField] private bool m_Parent = false;


        // on enable call a yield wait
        private void OnEnable()
        {
            StartCoroutine(this.OnEnable_IEnumerator());
        }

        // Coroutine funciont of Download
        private IEnumerator OnEnable_IEnumerator()
        {
            // if it will wit until this current frame is over
            if (this.m_NextFrame)
            {
                yield return null;
            }

            // call my parent
            if (this.m_Parent)
            {
                this.DestroyParent();
            }

            // or myself
            else
            {
                this.DestroyMe();
            }

            // finish next frame
            yield return null;
        }

        // May the force be with you
        private void DestroyMe()
        {
            WAS_echo("onEnabled: DestroyMe(" + this.transform.name + ");");
            Object.Destroy(this.gameObject, this.m_Delay);
        }

        // Father, May the force be with you
        private void DestroyParent()
        {
            if (this.transform.parent)
            {
                WAS_echo("onEnabled: DestroyParent(" + this.transform.name + ");");
                Object.Destroy(this.transform.parent.gameObject, this.m_Delay);
            }
            else
            {
                this.DestroyMe();
            }
        }

    }
}
