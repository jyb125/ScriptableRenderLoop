using UnityEngine;

[ExecuteInEditMode]
public class AutoRefreshGameView : MonoBehaviour
{
    public bool autoRepaint = true;
    public double frameRate = 30;

    double m_NextForceRepaintTime = 0;

#if UNITY_EDITOR
    void OnEnable()
    {
        UnityEditor.EditorApplication.update += ForceRepaint;
    }

    void OnDisable()
    {
        UnityEditor.EditorApplication.update -= ForceRepaint;
    }

    void ForceRepaint()
    {
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            var time = UnityEditor.EditorApplication.timeSinceStartup;

            if (time > m_NextForceRepaintTime)
            {
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                m_NextForceRepaintTime = time + (1.0 / frameRate);
            }
        }
    }
#endif
}
