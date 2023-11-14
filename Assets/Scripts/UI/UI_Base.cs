using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, Transform[]> _objectsTransforms = new Dictionary<Type, Transform[]>();
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    protected abstract void Init();
    
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        BindTransform<T>(type);
        
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.LogWarning($"Failed to bind({names[i]})");
        }
    }

    private void BindTransform<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        Transform[] objects = new Transform[names.Length];
        _objectsTransforms.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            Transform tr = Util.FindChildTransformRecursive(gameObject.transform, names[i]);
            if (tr == null)
                Debug.LogWarning($"Failed to bind({names[i]}'s transform)");
            else
                objects[i] = tr;
        }
        
        //목록에 있는 아이템은 모두 SetActive(true)를 해줌으로써 GetComponent Bind 성공을 강제한다.
        foreach (var value in _objectsTransforms[typeof(T)])
            if (value != null)
                value.gameObject.SetActive(true);
        
    }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;
        
        return objects[idx] as T;
    }
    
    protected T GetG<T>(int idx) where T : UnityEngine.Object { return Get<GameObject>(idx).GetComponent<T>(); }
    protected GameObject GetG(int idx) { return Get<GameObject>(idx); }
    protected Text GetT(int idx) { return Get<Text>(idx); }
    protected Button GetB(int idx) { return Get<Button>(idx); }
    protected Image GetI(int idx) { return Get<Image>(idx); }
    protected LayerViewBase GetL(int idx) { return Get<LayerViewBase>(idx); }
    
    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }
    }
}
