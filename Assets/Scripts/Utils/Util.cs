using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
  public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
		if (component == null)
            component = go.AddComponent<T>();
        return component;
	}

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
		}
        else
        {
//            return FindChildTransformRecursive(go.transform, name).GetComponent<T>();
            
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static Transform FindChildTransformRecursive(Transform tr, string name)
    {
        if (tr.childCount == 0)
            return null;
        
        Transform result = null;
        for (int i = 0; i < tr.childCount; i++)
        {
            Transform transform = tr.GetChild(i);
            if (transform.name == name)
            {
                return transform;
            }

            result = FindChildTransformRecursive(transform, name);
        }

        return result;
    }
    
    public static T FindParent<T>(GameObject go, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            T component = go.transform.GetComponentInParent<T>();
            if (component != null)
                return component;
        }
        else
        {
            //bool noParent = false;
            T targetComponent = null;
            GameObject parentGo = null;
            while (targetComponent == null)
            {
                parentGo = go.transform.GetComponentInParent<GameObject>();
                if (parentGo != null)
                {
                    targetComponent = parentGo.GetComponent<T>();
                }
                else
                {
                    break;
                }
            }
        }

        return null;
    }

    public static void StrechRectTransformToFullScreen(ref RectTransform _mRect)
    {
        _mRect.anchoredPosition3D = Vector3.zero;
        _mRect.anchorMin = Vector2.zero;
        _mRect.anchorMax = Vector2.one;
        _mRect.pivot = new Vector2(0.5f, 0.5f);
        _mRect.sizeDelta  =Vector2.zero;

    }
    
    public static void StrechRt_Text(ref RectTransform _mRect)
    {
        _mRect.anchoredPosition3D = Vector3.zero;
        _mRect.anchorMin = Vector2.zero;
        _mRect.anchorMax = Vector2.one;
        _mRect.pivot = new Vector2(0.5f, 0.5f);
        _mRect.sizeDelta  =Vector2.zero;
        _mRect.offsetMax = new Vector2(-12, -12);
        _mRect.offsetMin = new Vector2(12, 12);

    }
}
