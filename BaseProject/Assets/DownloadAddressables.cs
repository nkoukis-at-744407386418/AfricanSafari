using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;


public class DownloadAddressables : MonoBehaviour
{
    public Image progressBarUI;
    //please make sure to change this path
    private string secondCatalogPath = "D:\\Documents\\Unity Projects\\IMREAL\\T3 Addressable Shell Desktop\\Server\\StandaloneWindows64\\catalog_TBC.json";

    // Start is called before the first frame update
    void Start()
    {
       
        StartCoroutine(LoadSecondCatalog());
    }
    private IEnumerator LoadSecondCatalog()
    {
        AsyncOperationHandle<IResourceLocator> handle = Addressables.LoadContentCatalogAsync(secondCatalogPath, true);

        while(!handle.IsDone)
        {
            if(progressBarUI != null)
            {
                progressBarUI.fillAmount = handle.PercentComplete;
            }
            yield return null;
        }
        //please make sure to change this UUID
        Addressables.LoadSceneAsync("To_Be_Changed_UUID", LoadSceneMode.Single);

    }
}
