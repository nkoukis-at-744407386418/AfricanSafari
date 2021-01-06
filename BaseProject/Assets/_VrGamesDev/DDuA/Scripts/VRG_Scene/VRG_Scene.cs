using System.Collections;

using Unity.RemoteConfig;

using UnityEngine;
using UnityEngine.SceneManagement;



namespace VrGamesDev.DDuA
{
    /// <summary>
    /// This class controls de flow of the scene loaded asynchronous
    /// it reads the data from remote and start the scene elements
    /// when the VRG_Carousel is ready to load ¡t
    /// </summary>
    public class VRG_Scene : VRG_Instantiate
    {
        /// <summary>
        /// The name of the scene that contains the VRG_Init object, by default tries to load the "Main" scene
        /// </summary>
        [Header("From: VRG_Scene")]
        [Tooltip("The name of the scene that contains the VRG_Init object, by default tries to load the 'Main' scene")]
        [SerializeField] [SceneName] private string m_VRG_Init;

        /// <summary>
        /// Add addressables to this array, this array will be downloaded but instanciated
        /// </summary>
        [Tooltip("Add addressables to this array, this array will be downloaded but instanciated")]
        [SerializeField] private string[] m_PreLoad = null;

        /// <summary>
        /// When the scene is fully loaded this is the array of Gameobjects to activate
        /// </summary>
        [Tooltip("When the scene is fully loaded this is the array of Gameobjects to activate")]
        [SerializeField] private GameObject[] m_Activate = null;

        //[Header("From: Debug - DO NOT EDIT unless you understand what is going on - ")]
        [Tooltip("The loaded scene, this data is used to save the logs")]
        //[SerializeField]
        private string MyOwnSceneName = "";

        [Tooltip("The loaded scene, this data is used to save the logs")]
        //[SerializeField]
        private string m_DefaultName = "Main";

        private void Awake()
        {
            // ... take the name of the scene and save it 
            this.MyOwnSceneName = SceneManager.GetActiveScene().name;

            // if this is loaded as alone scene
            if (VRG_Core.Instance == null)
            {
                // save the scene name to come here later
                VRG_Session.String("VRG_Init", "Scene", this.MyOwnSceneName);

                // if nothing is configured
                if (this.m_VRG_Init == "[RELOAD SCENE]")
                {
                    // try the Main default
                    this.m_VRG_Init = m_DefaultName;
                }

                // Load the VRG_init then come back
                SceneManager.LoadScene(this.m_VRG_Init);
            }
            else
            {
                // Add a listener to apply settings when successfully retrieved: 
                ConfigManager.FetchCompleted += ApplyRemoteSettings;

                // Fetch configuration setting from the remote service: 
                ConfigManager.FetchConfigs<VRG_Remote_UserAttributes, VRG_Remote_AppAttributes>(new VRG_Remote_UserAttributes(), new VRG_Remote_AppAttributes());                
            }
        }

        // the function called when there is a remote setting
        protected override void RemoteSettings_Remote()
        {
            // Remove a listener to apply settings when successfully retrieved: 
            ConfigManager.FetchCompleted -= ApplyRemoteSettings;

            string sLogs = VRG_Remote.GetString("VRG_Scene." + this.MyOwnSceneName);

            // Retrieve the remoteData of this scene
            string[] as_Remote = sLogs.Split('|');

            // cycle the remote array
            for (int i = 0; i < as_Remote.Length; i++)
            {
                // make sure there aren't spaces or line breaks
                as_Remote[i] = as_Remote[i].Trim();

                // .. In case it's not empty
                if (as_Remote[i].Trim() != "")
                {
                    // add it to the Addressable List to download it 
                    this.m_Addressables.Add(new VRG_Addressable(as_Remote[i], true));
                }
            }

            // cycle the Pre_Load array
            for (int i = 0; i < this.m_PreLoad.Length; i++)
            {
                // make sure there aren't spaces or line breaks
                this.m_PreLoad[i] = this.m_PreLoad[i].Trim();

                // .. In case it's not empty
                if (this.m_PreLoad[i].Trim() != "")
                {
                    // add it to the Addressable List to download it 
                    this.m_Addressables.Add(new VRG_Addressable(this.m_PreLoad[i], false));
                }
            }

            // Add a listener to the VRG_Core when it is done downloading the addressables from remote
            VRG_Core.OnDownload += OnDownload;

            // Execute the download
            VRG_Core.Download(this.m_Addressables);

            // save to logs
            this.Logs
            (
                "<b>(" + this.m_Addressables.Count + ") Loadings: </b> " + sLogs.Replace("|", "<br>"),
                "VRG_Scene->RemoteSettings_Remote()",
                ENUM_Verbose.STATUS
            );
        }

        // Delegated function, it is called when the download of the Addressables is done
        private void OnDownload()
        {
            // Unsuscribe, we have all the data downloaded
            VRG_Core.OnDownload -= OnDownload;

            // call a the routine to Instantiate the downloaded Addressables
            StartCoroutine(this.OnDownload_IEnumerator());
        }

        // Coroutine funciont of Download
        private IEnumerator OnDownload_IEnumerator()
        {
            // Wait until the VRG_core Instantiate All
            yield return base.InstantiateAll();

            // Suscribe until the player is ready to activate the scene
            VRG_Core.OnSceneActivated += OnSceneActivated;

            // Inform the scene is fully loaded 
            VRG_Core.SceneLoaded();

            // finish next frame
            yield return null;
        }

        // Delegated function, when the user want to activate the scene
        private void OnSceneActivated()
        {
            // Unsuscribe, the scene is active now
            VRG_Core.OnSceneActivated -= OnSceneActivated;

            // check all the gameObjects of this.m_Activate
            foreach (GameObject child in this.m_Activate)
            {
                if (child != null)
                {
                    // activate all the Gameobjects
                    child.SetActive(true);
                }
            }

            // check all the gameObjects of this.m_GameObjects
            foreach (GameObject child in this.m_GameObjects)
            {
                if (child != null)
                {
                    // activate all the Gameobjects
                    child.SetActive(true);
                }
            }

            // Suscribe when the scene is unloaded
            VRG_Core.OnSceneUnload += OnSceneUnload;

            this.Logs
            (
                "<color=blue>Scene: <i>" + this.MyOwnSceneName + "</i></color> activated",
                "VRG_Scene->OnSceneActivated()",
                ENUM_Verbose.LOGS
            );
        }

        // Delegated function, when the scene is unloaded
        private void OnSceneUnload()
        {
            // Unsuscribe, the scene is no more
            VRG_Core.OnSceneUnload -= OnSceneUnload;

            // check false all the gameObjects of this.m_Activate
            foreach (GameObject child in this.m_Activate)
            {
                if (child != null)
                {
                    // ... and Deactivate the objects so it is consistent the user experience
                    child.SetActive(false);
                }
            }

            // check false all the gameObjects of this.m_GameObjects
            foreach (GameObject child in this.m_GameObjects)
            {
                if (child != null)
                {
                    // activate all the Gameobjects
                    child.SetActive(false);
                }
            }

            this.Logs
            (
                "<color=blue>Scene: <i>" + this.MyOwnSceneName + "</i></color> Unloaded",
                "VRG_Scene->OnSceneUnload()",
                ENUM_Verbose.INFO
            );
        }

    }
}