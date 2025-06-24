using UnityEngine;

public class InteractableCandidate : MonoBehaviour
{
    [HideInInspector] public Camera CurrentCamera;

    private bool _isInteracting;

    public bool IsInteracting
    {
        get => _isInteracting;
        set => _isInteracting = value;
    }
}
