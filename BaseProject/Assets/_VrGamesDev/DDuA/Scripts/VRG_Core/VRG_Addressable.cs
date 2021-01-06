using UnityEngine;

using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;



namespace VrGamesDev.DDuA
{
    /*
     * This class is a serializable to be able to watch it in inspector
     * it saves and define the Async data and status of each of the Addresables
     */
    [System.Serializable]
    public class VRG_Addressable
    {
        [Tooltip("The name of the addressble to load it should be the same as in the Addressables Group Panel")]
        [SerializeField] public string m_Name = "";

        [Tooltip("If it will just be loaded or Instantiated")]
        [SerializeField] public bool m_Create = false;

        [Tooltip("If it will just be loaded or Instantiated")]
        [SerializeField] public bool m_Active = false;

        [Tooltip("Async Data of the Handler from Size")]
        [SerializeField] public AsyncOperationHandle<long> m_Size = new AsyncOperationHandle<long>();

        [Tooltip("Async Data of the Handler from the download progress")]
        [SerializeField] public AsyncOperationHandle m_Progress = new AsyncOperationHandle();

        [Tooltip("Async Data of the Handler from Scene")]
        [SerializeField] public AsyncOperationHandle<SceneInstance> m_SceneInstance = new AsyncOperationHandle<SceneInstance>();

        // Just reset everything to null, "", false, or default
        public void Reset()
        {
            this.m_Name = "";
            this.m_Create = false;
            this.m_Active = false;

            this.m_Size = new AsyncOperationHandle<long>();
            this.m_Progress = new AsyncOperationHandle();
            this.m_SceneInstance = new AsyncOperationHandle<SceneInstance>();
        }

        // Creator with:
        // string nameLocal = the name that will be loaded and instantiated
        public VRG_Addressable(string nameLocal)
        {
            // Back to new
            this.Reset();

            // assign the new name
            this.m_Name = nameLocal;
        }

        // Creator with the name and create status
        // string nameLocal = the name that will be loaded and instantiated
        // bool createLocal = the status, if false, it will not be instantiated
        public VRG_Addressable(string nameLocal, bool createLocal)
        {
            // Back to new
            this.Reset();

            // assign the new name
            this.m_Name = nameLocal;

            // design the create status, usually set to true
            this.m_Create = createLocal;
        }

        // Creator with the name and create status and the size handler
        // string nameLocal = the name that will be loaded and instantiated
        // bool createLocal = the status, if false, it will not be instantiated
        // AsyncOperationHandle<long> sizeLocal = the Handler that will check and save the size of the Addressable
        public VRG_Addressable(string nameLocal, bool createLocal, AsyncOperationHandle<long> sizeLocal)
        {
            // Back to new
            this.Reset();

            // assign the new name
            this.m_Name = nameLocal;

            // design the create status, usually set to true
            this.m_Create = createLocal;

            // In case we have a size handler, how to follow it up
            this.m_Size = sizeLocal;
        }
    }
}