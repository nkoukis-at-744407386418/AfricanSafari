using System.Collections;

using UnityEngine;

using UnityEngine.AddressableAssets;



namespace VrGamesDev.DDuA.Examples
{
    /// <summary>
    /// Class that controls the spawn and duration time of the example 03 Spawner
    /// </summary>
    public class VRG_SpawnRandomItem : VRG_Base
    {
        /// <summary>
        /// The material to modify the colors
        /// </summary>
        [Tooltip("The material to modify the colors")]
        [SerializeField] public Renderer m_Material = null;

        /// <summary>
        /// Random position to spawn in the X coordinates
        /// </summary>
        [Tooltip("Random position to spawn in the X coordinates")]
        [SerializeField] private Vector2 m_X = new Vector2(-7, 7);

        /// <summary>
        /// Random position to spawn in the Y coordinates
        /// </summary>
        [Tooltip("Random position to spawn in the Y coordinates")]
        [SerializeField] private Vector2 m_Y = new Vector2(-3, 6);

        /// <summary>
        /// Duration in seconds before it dissapears
        /// </summary>
        [Tooltip("Duration in seconds before it dissapears")]
        [SerializeField] private Vector2 m_Duration = new Vector2(4, 8);

        // on enable call a yield wait
        private void OnEnable()
        {
            StartCoroutine(this.OnEnable_IEnumerator());
        }

        // Coroutine funciont of Download
        private IEnumerator OnEnable_IEnumerator()
        {
            // set position
            this.gameObject.transform.position = new Vector3(Random.Range(this.m_X.x, this.m_X.y), Random.Range(this.m_Y.x, this.m_Y.y), 0);

            // set scale
            this.gameObject.transform.localScale = new Vector3(Random.Range(0.35f, 3.0f), Random.Range(0.35f, 3.0f), Random.Range(0.35f, 3.0f));

            // wait alive
            yield return new WaitForSeconds(Random.Range(this.m_Duration.x, (this.m_Duration.y + 1)));

            // release instance
            Addressables.ReleaseInstance(this.gameObject);

            // wait until next frame.
            yield return null;
        }
    }
}