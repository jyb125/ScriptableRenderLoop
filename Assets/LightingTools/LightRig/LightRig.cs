using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LightingTools;

public class LightRig : MonoBehaviour
{
    [SerializeField]
    public List<LightTarget> lightTargetList;
    [SerializeField]
    public List<LightTargetParameters> lightTargetParametersList;
    [SerializeField]
    public bool initialized = false;
    [SerializeField]
    public int focus = -1;

    [ExecuteInEditMode]
	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    public void AddTargetedLight()
    {
		var newLightTarget = gameObject.AddComponent<LightTarget>();
        newLightTarget.hideFlags = HideFlags.HideInInspector;
        var newLightTargetParameters = new LightTargetParameters();
        newLightTarget.lightTargetParameters = newLightTargetParameters;
        lightTargetList.Add(newLightTarget);
        lightTargetParametersList.Add(newLightTargetParameters);
    }
    
	public void RemoveTargetedLight()
	{
        var lastindex = lightTargetList.Count - 1 ;
        if (lastindex == -1) return;
        DestroyImmediate(lightTargetList[lastindex]);
        lightTargetList.RemoveAt (lastindex);
        lightTargetParametersList.RemoveAt(lastindex);
    }

    public void InitializeList()
    {
        if (!initialized)
        {
            lightTargetList = new List<LightTarget>();
            lightTargetParametersList = new List<LightTargetParameters>();
            initialized=true;
        }
    }

    public void FillLists()
    {
        initialized = false;
        InitializeList();
        lightTargetList = gameObject.GetComponents<LightTarget>().ToList();
        foreach (LightTarget target in lightTargetList)
        {
            lightTargetParametersList.Add(target.lightTargetParameters);
        }
    }

    public LightRigPreset SaveLightRigProperties()
    {
        var newPreset = ScriptableObject.CreateInstance<LightRigPreset>();
        newPreset.lightTargetParametersList = new List<LightTargetParameters>();
        //newPreset.lightTargetParametersList = 
        var temp = new LightTargetParameters[lightTargetParametersList.Count];
        lightTargetParametersList.CopyTo(temp);
        newPreset.lightTargetParametersList = temp.ToList();
        return newPreset;
    }
}
