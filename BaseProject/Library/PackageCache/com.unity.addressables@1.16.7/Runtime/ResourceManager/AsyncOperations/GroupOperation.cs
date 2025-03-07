using System;
using System.Collections.Generic;
using System.Security;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
    class GroupOperation : AsyncOperationBase<IList<AsyncOperationHandle>>, ICachable
    {
        Action<AsyncOperationHandle> m_InternalOnComplete;
        int m_LoadedCount;
        bool m_ReleaseDependenciesOnFailure = true;
        string debugName = null;
        private const int k_MaxDisplayedLocationLength = 45;

        public GroupOperation()
        {
            m_InternalOnComplete = OnOperationCompleted;
            Result = new List<AsyncOperationHandle>();
        }

        int ICachable.Hash { get; set; }

        internal IList<AsyncOperationHandle> GetDependentOps()
        {
            return Result;
        }

        protected override void GetDependencies(List<AsyncOperationHandle> deps)
        {
            deps.AddRange(Result);
        }

        internal override void ReleaseDependencies()
        {
            for (int i = 0; i < Result.Count; i++)
                if (Result[i].IsValid())
                    Result[i].Release();
            Result.Clear();
        }

        internal override DownloadStatus GetDownloadStatus(HashSet<object> visited)
        {
            var status = new DownloadStatus() { IsDone = IsDone };
            for (int i = 0; i < Result.Count; i++)
            {
                if (Result[i].IsValid())
                {
                    var depStatus = Result[i].InternalGetDownloadStatus(visited);
                    status.DownloadedBytes += depStatus.DownloadedBytes;
                    status.TotalBytes += depStatus.TotalBytes;
                }
            }
            return status;
        }

        HashSet<string> m_CachedDependencyLocations = new HashSet<string>();

        private bool DependenciesAreUnchanged(List<AsyncOperationHandle> deps)
        {
            if (m_CachedDependencyLocations.Count != deps.Count) return false;
            foreach (var d in deps)
                if (!m_CachedDependencyLocations.Contains(d.LocationName))
                    return false;
            return true;
        }
        
        protected override string DebugName
        {
            get
            {
                List<AsyncOperationHandle> deps = new List<AsyncOperationHandle>();
                GetDependencies(deps);

                if (deps.Count == 0)
                    return "Dependencies";
                
                //Only recalculate DebugName if a name hasn't been generated for currently held dependencies
                if (debugName != null && DependenciesAreUnchanged(deps))
                    return debugName;
                
                m_CachedDependencyLocations.Clear();
                
                string toBeDisplayed = "Dependencies [";
                for (var i = 0; i < deps.Count; i++)
                {
                    var d = deps[i];
                    
                    var locationString = d.LocationName;
                    m_CachedDependencyLocations.Add(locationString);

                    if (locationString == null) 
                        continue;

                    //Prevent location display from being excessively long
                    if (locationString.Length > k_MaxDisplayedLocationLength)
                    {
                        locationString = AsyncOperationBase<object>.ShortenPath(locationString, true);
                        locationString = locationString.Substring(0, Math.Min(k_MaxDisplayedLocationLength, locationString.Length)) + "...";
                    }

                    if (i == deps.Count - 1)
                        toBeDisplayed += locationString;
                    else
                        toBeDisplayed += locationString + ", ";
                }
                
                toBeDisplayed += "]";
                
                debugName = toBeDisplayed;

                return debugName;
            }

        }

        protected override void Execute()
        {
            m_LoadedCount = 0;
            for (int i = 0; i < Result.Count; i++)
            {
                if (Result[i].IsDone)
                    m_LoadedCount++;
                else
                    Result[i].Completed += m_InternalOnComplete;
            }
            CompleteIfDependenciesComplete();
        }

        private void CompleteIfDependenciesComplete()
        {
            if (m_LoadedCount == Result.Count)
            {
                bool success = true;
                string errorMsg = string.Empty;
                for (int i = 0; i < Result.Count; i++)
                {
                    if (Result[i].Status != AsyncOperationStatus.Succeeded)
                    {
                        success = false;
                        errorMsg = Result[i].OperationException != null ? Result[i].OperationException.Message : string.Empty;
                        break;
                    }
                }
                Complete(Result, success, errorMsg, m_ReleaseDependenciesOnFailure);
            }
        }

        protected override void Destroy()
        {
            ReleaseDependencies();
        }

        protected override float Progress
        {
            get
            {
                float total = 0f;
                for (int i = 0; i < Result.Count; i++)
                {
                    var handle = Result[i];
                    if (!handle.IsDone)
                        total += handle.PercentComplete;
                    else
                        total++;
                }

                return total / Result.Count;
            }
        }


        public void Init(List<AsyncOperationHandle> operations, bool releaseDependenciesOnFailure = true)
        {
            Result = new List<AsyncOperationHandle>(operations);
            m_ReleaseDependenciesOnFailure = releaseDependenciesOnFailure;
        }

        void OnOperationCompleted(AsyncOperationHandle op)
        {
            m_LoadedCount++;
            CompleteIfDependenciesComplete();
        }
    }
}
