using System;
using System.Collections;
using System.Collections.Generic;

using Unity.RemoteConfig;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;


namespace VrGamesDev.DDuA
{
    /// <summary>
    /// This class is the core that controls the whole Addressable system
    /// 
    /// - Download and play the Carousel
    /// - Reads the data from VRG_remote and start the download of the singletons modules
    /// - Gets the scenes and their prefab data from the addressable server/cache
    /// - Inform the player with a loading bar what is happening behind curtain
    /// </summary>
    public class VRG_Core : VRG_Instantiate
    {
        /// <summary>
        /// From Remote: The remote variable that host the list of the Carousels
        /// </summary>
        [Header("From: Remote Settings")]
        [Tooltip("From Remote: The remote variable that host the list of the Carousels")]
        //[SerializeField]
        private string m_Settings_Carousels = "VRG_Core.carousels";

        /// <summary>
        /// From Remote: All the Prefab and singletons, it is useful to get all instantiated
        /// </summary>
        [Tooltip("From Remote: All the Prefab and singletons, it is useful to get all instantiated")]
        //[SerializeField]
        private string m_Settings_Singletons = "VRG_Core.singletons";

        /// <summary>
        /// From Remote: All the scenes in game, it will predownload all of them, the first one will be the home
        /// </summary>
        [Tooltip("From Remote: All the scenes in game, it will predownload all of them, the first one will be the home")]
        //[SerializeField]
        private string m_Settings_Scenes = "VRG_Core.scenes";

        /// <summary>
        /// From Remote: The remote variable that host the list of the Carousels
        /// </summary>
        [Tooltip("From Remote: The remote variable that host the list of the Carousels")]
        //[SerializeField]
        private string m_Settings_Downloads = "VRG_Core.downloads";

        [Tooltip("From Remote Settings: The list of all the 'Carousels assets' while Carousel for downloads")]
        //[SerializeField]
        private List<string> m_RemoteCarousels = new List<string>();

        [Tooltip("From Remote Settings: The list of all the classes and objects needed, usually singletons")]
        //[SerializeField]
        private List<string> m_RemoteSingletons = new List<string>();

        [Tooltip("From Remote Settings: The list of all the scenes, the first one will be loaded as home")]
        //[SerializeField]
        private List<string> m_RemoteScenes = new List<string>();

        [Tooltip("From Remote Settings: The list of all the 'download assets' they are not instanced, just downloaded")]
        //[SerializeField]
        private List<string> m_RemoteDownload = new List<string>();

        /* TODO: Implement the catalog default
        [Tooltip("From Remote Settings: The Catalog of the new updates")]
        [SerializeField] private List<string> m_RemoteCatalog = new List<string>();
        */



        /// <summary>
        /// The Parent object that contains the camera, this camera is just local for the loader
        /// </summary>
        [Header("From: UI")]
        [Tooltip("The Parent object that contains the camera, this camera is just local for the loader")]
        [SerializeField] private GameObject m_CameraBox = null;

        /// <summary>
        /// The Parent object that contains the Loading bar and the download info
        /// </summary>
        [Tooltip("The Parent object that contains the Loading bar and the download info")]
        [SerializeField] private GameObject m_DownloadBox = null;

        /// <summary>
        /// Status that inform the player what is going in
        /// </summary>
        [Tooltip("Status that inform the player what is going in")]
        [SerializeField] private Text m_UiDownloadStatus = null;

        /// <summary>
        /// Internet network status and tries to get the bytes
        /// </summary>
        [Tooltip("Internet network status and tries to get the bytes")]
        [SerializeField] private Text m_UiInternetNetwork = null;

        /// <summary>
        /// The slider progress bar
        /// </summary>
        [Tooltip("The slider progress bar")]
        [SerializeField] private Slider m_UiSlider = null;

        /// <summary>
        /// The accept button, it allows to go to scene
        /// </summary>
        [Tooltip("The accept button, it allows to go to scene")]
        [SerializeField] private Image m_UiButton = null;

        /// <summary>
        /// Is there a progress in progress? (Pun intended)
        /// </summary>
        [Tooltip("Is there a progress in progress? (Pun intended)")]
        [SerializeField] private string m_UiButtonText = "Start";

        /// <summary>
        /// The download persentage text
        /// </summary>
        [Tooltip("The download persentage text")]
        [SerializeField] private Text m_UiPercentage = null;

        /// <summary>
        /// Luke, I'm your <a href="index.html#VrGamesDev.VRG_Fader">Fader</a>
        /// </summary>
        [Tooltip("Luke, I'm your Fader")]
        [SerializeField] private VRG_Fader m_VRG_Fader = null;







        [Header("From: Carousels")]
        [Tooltip("The game object instantiated that holds the Loading asset")]
        //[SerializeField]
        private GameObject m_CarouselGO;

        [Tooltip("The current index of the Loading asset that is running")]
        //[SerializeField]
        private int m_CarouselIndex = 0;

        [Tooltip("Flag the indicates if the loading asset is being created or is free to go")]
        //[SerializeField]
        private bool m_CarouselFreeToCreate = true;

        [Tooltip("The little box prefab to instantiate")]
        [SerializeField] private GameObject m_CarouselButton = null;

        [Tooltip("The menu box")]
        [SerializeField] private GameObject m_CarouselMenuBox = null;



        [Header("From: Scenes")]
        [Tooltip("The scene Addressable")]
        //[SerializeField]
        private VRG_Addressable m_Scene = new VRG_Addressable("",false);

        [Tooltip("Is some Scene loading?")]
        //[SerializeField]
        private bool m_IsLoadingScene = false;

        [Tooltip("Is some Scene Unloading?")]
        //[SerializeField]
        private bool m_IsUnloadingScene = false;

        [Tooltip("Is the Fade screen fading?")]
        //[SerializeField]
        private bool m_IsFading = false;

        [Tooltip("AutoLoad the next scene when ready?")]
        //[SerializeField]
        private bool m_SceneAutoLoad = false;



        /// <summary>
        /// The default time to pre download assets
        /// </summary>
        [Header("From: Downloads")]
        [Tooltip("The default time to pre download assets")]
        [SerializeField]
        private float m_DelayBetweenDownloads = 1.5f;

        [Tooltip("The current time for the next predownloadable")]
        //[SerializeField]
        private float m_DelayBetweenDownloadsCurrent = 0.0f;

        [Tooltip("The current index of the download asset that is running")]
        //[SerializeField]
        private int m_DownloadIndex = 0;

        [Tooltip("The current status of the internet conection")]
        //[SerializeField]
        private string m_ReachabilityText = "";

        [Tooltip("The times it tries to reach the content to download it")]
        //[SerializeField]
        private int m_ProgressTry = 0;

        [Tooltip("Is there a progress in progress? (Pun intended)")]
        //[SerializeField]
        private bool m_IsProgress = false;

        [Tooltip("Is there a process of instantiating the Addressables?")]
        //[SerializeField]
        private bool m_IsInstantiate = false;





        /// <summary>
        /// Public Event: Trigers WHEN = A set of Addressables is finished and fully downloaded
        /// </summary>
        public static event Action OnDownload;

        /// <summary>
        /// Public Event: Trigers WHEN = The scene is unloaded and it is over
        /// </summary>
        public static event Action OnSceneUnload;

        /// <summary>
        /// Public Event: Trigers WHEN = The scene is loaded and activated
        /// </summary>
        public static event Action OnSceneActivated;


        // singletoning Pattern
        public static VRG_Core Instance; private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;

                // ... and I will live forever
                DontDestroyOnLoad(this);

                // Listener to the event OnDownload, when the whole package is downloaded
                VRG_Core.OnDownload += OnDownload_Delegated;

                // Listener to the event OnSceneActivated, when the whole package is downloaded
                VRG_Core.OnSceneActivated += PreloadAssets;

                // Listener to the event OnFadeIn, when the whole package is downloaded
                this.m_VRG_Fader.OnFadeIn += FadeTheScene_Medium;

                // Listener to the event OnFadeOut, when the whole package is downloaded
                this.m_VRG_Fader.OnFadeOut += FadeTheScene_End;

                // Add a listener to apply settings when successfully retrieved: 
                ConfigManager.FetchCompleted += ApplyRemoteSettings;

                // Fetch configuration setting from the remote service: 
                ConfigManager.FetchConfigs<VRG_Remote_UserAttributes, VRG_Remote_AppAttributes>(new VRG_Remote_UserAttributes(), new VRG_Remote_AppAttributes());
            }
            else
            {
                // I am not the one... I will walk to the eternal darkness
                Destroy(this.gameObject);
            }
        }

        // Ensure that all events are unsubscribed when this is destroyed.
        private void OnDestroy()
        {
            // three events, three destroys
            OnDownload = null;
            OnSceneUnload = null;
            OnSceneActivated = null;
        }

        // virtual functioned fomr the apply settings
        protected override void RemoteSettings_Remote()
        {
            string sLogs = "";

            string sCheckForNullArray = "";

            // the autoload is off by default
            this.m_SceneAutoLoad = false;

            // Remove a listener to apply settings when successfully retrieved: 
            ConfigManager.FetchCompleted -= ApplyRemoteSettings;

            //Check if the device cannot reach the internet
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //Not Reachable
                this.m_ReachabilityText = "No internet conection";
            }

            //Check if the device can reach the internet via a carrier data network
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                //Reachable via carrier data network.
                this.m_ReachabilityText = "Mobile network";
            }

            //Check if the device can reach the internet via a LAN
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                //Reachable via Local Area Network;
                this.m_ReachabilityText = "Wifi";
            }



            // reset the logs
            sLogs = "";

            // Load remote Carousels settings
            sCheckForNullArray = VRG_Remote.GetString(this.m_Settings_Carousels).Trim();

            // just if there are items
            if (sCheckForNullArray != "")
            {
                // Load remote Carousels settings into an array using the | separator
                string[] sRemoteCarousels = sCheckForNullArray.Split('|');

                //cycle the array from the Remote server
                for (int i = 0; i < sRemoteCarousels.Length; i++)
                {
                    if (sRemoteCarousels[i].Trim() != "")
                    {
                        // make sure there aren't spaces or line breaks
                        this.m_RemoteCarousels.Add(sRemoteCarousels[i].Trim());

                        // save to logs
                        sLogs += "<br> - " + sRemoteCarousels[i].Trim();
                    }
                }
            }

            if (this.m_RemoteCarousels.Count > 0)
            {
                // LOGS:
                this.Logs
                (
                    "<b>(" + this.m_RemoteCarousels.Count + ") Carousels: </b><br> " + sLogs + "<br><br>",
                    "VRG_Core->RemoteSettings_Remote()",
                    ENUM_Verbose.STATUS
                );

                // select a random Waitins from Remote
                this.m_CarouselIndex = UnityEngine.Random.Range(0, this.m_RemoteCarousels.Count);
            }
            else
            {
                // LOGS:
                this.Logs
                (
                    "<b>No Carousels: </b> Please add a Carousel in the <b>VRG_Remote</b>",
                    "VRG_Core->RemoteSettings_Remote()",
                    ENUM_Verbose.WARNING
                );
            }

            if (this.m_RemoteCarousels.Count > 1)
            {
                // cycle the carrousels to create the mini-buttons
                for (int i = 0; i < this.m_RemoteCarousels.Count; i++)
                {
                    // instantiate the button
                    GameObject go_GameObject = UnityEngine.Object.Instantiate(this.m_CarouselButton);

                    // its name is the if of the carousel
                    go_GameObject.name = i.ToString();

                    // asign its father, the menu
                    go_GameObject.transform.SetParent(this.m_CarouselMenuBox.transform, true);

                    // ... and its number
                    go_GameObject.GetComponent<VRG_CarouselButton>().SetNumber(i);

                    // ... and selected
                    go_GameObject.GetComponent<VRG_CarouselButton>().IsItMine(this.m_CarouselIndex);
                }
            }


            // Reset the logs
            sLogs = "";

            // Load remote Singletons settings
            sCheckForNullArray = VRG_Remote.GetString(this.m_Settings_Singletons).Trim();

            // just if there are items
            if (sCheckForNullArray != "")
            {
                // ... into an array using the | separator
                string[] sRemoteSingletons = sCheckForNullArray.Split('|');

                //cycle the array from the Remote server
                for (int i = 0; i < sRemoteSingletons.Length; i++)
                {
                    if (sRemoteSingletons[i].Trim() != "")
                    {
                        // make sure there aren't spaces or line breaks
                        this.m_RemoteSingletons.Add(sRemoteSingletons[i].Trim());
                        sLogs += "<br> - " + sRemoteSingletons[i];
                    }
                }
            }


            if (this.m_RemoteSingletons.Count > 0)
            { 
                // LOGS:
                this.Logs
                (
                    "<b>(" + this.m_RemoteSingletons.Count + ") Singletons: </b>" + sLogs + "<br><br>",
                    "VRG_Core->RemoteSettings_Remote()",
                    ENUM_Verbose.STATUS
                );
            }
            else
            {
                // LOGS:
                this.Logs
                (
                    "<b>No Singletons: </b> Please add a singleton in the <b>VRG_Remote</b>",
                    "VRG_Core->RemoteSettings_Remote()",
                    ENUM_Verbose.WARNING
                );
            }




            // Reset the logs
            sLogs = "";

            // Load remote Scenes settings
            sCheckForNullArray = VRG_Remote.GetString(this.m_Settings_Scenes).Trim();

            // just if there are items
            if (sCheckForNullArray != "")
            {
                // Load remote Scenes settings into an array using the | separator
                string[] sRemoteScenes = sCheckForNullArray.Split('|');

                sLogs = "";
                //cycle the array from the Remote server
                for (int i = 0; i < sRemoteScenes.Length; i++)
                {
                    if (sRemoteScenes[i].Trim() != "")
                    {
                        // make sure there aren't spaces or line breaks
                        this.m_RemoteScenes.Add(sRemoteScenes[i].Trim());
                        sLogs += "<br> - " + sRemoteScenes[i];
                    }
                }
            }

            if (this.m_RemoteScenes.Count > 0)
            {
                // LOGS:
                this.Logs
                (
                    "<b>(" + this.m_RemoteScenes.Count + ") Scenes: </b>" + sLogs + "<br><br>",
                    "VRG_Core->RemoteSettings_Remote()",
                    ENUM_Verbose.STATUS
                );
            }
            else
            {
                // LOGS:
                this.Logs
                (
                    "<b>No Scenes: </b> Please add a Scene in the <b>VRG_Remote</b>",
                    "VRG_Core->RemoteSettings_Remote()",
                    ENUM_Verbose.WARNING
                );
            }





            // a clean child is a good child
            string childTrimmed = "";

            // Load remote downloads settings into an array using the | separator
            string[] sRemoteDownload = VRG_Remote.GetString(this.m_Settings_Downloads).Split('|');

            //cycle the array from the Remote server
            foreach (string child in sRemoteDownload)
            {
                // clean the spaces fomr your children
                childTrimmed = child.Trim();

                // if it is new
                if (!this.m_RemoteDownload.Contains(childTrimmed) && childTrimmed != "")
                {
                    // make sure there aren't spaces or line breaks
                    this.m_RemoteDownload.Add(childTrimmed);
                }
            }

            // save the assets needed in each scene
            string[] sScenesItems = null;
            foreach (string child in this.m_RemoteScenes)
            {
                sScenesItems = VRG_Remote.GetString("VRG_Scene." + child).Split('|');

                //cycle the array from the Remote server
                foreach (string childScene in sScenesItems)
                {
                    // nothing like a clean child
                    childTrimmed = childScene.Trim();

                    // if it new
                    if (!this.m_RemoteDownload.Contains(childTrimmed) && childTrimmed != "")
                    {
                        // make sure there aren't spaces or line breaks and then add it
                        this.m_RemoteDownload.Add(childTrimmed);
                    }
                }
            }

            // LOGS:
            sLogs = "";
            if (this.m_RemoteDownload.Count > 0)
            {
                foreach (string child in this.m_RemoteDownload)
                {
                    sLogs += "<br> - " + child;
                }
                this.Logs
                (
                    "<b>(" + this.m_RemoteDownload.Count + ") Downloads:</b><br>" + sLogs + "<br><br>",
                    "VRG_Core->RemoteSettings_Remote()",
                    ENUM_Verbose.STATUS
                );
            }
            else
            {
                this.Logs
                (
                    "<b>No Downloads: </b> Please add a Download in the <b>VRG_Remote</b>",
                    "VRG_Core->RemoteSettings_Remote()",
                    ENUM_Verbose.WARNING
                );
            }



            // Check for scene, this funcion trigger a Carousel asset if needed
            VRG_Core.CheckScene();

            // the instantiate process start now and will not end until all is loaded
            this.m_IsInstantiate = true;

            if (this.m_RemoteSingletons.Count > 0)
            {
                // check and review all the singletons needed preloaded
                for (int i = 0; i < this.m_RemoteSingletons.Count; i++)
                {
                    // get all the sizes of the singletons
                    this.GetSize(this.m_RemoteSingletons[i], true);
                }
            }

            // if no clases to load
            else
            {
                // since the download already started, inform the player of the progress
                StartCoroutine(this.Progress());
            }
        }

        // VRG_Logs wrapper: class that display the valueLocal into the message of the download slider
        protected override void Logs(string valueLocal, string sourceLocal, ENUM_Verbose ENUM_VerboseLocal)
        {
            // If it is already properly initalized
            if (Instance != null)
            {
                // I'm your father
                base.Logs(valueLocal, sourceLocal, ENUM_VerboseLocal);

                // in case of error
                if (ENUM_VerboseLocal == ENUM_Verbose.ERROR || ENUM_VerboseLocal == ENUM_Verbose.WARNING)
                {
                    // change the color
                    valueLocal = "<color=" + VRG_DataBase.GetEnumColor(ENUM_VerboseLocal) + ">" + valueLocal + "</color>";
                }

                if ((int)ENUM_VerboseLocal < (int)ENUM_Verbose.DEBUG)
                {
                    // assign the text 
                    VRG_Core.SetStatus(valueLocal);
                }
            }
        }

        /// <summary>
        /// Show and display the status into the local text
        /// </summary>
        /// <param name="valueLocal"></param>
        public static void SetStatus(string valueLocal)
        {
            // If the object is declared 
            if (Instance.m_UiDownloadStatus != null)
            {
                // assign the text 
                Instance.m_UiDownloadStatus.text = valueLocal;
            }
        }

        /// <summary>
        /// this function decide if it creates a new Carousel asset or
        /// </summary>
        public static void CheckScene()
        {
            // local variable that holds if it is the first scene
            bool bFirstScene = true;

            // you can't load a new game until the scene is loaded, 
            // the button is white because it is not clickable
            Instance.m_UiButton.color = new Color32(255, 255, 255, 255);

            // label it is white and downloading
            Instance.m_UiPercentage.text = "...";

            // ... and the button is not interactable
            Instance.m_UiButton.GetComponent<Button>().interactable = false;


            // was it properly inited?
            if (Instance.m_Scene != null)
            {
                // If we have a valid scene, it is not the first load
                if (Instance.m_Scene.m_SceneInstance.IsValid())
                {
                    if
                    (
                        // it was loaded with succeeded status,
                        Instance.m_Scene.m_SceneInstance.Status == AsyncOperationStatus.Succeeded
                        &&
                        // successfully created
                        Instance.m_Scene.m_Create
                        &&
                        // and it is not already loading a scene 
                        !Instance.m_IsLoadingScene
                    )
                    {
                        // it is not the first one
                        bFirstScene = false;

                        // I am not unloading a scene
                        Instance.m_IsUnloadingScene = false;

                        // if it needs to continue because the fade is on
                        if (Instance.m_VRG_Fader.inOut)
                        {
                            // so start to fade it
                            Instance.FadeTheScene();
                        }
                        else
                        {
                            // since we have no scene, go to the Carousel
                            Instance.UI_Activate(false);

                            // ...  finish the fade
                            Instance.m_IsFading = true;

                            // ... continue where you were
                            Instance.FadeTheScene_Medium();
                        }

                        // inform Logs
                        Instance.Logs
                        (
                            "Scene: <color=blue><i>" + Instance.m_Scene.m_Name + "</i></color> Activating ... ",
                            "VRG_Core->CheckScene()",
                            ENUM_Verbose.STATUS
                        );
                    }
                }
            }

            // if this is the first scene to load
            if (bFirstScene)
            {
                // since we have no scene, go to the Carousel
                Instance.UI_Activate(bFirstScene);
            }
        }

        // Activate the slider and the messages of the UI
        private void UI_Activate(bool valueLocal)
        {
            // if turn it on
            if (valueLocal)
            {
                // just create the Carousel in case of no reload  scene
                if (Instance.m_VRG_Fader.inOut)
                {
                    // since we are loading, create a Carousel 
                    this.CreateCarousel();
                }
            }

            // ... and off
            else
            {
                // Destroy the warning we are about to leave
                this.DestroyCarousel();
            }

            // the slider and download information
            this.m_DownloadBox.SetActive(valueLocal);

            // since we don't need the camera because the scene is active, just turn camera off
            this.m_CameraBox.SetActive(valueLocal);
        }

        // create a Carousel Asset from remoteCarousels
        public static void CreateCarousel(int valueLocal)
        {
            if (valueLocal >= 0 && valueLocal < Instance.m_RemoteCarousels.Count)
            {
                // next carousel
                Instance.m_CarouselIndex = valueLocal;

                Instance.CreateCarousel();
            }
        }

        // create a Carousel Asset from remoteCarousels
        private void CreateCarousel()
        {
            string sCarousel = "";
            bool bShowCarouselMenu = true;

            // if it is not creating a Carousel
            if (this.m_CarouselFreeToCreate)
            {
                // we are creating the Carousel, so do not try again
                this.m_CarouselFreeToCreate = false;

                if (this.m_RemoteCarousels.Count > 0)
                {
                    // get the info from the session to know if we will load a specific carousel
                    sCarousel = VRG_Session.String("VRG_Scene", "Carousel").Trim();

                    if (sCarousel == "")
                    {
                        // save the random Carousel
                        sCarousel = this.m_RemoteCarousels[this.m_CarouselIndex];
                    }
                    else
                    {
                        bShowCarouselMenu = false;
                        VRG_Session.String("VRG_Scene", "Carousel", "");
                    }

                    this.m_CarouselMenuBox.SetActive(bShowCarouselMenu);

                    // inform the logs
                    this.Logs
                    (
                        "Creating <b>" + sCarousel + "</b>",
                        "VRG_Core->CreateCarousel()",
                        ENUM_Verbose.DEBUG
                    );

                    // Instantiate the Carousel defined in Index, we will know it is loaded in Carousel_Completed
                    Addressables.InstantiateAsync(sCarousel).Completed += Carousel_Completed;
                }
            }
        }

        // this function destroy the Carousel GameObject
        private void DestroyCarousel()
        {
            // It will only get destroyed if there is a previous Carousel Asset
            if (Instance.m_CarouselGO != null)
            {
                // save logs to file
                this.Logs
                (
                    "Destroying <b>" + Instance.m_RemoteCarousels[Instance.m_CarouselIndex] + "</b>",
                    "VRG_Core->DestroyCarousel()",
                    ENUM_Verbose.DEBUG
                );

                // cycle the carrousels to create the mini-buttons
                foreach (Transform child in this.m_CarouselMenuBox.transform)
                {
                    // ... and its number
                    child.GetComponent<VRG_CarouselButton>().IsItMine(this.m_CarouselIndex);
                }

                // next carousel
                this.m_CarouselIndex++;

                // unless is the last one
                if (this.m_CarouselIndex >= this.m_RemoteCarousels.Count || this.m_CarouselIndex < 0)
                {
                    // go to the first one
                    this.m_CarouselIndex = 0;
                }

                // Release instance and Addressable memory
                Addressables.ReleaseInstance(Instance.m_CarouselGO);
            }
        }

        // delegated funciont, that triggers when the Instancing is completed
        private void Carousel_Completed(AsyncOperationHandle<GameObject> obj)
        {
            // If the instantiate succeedd
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                // free the create flag, we are not creating it anymore
                this.m_CarouselFreeToCreate = true;

                // destroy the previous game if it does exist
                this.DestroyCarousel();

                // save the game object to release later in DestroyCarousel()
                this.m_CarouselGO = (GameObject)obj.Result;

                // change name clone -> addressable , useful in editor 
                this.m_CarouselGO.name = this.m_CarouselGO.name.ToString().Replace("(Clone)", "");

                // activate it 
                this.m_CarouselGO.SetActive(true);

                // set the parent as myself
                this.m_CarouselGO.transform.SetParent(this.transform);

                // LOGS:
                this.Logs
                (
                    "<b>" + this.m_RemoteCarousels[this.m_CarouselIndex] + "</b> created successfully",
                    "VRG_Core->Carousel_Completed()",
                    ENUM_Verbose.DEBUG
                );
            }

            // ... since not
            else
            {
                // LOGS: something is broken
                this.Logs
                (
                    this.m_RemoteCarousels[this.m_CarouselIndex] + " = " + obj.Status,
                    "VRG_Core->Carousel_Completed()",
                    ENUM_Verbose.ERROR
                );
            }
        }

        /// <summary>
        /// Public method Overload: Download a list of Addressable, every single Addressable decide if it is Instantiated
        /// </summary>
        /// <param name="valueLocal"></param>
        public static void Download(List<VRG_Addressable> valueLocal) => Instance.GetSize(valueLocal);

        /// <summary>
        /// Public method Overload: Download a singe Addressable by name, it is NOT Instantiated
        /// </summary>
        /// <param name="valueLocal"></param>
        public static void Download(string valueLocal) => Instance.GetSize(valueLocal, false);

        // Get the size of the addressables in the list valueLocal that will be downloaded
        private void GetSize(List<VRG_Addressable> valueLocal)
        {
            // if the list is not empty
            if (valueLocal.Count > 0)
            {
                // cycle the list for Addressables
                for (int i = 0; i < valueLocal.Count; i++)
                {
                    // try to get the size and save it in the handler
                    valueLocal[i].m_Size = Addressables.GetDownloadSizeAsync(valueLocal[i].m_Name);

                    // delegate to the .Completed and wait for the result in the GetAddressable
                    valueLocal[i].m_Size.Completed += Instance.GetAddressable;

                    // copy to the local Addressable list
                    Instance.m_Addressables.Add(valueLocal[i]);
                }
            }

            // if it is empty...
            else
            {
                // inform the logs
                this.Logs
                (
                    "Scene: <color=blue><i>" + SceneManager.GetActiveScene().name + "</i></color>  addressable list is empty",
                    "VRG_Core->GetSize()",
                    ENUM_Verbose.WARNING
                );


                // just close it to trigger events
                StartCoroutine(this.InstantiateAll());
            }
        }

        // Get the size of the asset that will be downloaded
        private void GetSize(string valueLocal, bool createLocal)
        {
            // if the valueLocal is not empty
            if (valueLocal.Trim() != "")
            {
                // save it in the logs
                this.Logs
                (
                    "<b>SIZING:</b> <color=green>" + valueLocal + "</color>",
                    "VRG_Core->GetSize()",
                    ENUM_Verbose.DEBUG
                );

                // get the download syze and save it in a handler
                AsyncOperationHandle<long> sizeHandle = Addressables.GetDownloadSizeAsync(valueLocal);

                // delegate to the .Completed and wait for the result in the GetAddressable
                sizeHandle.Completed += Instance.GetAddressable;

                // copy to the local Addressable list
                Instance.m_Addressables.Add(new VRG_Addressable(valueLocal, createLocal, sizeHandle));
            }

            // if it is empty...
            else
            {
                // log the warning
                this.Logs
                (
                    valueLocal + " asset name is empty",
                    "VRG_Core->GetSize()",
                    ENUM_Verbose.WARNING
                );
            }
        }

        // the Addressable has a size assigned, try to get it from Remote or Cache
        private void GetAddressable(AsyncOperationHandle<long> obj)
        {
            // log dialog
            string sLogs;

            // if the operation was valid
            if (obj.IsValid())
            {
                // ... and it was a success
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    // cycle the Addressable to know who is the owner of this call
                    foreach (VRG_Addressable child in this.m_Addressables)
                    {
                        // Am I the one?
                        if (obj.Equals(child.m_Size))
                        {
                            // if the size is 0, then it is already loaded in cache
                            if (obj.Result > 0)
                            {
                                sLogs = "<b>DOWNLOADING:</b> <color=green>" + child.m_Name + "</color> for " + child.m_Size.Result + " bytes";
                            }
                            else
                            {
                                sLogs = "<b>CACHE:</b> <color=green>" + child.m_Name + " </color>";
                            }

                            // inform the logs ...
                            this.Logs
                            (
                                sLogs,
                                "VRG_Core->GetAddressable()",
                                ENUM_Verbose.INFO
                            );

                            // download it save the handler in the child 
                            child.m_Progress = Addressables.DownloadDependenciesAsync(child.m_Name);

                            // since the download already started, inform the player of the progress
                            StartCoroutine(this.Progress());

                            // we found it, stop searching
                            break;
                        }
                    }
                }

                // ... if wasn't a success, something goes wrong
                else
                {
                    // inform the logs ...
                    this.Logs
                    (
                        "obj.Status = " + obj.Status,
                        "VRG_Core->GetAddressable()",
                        ENUM_Verbose.ERROR
                    );
                }
            }

            // ... if is not valid, something goes wrong
            else
            {
                // inform the logs ...
                this.Logs
                (
                    "OBJ Not Valid",
                    "VRG_Core->GetAddressable()",
                    ENUM_Verbose.ERROR
                );
            }
        }

        // Coruotine to inform the player of the progress with a nice loading bar
        private IEnumerator Progress()
        {
            // If it is already loading a download, well... it is already doing it
            if (!this.m_IsProgress)
            {
                // I started the progress monitoring
                this.m_IsProgress = true;

                // continue the next frame, and wait for any other Addressable that is coming
                yield return null;

                // Log dialog
                string sLogs;

                // total size of all the downloads in progress
                long lSize;

                // the current progress
                long lProgress;

                // stop the process when everything is fully downloaded
                bool bContinue = true;

                // if it not over... just keep it informing
                while (bContinue)
                {
                    // since we dont know if there are new downloads, lets calculate size
                    lSize = 0;

                    // I guess we are done
                    bContinue = false;

                    // cycle through the addressables
                    foreach (VRG_Addressable child in this.m_Addressables)
                    {
                        // just check for the childs that has a size succeess
                        if (child.m_Size.Status == AsyncOperationStatus.Succeeded)
                        {
                            // if the handler is valid
                            if (child.m_Progress.IsValid())
                            {
                                // and it is not done and under 100%
                                if (!child.m_Progress.IsDone || child.m_Progress.PercentComplete < 1)
                                {
                                    // we are not done
                                    bContinue = true;
                                }

                                // add to the total size my child size
                                lSize += child.m_Size.Result;
                            }

                            // I dont know, it's not ready, so lets wait
                            else
                            {
                                // ... maybe it is the same frame, lets try again
                                bContinue = true;
                            }
                        }
                    }

                    // I have a size, If i don't try again, or it is over
                    if (lSize > 0)
                    {
                        // my progress is zero as far as I know
                        lProgress = 0;

                        // reset the log dialog
                        sLogs = "";

                        // cycle the Addressables to know current progress
                        foreach (VRG_Addressable child in this.m_Addressables)
                        {
                            // if my handler is valid
                            if (child.m_Progress.IsValid())
                            {
                                // inform the logs
                                if (child.m_Progress.PercentComplete < 0.25f)
                                {
                                    sLogs += "<color=orange>";
                                }
                                else if (child.m_Progress.PercentComplete > 0.75f)
                                {
                                    sLogs += "<color=green>";
                                }
                                else
                                {
                                    sLogs += "<color=yellow>";
                                }
                                sLogs +=
                                    child.m_Name + " = size " + child.m_Size.Result
                                    + " at " + child.m_Progress.PercentComplete
                                    + "</color><br>";

                                // add to the total progress my own size and progress
                                lProgress += (long)(child.m_Size.Result * child.m_Progress.PercentComplete);
                            }
                        }

                        // register the logs what we are saving
                        this.Logs
                        (
                            sLogs,
                            "VRG_Core->Progress()",
                            ENUM_Verbose.DEBUG
                        );

                        // and update the UI slider 
                        VRG_Core.SetProgress((float)((float)lProgress / (float)lSize), (float)lSize);
                    }

                    // continue the next frame
                    yield return null;
                }

                // The progress monitoring is over
                this.m_IsProgress = false;

                // set the progress bar at 100% to make sure the comunication to the user is clear
                VRG_Core.SetProgress(1, 1, true);

                // Create the Instantiate Addressables
                StartCoroutine(this.InstantiateAll());

                // continue the next frame
                yield return null;
            }

            // continue the next frame
            yield return null;
        }

        /// <summary>
        /// overload function: Display in the slider the progress, this  resets the tries to 0
        /// </summary>
        /// <param name="valueLocal"></param>
        /// <param name="totalLocal"></param>
        /// <param name="progressTry"></param>
        public static void SetProgress(float valueLocal, float totalLocal, bool progressTry)
        {
            // reset the progress tries to 0
            Instance.m_ProgressTry = 0;

            // call the overloaded without progress tries
            VRG_Core.SetProgress(valueLocal, totalLocal);
        }

        /// <summary>
        /// overload function: Display in the slider the progress with valueLocal as percentage of totalLocal
        /// </summary>
        /// <param name="valueLocal"></param>
        /// <param name="totalLocal"></param>
        public static void SetProgress(float valueLocal, float totalLocal)
        {
            // Dialog log
            string sLogs;

            // decimal format floating point
            string sPuntosDecimal = "F2";

            // the byte unit
            string sMegasOrKs = " Mb";

            // the separator union
            string sUnion = " / ";

            // the detail of zoom of the display
            float fDownloadSize = totalLocal / 1000000;

            // if it is in not in Mbs format the local variables
            if (totalLocal < 1000000)
            {
                sPuntosDecimal = "F1";
                sMegasOrKs = " kb";
                fDownloadSize = totalLocal / 1000;
            }

            // if it is in not in kbs format the local variables
            if (totalLocal < 10000)
            {
                sPuntosDecimal = "F0";
                sMegasOrKs = " bytes";
                fDownloadSize = totalLocal;
            }

            // Ok, then singles
            if (totalLocal < 100)
            {
                sPuntosDecimal = "F0";
                sUnion = " of ";
                sMegasOrKs = "";
                fDownloadSize = totalLocal;
            }

            // if there is a size of download
            if (fDownloadSize > 0.0f)
            {
                // format the log
                // just a single download, the progress is direct
                if (totalLocal == 1)
                {
                    sLogs = ""
                        + (valueLocal * 100).ToString("F1") + "%";
                    if (valueLocal >= 1)
                    {
                        sLogs = "download complete";
                    }
                }

                // multiple items
                else
                {
                    sLogs = ""
                        + (fDownloadSize * valueLocal).ToString(sPuntosDecimal)
                        + sMegasOrKs
                        + sUnion
                        + fDownloadSize.ToString(sPuntosDecimal)
                        + sMegasOrKs;
                }

                // if the current value changed, if not, do not spam the logs
                if (valueLocal != Instance.m_UiSlider.value)
                {
                    // inform the logs
                    Instance.Logs
                    (
                        "<b>" + (valueLocal * 100).ToString("F1") + "% - " + sLogs + "</b>",
                        "VRG_Core->SetProgress(<b> " + Instance.m_ProgressTry + " </b>)",
                        ENUM_Verbose.DEBUG
                    );
                }
            }

            // there is no download, just cache 
            else
            {
                // ellipsis: animate 1 per frame
                string sElipsis = "";
                int iPuntitos = Instance.m_ProgressTry % 4;
                for (int i = 0; i < iPuntitos; i++)
                {
                    sElipsis += " .";
                }

                // inform the player
                VRG_Core.SetStatus("CACHE: " + Instance.m_Addressables.Count.ToString() + " assets " + sElipsis);
            }

            // if the download is over
            if (valueLocal >= 1)
            {
                // set slider to 100%
                Instance.m_UiSlider.value = 1;

                // ... and the text
                Instance.m_UiPercentage.text = "100 %";

                // ... and the wifi status
                Instance.m_UiInternetNetwork.text = Instance.m_ReachabilityText;
            }

            // if is still loading
            else
            {
                // set slider to current percentage
                Instance.m_UiSlider.value = valueLocal;

                // ... and the text
                Instance.m_UiPercentage.text = (valueLocal * 100).ToString("F1") + " %";

                // ... and the wifi status PLUS the progress tries
                Instance.m_UiInternetNetwork.text = (Instance.m_ProgressTry++).ToString() + " - " + Instance.m_ReachabilityText;
            }

        }

        // clear or create the instantiates 
        protected new IEnumerator InstantiateAll()
        {
            this.m_DelayBetweenDownloadsCurrent = this.m_DelayBetweenDownloads;
            if (this.m_Addressables.Count == 1)
            {
                if (this.m_Addressables[0].m_Size.Result < 0.1)
                {
                    this.m_DelayBetweenDownloadsCurrent = 0.0f;
                }
            }
            // If I am Instanstiating for the first time
            if (this.m_IsInstantiate)
            {
                // I will not do it again
                this.m_IsInstantiate = false;

                foreach (VRG_Addressable child in this.m_Addressables)
                {
                    child.m_Active = true;
                }

                // wait until my parent finish the instantiate all
                yield return base.InstantiateAll();
            }

            // then just clear the addressable local
            else
            {
                // clear
                this.m_Addressables.Clear();
            }

            // invoke the events delegateds
            OnDownload?.Invoke();

            // continue the next frame
            yield return null;
        }

        /// <summary>
        /// Set if you need to autoload when the scene is ready
        /// </summary>
        /// <param name="valueLocal"></param>
        public static void SetSceneAutoLoad(bool valueLocal)
        {
            Instance.m_SceneAutoLoad = valueLocal;
        }



        // Delegated function, it is called when the download of the Addressables is done
        private void OnDownload_Delegated()
        {
            // stop listening for this event, it is just for the first load
            VRG_Core.OnDownload -= OnDownload_Delegated;

            // will it load the scene as soon as it is ready becuae it is loaded from a scene?
            string sScenePreLoaded = VRG_Session.String("VRG_Init", "Scene");

            // if no scene preload
            if (sScenePreLoaded.Trim() == "")
            {
                if (this.m_RemoteScenes.Count > 0)
                {
                    // load the first scene
                    sScenePreLoaded = this.m_RemoteScenes[0];
                }
            }

            // Load the first scene in the remote settings
            VRG_Core.Scene(sScenePreLoaded);
        }

        /// <summary>
        /// Funtion that instantiate the valueLocal Scene
        /// </summary>
        /// <param name="valueLocal"></param>
        public static void Scene(string valueLocal)
        {
            // if am already loading a scene, ignore the second request
            if (!Instance.m_IsLoadingScene)
            {
                // well... now i am loading
                Instance.m_IsLoadingScene = true;

                // if properly inited
                if (Instance.m_Scene != null)
                {
                    // auto fade since it is a new scene
                    bool bSetAutoFade = true;

                    // if we just want to reload
                    if (valueLocal == "[RELOAD SCENE]")
                    {
                        // .. or not
                        bSetAutoFade = false;

                        // the name is current one
                        valueLocal = SceneManager.GetActiveScene().name;
                    }

                    // set the fade status
                    Instance.m_VRG_Fader.SetAutoFade(bSetAutoFade);

                    // If there is a scene already loaded and it is valid
                    if (Instance.m_Scene.m_SceneInstance.IsValid())
                    {
                        // also... unloading
                        Instance.m_IsUnloadingScene = true;

                        // Fade the scene black screen since we are hiding the previous scene
                        Instance.FadeTheScene();
                    }

                    // reset the current scene
                    Instance.m_Scene.Reset();

                    // the name is the one that was given
                    Instance.m_Scene.m_Name = valueLocal.Trim();

                    // just load if the scene is valid
                    if (Instance.m_Scene.m_Name != "")
                    {
                        // inform the logs
                        Instance.Logs
                        (
                            "<b>TRYING: </b> Scene: <color=blue><i>" + Instance.m_Scene.m_Name + "</i></color>",
                            "VRG_Core->Scene()",
                            ENUM_Verbose.LOGS
                        );

                        // cycle the carrousels to create the mini-buttons
                        foreach (Transform child in Instance.m_CarouselMenuBox.transform)
                        {
                            // ... and its number
                            child.GetComponent<VRG_CarouselButton>().IsItMine(Instance.m_CarouselIndex);
                        }

                        // if we are NOT unloading
                        if (!Instance.m_IsUnloadingScene)
                        {
                            // load the next scene ascynchronous and save the handler
                            Instance.m_Scene.m_SceneInstance = Addressables.LoadSceneAsync(Instance.m_Scene.m_Name);

                            // delegate and wait until it is Completed
                            Instance.m_Scene.m_SceneInstance.Completed += Instance.Scene_Completed;

                            // start the progress
                            Instance.StartCoroutine(Instance.ProgressScene());
                        }

                        // wait until everything is loaded and turn the button green and ready
                        Instance.StartCoroutine(Instance.ActivateSceneButton());
                    }
                    else
                    {
                        // inform the logs
                        Instance.Logs
                        (
                            "No scene defined, add a scene in VRG_Remote",
                            "VRG_Core->Scene()",
                            ENUM_Verbose.WARNING
                        );
                    }
                }
                else
                {
                    // inform the logs
                    Instance.Logs
                    (
                        "Scene object null, add a scene in VRG_Remote",
                        "VRG_Core->Scene()",
                        ENUM_Verbose.WARNING
                    );
                }
            }
        }

        // update the progress of the Scene Addressable
        private IEnumerator ProgressScene()
        {
            // init the progress Slider 
            VRG_Core.SetProgress(0, 1, true);

            // repeat until the scene is done
            while (!Instance.m_Scene.m_Progress.IsDone)
            {
                // update the progress by percent completed
                VRG_Core.SetProgress((float)Instance.m_Scene.m_SceneInstance.PercentComplete, 1);

                // continue next frame
                yield return null;
            }

            // set it at 100% to make sure it is correct to the player
            VRG_Core.SetProgress(1, 1);

            // finish next frame
            yield return null;
        }

        // delegated function that triggers when the scene is fully loaded
        private void Scene_Completed(AsyncOperationHandle<SceneInstance> obj)
        {
            // if the try was valid
            if (obj.IsValid())
            {
                // ... and successful
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    // save the scene for later reference
                    this.m_Scene.m_SceneInstance = obj;

                    // inform the logs
                    this.Logs
                    (
                        "Scene: <color=blue><i>" + this.m_Scene.m_Name + "</i></color> loaded in background",
                        "VRG_Core->Scene_Completed()",
                        ENUM_Verbose.INFO
                    );
                }

                // damn! we failed...
                else
                {
                    // inform the logs
                    this.Logs
                    (
                        "obj.Status(" + this.m_Scene.m_Name + ") = " + obj.Status,
                        "VRG_Core->Scene_Completed()",
                        ENUM_Verbose.ERROR
                    );
                }
            }

            // ok, we failed...
            else
            {
                // ... inform the logs
                this.Logs
                (
                    "<color=blue><i>" + this.m_Scene.m_Name + "</i></color> = OBJ Not Valid",
                    "VRG_Core->Scene_Completed()",
                    ENUM_Verbose.ERROR
                );
            }
        }

        /// <summary>
        /// Function that receive the notification that scene is ready itself
        /// </summary>
        public static void SceneLoaded() => Instance.StartCoroutine(Instance.SceneLoaded_IEnumerator());

        // The inumerator of the SceneLoaded() function
        private IEnumerator SceneLoaded_IEnumerator()
        {
            // wait until the scene Addressable is Valid
            while (!this.m_Scene.m_SceneInstance.IsValid())
            {
                // wait next frame
                yield return null;
            }

            // while statu does not Succeeded ... wait
            while (this.m_Scene.m_SceneInstance.Status != AsyncOperationStatus.Succeeded)
            {
                // wait next frame
                yield return null;
            }

            // inform logs
            this.Logs
            (
                "Scene: <color=blue><i>" + this.m_Scene.m_Name + "</i></color> Loaded, press start to continue",
                "VRG_Core->SceneLoaded()",
                ENUM_Verbose.INFO
            );

            // the flag of the scene Adddressable is true
            this.m_Scene.m_Create = true;

            // if we are not unloading
            if (!Instance.m_IsUnloadingScene)
            {
                // ... then not loading either
                this.m_IsLoadingScene = false;
            }

            // finish next frame
            yield return null;
        }

        // function that fade the scene
        private void FadeTheScene()
        {
            // just do it if is not already fading
            if (!this.m_IsFading)
            {
                // ... now i am fading
                this.m_IsFading = true;

                // activate the box object that hold the fader
                this.m_VRG_Fader.Fade(this.m_IsFading);
            }
        }

        // The fading in is finished the alpha is 1.0
        private void FadeTheScene_Medium()
        {
            // if I am fading
            if (this.m_IsFading)
            {
                // in case it is unloading or loading, it displays or hides the UI 
                Instance.UI_Activate(Instance.m_IsUnloadingScene);

                // if we are unloading
                if (Instance.m_IsUnloadingScene)
                {
                    // Inform all the delegateds that the current scene is about to get unloaded
                    OnSceneUnload?.Invoke();

                    // since we don't need the camera because the scene is active, just turn camera off
                    Instance.m_CameraBox.SetActive(true);

                    // load the next scene ascynchronous and save the handler
                    Instance.m_Scene.m_SceneInstance = Addressables.LoadSceneAsync(Instance.m_Scene.m_Name);

                    // delegate and wait until it is Completed
                    Instance.m_Scene.m_SceneInstance.Completed += Instance.Scene_Completed;

                    // start the progress
                    Instance.StartCoroutine(Instance.ProgressScene());

                    // if it needs to continue because the fade is on
                    if (!Instance.m_VRG_Fader.inOut)
                    {
                        Instance.FadeTheScene_End();
                    }
                }

                // if not 
                else
                {
                    // the autoload is off by default
                    Instance.m_SceneAutoLoad = false;

                    // trigger delegates on scene activated
                    OnSceneActivated?.Invoke();

                    // if it needs to continue because the fade is on
                    if (!Instance.m_VRG_Fader.inOut)
                    {
                        // activate the box object that hold the fader
                        Instance.m_VRG_Fader.Fade(false);
                    }
                }
            }
        }

        // the fading finished the alpha = 0.0f
        private void FadeTheScene_End()
        {
            // just do it if it is already fading ... 
            if (this.m_IsFading)
            {
                // ... we are done
                this.m_IsFading = false;

                // ... not loading
                this.m_IsLoadingScene = false;

                // ... and not unloading
                this.m_IsUnloadingScene = false;
            }
        }

        // The button is now ready, green and interactable
        private IEnumerator ActivateSceneButton()
        {
            // inform logs
            this.Logs
            (
                "Scene: <color=blue><i>" + this.m_Scene.m_Name + "</i></color> Carousel to finish",
                "VRG_Core->ActivateSceneButton()",
                ENUM_Verbose.STATUS
            );

            // wait until it is fully created
            while (!this.m_Scene.m_Create || this.m_IsLoadingScene)
            {
                // wait until next frame
                yield return null;
            }

            // interactable
            this.m_UiButton.GetComponent<Button>().interactable = true;

            // green
            this.m_UiButton.color = new Color32(155, 230, 95, 255);

            // label it is ok
            this.m_UiPercentage.text = this.m_UiButtonText;

            // if no scene preload
            if (VRG_Session.String("VRG_Init", "Scene").Trim() != "")
            {
                // send it inmediatly
                this.m_SceneAutoLoad = true;

                // reset it from the session
                VRG_Session.String("VRG_Init", "Scene", "");
            }


            // if it needs to continue because the fade is on
            if (!Instance.m_VRG_Fader.inOut)
            {
                // send it inmediatly
                this.m_SceneAutoLoad = true;
            }

            // if autoload, then proceed to the scen directly
            if (this.m_SceneAutoLoad)
            {
                VRG_Core.CheckScene();
            }

            // always return to the one designed by the program
            this.m_SceneAutoLoad = false;

            // inform logs
            this.Logs
            (
                "Scene: <color=blue><i>" + this.m_Scene.m_Name + "</i></color> button Activated",
                "VRG_Core->ActivateSceneButton()",
                ENUM_Verbose.DEBUG
            );

            // check for new updates
            //Addressables.CheckForCatalogUpdates().Completed += CheckForCatalogUpdates;

            // finish the next frame
            yield return null;
        }

        // the download data not needed right now
        private void PreloadAssets()
        {
            // the autoload is off by default
            Instance.m_SceneAutoLoad = false;

            // just do it when we are done and we don't need more to start a scene
            if (!this.m_IsLoadingScene)
            {
                // if tthere are stil some assets to preload
                if (this.m_DownloadIndex < this.m_RemoteDownload.Count)
                {
                    // Listener to the event OnDownload, when the whole package is downloaded
                    VRG_Core.OnDownload += OnDownload_PreloadAssets;

                    this.Logs
                    (
                        "<b>PRELOAD:</b> <color=green>" + this.m_RemoteDownload[this.m_DownloadIndex] + "</color> ("
                        + this.m_DownloadIndex + " / " + this.m_RemoteDownload.Count + ")",
                        "VRG_Core->PreloadAssets()",
                        ENUM_Verbose.DEBUG
                    );

                    // download, but do not instantiate
                    VRG_Core.Download(this.m_RemoteDownload[this.m_DownloadIndex]);

                    // the next time, get the next item
                    this.m_DownloadIndex++;
                }
                else
                {
                    this.Logs
                    (
                        "<b>PRELOAD FINISHED</b>",
                        "VRG_Core->PreloadAssets()",
                        ENUM_Verbose.LOGS
                    );

                    // remove to the event OnSceneActivated
                    VRG_Core.OnSceneActivated -= PreloadAssets;
                }
            }
        }

        // the download data not needed right now
        private void OnDownload_PreloadAssets()
        {
            // Listener to the event OnDownload, when the whole package is downloaded
            VRG_Core.OnDownload -= OnDownload_PreloadAssets;

            // wait until it is over
            StartCoroutine(this.OnDownload_PreloadAssets_IEnumerator());
        }

        // the download data not needed right now
        private IEnumerator OnDownload_PreloadAssets_IEnumerator()
        {
            // wait the delayed time setting 
            yield return new WaitForSeconds(this.m_DelayBetweenDownloadsCurrent);

            // try again the next item
            this.PreloadAssets();

            // close at next frame
            yield return null;
        }



        /* TODO: integrate the addressable default catalog
        // download addressable catalog
        private void CheckForCatalogUpdates(AsyncOperationHandle<List<string>> obj)
        {
            // dialog logs
            string sLogs = "Catalogo Nuevo <br>";

            // if the catalog is valid
            if (obj.IsValid())
            {
                // ... and it was a success
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    // save the remote catalog
                    this.m_RemoteCatalog = obj.Result;

                    // save the logs of the new items and inform the logs
                    sLogs += "<b> * List childs: * </b><br><br>" + this.m_RemoteCatalog + "<br><br>";
                    foreach (string child in this.m_RemoteCatalog)
                    {
                        sLogs += " - " + child.ToString() + "<br>";
                    }

                    this.Logs
                    (
                        sLogs,
                        "VRG_Core->CheckForCatalogUpdates()",
                        ENUM_Verbose.DEBUG
                    );
                }

                // if it wasn't
                else
                {
                    // inform the logs
                    this.Logs
                    (
                        "obj.Status(" + this.m_Scene.m_Name + ") = " + obj.Status,
                        "VRG_Core->CheckForCatalogUpdates()",
                        ENUM_Verbose.DEBUG
                    );
                }
            }

            // if it wasn't
            else
            {
                // inform the logs
                this.Logs
                (
                    "OBJ Not Valid",
                    "VRG_Core->CheckForCatalogUpdates()",
                    ENUM_Verbose.DEBUG
                );
            }
        }
        */

    }
}