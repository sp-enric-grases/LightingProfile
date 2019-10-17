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
        public int blendFrameSkip = 0;
        public int volumesCheckFrameSkip = 0;
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
        private LightingVolume volA = new LightingVolume();
        private LightingVolume volB = new LightingVolume();

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
            //CheckPosition();
            BlendLightingSettings();
            //FadeLocalVolume();
            //LightingBlendingManager.instance.UpdateLightingSettings(transform.position, switchSkybox, useEnvLighting, useEnvReflection, useMixedLighting, useFog, useHalo, showDebugLines);

            //if (GetCheckVolumesFrame())
            //    GetCurrentVolumes();
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

        private void BlendLightingSettings()
        {
            if (!hasToFade) return;

            counter += Time.deltaTime / blendTime;

            if (GetBlendFrame())
            {
                blend = curve.Evaluate(counter);
                volB.blend = blend * 100;
                if (volA != null) volA.blend = 100 - volB.blend;
                temporalProfile.Lerp(currentProfile, desireProfile, blend, switchSkybox, useEnvLighting, useEnvReflection, useMixedLighting, useFog, useHalo);
                temporalProfile.ApplyRenderSettings();

                if (counter > 1)
                    ResetTemporalProfile();
            }
        }

        //public void GetHigherGlobalProfile()
        //{
        //    if (ListOfVolumesIsEmpty()) return;

        //    List<LightingVolume> globalVolumes = new List<LightingVolume>();

        //    for (int i = 0; i < volumes.Count; i++)
        //    {
        //        if (volumes[i].isGlobal && volumes[i].profile != null)
        //            globalVolumes.Add(volumes[i]);
        //    }

        //    if (globalVolumes.Count > 0)
        //    {
        //        LightingVolume v = globalVolumes.OrderBy(p => p.priority).Last();

        //        desireProfile = v.profile;
        //        desiredProfileName = v.profile.name;
        //        blendTime = v.timeToBlend;
        //        curve = v.timeCurve;
        //        hasToFade = true;
        //    }
        //    else
        //    {
        //        Debug.LogWarning("No global volumes found in the scene.");
        //        hasToFade = false;
        //    }
        //}

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
        }

        #endregion

        #region COMMON METHODS

        private bool GetBlendFrame()
        {
            if (blendFrameSkip > 0)
                return Time.frameCount % (blendFrameSkip + 1) == 0;
            else
                return true;
        }

        private bool GetCheckVolumesFrame()
        {
            if (volumesCheckFrameSkip > 0)
                return Time.frameCount % (volumesCheckFrameSkip + 1) == 0;
            else
                return true;
        }

        private void ResetTemporalProfile()
        {
            counter = blend = 0;
            if (volA != null) volA.blend = 0;
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
            GetCurrentVolumes();
            checkVolumes = null;
        }

        private void GetCurrentVolumes()
        {
            if (ListOfVolumesIsEmpty()) return;

            if (!LayerIsInsideBlendVolume())
            {
                if (CheckGlobalVolumes())
                {
                    LightingVolume temp = volumes.Where(l => l.isGlobal).OrderBy(l => l.priority).Last();

                    if (volB != temp)
                    {
                        if (volB == null)
                            SetLightingProfiles(null, temp);
                        else
                            SetLightingProfiles(volB, temp);

                        desireProfile = volB.profile;
                        desiredProfileName = volB.profile.name;
                        blendTime = volB.timeToBlend;
                        curve = volB.timeCurve;
                        hasToFade = true;
                    }
                }
            }
            else
            {
                List<LightingVolume> blendVolumes = volumes.Where(l => !l.isGlobal).ToList();
                //List<LightingVolume> tempBlendVols = new List<LightingVolume>();

                if (blendVolumes.Count == 0) return;

                foreach (var v in blendVolumes.ToList())
                {
                    Vector3 pos = transform.position;
                    Vector3 blendClosestPoint = v.blendCol.ClosestPoint(pos);
                    //Vector3 boxClosestPoint = v.boxCol.ClosestPoint(pos);
                    float blendMagnitude = (blendClosestPoint - pos).magnitude;
                    //float boxMagnitude = (boxClosestPoint - pos).magnitude;

                    if (blendMagnitude > 0)
                        blendVolumes.Remove(v);

                    //if (blendMagnitude == 0 && boxMagnitude > 0)
                    //    tempBlendVols.Add(v);
                }

                volA = blendVolumes.OrderBy(l => l.priority).Last();
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
                Vector3 boxClosestPoint = v.boxCol.ClosestPoint(pos);

                if ((boxClosestPoint - pos).magnitude == 0) return true;
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

        private void SetLightingProfiles(LightingVolume volA, LightingVolume volB)
        {
            this.volA = volA;
            this.volB = volB;
            this.volB.blend = 0;
        }
        #endregion
    }
}
