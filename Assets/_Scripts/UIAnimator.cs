using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Collections;

[System.Serializable]
public class TransformData
{
    public Vector2 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformData(Vector2 position, Quaternion rotation, Vector3 scale)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }
}

[System.Serializable]
public class AnimationEntry
{
    public bool active = false;
    public List<GameObject> objects;
    public Dictionary<GameObject, TransformData> original = new Dictionary<GameObject, TransformData>();
    
    public Vector2 targetPosition = new Vector2(0,0.1f);
    public Vector3 targetRotation = new Vector3(0,0,15f);
    public float targetScale = 1.1f;

    public Ease ease;

    public float duration = 0.15f;
}

public class UIAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler

{
    [Header("Hover")]
    public List<AnimationEntry> hoverAnimations = new List<AnimationEntry>();

    [Header("Press")]
    public List<AnimationEntry> pressAnimations = new List<AnimationEntry>();

    [Header("Toggle")]
    public List<AnimationEntry> toggleAnimations = new List<AnimationEntry>();

    public IEnumerator Start()
    {
        yield return null;

        foreach(AnimationEntry entry in hoverAnimations)
        {
            foreach(GameObject go in entry.objects)
            {
                RectTransform rect = go.GetComponent<RectTransform>();
                entry.original.Add(go, new TransformData(rect.anchoredPosition, rect.localRotation, rect.localScale));
            }
        }
        foreach (AnimationEntry entry in pressAnimations)
        {
            foreach (GameObject go in entry.objects)
            {
                RectTransform rect = go.GetComponent<RectTransform>();
                entry.original.Add(go, new TransformData(rect.anchoredPosition, rect.localRotation, rect.localScale));
            }
        }
        foreach (AnimationEntry entry in toggleAnimations)
        {
            foreach (GameObject go in entry.objects)
            {
                RectTransform rect = go.GetComponent<RectTransform>();
                entry.original.Add(go, new TransformData(rect.anchoredPosition, rect.localRotation, rect.localScale));
            }
        }
    }

    public void Toggle(GameObject firstGO)
    {
        foreach(AnimationEntry entry in toggleAnimations){
            if (entry.objects[0] == firstGO)
            {
                if(!entry.active)
                    SetEntry(entry);
                else 
                    ResetEntry(entry);
                continue;
            }
            if (entry.active)
            {
                ResetEntry(entry);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        foreach (var entry in hoverAnimations)
            SetEntry(entry);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        foreach (var entry in hoverAnimations)
            ResetEntry(entry);
    }

    public void SetEntry(AnimationEntry entry)
    {
        entry.active = true;
        foreach (GameObject go in entry.objects)
        {
            if (go != null && entry.original.ContainsKey(go))
            {
                RectTransform rect = go.GetComponent<RectTransform>(); 
                rect.DOKill();
                rect.DOAnchorPos(entry.original[go].position + entry.targetPosition, entry.duration).SetEase(entry.ease);
                rect.DORotateQuaternion(Quaternion.Euler(entry.original[go].rotation.eulerAngles + entry.targetRotation), entry.duration).SetEase(entry.ease);
                rect.DOScale(entry.original[go].scale*entry.targetScale, entry.duration).SetEase(entry.ease);
            }
        }
    }

    public void ResetEntry(AnimationEntry entry)
    {
        foreach (GameObject go in entry.objects)
        {
            if (go != null && entry.original.ContainsKey(go))
            {
                RectTransform rect = go.GetComponent<RectTransform>();
                rect.DOKill();
                rect.DOAnchorPos(entry.original[go].position, entry.duration).SetEase(entry.ease);
                rect.DORotateQuaternion(Quaternion.Euler(entry.original[go].rotation.eulerAngles), entry.duration).SetEase(entry.ease);
                rect.DOScale(entry.original[go].scale, entry.duration).SetEase(entry.ease);
            }
        }
        entry.active = false;
    }
}
