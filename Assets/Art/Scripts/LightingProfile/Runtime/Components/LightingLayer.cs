using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SocialPoint.Art.LightingProfiles
{
    [AddComponentMenu("Rendering/Lighting Layer", -10)]
    [DisallowMultipleComponent]
    public class LightingLayer : MonoBehaviour
    {
        #region VARIABLES
        public static LightingLayer Instance;

        public bool blendAllSettings = true;
        public bool switchSkybox = true;
        public bool useEnvLighting = true;
        public bool useEnvReflection = true;
        public bool useMixedLighting = true;
        public bool useFog = true;
        public bool useHalo = true;
        public int frameSkip = 0;
        public string desiredProfileName;
        public string currentProfileName;
        public float blend = 0;
        public bool showDebugLines = false;
        public List<LightingVolume> volumes;

        private LightingProfile initialProfile;
        private LightingProfile temporalProfile;
        private LightingProfile currentProfile;
        private LightingProfile desireProfile;

        private float blendTime = 0;
        private AnimationCurve curve;
        private float counter = 0;
        private bool hasToFade = false;
        private Coroutine checkVolumes;

        #endregion

        private void Awake()
        {
            Instance = this;

            SetProfiles();

            if (blendAllSettings)
                switchSkybox = useEnvLighting = useEnvReflection = useMixedLighting = useFog = useHalo = true;
        }

        internal void SetProfiles()
        {
            Debug.Log("Copying init profile settings");
            initialProfile = ScriptableObject.CreateInstance<LightingProfile>();
            initialProfile.CopyFromCurrentScene();

            currentProfile = ScriptableObject.CreateInstance<LightingProfile>();
            temporalProfile = ScriptableObject.CreateInstance<LightingProfile>();
            currentProfile.CopyFromCurrentScene();
            temporalProfile.CopyFromCurrentScene();
        }

        void LateUpdate()
        {
            CheckPosition();
            //FadeGlobalSettings();
            //FadeLocalVolume();
            //LightingBlendingManager.instance.UpdateLightingSettings(transform.position, switchSkybox, useEnvLighting, useEnvReflection, useMixedLighting, useFog, useHalo, showDebugLines);
        }

        private void CheckPosition()
        {
            foreach (var v in volumes)
            {
                if (v.isGlobal) continue;

                Vector3 pos = transform.position;
                Vector3 blendClosestPoint = v.blendCol.ClosestPoint(pos);

                if (showDebugLines) Debug.DrawLine(pos, blendClosestPoint, Color.red);

                if ((blendClosestPoint - pos).magnitude > 0)
                {
                    Vector3 volumeClosestPoint = v.boxCol.ClosestPoint(blendClosestPoint);
                    if (showDebugLines) Debug.DrawLine(blendClosestPoint, volumeClosestPoint, Color.green);
                }
                //else
                //{
                //    Vector3 volumeClosestPoint = v.boxCol.ClosestPoint(blendClosestPoint);
                //    if (showDebugLines) Debug.DrawLine(pos, volumeClosestPoint, Color.yellow);
                //}
            }
        }

        #region GLOBAL PROFILES

        private void FadeGlobalSettings()
        {
            if (!hasToFade) return;

            counter += Time.deltaTime / blendTime;

            if (GetFrameSkip())
            {
                blend = curve.Evaluate(counter);
                temporalProfile.Lerp(currentProfile, desireProfile, blend, switchSkybox, useEnvLighting, useEnvReflection, useMixedLighting, useFog, useHalo);
                temporalProfile.ApplyRenderSettings();

                if (counter > 1)
                    ResetTemporalProfile();
            }
        }

        public void GetHigherGlobalProfile()
        {
            if (ListOfVolumesIsEmpty()) return;

            List<LightingVolume> globalVolumes = new List<LightingVolume>();

            for (int i = 0; i < volumes.Count; i++)
            {
                if (volumes[i].isGlobal && volumes[i].profile != null)
                    globalVolumes.Add(volumes[i]);
            }

            if (globalVolumes.Count > 0)
            {
                LightingVolume v = globalVolumes.OrderBy(p => p.priority).Last();

                desireProfile = v.profile;
                desiredProfileName = v.profile.name;
                blendTime = v.timeToBlend;
                curve = v.timeCurve;
                hasToFade = true;
            }
            else
            {
                Debug.LogWarning("No global volumes found in the scene.");
                hasToFade = false;
            }
        }

        #endregion

        #region LOCAL PROFILES

        private void FadeLocalVolume()
        {
            foreach (var v in volumes)
            {
                Vector3 pos = v.transform.position;
                Vector3 blendClosestPoint = v.blendCol.ClosestPoint(pos);

                if (showDebugLines) Debug.DrawLine(pos, blendClosestPoint, Color.red);

                if ((blendClosestPoint - pos).magnitude > 0)
                {
                    Vector3 volumeClosestPoint = v.boxCol.ClosestPoint(blendClosestPoint);
                    if (showDebugLines) Debug.DrawLine(blendClosestPoint, volumeClosestPoint, Color.green);
                }
                else
                {
                    Vector3 volumeClosestPoint = v.boxCol.ClosestPoint(blendClosestPoint);
                    if (showDebugLines) Debug.DrawLine(pos, volumeClosestPoint, Color.yellow);

                    //blend = Mathf.Clamp01((volumeClosestPoint - pos).magnitude / v.blendDist);
                }
            }
            //internal void UpdateLightingSettings(Vector3 worldPosition, bool switchSkybox, bool useEnvLighting, bool useEnvReflection, bool useMixedLighting, bool useFog, bool useHalo, bool showDebugLines)
            //{
            //    volumes = GetListOfBlendVolumes();

            //    foreach (var v in volumes)
            //    {
            //        Vector3 blendClosestPoint = v.blendCol.ClosestPoint(worldPosition);
            //        if (showDebugLines) Debug.DrawLine(worldPosition, blendClosestPoint, Color.red);

            //        if ((blendClosestPoint - worldPosition).magnitude > 0)
            //        {
            //            Vector3 volumeClosestPoint = v.boxCol.ClosestPoint(blendClosestPoint);
            //            if (showDebugLines) Debug.DrawLine(blendClosestPoint, volumeClosestPoint, Color.green);
            //        }
            //        else
            //        {
            //            Vector3 volumeClosestPoint = v.boxCol.ClosestPoint(blendClosestPoint);
            //            if (showDebugLines) Debug.DrawLine(worldPosition, volumeClosestPoint, Color.yellow);

            //            blend = Mathf.Clamp01((volumeClosestPoint - worldPosition).magnitude / v.blendDist);
            //            //tempLightingProfile.Lerp(v.profile, blend, switchSkybox, useEnvLighting, useEnvReflection, useMixedLighting, useFog, useHalo);
            //            //tempLightingProfile.Apply();
            //        }
            //    }
            //}

        }

        #endregion

        #region COMMON METHODS

        private bool GetFrameSkip()
        {
            if (frameSkip > 0)
                return Time.frameCount % (frameSkip + 1) == 0;
            else
                return true;
        }

        private void ResetTemporalProfile()
        {
            counter = blend = 0;
            hasToFade = false;
            currentProfile.CopyFromCurrentScene();
            currentProfileName = desireProfile.name;
            desireProfile = null;
            desiredProfileName = "---";
        }

        private void OnDestroy()
        {
            Debug.Log("Settings initial lighting profile");
            initialProfile.ApplyRenderSettings();
        }

        internal void Register(LightingVolume volume)
        {
            Debug.Log("<color=green>ADDING</color> New volume of type: " + (volume.isGlobal ? "GLOBAL" : "BLEND"));
            volumes.Add(volume);

            if (checkVolumes == null)
                checkVolumes = StartCoroutine(CheckVolumes());
        }

        internal void Unregister(LightingVolume volume)
        {
            Debug.Log("<color=red>REMOVING</color> lighting volume of type: " + (volume.isGlobal ? "GLOBAL" : "BLEND"));
            volumes.Remove(volume);

            if (checkVolumes == null)
                checkVolumes = StartCoroutine(CheckVolumes());
        }

        internal IEnumerator CheckVolumes()
        {
            yield return new WaitForEndOfFrame();
            GetCurrentVolume();
            checkVolumes = null;
        }

        private void GetCurrentVolume()
        {
            if (ListOfVolumesIsEmpty()) return;

            foreach (var v in volumes) v.current = false;

            if (!LayerIsInsideBlendVolume())
            {
                if (CheckGlobalVolumes())
                {
                    LightingVolume vol = volumes.Where(l => l.isGlobal).OrderBy(l => l.priority).Last();
                    vol.current = true;
                }
            }
            else
            {
                Debug.LogWarning("There is at least 1 blend volume in the scene and we're inside.");
            }
        }

        private bool CheckGlobalVolumes()
        {
            return volumes.Where(l => l.isGlobal).Count() > 0;
        }

        private bool LayerIsInsideBlendVolume()
        {
            foreach (var v in volumes)
            {
                if (v.isGlobal) continue;

                Vector3 pos = transform.position;
                Vector3 blendClosestPoint = v.blendCol.ClosestPoint(pos);

                if ((blendClosestPoint - pos).magnitude == 0) return true;
            }

            return false;
        }

        /// <summary>
        /// Return a warning if there are no lighting volumes in the scene
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

        #endregion
    }
}
