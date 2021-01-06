using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace VrGamesDev.DDuA
{
    public class VRG_CarouselButton : VRG_Base
    {
        [Tooltip("From Remote: The setting that will decide if it is saved or not ")]
        [SerializeField] private int m_Number = 0;

        [Tooltip("From Remote: The setting that will decide if it is saved or not ")]
        [SerializeField] private float m_Scale = 0.8f;

        [Tooltip("From Remote: The setting that will decide if it is saved or not ")]
        [SerializeField] private Button m_Button = null;

        [Tooltip("From Remote: The setting that will decide if it is saved or not ")]
        [SerializeField] private Image m_Image = null;

        [Tooltip("From Remote: The setting that will decide if it is saved or not ")]
        [SerializeField] private Color32 m_ColorUnSelected = new Color32(180, 180, 180, 128);

        [Tooltip("From Remote: The setting that will decide if it is saved or not ")]
        [SerializeField] private Color32 m_ColorSelected = new Color32(255, 255, 255, 255);


        // on enable call a yield wait
        private void OnEnable()
        {
            StartCoroutine(this.OnEnable_IEnumerator());
        }

        // enumerator from enable
        private IEnumerator OnEnable_IEnumerator()
        {
            if (this.m_Button != null)
            {
                // get the button
                Button btn = this.m_Button.GetComponent<Button>();

                // add a listener
                btn.onClick.AddListener(TaskOnClick);
            }

            yield return null;
        }

        // the listener function
        private void TaskOnClick()
        {
            // call my Number Carousel
            VRG_Core.CreateCarousel(this.m_Number);
        }

        // the listener function
        public void SetNumber(int valueLocal)
        {
            // set number
            this.m_Number = valueLocal;
        }

        // the listener function
        public void IsItMine(int valueLocal)
        {
            // assign the default value
            float fScale = this.m_Scale;
            Color32 c32Color = this.m_ColorUnSelected;

            // if i am the one selected
            if (valueLocal == this.m_Number)
            {
                // the default data is assigned
                fScale = 1.0f;
                c32Color = this.m_ColorSelected;
            }

            // set the data
            if (this.m_Image != null)
            {
                this.m_Image.color = c32Color;
            }
            this.transform.localScale = new Vector3(fScale, fScale, fScale);
        }


    }
}
