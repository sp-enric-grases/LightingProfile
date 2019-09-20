using System.Collections.Generic;
using UnityEngine;

namespace SocialPoint.Art.LightingProfiles
{
    public class LightingVolumeManager
    {
        static LightingVolumeManager Instance;

        public static LightingVolumeManager instance
        {
            get
            {
                if (Instance == null)
                {
                    Debug.Log("Creating singleton -> LightingVolumeManager");
                    Instance = new LightingVolumeManager();
                }
                return Instance;
            }
        }

        readonly List<LightingVolume> volumes;
        readonly List<Collider> colliders;
        private LightingProfile tempLightingProfile;
        bool sortingIsNeeded;

        LightingVolumeManager()
        {
            volumes = new List<LightingVolume>();
            //colliders = new List<Collider>(5);
            colliders = new List<Collider>();
            tempLightingProfile = ScriptableObject.CreateInstance<LightingProfile>();
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

        internal void SetDirty()
        {
            sortingIsNeeded = true;
        }

        internal void Register(LightingVolume volume)
        {
            volumes.Add(volume);
            SetDirty();
        }

        internal void Unregister(LightingVolume volume)
        {
            volumes.Remove(volume);
            SetDirty();
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
