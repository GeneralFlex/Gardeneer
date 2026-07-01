using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ConveyorBeltGroup
{
    public int speed;
    [HideInInspector]public UnityEvent<int> newFrame = new UnityEvent<int>();
    private int animationIndex;

    public IEnumerator Animate()
    {
        while (true)
        {
            animationIndex = (animationIndex + 1) % 4;
            newFrame.Invoke(animationIndex);
            yield return new WaitForSeconds(1f / speed);
        }
    }
}

public class ConveyorBeltManager : MonoBehaviour
{
    public static ConveyorBeltManager Instance { get; private set; }
    public List<ConveyorBeltGroup> groups;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        foreach (ConveyorBeltGroup group in groups)
        {
            StartCoroutine(group.Animate());
        }
    }
}
