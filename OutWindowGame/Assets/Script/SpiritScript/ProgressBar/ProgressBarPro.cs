﻿using UnityEngine;
using System.Collections;
using PlayfulSystems.ProgressBar;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ProgressBarPro : MonoBehaviour {

    public enum AnimationType { FixedTimeForChange, ChangeSpeed }

    [SerializeField] [Range(0f,1f)]
	private float m_value = 1f;
    private float displayValue = -1f;

    [Space(10)]
    [Tooltip("Smoothes out the animation of the bar.")]
    [SerializeField] bool animateBar = true;
    [SerializeField] AnimationType animationType = AnimationType.FixedTimeForChange;
    [SerializeField] float animTime = .25f;
    
    [Space(10)]
    [SerializeField] ProgressBarProView[] views;

    private Coroutine sizeAnim;
    //判断Value值+-
    private bool Shown = false;
    //判断是否启动Value值变化 不启动变成1后停止
    public bool Update = true;

    public void Start() {
        if (views == null || views.Length == 0)
            views = GetComponentsInChildren<ProgressBarProView>();
        m_value = 0f;
        SetDisplayValue(m_value, true);
    }

    void FixedUpdate()
    {
        Move();
    }
    void OnEnable() {
        SetDisplayValue(m_value, true);
    }
    //循环变大小
    void Move()
    {
        if (Math.Round(m_value, 2) >= 1.00F)
        {
            Shown = true;
        }
        else if (m_value < 0f)
        {
            Shown = false;
        }
        if (Shown && Update)
            m_value -= 0.01F;
        else if (!Shown)
            m_value += 0.01F;
        SetDisplayValue(m_value, true);
    }
    // Public Methods 

    public float Value {
        get {
            return m_value;
        }
        set {
            if (value == m_value)
                return;
            SetValue(value);
        }
    }

    public void SetValue(float value, float maxValue) {
        if (maxValue != 0f)
            SetValue(value / maxValue);
        else
            SetValue(0f);
    }

    public void SetValue(int value, int maxValue) {
        if (maxValue != 0)
            SetValue((float)value / (float)maxValue);
        else
            SetValue(0f);
    }

    public void SetValue(float percentage) {
        if (Mathf.Approximately(m_value, percentage))
            return;

        m_value = Mathf.Clamp01(percentage);

        for (int i = 0; i < views.Length; i++) 
            views[i].NewChangeStarted(displayValue, m_value);

        if (animateBar && Application.isPlaying && gameObject.activeInHierarchy) 
            StartSizeAnim(percentage);
        else
            SetDisplayValue(percentage);
    }

    public bool IsAnimating() {
        if (animateBar == false)
            return false;

        return displayValue != m_value;
    }

	// COLOR SETTINGS

    public void SetBarColor(Color color) {
        for (int i = 0; i < views.Length; i++) 
            views[i].SetBarColor(color);
    }

    // SIZE ANIMATION

    void StartSizeAnim(float percentage) {
        if (sizeAnim != null)
            StopCoroutine(sizeAnim);

        sizeAnim = StartCoroutine(DoBarSizeAnim());
    }

    IEnumerator DoBarSizeAnim() {
        float startValue = displayValue;
        float time = 0f;
        float change = m_value - displayValue;
        float duration = (animationType == AnimationType.FixedTimeForChange ? animTime : Mathf.Abs(change) / animTime);

        while (time < duration) {
            time += Time.deltaTime;
            SetDisplayValue(Utils.EaseSinInOut(time/duration, startValue, change));
            yield return null;
        }

        SetDisplayValue(m_value);
        sizeAnim = null;
    }

    // Set Value & Update Views

	void SetDisplayValue(float value, bool forceUpdate = false) {
        // If the value hasn't changed don't update any views.
        if (!forceUpdate && displayValue >= 0f && Mathf.Approximately(displayValue, value))
            return;

        displayValue = value;
		UpdateBarViews(displayValue, m_value, forceUpdate);
	}

	void UpdateBarViews(float currentValue, float targetValue, bool forceUpdate = false) {
        if (views != null)
            for (int i = 0; i < views.Length; i++)
                if (views[i] != null)
                    if (forceUpdate || views[i].CanUpdateView(currentValue, targetValue))
				        views[i].UpdateView(currentValue, targetValue);
	}
    
    // Update Bar in editor

#if UNITY_EDITOR
    private void OnValidate() {
        m_value = Mathf.Clamp01(m_value);

        // This is to also display shadows in editor
        if (m_value >= 1f)
            UpdateBarViews(m_value, 0.75f);
        else
            UpdateBarViews(m_value, m_value + (1-m_value)/ 2f);
    }

    private void Reset() {
        DetectViewObjects();
    }

	public void AddView(ProgressBarProView view) {
        DetectViewObjects();
	}

    public void DetectViewObjects() {
        views = GetComponentsInChildren<ProgressBarProView>(true);
    }
#endif
}
