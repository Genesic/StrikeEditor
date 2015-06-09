
using UnityEditor;

/// <summary>
/// 為了方便使用'target'的基底類別。
/// Edited by Fred. 2012.11.26
/// 參考: http://www.altdevblogaday.com/2011/08/22/extending-the-unity3d-editor/
/// </summary>
/// <typeparam name="T"></typeparam>
public class GD_EditorBase<T> : Editor where T : UnityEngine.Object
{
    protected T Target { get { return (T)target; } }
}

