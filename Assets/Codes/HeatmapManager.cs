using UnityEngine;
using UnityEngine.Events;

public class HeatmapManager : MonoBehaviour
{
    [SerializeField] private GameObject heatmapSphere;
    [SerializeField] private UnityEvent refreshHeatmap;
    
    public void ShowHeatmap()
    {
        heatmapSphere.SetActive(true);
        refreshHeatmap.Invoke();
    }
}
