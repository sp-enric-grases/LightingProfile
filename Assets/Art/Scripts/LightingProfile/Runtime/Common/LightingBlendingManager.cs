using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SocialPoint.Art.LightingProfiles
{
    public class LightingBlendingManager
    {
        static LightingBlendingManager Instance;

        public static LightingBlendingManager instance
        {
            get
            {
                if (Instance == null)
                {
                    Debug.Log("Creating singleton -> Lighting Blending Manager");
                    Instance = new LightingBlendingManager();
                }
                return Instance;
            }
        }

        private List<LightingVolume> volumes;
        private static LightingProfile initProfile;
        private LightingProfile tempLightingProfile;
        private LightingProfile currentLightingProfile;
        private LightingProfile sourceLightingProfile;
        private float blend;

        bool sortingIsNeeded;

        LightingBlendingManager()
        {
            volumes = new List<LightingVolume>();
            tempLightingProfile = ScriptableObject.CreateInstance<LightingProfile>();
        }

        /// <summary>
        /// Returns all valid blend volumes from the scene
        /// </summary>
        /// <returns></returns>
        public List<LightingVolume> GetListOfBlendVolumes()
        {
            if (ListOfVolumesIsEmpty()) return null;

            List<LightingVolume> blendVolumes = new List<LightingVolume>();

            for (int i = 0; i < volumes.Count; i++)
            {
                if (!volumes[i].isGlobal && volumes[i].profile != null)
                    blendVolumes.Add(volumes[i]);
            }

            if (blendVolumes.Count > 0)
                return blendVolumes;
            else
            {
                Debug.LogWarning("No blend volumes found in the scene.");
                return null;
            }
                
        }

        /// <summary>
        /// Returns the default global lighting profile when executing the first time.
        /// In case it finds more than one profiles with the same priority, returns the last one.
        /// </summary>
        /// <returns></returns>
        public LightingProfile GetDefaultGlobalProfileLighting()
        {
            if (ListOfVolumesIsEmpty()) return null;

            List<LightingVolume> globalVolumes = new List<LightingVolume>();

            for (int i = 0; i < volumes.Count; i++)
            {
                if (volumes[i].isGlobal && volumes[i].profile != null)
                    globalVolumes.Add(volumes[i]);
            }

            if (globalVolumes.Count > 0)
                return globalVolumes.OrderBy(p => p.priority).Last().profile;
            else
            {
                Debug.LogWarning("No global volumes found in the scene.");
                return null;
            }
        }

        /// <summary>
        /// Return a warning if there are no volume lighting in the scene
        /// </summary>
        /// <returns></returns>
        private bool ListOfVolumesIsEmpty()
        {
            if (volumes.Count == 0)
            {
                Debug.LogWarning("There are no volume lightings in the scene.");
                return true;
            }

            return false;
        }

        public LightingVolume GetHighestPriorityVolume()
        {
            float highestPriority = float.NegativeInfinity;
            LightingVolume output = null;

            foreach (var volume in volumes)
            {
                if (volume.priority > highestPriority)
                {
                    highestPriority = volume.priority;
                    output = volume;
                }
            }

            return output;
        }



        internal LightingProfile GetGlobalLighting()
        {
            tempLightingProfile = GetDefaultGlobalProfileLighting();

            if (tempLightingProfile == null)
            {
                Debug.Log("No Global profile found. TEMP profile is now the initial profile");
                tempLightingProfile = initProfile;
            }

            Debug.Log("Applying TEMP profile");
            tempLightingProfile.Apply();
            return tempLightingProfile;
        }

        internal void SetDirty(bool isGlobal)
        {
            sortingIsNeeded = true;
        }



        internal void UpdateLightingSettings(Vector3 worldPosition, bool switchSkybox, bool useEnvLighting, bool useEnvReflection, bool useMixedLighting, bool useFog, bool useHalo, bool showDebugLines)
        {
            volumes = GetListOfBlendVolumes();

            foreach (var v in volumes)
            {
                Vector3 blendClosestPoint = v.blendCollider.ClosestPoint(worldPosition);
                if (showDebugLines) Debug.DrawLine(worldPosition, blendClosestPoint, Color.red);

                if ((blendClosestPoint - worldPosition).magnitude > 0)
                {
                    Vector3 volumeClosestPoint = v.boxCollider.ClosestPoint(blendClosestPoint);
                    if (showDebugLines) Debug.DrawLine(blendClosestPoint, volumeClosestPoint, Color.green);
                }
                else
                {
                    Vector3 volumeClosestPoint = v.boxCollider.ClosestPoint(blendClosestPoint);
                    if (showDebugLines) Debug.DrawLine(worldPosition, volumeClosestPoint, Color.yellow);

                    blend = Mathf.Clamp01((volumeClosestPoint - worldPosition).magnitude / v.blendDistance);
                    //tempLightingProfile.Lerp(v.profile, blend, switchSkybox, useEnvLighting, useEnvReflection, useMixedLighting, useFog, useHalo);
                    //tempLightingProfile.Apply();
                }
            }
        }

        //internal LightingProfile GetSourceProfile()
        //{

        //}

        internal float GetBlendValue()
        {
            return blend;
        }

        //internal void UpdateSettings(Vector3 worldPosition, bool onlyGlobal, bool affectSkybox = false, bool interpolateAmbient = true, bool interpolateReflection = true, bool interpolateFog = true)
        //{
        //    // Sort the cached volume list if needed and return it
        //    List<LightingVolume> allVolumes = GetVolumesSortedByPriority();

        //    if (allVolumes.Count == 0)
        //        return;

        //    // Traverse all volumes and lerp them.
        //    for (int volumeIndex = 0; volumeIndex < allVolumes.Count; volumeIndex++)
        //    {
        //        LightingVolume volume = allVolumes[volumeIndex];

        //        // Skip disabled volumes and volumes without any data or weight
        //        if (!volume.enabled || volume.profileRef == null || volume.weight <= 0f)
        //            continue;

        //        // Global volume always have influence
        //        if (volume.isGlobal)
        //        {
        //            tempLightingProfile.Lerp(volume.profile, volume.weight, affectSkybox, interpolateAmbient, interpolateReflection, interpolateFog);
        //            continue;
        //        }

        //        if (onlyGlobal)
        //            continue;

        //        // If volume isn't global and has no collider, skip it as it's useless
        //        var colliders = this.colliders;
        //        volume.GetComponents(colliders);
        //        if (colliders.Count == 0)
        //        {
        //            Debug.LogWarning("Found a LightingVolume that is not global and has no collider!", volume);
        //            continue;
        //        }

        //        // Find closest distance to volume, 0 means it's inside it
        //        float closestDistanceSqr = float.PositiveInfinity;

        //        foreach (var collider in colliders)
        //        {
        //            if (!collider.enabled)
        //                continue;

        //            var closestPoint = collider.ClosestPoint(worldPosition); // 5.6-only API
        //            var d = ((closestPoint - worldPosition) / 2f).sqrMagnitude;

        //            if (d < closestDistanceSqr)
        //                closestDistanceSqr = d;
        //        }

        //        colliders.Clear();


        //        float blendDistSqr = volume.blendDistance * volume.blendDistance;

        //        // Volume has no influence, ignore it
        //        // Note: Volume doesn't do anything when `closestDistanceSqr = blendDistSqr` but
        //        //       we can't use a >= comparison as blendDistSqr could be set to 0 in which
        //        //       case volume would have total influence
        //        if (closestDistanceSqr > blendDistSqr)
        //            continue;

        //        // Volume has influence
        //        float interpFactor = 1f;

        //        if (blendDistSqr > 0f)
        //            interpFactor = 1f - (closestDistanceSqr / blendDistSqr);

        //        // No need to clamp01 the interpolation factor as it'll always be in [0;1] range
        //        tempLightingProfile.Lerp(volume.profile, interpFactor, affectSkybox, interpolateAmbient, interpolateReflection, interpolateFog);
        //    } // end volumes loop

        //    // Finally, this line is what actually applies the lighting.
        //    tempLightingProfile.Apply();
        //}

        List<LightingVolume> GetVolumesSortedByPriority()
        {
            if (sortingIsNeeded)
            {
                sortingIsNeeded = false;
                SortByPriorityAscending(volumes);
            }

            return volumes;
        }

        // Custom insertion sort. First sort will be slower but after that it'll be faster than
        // using List<T>.Sort() which is also unstable by nature.
        // Sort order is ascending.
        static void SortByPriorityAscending(List<LightingVolume> volumes)
        {
            for (int i = 1; i < volumes.Count; i++)
            {
                var temp = volumes[i];
                int j = i - 1;

                while (j >= 0 && volumes[j].priority > temp.priority)
                {
                    volumes[j + 1] = volumes[j];
                    j--;
                }

                volumes[j + 1] = temp;
            }
        }

        
    }
}