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
        private List<LightingVolume> insideBlendVls;
        private LightingProfile initialProfile;
        private LightingProfile temporalProfile;
        private LightingProfile currentProfile;
        private LightingProfile desireProfile;

        private float blendTime = 0;
        private AnimationCurve curve;
        private float counter = 0;
        private bool hasToFade = false;
        private bool isInBlend = false;
        private Coroutine checkVolumes;
        private LightingVolume volA = new LightingVolume();
        private LightingVolume volB = new LightingVolume();
        private LightingVolume currentBlendVolume = new LightingVolume();
        private LightingVolume currentVolume = new LightingVolume();

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
            if (GetBlendFrame())
            {
                if (isInBlend && currentProfile != null && desireProfile != null)
                {
                    temporalProfile.Lerp(currentProfile, desireProfile, GetBlendValue(), switchSkybox, useEnvLighting, useEnvReflection, useMixedLighting, useFog, useHalo);
                    temporalProfile.ApplyRenderSettings();
                }

                BlendLightingSettings();
            }

            if (GetCheckVolumesFrame())
            {
                GetCurrentVolumes();
                DebugPosition();
            }
        }

        private void BlendLightingSettings()
        {
            if (!hasToFade || isInBlend) return;

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

            if (LayerIsInsideBlendArea())
            {
                isInBlend = true;
                if (LayerIsInsideVolume())
                {
                    string blend = currentBlendVolume.name;
                    string inside = currentVolume.name;
                    //Debug.Log(string.Format("Layer is in a <color=yellow>BLEND AREA</color> between <color=#FF9000>{0}</color> and <color=#FF9000>{1}</color>.", blend, inside));

                    if (currentBlendVolume.priority > currentVolume.priority)
                    {
                        SetLightingProfiles(currentBlendVolume, currentVolume);
                        currentProfile = volA.profile;
                        desireProfile = volB.profile;
                        volB.blend = GetBlendValue() * 100;
                        if (volA != null) volA.blend = 100 - volB.blend;

                        //Debug.Log("Blend position: " + GetBlendValue());
                    }
                    //else
                    //{
                    //    Debug.Log(string.Format("but nothing changes because <color=#FF9000>{0}</color> has more priority than <color=#FF9000>{1}</color>", inside, blend));
                    //}
                }
                else
                {
                    SetLightingProfiles(GetHighestCurrentBlendVolume(), GetHighestCurrentGlobalVolume());
                    currentProfile = volA.profile;
                    desireProfile = volB.profile;
                    volB.blend = GetBlendValue() * 100;
                    if (volA != null) volA.blend = 100 - volB.blend;
                    //Debug.Log(string.Format("Layer is in a <color=yellow>BLEND AREA</color> between <color=#FF9000>{0}</color> and <color=#FF9000>{1}</color>.", volA.name, volB.name));
                    //Debug.Log("Blend position: " + GetBlendValue());
                }
            }
            //else if (LayerIsInsideVolume())
            //{
            //    isInBlend = true;
            //    Debug.Log(string.Format("Layer is <color=cyan>INSIDE</color> of the <color=#FF9000>{0}</color> blend volume.", GetHighestCurrentBlendVolume().name));
            //}
            else
            {
                isInBlend = false;

                if (volB != GetHighestCurrentGlobalVolume())
                {
                    if (volB == null)
                        SetLightingProfiles(null, GetHighestCurrentGlobalVolume());
                    else
                        SetLightingProfiles(volB, GetHighestCurrentGlobalVolume());

                    desireProfile = volB.profile;
                    desiredProfileName = volB.profile.name;
                    blendTime = volB.timeToBlend;
                    curve = volB.timeCurve;
                    hasToFade = true;
                }

                //Debug.Log(string.Format("Layer is <color=green>OUTSIDE</color> of any blend volume. Using <color=#FF9000>{0}</color> as a profile.", GetHighestCurrentGlobalVolume().name));
            }
        }

        private bool CheckGlobalVolumes()
        {
            return volumes.Where(l => l.isGlobal).Count() > 0;
        }

        private bool LayerIsInsideVolume()
        {
            foreach (var vol in volumes.Where(v => !v.isGlobal))
            {
                Vector3 pos = transform.position;
                Vector3 innerClosestPoint = vol.innerCol.ClosestPoint(pos);

                if ((innerClosestPoint - pos).magnitude == 0)
                {
                    currentVolume = vol;
                    return true;
                }
            }

            return false;
        }

        private bool LayerIsInsideBlendArea()
        {
            insideBlendVls = new List<LightingVolume>();

            foreach (var vol in volumes.Where(v => !v.isGlobal))
            {
                Vector3 pos = transform.position;
                Vector3 outherClosestPoint = vol.outherCol.ClosestPoint(pos);
                Vector3 innerClosestPoint = vol.innerCol.ClosestPoint(pos);
                float outherMagnitude = (outherClosestPoint - pos).magnitude;
                float innerMagnitude = (innerClosestPoint - pos).magnitude;

                if (outherMagnitude == 0 && innerMagnitude > 0)
                    insideBlendVls.Add(vol);
            }

            if (insideBlendVls.Count > 0)
                currentBlendVolume = insideBlendVls.OrderBy(v => v.priority).Last();

            return insideBlendVls.Count > 0 ? true : false;
        }

        private float GetBlendValue()
        {
            Vector3 pos = transform.position;
            Vector3 innerClosestPoint = currentBlendVolume.innerCol.ClosestPoint(pos);
            return (innerClosestPoint - pos).magnitude / currentBlendVolume.blendDist;
        }

        private LightingVolume GetHighestCurrentBlendVolume()
        {
            List<LightingVolume> highestVolumes = new List<LightingVolume>();

            foreach (var vol in volumes.Where(v => !v.isGlobal))
            {
                Vector3 pos = transform.position;
                Vector3 outherClosestPoint = vol.outherCol.ClosestPoint(pos);
                float outherMagnitude = (outherClosestPoint - pos).magnitude;

                if (outherMagnitude == 0 )
                    highestVolumes.Add(vol);
            }

            return highestVolumes.OrderBy(v => v.priority).Last();
        }

        private LightingVolume GetHighestCurrentGlobalVolume()
        {
            return volumes.Where(v => v.isGlobal).OrderBy(v => v.priority).Last();
        }

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

        private void DebugPosition()
        {
            if (ListOfVolumesIsEmpty() || !showDebugLines) return;

            foreach (var v in volumes)
            {
                if (v.isGlobal) continue;

                Vector3 pos = transform.position;
                Vector3 blendClosestPoint = v.outherCol.ClosestPoint(pos);

                Debug.DrawLine(pos, blendClosestPoint, Color.red);

                if ((blendClosestPoint - pos).magnitude > 0)
                {
                    Vector3 volumeClosestPoint = v.innerCol.ClosestPoint(blendClosestPoint);
                    Debug.DrawLine(blendClosestPoint, volumeClosestPoint, Color.green);
                }
                else
                {
                    Vector3 volumeClosestPoint = v.outherCol.ClosestPoint(blendClosestPoint);
                    Debug.DrawLine(pos, volumeClosestPoint, Color.yellow);
                }
            }
        }
    }
}
