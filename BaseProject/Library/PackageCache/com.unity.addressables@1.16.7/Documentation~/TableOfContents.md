* [Unity Addressable Asset system](index.md)
    * [Addressable Assets overview](AddressableAssetsOverview.md)
        * [Concepts](AddressableAssetsOverview.md#concepts)
    * [Getting started](AddressableAssetsGettingStarted.md)
        * [Installing the Addressable Assets package](AddressableAssetsGettingStarted.md#installing-the-addressable-assets-package)
        * [Preparing Addressable Assets](AddressableAssetsGettingStarted.md#preparing-addressable-assets)
        * [Using Addressable Assets](AddressableAssetsGettingStarted.md#using-addressable-assets)
        * [Build considerations](AddressableAssetsGettingStarted.md#build-considerations)
    * [Addressable Assets development cycle](AddressableAssetsDevelopmentCycle.md)
        * [Traditional asset management](AddressableAssetsDevelopmentCycle.md#traditional-asset-management)
        * [Addressable Asset management](AddressableAssetsDevelopmentCycle.md#addressable-asset-management)
        * [Build scripts](AddressableAssetsDevelopmentCycle.md#build-scripts)
        * [Analysis and debugging](AddressableAssetsDevelopmentCycle.md#analysis-and-debugging)
        * [Initialization objects](AddressableAssetsDevelopmentCycle.md#initialization-objects)
        * [Customizing URL Evaluation](AddressableAssetsDevelopmentCycle.md#customizing-url-evaluation)
        * [Content update workflow](AddressableAssetsDevelopmentCycle.md#content-update-workflow)
    * [Content update workflow](ContentUpdateWorkflow.md)
        * [Project structure](ContentUpdateWorkflow.md#project-structure)
        * [How it works](ContentUpdateWorkflow.md#how-it-works)
        * [Planning for content updates](ContentUpdateWorkflow.md#planning-for-content-updates)
        * [Identifying changed assets](ContentUpdateWorkflow.md#identifying-changed-assets)
        * [Building for content updates](ContentUpdateWorkflow.md#building-for-content-updates)
        * [Checking for content updates at runtime](ContentUpdateWorkflow.md#checking-for-content-updates-at-runtime)
        * [Content update examples](ContentUpdateWorkflow.md#content-update-examples)
    * [Addressable Assets profiles](AddressableAssetsProfiles.md)
        * [Profile Setup](AddressableAssetsProfiles.md#profile-setup)
        * [Specifying packing and loading paths](AddressableAssetsProfiles.md#specifying-packing-and-loading-paths)
        * [Examples](AddressableAssetsProfiles.md#examples)
    * [Asset Hosting Services](AddressableAssetsHostingServices.md)
        * [Overview](AddressableAssetsHostingServices.md#overview)
        * [Setup](AddressableAssetsHostingServices.md#setup)
        * [Custom services](AddressableAssetsHostingServices.md#custom-services)
    * [Addressable Asset System with Cloud Content Delivery](AddressablesCCD.md)
    * [Memory management](MemoryManagement.md)
        * [Mirroring load and unload](MemoryManagement.md#mirroring-load-and-unload)
        * [The Addressables Event Viewer](MemoryManagement.md#the-addressables-event-viewer)
        * [When is memory cleared?](MemoryManagement.md#when-is-memory-cleared)
    * [Async operation handling](AddressableAssetsAsyncOperationHandle.md)
        * [Type vs. typeless handles](AddressableAssetsAsyncOperationHandle.md#type-vs-typeless-handles)
        * [AsyncOperationHandle use case examples](AddressableAssetsAsyncOperationHandle.md#asyncoperationhandle-use-case-examples)
    * [Custom operations](AddressableAssetsCustomOperation.md)
        * [Creating custom operations](AddressableAssetsCustomOperation.md#creating-custom-operations)
    * [Upgrading to the Addressables system](AddressableAssetsMigrationGuide.md)
       * [The direct reference method](AddressableAssetsMigrationGuide.md#the-direct-reference-method)
       * [The Resource folders method](AddressableAssetsMigrationGuide.md#the-resource-folders-method)
       * [The AssetBundles method](AddressableAssetsMigrationGuide.md#the-assetbundles-method)
    * [Expanded API documentation](AddressablesAPI.md)
        * [LoadingAddressableAssets](LoadingAddressableAssets.md)
        * [InitializeAsync](InitializeAsync.md)
        * [TransformInternalId](TransformInternalId.md)
        * [InstantiateAsync](InstantiateAsync.md)
        * [DownloadDependenciesAsync](DownloadDependenciesAsync.md)
        * [LoadContentCatalogAsync](LoadContentCatalogAsync.md)
        * [UpdateCatalogs](UpdateCatalogs.md)
        * [LoadSceneAsync](LoadSceneAsync.md)
        * [ExceptionHandler](ExceptionHandler.md)
        * [BuildPlayerContent](BuildPlayerContent.md)
        * [LoadResourceLocationsAsync](LoadResourceLocations.md)
    * [Diagnostic Tools](DiagnosticTools.md)
        * [Build Layout](DiagnosticTools.md#build-layout-report)
        * [Build Profiling](DiagnosticTools.md#build-profiling)
        * [The Addressables Analyze tool](DiagnosticTools.md#the-addressables-analyze-tool)
            * [Using Analyze](DiagnosticTools.md#using-analyze)
            * [Provided Analyze rules](DiagnosticTools.md#provided-analyze-rules)
            * [Extending Analyze](DiagnosticTools.md#extending-analyze)
