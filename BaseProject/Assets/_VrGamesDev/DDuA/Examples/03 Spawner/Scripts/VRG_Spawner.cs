using System.Collections;using UnityEngine;using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace VrGamesDev.DDuA.Examples
{
    /// <summary>
    /// This class controls the mechanics of the example DDuA/03 Spawner
    /// Custom the duration and number of random spawns before they dissapears
    /// </summary>
    public class VRG_Spawner : VRG_Base
    {
        /// <summary>
        /// The array of the items to spawn, it should be an address name valid
        /// </summary>
        [Tooltip("The array of the items to spawn, it should be an address name valid")]
        [SerializeField] private string[] m_Spawns = { "Item 1", "Item 2", "Item 3"};

        /// <summary>
        /// The random duration range in seconds before it is self-destroyed
        /// </summary>
        [Tooltip("The random duration range in seconds before it is self-destroyed")]
        [SerializeField] private Vector2 m_Duration = new Vector2(1, 3);

        /// <summary>
        /// The number of items per spawn
        /// </summary>
        [Tooltip("The number of items per spawn")]
        [SerializeField] private int m_SpawnsMax = 3;


        // on enable call a yield wait
        private void OnEnable()
        {
            StartCoroutine(this.OnEnable_IEnumerator());
        }

        // Coroutine funciont of Download
        private IEnumerator OnEnable_IEnumerator()
        {
            for(int i = 0; i < this.m_SpawnsMax; i++)
            {
                Addressables.InstantiateAsync(this.m_Spawns[Random.Range(0, this.m_Spawns.Length)]).Completed += SpawnCompleted;
            }

            yield return new WaitForSeconds( Random.Range(this.m_Duration.x, (this.m_Duration.y + 1)));

            StartCoroutine(this.OnEnable_IEnumerator());
        }

        // delegated funciont, that triggers when the Instancing is completed
        private void SpawnCompleted(AsyncOperationHandle<GameObject> obj)
        {
            // If the instantiate succeedd
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                // save the Result to alter it
                GameObject go_Item = (GameObject)obj.Result;

                // change name clone -> addressable , useful in editor 
                go_Item.name = go_Item.name.ToString().Replace("(Clone)", " (Addressable)");

                // activate it 
                go_Item.SetActive(true);

                // set the parent as myself
                go_Item.transform.SetParent(this.transform);

                // set a random color
                go_Item.GetComponent<VRG_SpawnRandomItem>().m_Material.material.SetColor("_Color", new Color32
                (
                    (byte)Random.Range(0, 255),
                    (byte)Random.Range(0, 255),
                    (byte)Random.Range(0, 255),
                    255
                ));
            }

            // ... since not
            else
            {
                // LOGS: something is broken
                this.Logs
                (
                    "Spawn an item failed",
                    "VRG_Spawner->SpawnCompleted()",
                    ENUM_Verbose.ERROR
                );
            }
        }
    }
}